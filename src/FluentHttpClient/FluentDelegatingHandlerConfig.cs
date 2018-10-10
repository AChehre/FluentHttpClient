using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public class FluentDelegatingHandlerConfig
    {
        public Type Handler { get; set; }


        public object[] Args { get; set; }

        public FluentDelegatingHandlerConfig(Type handler, object[] args)
        {
            Handler = handler;
            Args = args;
        }

    }
}
