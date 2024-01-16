using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ThisApp.Data;

namespace ThisApp.Code
{
  public class FormBuilder2: Custom.Hybrid.CodeTyped
  {
    public FormBuilder2(IHasCodeContext parent, FormConfiguration formConfiguration): base(parent) {
      FormConfiguration = formConfiguration;
    }

    public FormConfiguration FormConfiguration { get; }

    public IHtmlTag Field(DynFormField field)
    {
      var type = field.FieldType;
      if (type == "string")
        return new BuildFieldText(FormConfiguration, field).GetTag();
      if (type == "number")
        return new BuildFieldNumber(FormConfiguration, field).GetTag();

      throw new System.Exception($"The type '{type}' is not supported yet in the new FormBuilder");

    }

    // TODO: 2DM - CONTINUE HERE

  // returns form-classes based on whether label is shown as placeholder or besides form - as row  
  // private string FormClasses()
  // {
  //   return $"{Constants.ClassMobiusField} {(LabelInPlaceholder ? "" : "row ")}{(Kit.Css.Is("bs3") ? "form-group" : "mb-3")}";
  // }

    // -- rest probably never used!

    /// <summary>
    /// Generate a label for a form field
    /// </summary>
    /// <param name="label"></param>
    /// <param name="forControl"></param>
    /// <param name="required"></param>
    /// <returns></returns>
    public IHtmlTag Label(string label, string forControl, bool required = false)
    {
      // TODO: class has "events6"?
      return Kit.HtmlTags.Label(label).Class("col-sm-3" + (required ? " app-events6-field-required " : "")).Attr("for", forControl);
    }

  }
}