// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "FormData.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class FormData
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   DataModelGenerator v17.02.01
// App/Edition: Mobius Forms 6/staging
// User:        2sic Web-Developer
// When:        2024-02-29 20:10:05Z
using System;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for FormData
  // To extend/modify it, see instructions above.

  /// <summary>
  /// FormData data. <br/>
  /// Generated 2024-02-29 20:10:05Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Files`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class FormData: FormDataAutoGenerated
  {  }

  /// <summary>
  /// Auto-Generated base class for FormData.
  /// </summary>
  public abstract class FormDataAutoGenerated: Custom.Data.CustomItem
  {
    /// <summary>
    /// Files as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Files")
    /// </summary>
    public string Files => _item.Url("Files");

    /// <summary>
    /// Get the file object for Files - or null if it's empty or not referencing a file.
    /// </summary>
    public IFile FilesFile => _item.File("FilesFile");

    /// <summary>
    /// Get the folder object for Files.
    /// </summary>
    public IFolder FilesFolder => _item.Folder("FilesFolder");

    /// <summary>
    /// FormId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("FormId", scrubHtml: true) etc.
    /// </summary>
    public string FormId => _item.String("FormId", fallback: "");

    /// <summary>
    /// ModuleId as int. <br/>
    /// To get other types use methods such as .Decimal("ModuleId")
    /// </summary>
    public int ModuleId => _item.Int("ModuleId");

    /// <summary>
    /// RawData as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("RawData", scrubHtml: true) etc.
    /// </summary>
    public string RawData => _item.String("RawData", fallback: "");

    /// <summary>
    /// SenderIP as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SenderIP", scrubHtml: true) etc.
    /// </summary>
    public string SenderIP => _item.String("SenderIP", fallback: "");

    /// <summary>
    /// Timestamp as DateTime.
    /// </summary>
    public DateTime Timestamp => _item.DateTime("Timestamp");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    public string Title => _item.String("Title", fallback: "");
  }
}