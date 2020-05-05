export class CollectFieldsAutomatic {
  // automatically build the send-object with all properties, 
  // based on all form-fields which have a item-property=""
  collect(wrapper: JQuery) {
    let data: any = {
      Files: []
    };
    const fields = wrapper.find(':input').not('button');
    const promises = fields.map((i, field) => add(i, field));
    
    function add(i: number, element: any) {
      element = $(element);
      // get the property name from special-attribut, name OR id
      const propName = element.attr('name') || element.attr('id');

      if (!propName)
        return;

      // extract data from file fields
      if (element.attr('type') && element.attr('type').toLowerCase() == 'file') {
        const deferred = $.Deferred();
        const file = element.get(0).files[0];
        if (!file)
          return;

        const reader = new FileReader();

        reader.addEventListener('load', function () {
          data.Files.push({
            Encoded: reader.result,
            Name: file.name,
            Field: propName
          });
          deferred.resolve();
        }, false);
        reader.readAsDataURL(file);

        return deferred.promise();

      } else if (element.attr('type') && element.attr('type').toLowerCase() == 'radio') { // For radio fields get checked values
        if (element.is(':checked')) {
          data[propName] = element.val();
        }
      } else if (element.attr('type') && element.attr('type').toLowerCase() == 'checkbox') { // For radio fields get checked values
        const checkValue = element.is(':checked') ? "True" : "False";
        data[propName] = checkValue;
      } else { // For all standard fields, set value directly
        data[propName] = element.val();
      }
    }

    return $.when.apply($, promises).then(() => {
      return data;
    });
  }
}