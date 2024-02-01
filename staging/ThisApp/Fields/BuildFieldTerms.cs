using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;

namespace ThisApp.Fields
{
  public class BuildFieldTerms : BuildFieldCheckbox
  {
    public BuildFieldTerms(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Terms checkbox use Title form App Resources and overriedTitle in CheckbocxWithLabelRight
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var checkbox = GetCheckbox();
      string overrideTitle = default;
      checkbox.Attr("terms", "true");
      if (Field.TermsAndGdprCombined) overrideTitle = Form.FormResources.LabelTermsAll;
      else if (Field.TermsEnabled) overrideTitle = Form.FormResources.LabelTerms;
      else if (Field.GdprEnabled) overrideTitle = Form.FormResources.LabelGdpr;

      return CheckboxWithLabelRight(checkbox, overrideTitle);
    }
  }
}