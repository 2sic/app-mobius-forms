using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using System.Linq;

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
      var tag = Builder.Kit.HtmlTags;

      // Create the dropdown element
      var dropdown = Tag.Select().Class(CssClasses.InputControl);

      if (Field.PickerMultiSelect) // Add the multiple attribute if necessary
        dropdown.Multiple().Attr("data-multiple-dropdown", Field.FieldId);


      // Add an empty option as a placeholder
      dropdown.Add(tag.Option(Text.First(Field.PickerPlaceholder, Form.FormResources.LabelSelect)).Value(""));

      // Add all options from PickerKeyValues
      var options = GetKeyValue(Field.PickerKeyValues)
          .Select(optionItem =>
              tag.Option(optionItem.Value).Value(optionItem.Key)
          );
          
      return dropdown.Add(options);
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