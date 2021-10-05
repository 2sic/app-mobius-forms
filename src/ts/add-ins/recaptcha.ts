declare let grecaptcha: any;

import { UiActions } from './uiActions';

export class Recaptcha {
  helperFunc = new UiActions();

  constructor() { }

  /*
    Initialize Recaptcha and create a Recapcha Checkbox below the Formfields 
  */
  init(wrapper: HTMLElement) {
    const recap = wrapper.querySelector('g-recaptcha') as HTMLElement;

    if(!isNaN(parseInt(wrapper.dataset.recapId))) {
        return;
    }

    const id = grecaptcha.render(recap, {
      'sitekey' : recap.dataset.sitekey,
      'size' : 'normal'
    });

    wrapper.setAttribute('recapId', id); // remember for later use       
  }
  
  /* 
    Checks if a recaptcha is implemented in the current Form
  */
  check(wrapper: Element) {
    const recap = wrapper.getElementsByClassName('g-recaptcha');

    // if no recaptcha found, probably ok
    if(recap.length === 0) {
      return true;
    }

    // if many found, probably not ok
    if(recap.length !== 1) {
      throw "recaptcha not found";
    }

    // return google response for the recap
    const res = grecaptcha.getResponse(); // null if failed, something cryptic if ok

    return res || false; 
  }
}