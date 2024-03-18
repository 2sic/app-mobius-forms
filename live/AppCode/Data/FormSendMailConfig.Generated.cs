// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "FormSendMailConfig.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class FormSendMailConfig
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.04.01
// App/Edition: Mobius Forms 6/staging
// User:        2sic Web-Developer
// When:        2024-03-18 14:34:38Z
namespace AppCode.Data
{
  // This is a generated class for FormSendMailConfig 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// FormSendMailConfig data. <br/>
  /// Generated 2024-03-18 14:34:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.CustomerMailCC`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class FormSendMailConfig: AutoGenerated.ZagFormSendMailConfig
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.FormSendMailConfig in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagFormSendMailConfig: Custom.Data.CustomItem
  {
    /// <summary>
    /// CustomerMailCC as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("CustomerMailCC", scrubHtml: true) etc.
    /// </summary>
    public string CustomerMailCC => _item.String("CustomerMailCC", fallback: "");

    /// <summary>
    /// CustomerMailTemplate as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("CustomerMailTemplate", scrubHtml: true) etc.
    /// </summary>
    public string CustomerMailTemplate => _item.String("CustomerMailTemplate", fallback: "");

    /// <summary>
    /// CustomerSend as bool. <br/>
    /// To get nullable use .Get("CustomerSend") as bool?;
    /// </summary>
    public bool CustomerSend => _item.Bool("CustomerSend");

    /// <summary>
    /// MailFrom as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("MailFrom", scrubHtml: true) etc.
    /// </summary>
    public string MailFrom => _item.String("MailFrom", fallback: "");

    /// <summary>
    /// Name as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Name", scrubHtml: true) etc.
    /// </summary>
    public string Name => _item.String("Name", fallback: "");

    /// <summary>
    /// OwnerMail as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("OwnerMail", scrubHtml: true) etc.
    /// </summary>
    public string OwnerMail => _item.String("OwnerMail", fallback: "");

    /// <summary>
    /// OwnerMailCC as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("OwnerMailCC", scrubHtml: true) etc.
    /// </summary>
    public string OwnerMailCC => _item.String("OwnerMailCC", fallback: "");

    /// <summary>
    /// OwnerMailTemplate as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("OwnerMailTemplate", scrubHtml: true) etc.
    /// </summary>
    public string OwnerMailTemplate => _item.String("OwnerMailTemplate", fallback: "");

    /// <summary>
    /// OwnerSend as bool. <br/>
    /// To get nullable use .Get("OwnerSend") as bool?;
    /// </summary>
    public bool OwnerSend => _item.Bool("OwnerSend");
  }
}