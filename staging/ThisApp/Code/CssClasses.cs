namespace ThisApp.Code
{
  public class CssClasses
  {
    /// <summary>
    /// Info if the set of classes is for Bootstrap 3 or 5.
    /// Relevant for scenarios where the code will have to do more than just set a class
    /// </summary>
    public bool IsBs3 { get; set; } = false;
    public string InputControl { get; set; } = "form-control";
    public string LabelOutside { get; set; }
    public string LabelInside { get; set; }
    public string Wrapper { get; set; }

    public string Label { get; set; }

    public static CssClasses Bs5 = new CssClasses()
    {
      IsBs3 = false,
      Label = "col-12 col-md-3",
      LabelOutside = "col-12 col-sm-9",
      LabelInside = "",
      Wrapper = "mb-3",
    };

    public static CssClasses Bs3 = new CssClasses()
    {
      IsBs3 = true,
      Label = "col col-xs-12 col-sm-3",
      LabelOutside = "col col-xs-12 col-sm-9",
      LabelInside = "",
      Wrapper = "form-group",
    };

  }
}