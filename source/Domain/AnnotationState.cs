/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  public enum AnnotationState
  {
    On,
    Off, //when turned off via the collection property
    Unauthorized, //when user is not authorized
    Closed, //when manually closed
    Expired //automatically closed via expiration
  }
}
