#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;
#endif
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System;
using System.Linq;
using System.Text;

using AppCode.FormData;

[AllowAnonymous]
public class CsvController : Custom.Hybrid.ApiTyped
{

  [HttpGet]
  public object Csv(int id)
  {
    var formData = GetService<FormDataService>().Setup(id);

    var columns = formData.Columns;
    var dataRows = GetRowDataList(formData.Data, columns);

    var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      Delimiter = ";"
    };

    // Write CSV data to the file
    using var memoryStream = new MemoryStream();
    using var writer = new StreamWriter(memoryStream, Encoding.Default);
    using var csv = new CsvWriter(writer, csvConfiguration);

    // Write the CSV header
    formData.ColumnHeaders.Values
        .ToList()
        .ForEach(header => csv.WriteField(header));
    csv.NextRecord();

    // Write the CSV data rows
    dataRows.ForEach(dataRow =>
    {
      dataRow.ForEach(field => csv.WriteField(field));
      csv.NextRecord();
    });

    writer.Flush(); // Flush the buffered text to stream
    memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position

    // Generate file name
    string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
    string fileName = $"FormData_Form{formData.FormId}_{todayDate}.csv";

    // Return the CSV data as a file
    var csvString = memoryStream.ToArray();
    return File(download: true, virtualPath: null, contentType: "text/csv", fileDownloadName: fileName, contents: csvString);
  }

  private List<List<string>> GetRowDataList(List<FormDataReader> data, List<string> columns)
    => data.Select(pair =>
      {
        int? idValue = pair.Item.Id;
        DateTime? timestampValue = pair.Item.DateTime("Timestamp");

        var rowData = new List<string>
          {
              idValue.HasValue ? idValue.ToString() : string.Empty,
              timestampValue.HasValue ? timestampValue.Value.ToString("yyyy-MM-dd") : string.Empty
          };

        rowData.AddRange(columns.Select(key => FormatFieldValue(pair.Json.Get(key, required: false))));
        return rowData;
      })
      .ToList();

  private string FormatFieldValue(object value)
  {
    try
    {
      // DateTime
      if (value is DateTime dtm)
        return dtm.ToString("yyyy-MM-ddTHH:mm");

      // List
      if (value is IList<object> objectList)
        return string.Join(", ", objectList.Select(item => item?.ToString() ?? string.Empty));

      // Strings
      return value?.ToString() ?? string.Empty;
    }
    catch (Exception ex)
    {
      Log.Exception(ex);
      return "error";
    }
  }
}