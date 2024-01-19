using System;
using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class DynForm : Custom.Data.Item16Experimental
  {
    public DynForm(ITypedItem item) : base(item) { }

    public string DesignField => GetThis(fallback: "");
    public Boolean ReuseConfig => GetThis(fallback: false);

  }
}