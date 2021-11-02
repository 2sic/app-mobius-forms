
using ToSic.Razor.Blade;
public class Helpers: Custom.Hybrid.Code12
{
  // TODO: @2mh 
  // - DO WE still need the recaptcha class? I believe it's invisible now
  // - do we really need the mailchimp class? does it do anything?
  public string WrapperClasses(dynamic data) {
      return "app-mobius5-wrapper" + " " + (data.Mailchimp ?? false ? "app-mobius5-mailchimp" : "") + " " + (data.Recaptcha ?? false ? "app-mobius5-recaptcha" : "");
  }
}