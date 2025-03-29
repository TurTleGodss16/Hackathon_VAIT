namespace Hackathon_VAIT_New.Model;

public enum UserRoleType
{
    Reviewer,
    User,
    Guest,
}

public enum UserEmploymentType
{
    Fulltime,
    Parttime,
    Student,
    Unemployed
}

public class User
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? DiscordId { get; set; }
    public string? CompanyName { get; set; }
    public List<UserRoleType> Roles { get; set; }
    public UserEmploymentType EmploymentStatus { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "Id", Id ?? string.Empty },
            { "Name", Name ?? string.Empty },
            { "Email", Email ?? string.Empty },
            { "ProfilePictureUrl", ProfilePictureUrl ?? string.Empty },
            { "DiscordId", DiscordId ?? string.Empty },
            { "CompanyName", CompanyName ?? string.Empty },
            { "Roles", string.Join(",", Roles ?? new List<UserRoleType>()) },
            { "EmploymentStatus", EmploymentStatus.ToString() },
            { "CreatedAt", CreatedAt.ToString("o") },
            { "UpdatedAt", UpdatedAt.ToString("o") },
            { "Title", Title ?? string.Empty }
        };
    }

    public static User FromDictionary(Dictionary<string, object> dictionary)
    {
        return new User
        {
            Id = dictionary.ContainsKey("Id") ? dictionary["Id"] as string : null,
            Name = dictionary.ContainsKey("Name") ? dictionary["Name"] as string : null,
            Email = dictionary.ContainsKey("Email") ? dictionary["Email"] as string : null,
            ProfilePictureUrl = dictionary.ContainsKey("ProfilePictureUrl")
                ? dictionary["ProfilePictureUrl"] as string
                : null,
            DiscordId = dictionary.ContainsKey("DiscordId") ? dictionary["DiscordId"] as string : null,
            CompanyName = dictionary.ContainsKey("CompanyName") ? dictionary["CompanyName"] as string : null,
            Roles = dictionary.ContainsKey("Roles")
                ? ((string)dictionary["Roles"]).Split(',').Select(role => Enum.Parse<UserRoleType>(role)).ToList()
                : new List<UserRoleType>(),
            EmploymentStatus = dictionary.ContainsKey("EmploymentStatus") &&
                               Enum.TryParse<UserEmploymentType>((string)dictionary["EmploymentStatus"],
                                   out var employmentStatus)
                ? employmentStatus
                : UserEmploymentType.Unemployed,
            CreatedAt = dictionary.ContainsKey("CreatedAt") &&
                        DateTime.TryParse((string)dictionary["CreatedAt"], out var createdAt)
                ? createdAt
                : DateTime.MinValue,
            UpdatedAt = dictionary.ContainsKey("UpdatedAt") &&
                        DateTime.TryParse((string)dictionary["UpdatedAt"], out var updatedAt)
                ? updatedAt
                : DateTime.MinValue,
            Title = dictionary.ContainsKey("Title") ? dictionary["Title"] as string : null
        };
    }
}