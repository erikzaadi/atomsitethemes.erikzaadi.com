/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using System.Xml;

    public class XmlWriterResult : ViewResult
    {
        public XmlWriterResult(Action<XmlWriter> writeXml)
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "text/xml";
            //ContentEncoding = Encoding.UTF8;
            WriteXml = writeXml;
        }

        public Action<XmlWriter> WriteXml { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public Uri Location { get; set; }
        public Uri ContentLocation { get; set; }
        //public Encoding ContentEncoding { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int)StatusCode;
            context.HttpContext.Response.ContentType = ContentType;
            context.HttpContext.Response.ContentEncoding = context.HttpContext.Response.Output.Encoding;
            if (ETag != null) context.HttpContext.Response.AddHeader("ETag", ETag);
            if (Location != null) context.HttpContext.Response.AddHeader("Location", Location.AbsoluteUri);
            if (ContentLocation != null) context.HttpContext.Response.AddHeader("Content-Location", ContentLocation.AbsoluteUri);
            using (XmlWriter w = new XmlTextWriter(context.HttpContext.Response.Output))
            {
                WriteXml(w);
            }
        }
    }
}
