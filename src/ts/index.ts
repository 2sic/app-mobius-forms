import { UiActions } from './add-ins/uiActions';
import { Recaptcha } from './add-ins/recaptcha';
import { CollectFieldsManual } from './collect-fields/manual';
import { CollectFieldsAutomatic } from './collect-fields/auto';

let Pristine = require('../../node_modules/pristinejs')

declare let $2sxc: any;

var winAny = window as any;
winAny.appMobius5 ??= {};
winAny.appMobius5.init ??= initAppMobius5;

function initAppMobius5({ domId } : { domId: string }) {
  const mobuisWrapper = document.getElementsByClassName(domId)[0];
  const helperFunc = new UiActions();

  if (document.getElementsByTagName('form').length) document.getElementsByTagName('form')[0].setAttribute('novalidate', '');

  mobuisWrapper.getElementsByClassName('btn-send-mobius5-form')[0].addEventListener('click', (event: Event) => {
    event.preventDefault();
    validate(mobuisWrapper, event)
  })
}

function validate(wrapper: Element, event: Event) {
  const helperFunc = new UiActions();
  const collectFieldsAutomatic = new CollectFieldsAutomatic();
  const pristine = new Pristine(wrapper);
  const btn = event.currentTarget as HTMLElement;
  const label = btn.innerText;

  helperFunc.showOneAlert(wrapper, '');

  const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'submit', label: label } });
  document.dispatchEvent(trackingEvent);
  
  const valid = pristine.validate();
  if (!valid)
    return helperFunc.showOneAlert(wrapper, 'msgIncomplete');

  collectFieldsAutomatic.collect(wrapper, collectCallback);
}

function collectCallback(data: any, wrapper: HTMLElement) {
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

  console.log(data);
  
  sxc.webApi.post(ws, null, data, true)
    .success(() => {
      const msg = data.mailchimp ? 'msgNewsletterSuccess' : 'msgOk';
      helperFunc.showOneAlert(wrapper, msg);

      const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'success', label: label } });
      document.dispatchEvent(trackingEvent);
    })
    .error(() => {
      const msg = data.mailchimp ? 'msgNewsletterFailed' : 'msgError';
      helperFunc.showOneAlert(wrapper, msg);
      helperFunc.disableInputs(wrapper, false);

      const trackingEvent = new CustomEvent('trackMobiusForm', { detail: { category: 'mobius-form', action: 'error', label: label } });
      document.dispatchEvent(trackingEvent);
    });
}