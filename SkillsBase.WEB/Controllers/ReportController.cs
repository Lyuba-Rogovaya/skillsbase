using Microsoft.AspNet.Identity;
using SkillsBase.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SkillsBase.WEB.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService reportService;
        public ReportsController(IReportService reportService)
        {
            if (reportService == null)
                throw new ArgumentNullException(nameof(reportService));

            this.reportService = reportService;
        }

        public ActionResult Statistics()
        {
            return View("UserStatistics");
        }

        [HttpPost]
        public async Task<ActionResult> GetUserTotals(string userId)
        {
            if(String.IsNullOrEmpty(userId))
            {
                userId = User.Identity.GetUserId();
            }
            try
            {
                var result = await reportService.GetUserStatistics(userId);
                return Json(result);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                reportService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}