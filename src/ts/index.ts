import { showAlert } from "./lib-2sxc-alerts";
import {
  disableInputs,
  enableInputs,
  getFormValues,
  sendForm,
  validateForm,
} from "./lib-2sxc-forms";
import { PristineOptions } from "./lib-2sxc-pristine-options";
import { getRecaptchaToken, requiresRecaptcha } from "./lib-2sxc-recaptcha";
import { addTrackingEvent } from "./lib-2sxc-tracking";

const debug = false;

var winAny = window as any;
winAny.appMobius5 ??= {};
winAny.appMobius5.init ??= initAppMobius5;

function initAppMobius5({
  domAttribute,
  webApiUrl,
  validationOptions,
}: {
  domAttribute: string;
  webApiUrl: string;
  validationOptions: PristineOptions;
}) {
  if (debug) console.log("Mobius5 loading, debug is enabled", domAttribute);
  if (document.getElementsByTagName("form").length)
    document.getElementsByTagName("form")[0].setAttribute("novalidate", "");

  const mobiusWrapper = document.querySelectorAll(`[${domAttribute}]`)[0];

  if (!mobiusWrapper) return;

  const submitButtom = mobiusWrapper.querySelectorAll(
    "[app-mobius5-send]"
  )[0] as HTMLButtonElement;
  submitButtom.addEventListener("click", async (event: Event) => {
    event.preventDefault();

    const eventBtn = event.currentTarget as HTMLElement;
    addTrackingEvent("trackMobiusForm", "mobius-form", eventBtn.innerText);

    var valid = validateForm(mobiusWrapper, validationOptions);
    if (!valid) {
      showAlert(mobiusWrapper, "msgIncomplete");
      return;
    }

    const formValues = await getFormValues(mobiusWrapper);

    if (requiresRecaptcha(mobiusWrapper)) {
      let token = await getRecaptchaToken(mobiusWrapper);
      if (!token) return showAlert(mobiusWrapper, "msgRecap");

      // set token for backend
      formValues.Recaptcha = token;
    }

    const mailchimp = mobiusWrapper.classList.contains("app-mobius5-mailchimp");
    formValues.MailChimp = mailchimp;

    // imply that message is sending by ui modifications

    disableInputs(mobiusWrapper, true);
    showAlert(mobiusWrapper, "msgSending");

    //#region request handling

    let endpoint = webApiUrl; // (should be "Form/ProcessForm" or a custom override)

    sendForm(formValues, submitButtom, endpoint).then((result: any) => {
      // error
      if (!result.ok) {
        if (debug) console.log("error", result.status);

        showAlert(mobiusWrapper, "msgError");
        enableInputs(mobiusWrapper);

        addTrackingEvent(
          "trackMobiusForm",
          "mobius-form",
          submitButtom.innerText
        );
        return;
      }

      // success
      if (debug) console.log("success", result);
      submitButtom.setAttribute("disabled", "");

      showAlert(mobiusWrapper, "msgOk");
      disableInputs(mobiusWrapper, false);

      addTrackingEvent(
        "trackMobiusForm",
        "mobius-form",
        submitButtom.innerText
      );
    });

    //#endregion
  });
}
