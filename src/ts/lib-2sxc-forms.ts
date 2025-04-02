import { SxcGlobal } from "@2sic.com/2sxc-typings";
import { PristineOptions } from "./lib-2sxc-pristine-options";

let Pristine = require("../../node_modules/pristinejs");
declare let $2sxc: SxcGlobal;

/*
  This is a shared code used in various 2sxc apps. Make sure that they are in sync, so if you improve it, improve all 2sxc apps which use this. 
  ATM they are:
  - EventsAndCourses6
  - MobiusForms6
  - Jobs
  The master with the newest / best version must always be the Gallery7, the others should then get a fresh copy.
  Because this is shared, all parameters like DOM-IDs etc. must be provided in the Init-call that it can work across apps
*/

const promiseFileMap = (file: { Encoded: Promise<unknown> }) => {
  if (!file.Encoded) return file;
  return new Promise((resolve) => {
    file.Encoded.then((result) => resolve({ ...file, Encoded: result }));
  });
};

// automatically build the send-object with all properties,
// based on all form-fields which have a item-property=""
export async function getFormValues(formWrapper: Element): Promise<any> {
  let data: any = {
    Files: [],
    Fields: {},
    Terms: {},
    CustomerMails: "",
  };

  const fields = formWrapper.querySelectorAll("input,textarea,select");

  fields.forEach((formField: Element) => {
    const formFieldElement = formField as HTMLInputElement;
    const fieldKey = getFieldKey(formFieldElement);
    if (!fieldKey || !formFieldElement.value) return;

    // for checkboxList to get a array with fieldId and the values
    if (formField.hasAttribute("data-checkbox")) {
      const fieldId = formField.getAttribute("data-checkbox");
      const fieldValue = getFieldValue(formFieldElement);
      // Check if the array for this fieldId exists, if not, create it in the data object

      if (fieldId) {
        if (!data["Fields"][fieldId]) {
          data["Fields"][fieldId] = [];
        }

        if (fieldValue !== "") {
          data["Fields"][fieldId].push(fieldValue);
        }
      }
    }

    // for Multiple Dropdown to get a array with fieldId and the values
    if (
      formField.hasAttribute("data-multiple-dropdown") &&
      formField.tagName == "SELECT"
    ) {
      const fieldId = formField.id; // Use the ID of the select element
      const selectedOptions = (formField as any).selectedOptions;

      // Check if the array for this fieldId exists, if not, create it in the data object
      if (!data["Fields"][fieldId]) {
        data["Fields"][fieldId] = [];
      }
      // loop for each option Value
      for (let i = 0; i < selectedOptions.length; i++) {
        const optionValue = selectedOptions[i].value;
        if (optionValue !== "") {
          data["Fields"][fieldId].push(optionValue);
        }
      }
      return;
    }

    // if it has a recipient email
    if (formField.getAttribute("mail") === "recipientEmail") {
      if (data["CustomerMails"] !== "") {
        data["CustomerMails"] += "; ";
      }
      data["CustomerMails"] += getFieldValue(formFieldElement);
    }

    // If it is an attachment then add it to Files
    if (
      formField.getAttribute("type") &&
      formField.getAttribute("type")?.toLowerCase() == "file"
    ) {
      data["Files"].push({
        ...(getFieldValue(formFieldElement) as object),
        Field: fieldKey,
      });
      return;
    }
    // If Checkbox or Radio not checked, data will not add in the Request
    if (
      formField.getAttribute("type") &&
      (formField.getAttribute("type")?.toLowerCase() == "checkbox" ||
        formField.getAttribute("type")?.toLowerCase() == "radio") &&
      !(formField as HTMLInputElement).checked
    ) {
      return;
    }
    // If the type is a checkbox with an attribute terms, then it will add it to the terms

    if (
      formField.getAttribute("type") &&
      formField.getAttribute("type")?.toLowerCase() == "checkbox" &&
      formField.getAttribute("terms")
    ) {
      data["Terms"][fieldKey] = getFieldValue(formFieldElement);
      return;
    } else if (!formField.hasAttribute("data-checkbox")) {
      // If it is a normal field, e.g. first name, then it is added to the field.
      data["Fields"][fieldKey] = getFieldValue(formFieldElement);
    }
  });

  return Promise.all(data.Files.map(promiseFileMap)).then((loadedFiles) => {
    return { ...data, Files: loadedFiles };
  });
}

function getFieldKey(formField: HTMLInputElement): string {
  // get the property name from special-attribute, name OR id
  return formField.getAttribute("name") ?? formField.getAttribute("id") ?? "";
}

function getFieldValue(
  formField: HTMLInputElement
): { Encoded: Promise<unknown>; Name: string } | unknown {
  // extract data from file fields
  if (!formField.getAttribute("type")) return formField.value;
  switch (formField.getAttribute("type")?.toLowerCase()) {
    case "file":
      const file = formField.files ? formField.files[0] : null;
      if (!file) return;
      return {
        Name: file.name,
        Encoded: new Promise((resolve) => {
          const reader = new FileReader();
          reader.readAsDataURL(file);
          reader.onload = (e) => {
            if (e.target) {
              resolve(e.target.result);
            }
          };
        }),
      };
    case "radio":
      return formField.value;
    case "checkbox":
      return formField.checked ? formField.value : "";
    default:
      return formField.value;
  }
}

export function validateForm(
  formWrapper: Element,
  options: PristineOptions
): boolean {
  const pristine = new Pristine(formWrapper, options);
  return pristine.validate();
}

export function sendForm(
  formData: any,
  submitButtom: HTMLButtonElement,
  endpoint: string = "Form/ProcessForm"
): Promise<unknown> {
  const sxc = $2sxc(submitButtom);
  return sxc.webApi.fetchRaw(endpoint, formData);
}

export function disableInputs(wrapper: Element, state: boolean) {
  wrapper.classList.toggle("disable", state);
  wrapper
    .querySelectorAll("input,textarea,select,button")
    .forEach((elem) => (elem as HTMLElement).setAttribute("disabled", "true"));
}

export function enableInputs(wrapper: Element) {
  wrapper.classList.remove("disable");
  wrapper
    .querySelectorAll("input,textarea,select,button")
    .forEach((elem) => (elem as HTMLElement).setAttribute("disabled", "false"));
}
