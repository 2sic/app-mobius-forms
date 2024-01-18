using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldPickerCheckbox : BuildFieldPicker
  {
    public BuildFieldPickerCheckbox(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => CheckBoxPicker();

    private IHtmlTag CheckBoxPicker()
    {
      if (Field.CheckboxWithHeadline) return CheckboxPickerWithHeadline();

      return CheckboxPickerBasic();
    }

    private IHtmlTag CheckboxPickerBasic()
    {
      var div = Tag.Div();

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var items = Tag.Div().Class(CssClasses.OutsideDiv);

        var wrapper = Tag.Div(GenerateCheckbox(item)).Class(CssClasses.CheckboxWrapper);
        var container = Tag.Div(wrapper).Class(CssClasses.LabelOutside);
        var label = Tag.Label(item.Value).Class(CssClasses.Label).For(GenearateHtmlId(item));

        items.Add(label, container);
        div.Add(items);
      }
      return div;
    }

    private IHtmlTag CheckboxPickerWithHeadline()
    {

      var items = Tag.Div().Class(CssClasses.OutsideDiv); // same
      var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(CssClasses.Label);
      items.Add(inputLabels);
      var container = Tag.Div().Class(CssClasses.LabelOutside); // same

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var checkbox = GenerateCheckbox(item);
        var wrapper = Tag.Div().Class(CssClasses.CheckboxWrapper); // same

        if (CssClasses.IsBs3)
        {
          var radioLabel = Tag.Label(checkbox + item.Value).For(GenearateHtmlId(item));
          wrapper.Add(radioLabel);
        }
        else
        {
          checkbox.Class(Constants.ClassCheckbox);
          var radioLabel = Tag.Label(item.Value).Class("form-check-label").For(GenearateHtmlId(item));
          wrapper.Add(checkbox, radioLabel);
        }
        container.Add(wrapper);
      }
      items.Add(container);

      return items;
    }

    private IHtmlTag GenerateCheckbox(KeyValuePair<string, string> item)
    {

      var checkboxId = GenearateHtmlId(item);
      var checkbox = Tag.Input().Type("checkbox").Name(Field.FieldId).Value(item.Key).Class(Constants.ClassCheckbox);
      checkbox = SetBasics(checkbox, false, checkboxId);
      return checkbox;
    }

  }
}

