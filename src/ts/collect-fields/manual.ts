/*
  This is an example of collecting the fields manually (instead of automatically). 
  You would modify this file list all the fields you want, which allows you to use
  a different naming schema than the default. 
  This is an advanced use case, and included so you could do this, but you usually won't want to.
*/
export class CollectFieldsManual {
  // automatically build the send-object with all properties, 
  // based on all form-fields which have a item-property=""
  collect(wrapper: JQuery) {
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