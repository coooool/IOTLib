using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.SaveDB.Interceptor
{
    public struct InterceptorData
    {
        public string block_url { get; set; }
        public string return_body { get; set; }
        public long state_code { get; set; }
    }
}
