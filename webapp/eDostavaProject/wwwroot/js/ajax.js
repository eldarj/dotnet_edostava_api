﻿function DodajAjaxEvente() {
    $("button[ajax-poziv='da']").click(function (event) {
        $(this).attr("ajax-poziv", "dodan");

        event.preventDefault();
        var urlZaPoziv = $(this).attr("ajax-url");
        var divZaRezultat = $(this).attr("ajax-rezultat");

        $.get(urlZaPoziv, function (data, status) {
            $("#" + divZaRezultat).html(data);

        });
    });


    $(document).ready(function () {
        $("#bt").click(function (event) {
            ("#val").html("Opcija dopustena samo moderatoru");

        });
    });

    $("a[ajax-poziv='ajaxDa']").off().click(function (event) {
        $(this).attr("ajax-poziv", "dodan");
        event.preventDefault();
        var urlZaPoziv1 = $(this).attr("ajax-url");
        var urlZaPoziv2 = $(this).attr("href");
        var divZaRezultat = $(this).attr("ajax-rezultat");

        var urlZaPoziv;

        if (urlZaPoziv1 instanceof String)
            urlZaPoziv = urlZaPoziv1;
        else
            urlZaPoziv = urlZaPoziv2;

        // Takođe provjeri linkove sa alertify.js popup-om
        if (typeof $(this).attr('data-alertify') !== 'undefined') {
            console.log("YES!");
            let self = $(this);
            let msg = '';
            if (self.attr('data-alertify-text')) {
                msg += `<p> ${self.attr('data-alertify-text')} </p>`;
            }
            if (self.attr('data-alertify-btn-pre-text')) {
                msg += `<span> ${self.attr('data-alertify-btn-pre-text')} </span>`;
            }
            if (self.attr('data-alertify-btn-text')) {
                msg += `<a class="btn btn-sm btn-danger center" href="#">${self.attr('data-alertify-btn-text')}</a>`;
            }
            alertify.log(msg);
        }

        $.ajax({
            type: "GET",
            url: urlZaPoziv,
            async: true,
            success: function (data) {
                $("#" + divZaRezultat).html(data);
            }
        });


    });

    $("a[ajax-poziv='da']").click(function (event) {
        $(this).attr("ajax-poziv", "dodan");
        event.preventDefault();
        var urlZaPoziv1 = $(this).attr("ajax-url");
        var urlZaPoziv2 = $(this).attr("href");
        var divZaRezultat = $(this).attr("ajax-rezultat");

        var urlZaPoziv;

        if (urlZaPoziv1 instanceof String)
            urlZaPoziv = urlZaPoziv1;
        else
            urlZaPoziv = urlZaPoziv2;

        $.get(urlZaPoziv, function (data, status) {
            $("#" + divZaRezultat).html(data);
        });
    });

    $("form[ajax-poziv='da']").submit(function (event) {
        $(this).attr("ajax-poziv", "dodan");
        event.preventDefault();
        var urlZaPoziv1 = $(this).attr("ajax-url");
        var urlZaPoziv2 = $(this).attr("action");
        var divZaRezultat = $(this).attr("ajax-rezultat");

        var urlZaPoziv;
        if (urlZaPoziv1 instanceof String)
            urlZaPoziv = urlZaPoziv1;
        else
            urlZaPoziv = urlZaPoziv2;

        var form = $(this);

        $.ajax({
            type: "POST",
            url: urlZaPoziv,
            data: form.serialize(),
            success: function (data) {
                $("#" + divZaRezultat).html(data);
            }
        });
    });
}
$(document).ready(function () {
    // izvršava nakon što glavni html dokument bude generisan
    DodajAjaxEvente();
});

$(document).ajaxComplete(function () {
    // izvršava nakon bilo kojeg ajax poziva
    DodajAjaxEvente();
});
