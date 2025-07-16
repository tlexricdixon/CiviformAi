using Repository;

namespace CiviformAi.Extensions;

public class AccessFormInteropBridge
{
    public string ExportForms(string accdbPath)
    {
        // Assuming FormExportBridge is the public entry point in the DLL
        var bridge = new FormExportBridge();
        return bridge.ExportFormsToJson(accdbPath);
    }
}
