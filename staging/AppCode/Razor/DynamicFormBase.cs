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
      var cssClasses = Kit.Css.Is("bs3")
        ? CssClasses.Bs3
        : (Kit.Css.Is("bs4")
          ? CssClasses.Bs4
          : CssClasses.Bs5);

      var formParams = new FormBuildParameters(AppRes, FormRes, MailConfig, cssClasses, MyUser, Kit, FormConfig.UseFloatingLabels);
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
    public string WrapperClasses() => $"app-mobius6-wrapper{(FormConfig.Get<bool>("EnableMailchimp") ? " app-mobius6-mailchimp" : "")}";


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