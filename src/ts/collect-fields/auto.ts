export class CollectFieldsAutomatic {
  // automatically build the send-object with all properties, 
  // based on all form-fields which have a item-property=""
  collect(wrapper: Element) {
    let data: any = {
      Files: []
    };
    const fields = wrapper.querySelectorAll('input,textarea,select');
    const inputData = fields.forEach((field: HTMLInputElement, index: number) => { add(index, field) });
    
    function add(i: number, element: HTMLInputElement) {
      // get the property name from special-attribut, name OR id
      const propName = element.getAttribute('name') || element.getAttribute('id');

      if (!propName)
        return;

      // extract data from file fields
      if (element.getAttribute('type') && element.getAttribute('type').toLowerCase() == 'file') {
        const file = element.files[0];
        if (!file)
          return;

        const reader = new FileReader();

        reader.addEventListener('load', function () {
          data.Files.push({
            Encoded: reader.result,
            Name: file.name,
            Field: propName
          });
        }, false);
        reader.readAsDataURL(file);
      } else if (element.getAttribute('type') && element.getAttribute('type').toLowerCase() == 'radio') { // For radio fields get checked values
        if (element.checked) {
          data[propName] = element.value;
        }
      } else if (element.getAttribute('type') && element.getAttribute('type').toLowerCase() == 'checkbox') { // For radio fields get checked values
        const checkValue = element.checked ? "True" : "False";
        data[propName] = checkValue;
      } else { // For all standard fields, set value directly
        data[propName] = element.value
      }      
    }

    return data;
  }
}