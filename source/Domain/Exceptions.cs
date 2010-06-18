/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Runtime.Serialization;

  public class AnnotationNotAllowedException : BaseException
  {
    public AnnotationNotAllowedException(Id id, string type, string reason)
      : base("The {1} annotation on \"{0}\" is not allowed because {2}.", id, type, reason)
    { }
  }

  public class UserNotAuthenticatedException : BaseException
  {
    public UserNotAuthenticatedException(string userName)
      : base("The user \"{0}\" is not authenticated.", userName) { }
  }

  public class UserNotAuthorizedException : BaseException
  {
    public UserNotAuthorizedException(string userName, string action)
      : base("The user \"{0}\" is not authorized to peform the action \"{1}.\"",
      userName, action) { }
  }

  public class UserNotFoundException : BaseException
  {
    public UserNotFoundException(string name)
      : base("The user \"{0}\" does not exist as an admin, author, or contributor.", name)
    { }
  }

  public class CategoryAddWhenFixedException : BaseException
  {
    public CategoryAddWhenFixedException(string category)
      : base("Can't add category \"{0}\" because the categories are fixed.", category)
    { }
  }

  public class CategoryTermAlreadyExistsException : BaseException
  {
    public CategoryTermAlreadyExistsException(string categoryTerm)
      : base("Can't add category because it conflicts with an existing term \"{0}\".", categoryTerm)
    { }
  }

  public class InvalidContentTypeException : BaseException
  {
    public InvalidContentTypeException(string contentType)
      : base("The collection does not accept content types of \"{0}\".", contentType)
    { }
  }

  public class ResourceNotFoundException : BaseException
  {
    public ResourceNotFoundException(string resourceType, string resourceName)
      : base("The {0} \"{1}\" was not found on the server.", resourceType, resourceName)
    { }
  }

  /// <summary>
  /// This simply extends the <see cref="Exception"/> class
  /// by adding a variable length parameter list in the basic
  /// constructor which takes the exception message, and then
  /// apply string.Format if necessary, which is an incredibly
  /// common expectation when throwing exceptions, and should have been
  /// part of the base exception class.
  /// </summary>
  /// <remarks>Taken from PublicDomain project.</remarks>
  [Serializable]
  public class BaseException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Exception"/> class.
    /// </summary>
    public BaseException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Exception"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatParameters">The format parameters.</param>
    public BaseException(string message, params object[] formatParameters)
      : base(formatParameters != null && formatParameters.Length > 0 ? string.Format(message, formatParameters) : message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Exception"/> class.
    /// </summary>
    /// <param name="inner">The inner.</param>
    /// <param name="message">The message.</param>
    /// <param name="formatParameters">The format parameters.</param>
    public BaseException(Exception inner, string message, params object[] formatParameters)
      : base(formatParameters != null && formatParameters.Length > 0 ? string.Format(message, formatParameters) : message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Exception"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected BaseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
