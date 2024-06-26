﻿using System.Collections.Specialized;

namespace LocalUtilities.Net.Http
{
    public class HttpResponseSource
    {
        public HttpResponseSource()
        {
            Cookies = new NameValueCollection();
            OutputStream = new MemoryStream();
            Headers = new NameValueCollection();
        }

        public string Charset { get; set; }

        public string ContentType { get; set; }

        public int ContentLength { get; set; }

        public NameValueCollection Cookies { get; set; }

        public DateTime ExipredDate { get; set; }

        public NameValueCollection Headers { get; set; }

        public string Protocol { get; set; }

        public int StatusCode { get; set; }

        public int SubStatusCode { get; set; }

        public string Status { get; set; }

        public Stream OutputStream { get; set; }
    }
}
