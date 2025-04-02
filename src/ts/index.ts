import { showAlert } from './lib-2sxc-alerts';
import { disableInputs, enableInputs, getFormValues, sendForm, validateForm } from './lib-2sxc-forms';
import { PristineOptions } from './lib-2sxc-pristine-options';
import { getRecaptchaToken, requiresRecaptcha } from './lib-2sxc-recaptcha';
import { addTrackingEvent } from './lib-2sxc-tracking';
import { initTippy } from './lib-2sxc-tippy';

let Pristine = require("../../node_modules/pristinejs");

const debug = false;

var winAny = window as any;
winAny.appMobius6 ??= {};
winAny.appMobius6.init ??= initAppMobius;
winAny.appMobius6.initTippy ??= initTippy;


function initAppMobius({ domAttribute, webApiUrl, validationOptions } : { domAttribute: string, webApiUrl: string, validationOptions: PristineOptions }) {
  if (debug) console.log("Mobius5 loading, debug is enabled", domAttribute);
  if (document.getElementsByTagName('form').length) document.getElementsByTagName('form')[0].setAttribute('novalidate', '');

  const mobiusWrapper = document.querySelectorAll(`[${domAttribute}]`)[0];

  if(!mobiusWrapper) return

  const nextStepButtons = mobiusWrapper.querySelectorAll('.btn-mobius-next-step');
  nextStepButtons.forEach((btn: Element) => {
    const sendButton = mobiusWrapper.querySelector('[app-mobius6-send]') as HTMLElement;
    sendButton.style.display = 'none';
    
    btn.addEventListener('click', (event: Event) => {
      event.preventDefault();
      const btnElement = btn as HTMLElement;
      const parentNode = btn.closest('.mobius-group') as HTMLElement;
      const stepForm = new Pristine(parentNode, validationOptions);

      if(stepForm.validate()) {
        const closestGroup = parentNode.closest('.mobius-group');
        if (!closestGroup) return;
        const nextStep = closestGroup.nextElementSibling;
        if (!nextStep) return;
        const nextStepButton = nextStep.querySelector('.btn-mobius-next-step');
        
        if(parentNode != null) {
          btnElement.style.display = 'none';
          
          nextStep.classList.add('active');

          if(nextStepButton == null)
            sendButton.style.display = 'block';

        }
      }
    })
  })

  const submitButton = (mobiusWrapper.querySelectorAll('[app-mobius6-send]')[0] as HTMLButtonElement)
  submitButton.addEventListener('click', async (event: Event) => {
    event.preventDefault();

    const eventBtn = event.currentTarget as HTMLElement;
    addTrackingEvent('trackMobiusForm', 'mobius-form', eventBtn.innerText)
    
    var valid = validateForm(mobiusWrapper, validationOptions)
    if (!valid) {
      showAlert(mobiusWrapper, 'msgIncomplete')
      return
    }
    
    const formValues = await getFormValues(mobiusWrapper)

    if (requiresRecaptcha(mobiusWrapper)) {
      let token = await getRecaptchaToken(mobiusWrapper)
      if (!token) return showAlert(mobiusWrapper, 'msgRecap')
  
      // set token for backend
      formValues.Recaptcha = token
    }

    const mailchimp = mobiusWrapper.classList.contains('app-mobius6-mailchimp');
    formValues.MailChimp = mailchimp; 

    // imply that message is sending by ui modifications 

    disableInputs(mobiusWrapper, true)
    showAlert(mobiusWrapper, 'msgSending')
      
    //#region request handling

    let endpoint = webApiUrl // (should be "Form/ProcessForm" or a custom override)

    sendForm(formValues, submitButton, endpoint) 
      .then((result: any) => {        
        // error
        if(!result.ok) {
          if(debug) console.log('error', result.status);
    
          showAlert(mobiusWrapper, 'msgError')
          enableInputs(mobiusWrapper)
    
          addTrackingEvent('trackMobiusForm', 'mobius-form', submitButton.innerText)
          return
        }
        
        // success
        if(debug) console.log('success', result)
  
        showAlert(mobiusWrapper, 'msgOk')
        disableInputs(mobiusWrapper, false)
  
        addTrackingEvent('trackMobiusForm', 'mobius-form', submitButton.innerText)
      })

    //#endregion
  })
}