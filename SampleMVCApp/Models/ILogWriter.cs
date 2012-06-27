using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleMVCApp.Models
{
    public interface ILogWriter
    {
        void Write(string category, string message);
        IEnumerable<LogMessage> GetMessages();
    }
}
