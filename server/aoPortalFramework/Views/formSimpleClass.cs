
using System;
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class FormSimpleClass {
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
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        // 
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            //
            // -- construct body
            HtmlDocRequest request = new HtmlDocRequest() {
                body = body,
                includeBodyPadding = includeBodyPadding,
                includeBodyColor = includeBodyColor,
                buttonList = buttonList,
                csvDownloadFilename = "",
                description = description,
                formActionQueryString = formActionQueryString,
                hiddenList = hiddenList,
                includeForm = includeForm,
                isOuterContainer = isOuterContainer,
                title = title,
                warning = warning
            };
            string result = HtmlController.getReportDoc(cp, request);
            //
            // -- set the optional title of the portal subnav
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            return result;
        }
        //
        //-------------------------------------------------
        // 
        public void addFormHidden(string Name, string Value) {
            hiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            includeForm = true;
        }
        private string hiddenList = "";
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        private bool includeForm { get; set; } = false;
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
            buttonList += HtmlController.getButton(buttonName, buttonValue, buttonId, buttonClass);
            includeForm = true;
        }
        private string buttonList = "";
        //
        //-------------------------------------------------
        // 
        public string formActionQueryString {
            get {
                return formActionQueryString_local;
            }
            set {
                formActionQueryString_local = value;
                includeForm |= !string.IsNullOrEmpty(value);
            }
        }
        private string formActionQueryString_local;
        public string formId {
            get {
                return formId_local;
            }
            set {
                formId_local = value;
                includeForm |= !string.IsNullOrEmpty(value);
            }
        }
        private string formId_local;
        //
        //-------------------------------------------------
        // 
        public string body { get; set; } = "";
    }
}
