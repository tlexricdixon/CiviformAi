using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using static System.Net.WebRequestMethods;

namespace BlazorCMS.Components
{
    // TODO: Implement drag-and-drop ACCDB (Northwind for now)
    // - Accept file input
    // - Save to temp folder
    // - Trigger backend parser
    // - Status: In Progress
    public partial class DragDrop()
    {
        [Inject]
        private HttpClient Http { get; set; } = default!;
        private List<UploadedFile> uploadedFiles = new();
        public async Task HandleDrop(DragEventArgs e){}
        public void RemoveFile(UploadedFile file)
        {
            uploadedFiles.Remove(file);
        }

        public void ClearFiles()
        {
            uploadedFiles.Clear();
        }

        public async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            uploadedFiles.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                uploadedFiles.Add(new UploadedFile
                {
                    Name = file.Name,
                    Size = file.Size,
                    BrowserFile = file
                });
            }
        }
        public async Task UploadFilesAsync()
        {
            const int maxFileSize = 50 * 1024 * 1024; // 50MB
            var uploadFolder = Path.Combine(AppContext.BaseDirectory, "TempUploads");
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in uploadedFiles)
            {
                try
                {
                    var tempPath = Path.Combine(uploadFolder, file.Name);

                    await using var stream = file.BrowserFile.OpenReadStream(maxFileSize);
                    await using var fs = new FileStream(tempPath, FileMode.Create);
                    await stream.CopyToAsync(fs);

                    file.LocalPath = tempPath;

                    await using var fileStream = System.IO.File.OpenRead(tempPath);
                    using var content = new MultipartFormDataContent();
                    using var streamContent = new StreamContent(fileStream);

                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    content.Add(streamContent, "file", file.Name);

                    var response = await Http.PostAsync("api/AccessDBController/upload-access-db", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.Error.WriteLine($"❌ Upload failed for {file.Name}: {response.StatusCode}");
                        // Optional: UI feedback
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"⚠️ Error uploading {file.Name}: {ex.Message}");
                    // Optional: log or report via UI
                }
            }
        }

        public class UploadedFile
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public string LocalPath { get; set; }
            public IBrowserFile BrowserFile { get; set; }
        }
    }
}