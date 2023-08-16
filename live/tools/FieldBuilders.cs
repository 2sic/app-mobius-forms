using System;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class FieldBuilders: Custom.Hybrid.CodeTyped
{
  /* 
    this file is for creating different fields e.g. input, textarea, file, dropdown and showing them in the template

    Example: 
    Shows a required input of type text with a label in front of it
    
    var FieldBuilder = GetCode("tools/FieldBuilders.cs");
    @FieldBuilder.Text("Subject", true)

    Shows a required input of type text with a label in front of it
    
    var FieldBuilder = GetCode("tools/FieldBuilders.cs");
    FieldBuilder.LabelInPlaceholder = true
    @FieldBuilder.EMail("Email", true)

    Shows a required input of type text with a placeholder
  */

  // handles the visibility of a label or a placeholder
  public bool LabelInPlaceholder = false;

  #region Koi based class selection

  // returns form validation class 
  private string FormValidationClass() {
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
  private string LabelClasses(bool required) {
    return "control-label "
      + (required ? "app-mobius5-field-required " : "")
      + (Kit.Css.Is("bs3") ? "col col-xs-12 col-sm-4" : "col-12 col-md-4");
  } 

  #endregion

  // Add a placeholder text to the inputs
  private string PhLabel(string key, bool required) {
    return LabelInPlaceholder ? App.Resources.String("Label" + key) + (required ? "*" : "") : "";
  }

  // Sets a RazorBlade Input/TextArea to required and adds the message which is different for each field type
  internal void SetRequired(IHtmlTag item, bool required, string message) 
  {
    if (!required) return;
    item.Attr("data-pristine-required-message", message).Attr("required");
  }



  // returns an input with common attributes and a possible placeholder
  public IHtmlTag Text(string idString, bool required) {
    var input = Tag.Input().Type("text").Id(idString).Placeholder(PhLabel(idString, required)).Class("form-control");
    SetRequired(input, required, App.Resources.String("LabelRequired"));
    return Field(idString, required, input);
  }

  // returns an input of type email with common attributes and a possible placeholder
  public IHtmlTag EMail(string idString, bool required) {
    var input = Tag.Input().Type("email").Id(idString).Placeholder(PhLabel(idString, required)).Class("form-control");
    SetRequired(input, required, App.Resources.String("LabelValidEmail"));
    return Field(idString, required, input);
  }

  // returns a textarea with common attributes and a possible placeholder
  public IHtmlTag Multiline(string idString, bool required) {
    var textarea = Tag.Textarea().Id(idString).Placeholder(PhLabel(idString, required)).Class("form-control");
    SetRequired(textarea, required, App.Resources.String("LabelRequired"));
    return Field(idString, required, textarea);
  }

  // returns a select and options with common attributes
  public IHtmlTag DropDown(string idString, bool required, string[] values) {
    var selectClass = Kit.Css.Is("bs5") ? "form-select" : "form-control";
    var select = Tag.Select().Id(idString).Class(selectClass);
    SetRequired(select, required, App.Resources.String("LabelRequired"));
    select.Add(Tag.Option(App.Resources.String("LabelSelect")).Attr("value", ""));
    foreach (var value in values){
      select.Add(Tag.Option(value));
    }
    
    return Field(idString, required, select);
  }

    // returns a checkbox with common attributes and a possible placeholder
  public IHtmlTag Checkbox(string idString, bool required) {
    var checkbox = Tag.Input().Attr("type", "checkbox").Id(idString).Name(idString).Class("form-check-input");
    SetRequired(checkbox, required, App.Resources.String("LabelRequired"));
    var labelTranslated = App.Resources.String("Label" + idString, scrubHtml: "p");
    var label = ToSic.Razor.Blade.Text.First(labelTranslated, idString) + (required ? "*" : "");

    // Slightly different HTML for Bootstrap3
    if (Kit.Css.Is("bs3")) {
      return Tag.Div().Class(FormValidationClass() + "form-group").Wrap(
        Tag.Div().Class("checkbox").Wrap(
          Tag.Label().Wrap(checkbox, label)
        )
      ); 
    } else {
    // Bootstrap4 and 5
    return Tag.Div().Class(FormValidationClass() + "mb-3 form-check" ).Wrap(
      checkbox,
      Tag.Label(label).Class("form-check-label").For(idString)
    );
    }
  }

  // returns a input of type file with common attributes
  public IHtmlTag File(string name, bool required, string acceptType, string idString = "") {
    var input = Tag.Input().Type("file").Id(idString).Name(name).Class("form-control-file");
    
    if (ToSic.Razor.Blade.Text.Has(acceptType)) {
      input = input.Attr("accept", acceptType);
    }
    SetRequired(input, required, App.Resources.String("LabelValidFile"));
    return Field(idString, required, input);
  }

  // shows a wrapping div with chosen content
  private IHtmlTag Field(string idString, bool required, IHtmlTag contents) {
    var inputWrapperClasses = Kit.Css.Is("bs3") ? "col col-xs-12 col-sm-8" : "col-12 col-md-8";
    var labelTranslated = App.Resources.String("Label" + idString);
    var label = ToSic.Razor.Blade.Text.First(labelTranslated, idString);
    // var label = "tt";
    var field = Tag.Div().Class(FormClasses());

    // If the label is _not_ in the placeholder, add the label first
    if (!LabelInPlaceholder)
      field = field.Add(Tag.Label(label).Class(LabelClasses(required)).For(idString));
    
    return field.Add(Tag.Div(contents).Class(LabelInPlaceholder ? "" : inputWrapperClasses));
  }

}
