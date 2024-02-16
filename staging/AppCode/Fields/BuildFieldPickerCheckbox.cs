using System.Collections.Generic;
using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldPickerCheckbox : BuildFieldPicker
  {
    public BuildFieldPickerCheckbox(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Generate CheckbocPicker with Headline or without
    /// </summary>
    public override IHtmlTag GetTag() => CheckBoxPicker();
    // public override IHtmlTag GetTag() {
    //   return CheckBoxPicker();
    // }

    private IHtmlTag CheckBoxPicker()
    {
      if (Field.PickerCheckboxGrouped) return CheckboxPickerWithHeadline();
      return CheckboxPickerBasic();
    }
    // Simple CheckboxPicker List without Headline
    private IHtmlTag CheckboxPickerBasic()
    {
      var div = Tag.Div();

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var items = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
        var checkbox = GenerateCheckbox(item);
        var wrapper = Tag.Div(checkbox).Class(CssClasses.CheckboxWrapper);

        var container = Tag.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);
        var label = Tag.Label(item.Value).Class(LabelClasses(Field.Required)).For(GenearateHtmlId(item));

        if (CssClasses.IsBs3) container.Add(checkbox);
        else container.Add(wrapper);

        items.Add(label, container);
        div.Add(items);
      }
      return div;
    }
    // Simple CheckboxPicker List with Headline (Title) Left
    private IHtmlTag CheckboxPickerWithHeadline()
    {
      var items = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      items.Add(inputLabels);
      var container = Tag.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);

      foreach (var item in GetKeyValue(Field.PickerKeyValues))
      {
        var checkbox = GenerateCheckbox(item);
        var wrapper = Tag.Div().Class(CssClasses.CheckboxWrapper);
        if (CssClasses.IsBs3)
        {
          var checkboxLabel = Tag.Label(checkbox + item.Value).For(GenearateHtmlId(item));
          wrapper.Add(checkboxLabel);
        }
        else
        {
          checkbox.Class(Constants.ClassCheckbox);
          var checkboxLabel = Tag.Label(item.Value).Class("form-check-label").For(GenearateHtmlId(item));
          wrapper.Add(checkbox, checkboxLabel);
        }
        container.Add(wrapper);
      }
      items.Add(container);

      return items;
    }
    private IHtmlTag GenerateCheckbox(KeyValuePair<string, string> item)
    {
      var checkbox = Tag.Input().Type("checkbox").Name(Field.FieldId).Value(item.Key).Class(Constants.ClassCheckbox).Attr("data-checkbox", Field.FieldId);
      checkbox = SetBasics(checkbox, false, GenearateHtmlId(item));
      return checkbox;
    }
  }
}