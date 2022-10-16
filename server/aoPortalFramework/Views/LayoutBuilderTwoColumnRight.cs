
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    /// <summary>
    /// layout split right and left, right larger
    /// </summary>
    public class LayoutBuilderTwoColumnRight : LayoutBuilderBaseClass {
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
        [Obsolete("deprecated, Use title", false)]
        public string headline {
            get {
                return base.title;
            }
            set {
                base.title = value;
            }
        }
        //
        //====================================================================================================
        [Obsolete("deprecated, Use warning", false)]
        public string warningMessage {
            get {
                return base.warning;
            }
            set {
                base.warning = value;
            }
        }
        public new string getHtml(CPBaseClass cp) {
            //
            // -- render layout
            string layout = cp.Layout.GetLayout(Constants.guidLayoutAdminUITwoColumnRight, Constants.nameLayoutAdminUITwoColumnRight, Constants.pathFilenameLayoutAdminUITwoColumnRight);
            body = cp.Mustache.Render(layout, this);
            //
            return base.getHtml(cp);
        }
    }
}
