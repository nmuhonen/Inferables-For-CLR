using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleMVCApp.Models
{
    public class SingletonLogProvider: ILogProvider
    {
        private object syncLock = new object();
        private List<LogWriter> writers = new List<LogWriter>();

        public IEnumerable<LogMessageGroup> GetMessageGroups()
        {
            lock (syncLock)
            {
                return writers
                    .Select
                    (
                        item => new LogMessageGroup(item.Title, item.GetMessages(), item.Time)
                    )
                    .ToArray()
                    .Reverse();
            }
        }

        public void Clear()
        {
            lock (syncLock)
            {
                writers.Clear();
            }
        }

        public ILogWriter GetLogWriter(string title)
        {
            lock (syncLock)
            {
                var logWriter = new LogWriter
                {
                    Time = DateTime.Now,
                    Title = title
                };

                writers.Add(logWriter);
                return logWriter;
            }
        }


        private class LogWriter: ILogWriter
        {
            public string Title { get; set; }
            public DateTime Time { get; set; }

            private object syncLock = new object();
            private List<LogMessage> messages = new List<LogMessage>();

            public void Write(string category, string message)
            {
                lock (syncLock)
                {
                    messages.Add(
                        new LogMessage(category, message, DateTime.Now));
                }
            }

            public IEnumerable<LogMessage> GetMessages()
            {
                lock (syncLock)
                {
                    return messages.ToArray().Reverse();
                }
            }
        }
    }
}