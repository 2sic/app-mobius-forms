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

  const submitButton = (mobiusWrapper.querySelectorAll('[app-mobius6-send]')[0] as HTMLButtonElement)
  submitButton.style.display = 'none';
  const steps = Array.from(mobiusWrapper.querySelectorAll('.app-mobius-step'));
  const nextStepButtons = mobiusWrapper.querySelectorAll('.btn-mobius-next-step');

  const fadeOut = (el: HTMLElement, callback?: () => void) => {
    el.style.transition = 'opacity 0.3s';
    el.style.opacity = '0';
    setTimeout(() => {
      el.style.display = 'none';
      callback?.();
    }, 300);
  };

  const fadeIn = (el: HTMLElement) => {
    el.style.display = '';
    el.style.opacity = '0';
    el.style.transition = 'opacity 0.3s';
    setTimeout(() => {
      el.style.opacity = '1';
    }, 10);
  };

  const updateStepperState = (targetStepClass: string) => {
    steps.forEach((step) => step.classList.remove('active'));
    const activeStep = mobiusWrapper.querySelector(`.app-mobius-step[data-step="${targetStepClass}"]`);
    activeStep?.classList.add('active');
  };

  const toggleSubmitButton = (show: boolean) => {
    if (show) {
      submitButton.style.display = '';
      fadeIn(submitButton);
    } else {
      fadeOut(submitButton);
    }
  };

  steps.forEach((step: HTMLElement) => {
    step.addEventListener('click', (e) => {
      e.preventDefault();

      const targetClass = step.dataset.step;
      const targetNode = mobiusWrapper.querySelector(`.${targetClass}`) as HTMLElement;
      const activeNode = mobiusWrapper.querySelector('.mobius-group.active') as HTMLElement;
      const prevNode = targetNode?.previousElementSibling as HTMLElement;

      if (targetNode && targetNode !== activeNode && (targetNode.classList.contains('valid') || prevNode?.classList.contains('valid'))) {
        fadeOut(activeNode, () => {
          activeNode.classList.remove('active');
          targetNode.classList.add('active');
          fadeIn(targetNode);
          updateStepperState(targetClass!);
          toggleSubmitButton(!targetNode.nextElementSibling);
        });
      }
    });
  });

  nextStepButtons.forEach((btn: HTMLElement) => {
    btn.addEventListener('click', (e) => {
      e.preventDefault();

      
      const currentGroup = btn.closest('.mobius-group') as HTMLElement;
      const shouldHideCurrentGroup = !currentGroup.parentElement?.classList.contains('mobius-groups-simple'); 

      const validator = new Pristine(currentGroup, validationOptions);

      if (!validator.validate()) return;

      const nextGroup = currentGroup.nextElementSibling as HTMLElement;
      const nextStepKey = nextGroup?.dataset.step;
      const isLastStep = !nextGroup?.querySelector('.btn-mobius-next-step');

      currentGroup.classList.add('valid');
      if (shouldHideCurrentGroup) {
        fadeOut(currentGroup, () => {
          currentGroup.classList.remove('active');
          nextGroup.classList.add('active');
          fadeIn(nextGroup);
          updateStepperState(nextStepKey!);
          toggleSubmitButton(isLastStep);
        });
      } else {
        currentGroup.classList.remove('active');
        nextGroup.classList.add('active');
        fadeIn(nextGroup);
        toggleSubmitButton(isLastStep);
      }
    });
  });

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