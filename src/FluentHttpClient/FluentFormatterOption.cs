using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace FluentHttpClient
{
    public class FluentFormatterOption
    {
        public MediaTypeFormatterCollection Formatters { get; } = new MediaTypeFormatterCollection();

        public MediaTypeFormatter Default { get; set; }

        public MediaTypeFormatter GetFormatter(MediaTypeHeaderValue contentType)
        {
            if (!Formatters.Any())
            {
                throw new InvalidOperationException("No media type formatters available.");
            }

            var formatter = contentType != null
                ? Formatters.FirstOrDefault(x => x.SupportedMediaTypes.Any(m => m.MediaType == contentType.MediaType))
                : Default ?? Formatters.FirstOrDefault();
            return formatter ?? throw new InvalidOperationException(
                       $"No media type formatters are available for '{contentType}' content-type.");
        }
    }
}