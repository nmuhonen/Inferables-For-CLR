using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleMVCApp.ViewModels.Log;
using SampleMVCApp.Models;

namespace SampleMVCApp.Controllers
{
    public class LogController : Controller
    {
        private ILogProvider logProvider;

        public LogController(ILogProvider logProvider)
        {
            this.logProvider = logProvider;
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ShowLog()
        {
            var viemModel = new ShowLogViewModel
            {
                MessageGroups = logProvider.GetMessageGroups()
            };

            return View(viemModel);
        }

    }
}
