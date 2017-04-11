$(function() {
    var sxc = $2sxc($("#subToMc"));
    var mid = $("#subToMc").data("mid");

    var inputArray = [];
    $(".app-jqfs-mailchimp-" + mid + " :input").each(function() {
        inputArray.push($(this));
    });

    function allInputsFilled(arr) {
        var notFilled = 0;

        $(arr).each(function() {
            if ($(this).val() != "") {
                notFilled++;
            }
        });

        if (notFilled > 0) {
            return false;
        } else {
            return true;
        }
    }

    var mc = {
        subscribeUser: function (email, fname, lname) {
            sxc.webApi.post("Mailchimp/Subscribe", { email: email, fname: fname, lname: lname }, null, true)
                .success(function (data) {
                    $(".app-jqfs-form-mailchimp").fadeOut();
                    $("#NewsletterSuccessMsg").fadeIn();
                })
                .error(function () {
                    $(".app-jqfs-form-mailchimp").fadeOut();
                    $("#NewsletterFailedMsg").fadeIn();
                })
        }
    }

    $("#subToMc").click(function () {
        if (allInputsFilled(inputArray) == true) {
            $("#FormReminder").fadeIn();
        } else {
            $("#FormReminder").fadeOut();
            mc.subscribeUser($(".app-jqfs-mailchimp-" + mid + " #SenderMail").val(), $(".app-jqfs-mailchimp-" + mid + " #SenderName").val(), $(".app-jqfs-mailchimp-" + mid + " #SenderSurname").val());
        }
        
    })
})