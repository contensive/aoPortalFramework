using Contensive.Addons.PortalFramework.Models;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework {
    public class PortalBuilderClass {
        const int navSize = 99;
        const int subNavSize = 99;
        /// <summary>
        /// The nav items at the top -- features in this portal with no parent
        /// </summary>
        private PortalBuilderDataNavItemModel[] navs { get; set; } = new PortalBuilderDataNavItemModel[navSize];
        private int navMax { get; set; } = -1;
        private int navPtr { get; set; } = -1;
        private PortalBuilderDataSubNavItemModel[] subNavs { get; set; } = new PortalBuilderDataSubNavItemModel[navSize];
        private int subNavMax { get; set; } = -1;
        private int subNavPtr { get; set; } = -1;
        //
        public bool includeBodyPadding { get; set; }
        //
        public bool includeBodyColor { get; set; }
        //
        public bool isOuterContainer { get; set; }
        //
        //====================================================================================================
        //
        private void checkNavPtr() {
            if (navPtr < 0) {
                addNav();
            }
        }
        //
        //====================================================================================================
        //
        public string styleSheet {
            get {
                return Properties.Resources.styles;
            }
        }
        //
        //====================================================================================================
        //
        public string javascript {
            get {
                return Properties.Resources.javascript;
            }
        }
        //
        public string body { get; set; }
        //
        //====================================================================================================
        //
        public string title { get; set; }
        //
        //====================================================================================================
        //
        public string warning { get; set; }
        //
        //====================================================================================================
        //
        public string description { get; set; }
        //
        //====================================================================================================
        //
        public string navCaption {
            get {
                checkNavPtr();
                return navs[navPtr].caption;
            }
            set {
                checkNavPtr();
                navs[navPtr].caption = value;
            }
        }
        //
        //====================================================================================================
        //
        public string navLink {
            get {
                checkNavPtr();
                return navs[navPtr].link;
            }
            set {
                checkNavPtr();
                navs[navPtr].link = value;
            }
        }
        //
        //====================================================================================================
        //
        public void setActiveNav(string caption) {
            for (int navPtr = 0; navPtr <= navMax; navPtr++) {
                if (navs[navPtr].caption.ToLower() == caption.ToLower()) {
                    navs[navPtr].active = true;
                }
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Add a navigation entry. The navCaption and navLink should be set after creating a new entry. The first nav entry does not need to be added.
        /// </summary>
        public void addNav() {
            if (navPtr < navSize) {
                navPtr += 1;
                navs[navPtr] = new PortalBuilderDataNavItemModel() {
                    caption = "",
                    link = "",
                    active = false,
                    isPortalLink = false,
                    navFlyoutList = new List<PortalBuilderDataSubNavItemModel>()
                };
            };
            if (navPtr > navMax) { navMax = navPtr; }
        }
        //
        //====================================================================================================
        //
        public void addNav(PortalBuilderDataNavItemModel navItem) {
            if (navPtr < navSize) {
                navPtr += 1;
                navs[navPtr] = navItem;
                if (navPtr > navMax) { navMax = navPtr; }
            }
        }
        //
        //====================================================================================================
        //
        public void addSubNav(PortalBuilderDataSubNavItemModel subNavItem) {
            if (subNavPtr < navSize) {
                subNavPtr += 1;
                subNavs[subNavPtr] = subNavItem;
                if (subNavPtr > subNavMax) { subNavMax = subNavPtr; }
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Add a navigation entry. The navCaption and navLink should be set after creating a new entry. The first nav entry does not need to be added.
        /// </summary>
        public void addPortalNav() => addNav();
        //
        //====================================================================================================
        //
        public string getHtml(CPBaseClass cp) {
            try {
                PortalBuilderDataModel viewModel = new PortalBuilderDataModel {
                    navList = new List<PortalBuilderDataNavItemModel>(),
                    navListEmpty = true,
                    warning = cp.Utils.EncodeText(cp.UserError.GetList()),
                    title = title,
                    description = description,
                    body = body
                };
                //
                // -- build nav
               for (navPtr = 0; navPtr <= navMax; navPtr++) {
                    PortalBuilderDataNavItemModel nav = navs[navPtr];
                    if (!string.IsNullOrEmpty(nav.caption)) {
                        viewModel.navList.Add(new PortalBuilderDataNavItemModel {
                            caption = nav.caption,
                            link = nav.link,
                            active = nav.active,
                            isPortalLink = nav.isPortalLink,
                            navFlyoutList = nav.navFlyoutList,
                            navFlyoutListEmpty = nav.navFlyoutList.Count == 0
                        });
                    }
                }
                viewModel.navListEmpty = viewModel.navList.Count == 0;
                //
                // -- build subnav

                //
                // -- if outer container, add styles and javascript
                if (isOuterContainer) {
                    cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                    cp.Doc.AddHeadStyle(Properties.Resources.styles);
                }
                //
                // -- render layout
                string layout = cp.Layout.GetLayout(Constants.guidLayoutPortal, Constants.nameLayoutPortal, "portalframework\\AdminUIPageWithNavLayout.html");
                return cp.Mustache.Render(layout, viewModel);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}
