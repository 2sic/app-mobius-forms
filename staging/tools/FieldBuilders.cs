using ToSic.Razor.Blade;
using System.Collections.Generic;
using ThisApp.Code;

public class FieldBuilders : Custom.Hybrid.CodeTyped
{
  public CssClasses CssClasses => _cssClasses ??= Kit.Css.Is("bs3") ? CssClasses.Bs3 : CssClasses.Bs5;
  private CssClasses _cssClasses;

  // handles the visibility of a label or a placeholder
  public bool LabelInPlaceholder = false;

  #region Koi based class selection

  // returns form-classes based on whether label is shown as placeholder or besides form - as row  
  private string FieldWrapperClasses()
  {
    return $"{Constants.ClassMobiusField} {(LabelInPlaceholder ? "" : "row ")}{CssClasses.Wrapper}";
  }

  // Choose CSS classes based on the framework
  // if you customize this, you probably know what css framework you want,
  // in which case you can skip framework detection and just write the classes

  // Choose CSS classes for the labels
  private string LabelClasses(bool required)
  {
    return "control-label "
      + (required ? "app-mobius5-field-required " : "")
      + " " + CssClasses.Label;
  }

  #endregion

  // Add a placeholder text to the inputs
  private string PhLabel(string label, bool required)
  {
    return LabelInPlaceholder ? label + (required ? " *" : "") : ""; ;
  }

  internal TTag SetRequired<TTag>(TTag item, bool required, string key = null) where TTag : class, IHtmlTag
  {
    if (!required) return item;

    var message = key != null
      ? App.Resources.String(key)
      : App.Resources.String("LabelRequired");

    item = item.Attr("data-pristine-required-message", message).Attr("required") as TTag;
    return item;
  }

  // Cache the message after first lookup for performance as we use it quite ofter
  private string MessageRequired = null;


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

  // returns an input of type email with common attributes and a possible placeholder
  // public IHtmlTag EMail(string idString, string label, bool required, bool recipientEmail = false)
  // {
  //   var item = Tag.Input().Type("email").Id(idString).Placeholder(PhLabel(label, required)).Class(CssClasses.InputControl);
  //   SetRequired(item, required, "LabelValidEmail");
    
  //   if (recipientEmail) { item.Attr("mail", "recipientEmail"); }

  //   return WrapWithLabel(idString, required, item, label);
  // }

  // returns a select and options with common attributes
  public IHtmlTag DropDown(string idString, Dictionary<string, string> valueDictionary, bool required, bool multiSelect, string label = "", string placeHolderSelect = "")
  {
    var item = Tag.Select().Id(idString).Class(CssClasses.InputControl);

    SetRequired(item, required);

    if (multiSelect) { item.Multiple().Attr("data-multiple-dropdown", idString); }
    
    item.Add(Tag.Option(Text.First(placeHolderSelect, App.Resources.String("LabelSelect"))));
    
    foreach (var optionItem in valueDictionary)
    {
      item.Add(Tag.Option(optionItem.Value).Value(optionItem.Key));
    }
    
    return WrapWithLabel(idString, required, item, label);
  }

  // The normal DropDown only has an array of strings for the value without a key
  // public IHtmlTag DynDropDown(string idString, bool required, Dictionary<string, string> valueDictionary, bool multiSelect, string label = "", string placeHolderSelect = "")
  // {
  //   var inputLabel = ToSic.Razor.Blade.Text.First(label, idString);
  //   var item = Tag.Select().Id(idString).Class(CssClasses.InputControl);
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
  //     var wrapper = Tag.Div().Class(CssClasses.IsBs3 ? "radio" : "form-check");
  //     var radio = Tag.Input().Attr("type", "radio").Id(radioId).Name(idString).Value(value);
  //     SetRequired(radio, required);
  //     if (CssClasses.IsBs3)
  //     {
  //       var radioLabel = Tag.Label(radio + value).For(radioId);
  //       wrapper.Add(radioLabel);
  //     }
  //     else
  //     {
  //       radio.Class(Constants.ClassCheckbox);
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
    var inputLabel = Text.First(label, idString);
    var item = Tag.Div();
    foreach (var tempItem in valueDictionary)
    {
      var radioId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(CssClasses.IsBs3 ? "radio" : "form-check");
      var radio = Tag.Input().Attr("type", "radio").Id(radioId).Name(idString).Value(tempItem.Key);
      SetRequired(radio, required);
      if (CssClasses.IsBs3)
      {
        var radioLabel = Tag.Label(radio + tempItem.Value).For(radioId);
        wrapper.Add(radioLabel);
      }
      else
      {
        radio.Class(Constants.ClassCheckbox);
        var radioLabel = Tag.Label(tempItem.Value).Class("form-check-label").For(radioId);
        wrapper.Add(radio + radioLabel);
      }
      item.Add(wrapper);
    }
    return WrapWithLabel(idString, required, item, inputLabel);
  }

  // returns a checkbox with common attributes
  public IHtmlTag Checkbox(string idString, bool required, bool terms = false, string label = "")
  {
    var checkbox = Tag.Input().Attr("type", "checkbox").Id(idString).Name(idString).Class(Constants.ClassCheckbox);

    if(label != "")
      checkbox.Value(label);

    if (terms)
      checkbox.Attr("terms", "true");

    SetRequired(checkbox, required);
    var labelTranslated = Text.First(label, App.Resources.String("Label" + idString, scrubHtml: "p", required: false));
    var checkboxLabel = Text.First(labelTranslated, idString) + (required ? "*" : "");

    // Slightly different HTML for Bootstrap3
    if (CssClasses.IsBs3)
    {
      return Tag.Div().Class($"{Constants.ClassMobiusField} form-group").Wrap(
          Tag.Div().Class("checkbox").Wrap(
          Tag.Label().Wrap(checkbox, checkboxLabel)
          )
      );
    }
    else
    {
      // Bootstrap4 and 5
      return Tag.Div().Class($"{Constants.ClassMobiusField} mb-3 form-check").Wrap(
      checkbox,
      Tag.Label(checkboxLabel).Class("form-check-label").For(idString)
      );
    }
  }

  // Alignment normale Field (Text Left and Checkbox Right)
  public IHtmlTag CheckboxFieldAligment(string idString, bool required, string label = "")
  {
    var checkbox = Tag.Input().Attr("type", "checkbox").Id(idString).Name(idString).Class(Constants.ClassCheckbox).Value(label);
    SetRequired(checkbox, required);
    var labelTranslated = Text.First(label, App.Resources.String("Label" + idString, scrubHtml: "p", required: false));
    var checkboxLabel = Text.First(labelTranslated, idString) + (required ? "*" : "");
    return WrapWithLabel(idString, required, checkbox, checkboxLabel);
  }

  // Left the Value and rith e empty Checkbox
  public IHtmlTag CheckboxList(string idString, bool required, Dictionary<string, string> valueDictionary)
  {
    var checkboxes = new List<IHtmlTag>();

    foreach (var tempItem in valueDictionary)
    {
      var checkboxId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(CssClasses.IsBs3 ? "checkbox" : "form-check");
      var checkbox = Tag.Input().Attr("type", "checkbox").Id(checkboxId).Name(idString).Value(tempItem.Key);
      SetRequired(checkbox, required);

      if (CssClasses.IsBs3)
      {
        var checkboxLabel = Tag.Label(checkbox + "").For(checkboxId);
        wrapper.Add(checkboxLabel);
      }
      else
      {
        checkbox.Class(Constants.ClassCheckbox).Attr("data-checkbox", idString);
        var checkboxLabel = Tag.Label("").Class("form-check-label").For(checkboxId);
        wrapper.Add(checkbox + checkboxLabel);
      }

      checkboxes.Add(WrapWithLabel(checkboxId, required, wrapper, tempItem.Value));
    }

    return Tag.Div().Add(checkboxes);
  }

  // CheckboxListLabel with Label (CheckboxWithHeadline) 
  public IHtmlTag CheckboxListWithLabel(string idString, bool required, Dictionary<string, string> valueDictionary, string label = "")
  {
    var inputLabel = Text.First(label, idString);
    var item = Tag.Div();

    foreach (var tempItem in valueDictionary)
    {
      var checkboxId = idString + tempItem.Value.ToLower().Replace(" ", "");
      var wrapper = Tag.Div().Class(CssClasses.IsBs3 ? "checkbox" : "form-check");
      var checkbox = Tag.Input().Attr("type", "checkbox").Id(checkboxId).Name(idString).Value(tempItem.Key);
      SetRequired(checkbox, required);

      if (CssClasses.IsBs3)
      {
        var checkboxLabel = Tag.Label(checkbox + tempItem.Value).For(checkboxId);
        wrapper.Add(checkboxLabel);
      }
      else
      {
        checkbox.Class(Constants.ClassCheckbox).Attr("data-checkbox", idString);
        var checkboxLabel = Tag.Label(tempItem.Value).Class("form-check-label").For(checkboxId);
        wrapper.Add(checkbox + checkboxLabel);
      }

      item.Add(wrapper);
    }

    return WrapWithLabel(idString, required, item, inputLabel);
  }

  // returns a input of type file with common attributes
  public IHtmlTag File(string label, bool required, string acceptType, string idString = "")
  {
    var inputLabel = Text.First(label, idString);

    var input = Tag.Input().Type("file").Id(idString).Name(label).Class("form-control-file");

    if (Text.Has(acceptType))
    {
      input = input.Attr("accept", acceptType);
    }
    SetRequired(input, required, "LabelValidFile");
    return WrapWithLabel(idString, required, input, inputLabel);
  }
  // Input Sort wrong
  public IHtmlTag DynFile(string idString, bool required, string acceptType, string label = "")
  {
    var inputLabel = Text.First(label, idString);

    var input = Tag.Input().Type("file").Id(idString).Name(label).Class("form-control-file");

    if (Text.Has(acceptType))
    {
      input = input.Attr("accept", acceptType);
    }
    SetRequired(input, required, "LabelValidFile");
    return WrapWithLabel(idString, required, input, inputLabel);
  }

  // shows a wrapping div with choosen content
  private IHtmlTag WrapWithLabel(string idString, bool required, IHtmlTag items, string label = "")
  {
    var inputWrapperClasses = CssClasses.IsBs3 ? "col col-xs-12 col-sm-9" : "col-12 col-sm-9";
    // TODO: SEEMS TO try to i18n the label, but that doesn't make sense
    var labelTranslated = Text.First(label, App.Resources.String("Label" + idString, required: false));
    var field = Tag.Div().Class(FieldWrapperClasses());

    // If the label is _not_ in the placeholder, add the label first
    if (!LabelInPlaceholder)
    {
      field = field.Add(
        Tag.Label(Text.First(labelTranslated, idString))
          .Class(LabelClasses(required))
          .For(idString)
      );
    }

    return field.Add(Tag.Div(items).Class(!LabelInPlaceholder ? inputWrapperClasses : ""));
  }

}