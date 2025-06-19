using System.CodeDom;
using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldFile : BuildFieldBase
  {
    public BuildFieldFile(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Build a File Upload Input
    /// </summary>
    public override IHtmlTag GetTag()
    {
      return FileField();
    }
    private IHtmlTag FileField()
    {
      var tags = Builder.Kit.HtmlTags;
      var input = tags.Input().Type("file").Name(Field.Title).Class("form-control-file" + " " + CssClasses.InputControl);

      if(Field.IsNotEmpty("AcceptedExtensions"))
        input = input.Attr("accept", Field.AcceptedExtensions);

      input = SetBasics(input, false);
      var container = tags.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);

      var inputLabels = tags.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var innerContainer = tags.Div().Class(CssClasses.LabelOutside);

      container = container.Add(inputLabels);
      innerContainer = innerContainer.Add(input);

      if (Field.IsNotEmpty("AcceptedExtensions"))
        innerContainer = innerContainer.Add(tags.Small(Field.AcceptedExtensions)); 
        
      container = container.Add(innerContainer);

      if (Field.IsNotEmpty("InfoText"))
        container = container.Add(tags.Div(Field.InfoText).Class("small-infotext"));
        
      if (Field.IsNotEmpty("AcceptedExtensions"))
        container = container.Add(tags.Small(Field.AcceptedExtensions));      

      return container;
    }

    private string LabelClasses(bool required)
    {
      return "control-label "
          + (required ? Constants.ClassRequired : "")
          + " "
         + (Form.UseFloatingLabels ? "col-12" : CssClasses.Label);
    }

  }
}