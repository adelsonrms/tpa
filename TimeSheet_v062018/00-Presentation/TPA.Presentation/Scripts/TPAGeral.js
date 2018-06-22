$(function () {
    $("input[type=text].tpa_datepicker_invariant")
    .datepicker({
        language: "pt-BR",
        todayBtn: "linked",
        format: "yyyy-mm-dd",
        autoclose: true,
        todayHighlight: true
    })
    .mask("9999-99-99");
});