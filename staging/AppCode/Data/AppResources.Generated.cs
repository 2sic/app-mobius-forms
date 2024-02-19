// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/hotbuild-autogen
// To extend it, create a "AppResources.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class AppResources
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   DataModelGenerator v17.01.08
// App/Edition: Mobius Forms 6/staging
// User:        2sic Web-Developer
// When:        2024-02-19 15:56:29Z
using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace AppCode.Data
{
  // This is a generated class for AppResources
  // To extend/modify it, see instructions above.

  /// <summary>
  /// AppResources data. <br/>
  /// Generated 2024-02-19 15:56:29Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.DefaultFormResources`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class AppResources: AppResourcesAutoGenerated
  {  }

  /// <summary>
  /// Auto-Generated base class for AppResources.
  /// </summary>
  public abstract class AppResourcesAutoGenerated: Custom.Data.Item16
  {
    /// <summary>
    /// DefaultFormResources as single itemof FormResources.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type FormResources was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public FormResources DefaultFormResources => _defaultFormResources ??= Child<FormResources>("DefaultFormResources");
    private FormResources _defaultFormResources;

    /// <summary>
    /// DefaultSendMailConfig as single itemof ITypedItem.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. 
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public ITypedItem DefaultSendMailConfig => _defaultSendMailConfig ??= Child("DefaultSendMailConfig");
    private ITypedItem _defaultSendMailConfig;

    /// <summary>
    /// LabelFirst as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LabelFirst", scrubHtml: true) etc.
    /// </summary>
    public string LabelFirst => String("LabelFirst", fallback: "");

    /// <summary>
    /// LabelFromDataAvailable as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LabelFromDataAvailable", scrubHtml: true) etc.
    /// </summary>
    public string LabelFromDataAvailable => String("LabelFromDataAvailable", fallback: "");

    /// <summary>
    /// LabelLast as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LabelLast", scrubHtml: true) etc.
    /// </summary>
    public string LabelLast => String("LabelLast", fallback: "");

    /// <summary>
    /// LabelRecaptcha as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LabelRecaptcha", scrubHtml: true) etc.
    /// </summary>
    public string LabelRecaptcha => String("LabelRecaptcha", fallback: "");

    /// <summary>
    /// LabelTimestamp as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LabelTimestamp", scrubHtml: true) etc.
    /// </summary>
    public string LabelTimestamp => String("LabelTimestamp", fallback: "");

    /// <summary>
    /// MessageContainsMailChimp as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageContainsMailChimp", scrubHtml: true) etc.
    /// </summary>
    public string MessageContainsMailChimp => String("MessageContainsMailChimp", fallback: "");

    /// <summary>
    /// MessageContainsRecaptcha as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageContainsRecaptcha", scrubHtml: true) etc.
    /// </summary>
    public string MessageContainsRecaptcha => String("MessageContainsRecaptcha", fallback: "");

    /// <summary>
    /// MessageDefaultMailChimpKey as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageDefaultMailChimpKey", scrubHtml: true) etc.
    /// </summary>
    public string MessageDefaultMailChimpKey => String("MessageDefaultMailChimpKey", fallback: "");

    /// <summary>
    /// MessageDemoItem as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageDemoItem", scrubHtml: true) etc.
    /// </summary>
    public string MessageDemoItem => String("MessageDemoItem", fallback: "");

    /// <summary>
    /// MessageEnableMailchimp as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageEnableMailchimp", scrubHtml: true) etc.
    /// </summary>
    public string MessageEnableMailchimp => String("MessageEnableMailchimp", fallback: "");

    /// <summary>
    /// MessageRecaptchaMissing as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageRecaptchaMissing", scrubHtml: true) etc.
    /// </summary>
    public string MessageRecaptchaMissing => String("MessageRecaptchaMissing", fallback: "");

    /// <summary>
    /// MessageRecaptchaWarning as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MessageRecaptchaWarning", scrubHtml: true) etc.
    /// </summary>
    public string MessageRecaptchaWarning => String("MessageRecaptchaWarning", fallback: "");

    /// <summary>
    /// NoFieldsInfo as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NoFieldsInfo", scrubHtml: true) etc.
    /// </summary>
    public string NoFieldsInfo => String("NoFieldsInfo", fallback: "");

    /// <summary>
    /// ToolbarAppResources as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("ToolbarAppResources", scrubHtml: true) etc.
    /// </summary>
    public string ToolbarAppResources => String("ToolbarAppResources", fallback: "");

    /// <summary>
    /// ToolbarConfigure as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("ToolbarConfigure", scrubHtml: true) etc.
    /// </summary>
    public string ToolbarConfigure => String("ToolbarConfigure", fallback: "");

    /// <summary>
    /// ToolbarPermissionInfo as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("ToolbarPermissionInfo", scrubHtml: true) etc.
    /// </summary>
    public string ToolbarPermissionInfo => String("ToolbarPermissionInfo", fallback: "");

    /// <summary>
    /// ToolbarReuseInfo as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("ToolbarReuseInfo", scrubHtml: true) etc.
    /// </summary>
    public string ToolbarReuseInfo => String("ToolbarReuseInfo", fallback: "");

    /// <summary>
    /// ToolbarViewData as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("ToolbarViewData", scrubHtml: true) etc.
    /// </summary>
    public string ToolbarViewData => String("ToolbarViewData", fallback: "");
  }
}