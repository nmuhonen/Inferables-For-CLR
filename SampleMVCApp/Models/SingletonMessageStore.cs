using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleMVCApp.Models
{
    public class SingletonMessageStore: IMessageStore
    {            
        private Log log;

        public SingletonMessageStore(ILogProvider provider)
        {
            this.log = new Log(provider.GetLogWriter("Message Store"));
            this.log.SetCategory("SingletonMessageStore");
        }

        private string[] messages = 
        {
            "You are great!",
            "You are funny!",
            "You smell good :)",
            "You smell bad :("
        };

        private int index = 0;

        public string GetNextMessage()
        {
            log.Write("Checking index for reset...");
            if (index >= messages.Length)
                index = 0;

            var currentIndex = index++;
            log.Write("Index Value: " + currentIndex);

            var message = messages[currentIndex];
            log.Write("Selected Message Value: " + message);

            return message;
        }
    }
}