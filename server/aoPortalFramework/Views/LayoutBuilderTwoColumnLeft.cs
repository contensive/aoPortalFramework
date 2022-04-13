
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    public class LayoutBuilderTwoColumnLeft {
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
        //====================================================================================================
        public string getHtml(CPBaseClass cp) {
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (!string.IsNullOrWhiteSpace(userErrors)) {
                warningMessage += userErrors;
            }
            //
            // -- render layout
            string layout = cp.Layout.GetLayout(Constants.guidLayoutAdminUITwoColumnLeft, Constants.nameLayoutAdminUITwoColumnLeft, Constants.pathFilenameLayoutAdminUITwoColumnLeft);
            return cp.Mustache.Render(layout, this);
        }
    }
}
