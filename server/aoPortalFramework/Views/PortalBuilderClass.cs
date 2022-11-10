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
        private List<PortalBuilderNavItemViewModel> navs { get; set; } = new List<PortalBuilderNavItemViewModel>();
        private int navMax { get; set; } = -1;
        private int navPtr { get; set; } = -1;
        private List<PortalBuilderSubNavItemViewModel> subNavs { get; set; } = new List<PortalBuilderSubNavItemViewModel>();
        private int subNavMax { get; set; } = -1;
        private int subNavPtr { get; set; } = -1;
        /// <summary>
        /// if true, background padding added
        /// </summary>
        public bool includeBodyPadding { get; set; }
        /// <summary>
        /// if true, background color added
        /// </summary>
        public bool includeBodyColor { get; set; }
        /// <summary>
        /// if true, styles and js are added on return
        /// </summary>
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
        /// <summary>
        /// title added to the Nav
        /// </summary>
        public string title { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// optional title added to the subnav. Example, if the main nav item is a list of accounts. Click on an account takes user to a child feature. The subnav appears for the child features and the subNavTitle could be Account 92
        /// </summary>
        public string subNavTitle { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// warning message added below nav
        /// </summary>
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
                navs.Add(new PortalBuilderNavItemViewModel() {
                    caption = "",
                    link = "",
                    active = false,
                    isPortalLink = false,
                    navFlyoutList = new List<PortalBuilderSubNavItemViewModel>()
                });
            };
            if (navPtr > navMax) { navMax = navPtr; }
        }
        //
        //====================================================================================================
        //
        public void addNav(PortalBuilderNavItemViewModel navItem) {
            if (navPtr < navSize) {
                navPtr += 1;
                navs.Add(navItem);
                if (navPtr > navMax) { navMax = navPtr; }
            }
        }
        //
        //====================================================================================================
        //
        public void addSubNav(PortalBuilderSubNavItemViewModel subNavItem) {
            if (subNavPtr < navSize) {
                subNavPtr += 1;
                subNavs.Add(subNavItem);
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
                //
                // todo, a second model is not needed
                PortalBuilderViewModel viewModel = new PortalBuilderViewModel {
                    navList = new List<PortalBuilderNavItemViewModel>(),
                    subNavList = new List<PortalBuilderSubNavItemViewModel>(),
                    warning = cp.Utils.ConvertHTML2Text(cp.UserError.GetList()),
                    title = title,
                    description = description,
                    body = body,
                    subNavTitle = subNavTitle
                };
                //
                // -- build nav
                foreach (var nav in navs) {
                    if (!string.IsNullOrEmpty(nav.caption)) {
                        viewModel.navList.Add(nav);
                    }
                }
                //
                // -- build subnav
                foreach (var subNav in subNavs) {
                    if (!string.IsNullOrEmpty(subNav.subCaption)) {
                        viewModel.subNavList.Add(subNav);
                    }
                }
                //
                // -- if outer container, add styles and javascript
                if (isOuterContainer) {
                    cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                    cp.Doc.AddHeadStyle(Properties.Resources.styles);
                }
                //
                // -- render layout
                string layout = cp.Layout.GetLayout(Constants.guidLayoutPageWithNav, Constants.nameLayoutPageWithNav, Constants.pathFilenameLayoutAdminUIPageWithNav);
                return cp.Mustache.Render(layout, viewModel);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}
