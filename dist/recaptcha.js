// this file helps 
// 1. initialize recaptchas, especilly if there are multiple forms with own recaptchas
// 2. verify / validate that it was filled in
$(function(){
    // recaptcha constants
    var c  = {
        recapId: "recId",
        clsRecap: "app-jqfs-recaptcha",
        clsRecapWrap: "app-recaptcha-wrapper"
    };

    // short callback for the Google Recaptcha after it loads
    window.appJqRecap = function(){
        window.appJqRecap.init();
    }

    // intialize all of our recaptchas on this page
    window.appJqRecap.init = function init() {
        // find all
        $("." + c.clsRecap).each(function(i, e){
            var $e = $(e), 
                wrapper = $e.closest("." + c.clsRecapWrap);
            
            // skip if already initialized
            if(!isNaN(wrapper.data(c.recapId)))  
                return;

            // setup recaptcha and remember the id
            var id = grecaptcha.render(e, {
                'sitekey' : $e.data("sitekey"),
                'size' : 'normal'
            });
            wrapper.data(c.recapId, id); // remember for later use
        })
    }

    // JS-check recaptcha, if enabled
    // this check is meant to help the UI show a warning before sending
    // it's not a security feature...
    // ...because the server will also verify the recaptcha
    window.appJqRecap.check = function(wrapper) {
        var recap = $(wrapper).find("." + c.clsRecap);
        // if no recaptcha found, probably ok
        if(recap.length === 0) return true;

        // if many found, probably not ok
        if(recap.length !== 1) throw "recaptcha not found";

        // return google response for the recap
        var res = grecaptcha.getResponse($(wrapper).data(c.recapId)); // null if failed, something cryptic if ok
        return res || false; 
    }


});