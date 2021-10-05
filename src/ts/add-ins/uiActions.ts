export class UiActions {
  showOneAlert(wrapper: Element, showId: string) {
    wrapper.querySelectorAll('.alert').forEach((elem: HTMLElement, index) => {
      elem.style.display = 'none'
    });
    
    if (showId !== '') {
      (wrapper.querySelector(`#${showId}`) as HTMLElement).style.display = 'block';
    }
  }

  disableInputs(wrapper: Element, state: boolean) {
    wrapper.classList.toggle('disable', state)
    wrapper.querySelectorAll('input,textarea,select').forEach((elem: HTMLElement, index) => {
      elem.setAttribute('disabled', 'true');
    })
  }
}