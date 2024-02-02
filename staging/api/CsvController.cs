#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;
#endif

using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;
using System;
using System.Linq;
using System.Text;
using ThisApp.Data;
using ThisApp.Form;

[AllowAnonymous]
public class CsvController : Custom.Hybrid.ApiTyped
{

  [HttpGet]
  public object Csv(int id)
  {
    var dynDataHelper = GetService<FormDataService>().Setup(id);// int.TryParse(id, out var intId) ? intId : 0);

    var columns = dynDataHelper.Columns; // Create the Header with all specifications

    var dataRows = GetRowDataList(dynDataHelper.FormData, columns); // Get Csv Data in a String List

    // Write CSV data to the file
    var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
    {
      Delimiter = ";"
    };
    using var memoryStream = new MemoryStream();
    using var writer = new StreamWriter(memoryStream, Encoding.Default);
    using var csv = new CsvWriter(writer, csvConfiguration);

    // Write the CSV header
    foreach (var headerProp in dynDataHelper.ColumnHeaders)
      csv.WriteField(headerProp.Value);
    csv.NextRecord();

    // Write the CSV data rows
    foreach (var dataRow in dataRows)
    {
      foreach (var field in dataRow)
        csv.WriteField(field);
      csv.NextRecord();
    }

    writer.Flush(); // flush the buffered text to stream
    memoryStream.Seek(0, SeekOrigin.Begin); // reset stream position

    var csvString = Encoding.UTF8.GetString(memoryStream.ToArray());
    string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
    string fileName = $"FormData_Form{dynDataHelper.FormId}_{todayDate}.csv";

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