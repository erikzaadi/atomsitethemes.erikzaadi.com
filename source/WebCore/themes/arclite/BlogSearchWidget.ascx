<%@ Control Language="C#" Inherits="ViewUserControl<BlogSearchModel>" %>
<!-- search form -->
<div class="search-block">
    <div class="searchform-wrap">
        <form action="<%= Model.SearchUrl %>" onsubmit="if (this.term.value == 'Search...' || this.term.value == '') { this.term.focus();  return false;} return true;" method="get">
        <fieldset>
            <input type="text" id="searchbox" class="searchfield watermark" title="Search..."
                name="term" value="Search..."  />
            <input type="submit" value="Go" class="go" />
        </fieldset>
        </form>
    </div>
</div>
<!-- /search form -->
