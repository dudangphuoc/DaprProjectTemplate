using AutoMapper;

namespace DXProjectName.Api.Application.AutoMapperProfiles;


/// <summary>
/// Ví dụ về bộ chuyển đổi đường dẫn tập tin trong AutoMapper
/// </summary>
public class FileUrlResolver : IValueConverter<string, string>
{
    /// <summary>
    /// Khởi tạo một đối tượng FileUrlResolver mới
    /// </summary>
    public FileUrlResolver()
    {
    }

    /// <summary>
    /// Chuyển đổi đường dẫn tập tin nguồn thành đường dẫn đích
    /// </summary>
    /// <param name="sourceMember">Đường dẫn tập tin nguồn</param>
    /// <param name="context">Ngữ cảnh của quá trình chuyển đổi</param>
    /// <returns>Đường dẫn tập tin đã được xử lý</returns>
    public string Convert(string sourceMember, ResolutionContext context)
    {
        return ResolveFileUrl("", sourceMember, context);
    }

    /// <summary>
    /// Xử lý và tạo đường dẫn tập tin hoàn chỉnh
    /// </summary>
    /// <param name="baseUri">URI cơ sở</param>
    /// <param name="resourcePath">Đường dẫn tài nguyên</param>
    /// <param name="context">Ngữ cảnh của quá trình chuyển đổi</param>
    /// <returns>
    /// - Chuỗi rỗng nếu đường dẫn tài nguyên trống
    /// - Đường dẫn tài nguyên gốc nếu không cần xử lý
    /// - Đường dẫn hoàn chỉnh sau khi kết hợp baseUri và resourcePath
    /// </returns>
    public static string ResolveFileUrl(string baseUri, string? resourcePath, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            return string.Empty;
        }
        try
        {
            if (!context.Items.TryGetValue("resolveUrl", out var val) || !(val is bool resolveUrl) || !resolveUrl)
            {
                return resourcePath;
            }
        }
        catch (Exception)
        {
            return resourcePath;
        }

        if (string.IsNullOrWhiteSpace(baseUri)
            || resourcePath.StartsWith("http://")
            || resourcePath.StartsWith("https://"))
        {
            return resourcePath;
        }
        else
        {
            return new Uri(new Uri(baseUri), resourcePath).ToString();
        }
    }
}
