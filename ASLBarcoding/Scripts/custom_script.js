//var $j = jQuery.noConflict();

function clearForm() {
    $(':input').not(':button, :submit, :reset, :hidden, :checkbox, :radio').val('');
    $(':checkbox, :radio').prop('checked', false);
}

$(document).ready(function () {

     $(".datecontrol").datepicker({
        dateFormat: 'dd/mm/yyyy'
     });

    $.ajaxSetup({ cache: false });  //Turn off caching
});
//var jQuery = jQuery.noConflict();