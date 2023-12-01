using System.Collections.Generic;
using System;
using System.Linq;
using ToSic.Sxc.Data;

public class DynData : Custom.Hybrid.CodeTyped

{
    public List<ITyped> GetDataJsonTyped(object data)
  {
    return AsItems(data).Select(i =>
  {
    var jsonDoc = Kit.Json.ToTyped(i.String("RawData"));
    return AsTyped(jsonDoc.Get("Fields"));
  }).ToList();
  }

  public List<string> CreateHeader(List<ToSic.Sxc.Data.ITyped> jsonTyped)
  {
    List<string> fieldKeysList = new List<string>();
    foreach (var rows in jsonTyped)
    {
      fieldKeysList.AddRange(rows.Keys());
    }

    List<string> fieldKeysListDistinct = fieldKeysList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    List<string> endSortedProps = new List<string> { "Timestamp" };
    List<string> hiddenProps = new List<string> { "SenderIP","ModuleId","FormId" };
    List<string> header = new List<string>();

    foreach (var prop in endSortedProps)
    {
      if (fieldKeysListDistinct.Contains(prop))
      {
        fieldKeysListDistinct.Remove(prop);
        fieldKeysListDistinct.Add(prop);
      }
    }

    foreach (var prop in hiddenProps)
    {
      if (fieldKeysListDistinct.Contains(prop))
      {
        fieldKeysListDistinct.Remove(prop);
      }
    }

    foreach (var prop in fieldKeysListDistinct)
    {
      if (!hiddenProps.Contains(prop))
      {
        header.Add(prop);
      }
    }

    return header;
  }

}