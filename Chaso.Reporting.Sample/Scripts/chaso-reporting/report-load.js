(function ($) {
    "use strict";

    var ApiReport = ApiReport || (function () {
        var path = "/Home/Render";

        this.render = function (params, widget, done) {
            var finalurl = path + "/?" + params;
            $.ajax({
                url: finalurl,
                type: "GET",
                cache: false,
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                    $("#" + widget).html(XMLHttpRequest.responseText);
                },
                success: function (html) {
                    $("#" + widget).html(html);
                }
            }).done(function () {
                if (typeof done == "function")
                    done();
            });
        };

        this.getParameterDataSource = function (parameter, callback) {
            $.ajax({
                url: "/Home/LoadParameter/?" + parameter,
                type: "GET",
                cache: false,
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                },
                success: function (data) {
                    if (typeof callback == "function")
                        callback(JSON.parse(data));
                }
            }).done(function () {
            });
        };
    });

    var apiReport = new ApiReport();

    $("button[name=btnExecute]").on("click", function (e) {
        var _this = this;
        var params = $("form[name=formRenderReport]").serialize();
        $("#divReport").html("");
        $(_this).button("loading");
        apiReport.render(params, "divReport", function () {
            $("iframe").width("100%");
            $("iframe").height("400px")
            $("#bodyFilters").collapse('hide');
            $(_this).button("reset");
        });
        e.preventDefault();
    });
    $(document).ready(function () {
        $("form[name=formRenderReport] select:not([data-filter])").each(function () {
            changeSelecDependent(this, null);
        });
        $("form[name=formRenderReport] select[data-filter]").each(function () {
            var _this = this;
            var reference = $(_this).data("filter");
            $('select[name=' + reference + ']').on("change", function () {
                changeSelecDependent(_this, $(this).val());
            });
        });
        function changeSelecDependent(select, referenceValue) {
            var _this = select;

            var name = _this.name;
            var report = $("[name=reportname]").val();

            var datasetname = $(_this).data("datasetname");

            var form = $("<form>");
            form.append($("<input>", { type: "text", name: "reportName", value: report }));
            form.append($("<input>", { type: "text", name: "name", value: name }));
            form.append($("<input>", { type: "text", name: "datasetname", value: datasetname }));
            var reference = $(_this).data("filter");
            if (reference !== undefined)
                form.append($("<input>", { type: "text", name: reference, value: referenceValue }));
            var params = $(form).serialize();

            apiReport.getParameterDataSource(params, function (data) {
                $(_this).html("");
                $(_this).append($("<option>", { html: "...Selecione...", value: "" }));
                $.each(data, function (i, item) {
                    $(_this).append($("<option>", { value: item[$(_this).data("value")], html: item[$(_this).data("text")] }));
                });
            });
        }
    });
})(jQuery);