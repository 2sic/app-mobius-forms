using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public class FormDataItem
  {
    public FormDataItem(ITypedItem item, ITyped json)
    {
      Json = json;
      Item = item;
    }
    public ITyped Json {get;}
    public ITypedItem Item {get;}
  }

}