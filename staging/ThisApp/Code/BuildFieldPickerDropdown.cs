using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldPickerDropdown : BuildFieldPicker
  {
    public BuildFieldPickerDropdown(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Generate Dropdown with multiple options
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var item = Dropdown();
      item = SetBasicsDropdown(item);
      return WrapInLabel(item);
    }

    private Select Dropdown()
    {
      var dropdown = Tag.Select().Class(CssClasses.InputControl);

      if (Field.MultiSelect) { dropdown.Multiple().Attr("data-multiple-dropdown", Field.FieldId); }

      dropdown.Add(Tag.Option(Text.First(Field.PlaceHolderSelect, Resources.LabelSelect)));

      foreach (var optionItem in GetKeyValue(Field.PickerKeyValues))
        dropdown.Add(Tag.Option(optionItem.Value).Value(optionItem.Key));

      return dropdown;
    }

    protected Select SetBasicsDropdown(Select item)
    {
      var result = item
        .Id(Field.FieldId)
        .Class(CssClasses.InputControl);

      if (Field.Required) result = SetRequired(result);
      if (Field.IsDisabled) result = result.Disabled();
      return result;
    }

  }
}