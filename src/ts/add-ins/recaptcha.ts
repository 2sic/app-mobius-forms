declare let grecaptcha: any;

// Checks if a recaptcha is implemented in the current Form and returns promise with token if existing

export function getRecaptchaToken(wrapper: Element) {
  const recap = wrapper.getElementsByClassName('app-mobius5-g-recaptcha')[0] as HTMLElement;

  if(!recap) return Promise.resolve(null);
  
  // if many found, probably not ok
  if(wrapper.getElementsByClassName('app-mobius5-g-recaptcha').length !== 1) throw "recaptcha not found";

  // return promise of google response for the recap
  // null if failed, something cryptic if ok
  return grecaptcha.execute(recap.dataset.sitekey, {action: 'submit'})
}

export function requiresRecaptcha(wrapper: Element): boolean {
  if (wrapper.getElementsByClassName('app-mobius5-g-recaptcha')[0]) return true;
  return false;
}