using Microsoft.Office.Interop.Access;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
namespace AccessInteropLib
{
    public class AccessFormExporterService
    {
        public string ExportFormMetadataToJson(string accdbPath)
        {
            var app = new Application();
            app.OpenCurrentDatabase(accdbPath);

            var forms = new List<object>();

            foreach (string formName in app.CurrentProject.AllForms.Cast<AccessObject>().Select(f => f.Name))
            {
                app.DoCmd.OpenForm(formName, AcFormView.acDesign); // open in design view
                Form form = app.Forms[formName];

                var controls = new List<object>();
                foreach (Control ctrl in form.Controls)
                {
                    controls.Add(new
                    {
                        Name = ctrl.Name,
                        Type = SafeGet(() => ctrl.GetType().InvokeMember("Caption", System.Reflection.BindingFlags.GetProperty, null, ctrl, null)?.ToString().ToString()),

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

                        Left = SafeGet(() => ctrl.GetType().InvokeMember(
                            "Left",
                            System.Reflection.BindingFlags.GetProperty,
                            null,
                            ctrl,
                            null
                        )?.ToString()),

                        Top = SafeGet(() => ctrl.GetType().InvokeMember(
                            "Top",
                            System.Reflection.BindingFlags.GetProperty,
                            null,
                            ctrl,
                            null
                        )?.ToString()),

                        Width = SafeGet(() => ctrl.GetType().InvokeMember(
                            "Width",
                            System.Reflection.BindingFlags.GetProperty,
                            null,
                            ctrl,
                            null
                        )?.ToString()),

                        Height = SafeGet(() => ctrl.GetType().InvokeMember(
                            "Height",
                            System.Reflection.BindingFlags.GetProperty,
                            null,
                            ctrl,
                            null
                        )?.ToString())
                    });
                }
                forms.Add(new
                {
                    FormName = formName,
                    RecordSource = form.RecordSource,
                    Controls = controls
                });

                app.DoCmd.Close(AcObjectType.acForm, formName, AcCloseSave.acSaveNo);
            }

            app.CloseCurrentDatabase();
            Marshal.ReleaseComObject(app);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return JsonConvert.SerializeObject(forms, Newtonsoft.Json.Formatting.Indented);
        }

        private T SafeGet<T>(Func<T> getter)
        {
            try { return getter(); }
            catch { return default(T); } // Works in C# 7.3
        }
    }
}
