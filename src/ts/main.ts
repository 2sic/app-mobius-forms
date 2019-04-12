declare let $2sxc: any;

import { Helpers } from './components/helpers';
import { Recaptcha } from  './components/recaptcha';
import { MailChimp } from './components/mailchimp';
export class App {
    helper = new Helpers();
    recaptcha = new Recaptcha();
    mailChimp = new MailChimp();
    moduleWrapper: JQuery;
    alreadyInit = false;

    c = {
        clsWrp: 'app-jqfs-wrapper',
        clsForm: 'app-jqfs-form',
    };

    constructor(
        moduleId: number,
    ) {
        // disable validate on the global asp.net form, to not interfere with the contact-form
        $('form').attr('novalidate', '');
        this.moduleWrapper = $(`.DnnModule-${moduleId}`);
    }

    public initialize() {
        const wrapper = this.moduleWrapper;
        // attach validation to enable as soon as we blur        
        wrapper.on('blur', ':input', this.attachFieldValidateOnBlur );

        wrapper.each((i, item) => {
            // prevent dupl execution
            if(this.alreadyInit) 
                return;

            const wrap = $(item);
            wrap.find('#sendFormWithApi').on('click', (evt) => this.send(evt) );  // handle click event
            
            this.alreadyInit = true;
        });

        this.mailChimp.init(wrapper);
    }

    public send(event: any) {
        let data = []; 
        const btn = event.currentTarget;
        const sxc = $2sxc(btn);
        const wrapper = this.moduleWrapper;
        
        // clear all alerts
        this.helper.showOneAlert(wrapper, '');
        
        // Validate form
        if (!(wrapper as any).smkValidate())
            return this.helper.showOneAlert(wrapper, 'msgIncomplete');

        // Do Recaptcha test, show alert & fail if required and not complete
        const recap = this.recaptcha.check(wrapper);
        if(!recap) 
            return this.helper.showOneAlert(wrapper, 'msgRecap');  

        // get data 
        // data = this.manuallyBuildData(wrapper); // alternative example with manual build, but we prefer automatic
        this.autoCollectData().then((data: any) => {
            const ws = wrapper.find('.app-jqfs-wrapper').data('webservice');   // should be "Form/ProcessForm" or a custom override
            data.Recaptcha = recap;

            // submission
            this.helper.disableInputs(wrapper, true);
            this.helper.showOneAlert(wrapper, 'msgSending'); // show "sending..."

            sxc.webApi.post(ws, {}, data, true)
                .success(() => {
                    this.helper.showOneAlert(wrapper, 'msgOk')
                    $(btn).hide();
                })
                .error(() => {
                    this.helper.showOneAlert(wrapper, 'msgError')
                    this.helper.disableInputs(wrapper, false);
            });
        });
    }

    // automatically build the send-object with all properties, 
    // based on all form-fields which have a item-property=""
    private autoCollectData() {
        const data: any = {
            Files: []
        };
        const fields = this.moduleWrapper.find(':input');

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
            } else if (e.attr('type') && e.attr('type').toLowerCase() == 'radio') {
                if (e.is(':checked')){
                    data[propName] = e.val();    
                }
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
        const data: any = {
            Subject: wrapper.find('#Subject'),
            Message: wrapper.find('#Message'),
            SenderName: wrapper.find('#Sendername'),
            SenderMail: wrapper.find('#Sendermail')
        };


        for (let prop in data) {
            if (data.hasOwnProperty(prop)) {
                data[prop] = data[prop].val();
            }
        }

        return data;
    }

    private attachFieldValidateOnBlur() {
        // skif if validation is already enabled
        if ($(this).data('alreadyRun')) {
            return;
        }

        // not yet enabled, let's enable and remember...
        ($(this) as any).smkValidate();
        $(this).data('alreadyRun', true);
    }
}
