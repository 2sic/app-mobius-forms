using System;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using System.Collections.Generic;
using ToSic.Sxc.Blocks;
using System.CodeDom;
using System.Runtime.Remoting.Messaging;
using ToSic.Eav.Data;

public class FieldBuilders : Custom.Hybrid.CodeTyped
{

  // handles the visibility of a label or a placeholder
  public bool LabelInPlaceholder = false;

  #region Koi based class selection

  // returns form validation class 
  private string FormValidationClass()
  {
    return "app-mobius5-form-fields ";
  }
  // returns form-classes based on whether label is shown as placeholder or besides form - as row  
  private string FormClasses()
  {
    return FormValidationClass()
      + (LabelInPlaceholder ? "" : "row ")
      + (Kit.Css.Is("bs3") ? "form-group" : "mb-3");
  }

  // Choose CSS classes based on the framework
  // if you customize this, you probably know what css framework you want,
  // in which case you can skip framework detection and just write the classes

  // Choose CSS classes for the labels
  private string LabelClasses(bool required)
  {
    return "control-label "
      + (required ? "app-mobius5-field-required " : "")
      + (Kit.Css.Is("bs3") ? "col col-xs-12 col-sm-3" : "col-12 col-md-3");
  }

  #endregion

  // Add a placeholder text to the inputs
  private string PhLabel(string label, bool required)
  {
    return LabelInPlaceholder ? label + (required ? " *" : "") : ""; ;
  }

  // Sets a RazorBlade Input/TextArea to required and adds the message which is different for each field type
  internal void SetRequired(IHtmlTag item, bool required, string key = null)
  {
    if (!required) return;

    var message = key != null
      ? App.Resources.String(key)
      : App.Resources.String("LabelRequired");

    item.Attr("data-pristine-required-message", message).Attr("required");
  }
  // Cache the message after first lookup for performance as we use it quite ofter
  private string MessageRequired = null;


  // returns a Label
  public object Label(string label, string forControl, bool required = false)
  {
    return Tag.Label(label).Class("col-sm-3" + (required ? " app-events6-field-required " : "")).Attr("for", forControl);
  }

  // returns a Hidden Input
  public IHtmlTag Hidden(string idString, string label = "", string value = "")
  {
    var hiddenInput = Tag.Input().Type("hidden").Id(idString).Value(value);

    var field = Tag.Div(hiddenInput);

    if (!string.IsNullOrEmpty(label))
    {
      field = field.Add(
          Tag.Label(label)
              .Attr("hidden")
              .For(idString)
      );
    }

    return field;
  }

  // returns an input with common attributes and a possible placeholder
  public IHtmlTag Text(string idString, string label, bool required, string value = "", bool disabled = false)
  {
    var item = Tag.Input().Type("text").Id(idString).Placeholder(PhLabel(label, required)).Class("form-control");
    SetRequired(item, required);
    
    if (value != null) { item.Attr("value", value); }
    if (disabled) { item = item.Disabled(); }
    
    return Field(idString, required, item, label);
  }

  public IHtmlTag Number(string idString, string label, bool required, int minValue = 0, int maxValue = 0, bool disabled = false, string value = null)
  {
    var item = Tag.Input().Type("number").Id(idString).Placeholder(PhLabel(label, required)).Class("form-control");
    SetRequired(item, required);

    if (minValue != 0) { item.Min(minValue.ToString()); }
    if (maxValue != 0) { item.Max(maxValue.ToString()); }
    if (value != null) { item.Attr("value", value); }
    if (disabled) { item = item.Disabled(); }

    return Field(idString, required, item, label);
  }

  // returns an input of type email with common attributes and a possible placeholder
  public IHtmlTag EMail(string idString, string label, bool required, bool recipientEmail = false)
  {
    var item = Tag.Input().Type("email").Id(idString).Placeholder(PhLabel(label, required)).Class("form-control");
    SetRequired(item, required, "LabelValidEmail");
    
    if (recipientEmail) { item.Attr("mail", "recipientEmail"); }

    return Field(idString, required, item, label);
  }

  // returns a textarea with common attributes and a possible placeholder
  public IHtmlTag Multiline(string idString, string label, bool required, bool disabled = false, string value = "", string rows = "")
  {
    var item = Tag.Textarea().Id(idString).Placeholder(PhLabel(label, required)).Class("form-control").Rows(rows);

    SetRequired(item, required);

    if (value != null) { item.Add(value); }
    if (disabled) { item = item.Disabled(); }

    return Field(idString, required, item, label);
  }

  // returns a select and options with common attributes
  public IHtmlTag DropDown(string idString, Dictionary<string, string> valueDictionary, bool required, bool multiSelect, string label = "", string placeHolderSelect = "")
  {
    var item = Tag.Select().Id(idString).Class("form-control");

    SetRequired(item, required);

    if (multiSelect) { item.Multiple().Attr("data-multiple-dropdown", idString); }
    
    item.Add(Tag.Option(ToSic.Razor.Blade.Text.First(placeHolderSelect, App.Resources.String("LabelSelect"))));
    
    foreach (var optionItem in valueDictionary)
    {
      item.Add(Tag.Option(optionItem.Value).Value(optionItem.Key));
    }
    
    return Field(idString, required, item, label);
  }

  // The normal DropDown only has an array of strings for the value without a key
  // public IHtmlTag DynDropDown(string idString, bool required, Dictionary<string, string> valueDictionary, bool multiSelect, string label = "", string placeHolderSelect = "")
  // {
  //   var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);
  //   var item = Tag.Select().Id(idString).Class("form-control");
  //   if (multiSelect)
  //   {
  //     item.Multiple().Attr("data-multiple-dropdown", idString);
  //   }

  //   SetRequired(item, required);
  //   item.Add(Tag.Option(placeHolderSelect.Length > 1 ? placeHolderSelect : "---Please select ---").Attr("value", "").Style("color: grey; font-weight: bold;"));
  //   foreach (var tempItem in valueDictionary)
  //   {
  //     item.Add(Tag.Option(tempItem.Value).Value(tempItem.Key));
  //   }
  //   return Field(idString, required, item, inputLabel);
  // }

  // // returns a Radio with common attributes
  // public IHtmlTag Radio(string idString, bool required, string[] values)
  // {
  //   var item = Tag.Div();
  //   foreach (var value in values)
  //   {
  //     var radioId = idString + value.ToLower().Replace(" ", "");
  //     var wrapper = Tag.Div().Class(Kit.Css.Is("bs3") ? "radio" : "form-check");
  //     var radio = Tag.Input().Attr("type", "radio").Id(radioId).Name(idString).Value(value);
  //     SetRequired(radio, required);
  //     if (Kit.Css.Is("bs3"))
  //     {
  //       var radioLabel = Tag.Label(radio + value).For(radioId);
  //       wrapper.Add(radioLabel);
  //     }
  //     else
  //     {
  //       radio.Class("form-check-input");
  //       var radioLabel = Tag.Label(value).Class("form-check-label").For(radioId);
  //       wrapper.Add(radio + radioLabel);
  //     }
  //     item.Add(wrapper);
  //   }
  //   return Field(idString, required, item);
  // }

  // The normal Radio only has an array of strings for the value without a key
  public IHtmlTag Radio(string idString, bool required, Dictionary<string, string> valueDictionary, string label = "")
  {
    var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);
    var item = Tag.Div();
    foreach (var tempItem in valueDictionary)
    {
      var radioId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(Kit.Css.Is("bs3") ? "radio" : "form-check");
      var radio = Tag.Input().Attr("type", "radio").Id(radioId).Name(idString).Value(tempItem.Key);
      SetRequired(radio, required);
      if (Kit.Css.Is("bs3"))
      {
        var radioLabel = Tag.Label(radio + tempItem.Value).For(radioId);
        wrapper.Add(radioLabel);
      }
      else
      {
        radio.Class("form-check-input");
        var radioLabel = Tag.Label(tempItem.Value).Class("form-check-label").For(radioId);
        wrapper.Add(radio + radioLabel);
      }
      item.Add(wrapper);
    }
    return Field(idString, required, item, inputLabel);
  }

  // returns a checkbox with common attributes
  public IHtmlTag Checkbox(string idString, bool required, bool terms = false, string label = "")
  {
    var checkbox = Tag.Input().Attr("type", "checkbox").Id(idString).Name(idString).Class("form-check-input");

    if(label != "")
      checkbox.Value(label);

    if (terms)
      checkbox.Attr("terms", "true");

    SetRequired(checkbox, required);
    var labelTranslated = ToSic.Razor.Blade.Text.First(label, App.Resources.String("Label" + idString, scrubHtml: "p", required: false));
    var checkboxLabel = ToSic.Razor.Blade.Text.First(labelTranslated, idString) + (required ? "*" : "");

    // Slightly different HTML for Bootstrap3
    if (Kit.Css.Is("bs3"))
    {
      return Tag.Div().Class(FormValidationClass() + "form-group").Wrap(
          Tag.Div().Class("checkbox").Wrap(
          Tag.Label().Wrap(checkbox, checkboxLabel)
          )
      );
    }
    else
    {
      // Bootstrap4 and 5
      return Tag.Div().Class(FormValidationClass() + "mb-3 form-check").Wrap(
      checkbox,
      Tag.Label(checkboxLabel).Class("form-check-label").For(idString)
      );
    }
  }

  // Alignment normale Field (Text Left and Checkbox Right)
  public IHtmlTag CheckboxFieldAligment(string idString, bool required, string label = "")
  {
    var checkbox = Tag.Input().Attr("type", "checkbox").Id(idString).Name(idString).Class("form-check-input").Value(label);
    SetRequired(checkbox, required);
    var labelTranslated = ToSic.Razor.Blade.Text.First(label, App.Resources.String("Label" + idString, scrubHtml: "p", required: false));
    var checkboxLabel = ToSic.Razor.Blade.Text.First(labelTranslated, idString) + (required ? "*" : "");
    return Field(idString, required, checkbox, checkboxLabel);
  }

  // Left the Value and rith e empty Checkbox
  public IHtmlTag CheckboxList(string idString, bool required, Dictionary<string, string> valueDictionary)
  {
    var checkboxes = new List<IHtmlTag>();

    foreach (var tempItem in valueDictionary)
    {
      var checkboxId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(Kit.Css.Is("bs3") ? "checkbox" : "form-check");
      var checkbox = Tag.Input().Attr("type", "checkbox").Id(checkboxId).Name(idString).Value(tempItem.Key);
      SetRequired(checkbox, required);

      if (Kit.Css.Is("bs3"))
      {
        var checkboxLabel = Tag.Label(checkbox + "").For(checkboxId);
        wrapper.Add(checkboxLabel);
      }
      else
      {
        checkbox.Class("form-check-input").Attr("data-checkbox", idString);
        var checkboxLabel = Tag.Label("").Class("form-check-label").For(checkboxId);
        wrapper.Add(checkbox + checkboxLabel);
      }

      checkboxes.Add(Field(checkboxId, required, wrapper, tempItem.Value));
    }

    return Tag.Div().Add(checkboxes);
  }

  // CheckboxListLabel with Label (CheckboxWithHeadline) 
  public IHtmlTag CheckboxListWithLabel(string idString, bool required, Dictionary<string, string> valueDictionary, string label = "")
  {
    var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);
    var item = Tag.Div();

    foreach (var tempItem in valueDictionary)
    {
      var checkboxId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(Kit.Css.Is("bs3") ? "checkbox" : "form-check");
      var checkbox = Tag.Input().Attr("type", "checkbox").Id(checkboxId).Name(idString).Value(tempItem.Key);
      SetRequired(checkbox, required);

      if (Kit.Css.Is("bs3"))
      {
        var checkboxLabel = Tag.Label(checkbox + tempItem.Value).For(checkboxId);
        wrapper.Add(checkboxLabel);
      }
      else
      {
        checkbox.Class("form-check-input").Attr("data-checkbox", idString);
        var checkboxLabel = Tag.Label(tempItem.Value).Class("form-check-label").For(checkboxId);
        wrapper.Add(checkbox + checkboxLabel);
      }

      item.Add(wrapper);
    }

    return Field(idString, required, item, inputLabel);
  }

  // returns a input of type file with common attributes
  public IHtmlTag File(string label, bool required, string acceptType, string idString = "")
  {
    var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);

    var input = Tag.Input().Type("file").Id(idString).Name(label).Class("form-control-file");

    if (ToSic.Razor.Blade.Text.Has(acceptType))
    {
      input = input.Attr("accept", acceptType);
    }
    SetRequired(input, required, "LabelValidFile");
    return Field(idString, required, input, inputLabel);
  }
  // Input Sort wrong
  public IHtmlTag DynFile(string idString, bool required, string acceptType, string label = "")
  {
    var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);

    var input = Tag.Input().Type("file").Id(idString).Name(label).Class("form-control-file");

    if (ToSic.Razor.Blade.Text.Has(acceptType))
    {
      input = input.Attr("accept", acceptType);
    }
    SetRequired(input, required, "LabelValidFile");
    return Field(idString, required, input, inputLabel);
  }

  // shows a wrapping div with choosen content
  private IHtmlTag Field(string idString, bool required, IHtmlTag items, string label = "")
  {
    var inputWrapperClasses = Kit.Css.Is("bs3") ? "col col-xs-12 col-sm-9" : "col-12 col-sm-9";
    var labelTranslated = ToSic.Razor.Blade.Text.First(label, App.Resources.String("Label" + idString, required: false));
    var field = Tag.Div().Class(FormClasses());

    // If the label is _not_ in the placeholder, add the label first
    if (!LabelInPlaceholder)
    {
      field = field.Add(
        Tag.Label(ToSic.Razor.Blade.Text.First(labelTranslated, idString))
          .Class(LabelClasses(required))
          .For(idString)
      );
    }

    return field.Add(Tag.Div(items).Class(!LabelInPlaceholder ? inputWrapperClasses : ""));
  }

}