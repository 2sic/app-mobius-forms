using DotNetNuke.Services.FileSystem;
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

      // If label right or floating labels, wrap in label
      if (Field.LabelRight || Form.UseFloatingLabels) return CheckboxWithLabelRight(GetCheckbox());

      // If label is to the left, behave as default
      return SetBasicsAndWrapInLabel(GetCheckbox(), setDefaultClass: false);
    }

    /// <summary>
    /// Just generate the checkbox. In standard use this is all we need
    /// </summary>
    protected Input GetCheckbox()
    {
      var checkbox = Tag.Input().Type("checkbox").Id(Field.FieldId).Class(Constants.ClassCheckbox);
      return checkbox;
    }

    protected IHtmlTag CheckboxWithLabelRight(Input checkbox, string overrideTitle = default)
    {
      var checkboxLabel = Text.First(overrideTitle, Field.Title, Field.FieldId) + (Field.Required ? "*" : "");

      checkbox = checkbox.Value(checkboxLabel);
      checkbox = SetBasics(checkbox, false);

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