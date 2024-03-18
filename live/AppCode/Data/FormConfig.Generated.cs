// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "FormConfig.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class FormConfig
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.04.01
// App/Edition: Mobius Forms 6/staging
// User:        2sic Web-Developer
// When:        2024-03-18 14:34:38Z
using System.Collections.Generic;

namespace AppCode.Data
{
  // This is a generated class for FormConfig 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// FormConfig data. <br/>
  /// Generated 2024-03-18 14:34:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.DesignField`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class FormConfig: AutoGenerated.ZagFormConfig
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.FormConfig in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagFormConfig: Custom.Data.CustomItem
  {
    /// <summary>
    /// DesignField as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("DesignField", scrubHtml: true) etc.
    /// </summary>
    public string DesignField => _item.String("DesignField", fallback: "");

    /// <summary>
    /// Fields as list of FormFieldConfig.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type FormFieldConfig was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<FormFieldConfig> Fields => _fields ??= _item.Children<FormFieldConfig>("Fields");
    private IEnumerable<FormFieldConfig> _fields;

    /// <summary>
    /// FormResources as single item of FormResources.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormResources was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormResources FormResources => _formResources ??= _item.Child<FormResources>("FormResources");
    private FormResources _formResources;

    /// <summary>
    /// FormSendMailConfig as single item of FormSendMailConfig.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormSendMailConfig was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormSendMailConfig FormSendMailConfig => _formSendMailConfig ??= _item.Child<FormSendMailConfig>("FormSendMailConfig");
    private FormSendMailConfig _formSendMailConfig;

    /// <summary>
    /// InheritedConfig as single item of FormConfig.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormConfig was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormConfig InheritedConfig => _inheritedConfig ??= _item.Child<FormConfig>("InheritedConfig");
    private FormConfig _inheritedConfig;

    /// <summary>
    /// Mailchimp as bool. <br/>
    /// To get nullable use .Get("Mailchimp") as bool?;
    /// </summary>
    public bool Mailchimp => _item.Bool("Mailchimp");

    /// <summary>
    /// PublishForReuse as bool. <br/>
    /// To get nullable use .Get("PublishForReuse") as bool?;
    /// </summary>
    public bool PublishForReuse => _item.Bool("PublishForReuse");

    /// <summary>
    /// PublishTitle as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("PublishTitle", scrubHtml: true) etc.
    /// </summary>
    public string PublishTitle => _item.String("PublishTitle", fallback: "");

    /// <summary>
    /// Recaptcha as bool. <br/>
    /// To get nullable use .Get("Recaptcha") as bool?;
    /// </summary>
    public bool Recaptcha => _item.Bool("Recaptcha");

    /// <summary>
    /// ReuseConfig as bool. <br/>
    /// To get nullable use .Get("ReuseConfig") as bool?;
    /// </summary>
    public bool ReuseConfig => _item.Bool("ReuseConfig");

    /// <summary>
    /// SaveToContentType as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SaveToContentType", scrubHtml: true) etc.
    /// </summary>
    public string SaveToContentType => _item.String("SaveToContentType", fallback: "");

    /// <summary>
    /// ShowRecaptchaIcon as bool. <br/>
    /// To get nullable use .Get("ShowRecaptchaIcon") as bool?;
    /// </summary>
    public bool ShowRecaptchaIcon => _item.Bool("ShowRecaptchaIcon");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    /// <remarks>
    /// This hides base property Title.
    /// To access original, convert using AsItem(...) or cast to ITypedItem.
    /// Consider renaming this field in the underlying content-type.
    /// </remarks>
    public new string Title => _item.String("Title", fallback: "");

    /// <summary>
    /// TitleInternal as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("TitleInternal", scrubHtml: true) etc.
    /// </summary>
    public string TitleInternal => _item.String("TitleInternal", fallback: "");
  }
}