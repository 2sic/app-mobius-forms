using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldCheckbox : BuildFieldBase
  {
    public BuildFieldCheckbox(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Get a checkbox with Left or Right Label
    /// </summary>
    public override IHtmlTag GetTag()
    {

      // If label right or floating labels, wrap in label
      if (Field.BooleanLabelRight || Form.UseFloatingLabels)
        return CheckboxWithLabelRight(GetCheckbox());

      // If label is to the left, behave as default
      return SetBasicsAndWrapInLabel(GetCheckbox(), setDefaultClass: false);
    }

    /// <summary>
    /// Just generate the checkbox. In standard use this is all we need
    /// </summary>
    protected Input GetCheckbox()
    {
      var tags = Builder.Kit.HtmlTags;
      return tags.Input()
        .Type("checkbox")
        .Id(Field.FieldId)
        .Name(Field.FieldId)
        .Class(CssClasses.FormCheckInput);
    }

    /// <summary>
    /// Generate a Checkbox with a label to the right (for long text (terms etc.))
    /// </summary>
    protected IHtmlTag CheckboxWithLabelRight(Input checkbox, string overrideTitle = default)
    {
      var checkboxLabel = Text.First(overrideTitle, Field.Title, Field.FieldId);

      checkbox = checkbox.Value(checkboxLabel);
      checkbox = SetBasics(checkbox, false);

      var tags = Builder.Kit.HtmlTags;
      var checkboxWrapper = tags.Div();

      if (CssClasses.IsBs3) {
        checkboxWrapper = checkboxWrapper.Class($"{Constants.ClassMobiusField} form-group").Wrap(
          tags.Div().Class("checkbox").Wrap(
            tags.Label().Wrap(checkbox, checkboxLabel)
          )
        );
        
        if (Field.IsNotEmpty("InfoText"))
          checkboxWrapper = checkboxWrapper.Add(tags.Div(Field.InfoText).Class("small-infotext"));        

        return checkboxWrapper;
      }

      checkboxWrapper = checkboxWrapper
        .Class($"{Constants.ClassMobiusField} mb-3 form-check")
        .Wrap(
          checkbox,
          tags.Label(checkboxLabel.Replace("<p>", "").Replace("</p>", ""))
            .Class("form-check-label")
            .Class(Field.Required ? Constants.ClassRequired : "")
            .For(Field.FieldId)
        );

      if (Field.IsNotEmpty("InfoText"))
        checkboxWrapper = checkboxWrapper.Add(tags.Div(Field.InfoText).Class("small-infotext"));

      return checkboxWrapper;
    }
  }
}