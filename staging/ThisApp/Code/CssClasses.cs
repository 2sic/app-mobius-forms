namespace ThisApp.Code
{
  public class CssClasses
  {
    public string InputControl { get; set; } = "form-control";
    public string LabelOutside { get; set; }
    public string LabelInside {get;set;}
    public string Wrapper {get;set;}

    public string Label {get;set;}

    public static CssClasses Bs5 = new CssClasses()
    {
      Label = "col-12 col-md-3",
      LabelOutside = "col-12 col-sm-9",
      LabelInside = "",
      Wrapper = "mb-3",
    };

    public static CssClasses Bs3 = new CssClasses()
    {
      Label = "col col-xs-12 col-sm-3",
      LabelOutside = "col col-xs-12 col-sm-9",
      LabelInside = "",
      Wrapper = "form-group",
    };

  }
}