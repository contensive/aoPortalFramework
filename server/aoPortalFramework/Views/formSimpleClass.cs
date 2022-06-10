
using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class FormSimpleClass {
        //
        //-------------------------------------------------
        //
        string localHiddenList { get; set; } = "";
        //
        //-------------------------------------------------
        //
        string localButtonList { get; set; } = "";
        //
        //-------------------------------------------------
        //
        string localFormId { get; set; } = "";
        //
        //-------------------------------------------------
        //
        string localFormActionQueryString { get; set; } = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        bool localIncludeForm { get; set; } = false;
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
        // ====================================================================================================
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
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
                result = Constants.cr + "<p id=\"afwDescription\">" + description + "</p>" + result;
            }
            if (warning != "") {
                result = Constants.cr + "<div id=\"afwWarning\">" + warning + "</div>" + result;
            }
            if (title != "") {
                result = Constants.cr + "<h2 id=\"afwTitle\">" + title + "</h2>" + result;
            }
            //
            // add form
            if (localIncludeForm) {
                if (localButtonList != "") {
                    localButtonList = ""
                        + Constants.cr + "<div class=\"afwButtonCon\">"
                        + indent(localButtonList)
                        + Constants.cr + "</div>";
                }
                result = Constants.cr + cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
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
                    + Constants.cr + "<div id=\"afw\">"
                    + indent(result)
                    + Constants.cr + "</div>";
            }
            //
            // -- set the optional title of the portal subnav
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            return result;
        }
        //
        //-------------------------------------------------
        // 
        public void addFormHidden(string Name, string Value) {
            localHiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
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
            localButtonList += Constants.cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
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
            return src.Replace(Constants.cr, Constants.cr2);
        }
    }
}
