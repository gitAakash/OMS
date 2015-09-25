


//......................Product Details Form Start..........................
$("#UpdateProductForm").validate({
    rules: {
        'ProductDescription': { required: true },
      //  'SalesUnitPrice': { required: true },
        'Group': { required: true }
      //  'ProductSchedule.Value': { required: true },
      //  'ProductSchedule.Title': { required: true }
       // 'ProductSchedule.ColorCode': { required: true }
       // 'hdnScheduleColorcode':{required:true}
    },
    messages: {
        'ProductDescription': { required: "Product Description is required" },
       // 'SalesUnitPrice': { required: "Sales Unit Price is required" },
        'Group': { required: "Product Group is required" }
      //  'ProductSchedule.Value': { required: "Value is required" },
      //  'ProductSchedule.Title': { required: "Title is required" }
       // 'ProductSchedule.ColorCode': { required: "Color is required" }
       //  'hdnScheduleColorcode': { required: "Color is required" }
    },
    errorPlacement: function (error, element) {
    //  alert(element.attr("id"));
        error.appendTo("#" + element.attr("id") + "Validation");
    }
});


//......................Product Details Form End..........................

//......................Company Details Form Start..........................
$("#UpdateCompanyForm").validate({
    rules: {
        'XeroName': { required: true }
    },
    messages: {
        'XeroName': { required: "Company name is required" }
    },
    errorPlacement: function (error, element) {
        //  alert(element.attr("id"));
        error.appendTo("#" + element.attr("id") + "Validation");
    }
});


//......................Company Details Form End..........................

//......................Company Details Form Start..........................
$("#UpdateCompanyContactsForm").validate({
    rules: {
        'Name': { required: true },
        'Value': { required: true },
        'ContactType': { required: true }
    },
    messages: {
        'Name': { required: "Name is required" },
        'Value': { required: "Value is required" },
        'ContactType': { required: "Contact Type is required" }
    },
    errorPlacement: function (error, element) {
        //  alert(element.attr("id"));
        error.appendTo("#" + element.attr("id") + "Validation");
    }
});


//......................Company Details Form End..........................

//......................Create User Form.................................

//$("#UpdateUserForm").validate({
//    rules: {
//        'FirstName': { required: true },
//        'LastName': { required: true },
//        'EmailAddress': { required: true, email: true },
//        'Password': { required: true, minlength: 5 },
//        'ConfirmPassword': { required: true, minlength: 5, equalTo: "#Password" },
//        'UserType': { required: true },
//        'Group': {
//            required: function (element) {
//                return $('#UserType :selected').text() == "Staff";


//            },
//        },
//        'Calendar': {
//            required: function (element) {
//                return $('#UserType :selected').text() == "Staff" ||  $('#UserType :selected').text()=="Admin";
//            }
//        },
//        'CompanyId': {
//            required: function (element) {
//                return $('#UserType :selected').text() == "Client";


//            }
//        },
//        'AboutMe': {maxlength: 500 }
//    },
//    messages: {
//        'FirstName': { required: "First Name is required" },
//        'LastName': { required: "Last Name is required" },
//        'EmailAddress': { required: "Email address is required", email: "Email address is invalid" },
//        'Password': { required: "Password is required", minlength: "Password is not valid. Please enter password between 5 -16 digits." },
//        'ConfirmPassword': { required: "Confirm password is required", minlength: "Confirm password is not valid. Please enter password between 5 -16 digits.", equalTo: "Passwords do not match, please reenter" },
//        'UserType': { required: "User Type is required" },
//        'Calendar': { required: "Calendar is required" },
//        'CompanyId': { required: "Company is required" },
//        'Group': { required: "Select atleast one group" },
//        'AboutMe': {maxlength: "Please enter no more than 500 characters. " }
//    },
//    errorPlacement: function (error, element) {
//        //  alert(element.attr("id"));
//        error.appendTo("#" + element.attr("id") + "Validation");
//    }
//});


//......................End Create User Form.................................

$("#CreateProdScheduleForm").validate({
    rules: {
        'ProductGroupId': {
            required: function (element) {

                if ($('#CreateEvent').is(":checked")) {
                    return;
                }


            }
        },
        'Title': {
            required: function (element) {

                if ($('#CreateEvent').is(":checked")) {
                    return;
                }


            }
        },
        'Value': {
            required: function (element) { if ($('#CreateEvent').is(":checked")) { return;}


            }
        },
        'EmailAddress': {
            required: function (element) { if ($('#SendEmail').is(":checked")) { return; } }, email: true
        },

       

        'WebOptionMax': {
            required: function (element) {
              // debugger;
                var ProdGrpName = $("#globleProdGrpName").val();
                $("#WebOptionNumValidation").text('');
                if (ProdGrpName == 'Package') {
                    if ($('#WebOptionMax').val() != "") {
                        var value = $('#WebOptionMax').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
                        var intRegex = /^\d+$/;
                        if (!intRegex.test(value)) {
                            $("#WebOptionNumValidation").text('Please enter a valid qunatity');
                            return false;

                        }
                        else {
                            var qty = parseInt($('#WebOptionMax').val());
                            if (qty <= 0) {
                                $("#WebOptionNumValidation").text('Product qunatity should greater than 0');
                                return false;
                            }
                        }
                    }
                    else {
                        $("#WebOptionNumValidation").text('Product qunatity is required');
                        return false;
                    }
                }
                else {

                    if ($('#WebOptionMax').val() != "") {
                        var value = $('#WebOptionMax').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
                        var intRegex = /^\d+$/;
                        if (!intRegex.test(value)) {
                            $("#WebOptionNumValidation").text('Please enter a valid qunatity');
                            return false;
                        }
                    }
                    else {
                        $('#WebOptionMax').val(0);
                    }
                }
            }, 
        },

    },
    messages: {
        'ProductGroupId': { required: "Product Group is required" },
        'Title': { required: "Title is required" },
        'Value': { required: "Value is required" },
        'EmailAddress': { required: "Email is required", email: "Email address is invalid" },
        'WebOptionMax': { required: "Product qty is required", number: "Please enter a valid number" }
    },
    errorPlacement: function (error, element) {
        //  alert(element.attr("id"));
        error.appendTo("#" + element.attr("id") + "Validation");
    }
});

//......................

$("#AddOrderForm").validate({
  
    rules: {

        'OfficeContactName': { required: true },
        'Phone': { required: true },
        'EmailId': { required: true, email: true },
        'PropertyAddress': { required: true },
        'OfficeContactName': { required: true },


        'txtAgentName': { required: "#Agent:checked" }, //  check box validation
        'txtAgentPhone': { required: "#Agent:checked" }, //  check box validation
        'txtAgentEmail': { required: "#Agent:checked", email: true },

        'txt_OwnerName': { required: "#Owner:checked" }, //  check box validation
        'txt_OwnerPhone': { required: "#Owner:checked" }, //  check box validation

        'txt_TenantName': { required: "#Tenant:checked" }, //  check box validation
        'txt_TenantPhone': { required: "#Tenant:checked" }, //  check box validation

        'ddlCompany': { selectcheck: true }
       // 'chkAgent': {required: true}
       // 'txtAgentEmail': { required: "#Agent:checked", email: true }


    },
    messages: {
        'OfficeContactName': { required: "Office Contact Name is required" },
        'Phone': { required: "Phone is required" },
        'EmailId': { required: "Email is required", email: "Email address is invalid" },
        'PropertyAddress': { required: "Property address is required" },

        'txtAgentName': { required: "Agent Name is required" },
        'txtAgentPhone': { required: "Agent Phone is required" },
        'txtAgentEmail': { required: "Agent Email is required", email: "Email address is invalid" },

        'txt_OwnerName': { required: "Owner Name is required" },
        'txt_OwnerPhone': { required: "Owner Phone is required" },

        'txt_TenantName': { required: "Tenant Name is required" },
        'txt_TenantPhone': { required: "Tenant Phone is required" }
       
       // 'ddlCompany': { required: "Please select Company" }

      //  'txtAgentEmail': { required: "Agent Email is required", email: "Email address is invalid" }


    



    },

 



    errorPlacement: function (error, element) {
       //  alert(element.attr("id"));
        error.appendTo("#" + element.attr("id") + "Validation");
    }




});

var Agent = $("#Agent");
// show when newsletter is checked
Agent.click(function () {
    //alert("manoj");
  
});


jQuery.validator.addMethod('selectcheck', function (value) {
    if (value != '') {
        $("#ddlCompanyValidation").html("");
        return (value != '');
    }
    else {
        $("#ddlCompanyValidation").html("Company name is required");
    }
}, "");
