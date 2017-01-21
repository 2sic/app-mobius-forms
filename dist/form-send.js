

$(function(){
    // disable validate on the global asp.net form, to not interfere with the contact-form
    $("form").attr("novalidate", "");

    // constants
    var c  = {
        clsWrp: "app-jqfs-wrapper",
        useRecapId: "useRecaptcha",
        recapId: "recId",
        iProp: "item-property",
        clsForm: "app-jqfs-form",
        clsRecap: "app-jqfs-recaptcha"
    };


    // we need a global object, so that the init can be called again when the
    // app is added to the page by ajax
    var jqfs = window.appJqF = {
        findWrapper: function(e){
            return $(e).closest("." + c.clsWrp);
        },
        // the main send method
        send: function() {

            var data, sxc = $2sxc(this),
                wrapper = jqfs.findWrapper(this);//  $(this).closest("." + c.clsWrp);
            
            // clear all alerts
            jqfs.alerts(wrapper);
            
            // Validate form
            if (!wrapper.smkValidate())
                return jqfs.alerts(wrapper, "msgIncomplete");

            // Do Recaptcha test, show alert & fail if required and not complete
            var recap = jqfs.recap.check(wrapper);
            if(!recap) 
                return jqfs.alerts(wrapper, "msgRecap");            

            // get data 
            // data = manuallyBuildData(wrapper); // alternative example with manual build, but we prefer automatic
            data = autoCollectData(wrapper);
            data.Recaptcha = recap;

            // submission
            jqfs.disable(wrapper, true);
            jqfs.alerts(wrapper, "msgSending"); // show "sending..."
            var ws = wrapper.attr("data-ws");   // should be "Form/ProcessForm" or a custom override
            sxc.webApi.post(ws, {}, data, true)
                .success(function() {
                    jqfs.alerts(wrapper, "msgOk")
                    wrapper.find("." + c.clsForm).hide();
                })
                .error(function() {
                    jqfs.alerts(wrapper, "msgError")
                    jqfs.disable(wrapper, false);
                });
        },

        disable: function(wrapper, state) {
            wrapper.find(":input").attr("disabled", state);
        },

        alerts: function(wrapper, showId){
            wrapper.find(".alert").hide();
            wrapper.find("#" + showId).show();
        },

        // recaptcha handling
        recap: {
            // define the method which binds the recaptcha to the DOM
            register: function () { // sitekey) { // recaptchaId, wrapper, sitekey) {
                // find all
                $("." + c.clsRecap).each(function(i, e){
                    var wrapper = jqfs.findWrapper(e);
                    var sitekey = $(e).data("sitekey");
                    if(isNaN(wrapper.data(c.recapId))) {  // only do if not init yet...
                        var id = grecaptcha.render(e, {
                            'sitekey' : sitekey,
                            'size' : 'normal'
                        });
                        wrapper.data(c.recapId, id); // remember for later use
                    }
                    else
                        grecaptcha.reset(wrapper.data(c.recapId)); // this is necessary, in case the google-script loads again
                })
            },

            // JS-check recaptcha, if enabled
            check: function(wrapper) {
                var useField = $(wrapper).find("#" + c.useRecapId);
                if(useField.length === 0 || !$(useField).val())
                    return true;
                var recap = $(wrapper).find("." + c.clsRecap);
                if(recap.length !== 1)
                    throw "recaptcha not found";
                var res = grecaptcha.getResponse($(wrapper).data(c.recapId)); // null if failed, something cryptic if ok
                return res || false; 
            },
        },


        // init all jqfs on the page
        init: function() {
            var wrappers = $("." + c.clsWrp);

            // attach validation to enable as soon as we blur
            wrappers.on("blur", ":input", attachFieldValidateOnce );

            wrappers.each(function(){
                // prevent dupl execution
                if(this.alreadyInit) 
                    return;

                var wrap = $(this);
                wrap.find("#submit").click(jqfs.send);  // handle click event
                
                this.alreadyInit = true;
            });
        }
    }

    // automatically build the send-object with all properties, 
    // based on all form-fields which have a item-property=""
    function autoCollectData(wrapper) {
        var data = {}, fields = $(wrapper).find(":input");
        function add(i, e) {
            e = $(e);
            // get the property name from special-attribut, name OR id
            var propName = e.attr(c.iProp) || e.attr("name") || e.attr("id");
            if(propName)
                data[propName] = e.val();
        }
        fields.each(add);
        return data;
    };

    // example using code to manually build a custom data-object
    // use something like this, if your form is very sophisticated  
    function manuallyBuildData(wrapper){
        var data = {
                Subject: wrapper.find("#subject"),
                Message: wrapper.find("#message"),
                SenderName: wrapper.find("#sendername"),
                SenderMail: wrapper.find("#sendermail")
        };

        for (var prop in data) 
            if (data.hasOwnProperty(prop)) 
                data[prop] = data[prop].val();

        return data;
    }

    function attachFieldValidateOnce() {
        // skif if validation is already enabled
        if ($(this).data("alreadyRun"))
            return;
        // not yet enabled, let's enable and remember...
        $(this).smkValidate();
        $(this).data("alreadyRun", true);
    }


});