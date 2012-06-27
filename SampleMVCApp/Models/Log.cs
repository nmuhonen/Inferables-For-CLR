using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleMVCApp.Models
{
    public class Log
    {
        ILogWriter logWriter;
        string category;

        public Log(ILogWriter logWriter)
        {
            this.logWriter = logWriter;
            this.category = "Default";
        }

        public void SetCategory(string category)
        {
            this.category = category;
        }

        public void Write(string message)
        {
            this.logWriter.Write(category, message);
        }
    }
}