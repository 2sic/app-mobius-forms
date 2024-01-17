using System.Collections.Generic;
using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldPickerRadio : BuildFieldPicker
  {
    public BuildFieldPickerRadio(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => Radio();

    private IHtmlTag Radio()
    {
      var items = Tag.Div().Class("app-mobius5-form-fields row mb-3");
      var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(CssClasses.Label);
      items.Add(inputLabels);

      var container = Tag.Div().Class(CssClasses.LabelOutside);

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var radioId = Field.FieldId + item.Value.ToLower().Replace(" ", "");
        var radio = Tag.Input().Type("radio").Name(Field.FieldId).Value(item.Key);  // Name is the same for all radios in the group
        radio = SetBasics(radio, false);

        var wrapper = Tag.Div().Class(CssClasses.RadioWrapper);
        if (CssClasses.IsBs3)
        {
          var radioLabel = Tag.Label(radio + item.Value).For(radioId);
          wrapper.Add(radioLabel);
        }
        else
        {
          radio.Class(Constants.ClassCheckbox);
          var radioLabel = Tag.Label(item.Value).Class("form-check-label").For(radioId);
          wrapper.Add(radio, radioLabel);
        }

        container.Add(wrapper);
      }

      items.Add(container);

      return items;
    }

  }
}