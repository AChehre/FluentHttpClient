using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    class FluentHttpClientMiddlewareConfig
    {
        public Type Middleware { get; set; }

      
        public object[] Args { get; set; }

        public FluentHttpClientMiddlewareConfig(Type middleware, object[] args)
        {
            Middleware = middleware;
            Args = args;
        }

    }
}
