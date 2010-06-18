/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AtomSite.Domain;

    public class ContactSettingsModel : AdminModel
    {
        public string Mode { get; set; }
        public string To { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public IEnumerable<SelectListItem> GetContactModeSelectList()
        {
            return Enum.GetNames(typeof(ContactMode)).Select(m => new SelectListItem() { Text = m, Value = m, Selected = m == Mode});
        }
    }

}
