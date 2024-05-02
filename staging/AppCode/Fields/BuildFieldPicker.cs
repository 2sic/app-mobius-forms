using System.Collections.Generic;
using AppCode.Data;
using AppCode.Form;

namespace AppCode.Fields
{
  public abstract class BuildFieldPicker : BuildFieldBase
  {
    public BuildFieldPicker(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    // Helper function to create a dictionary with keys and values from the string use in Radio and Dropdown 
    protected static Dictionary<string, string> GetKeyValue(string pickerKeyValues)
    {
      Dictionary<string, string> valueDictionaryRadio = new Dictionary<string, string>();
      foreach (var item in pickerKeyValues.Split('\n'))
      {
        var values = item.Split(':');
        // If values has Key and Value
        if (values.Length == 2)
        {
          var key = values[0].Trim();
          var value = values[1].Trim();
          valueDictionaryRadio[key] = value;
        }
        // If only have a Value
        else if (values.Length == 1)
        {
          var key = values[0].Trim();
          valueDictionaryRadio[key] = key;
        }
        else
        {
          var key = "Empty";
          valueDictionaryRadio[key] = key;
        }
      }
      return valueDictionaryRadio;
    }

    // Generate the HTML ID for the radio button and Checkbox 
    protected string GeneratedHtmlId(KeyValuePair<string, string> tempItem)
    {
      return Field.FieldId + tempItem.Value.ToLower().Replace(" ", ""); ;
    }

   protected string LabelClasses(bool required)
    {
      return "control-label "
          + (required ? Constants.ClassRequired : "")
          + " " + (Form.UseFloatingLabels ? "col-12" : CssClasses.Label);
    }
  }
}