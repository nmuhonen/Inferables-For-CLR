using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleMVCApp.Models.Mocks
{
    public class SingletonMockMessageStore: IMessageStore
    {
        public string GetNextMessage()
        {
            return "Ah... You are mocking me!";
        }
    }
}