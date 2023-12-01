using System.Collections.Generic;
using System;
using System.Linq;
using ToSic.Sxc.Data;

public class DynData : Custom.Hybrid.CodeTyped

{

  public List<ITyped> GetDataJsonTyped(object data)
  {

    List<ITyped> fields = GetRawData(data);


    return fields;

  }

  public List<ITyped> GetRawData(object data)
  {

    return AsItems(data).Select(i =>
      {
        var rawData = Kit.Json.ToTyped(i.String("RawData"));
        return AsTyped(rawData.Get("Fields"));
      }).ToList();
  }

// TODO:: Hier möchte ich die Id und Timestamp noch hinzufügen und eine neue liste erstellen 
  // public List<ITyped> GetRawData(object data)
  // {

  //   return AsItems(data).Select(i =>
  //     {
  //       var id = i.Int("Id");
  //       var timestamp = i.DateTime("Timestamp");
  //       var rawData = Kit.Json.ToTyped(i.String("RawData"));
  //       var field = AsTyped(rawData.Get("Fields"));
        
  //       return 
  //     }).ToList();
  // }


  public List<string> CreateHeader(List<ITyped> jsonTyped)
  {
    List<string> fieldKeysList = new List<string>();
    foreach (var rows in jsonTyped)
    {
      fieldKeysList.AddRange(rows.Keys());
    }

    List<string> fieldKeysListDistinct = fieldKeysList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    List<string> endSortedProps = new List<string> { "Timestamp" };
    List<string> hiddenProps = new List<string> { "SenderIP", "ModuleId", "FormId" };
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