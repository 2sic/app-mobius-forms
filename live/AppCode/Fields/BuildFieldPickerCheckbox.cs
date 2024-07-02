using System.Collections.Generic;
using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using System.Linq;

namespace AppCode.Fields
{
  public class BuildFieldPickerCheckbox : BuildFieldPicker
  {
    public BuildFieldPickerCheckbox(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Generate CheckboxPicker with Headline or without
    /// </summary>
    public override IHtmlTag GetTag() => CheckBoxPicker();

    private IHtmlTag CheckBoxPicker()
    {
      if (Field.PickerCheckboxGrouped) return CheckboxPickerWithHeadline();
      return CheckboxPickerBasic();
    }
    // Simple CheckboxPicker List without Headline
    private IHtmlTag CheckboxPickerBasic()
    {
      var tag = Builder.Kit.HtmlTags;

      // Generate all item wrappers, functional
      var checkboxes = GetKeyValue(Field.PickerKeyValues)
          .Select(item =>
          {
            var checkboxId = GeneratedHtmlId(item);
            var checkbox = GenerateCheckbox(item);
            var wrapper = tag.Div(checkbox).Class(CssClasses.CheckboxWrapper);

            // Create the label for the checkbox
            var label = tag.Label(item.Value)
              .Class(LabelClasses(Field.Required))
              .For(checkboxId);

            // Create container and add checkbox or wrapper based on Bootstrap version
            var container = Tag.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);
            container.Add(CssClasses.IsBs3 ? checkbox : wrapper);

            // Create items div and add label and container
            var items = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
            items.Add(label, container);

            return items;
          });

      // Create div and add all items
      return tag.Div().Add(checkboxes);
    }

    // Simple CheckboxPicker List with Headline (Title) Left
    private IHtmlTag CheckboxPickerWithHeadline()
    {
      var tag = Builder.Kit.HtmlTags;

      // Generate all item wrappers, functional
      var checkboxes = GetKeyValue(Field.PickerKeyValues)
          .Select(item =>
          {
            var checkboxId = GeneratedHtmlId(item);
            var checkbox = tag.Input().Type("checkbox")
              .Name(Field.FieldId)
              .Value(item.Key)
              .Class(CssClasses.Checkbox);
            checkbox = SetBasics(checkbox, false, checkboxId);

            // Create the label for the checkbox
            var label = tag.Label(item.Value)
              .Class(LabelClasses(Field.Required))
              .For(checkboxId);

            // BS4+ has the checkbox before the label, BS3 has it inside the label
            var wrapper = Tag.Div()
              .Class(CssClasses.CheckboxWrapper)
              .Add(CssClasses.IsBs3 ? (IHtmlTag)label.Add(checkbox) : checkbox, label);

            return wrapper;
          })
          .ToList();

      // Create container for all checkboxes
      var container = Tag.Div()
          .Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);

      // Add each checkbox wrapper to the container
      foreach (var checkbox in checkboxes)
        container.Add(checkbox);

      // Add optional info text if available
      if (!Field.IsEmpty("InfoText"))
        container.Add(tag.Div(Field.InfoText).Class("small-infotext"));

      // Create the outside div with label and container
      var items = tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = tag.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var itemsLabel = items.Add(inputLabels);
      var itemsContainer = itemsLabel.Add(container);

      return itemsContainer;
    }
    private IHtmlTag GenerateCheckbox(KeyValuePair<string, string> item)
    {
      var tag = Builder.Kit.HtmlTags;
      var checkbox = tag.Input().Type("checkbox").Name(Field.FieldId).Value(item.Key).Class(Constants.ClassCheckbox).Attr("data-checkbox", Field.FieldId);
      checkbox = SetBasics(checkbox, false, GeneratedHtmlId(item));
      return checkbox;
    }
  }
}