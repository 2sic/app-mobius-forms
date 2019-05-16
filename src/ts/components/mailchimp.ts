declare let $2sxc: any;

import { Helpers } from '../helpers/helpers';

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
    wrapper.find('#subToMc').on('click', (evt) => this.send(evt) );
  }

  send(event: any) {
    const wrapper = this.moduleWrapper;
    const ws = wrapper.find('.app-jqfs-wrapper').data('webservice');
    const btn = event.currentTarget;
    const sxc = $2sxc(btn);
  
    // Validate form
    if (!(wrapper as any).smkValidate())
      return this.helper.showOneAlert(wrapper, 'msgIncomplete');
    
    const data = {
      SenderMail: wrapper.find('.sender-mail').val(),
      SenderName: wrapper.find('.sender-name').val(),
      SenderLastName: wrapper.find('.sender-surname').val(),
      MailChimp: true
    }
    
    sxc.webApi.post(ws, {}, data, true)
      .success(() => {
        this.moduleWrapper.find('.app-jqfs-form-mailchimp').fadeOut();
        this.moduleWrapper.find('#NewsletterSuccessMsg').fadeIn();
      })
      .error(() => {
        this.moduleWrapper.find('.app-jqfs-form-mailchimp').fadeOut();
        this.moduleWrapper.find('#NewsletterFailedMsg').fadeIn();
      })
  }
}