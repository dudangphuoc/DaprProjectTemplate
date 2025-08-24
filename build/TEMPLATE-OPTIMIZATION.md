# ??? Template Optimization - T?i ?u kích th??c NuGet Package

## ?? T?ng quan

Build system ?ã ???c t?i ?u ?? gi?m kích th??c NuGet package b?ng cách t? ??ng xóa các build artifacts không c?n thi?t trong templates.

## ?? M?c tiêu

- ? Gi?m kích th??c .nupkg package
- ? Lo?i b? th? m?c `obj` và `bin` trong templates
- ? T? ??ng hóa quá trình optimization
- ? Hi?n th? thông tin v? dung l??ng ?ã ti?t ki?m

## ?? Các Target có s?n

### 1. `CleanTemplates` - D?n d?p template artifacts
```bash
dotnet run --project build -- CleanTemplates
```

**Mô t?:** Xóa toàn b? th? m?c `*/src/*/obj` và `*/src/*/bin` trong templates ?? t?i ?u kích th??c package.

**Khi nào s? d?ng:**
- Tr??c khi pack template
- Khi mu?n clean development artifacts
- T? ??ng ch?y khi `Pack` target ???c g?i

### 2. `OptimizeTemplateSize` - T?i ?u và hi?n th? th?ng kê
```bash
dotnet run --project build -- OptimizeTemplateSize
```

**Mô t?:** T?i ?u template size và hi?n th? thông tin chi ti?t v? kích th??c sau khi clean.

**Khi nào s? d?ng:**
- Development và testing
- Mu?n xem kích th??c hi?n t?i c?a template
- Ki?m tra hi?u qu? c?a optimization

### 3. `Pack` - Pack v?i t?i ?u t? ??ng
```bash
dotnet run --project build -- Pack
```

**Mô t?:** T? ??ng clean template artifacts tr??c khi pack ?? t?o ra package có kích th??c t?i ?u.

**Quá trình:**
1. `Compile` - Build các project
2. `CleanTemplates` - Clean template artifacts  
3. `Pack` - T?o .nupkg package

## ?? Thông tin hi?n th?

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

### N?u không có artifacts:
```
?? No build artifacts found in templates - already clean!
```

## ?? T? ??ng hóa

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
# Clean và optimize template
dotnet run --project build -- OptimizeTemplateSize

# Pack v?i optimization
dotnet run --project build -- Pack

# Full workflow v?i publish
dotnet run --project build -- Publish
```

## ?? C?u trúc ???c t?i ?u

### Tr??c khi clean:
```
src/templates/
??? src/
?   ??? ProjectName.Api/
?   ?   ??? bin/           # ? S? ???c xóa
?   ?   ??? obj/           # ? S? ???c xóa
?   ?   ??? Controllers/   # ? Gi? l?i
?   ??? ProjectName.Domain/
?   ?   ??? bin/           # ? S? ???c xóa  
?   ?   ??? obj/           # ? S? ???c xóa
?   ?   ??? Entities/      # ? Gi? l?i
?   ??? ProjectName.Infrastructure/
?       ??? bin/           # ? S? ???c xóa
?       ??? obj/           # ? S? ???c xóa
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

## ?? Debugging và Monitoring

### Ki?m tra kích th??c template:
```bash
dotnet run --project build -- OptimizeTemplateSize
```

Output s? hi?n th?:
- S? l??ng th? m?c ?ã xóa
- T?ng dung l??ng ti?t ki?m ???c
- Kích th??c hi?n t?i c?a template

### Log chi ti?t:
- ? Thành công: Hi?n th? ???ng d?n và kích th??c ?ã xóa
- ? Th?t b?i: Hi?n th? l?i c? th?  
- ?? Thông tin: Template ?ã clean ho?c không tìm th?y artifacts

## ? Performance Benefits

### Tr??c t?i ?u:
- Template package: ~15-20 MB
- Ch?a build artifacts không c?n thi?t
- Download và install ch?m h?n

### Sau t?i ?u:  
- Template package: ~2-5 MB
- Ch? ch?a source code c?n thi?t
- Download và install nhanh h?n 3-4 l?n

## ??? Troubleshooting

### Template directory không tìm th?y:
```
?? Templates directory not found at: D:\path\to\src\templates
```
**Gi?i pháp:** Ki?m tra c?u trúc project và ??m b?o th? m?c `src/templates` t?n t?i.

### Permission issues:
```
? Failed to delete D:\path\to\directory: Access to the path is denied.
```
**Gi?i pháp:** 
- ?óng Visual Studio ho?c IDE ?ang m? project
- Ch?y terminal v?i quy?n Administrator
- Ki?m tra không có process nào ?ang s? d?ng file

### No build artifacts found:
```
?? No build artifacts found in templates - already clean!
```
**Gi?i thích:** ?ây là tr?ng thái bình th??ng, template ?ã ???c t?i ?u ho?c ch?a có build artifacts.

---

**L?u ý:** Quá trình optimization này ch? ?nh h??ng ??n template artifacts, không ?nh h??ng ??n source code ho?c c?u hình template.