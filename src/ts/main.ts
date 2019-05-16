declare let $2sxc: any;

import { Helpers } from './helpers/helpers';
import { DataCollect } from './helpers/datacollect';
import { Recaptcha } from './components/recaptcha';
import { MailChimp } from './components/mailchimp';
export class App {
  helper = new Helpers();
  datacollect = new DataCollect();
  recaptcha = new Recaptcha();
  mailChimp = new MailChimp();
  moduleWrapper: JQuery;
  alreadyInit = false;

  c = {
    clsWrp: 'app-jqfs-wrapper',
    clsForm: 'app-jqfs-form',
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
    wrapper.on('blur', ':input', this.helper.attachFieldValidateOnBlur);

    wrapper.each((i, item) => {
      // prevent dupl execution
      if (this.alreadyInit)
        return;

      const wrap = $(item);
      wrap.find('#sendFormWithApi').on('click', (evt: JQueryEventObject) => this.send(evt)); // handle click event

      this.alreadyInit = true;
    });

    this.mailChimp.init(wrapper);
  }

  public send(event: JQueryEventObject) {
    const btn = event.currentTarget;
    const sxc = $2sxc(btn);
    const wrapper = this.moduleWrapper;

    // clear all alerts
    this.helper.showOneAlert(wrapper, '');

    // Validate form
    if (!(wrapper as any).smkValidate())
      return this.helper.showOneAlert(wrapper, 'msgIncomplete');

    // Do Recaptcha test, show alert & fail if required and not complete
    const recap = this.recaptcha.check(wrapper);
    if (!recap)
      return this.helper.showOneAlert(wrapper, 'msgRecap');

    // get data 
    // let data;
    // data = this.datacollect.manual(wrapper); // alternative example with manual build, but we prefer automatic
    this.datacollect.auto(wrapper).then((data: any) => {
      const ws = wrapper.find('.app-jqfs-wrapper').data('webservice'); // should be "Form/ProcessForm" or a custom override
      data.Recaptcha = recap;

      // submission
      this.helper.disableInputs(wrapper, true);
      this.helper.showOneAlert(wrapper, 'msgSending'); // show "sending..."

      sxc.webApi.post(ws, {}, data, true)
        .success(() => {
          this.helper.showOneAlert(wrapper, 'msgOk')
          $(btn).hide();
        })
        .error(() => {
          this.helper.showOneAlert(wrapper, 'msgError')
          this.helper.disableInputs(wrapper, false);
        });
    });
  }
}