using Microsoft.Reporting.WebForms;

namespace HRMS_Web.Controllers
{
    internal class ViewBag
    {
        public static implicit operator ViewBag(ReportViewer v)
        {
            throw new NotImplementedException();
        }
    }
}