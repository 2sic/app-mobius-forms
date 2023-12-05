#if NETCOREAPP
  using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
  using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;
#endif

using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System;
using System.Linq;
using System.Text;
using ToSic.Sxc.Data;
using ToSic.Razor.Blade;

[AllowAnonymous]
public class CsvController : Custom.Hybrid.ApiTyped
{

  [HttpGet]
  public object Csv(string id = "6573")
  {
    var dynDataHelper = GetCode("../tools/DynData.cs");
    var query = Kit.Data.GetQuery("DynamicData", parameters: new { ModuleId = id }); // Get the dynamic data from Query by ModuleId
    // Initialize lists to store CSV-related data
    List<(ITyped Json, ITypedItem Item)> dataPairs = dynDataHelper.PrepareData(query.List); // Initialize Data

    List<string> headerProps = dynDataHelper.ListOfHeaderProps(dataPairs.Select(p => p.Json).ToList()); // Create the Header with all specifications

    List<List<string>> dataRows = GetRowDataList(dataPairs, headerProps); // Get Csv Data in a String List

    var formId = dataPairs.Select(p => p.Item.String("formId")).FirstOrDefault();

    Dictionary<string, string> fieldHeaderDictionary = dynDataHelper.GetFieldDictionary(formId);

    // Write CSV data to the file
    var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
    {
      Delimiter = ";"
    };
    using (var memoryStream = new MemoryStream())
    using (var writer = new StreamWriter(memoryStream, Encoding.Default))
    using (var csv = new CsvWriter(writer, csvConfiguration))
    {
      // Write the CSV header
      csv.WriteField("Id");
      csv.WriteField("Timestamp");
      // TODO:: Übersetzung fehlt im CSV
      // csv.WriteField(App.Resources.String("LabelTimestamp"));
      foreach (var headerProp in headerProps)
      {
        csv.WriteField(dynDataHelper.GetFieldValueOrKey(fieldHeaderDictionary, headerProp));
      }
      csv.NextRecord();

      // Write the CSV data rows
      foreach (var dataRow in dataRows)
      {
        foreach (var field in dataRow)
        {
          csv.WriteField(field);
        }
        csv.NextRecord();
      }

      writer.Flush(); // flush the buffered text to stream
      memoryStream.Seek(0, SeekOrigin.Begin); // reset stream position

      var csvString = Encoding.UTF8.GetString(memoryStream.ToArray());
      string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
      string fileName = $"csvDynFormData_{formId}_{todayDate}.csv";

      return File(download: true, virtualPath: null, contentType: "text/csv", fileDownloadName: fileName, contents: csvString);
    }
  }

  private List<List<string>> GetRowDataList(List<(ITyped Json, ITypedItem Item)> dataPairs, List<string> headerProps)
  {
    return dataPairs
        .Select(pair =>
        {
          int? idValue = pair.Item.Id;
          DateTime? timestampValue = pair.Item.DateTime("Timestamp");

          var rowData = new List<string>
            {
                idValue.HasValue ? idValue.ToString() : string.Empty,
                timestampValue.HasValue ? timestampValue.Value.ToString("yyyy-MM-dd") : string.Empty
            };

          rowData.AddRange(headerProps.Select(key => FormatFieldValue(pair.Json.Get(key, required: false))));
          return rowData;
        })
        .ToList();
  }

  private string FormatFieldValue(object value)
  {
    try
    {
      // DateTime
      if (value is DateTime dateTimeValue)
        return dateTimeValue.ToString("yyyy-MM-ddTHH:mm");

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