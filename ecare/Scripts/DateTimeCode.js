//$(function () {
//    $(function () {
//        $('#datetimepicker3').datetimepicker({
//            format: 'LT'
//        });
//});

    //$(function() {
    //    $("#datepicker").datepicker({
    //        changeMonth: true,
    //        changeYear: true,
    //        yearRange: '2020:2020',
    //        dateFormat: 'dd/mm/yy',
    //        minDate: 0,
    //        defaultDate: null
    //    }).on('changeDate', function (ev) {
    //        if ($('#datepicker').valid()) {
    //            $('#datepicker').removeClass('invalid').addClass('success');
    //        }
    //    });
   
    //   });

$(function () {
    $("#select_date").datepicker();
    $("#select_date").on('change', function () {

        var date = Date.parse($(this).val());

        if (date > Date.now()) {
            alert('Please select another date');
            $(this).val('');
        }

    });
});

