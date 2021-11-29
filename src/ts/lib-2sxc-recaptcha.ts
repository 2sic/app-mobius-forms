declare let grecaptcha: any;

/*
  This is a shared code used in various 2sxc apps. Make sure that they are in sync, so if you improve it, improve all 2sxc apps which use this. 
  ATM they are:
  - EventsAndCourses6
  - MobiusForms5
  The master with the newest / best version must always be the Gallery7, the others should then get a fresh copy.
  Because this is shared, all parameters like DOM-IDs etc. must be provided in the Init-call that it can work across apps
*/ 

// Checks if a recaptcha is implemented in the current Form and returns promise with token if existing

export function getRecaptchaToken(wrapper: Element) {
  const recap = wrapper.getElementsByClassName('recaptcha')[0] as HTMLElement;

  if(!recap) return Promise.resolve(null);
  
  // if many found, probably not ok
  if(wrapper.getElementsByClassName('recaptcha').length !== 1) throw "recaptcha not found";

  // return promise of google response for the recap
  // null if failed, something cryptic if ok
  return grecaptcha.execute(recap.dataset.sitekey, {action: 'submit'})
}

export function requiresRecaptcha(wrapper: Element): boolean {
  if (wrapper.getElementsByClassName('recaptcha')[0]) return true;
  return false;
}