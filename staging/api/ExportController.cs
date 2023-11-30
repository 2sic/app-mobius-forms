
using System.Collections.Generic;
using System.IO;
using System.Web.Http;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net;
using ToSic.Razor.Html5;

[AllowAnonymous]
public class ExportController : Custom.Hybrid.ApiTyped
{
    [HttpGet]
    [AllowAnonymous]


// public object CSV(string id = "6573")
//     {

//         // Get Data from Query
//         // var query = Kit.Data.GetQuery("DynamicData", parameters: new { 
//         //     ModuleId = 6573
//         // });

//         // var data = query.List; // AsItems(query);

//         // return data;
//         // url:https://app-dev.2sxc.org/mobius/api/2sxc/app/auto/staging/api/Export/CSV?PageId=3099&ModuleId=6577

//         return "x";

//     }

// Link zum testen
// https://joshclose.github.io/CsvHelper/examples/writing/
    public dynamic CSV()
    {
        // Dummy-Daten für das Beispiel
        var records = new List<SampleData>
        {
            new SampleData { Id = 1, Name = "John Doe", Age = 30 },
            new SampleData { Id = 2, Name = "Jane Smith", Age = 25 },
            // Weitere Datensätze hinzufügen...
        };

        var stream = new MemoryStream();

        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(records);
        }

        stream.Position = 0;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(stream)
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        {
            FileName = "example.csv"
        };

        return response;
    }

     private class SampleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    
}

