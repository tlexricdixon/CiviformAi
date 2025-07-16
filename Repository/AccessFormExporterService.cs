using Microsoft.Office.Interop.Access;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AccessApplication = Microsoft.Office.Interop.Access.Application; // Alias to resolve ambiguity
using Form = Microsoft.Office.Interop.Access.Form; // Alias to resolve ambiguity

namespace Repository
{
    public class AccessFormExporterService
    {
        public string ExportFormMetadataToJson(string accdbPath)
        {
            var app = new Application();
            try
            {
                app.OpenCurrentDatabase(accdbPath);

                var forms = new List<object>();

                foreach (AccessObject formObj in app.CurrentProject.AllForms)
                {
                    string formName = formObj.Name;

                    app.DoCmd.OpenForm(formName, AcFormView.acDesign);
                    Form form = app.Forms[formName];

                    var controls = new List<object>();

                    foreach (Control ctrl in form.Controls)
                    {
                        // Add the necessary assembly reference for 'IAccessible'  
                        // In Visual Studio, right-click on the project in Solution Explorer, select 'Add Reference...',  
                        // then find and add the 'Microsoft.Office.Interop.Access' assembly.  
                        // Ensure that the 'office' assembly is also referenced if not already.
                        controls.Add(new
                        {
                            Name = ctrl.Name,
                            Type = SafeGet(() => ctrl.GetType().InvokeMember(
                                "ControlType",
                                System.Reflection.BindingFlags.GetProperty,
                                null,
                                ctrl,
                                null
                            )?.ToString()),
                            ControlSource = SafeGet(() => ctrl.GetType().InvokeMember(
                                "ControlSource",
                                System.Reflection.BindingFlags.GetProperty,
                                null,
                                ctrl,
                                null
                            )?.ToString()),
                            Label = SafeGet(() => ctrl.GetType().InvokeMember(
                                "Caption",
                                System.Reflection.BindingFlags.GetProperty,
                                null,
                                ctrl,
                                null
                            )?.ToString()),
                            Left = SafeGet(() => (int?)ctrl.GetType().InvokeMember("Left", BindingFlags.GetProperty, null, ctrl, null)),
                            Top = SafeGet(() => (int?)ctrl.GetType().InvokeMember("Top", BindingFlags.GetProperty, null, ctrl, null)),
                            Width = SafeGet(() => (int?)ctrl.GetType().InvokeMember("Width", BindingFlags.GetProperty, null, ctrl, null)),
                            Height = SafeGet(() => (int?)ctrl.GetType().InvokeMember("Height", BindingFlags.GetProperty, null, ctrl, null))
                        });
                    }

                        forms.Add(new
                    {
                        FormName = formName,
                        RecordSource = SafeGet(() => form.RecordSource),
                        Controls = controls
                    });

                    app.DoCmd.Close(AcObjectType.acForm, formName, AcCloseSave.acSaveNo);
                }

                return JsonConvert.SerializeObject(forms, Formatting.Indented);
            }
            finally
            {
                // Cleanup to avoid COM object lock
                app.CloseCurrentDatabase();
                Marshal.ReleaseComObject(app);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            } 
        }

        private T SafeGet<T>(Func<T> getter)
        {
            try { return getter(); }
            catch { return default!; }
        }
    }

    public class FormExportBridge
    {
        public string ExportFormsToJson(string path)
        {
            var exporter = new AccessFormExporterService();
            return exporter.ExportFormMetadataToJson(path);
        }
    }
}
