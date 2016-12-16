// define the method which binds the recaptcha to the DOM
function appJqFormReCaptcha(mid, sitekey) {
    console.log("grecaptcha is ready!");
    grecaptcha.render('grecaptcha_' + mid, {
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
        $(".cform-field-required").each(function(){
            $(this).parent().removeClass("has-error");
            
            if ($(this).next().val().length > 0) {
                $(this).parent().addClass("has-success");
                
            } else {
                $(this).parent().addClass("has-error");
                $(this).next().next().fadeIn();
            }
        });
    });

    var submitBtns = $(".cform-wrapper #submit");
    submitBtns.click(sendContactSimple);    

    function sendContactSimple(){
        var sxc = $2sxc(this);

        var container = $(".cform-id" + sxc.id),
            data = {
                Subject: container.find("#subject"),
                Message: container.find("#message"),
                SenderName: container.find("#sendername"),
                SenderMail: container.find("#sendermail")
        };

        for (var prop in data) 
            if (data.hasOwnProperty(prop)) 
                data[prop] = data[prop].val();


        console.log("will send", data);
        sxc.webApi.post("Form/ProcessForm", {}, data);

        console.log("sent");
    }

});