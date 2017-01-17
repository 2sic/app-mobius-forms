

$(function(){

    // constants
    var c  = {
        clsWrp: "app-jqfs-wrapper",
        useRecapId: "useRecaptcha",
        recapId: "recId",
        iProp: "item-property",
        clsForm: "app-jqfs-form"
    };


    // we need a global object, so that the init can be called again when the
    // app is added to the page by ajax
    var jqfs = window.appJqFormSimple = {
        val: function() {
            var inputsInForm = $("." + c.clsForm + " :input");

            inputsInForm.each(function() {
                console.log("elem : " + $(this).attr("item-property") + " -  valid : " + $(this).valid());
            });
        },

        // the main send method
        send: function() {

            var data, sxc = $2sxc(this),
                wrapper = $(this).closest("." + c.clsWrp);
            
            // clear all alerts
            jqfs.alerts(wrapper);

            // todo: validate form
            var val = wrapper[0].validator; // pre-initialized validator... wrapper.validate();
            
            // ToDo: Change to the custom validation function
            // console.log("valid: " + val.valid());
            jqfs.val();
            
            // return;

            // Do Recaptcha test
            var recap = jqfs.recap.check(wrapper);

            // show alert if recap required and not complete
            if(!recap)
                return jqfs.alerts(wrapper, "msgRecap");            

            // get data 
            // data = manuallyBuildData(wrapper); // example with manual build, but we prefer automatic
            data = jqfs.autoCollectData(wrapper);
            data.Recaptcha = recap;

            // dddddddddddddd
            /*jqfs.disable(wrapper, true);
            sxc.webApi.post("Form/ProcessForm", {}, data, true)
                .success(function() {
                    jqfs.alerts(wrapper, "msgOk")
                    wrapper.find("." + c.clsForm).hide();
                })
                .error(function() {
                    jqfs.alerts(wrapper, "msgError")
                    jqfs.disable(wrapper, false);
                });*/
        },

        disable: function(wrapper, state) {
            wrapper.find(":input").attr("disabled", state);
        },

        alerts: function(wrapper, showId){
            wrapper.find(".alert").hide();
            wrapper.find("#" + showId).show();
        },

        // automatically build the send-object with all properties, 
        // based on all form-fields which have a item-property=""
        autoCollectData: function(wrapper) {
            var data = {}, fields = $(wrapper).find(":input");//.find("[" + c.iProp + "]");
            function add(i, e) {
                e = $(e);
                // get the property name from special-attribut, name OR id
                var propName = e.attr(c.iProp) || e.attr("name") || e.attr("id");
                if(propName)
                    data[propName] = e.val();
            }
            fields.each(add);
            return data;
        },

        // recaptcha handling
        recap: {
            // define the method which binds the recaptcha to the DOM
            register: function (recaptchaId, wrapper, sitekey) {
                var id = grecaptcha.render(recaptchaId, {
                    'sitekey' : sitekey,
                    'size' : 'normal'
                });
                wrapper.data(c.recapId, id); // remember for later use
            },

            // JS-check recaptcha, if enabled
            check: function(wrapper) {
                var useField = $(wrapper).find("#" + c.useRecapId);
                if(useField.length === 0 || !$(useField).val())
                    return true;
                var recap = $(wrapper).find(".app-recaptcha");
                if(recap.length !== 1)
                    throw "recaptcha not found";
                var res = grecaptcha.getResponse($(wrapper).data(c.recapId)); // null if failed, something cryptic if ok
                return res || false; 
            },
        },


        // init all jqfs on the page
        init: function() {
            var wrappers = $("." + c.clsWrp);
            wrappers.each(function(){
                // prevent dupl execution
                if(this.alreadyInit) 
                    return;

                jqfs.val();

                var wrap = $(this);
                wrap.find("#submit").click(jqfs.send);  // handle click event
                
                // ToDo: Change the selector
                this.validator = $("form").validate({
                    invalidHandler: function(form) {
                        form.submit();
                    },
                    submitHandler: function(form) {
                        // do other things for a valid form
                        form.submit();
                    }
                });
                
                this.alreadyInit = true;
            });
        }
    }


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

});