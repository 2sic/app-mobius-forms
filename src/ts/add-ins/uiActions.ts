// rename to UiActions
export class UiActions {
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

  attachFieldValidateOnBlur() {
    // skif if validation is already enabled
    if ($(this).data('alreadyRun')) return;

    // not yet enabled, let's enable and remember...
    ($(this) as any).smkValidate();
    $(this).data('alreadyRun', true);
  }
}