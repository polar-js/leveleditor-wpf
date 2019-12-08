using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leveleditor
{
    public enum StatusType
    {
        Error, Warning, Info, Trace
    }
    public struct Status
    {
        public StatusType Type { get; set; }
        public string Body { get; set; }

        public Status(StatusType type = StatusType.Trace, string body = "")
        {
            Type = type;
            Body = body;
        }
    }
}
