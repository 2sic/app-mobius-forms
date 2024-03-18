// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "Newsletter.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class Newsletter
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
  // This is a generated class for Newsletter 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// Newsletter data. <br/>
  /// Generated 2024-03-18 14:34:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Email`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class Newsletter: AutoGenerated.ZagNewsletter
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.Newsletter in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagNewsletter: Custom.Data.CustomItem
  {
    /// <summary>
    /// Email as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Email", scrubHtml: true) etc.
    /// </summary>
    public string Email => _item.String("Email", fallback: "");

    /// <summary>
    /// Name as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Name", scrubHtml: true) etc.
    /// </summary>
    public string Name => _item.String("Name", fallback: "");
  }
}