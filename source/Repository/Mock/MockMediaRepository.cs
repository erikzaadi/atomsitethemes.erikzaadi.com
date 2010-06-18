using System;
using System.IO;
using AtomSite.Domain;

namespace AtomSite.Repository.Mock
{
    /// <summary>
    /// Mock Implementation of IMediaRepository
    ///  - data isn't put anywhere, just stored in memory
    /// </summary>
    public class MockMediaRepository: IMediaRepository
    {
        #region data

        #endregion

        #region constructor

        public MockMediaRepository()
        {
        }

        #endregion

        #region IMediaRepository Members

        public Stream GetMedia(AtomEntry mediaLinkEntry)
        {
            throw new NotImplementedException();
        }

        public string GetMediaEtag(AtomEntry mediaLinkEntry)
        {
            throw new NotImplementedException();
        }

        public void DeleteMedia(AtomEntry mediaLinkEntry)
        {
            throw new NotImplementedException();
        }

        public AtomEntry CreateMedia(AtomEntry mediaLinkEntry, Stream stream)
        {
            throw new NotImplementedException();
        }

        public AtomEntry UpdateMedia(AtomEntry mediaLinkEntry, Stream stream)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
