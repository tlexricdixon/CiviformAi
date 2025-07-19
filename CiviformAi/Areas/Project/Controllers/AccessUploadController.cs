using CiviformAi.Models;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace CiviformAi.Areas.Project.Controllers;

[Area("Admin")]
public class AccessUploadController : Controller
{
    private readonly IAccessSchemaReader<TableSchema> _reader;
    private readonly TempAccessSchemaStore _tempAccessStore;
    private readonly IAccessImportService<TableSchema> _service;
    private readonly string path;
    public AccessUploadController(IWebHostEnvironment env, IAccessSchemaReader<TableSchema> reader, 
        TempAccessSchemaStore tempAccessStore, IAccessImportService<TableSchema> service)
    {
        _reader = reader;
        path = env.ContentRootPath;
        _tempAccessStore = tempAccessStore;
        _service = service;
    }
    [HttpGet]
    public IActionResult UploadAccessDb()
    {
        var schemaMap = _tempAccessStore.GetAll();
        return View(schemaMap);
    }

    //[HttpPost]
    //[RequestSizeLimit(50_000_000)]
    //public async Task<IActionResult> UploadAccessDb(List<IFormFile> files)
    //{
    //    if (files == null || files.Count == 0)
    //        return BadRequest("No files uploaded.");
    //    var schemaMap = await _reader.GetAccessSchemaAsync(files, path);
    //    // For now, just show it in a View or return JSON
    //    _tempAccessStore.SaveSchema(schemaMap);
    //    return View(schemaMap);
    //}
    public IActionResult Details(string tableName)
    {
        var table = _tempAccessStore.GetTable(tableName);
        return View(table);
    }
    [HttpPost]
    public IActionResult GenerateSql(string tableName)
    {
        var schemaMap = _tempAccessStore.GetTable(tableName);
        ViewBag.SqlPreview = _service.GenerateSQL(schemaMap);
        return View("Details", schemaMap);
    }
    [HttpPost]
    public async Task<IActionResult> CreateTable(TableSchema schema)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid schema");

        await _service.Import(schema);

        // 👇 Redirect to MigrateTableData with table name
        return RedirectToAction("MigrateTableData", new { tableName = schema.TableName });
    }

    [HttpGet]
    public async Task<IActionResult> MigrateTableData(string tableName)
    {
        var schemaMap = _tempAccessStore.GetTable(tableName);

        if (schemaMap == null)
            return NotFound("Table not found");

        var viewModel = new TableMigrationViewModel
        {
            TableName = schemaMap.TableName,
            FileName = schemaMap.FileName,
            Columns =schemaMap.Columns,
            Records = null // initially null, will be populated post-migration
        };

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> MigrateTableData(string tableName, string fileName)
    {
        var fullPath = Path.Combine(path, "TempUploads", fileName);
        var rows = await _service.MigrateTableDataAsync(fullPath, tableName);
        var columns = rows.FirstOrDefault()?.Keys
           .Select(col => new ColumnSchema
           {
               ColumnName = col,
               DataType = "UNKNOWN",   // or try to infer from row[col].GetType()
               SqlType = "UNKNOWN"
           }).ToList() ?? new List<ColumnSchema>();

        var viewModel = new TableMigrationViewModel
        {
            TableName = tableName,
            FileName = fileName,
            Columns = columns,
            Records = rows.Take(10).ToList()
        };
        var table = _tempAccessStore.GetTable(tableName);
        table.IsConverted = true;
        _tempAccessStore.UpdateSchema(table); // You’ll need to implement this update method
        return View("MigrateTableData", viewModel);
    }
}
