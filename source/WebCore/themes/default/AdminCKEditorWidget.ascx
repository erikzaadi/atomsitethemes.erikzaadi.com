<%@ Control Language="C#" Inherits="ViewUserControl<AdminEntryModel>" %>
<textarea class="ckeditor" cols="80" name="content" rows="10">
<%= Html.Encode(Model.Entry.Content.ToString()) %>
</textarea>
