using Contensive.Addons.PortalFramework.Models;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework {
    public class PageWithNavClass {
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        const int navSize = 99;
        private PageWithNavDataNavItemModel[] navs { get; set; } = new PageWithNavDataNavItemModel[navSize];
        private int navMax { get; set; } = -1;
        private int navPtr { get; set; } = -1;
        private string localBody { get; set; } = "";
        private string localTitle { get; set; } = "";
        private string localWarning { get; set; } = "";
        private string localDescription { get; set; } = "";
        private bool localIsOuterContainer { get; set; } = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyPadding {
            get {
                return _includeBodyPadding;
            }
            set {
                _includeBodyPadding = value;
            }
        }
        bool _includeBodyPadding = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyColor {
            get {
                return _includeBodyColor;
            }
            set {
                _includeBodyColor = value;
            }
        }
        bool _includeBodyColor = false;
        //
        //-------------------------------------------------
        //
        public bool isOuterContainer {
            get {
                return localIsOuterContainer;
            }
            set {
                localIsOuterContainer = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkNavPtr() {
            if (navPtr < 0) {
                addNav();
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string styleSheet {
            get {
                return Properties.Resources.styles;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string javascript {
            get {
                return Properties.Resources.javascript;
            }
        }
        //
        //-------------------------------------------------
        // body
        //-------------------------------------------------
        //
        public string body {
            get {
                return localBody;
            }
            set {
                localBody = value;
            }
        }
        //
        //-------------------------------------------------
        // title
        //-------------------------------------------------
        //
        public string title {
            get {
                return localTitle;
            }
            set {
                localTitle = value;
            }
        }
        //
        //-------------------------------------------------
        // Warning
        //-------------------------------------------------
        //
        public string warning {
            get {
                return localWarning;
            }
            set {
                localWarning = value;
            }
        }
        //
        //-------------------------------------------------
        // Description
        //-------------------------------------------------
        //
        public string description {
            get {
                return localDescription;
            }
            set {
                localDescription = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
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
        //-------------------------------------------------
        // 
        //-------------------------------------------------
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
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public void setActiveNav(string caption) {
            for (int navPtr = 0; navPtr <= navMax; navPtr++) {
                if (navs[navPtr].caption.ToLower() == caption.ToLower()) {
                    navs[navPtr].active = true;
                }
            }
        }
        //
        //-------------------------------------------------
        // add a column
        //-------------------------------------------------
        //
        /// <summary>
        /// Add a navigation entry. The navCaption and navLink should be set after creating a new entry. The first nav entry does not need to be added.
        /// </summary>
        public void addNav() {
            if (navPtr < navSize) {
                navPtr += 1;
                navs[navPtr] = new PageWithNavDataNavItemModel() {
                    caption = "",
                    link = "",
                    active = false,
                    isPortalLink = false,
                    subNavList = new List<PageWithNavDataSubNavItemModel>()
                };
            };
            if (navPtr > navMax) { navMax = navPtr; }
        }

        public void addNav(PageWithNavDataNavItemModel navItem) {
            if (navPtr < navSize) {
                navPtr += 1;
                navs[navPtr] = navItem;
                if (navPtr > navMax) { navMax = navPtr; }
            }
        }
        //
        /// <summary>
        /// Add a navigation entry. The navCaption and navLink should be set after creating a new entry. The first nav entry does not need to be added.
        /// </summary>
        public void addPortalNav() => addNav();
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            try {
                PageWithNavDataModel viewModel = new PageWithNavDataModel {
                    navList = new List<PageWithNavDataNavItemModel>(),
                    navListEmpty = true
                };
                string legacyresult = "";
                string navList = "";
                string navItem = "";
                string userErrors;
                //
                // add user errors
                //
                viewModel.warning = cp.Utils.EncodeText(cp.UserError.GetList());
                // ----
                userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
                if (!string.IsNullOrEmpty(userErrors)) {
                    warning = userErrors;
                }
                //
                // title at top
                //
                viewModel.title = localTitle;
                // ----
                if (!string.IsNullOrEmpty(localTitle)) {
                    legacyresult += cr + "<h2 class=\"afwManagerTitle\">" + localTitle + "</h2>";
                }
                //
                // build nav
                //
                for (navPtr = 0; navPtr <= navMax; navPtr++) {
                    PageWithNavDataNavItemModel nav = navs[navPtr];
                    if (!string.IsNullOrEmpty(nav.caption)) {
                        viewModel.navList.Add(new PageWithNavDataNavItemModel {
                            caption = nav.caption,
                            link = nav.link,
                            active = nav.active,
                            isPortalLink = nav.isPortalLink,
                            subNavList = nav.subNavList,
                            subNavListEmpty = nav.subNavList.Count==0
                        });
                    }
                    // ----
                    navItem = nav.caption;
                    if (!string.IsNullOrEmpty(navItem)) {
                        if (!string.IsNullOrEmpty(nav.link)) {
                            navItem = "<a href=\"" + nav.link + "\">" + navItem + "</a>";
                        } else {
                            navItem = "<a href=\"#\" onclick=\"return false;\">" + navItem + "</a>";
                        }
                        if (nav.active) {
                            navItem = "<li class=\"afwNavActive\">" + navItem + "</li>";
                        } else if (nav.isPortalLink) {
                            navItem = "<li class=\"afwNavPortalLink\">" + navItem + "</li>";
                        } else {
                            navItem = cr + "<li>" + navItem + "</li>";
                        }
                        navList += navItem;
                    }
                }
                viewModel.navListEmpty = viewModel.navList.Count == 0;
                if (!string.IsNullOrEmpty(navList )) {
                    legacyresult += ""
                        + cr + "<ul class=\"afwNav\">"
                        + indent(navList)
                        + cr + "</ul>";
                }
                //
                // description under nav, over body
                //
                viewModel.description = localDescription;
                // ----
                if (!string.IsNullOrEmpty(localDescription )) {
                    legacyresult += cr + "<div class=\"afwManagerDescription\">" + localDescription + "</div>";
                }
                if (!string.IsNullOrEmpty(localWarning )) {
                    legacyresult += cr + "<div id=\"afwWarning\">" + localWarning + "</div>";
                }
                //
                // body
                //
                viewModel.body = localBody;
                // ----
                if (!string.IsNullOrEmpty(localBody )) {
                    //
                    // body padding and color
                    //
                    if (_includeBodyPadding) { localBody = cp.Html.div(localBody, "", "afwBodyPad", ""); };
                    if (_includeBodyColor) { localBody = cp.Html.div(localBody, "", "afwBodyColor", ""); };
                    legacyresult += localBody;
                    //s += cp.Html.div(localBody, "", "afwBodyPad", "");
                    ///*
                    //s += ""
                    //    + cr + "<div class=\"afwBodyPad\">"
                    //    + indent(  )
                    //    + cr + "</ul>";
                    // */
                }
                //
                // if outer container, add styles and javascript
                //
                if (localIsOuterContainer) {
                    cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                    cp.Doc.AddHeadStyle(Properties.Resources.styles);
                    legacyresult = ""
                        + cr + "<div id=\"afw\">"
                        + indent(legacyresult)
                        + cr + "</div>";
                }
                if(cp.Site.GetBoolean("AdminUI Update 22.3",false)) {
                    string layout = cp.Layout.GetLayout(Constants.guidAdminUIPageWithNavLayout, Constants.nameAdminUIPageWithNavLayout, "portalframework\\AdminUIPageWithNavLayout.html");
                    string updateresult = cp.Mustache.Render(layout, viewModel);
                    return updateresult;
                }
                return legacyresult;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private string indent(string src) {
            return src.Replace(cr, cr2);
        }
    }
}
