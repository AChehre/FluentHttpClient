using System;

namespace FluentHttpClient
{
    public class FluentHttpClientMiddlewareConfig
    {
        public FluentHttpClientMiddlewareConfig(Type middleware, object[] args)
        {
            Middleware = middleware;
            Args = args;
        }

        public Type Middleware { get; set; }


        public object[] Args { get; set; }
    }
}