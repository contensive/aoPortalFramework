using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class pageWithNavClass
    {
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        const int navSize = 99;
        struct navStruct
        {
            public string caption;
            public string link;
            public bool active;
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
        public bool includeBodyPadding
        {
            get
            {
                return _includeBodyPadding;
            }
            set
            {
                _includeBodyPadding = value;
            }
        }
        bool _includeBodyPadding = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyColor
        {
            get
            {
                return _includeBodyColor;
            }
            set
            {
                _includeBodyColor = value;
            }
        }
        bool _includeBodyColor = true;
        //
        //-------------------------------------------------
        //
        public bool isOuterContainer
        {
            get
            {
                return localIsOuterContainer;
            }
            set
            {
                localIsOuterContainer = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkNavPtr()
        {
            if (navPtr < 0)
            {
                addNav();
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string styleSheet
        {
            get
            {
                return Properties.Resources.styles;
                //return adminFramework.Resource1.styles.ToString();
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string javascript
        {
            get
            {
                return Properties.Resources.javascript;
            }
        }
        //
        //-------------------------------------------------
        // body
        //-------------------------------------------------
        //
        public string body
        {
            get
            {
                return localBody;
            }
            set
            {
                localBody = value;
            }
        }
        //
        //-------------------------------------------------
        // title
        //-------------------------------------------------
        //
        public string title
        {
            get
            {
                return localTitle;
            }
            set
            {
                localTitle = value;
            }
        }
        //
        //-------------------------------------------------
        // Warning
        //-------------------------------------------------
        //
        public string warning
        {
            get
            {
                return localWarning;
            }
            set
            {
                localWarning = value;
            }
        }
        //
        //-------------------------------------------------
        // Description
        //-------------------------------------------------
        //
        public string description
        {
            get
            {
                return localDescription;
            }
            set
            {
                localDescription = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public string navCaption
        {
            get
            {
                checkNavPtr();
                return navs[navPtr].caption;
            }
            set
            {
                checkNavPtr();
                navs[navPtr].caption = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public string navLink
        {
            get
            {
                checkNavPtr();
                return navs[navPtr].link;
            }
            set
            {
                checkNavPtr();
                navs[navPtr].link = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public void setActiveNav( string caption )
        {
            int navPtr = 0;
            for (navPtr = 0; navPtr <= navMax; navPtr++)
            {
                if (navs[navPtr].caption.ToLower() == caption.ToLower())
                {
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
        public void addNav()
        {
            if (navPtr < navSize)
            {
                navPtr += 1;
                navs[navPtr].caption = "";
                navs[navPtr].link = "";
                navs[navPtr].active = false;
                if (navPtr > navMax)
                {
                    navMax = navPtr;
                }
            }
        }
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp)
        {
            string result = "";
            string navList = "";
            string navItem = "";
            string userErrors;
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "")
            {
                warning = userErrors;
            }
            //
            // title at top
            //
            if (localTitle != "")
            {
                result += cr + "<h2 class=\"afwManagerTitle\">" + localTitle + "</h2>";
            }
            //
            // build nav
            //
            for (navPtr = 0; navPtr <= navMax; navPtr++)
            {
                navItem = navs[navPtr].caption;
                if (navItem != "")
                {
                    if (navs[navPtr].link != "")
                    {
                        navItem = "<a href=\"" + navs[navPtr].link + "\">" + navItem + "</a>";
                    }
                    else
                    {
                        navItem = "<a href=\"#\" onclick=\"return false;\">" + navItem + "</a>";
                    }
                    if (navs[navPtr].active)
                    {
                        navItem = "<li class=\"afwNavActive\">" + navItem + "</li>";
                    }
                    else
                    {
                        navItem = cr + "<li>" + navItem + "</li>";
                    }
                    navList += navItem;
                }
            }
            if (navList != "") 
            { 
                result += ""
                    + cr + "<ul class=\"afwNav\">"
                    + indent( navList )
                    + cr + "</ul>";
            }
            //
            // description under nav, over body
            //
            if (localDescription != "")
            {
                result += cr + "<div class=\"afwManagerDescription\">" + localDescription + "</div>";
            }
            if (localWarning != "")
            {
                result += cr + "<div id=\"afwWarning\">" + localWarning + "</div>";
            }
            //
            // body
            //
            if (localBody != "") 
            {
                //
                // body padding and color
                //
                if (_includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
                if (_includeBodyColor) { result = cp.Html.div(result, "", "afwBodyColor", ""); };
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
            if (localIsOuterContainer)
            {
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
        private string indent(string src)
        {
            return src.Replace(cr, cr2);
        }




    }
}
