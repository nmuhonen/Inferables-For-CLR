using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleMVCApp.Models
{
    public class MessageManager: IMessageManager
    {
        private Log log;
        private IMessageStore store;

        public MessageManager(Log log, IMessageStore messageStore)
        {
            this.log = log;
            this.store = messageStore;
            this.log.SetCategory("Message");
        }

        public string GetMessage()
        {
            log.Write("Getting Message.");
            var message = store.GetNextMessage();
            log.Write("Message Recieved from store: " + message);
            return message;
        }
    }
}
