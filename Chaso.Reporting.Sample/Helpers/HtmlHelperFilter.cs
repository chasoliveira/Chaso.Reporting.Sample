using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Chaso.Reporting.Sample.Helpers
{
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
}