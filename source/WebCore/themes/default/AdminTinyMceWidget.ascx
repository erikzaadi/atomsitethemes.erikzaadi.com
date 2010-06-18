<%@ Control Language="C#" Inherits="ViewUserControl<AdminEntryModel>" %>
<textarea name="content" class="tinymce" style="width:100%;height:300px;">
<%= Model.Entry.Content.ToString() %>
</textarea>
