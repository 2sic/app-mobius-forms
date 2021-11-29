import { getRecaptchaToken, requiresRecaptcha } from './add-ins/recaptcha';
import { UiActions } from './add-ins/uiActions';
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

  mobiusWrapper.querySelectorAll('[app-mobius5-send]')[0].addEventListener('click', (event: Event) => {
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

async function sendWhenReady(data: any, wrapper: HTMLElement) {
  const helperFunc = new UiActions();
  
  if (requiresRecaptcha(wrapper)) {
    let token = await getRecaptchaToken(wrapper)
    if (!token) return helperFunc.showOneAlert(wrapper, 'msgRecap');    

    // set token for backend
    data.Recaptcha = token;
  }

  const mailchimp = wrapper.classList.contains('app-mobius5-mailchimp');
  data.MailChimp = mailchimp;

  helperFunc.disableInputs(wrapper, true);
  helperFunc.showOneAlert(wrapper, 'msgSending');

  sendForm(data, wrapper)
}

function sendForm(data: any, wrapper: HTMLElement) {
  const helperFunc = new UiActions();
  const ws = (wrapper as HTMLElement).dataset.webservice; // should be "Form/ProcessForm" or a custom override
  const btn = (wrapper.querySelectorAll('[app-mobius5-send]')[0] as HTMLButtonElement);
  const label = btn.innerText;
  const sxc = $2sxc(btn);
  
  if(debug) console.log(data);
  
  sxc.webApi.fetch(ws, data)
  .then((result: any) => {
    // error
    if(!result.ok) {
      if(debug) console.log('error', result.status());
      showError();
      return
    }
    
    // success
    if(debug) console.log('success', result.json());
    btn.setAttribute("disabled", "")
    showSuccess();
  })

  
  function showSuccess() {
    const msg = data.mailchimp ? 'msgNewsletterSuccess' : 'msgOk';
    helperFunc.showOneAlert(wrapper, msg);
    helperFunc.showConfigWarnings(wrapper);
    
    // Send browser event in case an Analytics-listener wants to get notified
    const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'success', label: label } });
    document.dispatchEvent(trackingEvent);
  }
  
  function showError() {
    const msg = data.mailchimp ? 'msgNewsletterFailed' : 'msgError';
    helperFunc.showOneAlert(wrapper, msg);
    helperFunc.showConfigWarnings(wrapper);
    helperFunc.disableInputs(wrapper, false);

    // Send browser event in case an Analytics-listener wants to get notified
    const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'error', label: label } });
    document.dispatchEvent(trackingEvent);
  }
}