using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldTerms : BuildFieldCheckbox
  {
    public BuildFieldTerms(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Terms checkbox use Title form App Resources and overrideTitle in CheckboxWithLabelRight
    /// </summary>
    public override IHtmlTag GetTag()
    {
      // Get a checkbox and mark it as a terms checkbox (for JavaScript validation)
      var checkbox = GetCheckbox().Attr("terms", "true");

      // Get correct title based on the purpose of field
      var res = Form.FormResources;
      string title = Field.TermsAndGdprCombined
        ? res.LabelTermsAll
        : Field.TermsEnabled
          ? res.LabelTerms
          : Field.GdprEnabled
            ? res.LabelGdpr
            : default;

      return CheckboxWithLabelRight(checkbox, title);
    }
  }
}