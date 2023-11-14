import { CommandNames } from "@2sic.com/2sxc-typings";
import { PristineOptions } from "./lib-2sxc-pristine-options";

let Pristine = require("../../node_modules/pristinejs");
declare let $2sxc: any;

/*
  This is a shared code used in various 2sxc apps. Make sure that they are in sync, so if you improve it, improve all 2sxc apps which use this. 
  ATM they are:
  - EventsAndCourses6
  - MobiusForms5
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
  };

  const fields = formWrapper.querySelectorAll("input,textarea,select");
  fields.forEach((formField: HTMLInputElement) => {
    const fieldKey = getFieldKey(formField);
    if (!fieldKey || !formField.value) return;

    // If it is an attachment then add it to Files
    if (
      formField.getAttribute("type") &&
      formField.getAttribute("type").toLowerCase() == "file"
    ) {
      data["Files"].push({
        ...(getFieldValue(formField) as object),
        Field: fieldKey,
      });
      return;
    }
    // If Checkbox or Radio not checked, data will not add in the Request 
    if (
      formField.getAttribute("type") &&
      (formField.getAttribute("type").toLowerCase() == "checkbox" ||
        formField.getAttribute("type").toLowerCase() == "radio") &&
      !formField.checked
    ) {
      return;
    }
    // If the type is a checkbox with an attribute terms, then it will add it to the terms
    if (
      formField.getAttribute("type") &&
      formField.getAttribute("type").toLowerCase() == "checkbox" &&
      formField.getAttribute("terms")
    ) {
      data["Terms"][fieldKey] = getFieldValue(formField);
      return;
    }
    // If it is a normal field, e.g. first name, then it is added to the field.
    data["Fields"][fieldKey] = getFieldValue(formField);
  });

  return Promise.all(data.Files.map(promiseFileMap)).then((loadedFiles) => {
    return { ...data, Files: loadedFiles };
  });
}

function getFieldKey(formField: HTMLInputElement): string {
  // get the property name from special-attribute, name OR id
  return formField.getAttribute("name") || formField.getAttribute("id");
}

function getFieldValue(
  formField: HTMLInputElement
): { Encoded: Promise<unknown>; Name: string } | unknown {
  // extract data from file fields
  if (!formField.getAttribute("type")) return formField.value;
  switch (formField.getAttribute("type").toLowerCase()) {
    case "file":
      const file = formField.files[0];
      if (!file) return;
      return {
        Name: file.name,
        Encoded: new Promise((resolve) => {
          const reader = new FileReader();
          reader.readAsDataURL(file);
          reader.onload = (e) => {
            resolve(e.target.result);
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
  // console.log("sendform",formData)
  return sxc.webApi.fetchRaw(endpoint, formData);
}

export function disableInputs(wrapper: Element, state: boolean) {
  wrapper.classList.toggle("disable", state);
  wrapper
    .querySelectorAll("input,textarea,select")
    .forEach((elem: HTMLElement) => elem.setAttribute("disabled", "true"));
}

export function enableInputs(wrapper: Element) {
  wrapper.classList.remove("disable");
  wrapper
    .querySelectorAll("input,textarea,select")
    .forEach((elem: HTMLElement) => elem.setAttribute("disabled", "false"));
}
