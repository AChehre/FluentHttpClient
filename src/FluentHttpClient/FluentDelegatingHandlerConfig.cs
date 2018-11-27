using System;

namespace FluentHttpClient
{
    public class FluentDelegatingHandlerConfig
    {
        public FluentDelegatingHandlerConfig(Type handler, object[] args)
        {
            Handler = handler;
            Args = args;
        }

        public Type Handler { get; set; }


        public object[] Args { get; set; }
    }
}