using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Data
{
  public partial class AppResources : TypedItem
  {
    public AppResources(ITypedItem item) : base(item) { }
      #region Terms
    public string LabelTermsAll => GetThis(fallback: "");
    public string LabelTerms => GetThis(fallback: "");
    public string LabelGdpr => GetThis(fallback: "");
    public string LabelSelect => GetThis(fallback: "");

    
    #endregion

    // public string String(string key, string fallback = null) => Item.String(key, fallback: fallback);
  }
}