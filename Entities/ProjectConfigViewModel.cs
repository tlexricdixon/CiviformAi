using Microsoft.AspNetCore.Http;

namespace Entities;

public class ProjectConfigViewModel
{
    public string ProjectName { get; set; }
    public string? AccessVersion { get; set; }
    public IFormFile? AccessFile { get; set; }
    public string? AccessFileName { get; set; }

    // For database config
    public string? DbType { get; set; } // "SQLServer", "Postgres", etc.
    public string? DbServer { get; set; }
    public string? DatabaseName { get; set; }
    public string? DbUser { get; set; }
    public string? DbPassword { get; set; }
    public bool UseIntegratedSecurity { get; set; } = false;

    public bool EnableLogging { get; set; } = false;
}
