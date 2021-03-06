using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class PageWithNavClass {
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        const int navSize = 99;
        struct navStruct {
            /// <summary>
            /// the displayed text on teh nav
            /// </summary>
            public string caption;
            /// <summary>
            /// if present, this link goes on the nav
            /// </summary>
            public string link;
            /// <summary>
            /// if true, the view is currently on this nav
            /// </summary>
            public bool active;
            /// <summary>
            /// if true, this nav goes to another portal
            /// </summary>
            public bool isPortalLink;
        }
        navStruct[] navs = new navStruct[navSize];
        int navMax = -1;
        int navPtr = -1;
        //
        string localBody = "";
        string localTitle = "";
        string localWarning = "";
        string localDescription = "";
        bool localIsOuterContainer = true;
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
            int navPtr = 0;
            for (navPtr = 0; navPtr <= navMax; navPtr++) {
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
                navs[navPtr].caption = "";
                navs[navPtr].link = "";
                navs[navPtr].active = false;
                navs[navPtr].isPortalLink = false;
                if (navPtr > navMax) {
                    navMax = navPtr;
                }
            }
        }
        //
        /// <summary>
        /// Add a navigation entry. The navCaption and navLink should be set after creating a new entry. The first nav entry does not need to be added.
        /// </summary>
        public void addPortalNav() {
            if (navPtr < navSize) {
                navPtr += 1;
                navs[navPtr].caption = "";
                navs[navPtr].link = "";
                navs[navPtr].active = false;
                navs[navPtr].isPortalLink = true;
                if (navPtr > navMax) {
                    navMax = navPtr;
                }
            }
        }
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            string result = "";
            string navList = "";
            string navItem = "";
            string userErrors;
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }
            //
            // title at top
            //
            if (localTitle != "") {
                result += cr + "<h2 class=\"afwManagerTitle\">" + localTitle + "</h2>";
            }
            //
            // build nav
            //
            for (navPtr = 0; navPtr <= navMax; navPtr++) {
                navItem = navs[navPtr].caption;
                if (navItem != "") {
                    if (navs[navPtr].link != "") {
                        navItem = "<a href=\"" + navs[navPtr].link + "\">" + navItem + "</a>";
                    } else {
                        navItem = "<a href=\"#\" onclick=\"return false;\">" + navItem + "</a>";
                    }
                    if (navs[navPtr].active) {
                        navItem = "<li class=\"afwNavActive\">" + navItem + "</li>";
                    } else if (navs[navPtr].isPortalLink) {
                        navItem = "<li class=\"afwNavPortalLink\">" + navItem + "</li>";
                    } else {
                        navItem = cr + "<li>" + navItem + "</li>";
                    }
                    navList += navItem;
                }
            }
            if (navList != "") {
                result += ""
                    + cr + "<ul class=\"afwNav\">"
                    + indent(navList)
                    + cr + "</ul>";
            }
            //
            // description under nav, over body
            //
            if (localDescription != "") {
                result += cr + "<div class=\"afwManagerDescription\">" + localDescription + "</div>";
            }
            if (localWarning != "") {
                result += cr + "<div id=\"afwWarning\">" + localWarning + "</div>";
            }
            //
            // body
            //
            if (localBody != "") {
                //
                // body padding and color
                //
                if (_includeBodyPadding) { localBody = cp.Html.div(localBody, "", "afwBodyPad", ""); };
                if (_includeBodyColor) { localBody = cp.Html.div(localBody, "", "afwBodyColor", ""); };
                result += localBody;
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
                result = ""
                    + cr + "<div id=\"afw\">"
                    + indent(result)
                    + cr + "</div>";
            }
            return result;
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
