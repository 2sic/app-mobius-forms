

$(function(){
    // disable validate on the global asp.net form, to not interfere with the contact-form
    $("form").attr("novalidate", "");

    // constants
    var c  = {
        clsWrp: "app-jqfs-wrapper",
        iProp: "item-property",
        clsForm: "app-jqfs-form"
    };


    // we need a global object, so that the init can be called again when the
    // app is added to the page by ajax
    var jqfs = window.appJqF = {
        findWrapper: function(e){
            return $(e).closest("." + c.clsWrp);
        },

        // the main send method
        send: function() {

            var data, 
                btn = this,
                sxc = $2sxc(btn),
                wrapper = jqfs.findWrapper(btn);
            
            // clear all alerts
            showOneAlert(wrapper);
            
            // Validate form
            if (!wrapper.smkValidate())
                return showOneAlert(wrapper, "msgIncomplete");

            // Do Recaptcha test, show alert & fail if required and not complete
            var recap = window.appJqRecap && window.appJqRecap.check(wrapper);
            if(window.appJqRecap && !recap) 
                return showOneAlert(wrapper, "msgRecap");            

            // get data 
            // data = manuallyBuildData(wrapper); // alternative example with manual build, but we prefer automatic
            data = autoCollectData(wrapper);
            data.Recaptcha = recap;

            // submission
            disableInputs(wrapper, true);
            showOneAlert(wrapper, "msgSending"); // show "sending..."
            var ws = wrapper.data("webservice");   // should be "Form/ProcessForm" or a custom override
            sxc.webApi.post(ws, {}, data, true)
                .success(function() {
                    showOneAlert(wrapper, "msgOk")
                    $(btn).hide();
                    // wrapper.find("." + c.clsForm).hide();
                })
                .error(function() {
                    showOneAlert(wrapper, "msgError")
                    disableInputs(wrapper, false);
                });
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


    function showOneAlert(wrapper, showId) {
        wrapper.find(".alert").hide();
        wrapper.find("#" + showId).show();
    }

    function disableInputs(wrapper, state) {
        wrapper.toggleClass("disable", state)
        wrapper.find(":input").attr("disabled", state);
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