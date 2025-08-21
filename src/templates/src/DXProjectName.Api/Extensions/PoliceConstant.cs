namespace DXProjectName.Api.Extensions;

public class PoliceResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string UserId { get; set; }

    public List<string>? Roles { get; set; }
}

public class PoliceConstant
{
    public static List<PoliceResponseDto> Polices()
    {
        return new List<PoliceResponseDto>
        {
            new PoliceResponseDto
            {
                Name = "service",
                Roles = new List<string> { "admin", "user", "guest", "superadmin", "restaurantadmin" }
            },
            new PoliceResponseDto
            {
                Name = "user",
                Roles = new List<string> { "user", "guest" }
            },

        };
    }
}