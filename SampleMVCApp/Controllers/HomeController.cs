using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleMVCApp.ViewModels.Home;
using SampleMVCApp.Models;

namespace SampleMVCApp.Controllers
{
    public class HomeController : Controller
    {
        private Log log;
        private IMessageManager manager;
 
        public HomeController(Log log, IMessageManager manager)
        {
            this.log = log;
            this.manager = manager;
            this.log.SetCategory("Home Controller");
        }


        //
        // GET: /Home/

        public ActionResult Index()
        {
            log.Write("Getting Message from Manager.");
            var message = manager.GetMessage();

            log.Write("Creating the View Model");
            var viewModel = new HomeViewModel
            {
                Message = message
            };

            log.Write("Rendering view");
            return View(viewModel);
        }


    }
}
