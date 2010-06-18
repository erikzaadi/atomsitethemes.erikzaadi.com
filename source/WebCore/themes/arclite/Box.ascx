<%@ Control Language="C#" Inherits="ViewUserControl<LiteralConfigModel>" %>
<!-- box -->
<div class="box">
    <div class="titlewrap">
        <h4>
            <span>
                <%= Model.IncludePath %></span></h4>
    </div>
    <div class="wrapleft">
        <div class="wrapright">
            <div class="tr">
                <div class="bl">
                    <div class="tl">
                        <div class="br the-content">
                            <%= Model.Html %>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- /box -->
