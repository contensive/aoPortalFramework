
using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class FormSimpleClass {
        //
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localIncludeForm = false;
        //
        //-------------------------------------------------
        //
        public bool includeBodyPadding { get; set; } = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyColor { get; set; } = true;
        //
        //-------------------------------------------------
        //
        public bool isOuterContainer { get; set; } = false;
        //
        //-------------------------------------------------
        // 
        public string title { get; set; } = "";
        //
        //-------------------------------------------------
        // 
        public string warning { get; set; } = "";
        //
        //-------------------------------------------------
        // 
        public string description { get; set; } = "";
        //
        //-------------------------------------------------
        //
        public string styleSheet => Properties.Resources.styles;
        //
        //-------------------------------------------------
        //
        public string javascript => Properties.Resources.javascript;
        // 
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }
            string result = "";
            //
            if (body != "") { result += body; }
            //
            // headers
            //
            if (description != "") {
                result = cr + "<p id=\"afwDescription\">" + description + "</p>" + result;
            }
            if (warning != "") {
                result = cr + "<div id=\"afwWarning\">" + warning + "</div>" + result;
            }
            if (title != "") {
                result = cr + "<h2 id=\"afwTitle\">" + title + "</h2>" + result;
            }
            //
            // a-- dd form
            if (localIncludeForm) {
                if (localButtonList != "") {
                    localButtonList = ""
                        + cr + "<div class=\"afwButtonCon\">"
                        + indent(localButtonList)
                        + cr + "</div>";
                }
                result = cr + cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
            }
            //
            // -- add wrappers
            if (includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
            if (includeBodyColor) { result = cp.Html.div(result, "", "afwBodyColor", ""); };
            //
            // if outer container, add styles and javascript
            //
            if (isOuterContainer) {
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
        public void addFormHidden(string Name, string Value) {
            localHiddenList += cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            localIncludeForm = true;
        }
        //
        public void addFormHidden(string name, int value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, double value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, DateTime value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, bool value) => addFormHidden(name, value.ToString());
        //
        //-------------------------------------------------
        // 
        public void addFormButton(string buttonValue) {
            addFormButton(buttonValue, "button", "", "");
        }
        public void addFormButton(string buttonValue, string buttonName) {
            addFormButton(buttonValue, buttonName, "", "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId) {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
            localButtonList += cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // 
        public string formActionQueryString {
            get {
                return localFormActionQueryString;
            }
            set {
                localFormActionQueryString = value;
                localIncludeForm = true;
            }
        }
        public string formId {
            get {
                return localFormId;
            }
            set {
                localFormId = value;
                localIncludeForm = true;
            }
        }
        //
        //-------------------------------------------------
        // 
        public string body { get; set; } = "";
        //
        //-------------------------------------------------
        //
        private string indent(string src) {
            return src.Replace(cr, cr2);
        }
    }
}
