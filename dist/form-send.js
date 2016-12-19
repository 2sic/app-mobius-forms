

$(function(){
    // Remove all Help blocks at every refresh
    // $(".help-block").css("display", "none");

        
    // Event listener for Post btn
    $("#post").click(function() {
        // Remove all Help blocks at every re post
        $(".help-block").fadeOut();
    
        // Validate Required fields
        $(".app-jqfs-field-required").each(function(){
            $(this).parent().removeClass("has-error");
            
            if ($(this).next().val().length > 0) {
                $(this).parent().addClass("has-success");
                
            } else {
                $(this).parent().addClass("has-error");
                $(this).next().next().fadeIn();
            }
        });
    });


    // constants
    var c  = {
        clsWrp: "app-jqfs-wrapper",
        useRecapId: "useRecaptcha",
        recapId: "recId",
        iProp: "item-property"
    };

    // we need a global object, so that the init can be called again when the
    // app is added to the page by ajax
    var jqfs = window.appJqFormSimple = {
        
        // the main send method
        send: function() {

            var data, sxc = $2sxc(this),
                wrapper = $(this).closest("." + c.clsWrp);
            
            // clear all alerts
            jqfs.alerts(wrapper);

            // todo: validate form
            

            var recap = jqfs.recap.check(wrapper);

            // show alert if recap required and not complete
            if(!recap)
                return jqfs.alerts(wrapper, "msgRecap");            

            // get data 
            // data = manuallyBuildData(wrapper); // example with manual build, but we preferr automatic
            data = jqfs.autoCollectData(wrapper);


            console.log('test auto', jqfs.autoCollectData(wrapper));

            console.log("will send", data);
            sxc.webApi.post("Form/ProcessForm", {}, data)
                .success(function() {
                })
                .error(function() {
                });

            console.log("sent");
        },

        alerts: function(wrapper, showId){
            wrapper.find(".alert").hide();
            wrapper.find("#" + showId).show();
        },

        // automatically build the send-object with all properties, 
        // based on all form-fields which have a item-property=""
        autoCollectData: function(wrapper) {
            var data = [], fields = $(wrapper).find("[" + c.iProp + "]");
            function add(i, e) {
                e = $(e);
                data[e.attr(c.iProp)] = e.val();
            }
            fields.each(add);
            return data;
        },

        // recaptcha handling
        recap: {
            // define the method which binds the recaptcha to the DOM
            register: function (recaptchaId, wrapper, sitekey) {
                var id = grecaptcha.render(recaptchaId, {
                    'sitekey' : sitekey
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
            $("." + c.clsWrp + " #submit").click(jqfs.send);    
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