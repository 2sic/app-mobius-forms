using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  /// <summary>
  /// Base class for all Field Builders
  /// </summary>
  public abstract class BuildFieldBase
  {

    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public BuildFieldBase(FormBuildParameters form, DynFormField field)
    {
      Form = form;
      Resources = form.Resources;
      SendMailConfig = form.SendMailConfig;
      Field = field;
    }

    protected FormBuildParameters Form { get; }
    protected AppResources Resources { get; }
    protected SendMailConfig SendMailConfig { get; }
    protected DynFormField Field { get; }
    protected CssClasses CssClasses => Form.CssClasses;

    public abstract IHtmlTag GetTag();

    public IHtmlTag SetBasicsAndWrapInLabel(Input item, bool setDefaultClass = true)
    {
      var modified = SetBasics(item, setDefaultClass: setDefaultClass);
      return WrapInLabel(modified);
    }


    /// <summary>
    /// Set all defaults like ID, Required label, common classes etc.
    /// </summary>
    /// <typeparam name="TTag"></typeparam>
    /// <param name="field"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected TTag SetBasics<TTag>(TTag item, bool setDefaultClass = true, string customId = "") where TTag : ToSic.Razor.Html5.Input
    {
      var result = item
        .Placeholder(PlaceholderLabel());
      if (customId != "")
        result = result.Id(customId);
      else
        result = result.Id(Field.FieldId);
      if (setDefaultClass) result = result
        .Class(CssClasses.InputControl);

      if (Field.Required) result = SetRequired(result);
      if (Field.IsDisabled) result = result.Disabled();
      return result as TTag;
    }


    protected string PlaceholderLabel()
      => Form.UseFloatingLabels ? Field.Title + (Field.Required ? " *" : "") : "";

    protected TTag SetRequired<TTag>(TTag item, string specialReqMessage = default) where TTag : class, IHtmlTag
    {
      if (!Field.Required) return item;
      var message = specialReqMessage != null
        ? Resources.String(specialReqMessage)
        : Form.FormResources.LabelRequired;

      item = item.Attr("data-pristine-required-message", message).Attr("required") as TTag;
      return item;
    }

    // Set Label Right or Floating Label (only for Bs5)
    protected IHtmlTag WrapInLabel(IHtmlTag inputHtml)
    {
      var htmlTag = Tag.Div().Class(FieldWrapperClasses());
      // Label Right
      if (!Form.UseFloatingLabels)
      {
        htmlTag = htmlTag.Add(
            Tag.Label(Text.First(Field.Title, Field.FieldId))
                .Class(LabelClasses(Field.Required))
                .For(Field.FieldId)
        );
        htmlTag = htmlTag.Add(Tag.Div(inputHtml).Class(Form.UseFloatingLabels ? CssClasses.LabelInside : CssClasses.LabelOutside));
      }
      else // Floating Labels (bs5)
      {
        htmlTag = htmlTag.Add(inputHtml);
        htmlTag = htmlTag.Add(
            Tag.Label(Text.First(Field.Title, Field.FieldId))
                .Class(LabelClasses(Field.Required, Form.UseFloatingLabels))
                .For(Field.FieldId)
        );
      }
      return htmlTag;
    }

    private string FieldWrapperClasses()
    {
      return $"{Constants.ClassMobiusField} {(Form.UseFloatingLabels ? "form-floating " : "row ")}{CssClasses.Wrapper}";
    }

    private string LabelClasses(bool required, bool floatingLabel = false)
    {
      return "control-label "
          + (required ? Constants.ClassRequired : "")
          + " " + CssClasses.Label
          + " " + (floatingLabel ? CssClasses.FloatingLabelHidden : "");  // Bs3 and Bs4 only - hidden Label
    }

  }
}