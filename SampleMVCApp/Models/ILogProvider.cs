using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleMVCApp.Models
{
    public interface ILogProvider
    {
        ILogWriter GetLogWriter(string title);
        IEnumerable<LogMessageGroup> GetMessageGroups();
        void Clear();    
    }

    public class LogMessage
    {
        public LogMessage(string category, string message, DateTime time)
        {
            this.Category = category;
            this.Message = message;
            this.Time = time;
        }

        public string Category { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }

    }

    public class LogMessageGroup
    {
        public string Title { get; private set; }
        public DateTime Time { get; private set; }
        public IEnumerable<LogMessage> Messages { get; private set; }

        public LogMessageGroup(string title, IEnumerable<LogMessage> messages, DateTime time)
        {
            Title = title;
            Messages = messages;
            Time = time;
        }
    }

}
