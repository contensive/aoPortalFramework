
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class formSimpleClass
    {
        //
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        string localBody = "";
        //string localFrameRqs = "";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localIncludeForm = false;
        string localDescription = "";
        string localWarning = "";
        string localTitle = "";
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
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp)
        {
            string s = "";
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
            if (localBody != "")
            {
                s += localBody;
                /*
                s += ""
                    + cr + "<div class=\"afwBodyColor\">"
                    + indent(localBody)
                    + cr + "</div>";
                */
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
            //
            // add form
            //
            if (localIncludeForm)
            {
                if (localButtonList != "")
                {
                    localButtonList = ""
                        + cr + "<div class=\"afwButtonCon\">" 
                        + indent(localButtonList)
                        + cr + "</div>";
                }
                s = cr + cp.Html.Form(localButtonList + s + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + "<form action=\"" + localFormActionQueryString + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            //
            // add wrappers
            //
            s = cp.Html.div(s, "", "afwBodyPad", "");
            s = cp.Html.div(s, "", "afwBodyColor", "");
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
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string Name, string Value)
        {
            localHiddenList += cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // add a form button
        //-------------------------------------------------
        //
        public void addFormButton(string buttonValue)
        {
            addFormButton(buttonValue, "button", "","");
        }
        public void addFormButton(string buttonValue, string buttonName)
        {
            addFormButton(buttonValue, buttonName, "","");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId)
        {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass)
        {
            localButtonList += cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // setForm
        //-------------------------------------------------
        //
        public string formActionQueryString
        {
            get
            {
                return localFormActionQueryString;
            }
            set
            {
                localFormActionQueryString = value;
                localIncludeForm = true;
            }
        }
        public string formId
        {
            get
            {
                return localFormId;
            }
            set
            {
                localFormId = value;
                localIncludeForm = true;
            }
        }
        //
        //-------------------------------------------------
        // Refresh Query String
        //-------------------------------------------------
        //
        //public string refreshQueryString
        //{
        //    get
        //    {
        //        return localFrameRqs;
        //    }
        //    set
        //    {
        //        localFrameRqs = value;
        //    }
        //}
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
        //
        //-------------------------------------------------
        //
        private string indent(string src)
        {
            return src.Replace(cr, cr2);
        }
    }
}
