using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtomSite.Domain;

namespace Domain.Test
{
    [TestClass]
    public class ExceptionTest
    {
        [TestMethod]
        [ExpectedException(typeof(AnnotationNotAllowedException))]
        public void AnnotationNotAllowedExceptionTest()
        {
            Id id = (Id) IdTest.ValidTagForTest;
            throw new AnnotationNotAllowedException(id, "comment", "reason");
        }

        [TestMethod]
        [ExpectedException(typeof(UserNotAuthenticatedException))]
        public void UserNotAuthenticatedExceptionTest()
        {
            throw new UserNotAuthenticatedException("username");
        }

        [TestMethod]
        [ExpectedException(typeof(UserNotAuthorizedException))]
        public void UserNotAuthorizedExceptionTest()
        {
            throw new UserNotAuthorizedException("username", "action");
        }

        //[TestMethod]
        //[ExpectedException(typeof(AuthorOrAdminNotFoundException))]
        //public void AuthorOrAdminNotFoundExceptionTest()
        //{
        //    throw new AuthorOrAdminNotFoundException("username");
        //}

        [TestMethod]
        [ExpectedException(typeof(CategoryAddWhenFixedException))]
        public void CategoryAddWhenFixedExceptionTest()
        {
            throw new CategoryAddWhenFixedException("cat");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidContentTypeException))]
        public void InvalidContentTypeExceptionTest()
        {
            throw new InvalidContentTypeException("contentType");
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void ResourceNotFoundExceptionTest()
        {
            throw new ResourceNotFoundException("resType", "resName");
        }

    }
}
