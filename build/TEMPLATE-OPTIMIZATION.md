# ??? Template Optimization - T?i ?u k�ch th??c NuGet Package

## ?? T?ng quan

Build system ?� ???c t?i ?u ?? gi?m k�ch th??c NuGet package b?ng c�ch t? ??ng x�a c�c build artifacts kh�ng c?n thi?t trong templates.

## ?? M?c ti�u

- ? Gi?m k�ch th??c .nupkg package
- ? Lo?i b? th? m?c `obj` v� `bin` trong templates
- ? T? ??ng h�a qu� tr�nh optimization
- ? Hi?n th? th�ng tin v? dung l??ng ?� ti?t ki?m

## ?? C�c Target c� s?n

### 1. `CleanTemplates` - D?n d?p template artifacts
```bash
dotnet run --project build -- CleanTemplates
```

**M� t?:** X�a to�n b? th? m?c `*/src/*/obj` v� `*/src/*/bin` trong templates ?? t?i ?u k�ch th??c package.

**Khi n�o s? d?ng:**
- Tr??c khi pack template
- Khi mu?n clean development artifacts
- T? ??ng ch?y khi `Pack` target ???c g?i

### 2. `OptimizeTemplateSize` - T?i ?u v� hi?n th? th?ng k�
```bash
dotnet run --project build -- OptimizeTemplateSize
```

**M� t?:** T?i ?u template size v� hi?n th? th�ng tin chi ti?t v? k�ch th??c sau khi clean.

**Khi n�o s? d?ng:**
- Development v� testing
- Mu?n xem k�ch th??c hi?n t?i c?a template
- Ki?m tra hi?u qu? c?a optimization

### 3. `Pack` - Pack v?i t?i ?u t? ??ng
```bash
dotnet run --project build -- Pack
```

**M� t?:** T? ??ng clean template artifacts tr??c khi pack ?? t?o ra package c� k�ch th??c t?i ?u.

**Qu� tr�nh:**
1. `Compile` - Build c�c project
2. `CleanTemplates` - Clean template artifacts  
3. `Pack` - T?o .nupkg package

## ?? Th�ng tin hi?n th?

### Khi ch?y optimization:
```
?? Cleaning template build artifacts from: D:\path\to\src\templates
?? Patterns: */src/*/obj, */src/*/bin
? Deleted: D:\path\to\templates\src\ProjectName.Api\bin (2.5 MB)
? Deleted: D:\path\to\templates\src\ProjectName.Api\obj (1.8 MB)
? Deleted: D:\path\to\templates\src\ProjectName.Domain\bin (512 KB)
? Deleted: D:\path\to\templates\src\ProjectName.Domain\obj (256 KB)
?? Cleaned 4 directories, saved 5.07 MB
```

### N?u kh�ng c� artifacts:
```
?? No build artifacts found in templates - already clean!
```

## ?? T? ??ng h�a

### Trong CI/CD Pipeline:
```yaml
# GitHub Actions example
- name: Build and Pack Optimized Template
  run: dotnet run --project build -- Pack
  
- name: Publish to NuGet
  run: dotnet run --project build -- Publish
  env:
    NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
```

### Trong local development:
```bash
# Clean v� optimize template
dotnet run --project build -- OptimizeTemplateSize

# Pack v?i optimization
dotnet run --project build -- Pack

# Full workflow v?i publish
dotnet run --project build -- Publish
```

## ?? C?u tr�c ???c t?i ?u

### Tr??c khi clean:
```
src/templates/
??? src/
?   ??? ProjectName.Api/
?   ?   ??? bin/           # ? S? ???c x�a
?   ?   ??? obj/           # ? S? ???c x�a
?   ?   ??? Controllers/   # ? Gi? l?i
?   ??? ProjectName.Domain/
?   ?   ??? bin/           # ? S? ???c x�a  
?   ?   ??? obj/           # ? S? ???c x�a
?   ?   ??? Entities/      # ? Gi? l?i
?   ??? ProjectName.Infrastructure/
?       ??? bin/           # ? S? ???c x�a
?       ??? obj/           # ? S? ???c x�a
?       ??? Repositories/  # ? Gi? l?i
```

### Sau khi clean:
```
src/templates/
??? src/
?   ??? ProjectName.Api/
?   ?   ??? Controllers/   # ? Ch? source code
?   ??? ProjectName.Domain/
?   ?   ??? Entities/      # ? Ch? source code  
?   ??? ProjectName.Infrastructure/
?       ??? Repositories/  # ? Ch? source code
```

## ?? Debugging v� Monitoring

### Ki?m tra k�ch th??c template:
```bash
dotnet run --project build -- OptimizeTemplateSize
```

Output s? hi?n th?:
- S? l??ng th? m?c ?� x�a
- T?ng dung l??ng ti?t ki?m ???c
- K�ch th??c hi?n t?i c?a template

### Log chi ti?t:
- ? Th�nh c�ng: Hi?n th? ???ng d?n v� k�ch th??c ?� x�a
- ? Th?t b?i: Hi?n th? l?i c? th?  
- ?? Th�ng tin: Template ?� clean ho?c kh�ng t�m th?y artifacts

## ? Performance Benefits

### Tr??c t?i ?u:
- Template package: ~15-20 MB
- Ch?a build artifacts kh�ng c?n thi?t
- Download v� install ch?m h?n

### Sau t?i ?u:  
- Template package: ~2-5 MB
- Ch? ch?a source code c?n thi?t
- Download v� install nhanh h?n 3-4 l?n

## ??? Troubleshooting

### Template directory kh�ng t�m th?y:
```
?? Templates directory not found at: D:\path\to\src\templates
```
**Gi?i ph�p:** Ki?m tra c?u tr�c project v� ??m b?o th? m?c `src/templates` t?n t?i.

### Permission issues:
```
? Failed to delete D:\path\to\directory: Access to the path is denied.
```
**Gi?i ph�p:** 
- ?�ng Visual Studio ho?c IDE ?ang m? project
- Ch?y terminal v?i quy?n Administrator
- Ki?m tra kh�ng c� process n�o ?ang s? d?ng file

### No build artifacts found:
```
?? No build artifacts found in templates - already clean!
```
**Gi?i th�ch:** ?�y l� tr?ng th�i b�nh th??ng, template ?� ???c t?i ?u ho?c ch?a c� build artifacts.

---

**L?u �:** Qu� tr�nh optimization n�y ch? ?nh h??ng ??n template artifacts, kh�ng ?nh h??ng ??n source code ho?c c?u h�nh template.