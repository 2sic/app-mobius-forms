declare let $2sxc: any;

import { UiActions } from './add-ins/uiActions';
import { CollectFieldsManual } from './collect-fields/manual';
import { CollectFieldsAutomatic } from './collect-fields/auto';
import { Recaptcha } from './add-ins/recaptcha';
export class App {
  helperFunc = new UiActions();
  collectFieldsManual = new CollectFieldsManual();
  collectFieldsAutomatic = new CollectFieldsAutomatic();
  recaptcha = new Recaptcha();
  moduleWrapper: JQuery;
  alreadyInit = false;

  c = {
    clsWrp: 'mobius-wrapper',
    clsForm: 'mobius-form',
  };

  constructor(
    moduleId: number,
  ) {
    // disable validate on the global asp.net form, to not interfere with the contact-form
    $('form').attr('novalidate', '');
    this.moduleWrapper = $(`.DnnModule-${moduleId}`);
  }

  public initialize() {
    const wrapper = this.moduleWrapper;
    // attach validation to enable as soon as we blur        
    wrapper.on('blur', ':input', this.helperFunc.attachFieldValidateOnBlur);

    wrapper.each((i, item) => {
      // prevent dupl execution
      if (this.alreadyInit)
        return;

      const wrap = $(item);
      wrap.find('#sendFormWithApi').on('click', (evt: JQueryEventObject) => this.send(evt)); // handle click event

      this.alreadyInit = true;
    });
  }

  public send(event: JQueryEventObject) {
    const btn = event.currentTarget;
    const sxc = $2sxc(btn);
    const wrapper = this.moduleWrapper;
    const label = wrapper.find('#sendFormWithApi').text();

    // clear all alerts
    this.helperFunc.showOneAlert(wrapper, '');

    const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'submit', label: label } });
    document.dispatchEvent(trackingEvent);

    // Validate form
    if (!(wrapper as any).smkValidate())
      return this.helperFunc.showOneAlert(wrapper, 'msgIncomplete');

    // Do Recaptcha test, show alert & fail if required and not complete
    const recap = this.recaptcha.check(wrapper);
    if (!recap)
      return this.helperFunc.showOneAlert(wrapper, 'msgRecap');

    const mailchimp = wrapper.find('.mobius-wrapper').hasClass('mobius-mailchimp');

    // get data 
    // alternative example with manual build, but we prefer automatic
    // let data;
    // data = this.collectFieldsManual.collect(wrapper);
    this.collectFieldsAutomatic.collect(wrapper).then((data: any) => {
      const ws = wrapper.find('.mobius-wrapper').data('webservice'); // should be "Form/ProcessForm" or a custom override
      data.Recaptcha = recap;
      data.MailChimp = mailchimp;
      // submission
      this.helperFunc.disableInputs(wrapper, true);
      this.helperFunc.showOneAlert(wrapper, 'msgSending'); // show "sending..."

      sxc.webApi.post(ws, null, data, true)
        .success(() => {
          const msg = mailchimp ? 'msgNewsletterSuccess' : 'msgOk';
          this.helperFunc.showOneAlert(wrapper, msg);

          const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'success', label: label } });
          document.dispatchEvent(trackingEvent);
        })
        .error(() => {
          const msg = mailchimp ? 'msgNewsletterFailed' : 'msgError';
          this.helperFunc.showOneAlert(wrapper, msg);
          this.helperFunc.disableInputs(wrapper, false);

          const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'error', label: label } });
          document.dispatchEvent(trackingEvent);
        });
    });
  }
}