declare let grecaptcha: any;

// Checks if a recaptcha is implemented in the current Form and returns promise with token if existing

export function getRecaptchaToken(wrapper: Element) {
  const recap = wrapper.getElementsByClassName('app-mobius5-g-recaptcha')[0] as HTMLElement;

  // if no recaptcha found, probably ok
  if(!recap) return Promise.resolve(true);
  
  // if many found, probably not ok
  if(wrapper.getElementsByClassName('app-mobius5-g-recaptcha').length !== 1) throw "recaptcha not found";

  // return promise of google response for the recap
  return grecaptcha.execute(recap.dataset.sitekey, {action: 'submit'}) // null if failed, something cryptic if ok
}