using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleMVCApp.Models
{
    public interface IMessageStore
    {
        string GetNextMessage();
    }
}
