namespace DXProjectName.Domain.Utilities;
public static class UnicodeExtensions
{
    public static string RemoveUnicodeCharacters(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;
        // Normalize the string to decompose characters
        var normalized = input.Normalize(System.Text.NormalizationForm.FormD);
        // Remove non-ASCII characters
        var ascii = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^\u0000-\u007F]+", string.Empty);
        return ascii;
    }

    // remove Vietnamese characters and replace with their ASCII equivalents
    public static string RemoveVietnameseCharacters(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var replacements = new Dictionary<string, string>
        {
            // Chữ thường - lowercase a
            { "á", "a" }, { "à", "a" }, { "ả", "a" }, { "ã", "a" }, { "ạ", "a" },
            { "ă", "a" }, { "ắ", "a" }, { "ằ", "a" }, { "ẳ", "a" }, { "ẵ", "a" }, { "ặ", "a" },
            { "â", "a" }, { "ấ", "a" }, { "ầ", "a" }, { "ẩ", "a" }, { "ẫ", "a" }, { "ậ", "a" },

            // Chữ hoa - uppercase A
            { "Á", "A" }, { "À", "A" }, { "Ả", "A" }, { "Ã", "A" }, { "Ạ", "A" },
            { "Ă", "A" }, { "Ắ", "A" }, { "Ằ", "A" }, { "Ẳ", "A" }, { "Ẵ", "A" }, { "Ặ", "A" },
            { "Â", "A" }, { "Ấ", "A" }, { "Ầ", "A" }, { "Ẩ", "A" }, { "Ẫ", "A" }, { "Ậ", "A" },

            // Chữ thường - lowercase e
            { "é", "e" }, { "è", "e" }, { "ẻ", "e" }, { "ẽ", "e" }, { "ẹ", "e" },
            { "ê", "e" }, { "ế", "e" }, { "ề", "e" }, { "ể", "e" }, { "ễ", "e" }, { "ệ", "e" },

            // Chữ hoa - uppercase E
            { "É", "E" }, { "È", "E" }, { "Ẻ", "E" }, { "Ẽ", "E" }, { "Ẹ", "E" },
            { "Ê", "E" }, { "Ế", "E" }, { "Ề", "E" }, { "Ể", "E" }, { "Ễ", "E" }, { "Ệ", "E" },

            // Chữ thường - lowercase i
            { "í", "i" }, { "ì", "i" }, { "ỉ", "i" }, { "ĩ", "i" }, { "ị", "i" },

            // Chữ hoa - uppercase I
            { "Í", "I" }, { "Ì", "I" }, { "Ỉ", "I" }, { "Ĩ", "I" }, { "Ị", "I" },

            // Chữ thường - lowercase o
            { "ó", "o" }, { "ò", "o" }, { "ỏ", "o" }, { "õ", "o" }, { "ọ", "o" },
            { "ô", "o" }, { "ố", "o" }, { "ồ", "o" }, { "ổ", "o" }, { "ỗ", "o" }, { "ộ", "o" },
            { "ơ", "o" }, { "ớ", "o" }, { "ờ", "o" }, { "ở", "o" }, { "ỡ", "o" }, { "ợ", "o" },

            // Chữ hoa - uppercase O
            { "Ó", "O" }, { "Ò", "O" }, { "Ỏ", "O" }, { "Õ", "O" }, { "Ọ", "O" },
            { "Ô", "O" }, { "Ố", "O" }, { "Ồ", "O" }, { "Ổ", "O" }, { "Ỗ", "O" }, { "Ộ", "O" },
            { "Ơ", "O" }, { "Ớ", "O" }, { "Ờ", "O" }, { "Ở", "O" }, { "Ỡ", "O" }, { "Ợ", "O" },

            // Chữ thường - lowercase u
            { "ú", "u" }, { "ù", "u" }, { "ủ", "u" }, { "ũ", "u" }, { "ụ", "u" },
            { "ư", "u" }, { "ứ", "u" }, { "ừ", "u" }, { "ử", "u" }, { "ữ", "u" }, { "ự", "u" },

            // Chữ hoa - uppercase U
            { "Ú", "U" }, { "Ù", "U" }, { "Ủ", "U" }, { "Ũ", "U" }, { "Ụ", "U" },
            { "Ư", "U" }, { "Ứ", "U" }, { "Ừ", "U" }, { "Ử", "U" }, { "Ữ", "U" }, { "Ự", "U" },

            // Chữ thường - lowercase y
            { "ý", "y" }, { "ỳ", "y" }, { "ỷ", "y" }, { "ỹ", "y" }, { "ỵ", "y" },

            // Chữ hoa - uppercase Y
            { "Ý", "Y" }, { "Ỳ", "Y" }, { "Ỷ", "Y" }, { "Ỹ", "Y" }, { "Ỵ", "Y" },

            // Chữ đ/Đ - d/D
            { "đ", "d" }, { "Đ", "D" },
        };

        foreach (var kvp in replacements)
        {
            input = input.Replace(kvp.Key, kvp.Value);
        }
        return input;
    }

    /// <summary>
    /// Tạo slug từ chuỗi tiếng Việt
    /// </summary>
    /// <param name="input">Chuỗi đầu vào</param>
    /// <param name="separator">Ký tự phân cách (mặc định là dấu gạch ngang)</param>
    /// <returns>Slug đã được chuẩn hóa</returns>
    public static string ToSlug(this string input, string separator = "-")
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Chuyển về chữ thường
        var slug = input.ToLowerInvariant();

        // Loại bỏ dấu tiếng Việt
        slug = slug.RemoveVietnameseCharacters();

        // Loại bỏ các ký tự đặc biệt, chỉ giữ lại chữ cái, số và khoảng trắng
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Thay thế nhiều khoảng trắng liên tiếp bằng một khoảng trắng
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();

        // Thay thế khoảng trắng bằng separator
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s", separator);

        // Loại bỏ separator trùng lặp
        slug = System.Text.RegularExpressions.Regex.Replace(slug, $@"{System.Text.RegularExpressions.Regex.Escape(separator)}+", separator);

        // Loại bỏ separator ở đầu và cuối
        slug = slug.Trim(separator.ToCharArray());

        return slug;
    }
}