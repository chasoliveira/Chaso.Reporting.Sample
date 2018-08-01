using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WebForms = Microsoft.Reporting.WebForms;

namespace Chaso.Reporting.Sample.Models
{
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
}