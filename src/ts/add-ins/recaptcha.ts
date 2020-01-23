declare let grecaptcha: any;

import { UiActions } from './uiActions';

export class Recaptcha {
  helperFunc = new UiActions();

  c  = {
    recapId: "recapId",
    clsRecap: "g-recaptcha"
  };

  constructor() { }

  /*
    Initialize Recaptcha and create a Recapcha Checkbox below the Formfields 
  */
  init(wrapper: JQuery) {
    const recap = wrapper.find(this.c.clsRecap);

    if(!isNaN(wrapper.data(this.c.recapId))) {
        return;
    }

    const id = grecaptcha.render(recap, {
      'sitekey' : recap.data("sitekey"),
      'size' : 'normal'
    });

    wrapper.data(this.c.recapId, id); // remember for later use       
  }


  
  /* 
    Checks if a recaptcha is implemented in the current Form
  */
  check(wrapper: JQuery) {
    const recap = wrapper.find("." + this.c.clsRecap);
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