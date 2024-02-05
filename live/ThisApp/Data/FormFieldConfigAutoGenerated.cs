/*
  * This file is automatically generated at 2024-01-31 17:59:00
  * DO NOT MODIFY IT BY HAND
  * If you need to extend it, create a partial class for "FormFieldConfig" in a separate file.
*/
namespace ThisApp.Data
{
  public partial class FormFieldConfig: FormFieldConfigAutoGenerated { }

  public abstract class FormFieldConfigAutoGenerated : Custom.Data.Item16
  {
    public string FieldId => String(fallback: "");
    public new string Title => String(fallback: "");


    public string FieldType => String(fallback: "");

    public string DefaultValue => String(fallback: "");

    public bool Required => Bool();


    public bool IsDisabled => Bool();

    #region String

    public int StringLines => Int();

    #endregion

    #region Number

    public int NumberMin => Int();
    public int NumberMax => Int();

    #endregion

    #region Email

    public string EmailUseAsRecipient => String(fallback: "");

    #endregion

    #region Boolean
    public bool BooleanLabelRight => Bool();

    #endregion

    #region Terms
    public bool TermsAndGdprCombined => Bool();
    public bool TermsEnabled => Bool();
    public bool GdprEnabled => Bool();

    #endregion

    #region Picker
    public string PickerType => String(fallback: "");
    public string PickerKeyValues => String(fallback: "");
    public bool PickerCheckboxGrouped => Bool();
    public bool PickerMultiSelect => Bool();
    public string PickerPlaceholder => String(fallback: "");

    #endregion

    #region Advanced
    public string RazorFile => String(fallback: "");
    #endregion

    public bool LabelStartsNewGroup => Bool();

  }
}