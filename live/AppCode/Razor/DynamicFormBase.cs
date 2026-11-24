using System.Collections.Generic;
using System.Linq;
using AppCode.Data;
using AppCode.Form;
using ToSic.Sxc.Edit.Toolbar;

namespace AppCode.Razor
{
  public abstract class DynamicFormBase : MobiusRazorBase
  {
    /// <summary>
    /// The FormBuilder which takes care of rendering the different input fields
    /// </summary>
    protected FormBuilder FormBuilder => _formBuilder ??= CreateFormBuilder();
    private FormBuilder _formBuilder;
    private FormBuilder CreateFormBuilder()
    {
      var formParams = new FormBuildParameters(AppRes, FormRes, MailConfig, CssClasses.Bs5, MyUser, Kit, FormConfig.UseFloatingLabels);
      return GetService<FormBuilder>().Setup(formParams);
    }

    /// <summary>
    /// Get all the fields, grouped together based on separator-fields which are marked as section-changes.
    /// </summary>
    /// <returns></returns>
    protected List<IGrouping<string, FormFieldConfig>> GetFieldsInGroups()
    {
      // Group Fields by their Group, which changes on labels with NewGroup...
      var groupIdPrefix = "mobius-group-";
      var groupCount = 0;
      return FormConfig.Fields
        .GroupBy(f => groupIdPrefix + (f.LabelStartsNewGroup ? ++groupCount : groupCount))
        .ToList();
    }

    /// <summary>
    /// Small helper to figure out the best classes for the wrapper div
    /// </summary>
    /// <returns></returns>
    public string WrapperClasses()
    {
      var classes = "app-mobius6-wrapper";
      
      if (FormConfig.Get<bool>("EnableMailchimp"))
        classes += " app-mobius6-mailchimp";
      
      if (GetFieldsInGroups().Count > 1)
        classes += " app-mobius-groupform";
      
      return classes;
    }


    #region Duplicate Field-ID Detection

    /// <summary>
    /// All FieldIds which are used by more than one field in this form (case-insensitive).
    /// Duplicate IDs cause problems on submit, since the values overwrite each other.
    /// Empty IDs and label-fields are ignored, since labels are never submitted.
    /// </summary>
    protected HashSet<string> DuplicateFieldIds => _duplicateFieldIds ??= new HashSet<string>(
      FormConfig.Fields
        .Where(f => f.FieldType != "label" && !string.IsNullOrWhiteSpace(f.FieldId))
        .GroupBy(f => f.FieldId.Trim(), System.StringComparer.OrdinalIgnoreCase)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key),
      System.StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _duplicateFieldIds;

    /// <summary>
    /// Check if a field shares its FieldId with another field - only relevant for admins, so others always get false.
    /// </summary>
    protected bool ShowDuplicateIdWarning(FormFieldConfig field)
      => MyUser.IsContentAdmin
        && field.FieldType != "label"
        && !string.IsNullOrWhiteSpace(field.FieldId)
        && DuplicateFieldIds.Contains(field.FieldId.Trim());

    #endregion

    #region Toolbar Stuff

    /// <summary>
    /// Prepare the main toolbar for the form - if the user has permissions for it
    /// </summary>
    protected IToolbarBuilder FormToolbar() => EnableEditExperience
        ? Kit.Toolbar.Default(FormConfig)
        : Kit.Toolbar.Empty().Info(tweak: b => b.Note(AppRes.ToolbarPermissionInfo));


    /// <summary>
    /// Generate a toolbar for a specific field
    /// </summary>
    protected IToolbarBuilder FieldToolbar(FormFieldConfig field)
    {
      // If the user is not admin, exit so save resources
      if (!MyUser.IsContentAdmin)
        return null;

      // Prepare some notes so the editor sees what field is being edited
      var fieldInfo = $"Field: <strong>{field.FieldId}</strong> (type: {field.FieldType})";
      if (ShowDuplicateIdWarning(field))
        fieldInfo += "<br><strong style='color:red'>Diese Field-ID wird mehrfach verwendet!</strong>";

      // Prepare a special message for admins which don't have permissions
      // or which would have permissions, but shouldn't edit
      // because the field definitions are inherited from another form
      var FieldToolbarLockedInfo = !EnableEditExperience
        ? AppRes.ToolbarPermissionInfo
        : FormConfig.ReuseConfig
          ? AppRes.ToolbarReuseInfo
          : null;

      // If we have a special "locked" message, then show an info-only toolbar with a note
      if (FieldToolbarLockedInfo != null)
        return Kit.Toolbar.Empty()
            .Info(tweak: b => b.Note(fieldInfo + "<br><br>" + FieldToolbarLockedInfo, format: "html"));

      // Normal editor / admin: show toolbar for the field with infos and various buttons
      return Kit.Toolbar.Empty(field)
            .Edit(tweak: b => b.Note(fieldInfo, format: "html"))
            .MoveDown()
            .New()
            .Remove();
    }

    #endregion
  }

  /// <summary>
  /// Helper class to group fields
  /// </summary>
  public class FieldInGroup
  {
    // public string GroupId { get; set; }
    public FormFieldConfig Field { get; set; }
  }
}
