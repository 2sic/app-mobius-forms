using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class DynFormField : Custom.Data.Item16Experimental
  {
    public DynFormField(ITypedItem item) : base(item) { }

    public string FieldId => GetThis(fallback: "");
    public new string Title => GetThis(fallback: "");


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
  
    #region Email

    public string RecipientEmail => GetThis(fallback: "");

    #endregion

    #region Boolean
    public bool LabelRight => GetThis(fallback: false);
    
    #endregion

    #region Terms
    public bool TermsAndGdprCombined => GetThis(fallback: false);
    public bool TermsEnabled => GetThis(fallback: false);
    public bool GdprEnabled => GetThis(fallback: false);
    
    #endregion

     #region Picker
    public string PickerType => GetThis(fallback: "");
    public string PickerKeyValues => GetThis(fallback: "");
    public bool CheckboxWithHeadline => GetThis(fallback: false);
    public bool MultiSelect => GetThis(fallback: false);    
    public string PlaceHolderSelect => GetThis(fallback: "");

    #endregion

    #region Hidden
    public string HiddenValue => GetThis(fallback: "");
    #endregion

    #region Advanced
    public string RazorFile => GetThis(fallback: "");
    #endregion

  }
}