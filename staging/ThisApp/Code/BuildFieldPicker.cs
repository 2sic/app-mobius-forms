using System.Collections.Generic;
using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldPicker : BuildFieldBase
  {
    public BuildFieldPicker(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Text must override it, because the Checkbox is not an Text input
    /// </summary>
    public override IHtmlTag GetTag()
    {
      return Radio();
    }

    private Input GetRadio()
    {
      
      var radio = Tag.Input().Type("Radio").Id(Field.FieldId);
      return radio;
    }

    private IHtmlTag Radio()
    {

      var items = Tag.Div();

      foreach (var item in GetKeyValue(Field.ValuesDropdownRadio))
      {
        var radioId = Field.FieldId + item.Value.ToLower().Replace(" ", "");
        var wrapper = Tag.Div().Class(CssClasses.IsBs3 ? "radio" : "form-check");
        var radio = GetRadio().Value(item.Key);
        radio = SetBasics(radio);

        if (CssClasses.IsBs3)
        {
          var radioLabel = Tag.Label(radio + item.Value).For(radioId);
          wrapper.Add(radioLabel);
        }

        else
        {
          radio.Class(Constants.ClassCheckbox);
          var radioLabel = Tag.Label(item.Value).Class("form-check-label").For(radioId);
          wrapper.Add(radio + radioLabel);
        }
        items.Add(wrapper);
      }
      return items;
    }


    // TODO::
    Dictionary<string, string> GetKeyValue(string valuesDropdownRadio)
    {
      Dictionary<string, string> valueDictionaryRadio = new Dictionary<string, string>();
      foreach (var item in valuesDropdownRadio.Split('\n'))
      {
        var values = item.Split(':');
        // If values has Key and Value
        if (values.Length == 2)
        {
          string key = values[0].Trim();
          string value = values[1].Trim();
          valueDictionaryRadio[key] = value;
        }
        // If only have a Value
        else if (values.Length == 1)
        {
          string key = values[0];
          string value = values[0];
          valueDictionaryRadio[key] = value;
        }
        else
        {
          string key = "Empty";
          string value = "Empty";
          valueDictionaryRadio[key] = value;
        }
      }
      return valueDictionaryRadio;
    }

  }






}