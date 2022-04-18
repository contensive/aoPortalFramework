
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
        //====================================================================================================
        public string getHtml(CPBaseClass cp) {
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (!string.IsNullOrWhiteSpace(userErrors)) {
                warningMessage += userErrors;
            }
            //
            // -- set the optional title of the portal subnav
            cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle);
            //
            // -- render layout
            string layout = cp.Layout.GetLayout(Constants.guidLayoutAdminUITwoColumnRight, Constants.nameLayoutAdminUITwoColumnRight, Constants.pathFilenameLayoutAdminUITwoColumnRight);
            return cp.Mustache.Render(layout, this);
        }
    }
}
