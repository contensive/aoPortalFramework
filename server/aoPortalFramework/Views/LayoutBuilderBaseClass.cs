
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    public class LayoutBuilderBaseClass {
        //
        //-------------------------------------------------
        /// <summary>
        /// if true, the optional form tag will be blocked. The form tag is added automaatically if buttons, hiddens or a form-action is added
        /// </summary>
        public bool blockFormTag { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// if true, the container between the button rows will include default padding
        /// </summary>
        public bool includeBodyPadding { get; set; } = true;
        //
        //-------------------------------------------------
        /// <summary>
        /// if true, the container between the button rows will include the default background color. Else it is transparent.
        /// </summary>
        public bool includeBodyColor { get; set; } = true;
        //
        //-------------------------------------------------
        /// <summary>
        /// if true, this layoutBuilder will not be contained in other layoutBuilder content. This is used by the default getHtml() to include an outer div with the htmlId "afw", and the styles and javascript
        /// </summary>
        public bool isOuterContainer { get; set; } = false;
        //
        //-------------------------------------------------
        /// <summary>
        /// The headline at the top of the form
        /// </summary>
        public string title { get; set; } = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// set to a message and the warning will appear at the top, between the subhead and the description copy
        /// </summary>
        public string warning { get; set; } = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// simple description text. Will be wrapped in an html paragraph tag.
        /// </summary>
        public string description { get; set; } = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// The default Layoutbuilder styles. Override to customize.
        /// </summary>
        public string styleSheet => Properties.Resources.styles;
        //
        //-------------------------------------------------
        /// <summary>
        /// The default Layoutbuilder script. Override to customize.
        /// </summary>
        public string javascript => Properties.Resources.javascript;
        //
        //-------------------------------------------------
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        // 
        //-------------------------------------------------
        /// <summary>
        /// The default body. Typically you would create a layout by adding content to the individual elements and calling this method. Oveerride this method and consider using HtmlController.getReportDoc()
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
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
                refreshQueryString = refreshQueryString,
                hiddenList = hiddenList,
                includeForm = includeForm,
                isOuterContainer = isOuterContainer,
                title = title,
                warning = warning,
                htmlAfterBody = htmlAfterTable,
                htmlBeforeBody = htmlBeforeTable,
                htmlLeftOfBody = htmlLeftOfTable,
                blockFormTag = blockFormTag
            };
            string result = HtmlController.getReportDoc(cp, request);
            //
            // -- set the optional title of the portal subnav
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            return result;
        }
        //
        //-------------------------------------------------
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void addFormHidden(string Name, string Value) {
            hiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            includeForm = true;
        }
        private string hiddenList = "";
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        private bool includeForm { get; set; } = false;
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void addFormHidden(string name, int value) => addFormHidden(name, value.ToString());
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void addFormHidden(string name, double value) => addFormHidden(name, value.ToString());
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void addFormHidden(string name, DateTime value) => addFormHidden(name, value.ToString());
        /// <summary>
        /// add a form hidden input to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void addFormHidden(string name, bool value) => addFormHidden(name, value.ToString());
        //
        //-------------------------------------------------
        /// <summary>
        /// add a form button to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="buttonValue"></param>
        public void addFormButton(string buttonValue) {
            addFormButton(buttonValue, "button", "", "");
        }
        /// <summary>
        /// add a form button to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        public void addFormButton(string buttonValue, string buttonName) {
            addFormButton(buttonValue, buttonName, "", "");
        }
        /// <summary>
        /// add a form button to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        /// <param name="buttonId"></param>
        public void addFormButton(string buttonValue, string buttonName, string buttonId) {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
        /// <summary>
        /// add a form button to the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        /// <param name="buttonId"></param>
        /// <param name="buttonClass"></param>
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
            buttonList += HtmlController.getButton(buttonName, buttonValue, buttonId, buttonClass);
            includeForm = true;
        }
        private string buttonList = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// The action attribute of the form element that wraps the layout. This will also create a form around the layout. Set blockForm to true to block the automatic form.
        /// </summary>
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
        //
        //-------------------------------------------------
        /// <summary>
        /// The body of the layout.
        /// </summary>
        public string body { get; set; } = "";
        //
        //-------------------------------------------------
        //
        /// <summary>
        /// An html block added to the left of the table. Typically used for filters.
        /// </summary>
        public string htmlLeftOfTable { get; set; } = "";
        //
        //-------------------------------------------------
        //
        /// <summary>
        /// An html block added above the table. Typically used for filters.
        /// </summary>
        public string htmlBeforeTable { get; set; } = "";
        //
        //-------------------------------------------------
        //
        /// <summary>
        /// An html block added below the table. Typically used for filters.
        /// </summary>
        public string htmlAfterTable { get; set; } = "";
        //
        //====================================================================================================
        /// <summary>
        /// Include all nameValue pairs required to refresh the page if someone clicks on a header. For example, if there is a filter dateTo that is not empty, add dateTo=1/1/2000 to the RQS
        /// </summary>
        public string refreshQueryString {
            get {
                return refreshQueryString_Local;
            }
            set {
                refreshQueryString_Local = value;
                //refreshQueryStringSet_Local = true;
            }
        }
        private string refreshQueryString_Local = "";
        //private bool refreshQueryStringSet_Local = false;

    }
}
