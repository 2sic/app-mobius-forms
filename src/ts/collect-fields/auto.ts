export class CollectFieldsAutomatic {
  // automatically build the send-object with all properties, 
  // based on all form-fields which have a item-property=""
  collect(wrapper: JQuery) {
    let data: any = {
      Files: []
    };
    const fields = wrapper.find(':input').not('button');
    const promises = fields.map((i, field) => add(i, field));
    
    function add(i: number, e: any) {
      e = $(e);
      // get the property name from special-attribut, name OR id
      const propName = e.attr('name') || e.attr('id');

      if (!propName)
        return;

      // extract data from file fields
      if (e.attr('type') && e.attr('type').toLowerCase() == 'file') {
        // todo 2ro: put this block into a function below
        const deferred = $.Deferred();
        const file = e.get(0).files[0];
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

      } else if (e.attr('type') && e.attr('type').toLowerCase() == 'radio') {
        if (e.is(':checked')) {
          data[propName] = e.val();
        }
      } else { // For all standard fields, set value directly
        data[propName] = e.val();
      }
    }

    return $.when.apply($, promises).then(() => {
      return data;
    });
  }
}