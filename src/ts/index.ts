import { UiActions } from './add-ins/uiActions';
import { Recaptcha } from './add-ins/recaptcha';
import { CollectFieldsAutomatic } from './collect-fields/auto';

let Pristine = require('../../node_modules/pristinejs')

const debug = true;

declare let $2sxc: any;

var winAny = window as any;
winAny.appMobius5 ??= {};
winAny.appMobius5.init ??= initAppMobius5;

function initAppMobius5({ domAttribute } : { domAttribute: string }) {

  if (debug) console.log("Mobius5 loading, debug is enabled");
  domAttribute
  const mobiusWrapper = document.querySelectorAll(`[${domAttribute}]`)[0];
  const helperFunc = new UiActions();

  if (document.getElementsByTagName('form').length) document.getElementsByTagName('form')[0].setAttribute('novalidate', '');

  // todo: 2mh rename to app-mobius5-send and make sure it's not a class
  mobiusWrapper.getElementsByClassName('btn-send-mobius5-form')[0].addEventListener('click', (event: Event) => {
    event.preventDefault();
    var valid = validate(mobiusWrapper, event);
    if (valid) {
      new CollectFieldsAutomatic().collect(mobiusWrapper, sendWhenReady);
    }
  })
}

function validate(wrapper: Element, event: Event): boolean {
  const helperFunc = new UiActions();
  const pristine = new Pristine(wrapper);
  const btn = event.currentTarget as HTMLElement;
  const label = btn.innerText;

  helperFunc.showOneAlert(wrapper, '');

  const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'submit', label: label } });
  document.dispatchEvent(trackingEvent);
  
  const valid = pristine.validate();
  if (!valid)
    helperFunc.showOneAlert(wrapper, 'msgIncomplete');

  return valid;
}

function sendWhenReady(data: any, wrapper: HTMLElement) {
  const helperFunc = new UiActions();
  const recaptcha = new Recaptcha();

  const recap = recaptcha.check(wrapper);
  if (!recap)
    return helperFunc.showOneAlert(wrapper, 'msgRecap');    

  const mailchimp = wrapper.classList.contains('app-mobius5-mailchimp');

  data.Recaptcha = recap;
  data.MailChimp = mailchimp;

  helperFunc.disableInputs(wrapper, true);
  helperFunc.showOneAlert(wrapper, 'msgSending');

  sendForm(data, wrapper)
}

function sendForm(data: any, wrapper: HTMLElement) {
  const helperFunc = new UiActions();
  const ws = (wrapper as HTMLElement).dataset.webservice; // should be "Form/ProcessForm" or a custom override
  const btn = (wrapper.querySelector('.btn-send-mobius5-form') as HTMLElement);
  const label = btn.innerText;
  const sxc = $2sxc(btn);

  if(debug) console.log(data);
  
  var saveCall = sxc.webApi.post(ws, null, data, true);

  function showSuccess() {
    const msg = data.mailchimp ? 'msgNewsletterSuccess' : 'msgOk';
    helperFunc.showOneAlert(wrapper, msg);

    // Send browser event in case an Analytics-listener wants to get notified
    const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'success', label: label } });
    document.dispatchEvent(trackingEvent);
  }
  
  function showError() {
    const msg = data.mailchimp ? 'msgNewsletterFailed' : 'msgError';
    helperFunc.showOneAlert(wrapper, msg);
    helperFunc.disableInputs(wrapper, false);

    // Send browser event in case an Analytics-listener wants to get notified
    const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'error', label: label } });
    document.dispatchEvent(trackingEvent);
  }
  
  saveCall.success((successData: unknown) => {
      if(debug) console.log('success', successData);
      showSuccess();
    });
  saveCall.error((errorData: unknown) => {
      if(debug) console.log('error', errorData);
      showError();
    });
}