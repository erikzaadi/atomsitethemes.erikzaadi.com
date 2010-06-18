<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<ContactModel>" %>
<div id="contact">
	<h4>Thank You!</h4>
  <p class="response"><strong>Your message was sent successfully.</strong> We will get back to you as soon as possible.</p>
  <p>
  Name: <%= Model.Name %><br />
  Email: <%= Model.Email %><br />
  Phone: <%= Model.Phone %><br />
  Message: <%= Model.Message %>
  </p>
</div>

