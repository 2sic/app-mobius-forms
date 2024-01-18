using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  /// <summary>
  /// Base class for all Field Builders
  /// </summary>
  public abstract class BuildFieldBase
  {

    // handles the visibility of a label or a placeholder
    // TODO: move to DynForm
    public bool LabelInPlaceholder = false;

    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public BuildFieldBase(FormBuildParameters form, DynFormField field)
    {
      Form = form;
      Resources = form.Resources;
      DynForm = form.Form;
      Field = field;
    }

    protected FormBuildParameters Form { get; }
    protected AppResources Resources { get; }
    protected DynForm DynForm { get; }
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
      // TODO: THIS is not tested, 2dm added the field
      if (Field.IsDisabled) result = result.Disabled();
      return result as TTag;
    }


    protected string PlaceholderLabel()
      => "🌟" + (LabelInPlaceholder ? Field.Title + (Field.Required ? " *" : "") : "");

    protected TTag SetRequired<TTag>(TTag item, string specialReqMessage = default) where TTag : class, IHtmlTag
    {
      if (!Field.Required) return item;

      var message = specialReqMessage != null
        ? Resources.String(specialReqMessage)
        : Resources.String("LabelRequired");

      item = item.Attr("data-pristine-required-message", message).Attr("required") as TTag;
      return item;
    }

    protected IHtmlTag WrapInLabel(IHtmlTag inputHtml)
    {
      var htmlTag = Tag.Div().Class(FieldWrapperClasses());

      // If the label is _not_ in the placeholder, add the label first
      if (!LabelInPlaceholder)
        htmlTag = htmlTag.Add(
          Tag.Label(Text.First(Field.Title, Field.FieldId))
            .Class(LabelClasses(Field.Required))
            .For(Field.FieldId)
        );

      return htmlTag.Add(Tag.Div(inputHtml).Class(LabelInPlaceholder ? CssClasses.LabelInside : CssClasses.LabelOutside));
    }

    private string FieldWrapperClasses()
    {
      return $"{Constants.ClassMobiusField} {(LabelInPlaceholder ? "" : "row ")}{CssClasses.Wrapper}";
    }

    private string LabelClasses(bool required)
    {
      return "control-label "
        + (required ? "app-mobius5-field-required " : "")
        + " " + CssClasses.Label;
    }

  }
}