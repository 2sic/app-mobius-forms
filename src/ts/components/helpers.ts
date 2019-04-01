export class Helpers {
  showOneAlert(wrapper: JQuery, showId: string) {
    wrapper.find('.alert').hide();
    if (showId !== '') {
        wrapper.find('#' + showId).show();
    }
  }

  disableInputs(wrapper: JQuery, state: boolean) {
    wrapper.toggleClass('disable', state)
    wrapper.find(':input').prop('disabled', state);
  }
}