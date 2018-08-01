using System.Web.Mvc;

namespace Chaso.Reporting.Sample.Controllers
{
    public class HomeController : Controller
    {
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
            var reportFactory = new Models.ReportFactory(reportPath, reportName, Request.Url.OriginalString, Request.QueryString);
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
        public ActionResult About()
        {
            ViewBag.Message = "Chaso Reporting .Net";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Chaso Reporting Sample";

            return View();
        }
    }
}