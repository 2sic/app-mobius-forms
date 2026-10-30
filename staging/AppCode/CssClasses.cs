namespace AppCode
{
  public class CssClasses
  {
    /// <summary>
    /// Bootstrap 5 classes used by the form builders.
    /// </summary>
    public string OutsideDiv { get; set; }
    public string InputControl { get; set; } = "form-control";
    public string LabelOutside { get; set; }
    public string LabelInside { get; set; }
    public string Wrapper { get; set; }
    public string RadioWrapper { get; set; }
    public string RadioLabel { get; set; }
    public string Checkbox { get; set; }
    public string CheckboxWrapper { get; set; }
    public string FloatingLabelHidden { get; set; }
    public string FormCheckInput { get; set; }
    public string FormSelect { get; set; }
    public string HiddenInputStyle { get; set; }

    public string Label { get; set; }

    public static CssClasses Bs5 = new CssClasses()
    {
      OutsideDiv = " row mb-3",
      Label = "col-12 col-md-3",
      LabelOutside = "col-12 col-sm-9",
      LabelInside = "",
      Wrapper = "mb-3",
      RadioWrapper = "form-check",
      RadioLabel = "form-check-label",
      Checkbox = "form-check-input",
      CheckboxWrapper = "form-check",
      FloatingLabelHidden = "",
      FormCheckInput = "form-check-input",
      FormSelect = "form-select",
      HiddenInputStyle = "hiddenInputStyle",
    };
  }
}
