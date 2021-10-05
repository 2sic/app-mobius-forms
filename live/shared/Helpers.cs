
using ToSic.Razor.Blade;
public class Helpers: Custom.Hybrid.Code12
{
    public string WrapperClasses(dynamic data) {
        return "mobius-wrapper app-mobius-" + CmsContext.Module.Id + " " + (data.Mailchimp ?? false ? "mobius-mailchimp" : "") + " " + (data.Recaptcha ?? false ? "mobius-recaptcha" : "");
    }
}