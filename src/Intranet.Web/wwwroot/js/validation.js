//function setError(formGroup) {
//    if (!formGroup.hasClass('has-error')) {
//        formGroup.addClass('has-error has-feedback').find('.form-control:first').after('<span class="form-control-feedback" aria-hidden="true"><i class="fa fa-exclamation-circle" aria-hidden="true"></i></span>');
//    }
//}

$(function () {
    $('.validation-summary-errors').each(function () {
        $(this).addClass('alert alert-danger alert-dismissible');
        //$(this).prepend('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
    });

    $('form').each(function () {
        $(this).find('div.form-group').each(function () {
            if ($(this).find('span.field-validation-error').length > 0) {
                //setError($(this));
                $(this).addClass('has-error');
            }
        });
    });
});

$.validator.setDefaults({
    highlight: function (element) {
        //setError($(element).closest('.form-group'));
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error');//.removeClass('has-error has-feedback').find('span.form-control-feedback:first').remove();
    },
    showErrors: function (errorMap, errorList) {
        this.defaultShowErrors();
        $('.validation-summary-errors').removeClass('animated shake').addClass('animated shake');
    },
    errorElement: 'span',
    errorClass: 'help-block',
    errorPlacement: function (error, element) {
        if (element.parent('.input-group').length) {
            error.insertAfter(element.parent());
        } else {
            error.insertAfter(element);
        }
    },
    ignore: []
});