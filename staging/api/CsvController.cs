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
    var helpers = GetCode("../tools/DynData.cs");
    var query = Kit.Data.GetQuery("DynamicData", parameters: new { ModuleId = id }); // Get the dynamic data from Query by ModuleId
    var data = query.List;
    // Initialize lists to store CSV-related data

    List<ITyped> jsonTyped = helpers.GetDataJsonTyped(data); // Initialize lists to store CSV-related data
    List<string> headerProps = helpers.CreateHeader(jsonTyped); // Create the Header with all specifications
    List<List<string>> dataRows = CreateDataRows(jsonTyped, headerProps); // Create Csv Data


return jsonTyped;

    // Write CSV data to the file

    var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) {
        Delimiter = ";"
    };
    using (var memoryStream = new MemoryStream())
    using (var writer = new StreamWriter(memoryStream, Encoding.Default))
    using (var csv = new CsvWriter(writer, csvConfiguration))
    {
      // Write the CSV header
      foreach (var header in headerProps)
      {
        csv.WriteField(header);
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

      return File(download: true, virtualPath: null, contentType: "text/csv", fileDownloadName: "csvDynFormData.csv", contents: csvString);
    }
  }

  private List<List<string>> CreateDataRows(List<ITyped> jsonTyped, List<string> headerProps)
  {
    var dataRowsNew = jsonTyped
      .Select(fields => headerProps
          .Where(key => Text.Has(key))
          .Select(key => {
            object valueObj = fields.Get(key, required: false); // Get the value for the key or set an empty value

            // Debug WIP (turn off during production)
            Log.Add($"CreateDataRows: {key}: '{valueObj}' ({valueObj?.GetType()})");

            try {
              // If it's a DateTime, format it accordingly
              if (valueObj is DateTime dateTimeValue)
                return dateTimeValue.ToString("yyyy-MM-ddTHH:mm");

              // If it's a list, convert it to a string
              if (valueObj is IList<object> objectList)
                return string.Join(", ", objectList.Select(item => item?.ToString() ?? string.Empty));

              // Test how exceptions work
              // throw new Exception("test");

              // Otherwise, add the value as a string
              return valueObj?.ToString() ?? string.Empty;
            } catch (Exception ex) {
              Log.Exception(ex);
              return "error";
            }
          })
          .ToList()
      )
      .ToList();
    
    return dataRowsNew;
  }

}