namespace ThisApp.Code
{
  public class CssClasses
  {
    /// <summary>
    /// Info if the set of classes is for Bootstrap 3 4 or 5.
    /// Relevant for scenarios where the code will have to do more than just set a class
    /// </summary>
    public bool IsBs3 { get; set; } = false;
    public bool IsBs5 { get; set; } = false;

    public string OutsideDiv { get; set; }
    public string InputControl { get; set; } = "form-control";
    public string LabelOutside { get; set; }
    public string LabelInside { get; set; }
    public string Wrapper { get; set; }
    public string RadioWrapper { get; set; }
    public string CheckboxWrapper { get; set; }
    public string FloatingLabelHidden { get; set; }


    public string Label { get; set; }

    public static CssClasses Bs5 = new CssClasses()
    {
      IsBs3 = false,
      OutsideDiv = " row mb-3",
      Label = "col-12 col-md-3",
      LabelOutside = "col-12 col-sm-9",
      LabelInside = "",
      Wrapper = "mb-3",
      RadioWrapper = "form-check",
      CheckboxWrapper = "form-check",
      FloatingLabelHidden = "",
    };

    public static CssClasses Bs4 = new CssClasses()
    {
      // Same as Bs5
      IsBs3 = false,

      OutsideDiv = " row mb-3",
      Label = "col-12 col-md-3",
      LabelOutside = "col-12 col-sm-9",
      LabelInside = "",
      Wrapper = "mb-3",
      RadioWrapper = "form-check",
      CheckboxWrapper = "form-check",
      //
      FloatingLabelHidden = "d-none",
    };

    public static CssClasses Bs3 = new CssClasses()
    {
      IsBs3 = true,
      OutsideDiv = "row form-group",
      Label = "col col-xs-12 col-sm-3",
      LabelOutside = "col col-xs-12 col-sm-9",
      LabelInside = "",
      Wrapper = "form-group",
      RadioWrapper = "radio",
      CheckboxWrapper = "checkbox",
      FloatingLabelHidden = "hidden",
    };

  }
}