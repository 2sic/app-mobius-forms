export class UiActions {
  showOneAlert(wrapper: Element, responseMessageElement: string) {
    wrapper.querySelectorAll('.alert').forEach((elem: HTMLElement, index) => {
      elem.style.display = 'none'
    });
    
    if (responseMessageElement !== '') {
      (wrapper.querySelector(`#${responseMessageElement}`) as HTMLElement).style.display = 'block';
    }
  }

  showConfigWarnings(wrapper: Element) {
    wrapper.querySelectorAll('[app-mobius5-config-warning]')
      .forEach((elem: HTMLElement) => elem.style.display = 'block');
  }

  disableInputs(wrapper: Element, state: boolean) {
    wrapper.classList.toggle('disable', state)
    wrapper.querySelectorAll('input,textarea,select').forEach((elem: HTMLElement, index) => {
      elem.setAttribute('disabled', 'true');
    })
  }
}