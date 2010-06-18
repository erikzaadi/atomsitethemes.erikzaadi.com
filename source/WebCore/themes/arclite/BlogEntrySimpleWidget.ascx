<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<EntryModel>" %>
<%if (Model.Entry == null)
  { %>
<div class="content">
    <em>The entry is missing. Please check the log for more information.</em>
</div>
<%}
  else
  {
      Html.RenderPartial("Box", new LiteralConfigModel
                                    {
                                        IncludePath = Model.Entry.Title.Text,
                                        Html =
                                            string.Format("{0}{1}",
                                                          Model.Entry.ContentBeforeSplit.ToString(),
                                                          Model.Entry.Content.IsExtended
                                                              ? string.Format("<a class=\"more\" href=\"{0}\">Read More</a>", Url.RouteIdUrl("BlogEntry", Model.Entry.Id))
                                                              : String.Empty)
                                    });
  }%>
