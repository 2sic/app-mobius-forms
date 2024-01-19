using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class AppResources : Custom.Data.Item16Experimental
  {
    public AppResources(ITypedItem item) : base(item) { }

    public string ToolbarReuseInfo => GetThis(fallback: "");
    public string LabelFromDataAvaible => GetThis(fallback: "");

      #region Terms
    public string LabelTermsAll => GetThis(fallback: "");
    public string LabelTerms => GetThis(fallback: "");
    public string LabelGdpr => GetThis(fallback: "");
    public string LabelSelect => GetThis(fallback: "");
    #endregion

  }
}