using System.Collections.Generic;
using System;
using System.Linq;
using ToSic.Sxc.Data;
using ThisApp.Data;

namespace ThisApp.FormData
{
  public class FormDataService : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Setup the service to get data for a specific module
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public FormDataService Setup(int moduleId)
    {
      _moduleId = moduleId;
      return this;
    }
    private int _moduleId;

    /// <summary>
    /// The FormId is retrieved from the first submitted data.
    /// It's then used to lookup columns names etc.
    /// </summary>
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
      var query = Kit.Data.GetQuery("DynamicData", parameters: new { ModuleId = _moduleId }); // Get the dynamic data from Query by ModuleId
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
      var fieldKeysList = new List<string>();
      foreach (var rows in jsonTyped)
        fieldKeysList.AddRange(rows.Keys());

      var fieldKeysListDistinct = fieldKeysList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
      var hiddenProps = new List<string> { "FormId" };
      var header = new List<string>();

      foreach (var prop in hiddenProps)
        if (fieldKeysListDistinct.Contains(prop))
          fieldKeysListDistinct.Remove(prop);

      foreach (var prop in fieldKeysListDistinct)
        if (!hiddenProps.Contains(prop))
          header.Add(prop);

      return header;
    }

    public Dictionary<string, string> ColumnHeaders => _columnHeaders ??= GetColumnHeaders();
    private Dictionary<string, string> _columnHeaders;

    private Dictionary<string, string> GetColumnHeaders()
    {
      var dynFormFields = AsItems(App.Data["FormConfig"])
        .FirstOrDefault(f => f.Id == FormId)
        ?.Children("Fields");

      var baseDictionary = new Dictionary<string, string>();

      foreach (var field in dynFormFields)
        baseDictionary[field.Url("FieldId")] = field.String("Title");

      // Create the fill list of headers and "translated" labels
      return new List<string> { "Id", "Timestamp" }
        .Concat(Columns)
        .Concat(new List<string> { "Files" })
        .ToDictionary(
          k => k,
          v => baseDictionary.ContainsKey(v) ? baseDictionary[v] : v
        );
    }
  }

}