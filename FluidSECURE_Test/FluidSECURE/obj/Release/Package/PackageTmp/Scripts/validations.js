function onlyNumbers(event) {
    var charCode = (event.which) ? event.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

//$(document).ready(function () {
//    $('.checkDecimal').change(function () {

//        var num = parseFloat($(this).val());
//        if (!isNaN(num)) {
//            var cleanNum = num.toFixed(2);
//            $(this).val(cleanNum);
//        }
//    });
//});

function onlyDecimal() {
    var num = parseFloat($(".checkDecimal").val());
    if (!isNaN(num)) {
        var cleanNum = num.toFixed(2);
        $(".checkDecimal").val(cleanNum);
    }
}