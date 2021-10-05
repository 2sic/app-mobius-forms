using ToSic.Razor.Blade;
using System;
using ToSic.Eav.Configuration;
public class FieldBuilders: Custom.Hybrid.Code12
{
  // TODO: update
  /* 
    this file is for creating different fields e.g. input, textarea, file, dropdown and showing them in the template

    Example: 
    Shows a required input of type text with a label infront of it
    
    var FieldBuilder = CreateInstance("shared/FieldBuilders.cs");
    @FieldBuilder.Text("Subject", true)

    Shows a required input of type text with a label infront of it
    
    var FieldBuilder = CreateInstance("shared/FieldBuilders.cs");
    FieldBuilder.LabelInPlaceholder = true
    @FieldBuilder.EMail("Email", true)

    Shows a required input of type text with a placeholder
  */

  // handles the visibility of a label or a placeholder
  public bool LabelInPlaceholder = false;
  internal dynamic PageCss { get { return _pageCss ?? (_pageCss = GetService<Connect.Koi.ICss>()); } }
  private dynamic _pageCss;
  internal string FormClasses() {
    return "form-group " + (LabelInPlaceholder ? "" : "row");
  }

  #region Koi based class selection
  // Choose CSS classes based on the framework
  // if you customize this, you probably know what css framework you want,
  // in which case you can skip framework detection and just write the classes

  // Choose CSS classes for the labels
  internal string LabelClasses(bool required) {
    return "control-label "
      + (required ? "mobius-field-required " : "")
      + (PageCss.Is("bs3") ? "col col-xs-12 col-sm-4" : "col-12 col-md-4");
  } 

  // Choose CSS classes for the wrapping div of an input
  internal string InputWrapperClasses;
  private string _inputWrapperClasses;
  #endregion

  // Add a placeholder text to the inputs
  internal string Placeholder(string key, bool required) {
    return LabelInPlaceholder ? Resources.Get("Label" + key) + (required ? "*" : "") : "";
  }

  // returns an input with common attributes and a possible placeholder
  public dynamic Text(string idString, bool required) {
    var content = Tag.Input().Type("text").Id(idString).Attr("placeholder", Placeholder(idString, required)).Class("form-control");
    if(required) {
      content = content.Attr("data-pristine-required-message", Resources.LabelRequired).Required();
    }
    return Field(idString, required, content);
  }

  // returns an input of type email with common attributes and a possible placeholder
  public dynamic EMail(string idString, bool required) {
    var content = Tag.Input().Type("email").Id(idString).Attr("placeholder", Placeholder(idString, required)).Class("form-control");
    if(required) {
      content = content.Attr("data-pristine-required-message", Resources.LabelValidEmail).Required();
    }
    return Field(idString, required, content);
  }

  // returns a textarea with common attributes and a possible placeholder
  public dynamic Multiline(string idString, bool required) {
    var content = Tag.Textarea().Id(idString).Attr("placeholder", Placeholder(idString, required)).Class("form-control");
    if(required) {
      content = content.Attr("data-pristine-required-message", Resources.LabelRequired).Required();
    }
    return Field(idString, required, content);
  }

  // returns a select and options with common attributes
  public dynamic DropDown(string idString, bool required, string[] values) {
    var content = Tag.Select().Id(idString).Class("form-control");
    if(required) {
      content = content.Attr("data-pristine-required-message", Resources.LabelRequired).Required();
    }

    content.Add(Tag.Option("--Please Select--").Attr("value", ""));

    foreach(var value in values){
      content.Add(Tag.Option(value));
    }
    
    return Field(idString, required, content);
  }

  // returns a input of type file with common attributes
  public dynamic File(string name, bool required, string acceptType, string idString = "") {
    var content = Tag.Input().Type("file").Id(idString).Attr("name", name).Attr("accept", acceptType).Class("form-control-file");
    if(required) {
      content = content.Attr("data-pristine-required-message", Resources.LabelValidFile).Required();
    }
    return Field(idString, required, content);

    
    // show warning if the save-attachments in web api isn't activated
    // this is a security feature to protect your installation from unwanted uploads

    var features = GetService<IFeaturesService>();
    if(!features.Enabled(FeatureIds.UseAdamInWebApi)) {
      return Tag.Div(Resources.MessageDisabledFeature).Class("alert alert-warning");
    }
  }

  // shows a wrapping div with choosen content
  public dynamic Field(string idString, bool required, dynamic contents) {
    InputWrapperClasses = _inputWrapperClasses ?? (_inputWrapperClasses = (PageCss.Is("bs3") ? "col col-xs-12 col-sm-8" : "col-12 col-md-8"));
    var labelTranslated = Resources.Get("Label" + idString);

    var labelInPlaceholder = Tag.Label();
    if (!LabelInPlaceholder) {
      labelInPlaceholder = Tag.Label(ToSic.Razor.Blade.Text.First(labelTranslated, idString)).Class(LabelClasses(required)).For(idString);
    }

    return Tag.Div(labelInPlaceholder, Tag.Div(contents).Class(!LabelInPlaceholder ? InputWrapperClasses : "")).Class(FormClasses());
  }
}


