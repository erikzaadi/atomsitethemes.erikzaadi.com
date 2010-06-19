using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtomSite.Domain;
using AtomSite.Repository;

namespace AtomSite.WebCore.plugins.ThemeSwitcher
{
    public class ThemeSwitcherPlugin : BasePlugin
    {
        public ThemeSwitcherPlugin(ILogService logger)
            : base(logger)
        {

        }
        public override void Register(StructureMap.IContainer container, List<SiteRoute> routes, System.Web.Mvc.ViewEngineCollection viewEngines, System.Web.Mvc.ModelBinderDictionary modelBinders, ICollection<Domain.Asset> globalAssets)
        {
            container.Inject(typeof (IThemeSwitcherService),
                             new ThemeSwitcherService(container.GetInstance<IThemeService>(),
                                                      container.GetInstance<IAtomPubService>(),
                                                      container.GetInstance<IAppServiceRepository>()));

            RegisterWidget(container, new CompositeWidget("ThemeSwitcherWidget", "ThemeSwitcher", "Widget")
            {
                Assets = new string[] { "ThemeSwitcher.css", "ThemeSwitcher.js" }.Select(a => new Asset(a)),
                SupportedScopes = SupportedScopes.All,
                AreaHints = new[] { "content" }
            });

            RegisterWidget(container, new CompositeWidget("ThemeWidget", "ThemeSwitcher", "ThemeWidget")
            {
                SupportedScopes = SupportedScopes.All,
            });

            RegisterController<ThemeSwitcherController>(container);

            SetupIncludeInPageArea(container, "Site", "tail", "ThemeSwitcherWidget");
        }


    }
}