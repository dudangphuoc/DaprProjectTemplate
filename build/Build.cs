using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    public static string NuGetApiKey { get; set; }
    public static string NuGetSource { get; set; }
    public static string VersionSuffix { get; set; }
    public static string VersionPrefix { get; set; }
    public static string PackageVersion { get; set; }
  
    static Build()
    {
        // Load environment variables from .env file if it exists
        LoadDotEnvFile().ContinueWith(x => { 
            if (x.IsFaulted)
            {
                Console.WriteLine($"📖 Error loading .env file: {x.Exception?.Message}");
                return;
            }
        }).GetAwaiter().GetResult();
        VersionSuffix = Environment.GetEnvironmentVariable("PACKAGE_VERSION_SUFFIX")
             ?? throw new ArgumentNullException("PACKAGE_VERSION_SUFFIX");
        VersionPrefix = Environment.GetEnvironmentVariable("PACKAGE_VERSION_PREFIX")
            ?? throw new ArgumentNullException("PACKAGE_VERSION_PREFIX");
        PackageVersion = VersionPrefix + "-" + VersionSuffix;
        Console.WriteLine($"📖 Loaded version: {PackageVersion}");
        NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY")
            ?? throw new ArgumentNullException("NUGET_API_KEY");
        NuGetSource = Environment.GetEnvironmentVariable("NUGET_SOURCE")
            ?? "https://baget.hexbox.vn/v3/index.json";
        Console.WriteLine($"📖 NuGet API Key: {new string('*', Math.Min(NuGetApiKey.Length, 20))}...");
    }

    public static int Main() => Execute<Build>(x => x.Compile);

    [Solution] readonly Solution Solution;
    AbsolutePath TestResultDirectory => RootDirectory / "output" / "test-results";
    static AbsolutePath PackagesDirectory => RootDirectory / "output" / "packages";

    /// <summary>
    /// Load environment variables from .env file in the build directory
    /// This allows developers to set NUGET_API_KEY and other variables locally
    /// without exposing them in source control
    /// </summary>
    private static async Task LoadDotEnvFile()
    {
        var envFilePath = Path.Combine(AppContext.BaseDirectory, ".env");
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine($"📖 Looking for .env file at: {envFilePath}");

        if (!File.Exists(envFilePath))
        {
            Console.WriteLine($"📖 .env file not found at: {envFilePath}");
            Console.WriteLine("📖 To use .env file, copy build\\.env.template to build\\.env and fill in your values");
            return;
        }

        try
        {
            Console.WriteLine($"📖 Loading environment variables from: {envFilePath}");
            var lines = File.ReadAllLines(envFilePath);
            var loadedCount = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // Remove quotes if present
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                // Only set if environment variable doesn't already exist
                // This allows system environment variables to override .env file
                if (Environment.GetEnvironmentVariable(key) == null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                    loadedCount++;
                    Console.WriteLine($"📖 Loaded: {key} = {(key.ToUpper().Contains("KEY") || key.ToUpper().Contains("SECRET") ? "***" : value)}");
                }
                else
                {
                    Console.WriteLine($"📖  Skipped {key} (already set in environment)");
                }
            }

            Console.WriteLine($"🎯 Successfully loaded {loadedCount} environment variables from .env file");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"📖 Error loading .env file: {ex.Message}");
        }
    }

    /// <summary>
    /// Tối ưu dung lượng nupkg bằng cách xóa toàn bộ thư mục obj và bin trong templates
    /// Điều này giúp giảm kích thước package khi publish
    /// </summary>
    private static void CleanTemplateArtifacts()
    {
        var templatesPath = RootDirectory / "src" / "templates";
        if (!Directory.Exists(templatesPath))
        {
            Console.WriteLine($"📁 Templates directory not found at: {templatesPath}");
            return;
        }

        Console.WriteLine($"🧹 Cleaning template build artifacts from: {templatesPath}");
        Console.WriteLine($"🎯 Patterns: */src/*/obj, */src/*/bin");

        var objDirectories = templatesPath.GlobDirectories("*/src/*/obj", "**/src/*/obj");
        var binDirectories = templatesPath.GlobDirectories("*/src/*/bin", "**/src/*/bin");
        
        var allDirectories = objDirectories.Concat(binDirectories).ToArray();
        var deletedCount = 0;
        var totalSizeDeleted = 0L;

        foreach (var directory in allDirectories)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    // Tính toán kích thước thư mục trước khi xóa
                    var dirInfo = new DirectoryInfo(directory);
                    var sizeInBytes = GetDirectorySize(dirInfo);
                    totalSizeDeleted += sizeInBytes;

                    directory.DeleteDirectory();
                    deletedCount++;
                    Console.WriteLine($"✅ Deleted: {directory} ({FormatBytes(sizeInBytes)})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to delete {directory}: {ex.Message}");
            }
        }

        Console.WriteLine($"🎉 Cleaned {deletedCount} directories, saved {FormatBytes(totalSizeDeleted)}");
        
        if (deletedCount == 0)
        {
            Console.WriteLine("📝 No build artifacts found in templates - already clean!");
        }
    }

    /// <summary>
    /// Tính toán kích thước của thư mục (bao gồm các file con)
    /// </summary>
    private static long GetDirectorySize(DirectoryInfo dirInfo)
    {
        try
        {
            long size = 0;
            
            // Tính kích thước tất cả files trong thư mục
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    size += file.Length;
                }
                catch
                {
                    // Ignore files that can't be accessed
                }
            }
            
            return size;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Format bytes thành readable format (KB, MB, GB)
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (bytes >= GB)
            return $"{bytes / (double)GB:F2} GB";
        else if (bytes >= MB)
            return $"{bytes / (double)MB:F2} MB";
        else if (bytes >= KB)
            return $"{bytes / (double)KB:F2} KB";
        else
            return $"{bytes} bytes";
    }

    Target Clean => _ => _
    .Before(Restore)
    .Executes(() =>
    {
        // Clean standard build directories
        var directories = RootDirectory
            .GlobDirectories(
                "*/src/*/obj",
                "*/src/*/bin");
        Console.WriteLine($"🧹 Cleaning {directories.Count} standard build directories...");
        directories.ForEach(path => path.DeleteDirectory());
        
        // Clean template artifacts for optimized package size
        Console.WriteLine("🧹 Cleaning template build artifacts...");
        CleanTemplateArtifacts();
        
        // Clean output directory
        var outputPath = RootDirectory / "output";
        if (Directory.Exists(outputPath))
        {
            Console.WriteLine($"🧹 Cleaning output directory: {outputPath}");
            outputPath.DeleteDirectory();
        }
        
        Console.WriteLine("✅ Cleaning completed!");
    });

    Target CleanTemplates => _ => _
        .Description("Tối ưu dung lượng nupkg bằng cách xóa toàn bộ */src/*/obj và */src/*/bin trong templates")
        .Executes(() =>
        {
            CleanTemplateArtifacts();
        });

    Target OptimizeTemplateSize => _ => _
        .Description("Chỉ tối ưu kích thước template mà không pack - hữu ích cho development")
        .Executes(() =>
        {
            Console.WriteLine("🔧 Optimizing template size for development...");
            CleanTemplateArtifacts();
            
            // Hiển thị thông tin về template sau khi clean
            var templatesPath = RootDirectory / "src" / "templates";
            if (Directory.Exists(templatesPath))
            {
                var templateSize = GetDirectorySize(new DirectoryInfo(templatesPath));
                Console.WriteLine($"📊 Current template size: {FormatBytes(templateSize)}");
            }
            
            Console.WriteLine("✨ Template optimization complete!");
        });

    Target Pack => _ => _
      .DependsOn(Compile)
      .DependsOn(CleanTemplates)
      .Produces(PackagesDirectory / "*.nupkg")
      .Executes(() =>
      {
          Console.WriteLine("📦 Packing template with optimized size...");
          
          DotNetPack(_ => _
              .SetConfiguration(Configuration)
              .SetVersionSuffix(VersionSuffix)
              .SetVersionPrefix(VersionPrefix)
              .SetOutputDirectory(PackagesDirectory)
              .SetNoBuild(InvokedTargets.Contains(Compile))
              .SetProperty("SourceLinkCreate", true)
              .CombineWith(
                  Solution.AllProjects.Where(x => x.Name == "DXDaprProjectTemplate"), (_, v) => _
                      .SetProject(v)));
                      
          Console.WriteLine("✅ Package created with optimized size!");
      });

    Target Restore => _ => _
       .Executes(() =>
       {
           DotNetRestore(_ => _
               .SetProjectFile(Solution));
       });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

    Target Publish => _ => _
      .DependsOn(Pack)
      .Executes(() =>
      {
          // Validate API key is provided
          if (string.IsNullOrWhiteSpace(NuGetApiKey))
          {
              throw new InvalidOperationException(
                  "NuGet API Key is required for publishing. " +
                  "Please provide it via:\n" +
                  "1. .env file in build directory (NUGET_API_KEY=your-key)\n" +
                  "2. Environment variable: NUGET_API_KEY\n" +
                  "3. Command line parameter: --nuget-api-key <key>\n" +
                  "4. CI/CD secret variable");
          }
          // Show configuration source for debugging
          var envSource = File.Exists(Path.Combine(AppContext.BaseDirectory, ".env")) ? " (from .env file)" : "";
          DotNetTasks.DotNetNuGetPush(x => x
              .SetApiKey(NuGetApiKey)
              .SetSource(NuGetSource)
              .SetSkipDuplicate(true)
              .SetTargetPath(PackagesDirectory / "*.nupkg"));
      });
}
