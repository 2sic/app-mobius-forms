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
      var input = Tag.Input().Type("file").Name(Field.Title).Class("form-control-file" + " " + CssClasses.InputControl);

      if(Text.Has(Field.AcceptedExtensions)) {
        input.Attr("accept", Field.AcceptedExtensions);
      }

      input = SetBasics(input, false);
      var container = Tag.Div().Class(CssClasses.OutsideDiv + " " + Constants.ClassMobiusField);

      var inputLabels = Tag.Label(Field.Title).For(Field.FieldId).Class(LabelClasses(Field.Required));
      var innerContainer = Tag.Div().Class(CssClasses.LabelOutside);

      container.Add(inputLabels);
      innerContainer.Add(input);
      container.Add(innerContainer);

      if (Text.Has(Field.InfoText))
        container.Add(Tag.Div(Field.InfoText).Class("small-infotext"));
        
      if (Text.Has(Field.AcceptedExtensions))
        container.Add(Tag.Small(Field.AcceptedExtensions));      

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