/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using System.Net.Mail;
  using System.Net;
  using System.Xml.Linq;

  public class ContactController : BaseController
  {
    protected IAnnotateService AnnotateService;
    protected IAtomPubService AtomPubService;

    public ContactController(IAtomPubService atompub, IAnnotateService annotate, ILogService logService)
    {
      AtomPubService = atompub;
      AnnotateService = annotate;
      LogService = logService;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SendMessage(ContactModel model)
    {
      LogService.Info("SendMessage");
      ValidateContactModel(model);

      if (this.ModelState.IsValid)
      {
        //TODO: support both ajax and full page
        try
        {
          // get contact configuration
          SendContactMessage(model);
          return PartialView("ContactSuccess", model);
        }
        catch (Exception ex)
        {
          LogService.Error(ex);
          ModelState.AddModelError("error", "An unexpected error occured, please contact the webmaster.");
        }
      }
      return PartialView("ContactWidget", model);
    }

    private void ValidateContactModel(ContactModel model)
    {
      if (string.IsNullOrEmpty(model.Name) || model.Name.Trim().Length == 0)
      {
        this.ModelState.AddModelError("name", "Your name is required.");
      }

      if (string.IsNullOrEmpty(model.Email) || model.Email.Trim().Length == 0)
      {
        this.ModelState.AddModelError("email", "Your email is required.");
      }
      else
      {
        try
        {
          var addr = new MailAddress(model.Email);
        }
        catch (Exception ex)
        {
          LogService.Warn(ex.ToString());
          this.ModelState.AddModelError("email", "Please enter a valid email address, example: bob@example.com");
        }
      }

      if (string.IsNullOrEmpty(model.Message) || model.Message.Trim().Length == 0)
      {
        this.ModelState.AddModelError("message", "Your message is required.");
      }
    }

    private void SendContactMessage(ContactModel model)
    {
      var ws = new ContactAppWorkspace(AppService.GetWorkspace(((Id)model.TargetId).Workspace));
      var contact = ws.Contact;
      //email and save annotation
      AtomEntry entry = new AtomEntry();
      entry.AnnotationType = "contact";
      entry.Title = new AtomTitle() { Text = "Contact Form Post" };
      entry.Content = new AtomContent()
      {
        Type = "text",
        Text = "** This message was posted from a contact form on the " + ws.Title + " website **" +
            Environment.NewLine + Environment.NewLine +
            "Name: " + model.Name.Trim() + Environment.NewLine +
            "Phone: " + model.Phone.Trim() + Environment.NewLine +
            "Email: " + model.Email.Trim() + Environment.NewLine +
            "Message: " + model.Message.Trim()
      };
      entry.Authors = new AtomPerson[] { new AtomAuthor() { Name = model.Name, Email = model.Email } };


      // annotate
      if ((contact.Mode & ContactMode.Annotate) == ContactMode.Annotate)
      {
        AnnotateService.Annotate(model.TargetId, entry, "contact");
      }

      // email
      if ((contact.Mode & ContactMode.Email) == ContactMode.Email)
      {
        // Create the mail message 
        MailMessage mail = new MailMessage();
        mail.Subject = entry.Title.Text;
        mail.Body = entry.Content.Text;

        // the displayed "from" email address 
        mail.From = new MailAddress(model.Email, model.Name);
        mail.IsBodyHtml = false;
        mail.BodyEncoding = System.Text.Encoding.Unicode;
        mail.SubjectEncoding = System.Text.Encoding.Unicode;

        // Add one or more addresses that will receive the mail 
        contact.To.Split(';').ToList().ForEach(s => mail.To.Add(s.Trim()));

        // create the credentials 
        NetworkCredential cred = new NetworkCredential(contact.UserName, contact.Password);

        // create the smtp client...these settings are for gmail 
        SmtpClient smtp = new SmtpClient(contact.Host);
        smtp.UseDefaultCredentials = false;
        smtp.EnableSsl = false;

        // credentials (username, pass of sending account) assigned here 
        smtp.Credentials = cred;
        if (contact.Port.HasValue) smtp.Port = contact.Port.Value;

        smtp.Send(mail);
      }
    }

    public ActionResult ContactWidget(Include include)
    {
      ContactModel model = new ContactModel();

      XElement contactElement = Workspace.Xml.Element(Contact.ContactXName);
      if (contactElement == null)
      {
        LogService.Warn("Contact settings are missing from the workspace. Please configure.");
      }

      model.TargetId = EntryId.ToString();
      return PartialView("ContactWidget", model);
    }


    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateSettings(string workspace, ContactSettingsModel m)
    {
      try
      {
        var w = new ContactAppWorkspace(AppService.GetWorkspace(workspace));
        if (w.Contact == null)
        {
          w.Contact = new Contact();
        }
        // TODO: validation, also use Automapper?
        w.Contact.Mode = (ContactMode)Enum.Parse(typeof(ContactMode), m.Mode);
        w.Contact.To = m.To;
        w.Contact.Host = m.Host;
        w.Contact.Port = m.Port;
        w.Contact.UserName = m.UserName;
        w.Contact.Password = m.Password;
        AtomPubService.UpdateService(AppService);
        TempData["saved"] = true;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        m.Errors.Add(ex.Message);
      }
      TempData["contactSettingsModel"] = m;
      return RedirectToAction("Settings", "Admin", new { workspace = workspace });
    }

    [ScopeAuthorize, AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult ContactSettings(string workspace)
    {
      ContactSettingsModel m;
      if (TempData.ContainsKey("contactSettingsModel"))
        m = (ContactSettingsModel)TempData["contactSettingsModel"];
      else
      {
        m = new ContactSettingsModel();
        var w = new ContactAppWorkspace(AppService.GetWorkspace(workspace));
        if (w.Contact != null)
        {
          //TODO: use automapper?
          m.Mode = w.Contact.Mode.ToString();
          m.To = w.Contact.To;
          m.Host = w.Contact.Host;
          m.Port = w.Contact.Port;
          m.UserName = w.Contact.UserName;
          m.Password = w.Contact.Password;
        }
      }
      return PartialView("ContactSettingsWidget", m);
    }
  }
}
