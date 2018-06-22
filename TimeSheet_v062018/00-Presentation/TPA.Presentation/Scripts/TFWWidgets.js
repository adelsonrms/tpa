var ErrorMessage = function (msg) {
    if(bootbox)
    {
        try{
            bootbox.alert(msg);
        }
        catch(e)
        {
            alert(msg);
            document.writeln(msg);
        }
    }
    else
    {
        alert(msg);
        document.writeln(msg);
    }
}

var InicializaTFWWidgets = function () {

    $(".disabled").keydown(function (e) {
        return false;
    }).focus(function () {
        $(this).blur();
    }).attr('readonly', true);

    if (jQuery().mask)
    {

        $("input[type=text].tfw_common_date")
            .mask("99/99/9999");
    }
    else
    {
        ErrorMessage("Biblioteca mask não carregada");
    }

    if ((jQuery().datepicker) && (jQuery().mask)) {

        $("input[type=text].tfw_datepicker")
            .datepicker({
                language: "pt-BR",
                todayBtn: "linked",
                format: "dd/mm/yyyy",
                autoclose: true,
                todayHighlight: true
            })
            .mask("99/99/9999");


        $(".input-group.date.tfw_datepicker")
            .datepicker({
                language: "pt-BR",
                todayBtn: "linked",
                format: "dd/mm/yyyy",
                autoclose: true,
                todayHighlight: true
            });

        $(".input-group.date.tfw_datepicker input[type=text]").mask("99/99/9999");
    }
    else
    {
        ErrorMessage("Biblioteca mask  e/ou datepicker não carregada(s)");
    }

    if ((jQuery().datetimepicker) && (jQuery().mask)) {

        try {
            $('div.input-group.tfw_datetimepicker input[type=text]').mask("99/99/9999 99:99");
            $('.input-group.tfw_datetimepicker').datetimepicker({ locale: 'pt-BR', showTodayButton: true, showClear: true, format: 'L LT' });
            $('input[type=text].tfw_datetimepicker').datetimepicker({ locale: 'pt-BR', showTodayButton: true, showClear: true, format: 'L LT' }).mask("99/99/9999 99:99");
            $("input[type=text].tfw_common_datetime").mask("99/99/9999 99:99");
        }
        catch (err) {

            alert(err.message);
        }
    }
    else {
        ErrorMessage("Biblioteca mask  e/ou datetimepicker não carregada(s)");
    }

    if (jQuery().mask) {

        $("input.tfw_time,  input.tfw_timepicker")
            .mask("99:99");

        $("input.tfw_bigtime,  input.tfw_bigtimepicker")
            .mask("999:99");
    }
    else {
        ErrorMessage("Biblioteca mask não carregada");
    }


    $(".tfw_integer, .tfw_time").keydown(function (e) {


        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 109, 189]) !== -1 ||
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });


    $(".tfw_decimal").keydown(function (e) {

        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 109, 110, 188, 189]) !== -1 ||
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    (e.keyCode >= 35 && e.keyCode <= 40)) {

            if ((e.keyCode == 110 || e.keyCode == 188) && ($(this).val().indexOf(',') > -1)) {
                e.preventDefault();
            }
            else {

                return;

            }
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });


    if (jQuery().maskMoney)
    {
        $(".tfw_money").maskMoney({ allowNegative: true, thousands: '.', decimal: ',', affixesStay: false });
    }
    else
    {
        ErrorMessage("Biblioteca maskMoney não carregada");
    }

    if (jQuery().select2) {
        
        function format(item) {
            
            try {

                var originalText = item.text;
                var originalClass = $(item.element).attr("class")||"";
                var tit = $(item).attr("title");
                if ((tit != null) && (tit != "")) {
                    return $( "<span class='" + originalClass + "'>" + originalText + "</span>");
                }
                else {
                    return $("<span class='" + originalClass + "'>" + originalText + "</span>");
                }
            }
            catch (err) {
                return $("<span>" + originalText + "</span>");
            }
        }

        $('.tfw_select2').each(function (i, obj)
        {
            //verificar se o select é um select2
            //if (!$(obj).hasClass("select2-hidden-accessible"))
            if (!($(obj).attr("data-isselect2")))
            {
                $(obj).attr("data-isselect2", true);
                var t = $(obj).select2({
                    dropdownAutoWidth: false,
                    width: '100%', theme: "classic", templateResult: format, placeholder: "Selecione um item ... "
                });
                
            }
        });
    }
    else
    {
        ErrorMessage("Biblioteca select2 não carregada");
    }

};






$(function () {

    try{
        if (InicializaTFWWidgets)
        {
            InicializaTFWWidgets();
        }
        else
        {
            ErrorMessage("Função InicializaTFWWidgets não definida");
        }
    }
    catch(e)
    {
        ErrorMessage("Erro ao carregar widgets :" + e.message);
    }

});