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
      if (Field.TermsAndGdprCombined) overrideTitle = Resources.LabelTermsAll;
      else if (Field.TermsEnabled) overrideTitle = Resources.LabelTerms;
      else if (Field.GdprEnabled) overrideTitle = Resources.LabelGdpr;

      return CheckboxWithLabelRight(checkbox, overrideTitle);
    }
  }
}