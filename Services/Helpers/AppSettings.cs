using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class AppSettings
    {
        public string? Secret { get; set; }
    }

    public class EmailSettings
    {
        public string? EmailHost { get; set; }
        public string? EmailUserName { get; set; }
        public string? EmailUserPassword { get; set; }
    }

    public class URL
    {
        public string? MainUrl { get; set; }
    }
}
