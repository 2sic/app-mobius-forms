using System.Collections.Generic;
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

    // private IHtmlTag CheckBoxPicker()
    // {
    //   var checkboxes = new List<IHtmlTag>();

    //   foreach (var tempItem in GetKeyValue(Field.PickerKeyValues))
    //   {
    //     var checkboxId = Field.FieldId + tempItem.Value.ToLower().Replace(" ", "");
    //     var wrapper = Tag.Div().Class(CssClasses.RadioWrapper);
    //     var checkbox = Tag.Input().Type("checkbox").Name(Field.FieldId).Value(tempItem.Key);
    //     checkbox = SetBasics(checkbox, false);

    //     if (CssClasses.IsBs3)
    //     {
    //       var checkboxLabel = Tag.Label(checkbox + "").For(checkboxId);
    //       wrapper.Add(checkboxLabel);
    //     }
    //     else
    //     {
    //       checkbox.Class(Constants.ClassCheckbox).Attr("data-checkbox", Field.FieldId);
    //       var checkboxLabel = Tag.Label("").Class("form-check-label").For(checkboxId);
    //       wrapper.Add(checkbox + checkboxLabel);
    //     }

    //     checkboxes.Add(checkbox);
    //   }

    //   return Tag.Div().Add(checkboxes);
    // }


   private IHtmlTag CheckBoxPicker()
{
    var checkboxes = new List<IHtmlTag>();

    foreach (var tempItem in GetKeyValue(Field.PickerKeyValues))
    {
        var checkboxId = Field.FieldId + tempItem.Value.ToLower().Replace(" ", "");
        var wrapper = Tag.Div().Class(CssClasses.RadioWrapper);
        var checkbox = Tag.Input().Type("checkbox").Name(Field.FieldId).Value(tempItem.Key);
        checkbox = SetBasics(checkbox, false);
        // Todo:: Reihenfolge anpassen
        var label = Tag.Label(tempItem.Value).Class("form-check-label").For(checkboxId);
        var container = Tag.Div().Class("col-12 col-sm-9");
        
        wrapper.Add(checkbox, label);
        container.Add(wrapper);
        checkboxes.Add(container);
    }

    return Tag.Div().Add(checkboxes);
}

    // var items = Tag.Div().Class("app-mobius5-form-fields row mb-3");
    // var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(CssClasses.Label);
    // items.Add(inputLabels);

    // var container = Tag.Div().Class(CssClasses.LabelOutside);

    // foreach (var item in GetKeyValue(Field.PickerKeyValues))
    // {
    //   var radioId = Field.FieldId + item.Value.ToLower().Replace(" ", "");
    //   var radio = Tag.Input().Type("radio").Name(Field.FieldId).Value(item.Key);  // Name is the same for all radios in the group
    //   radio = SetBasics(radio, false);

    //   var wrapper = Tag.Div().Class(CssClasses.RadioWrapper);
    //   if (CssClasses.IsBs3)
    //   {
    //     var radioLabel = Tag.Label(radio + item.Value).For(radioId);
    //     wrapper.Add(radioLabel);
    //   }
    //   else
    //   {
    //     radio.Class(Constants.ClassCheckbox);
    //     var radioLabel = Tag.Label(item.Value).Class("form-check-label").For(radioId);
    //     wrapper.Add(radio, radioLabel);
    //   }

    //   container.Add(wrapper);
    // }

    // items.Add(container);

    // return items;

  }
}