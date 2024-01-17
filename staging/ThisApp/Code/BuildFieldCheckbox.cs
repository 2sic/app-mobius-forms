using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldCheckbox : BuildFieldBase
  {
    public BuildFieldCheckbox(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Text must override it, because the Checkbox is not an Text input
    /// </summary>
    public override IHtmlTag GetTag()
    {

      if (Field.TermsAndGdprCombined || Field.TermsEnabled || Field.GdprEnabled)
        return CheckboxWithLabelRight();

      // If label is to the left, behave as default
      if (!Field.LabelRight) return SetBasicsAndWrapInLabel(GetCheckbox());

      // If label right 
      return CheckboxWithLabelRight();
    }

    /// <summary>
    /// Just generate the checkbox. In standard use this is all we need
    /// </summary>
    private Input GetCheckbox()
    {
      // TODO: check size / class "form-control"
      var checkbox = Tag.Input().Attr("type", "checkbox").Id(Field.FieldId).Class(Constants.ClassCheckbox);
      return checkbox;
    }

    private IHtmlTag CheckboxWithLabelRight()
    {

      var checkbox = GetCheckbox();
      string overrideTitle = null;

      if (Field.TermsAndGdprCombined || Field.TermsEnabled || Field.GdprEnabled)
      {
        checkbox.Attr("terms", "true");
        if (Field.TermsAndGdprCombined) overrideTitle = Resources.LabelTermsAll;
        else if (Field.TermsEnabled)  overrideTitle = Resources.LabelTerms;
        else if (Field.GdprEnabled) overrideTitle = Resources.LabelGdpr;
      }

      var checkboxLabel = Text.First(overrideTitle, Field.Title, Field.FieldId) + (Field.Required ? "*" : "");

      checkbox = checkbox.Value(checkboxLabel);
      checkbox = SetBasics(checkbox);

      if (CssClasses.IsBs3)
        return Tag.Div().Class($"{Constants.ClassMobiusField} form-group").Wrap(
          Tag.Div().Class("checkbox").Wrap(
            Tag.Label().Wrap(checkbox, checkboxLabel)
          )
        );

      return Tag.Div().Class($"{Constants.ClassMobiusField} mb-3 form-check").Wrap(
        checkbox,
        Tag.Label(checkboxLabel).Class("form-check-label").For(Field.FieldId)
      );
    }
  }
}