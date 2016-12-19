// define the method which binds the recaptcha to the DOM
function appJqFormReCaptcha(recaptchaId, sitekey) {
    grecaptcha.render(recaptchaId, {
        'sitekey' : sitekey
    });
};

$(function(){
    // Remove all Help blocks at every refresh
    $(".help-block").css("display", "none");

        
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


    // we need a global object, so that the init can be called again when the
    // app is added to the page by ajax
    var jqfs = window.appJqFormSimple = {
        
        // the main send method
        send: function() {
            var sxc = $2sxc(this);

            var container = $(".app-jqfs-id" + sxc.id), 
                data = manuallyBuildData(container);


            console.log("will send", data);
            // sxc.webApi.post("Form/ProcessForm", {}, data);

            console.log("sent");
        },

        // automatically build the send-object with all properties, 
        // based on all form-fields which have a item-property=""
        autoCollectData: function(wrapper) {

        },

        // init all jqfs on the page
        init: function() {
            $(".app-jqfs-wrapper #submit").click(jqfs.send);    
        }   
    }


    function sendContactSimple(){
    }

    function manuallyBuildData(container){
        var data = {
                Subject: container.find("#subject"),
                Message: container.find("#message"),
                SenderName: container.find("#sendername"),
                SenderMail: container.find("#sendermail")
        };

        for (var prop in data) 
            if (data.hasOwnProperty(prop)) 
                data[prop] = data[prop].val();

        return data;
    }

});