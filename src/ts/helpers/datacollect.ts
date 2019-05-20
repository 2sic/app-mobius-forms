export class DataCollect {
  // automatically build the send-object with all properties, 
  // based on all form-fields which have a item-property=""
  auto(wrapper: JQuery) {
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

  manual(wrapper: JQuery) {
    const data: any = {
      Subject: wrapper.find('#Subject'),
      Message: wrapper.find('#Message'),
      SenderName: wrapper.find('#Sendername'),
      SenderMail: wrapper.find('#Sendermail')
    };

    for (let prop in data) {
      if (data.hasOwnProperty(prop)) {
        data[prop] = data[prop].val();
      }
    }

    return data;
  }
}