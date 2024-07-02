using System.Collections.Generic;
using System;
using System.Linq;
using ToSic.Sxc.Data;

namespace AppCode.FormData
{
  public class FormDataService : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Setup the service to get data for a specific module
    /// </summary>
    /// <param name="_formId1"></param>
    /// <returns></returns>
    public FormDataService Setup(int formId)
    {
      _formId1 = formId;
      return this;
    }
    private int _formId1;

    /// <summary>
    /// The FormId is retrieved from the first submitted data.
    /// It's then used to lookup columns names etc.
    /// </summary>
    /// 
    public int FormId => _formId ??= Data.FirstOrDefault(p => p.Item.Int("FormId") != 0)?.Item.Int("FormId") ?? 0;
    private int? _formId;



    /// <summary>
    /// The form data for the current module.
    /// It's cached, so accessing it multiple times won't rerun the query.
    /// </summary>
    public List<FormDataReader> Data => _data ??= GetData();
    private List<FormDataReader> _data;

    private List<FormDataReader> GetData()
    {
      var query = Kit.Data.GetQuery("DynamicData", parameters: new { FormId = _formId1 }); // Get the dynamic data from Query by FormId

      var data = query.List;

      return AsItems(data)
        .Select(i =>
        {
          var rawData = Kit.Json.ToTyped(i.String("RawData"));
          return new FormDataReader(i, AsTyped(rawData.Get("Fields")));
        })
        .ToList();
    }

    /// <summary>
    /// Columns in the dynamic json data
    /// </summary>
    public List<string> Columns => _columns ??= GetColumns(Data.Select(p => p.Json).ToList());
    private List<string> _columns;

    // internal helper
    private List<string> GetColumns(List<ITyped> jsonTyped)
    {
      var fieldKeysList = jsonTyped.SelectMany(rows => rows.Keys()).ToList();

      var hiddenProps = new List<string> { "FormId" };

      var fieldKeysListDistinct = fieldKeysList
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .Where(prop => !hiddenProps.Contains(prop))
          .ToList();

      return fieldKeysListDistinct.ToList();
    }
    public Dictionary<string, string> ColumnHeaders => _columnHeaders ??= GetColumnHeaders();
    private Dictionary<string, string> _columnHeaders;

    private Dictionary<string, string> GetColumnHeaders()
    {
      var formFields = AsItems(App.Data["FormConfig"])
          .FirstOrDefault(f => f.Id == FormId)
          ?.Children("Fields");

      var baseDictionary = formFields?.ToDictionary(field => field.Url("FieldId"), field => field.String("Title"))
                              ?? new Dictionary<string, string>();

      // Create the full list of headers and "translated" labels
      var headers = new List<string> { "Id", "Timestamp" }
                        .Concat(Columns)
                        .Concat(new List<string> { "Files" });

      return headers.ToDictionary(
          k => k,
          v => baseDictionary.ContainsKey(v) ? baseDictionary[v] : v
      );
    }
  }
}