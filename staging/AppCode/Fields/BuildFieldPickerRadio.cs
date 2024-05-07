using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldPickerRadio : BuildFieldPicker
  {
    public BuildFieldPickerRadio(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Generate simple Radio 
    /// </summary>
    public override IHtmlTag GetTag() => Radio();

    private IHtmlTag Radio()
    {
      // TODO::
      var tag = Builder.Kit.HtmlTags;
      var items = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = tag.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      items.Add(inputLabels);

      var container = Tag.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var radioId = GeneratedHtmlId(item);
        var radio = Tag.Input().Type("radio").Name(Field.FieldId).Value(item.Key);  // Name is the same for all radios in the group
        radio = SetBasics(radio, false, radioId);

        var wrapper = Tag.Div().Class(CssClasses.RadioWrapper);
        if (CssClasses.IsBs3)
        {
          var radioLabel = tag.Label(radio + item.Value).For(radioId);
          wrapper.Add(radioLabel);
        }
        else
        {
          radio.Class(Constants.ClassCheckbox);
          var radioLabel = tag.Label(item.Value).Class("form-check-label").For(radioId);
          wrapper.Add(radio, radioLabel);
        }

        container.Add(wrapper);
      }
      
      if (Field.IsNotEmpty("InfoText"))
        container.Add(tag.Div(Field.InfoText).Class("small-infotext"));
      
      items.Add(container);
      return items;
    }
  }
}