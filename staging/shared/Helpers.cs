
using ToSic.Razor.Blade;
public class Helpers: Custom.Hybrid.Code12
{
    public string WrapperClasses(dynamic data) {
        return "mobius-wrapper sc-element" + (data.Mailchimp ?? false ? "mobius-mailchimp" : "") + (data.Recaptcha ?? false ? "mobius-recaptcha" : "");
    }
}