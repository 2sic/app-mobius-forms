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
      var tag = Builder.Kit.HtmlTags;
      var input = tag.Input().Type("file").Name(Field.Title).Class("form-control-file" + " " + CssClasses.InputControl);

      if(Field.IsNotEmpty("AcceptedExtensions")) {
        input.Attr("accept", Field.AcceptedExtensions);
      }

      input = SetBasics(input, false);
      var container = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);

      var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var innerContainer = Tag.Div().Class(CssClasses.LabelOutside);

      container.Add(inputLabels);
      innerContainer.Add(input);
      container.Add(innerContainer);

      if (Field.IsNotEmpty("InfoText"))
        container.Add(tag.Div(Field.InfoText).Class("small-infotext"));
        
      if (Field.IsNotEmpty("AcceptedExtensions"))
        container.Add(tag.Small(Field.AcceptedExtensions));      

      return container;
    }

    private string LabelClasses(bool required)
    {
      return "control-label "
          + (required ? Constants.ClassRequired : "")
          + " " + (Form.UseFloatingLabels ? "col-12" : CssClasses.Label);
    }

  }
}