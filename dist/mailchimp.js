$(function() {
    var sxc = $2sxc($("#subToMc"));
    var mid = $("#subToMc").data("mid");
    var wrapper = $(".app-jqfs-mailchimp-wrapper");

    $("form").attr("novalidate", "");

    var mc = {
        subscribeUser: function (email, fname, lname) {
            sxc.webApi.post("Mailchimp/Subscribe", { email: email, fname: fname, lname: lname }, null, true)
                .success(function (response) {
                    console.log(response);
                    $(".app-jqfs-form-mailchimp").fadeOut();
                    $("#NewsletterSuccessMsg").fadeIn();
                })
                .error(function () {
                    $(".app-jqfs-form-mailchimp").fadeOut();
                    $("#NewsletterFailedMsg").fadeIn();
                })
        }
    }

    // automatically build the send-object with all properties, 
    // based on all form-fields which have a item-property=""
    function autoCollectData(wrapper) {
        
        var data = {
            Files: []
        };
        var fields = $(wrapper).find(":input");

        function add(i, e) {
            e = $(e);
            // get the property name from special-attribut, name OR id
            var propName = e.attr("name") || e.attr("id");
            
            if (!propName)
                return;
            
            // extract data from file fields
            if (e.attr('type') && e.attr('type').toLowerCase() == 'file') {
                var deferred = $.Deferred();
                var file = e.get(0).files[0];
                if (!file)
                    return;
                var reader = new FileReader();

                reader.addEventListener("load", function () {
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
        var promises = fields.map(add);

        return $.when.apply($, promises).then(function () {
            return data;
        });
    };

    $("#subToMc").click(function () {
        // Validate form
        if (!wrapper.smkValidate()) 
            return;

        var u = {
            mail: $(".app-jqfs-mailchimp-" + mid + " #SenderMail").val(),
            name: $(".app-jqfs-mailchimp-" + mid + " #SenderName").val(),
            surname: $(".app-jqfs-mailchimp-" + mid + " #SenderSurname").val()
        }

        // get data 
        // data = manuallyBuildData(wrapper); // alternative example with manual build, but we prefer automatic
        autoCollectData(wrapper).then(function (data) {
            //data.Recaptcha = recap;

            // submission
            //disableInputs(wrapper, true);
            //showOneAlert(wrapper, "msgSending"); // show "sending..."
            var ws = wrapper.data("webservice");   // should be "Form/ProcessForm" or a custom override
            sxc.webApi.post(ws, {}, data, true)
                .success(function () {
                    //showOneAlert(wrapper, "msgOk")
                    //$(btn).hide();
                    mc.subscribeUser(u.mail, u.name, u.surname);
                    // wrapper.find("." + c.clsForm).hide();
                })
                .error(function () {
                    //showOneAlert(wrapper, "msgError")
                    //disableInputs(wrapper, false);
                });
        });
    })
})