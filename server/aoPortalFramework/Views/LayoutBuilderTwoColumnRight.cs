
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    public class LayoutBuilderTwoColumnRight {
        //
        //====================================================================================================
        //
        public string contentRight { get; set; } = "";
        //
        //====================================================================================================
        //
        public string contentLeft { get; set; } = "";
        //
        //====================================================================================================
        //
        public string headline { get; set; } = "";
        //
        //====================================================================================================
        public string warningMessage { get; set; } = "";
        //
        //====================================================================================================
        public string description { get; set; } = "";
        //
        // ====================================================================================================
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        //
        public string buttonList { get; set; }
        //
        public bool hasButtonList {
            get { 
                return !string.IsNullOrEmpty(buttonList);
            }
        }
        //
        //====================================================================================================
        //
        public void addFormHidden(string Name, string Value) {
            inputHiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
        }
        private string inputHiddenList = "";
        //
        public void addFormHidden(string name, int value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, double value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, DateTime value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, bool value) => addFormHidden(name, value.ToString());
        //
        //====================================================================================================
        //
        public string getHtml(CPBaseClass cp) {
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (!string.IsNullOrWhiteSpace(userErrors)) {
                warningMessage += userErrors;
            }
            //
            // -- set the optional title of the portal subnav
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            //
            // -- render layout
            string layout = cp.Layout.GetLayout(Constants.guidLayoutAdminUITwoColumnRight, Constants.nameLayoutAdminUITwoColumnRight, Constants.pathFilenameLayoutAdminUITwoColumnRight);
            return cp.Mustache.Render(layout, this) + inputHiddenList;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// add a button to the form
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        /// <param name="buttonId"></param>
        /// <param name="buttonClass"></param>
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
            buttonList += Constants.cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
        }
        public void addFormButton(string buttonValue) {
            addFormButton(buttonValue, "button", "", "");
        }
        public void addFormButton(string buttonValue, string buttonName) {
            addFormButton(buttonValue, buttonName, "", "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId) {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
    }
}
