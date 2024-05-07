using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldPickerDropdown : BuildFieldPicker
  {
    public BuildFieldPickerDropdown(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Generate Dropdown with multiple options
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var tag = Builder.Kit.HtmlTags;
      var item = Dropdown();
      item = SetBasicsDropdown(item);

      if (Field.IsNotEmpty("InfoText"))
        item.Add(tag.Div(Field.InfoText).Class("small-infotext"));
        
      return WrapInLabel(item);
    }

    private Select Dropdown()
    {
      // TODO::
      var tag = Builder.Kit.HtmlTags;
      var dropdown = Tag.Select().Class(CssClasses.InputControl);

      if (Field.PickerMultiSelect) { dropdown.Multiple().Attr("data-multiple-dropdown", Field.FieldId); }

      dropdown.Add(tag.Option(Text.First(Field.PickerPlaceholder, Form.FormResources.LabelSelect)).Value(""));

      foreach (var optionItem in GetKeyValue(Field.PickerKeyValues))
        dropdown.Add(tag.Option(optionItem.Value).Value(optionItem.Key));

      return dropdown;
    }

    protected Select SetBasicsDropdown(Select item)
    {
      var result = item
        .Id(Field.FieldId)
        .Class(CssClasses.FormSelect);

      if (Field.Required) result = SetRequired(result);
      if (Field.IsDisabled) result = result.Disabled();
      return result;
    }
  }
}