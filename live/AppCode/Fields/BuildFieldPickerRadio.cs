using System.Linq;
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

    private IHtmlTag RadioOriginal()
    {
      var tags = Builder.Kit.HtmlTags;
      var items = tags.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = tags.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var itemsLabel = items.Add(inputLabels);

      var container = tags.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var radioId = GeneratedHtmlId(item);
        var radio = tags.Input().Type("radio").Name(Field.FieldId).Value(item.Key);  // Name is the same for all radios in the group
        radio = SetBasics(radio, false, radioId);

        var wrapper = tags.Div().Class(CssClasses.RadioWrapper);
        if (CssClasses.IsBs3)
        {
          var radioLabel = tags.Label(radio + item.Value).For(radioId);
          wrapper = wrapper.Add(radioLabel);
        }
        else
        {
          radio = radio.Class(Constants.ClassCheckbox);
          var radioLabel = tags.Label(item.Value).Class("form-check-label").For(radioId);
          wrapper = wrapper.Add(radio, radioLabel); // <-- will be needed as well
        }

        container = container.Add(wrapper);
      }
      
      if (Field.IsNotEmpty("InfoText"))
        container = container.Add(tags.Div(Field.InfoText).Class("small-infotext"));
      
      var itemsContainer = itemsLabel.Add(container);
      return itemsContainer;
    }

    private IHtmlTag Radio()
    {
      var tags = Builder.Kit.HtmlTags;
      // Generate all item wrappers, functional
      var radioButtons = GetKeyValue(Field.PickerKeyValues)
        .Select(item =>
        {
          var radioId = GeneratedHtmlId(item);
          var radio = tags.Input().Type("radio")
            .Name(Field.FieldId)    // Name is the same for all radios in the group
            .Value(item.Key)
            .Class(CssClasses.Checkbox);
          radio = SetBasics(radio, false, radioId);

          // BS4+ has the radio before the label, BS3 has it inside the label
          var radioLabel = tags.Label(CssClasses.IsBs3 ? radio : null, item.Value)
            .Class(CssClasses.RadioLabel)
            .For(radioId);

          var wrapper = tags.Div(CssClasses.IsBs3 ? null : radio, radioLabel)
            .Class(CssClasses.RadioWrapper);
          return wrapper;
        })
        .ToList();

      var container = tags.Div(radioButtons)
        .Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside)
        .Add(Field.IsEmpty("InfoText") ? null : tags.Div(Field.InfoText).Class("small-infotext"));
      
      var items = tags.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = tags.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var itemsLabel = items.Add(inputLabels);
      var itemsContainer = itemsLabel.Add(container);
      return itemsContainer;
    }
  }

}