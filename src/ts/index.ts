import { showAlert, showConfigWarnings } from './lib-2sxc-alerts';
import { disableInputs, enableInputs, getFormValues, sendForm, validateForm } from './lib-2sxc-forms';
import { getRecaptchaToken, requiresRecaptcha } from './lib-2sxc-recaptcha';
import { addTrackingEvent } from './lib-2sxc-tracking';

const debug = true;

var winAny = window as any;
winAny.appMobius5 ??= {};
winAny.appMobius5.init ??= initAppMobius5;

function initAppMobius5({ domAttribute } : { domAttribute: string }) {
  if (document.getElementsByTagName('form').length) document.getElementsByTagName('form')[0].setAttribute('novalidate', '');
  if (debug) console.log("Mobius5 loading, debug is enabled");

  const mobiusWrapper = document.querySelectorAll(`[${domAttribute}]`)[0];

  if(!mobiusWrapper) return

  const submitButtom = (mobiusWrapper.querySelectorAll('[app-mobius5-send]')[0] as HTMLButtonElement)
  submitButtom.addEventListener('click', async (event: Event) => {
    event.preventDefault();

    const eventBtn = event.currentTarget as HTMLElement;
    addTrackingEvent('trackMobiusForm', 'mobius-form', eventBtn.innerText)
    
    var valid = validateForm(mobiusWrapper)
    if (!valid) {
      showAlert(mobiusWrapper, 'msgIncomplete')
      return
    }
    
    const formValues = await getFormValues(mobiusWrapper)
    console.log(formValues)

    if (requiresRecaptcha(mobiusWrapper)) {
      let token = await getRecaptchaToken(mobiusWrapper)
      if (!token) return showAlert(mobiusWrapper, 'msgRecap')
  
      // set token for backend
      formValues.Recaptcha = token
    }

    const mailchimp = mobiusWrapper.classList.contains('app-mobius5-mailchimp');
    formValues.MailChimp = mailchimp; 

    // imply that message is sending by ui modifications 

    disableInputs(mobiusWrapper, true)
    showAlert(mobiusWrapper, 'msgSending')
      
    //#region request handling

    let endpoint = (mobiusWrapper as HTMLElement).dataset.webservice // (should be "Form/ProcessForm" or a custom override)

    sendForm(formValues, submitButtom, endpoint) 
      .then((result: any) => {
        // error
        if(!result.ok) {
          if(debug) console.log('error', result.status);
    
          showAlert(mobiusWrapper, 'msgError')
          showConfigWarnings(mobiusWrapper, 'app-mobius5-config-warning')
          enableInputs(mobiusWrapper)
    
          addTrackingEvent('trackMobiusForm', 'mobius-form', submitButtom.innerText)
          return
        }
        
        // success
        if(debug) console.log('success', result.json())
        submitButtom.setAttribute("disabled", "")
  
        showAlert(mobiusWrapper, 'msgOk')
        showConfigWarnings(mobiusWrapper, 'app-mobius5-config-warning')
        disableInputs(mobiusWrapper, false)
  
        addTrackingEvent('trackMobiusForm', 'mobius-form', submitButtom.innerText)
      })

    //#endregion
  })
}