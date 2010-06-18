namespace AtomSite.Plugins.Rater
{
  using System.Collections.Generic;
  using AtomSite.Domain;
  using AtomSite.WebCore;

  public class RaterWidget : CompositeWidget
  {
    public RaterWidget()
      : base("RaterWidget", "Rater", "Rater")
    {
      Description = "This widget includes a star rating that users can rate content with.";
      SupportedScopes = SupportedScopes.Entry;
      AddAsset("jquery.rater-1.1.js");
      AddAsset("RaterWidget.css");
      AreaHints = new[] { "sidetop" };
    }

    public override bool IsEnabled(BaseModel baseModel, Include include)
    {
      return new RaterAppCollection(baseModel.Collection).RatingsOn;
    }

    public override void UpdatePageModel(PageModel pageModel, Include include)
    {
      pageModel.AddToTailScript("$(function() {$('.canrate').rater({ postHref: $('.canrate form').attr('action') });});");
      base.UpdatePageModel(pageModel, include);
    }
  }
}
