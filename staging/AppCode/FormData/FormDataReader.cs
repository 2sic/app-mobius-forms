using System;
using System.Linq;
using ToSic.Sxc.Data;

namespace AppCode.FormData
{
  public class FormDataReader
  {
    public FormDataReader(ITypedItem item, ITyped json)
    {
      Json = json;
      Item = item;
    }
    public ITyped Json { get; }
    public ITypedItem Item { get; }

    public object Get(string key)
    {
      if (key == "Id") return Item.Id;
      if (key == "Timestamp") return Item.DateTime("Timestamp").ToString("yyyy-MM-dd");
      if (key == "Files") return Item.Folder("Files").Files.Count();
      return Json.Get(key, required: false);
    }

    public string GetString(string key) => Get(key) switch
    {
      null => "",
      string sVal => sVal,
      int iVal => iVal.ToString(),
      DateTime dtm => dtm.ToString("yyyy-MM-ddTHH:mm"),
      bool bVal => bVal.ToString(),
      object oVal when oVal is System.Collections.IEnumerable objList => string.Join(", ", objList.Cast<object>().Select(o => o?.ToString() ?? "")),
      // object _ => "Unknown Object",
      _ => "Unknown Type",
    };

  }

}