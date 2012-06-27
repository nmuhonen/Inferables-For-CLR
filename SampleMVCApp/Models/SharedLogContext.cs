using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleMVCApp.Models
{
    public static class SharedLogContext
    {
        public static ILogWriter CreateSharedLogWriter(ILogProvider provider)
        {
            return provider.GetLogWriter("Request " + HttpContext.Current.Request.Url.AbsolutePath);
        }

    }
}