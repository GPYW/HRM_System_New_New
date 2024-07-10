using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.CodeDom;

namespace HRMS_Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            ReportEmployee();
            return View();
        }

        MyDataSet ds=new MyDataSet();

        public IActionResult ReportEmployee()
        {
            ReportViewer reportViewer = new ReportViewer();

            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.SizeToReportContent = true;

            reportViewer.Width = Unit.Percentage(15000);

            reportViewer.Height = Unit.Percentage(15000);

            var connectionString = "Data Source=.;Initial Catalog=DbEmployee, Integrated Security= True";

            SqlConnection conx = new SqlConnection(connectionString);

            SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM LeaveRequests", conx);

            adp.Fill(ds, ds.LeaveRequests.TableName);

            reportViewer.LocalReport.ReportPath = Server.MapPath(@"/MyReport.rdic");

            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MyDataSet", ds.Tables[0]));

            ViewBag ReportViewer = reportViewer;

            return View();
        }
    }
}
