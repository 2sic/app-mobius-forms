
using ToSic.Razor.Blade;
public class Helpers: Custom.Hybrid.Code12
{
  public string WrapperClasses(dynamic data) { 
    return "app-mobius5-wrapper" + (data.Mailchimp ?? false ? " app-mobius5-mailchimp" : "");
  }
}