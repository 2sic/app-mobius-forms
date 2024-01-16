using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Data
{
  public partial class DynFormField : TypedItem
  {
    public DynFormField(ITypedItem item) : base(item) { }

    public string FieldId => GetThis(fallback: "");

    public string FieldType => GetThis(fallback: "");

    public string InitialValue => GetThis(fallback: "");

    public bool Required => GetThis(fallback: false);


    public bool IsDisabled => GetThis(fallback: false);

    #region String

    public int StringLines => GetThis(fallback: 0);

    #endregion

    #region Number

    public int MinLength => GetThis(fallback: 0);
    public int MaxLength => GetThis(fallback: 0);
    
    #endregion
  }
}