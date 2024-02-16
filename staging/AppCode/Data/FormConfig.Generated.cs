// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/hotbuild-autogen
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

// Generator:   DataModelGenerator v17.01.08
// App/Edition: Mobius Forms 6/staging
// User:        2sic Web-Developer
// When:        2024-02-16 18:53:14Z
using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace AppCode.Data
{
  // This is a generated class for FormConfig
  // To extend/modify it, see instructions above.

  /// <summary>
  /// FormConfig data. <br/>
  /// Generated 2024-02-16 18:53:14Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.DesignField`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class FormConfig: FormConfigAutoGenerated
  {  }

  /// <summary>
  /// Auto-Generated base class for FormConfig.
  /// </summary>
  public abstract class FormConfigAutoGenerated: Custom.Data.Item16
  {
    /// <summary>
    /// DesignField as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("DesignField", scrubHtml: true) etc.
    /// </summary>
    public string DesignField => String("DesignField", fallback: "");

    /// <summary>
    /// Fields as listof FormFieldConfig.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type FormFieldConfig was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<FormFieldConfig> Fields => _fields ??= Children<FormFieldConfig>("Fields");
    private IEnumerable<FormFieldConfig> _fields;

    /// <summary>
    /// FormResources as single itemof FormResources.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormResources was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormResources FormResources => _formResources ??= Child<FormResources>("FormResources");
    private FormResources _formResources;

    /// <summary>
    /// FormSendMailConfig as single itemof FormSendMailConfig.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormSendMailConfig was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormSendMailConfig FormSendMailConfig => _formSendMailConfig ??= Child<FormSendMailConfig>("FormSendMailConfig");
    private FormSendMailConfig _formSendMailConfig;

    /// <summary>
    /// InheritedConfig as single itemof ITypedItem.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. 
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public ITypedItem InheritedConfig => _inheritedConfig ??= Child("InheritedConfig");
    private ITypedItem _inheritedConfig;

    /// <summary>
    /// Mailchimp as bool. <br/>
    /// To get nullable use .Get("Mailchimp") as bool?;
    /// </summary>
    public bool Mailchimp => Bool("Mailchimp");

    /// <summary>
    /// PublishForReuse as bool. <br/>
    /// To get nullable use .Get("PublishForReuse") as bool?;
    /// </summary>
    public bool PublishForReuse => Bool("PublishForReuse");

    /// <summary>
    /// PublishTitle as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("PublishTitle", scrubHtml: true) etc.
    /// </summary>
    public string PublishTitle => String("PublishTitle", fallback: "");

    /// <summary>
    /// Recaptcha as bool. <br/>
    /// To get nullable use .Get("Recaptcha") as bool?;
    /// </summary>
    public bool Recaptcha => Bool("Recaptcha");

    /// <summary>
    /// ReuseConfig as bool. <br/>
    /// To get nullable use .Get("ReuseConfig") as bool?;
    /// </summary>
    public bool ReuseConfig => Bool("ReuseConfig");

    /// <summary>
    /// SaveToContentType as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SaveToContentType", scrubHtml: true) etc.
    /// </summary>
    public string SaveToContentType => String("SaveToContentType", fallback: "");

    /// <summary>
    /// ShowRecaptchaIcon as bool. <br/>
    /// To get nullable use .Get("ShowRecaptchaIcon") as bool?;
    /// </summary>
    public bool ShowRecaptchaIcon => Bool("ShowRecaptchaIcon");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    public string Title => String("Title", fallback: "");

    /// <summary>
    /// TitleInternal as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("TitleInternal", scrubHtml: true) etc.
    /// </summary>
    public string TitleInternal => String("TitleInternal", fallback: "");
  }
}