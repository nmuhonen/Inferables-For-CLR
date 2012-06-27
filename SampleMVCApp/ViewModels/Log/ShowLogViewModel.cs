using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleMVCApp.Models;

namespace SampleMVCApp.ViewModels.Log
{
    public class ShowLogViewModel
    {
        public IEnumerable<LogMessageGroup> MessageGroups;
    }

}