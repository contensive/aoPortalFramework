using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class contentWithTabsClass
    {
        //
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        const int tabSize = 99;
        struct navStruct
        {
            public string caption;
            public string link;
            public bool active;
        }
        navStruct[] navs = new navStruct[tabSize];
        int tabMax = -1;
        int tabPtr = -1;
        //
        string localBody = "";
        string localTitle = "";
        string localWarning = "";
        string localDescription = "";
        bool localIsOuterContainer = false;
        //
        //-------------------------------------------------
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
        private void checkTabPtr()
        {

            if (tabPtr < 0)
            {
                addTab();
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
        // Title
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
        public string tabCaption
        {
            get
            {
                checkTabPtr();
                return navs[tabPtr].caption;
            }
            set
            {
                checkTabPtr();
                navs[tabPtr].caption = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public string tabLink
        {
            get
            {
                checkTabPtr();
                return navs[tabPtr].link;
            }
            set
            {
                checkTabPtr();
                navs[tabPtr].link = value;
            }
        }
        //
        //-------------------------------------------------
        // 
        //-------------------------------------------------
        //
        public void setActiveTab(string caption)
        {
            int ptr = 0;
            for (ptr = 0; ptr <= tabMax; ptr++)
            {
                if (navs[ptr].caption.ToLower() == caption.ToLower())
                {
                    navs[ptr].active = true;
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
        public void addTab()
        {
            if (tabPtr < tabSize)
            {
                tabPtr += 1;
                navs[tabPtr].caption = "";
                navs[tabPtr].link = "";
                navs[tabPtr].active = false;
                if (tabPtr > tabMax)
                {
                    tabMax = tabPtr;
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
            string s = "";
            string list = "";
            string item = "";
            string userErrors;
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer)
            {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
            }
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "")
            {
                warning = userErrors;
            }
            //
            for (tabPtr = 0; tabPtr <= tabMax; tabPtr++)
            {
                item = navs[tabPtr].caption;
                if (item != "")
                {
                    if (navs[tabPtr].link != "")
                    {
                        item = "<a href=\"" + navs[tabPtr].link + "\">" + item + "</a>";
                    }
                    if (navs[tabPtr].active)
                    {
                        item = "<li class=\"afwTabActive\">" + item + "</li>";
                    }
                    else
                    {
                        item = cr + "<li>" + item + "</li>";
                    }
                    list += item;
                }
            }
            if (list != "")
            {
                s += ""
                    + cr + "<ul class=\"afwBodyTabs\">"
                    + indent(list)
                    + cr + "</ul>";
            }
            //
            // headers
            //
            if (localDescription != "")
            {
                s = cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + s;
            }
            if (localWarning != "")
            {
                s = cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + s;
            }
            if (localTitle != "")
            {
                s = cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + s;
            }
            if (localBody != "")
            {
                s += localBody;
            }
            s = ""
                + cr + "<div class=\"afwTabbedBody\">"
                + indent(s)
                + cr + "</div>";
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer)
            {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                s = ""
                    + cr + "<div id=\"afw\">"
                    + indent(s)
                    + cr + "</div>";
            }
            return s;
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
