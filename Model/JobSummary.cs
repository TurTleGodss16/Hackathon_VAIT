namespace Hackathon_VAIT_New.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JobSummary
{
    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; }

    [JsonPropertyName("job_title")]
    public string JobTitle { get; set; }

    [JsonPropertyName("main_responsibility")]
    public string MainResponsibility { get; set; }

    [JsonPropertyName("job_location")]
    public string JobLocation { get; set; }

    [JsonPropertyName("tech_stack")]
    public List<string> TechStack { get; set; }

    public static JobSummary FromJson(string json)
    {
        return JsonSerializer.Deserialize<JobSummary>(json);
    }
}