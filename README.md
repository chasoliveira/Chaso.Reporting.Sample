Chaso.Reporting.Sample

This articles describe how to use the library Chaso.Reporting. 
For this sample area used the following structure:

Visual Studio Community 2017
ASP.Net MVC
ReportViwerForMvc14
Chaso.Reporting

if you doens't have an already project, starts creane new one. Open Visual Studio.
1) File -> New -> Project.
2) Visual C# -> Web -> ASP.NET Web Application (.Net Framework)
3) Give a name (My Chaso.Reporting)
4) Select the template MVC, without authentication.

When the process of creation new project finish, you will need add some thirdth librarires.
Click with right button in project name on Project Solution, select Manage NuGet Packages option, see image 01.

1) Search for Chaso.ReportViewerForMvc and install
	Install-Package Chaso.ReportViewerForMvc -Version 1.1.1.1
2) Search for Chaso.Reporting and install

Change file ReportViwerWebForm.aspx 
	from 
		<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
	to 
		<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

Add some style and meta tag:
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style>
        html, body, form, #div1 {
            height: 100%;
        }
    </style>
	
Add script referece after asp:ScriptManager tag.
	<script src="/Scripts/chaso-reporting/render-report-viewer.js" type="text/javascript"></script>

Change Web.Config file. Remove any referece for Microsoft.ReportViewer.WebForms in 11 version.
See in this repository how this file look like.

Creating script for replace default ScriptManager of ReportViwerForMvc.
On Script folder crete new folder as chaso-reporting, inside of this create new js file as render-report-viewer.js with this content.

var ReportViewerForMvc = (new function () {

    var _iframeId = null;
    var _iframe = null;

    var resizeIframe = function (msg) {
        var height = msg.source.document.body.scrollHeight;
        //var width = msg.source.document.body.scrollWidth;

        var iframe = ReportViewerForMvc.getIframe();
        $(iframe).removeAttr("style")
        $(ReportViewerForMvc.getIframeId()).height(height);
        $(ReportViewerForMvc.getIframeId()).width("100%");
    }

    var addEvent = function (element, eventName, eventHandler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, eventHandler);
        } else if (element.attachEvent) {
            element.attachEvent('on' + eventName, eventHandler);
        }
    }

    this.setIframeId = function (value) {

    };

    this.getIframeId = function () {
        return _iframeId || (_iframeId = "#" + $("iframe").attr("id"));
    };
    this.getIframe = function () {
        return _iframe || (_iframe = $("iframe"));
    };
    this.setAutoSize = function () {
        addEvent(window, 'message', resizeIframe);
    }

}());

ReportViewerForMvc.setAutoSize();

Also you need other script to render assyncronously the report, create a js file called as 'report-load.js' with the following content;

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

    $("button[name=btnExecutar]").on("click", function (e) {
        var params = $("form[name=formRenderReport]").serialize();
        $("#divReport").html("");
        apiReport.render(params, "divReport", function () {
            $("iframe").width("100%");
            $("iframe").height("400px")
            $("#bodyFilters").collapse('hide');
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



in this project the rdls file will be placed in App_Rdl folder, so let´s create this directory on project root and reader in list al files in this folder.
To help us on listing all rdl's files, we needs to create two classes 'ReportFile' and 'ReportDirectory'.

 public class ReportDirectory
    {
        public ReportDirectory(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);

            this.Name = di.Name;
            this.FullName = di.FullName;
            this.ReportFiles = new List<ReportFile>();
            this.SubDirectories = new List<ReportDirectory>();
            ProcessDirectory();
        }
        public string Name { get; set; }
        public string FullName { get; }
        public IList<ReportFile> ReportFiles { get; set; }
        public IList<ReportDirectory> SubDirectories { get; set; }


        private void ProcessDirectory()
        {
            var files = new List<KeyValuePair<string, string>>();
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(FullName);
            foreach (string fileName in fileEntries)
            {
                var re = new FileInfo(fileName);
                ReportFiles.Add(new ReportFile(re));
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(FullName);
            foreach (string subdirectory in subdirectoryEntries)
            {
                var subReportDirectory = new ReportDirectory(subdirectory);
                SubDirectories.Add(subReportDirectory);
            }
        }
    }
	
    public class ReportFile
    {
        public ReportFile(FileInfo fi)
        {
            FileName = fi.Name;
            FullName = fi.FullName;
            CreationTime = fi.CreationTime;
            LastWriteTime = fi.LastWriteTime;
            Length = fi.Length;
        }

        public string FileName { get; private set; }
        public string FullName { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public long Length { get; private set; }
    }

now we need to create some actions in home controller to show a list of rdl's files, to render a ReportViewer when an rdl is selected, and in case the file has some parameters, one action to show than.
First let´s create an class to fabrick an instance of Chaso.Reporting.Engine, this will load rdl, fill with parameters and return the ReportViewer ready to show on component ReportViewerForMvc.

public class ReportFactory
    {
        Chaso.Reporting.IEngine _engine;
        public IList<ErrorMessage> Erros { get; private set; }
        public ReportFactory(string reportPath, string reportName, string originalUrl, NameValueCollection queryString)
        {
            NameValueCollection ReportParameters = GetQueryString(queryString);
            this._engine = new Chaso.Reporting.Engine(originalUrl, reportPath, reportName, ReportParameters);
            this._engine.OnError += (s, o) => Erros = o.ErrorMessages;
        }


        private NameValueCollection GetQueryString(NameValueCollection queryString)
        {
            NameValueCollection ReportParameters = new NameValueCollection();
            var excludes = new[] { "reportpath", "reportname", "_" };

            foreach (string key in queryString.AllKeys.Where(k => !excludes.Contains(k.ToLower())))
            {
                ReportParameters.Add(key, queryString[key]);
            }

            return ReportParameters;
        }

        public string GetParameterDataSourceAsJson(string name, string dataSetName)
        {
            return this._engine.GetDataForParameterAsJson(name, dataSetName);
        }

        public List<RDL.ReportParameter> GetParameters()
        {
            return this._engine.Parameters();
        }

        public WebForms.ReportViewer ViewerInstance()
        {
            return this._engine.ReportViewer();
        }
    }
	
	
Now Open HomeController and create the following actions:

        private string ReportPath() { return Server.MapPath("~/App_Rdl"); }
        public ActionResult Index()
        {
            var reportPath = ReportPath();
            var reportDirectory = new Models.ReportDirectory(reportPath);

            return View(reportDirectory);
        }

        public ActionResult Load(string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return RedirectToAction(nameof(Index));

            ViewBag.ReportName = reportName;
            var reportPath = ReportPath();
            var reportFactory = new Models.ReportFactory(reportPath, reportName, Request.Url.OriginalString, Request.QueryString);
            var reportParameter = reportFactory.GetParameters();
            ViewBag.Erros = reportFactory.Erros;

            return View(reportParameter);
        }
        public JsonResult LoadParameter(string reportName, string name, string dataSetName)
        {
            var reportPath = ReportPath();
            var reportFactory = new Models.ReportFactory(reportPath,reportName, Request.Url.OriginalString, Request.QueryString);
            string reportViewer = reportFactory.GetParameterDataSourceAsJson(name, dataSetName);

            return Json(reportViewer, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Render(string reportName)
        {
            var reportPath = ReportPath();
            var reportFactory = new Models.ReportFactory(reportPath, reportName, Request.Url.OriginalString, Request.QueryString);
            var reportViewer = reportFactory.ViewerInstance();
            ViewBag.Erros = reportFactory.Erros;

            return View(reportViewer);
        }
		
In View's Home folder create respectives cshtml file for each action previously created.

Index.cshtml
@model Chaso.Reporting.Web.Models.ReportDirectory
@{
    ViewBag.Title = "Home Page";
}

<div class="jumbotron">
    <h1>Chaso.Reporting for ReportViewer</h1>
    <p class="lead">Chaso.Reportin is a full-featured reporting solution for Windows Forms, ASP.NET, MVC. It can be used in Microsoft Visual Studio 2005-2017. Supports .Net Framework 2.0-4.x.</p>
</div>
@helper RenderDirectory(Chaso.Reporting.Web.Models.ReportDirectory reportDirectory)
{
    <ul class="list-group">
        <li class="list-group-item">
            @reportDirectory.Name
            <ul class="list-group">
                @foreach (var item in reportDirectory.ReportFiles)
                {
                    <li class="list-group-item  list-group-item-info">
                        <a href="/Home/Load/?reportName=@item.FileName" class="btn-link">@item.FileName <span class="pull-right">@item.LastWriteTime <span>@item.Length (bytes)</span></span></a>
                    </li>
                }
            </ul>
            @foreach (var item in reportDirectory.SubDirectories)
            {
                @RenderDirectory(item)
            }
        </li>
    </ul>
}
<div class="row">
    @RenderDirectory(Model)
</div>


Load.cshtml

@using Chaso.Reporting.Web.Helpers
@model IEnumerable<Chaso.Reporting.RDL.ReportParameter>
@{
    ViewBag.Title = "Relatório " + ViewBag.ReportName;
    IList<Chaso.Reporting.ErrorMessage> errorMessages = ViewBag.Erros as IList<Chaso.Reporting.ErrorMessage>;

}

<h2>@ViewBag.ReportName</h2>
<div class="row">
    <div class="col-md-12">
        @if (errorMessages != null && errorMessages.Any())
        {
            foreach (var erro in errorMessages)
            {
                <div id="alertaPermissao" class="alert alert-error" style="opacity: 1; display: block;">
                    <div class="row-fluid">
                        <a class="close" data-dismiss="alert" href="#">×</a>
                        <strong>@erro.Message</strong>
                        <p>Informe os dados abaixo para o suporte. <a href=""></a> </p>
                        <p>@erro.StackTrace</p>
                    </div>
                </div>
            }
        }
    </div>
    <div class="col-md-12">
        <div class="panel panel-default">
            <div class="panel-heading" role="button" data-toggle="collapse" data-target="#bodyFilters" aria-expanded="false" aria-controls="collapseFilters">Filtros</div>
            <div id="bodyFilters" class="panel-body collapse in" aria-controls="collapseFilters">
                <form name="formRenderReport">
                    <div class="row">
                        <div class="col-md-12">
                            <input type="hidden" name="reportname" value="@ViewBag.ReportName" />
                            @foreach (var param in Model)
                            {
                                @Html.ReportFilter(param)
                            }
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="col-md-12 form-group">
        <button type="button" class="btn btn-primary" title="Executar o Relatório" name="btnExecutar" data-loading-text="<i class='fa fa-spinner fa-spin '></i> Carregando o Relatório..."><span class="fa fa-print"></span>Executar</button>
    </div>
    <div class="col-md-12" id="divReport">
    </div>
</div>
@section Scripts{
    <script src="~/Scripts/chaso-reporting/render-report-viewer.js" type="text/javascript"></script>
    <script src="~/Scripts/chaso-reporting/report-load.js" type="text/javascript"></script>
}


Render.cshtml

@using ReportViewerForMvc
@using System.Web.UI.WebControls
@model  Microsoft.Reporting.WebForms.ReportViewer
@{
    Layout = null;
    IList<Chaso.Reporting.ErrorMessage> errorMessages = ViewBag.Erros as IList<Chaso.Reporting.ErrorMessage>;
}
<div class="row-fluid">
    @{
        try
        {
            if (errorMessages != null && errorMessages.Any())
            {
                foreach (var erro in errorMessages)
                {
                    <div class="alert alert-danger" role="alert">
                        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <strong>@erro.Message</strong>
                        <p>Informe os dados abaixo para o suporte. <a href=""></a> </p>
                        <p>@erro.StackTrace</p>
                    </div>
                }
            }
            else
            {
                @Html.ReportViewer(Model, new { scrolling = "yes" })
            }
        }
        catch (Exception exception)
        {
            <div class="alert alert-danger" role="alert">
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <strong>@exception.Message</strong>
                    <p>Informe os dados abaixo para o suporte. <a href=""></a> </p>
                    <p>@exception.InnerException</p>
                    <p>@exception.Source</p>
            </div>
        }
    }
</div>
@section Scripts{
    <script type="text/javascript" src="~/Scripts/chaso-report-engine/render-report-viewer.js"> </script>
}

To help us with report parameters let's create an HtmlHelper, in root  directory create an folder called Helpers, inside of it create the following class.

    public static class HtmlHelperFilter
    {
        public static MvcHtmlString ReportFilter(this HtmlHelper html, Chaso.Reporting.RDL.ReportParameter param)
        {
            switch (param.Type)
            {
                case Chaso.Reporting.RDL.ParameterDataType.Float:
                    return new MvcHtmlString(Input("text", param));
                case Chaso.Reporting.RDL.ParameterDataType.DateTime:
                    return new MvcHtmlString(Input("date", param));
                case Chaso.Reporting.RDL.ParameterDataType.Boolean:
                    return new MvcHtmlString(Input("checkbox", param));
                case Chaso.Reporting.RDL.ParameterDataType.Integer:
                    return new MvcHtmlString(Input("number", param));
                case Chaso.Reporting.RDL.ParameterDataType.String:
                default:
                    return new MvcHtmlString(Input("text", param));
            }
        }

        private static string Input(string tipoInput, Chaso.Reporting.RDL.ReportParameter param)
        {
            if (param.ValidValues.Any())
                return Select(param);

            var html = new StringBuilder();
            html.Append($"<div class='col-md-3'>");
            html.Append($"  <label for='{param.Name}'>{param.Prompt}</label>");
            html.Append($"  <input type='{tipoInput}' name='{param.Name}'  class='form-control' />");
            html.Append("</div>");

            return html.ToString();
        }

        private static string Select(Chaso.Reporting.RDL.ReportParameter param)
        {
            var html = new StringBuilder();

            var dataset = param.ValidValues.First();
            var parameters = dataset.DataSet.Query.QueryParameters;
            var filters = "";
            if (parameters.Any())
            {
                var item = parameters.Select(p => new { name = p.Name.Replace("@", "") });
                var join = string.Join(",", item.Select(s => s.name));
                filters += $"data-filter='{join}'";
            }
            html.Append($"<div class='col-md-3'>");
            html.Append($"  <label for='{param.Name}'>{param.Prompt}</label>");
            html.Append($"  <select name='{param.Name}' data-datasetname='{dataset.DataSetName}' data-value='{dataset.ValueField}' data-text='{dataset.LabelField}' data-nullable='{param.Nullable}' {filters} class='form-control' >");
            html.Append($"  <option value=\"{null}\">...Selecione...</option>");
            html.Append($"  </select>");
            html.Append("</div>");

            return html.ToString();
        }

    }
	
Let's create an database, for this sample we can create a compact sql database. or uou can use your owner database.
Clique with right button on App_Data on root project, select option Visual C#-> Data -> SQL Server Database. named as ChasoDB.mdf.

After process open with your prefered SQL Manager and create the following tables.

CREATE TABLE [dbo].[Category]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(60) NOT NULL,
)

CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[CategoryId] INT NOT NULL,
	[Name] NVARCHAR(60) NOT NULL,
	[Value] decimal(18,4) NOT NULL,
	CONSTRAINT [FK_Category_Project] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
)

Insert the following datas:

INSERT INTO Category VALUES('Category One');
INSERT INTO Category VALUES('Category Two');
INSERT INTO Category VALUES('Category Three');

INSERT INTO Product VALUES(1, 'Product One', 3.4);
INSERT INTO Product VALUES(1, 'Product Two', 4.2);
INSERT INTO Product VALUES(2, 'Product Three', 5.3);
INSERT INTO Product VALUES(2, 'Product Four', 6.4);
INSERT INTO Product VALUES(3, 'Product Five', 7.5);
INSERT INTO Product VALUES(3, 'Product Six', 8.6);

The next step is create ower first rdl file.
First download the install file of Microsoft ReportBuilder https://www.microsoft.com/en-us/download/details.aspx?id=53613.
After install open the exectable and create an empty report.
You can find more information abou create and design an report on this site https://docs.microsoft.com/pt-br/sql/reporting-services/report-builder-tutorials?view=sql-server-2017


