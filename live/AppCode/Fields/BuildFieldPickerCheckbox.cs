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
      var tags = Builder.Kit.HtmlTags;

      // Generate all item wrappers, functional
      var checkboxes = GetKeyValue(Field.PickerKeyValues)
          .Select(item =>
          {
            var checkboxId = GeneratedHtmlId(item);
            var checkbox = GenerateCheckbox(item);
            var wrapper = BuildWrapper(checkbox, BuildLabel(item.Value, checkboxId));

            // Create container and add checkbox or wrapper based on Bootstrap version
            var container = tags.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);
            container = container.Add(CssClasses.IsBs3 ? checkbox : wrapper);

            // Create items div and add label and container
            var items = tags.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
            items = items.Add(BuildLabel(Field.Title, Field.FieldId, Field.Required), container);

            return items;
          });

      return tags.Div().Add(checkboxes);
    }

    // Simple CheckboxPicker List with Headline (Title) Left
    private IHtmlTag CheckboxPickerWithHeadline()
    {
      var tags = Builder.Kit.HtmlTags;

      // Generate all item wrappers, functional
      var wrappers = GetKeyValue(Field.PickerKeyValues)
          .Select(item =>
          {
            var checkboxId = GeneratedHtmlId(item);
            var checkbox = GenerateCheckbox(item);
            var label = BuildLabel(item.Value, checkboxId);
            return BuildWrapper(checkbox, label);
          })
          .ToList();

      // Create container for all checkboxes
      var container = tags.Div().Class(Form.UseFloatingLabels ? "col-12" : CssClasses.LabelOutside);

      foreach (var wrapper in wrappers)
        container = container.Add(wrapper);

      // Add optional info text if available
      if (!Field.IsEmpty("InfoText"))
        container = container.Add(tags.Div(Field.InfoText).Class("small-infotext"));

      // Create the outside div with label and container
      var items = tags.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);
      var inputLabels = BuildLabel(Field.Title, Field.FieldId, Field.Required);
      var itemsLabel = items.Add(inputLabels);
      var itemsContainer = itemsLabel.Add(container);

      return itemsContainer;
    }

    private IHtmlTag GenerateCheckbox(KeyValuePair<string, string> item)
    {
      var tags = Builder.Kit.HtmlTags;
      var id = GeneratedHtmlId(item);
      var checkbox = tags.Input()
        .Type("checkbox")
        .Name(Field.FieldId)
        .Value(item.Key)
        .Class(Constants.ClassCheckbox)
        .Attr("data-checkbox", Field.FieldId);

      checkbox = SetBasics(checkbox, false, id);
      return checkbox;
    }

    // Helper to build a label consistently
    private IHtmlTag BuildLabel(string text, string forId, bool required = false)
    {
      var tags = Builder.Kit.HtmlTags;
      return tags.Label(text)
        .Class(LabelClasses(required))
        .For(forId);
    }

    // Helper to build a wrapper handling BS3 vs BS4+ ordering
    private IHtmlTag BuildWrapper(IHtmlTag checkbox, IHtmlTag label)
    {
      var tags = Builder.Kit.HtmlTags;
      var wrapper = tags.Div().Class(CssClasses.CheckboxWrapper);

      // BS4+ has the checkbox before the label, BS3 has it inside the label
      var content = CssClasses.IsBs3 ? (IHtmlTag)label.Add(checkbox) : checkbox;
      wrapper = wrapper.Add(content, label);

      return wrapper;
    }
  }
}