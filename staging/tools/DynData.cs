using System.Collections.Generic;
using System;
using System.Linq;
using ToSic.Sxc.Data;

public class DynData : Custom.Hybrid.CodeTyped
{
  public List<(ITyped Json, ITypedItem Item)> PrepareData(object data)
  {

    return AsItems(data).Select(i =>
      {
        var rawData = Kit.Json.ToTyped(i.String("RawData"));
        return (AsTyped(rawData.Get("Fields")), i);
      }).ToList();
  }

  public List<string> ListOfHeaderProps(List<ITyped> jsonTyped)
  {
    List<string> fieldKeysList = new List<string>();
    foreach (var rows in jsonTyped)
    {
      fieldKeysList.AddRange(rows.Keys());
    }

    List<string> fieldKeysListDistinct = fieldKeysList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    List<string> hiddenProps = new List<string> { "FormId" };
    List<string> header = new List<string>();

    foreach (var prop in hiddenProps)
      if (fieldKeysListDistinct.Contains(prop))
        fieldKeysListDistinct.Remove(prop);

    foreach (var prop in fieldKeysListDistinct)
      if (!hiddenProps.Contains(prop))
        header.Add(prop);

    return header;

  }

  public Dictionary<string, string> GetFieldDictionary(string formId)
  {
    var dynFormFields = AsItems(App.Data["DynForm"]).Where(f => f.Id == Int32.Parse(formId)).FirstOrDefault().Children("Fields");
    Dictionary<string, string> dictionary = new Dictionary<string, string>();

    foreach (var field in dynFormFields)
      dictionary[field.Url("FieldId")] = field.String("Title");

    return dictionary;
  }

  public string GetFieldValueOrKey(Dictionary<string, string> dictionary, string key)
  {
    return dictionary.ContainsKey(key) ? dictionary[key] : key;
  }

}
