<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<ContactModel>" %>

<div id="contact">
  <h4>Send A Message</h4>
  <form method="post" action="<%= Url.Action("SendMessage", "Contact") %>">
    <fieldset>
      <%= Html.Hidden("targetId", Model.TargetId) %>

      <label for="name">Name <small>(required)</small></label><%= Html.ValidationMessage("name") %>
      <%= Html.TextBox("name", Model.Name) %>

      <label for="email">Email <small>(required)</small></label><%= Html.ValidationMessage("email") %>      
      <%= Html.TextBox("email", Model.Email) %>

      <label for="phone">Phone</label><%= Html.ValidationMessage("phone") %>
      <%= Html.TextBox("phone", Model.Phone) %>

      <label for="message">Message <small>(required)</small></label><%= Html.ValidationMessage("message")%>
      <%= Html.TextArea("message", Model.Message, 4, 100, null) %>
      
      <%= Html.ValidationMessage("error") %>
      <input type="submit" value="Send Message" />
    </fieldset>
  </form>
</div>
