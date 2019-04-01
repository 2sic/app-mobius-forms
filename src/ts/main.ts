export class App {
    c = {
        clsWrp: 'app-jqfs-wrapper',
        clsForm: 'app-jqfs-form',
        iProp: 'item-property',
    };

    alreadyInit = false;


    constructor(moduleId: number) {
        // disable validate on the global asp.net form, to not interfere with the contact-form
        $('form').attr('novalidate', '');
    }

    public initialize() {
        const wrappers = $('.' + this.c.clsWrp);

        // attach validation to enable as soon as we blur
        wrappers.on('blur', ':input', this.attachFieldValidateOnce );

        wrappers.each((i, item) => {
            // prevent dupl execution
            if(this.alreadyInit) 
                return;

            const wrap = $(item);
            wrap.find('#sendFormWithApi').on('click', (evt) => this.send(evt) );  // handle click event
            
            this.alreadyInit = true;
        });
    }

    public send(event: any) {
        const data = []; 
        const btn = event.currentTarget;
        const sxc = (window as any).$2sxc(btn);
        const wrapper = this.findWrapper(btn);

        
        // clear all alerts
        this.showOneAlert(wrapper, '');
        
        // // Validate form
        // if (!wrapper.smkValidate())
        //     return this.showOneAlert(wrapper, 'msgIncomplete');

        // // Do Recaptcha test, show alert & fail if required and not complete
        // const recap = window.appJqRecap && window.appJqRecap.check(wrapper);
        // if(window.appJqRecap && !recap) 
        //     return this.showOneAlert(wrapper, 'msgRecap');            

        // // get data 
        // // data = manuallyBuildData(wrapper); // alternative example with manual build, but we prefer automatic
        this.autoCollectData(wrapper).then((data: any) => {
            // data.Recaptcha = recap;

            // submission
            this.disableInputs(wrapper, true);
            this.showOneAlert(wrapper, 'msgSending'); // show "sending..."
            const ws = wrapper.data('webservice');   // should be "Form/ProcessForm" or a custom override
            sxc.webApi.post(ws, {}, data, true)
                .success(() => {
                    this.showOneAlert(wrapper, 'msgOk')
                    $(btn).hide();
                    // wrapper.find('.' + c.clsForm).hide();
                })
                .error(() => {
                    this.showOneAlert(wrapper, 'msgError')
                    this.disableInputs(wrapper, false);
                });
        });
    }

    public findWrapper(e: any) {
        return $(e).closest('.' + this.c.clsWrp);
    }

    private showOneAlert(wrapper: JQuery, showId: string) {
        wrapper.find('.alert').hide();
        if (showId !== '') {
            wrapper.find('#' + showId).show();
        }
    }

    private disableInputs(wrapper: JQuery, state: boolean) {
        wrapper.toggleClass('disable', state)
        wrapper.find(':input').prop('disabled', state);
    }

    // automatically build the send-object with all properties, 
    // based on all form-fields which have a item-property=""
    private autoCollectData(wrapper: JQuery) {
        
        const data = {
            Files: []
        };
        const fields = $(wrapper).find(':input');

        function add(i: number, e: any) {
            e = $(e);
            // get the property name from special-attribut, name OR id
            const propName = e.attr('name') || e.attr('id');
            
            if (!propName)
                return;
            
            // extract data from file fields
            if (e.attr('type') && e.attr('type').toLowerCase() == 'file') {
                const deferred = $.Deferred();
                const file = e.get(0).files[0];
                if (!file)
                    return;
                const reader = new FileReader();

                reader.addEventListener('load', function () {
                    data.Files.push({
                        Encoded: reader.result,
                        Name: file.name,
                        Field: propName
                    });
                    deferred.resolve();
                }, false);
                reader.readAsDataURL(file);
                return deferred.promise();
            }
            else { // For all standard fields, set value directly
                data[propName] = e.val();
            }
        }
        const promises = fields.map((i, field) => add(i, field));

        return $.when.apply($, promises).then(() => {
            return data;
        });
    }

    private manuallyBuildData(wrapper: JQuery){
        const data = {
                Subject: wrapper.find('#subject'),
                Message: wrapper.find('#message'),
                SenderName: wrapper.find('#sendername'),
                SenderMail: wrapper.find('#sendermail')
        };

        for (let prop in data) 
            if (data.hasOwnProperty(prop)) 
                data[prop] = data[prop].val();

        return data;
    }

    private attachFieldValidateOnce() {
        // skif if validation is already enabled
        if ($(this).data('alreadyRun'))
            return;

        // not yet enabled, let's enable and remember...
        // $(this).smkValidate();
        $(this).data('alreadyRun', true);
    }
}
