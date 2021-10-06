
using ToSic.Razor.Blade;
public class Helpers: Custom.Hybrid.Code12
{
    public string WrapperClasses(dynamic data) {
        return "app-mobius5-wrapper app-mobius5-js-" + CmsContext.Module.Id + " " + (data.Mailchimp ?? false ? "app-mobius5-mailchimp" : "") + " " + (data.Recaptcha ?? false ? "app-mobius5-recaptcha" : "");
    }
}