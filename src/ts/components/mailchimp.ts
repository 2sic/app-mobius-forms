declare let $2sxc: any;

import { Helpers } from './helpers';

export class MailChimp {
  helper = new Helpers();

  c = {
    clsWrp: 'app-jqfs-wrapper',
    clsForm: 'app-jqfs-form',
  };
  moduleWrapper: JQuery;

  constructor() { }

  init(wrapper: JQuery) {
    this.moduleWrapper = wrapper;
    wrapper.find('#subToMc').on('click', (evt) => this.send(evt) );  // handle click event
  }

  send(event: any) {
    const wrapper = this.moduleWrapper;
    const btn = event.currentTarget;
    const sxc = $2sxc(btn);
  
    // Validate form
    if (!(wrapper as any).smkValidate())
      return this.helper.showOneAlert(wrapper, 'msgIncomplete');
    
    var u = {
      mail: wrapper.find('.sender-mail').val(),
      name: wrapper.find('.sender-name').val(),
      surname: wrapper.find('.sender-surname').val()
    }
    
    sxc.webApi.post("Mailchimp/Subscribe", { email: u.mail, fname: u.name, lname: u.surname }, null, true)
      .success((response: any) => {
          $(".app-jqfs-form-mailchimp").fadeOut();
          $("#NewsletterSuccessMsg").fadeIn();
      })
      .error(() => {
          $(".app-jqfs-form-mailchimp").fadeOut();
          $("#NewsletterFailedMsg").fadeIn();
      })
  }
}