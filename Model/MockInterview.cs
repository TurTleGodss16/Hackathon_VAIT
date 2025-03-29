namespace Hackathon_VAIT_New.Model;

public enum MockInterviewCategory
{
    Technical,
    Behavioral,
    SystemDesign,
    CodingChallenge,
    CaseStudy,
    Any
}

public enum MockInterviewRole
{
    Interviewer,
    Interviewee,
    Both
}

public class TimeSlot
{
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class MockInterviewProfile
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required List<MockInterviewCategory> Categories { get; set; }
    public required MockInterviewRole Role { get; set; } // New field
    public required List<TimeSlot> Availability { get; set; } // New field
    public int? NumberOfInterviews { get; set; }
    public List<string>? PreferredLanguage { get; set; }
    public string? PreferredTimeZone { get; set; }
    public List<string>? RealInterviewsFromCompany { get; set; }

    // add partial class User
    public User? User { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "Id", Id },
            { "UserId", UserId },
            { "Categories", string.Join(",", Categories) },
            { "Role", Role.ToString() },
            { "Availability", string.Join(";", Availability.Select(ts => $"{ts.Day},{ts.StartTime},{ts.EndTime}")) },
            { "NumberOfInterviews", NumberOfInterviews?.ToString() ?? "" },
            { "PreferredLanguage", PreferredLanguage != null ? string.Join(",", PreferredLanguage) : "" },
            { "PreferredTimeZone", PreferredTimeZone ?? "" },
            {
                "RealInterviewsFromCompany",
                RealInterviewsFromCompany != null ? string.Join(",", RealInterviewsFromCompany) : ""
            }
        };
    }

    // Convert from Dictionary<string, object>
    public static MockInterviewProfile FromDictionary(Dictionary<string, object> dict)
    {
        return new MockInterviewProfile
        {
            Id = dict["Id"].ToString()!,
            UserId = dict["UserId"].ToString()!,
            Categories = dict["Categories"].ToString()!.Split(',').Select(Enum.Parse<MockInterviewCategory>)
                .ToList(),
            Role = Enum.Parse<MockInterviewRole>(dict["Role"].ToString()!),
            Availability = dict["Availability"].ToString()!
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(entry =>
                {
                    var parts = entry.Split(',');
                    return new TimeSlot
                    {
                        Day = Enum.Parse<DayOfWeek>(parts[0]),
                        StartTime = TimeSpan.Parse(parts[1]),
                        EndTime = TimeSpan.Parse(parts[2])
                    };
                }).ToList(),
            NumberOfInterviews = int.TryParse(dict["NumberOfInterviews"].ToString(), out var num) ? num : null,
            PreferredLanguage = string.IsNullOrEmpty(dict["PreferredLanguage"].ToString())
                ? null
                : dict["PreferredLanguage"].ToString()!.Split(',').ToList(),
            PreferredTimeZone = string.IsNullOrEmpty(dict["PreferredTimeZone"].ToString())
                ? null
                : dict["PreferredTimeZone"].ToString(),
            RealInterviewsFromCompany = string.IsNullOrEmpty(dict["RealInterviewsFromCompany"].ToString())
                ? null
                : dict["RealInterviewsFromCompany"].ToString()!.Split(',').ToList()
        };
    }

    public static List<MockInterviewProfile> GetMockData()
    {
        return
        [
            new MockInterviewProfile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "02f9d196-489c-4f7b-b08d-0a568605c02b",
                Categories = new List<MockInterviewCategory>
                    { MockInterviewCategory.Technical, MockInterviewCategory.SystemDesign },
                Role = MockInterviewRole.Both,
                Availability = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Day = DayOfWeek.Monday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(20, 0, 0)
                    },
                    new TimeSlot
                    {
                        Day = DayOfWeek.Thursday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(21, 0, 0)
                    }
                },
                NumberOfInterviews = 5,
                PreferredLanguage = new List<string> { "Python", "C++" },
                PreferredTimeZone = "UTC+10.5",
                RealInterviewsFromCompany = new List<string> { "Google", "Amazon" }
            },

            new MockInterviewProfile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "68af5783-2dcf-4767-bc9f-8e271fe514ee",
                Categories = new List<MockInterviewCategory>
                    { MockInterviewCategory.Behavioral, MockInterviewCategory.CaseStudy },
                Role = MockInterviewRole.Interviewee,
                Availability = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Day = DayOfWeek.Tuesday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0)
                    },
                    new TimeSlot
                        { Day = DayOfWeek.Friday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) }
                },
                NumberOfInterviews = 3,
                PreferredLanguage = new List<string> { "JavaScript" },
                PreferredTimeZone = "UTC+8",
                RealInterviewsFromCompany = new List<string> { "Meta", "Microsoft" }
            },

            new MockInterviewProfile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "fe78a696-b2ac-4f53-9232-7b181587fba1",
                Categories = new List<MockInterviewCategory> { MockInterviewCategory.Any },
                Role = MockInterviewRole.Interviewer,
                Availability = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Day = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0)
                    },
                    new TimeSlot
                    {
                        Day = DayOfWeek.Saturday, StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(17, 0, 0)
                    }
                },
                NumberOfInterviews = 10,
                PreferredLanguage = new List<string> { "C#", "Java" },
                PreferredTimeZone = "UTC+11",
                RealInterviewsFromCompany = new List<string> { "Tesla", "Netflix" }
            }
        ];
    }
}