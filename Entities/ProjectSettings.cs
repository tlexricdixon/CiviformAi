using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities;

public class ProjectSettings
{
    public string ProjectName { get; set; }
    public string DbServer { get; set; }
    public string DatabaseName { get; set; }
    public string DbUser { get; set; }
    public string DbPassword { get; set; }
    public string AccessVersion { get; set; }
    public bool UseIntegratedSecurity { get; set; }
    public bool EnableLogging { get; set; }
    public string AccessFileName { get; set; }
    public static ProjectSettings FromVm(ProjectConfigViewModel vm) =>
new ProjectSettings
{
    ProjectName = vm.ProjectName,
    DbServer = vm.DbServer,
    DatabaseName = vm.DatabaseName,
    DbUser = vm.DbUser,
    DbPassword = vm.DbPassword,
    AccessVersion = vm.AccessVersion,
    UseIntegratedSecurity = vm.UseIntegratedSecurity,
    EnableLogging = vm.EnableLogging
};
}
