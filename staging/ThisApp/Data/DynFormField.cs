namespace ThisApp.Data
{
  public partial class DynFormField : Custom.Data.Item16
  {
    public string FieldId => String(fallback: "");
    public new string Title => String(fallback: "");


    public string FieldType => String(fallback: "");

    public string InitialValue => String(fallback: "");

    public bool Required => Bool();


    public bool IsDisabled => Bool();

    #region String

    public int StringLines => Int();

    #endregion

    #region Number

    public int MinLength => Int();
    public int MaxLength => Int();

    #endregion

    #region Email

    public string RecipientEmail => String(fallback: "");

    #endregion

    #region Boolean
    public bool LabelRight => Bool();

    #endregion

    #region Terms
    public bool TermsAndGdprCombined => Bool();
    public bool TermsEnabled => Bool();
    public bool GdprEnabled => Bool();

    #endregion

    #region Picker
    public string PickerType => String(fallback: "");
    public string PickerKeyValues => String(fallback: "");
    public bool CheckboxWithHeadline => Bool();
    public bool MultiSelect => Bool();
    public string PlaceHolderSelect => String(fallback: "");

    #endregion

    #region Hidden
    public string HiddenValue => String(fallback: "");
    #endregion

    #region Advanced
    public string RazorFile => String(fallback: "");
    #endregion

  }
}