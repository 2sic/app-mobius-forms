using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Data
{
  public partial class DynForm : TypedItem
  {
    public DynForm(ITypedItem item) : base(item) { }

    public string DesignField => GetThis(fallback: "");

  }
}