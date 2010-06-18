/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.IO;
  using System.Web.Mvc;

  /// <summary>
  /// Returns a stream as a result. //TODO: check on new FileResult in MVC RC1
  /// </summary>
  public class StreamResult : ViewResult
  {
	  public Stream Stream { get; set; }
	  public string ContentType { get; set; }
	  public string ETag { get; set; }

	  public override void ExecuteResult(ControllerContext context)
	  {
		  context.HttpContext.Response.ContentType = ContentType;
		  if (ETag != null) context.HttpContext.Response.AddHeader("ETag", ETag);
		  const int size = 4096;
		  byte[] bytes = new byte[size];
		  int numBytes;
		  while ((numBytes = Stream.Read(bytes, 0, size)) > 0)
			  context.HttpContext.Response.OutputStream.Write(bytes, 0, numBytes);
	  }
  }
}
