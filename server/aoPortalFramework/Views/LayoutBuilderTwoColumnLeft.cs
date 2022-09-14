
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    public class LayoutBuilderTwoColumnLeft : LayoutBuilderBaseClass {
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
        [Obsolete("deprecated, Use title",false)] public string headline {
            get {
                return base.title;
            } 
            set {
                base.title = value; 
            }
        }
        //
        //====================================================================================================
        [Obsolete("deprecated, Use warning",false)] public string warningMessage {
            get {
                return base.warning;
            }
            set { 
                base.warning = value;   
            } 
        }
        //
        //====================================================================================================
        /// <summary>
        /// render the form to html
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public new string getHtml(CPBaseClass cp) {
            //
            // -- render layout
            string layout = cp.Layout.GetLayout(Constants.guidLayoutAdminUITwoColumnLeft, Constants.nameLayoutAdminUITwoColumnLeft, Constants.pathFilenameLayoutAdminUITwoColumnLeft);
            body = cp.Mustache.Render(layout, this);
            //
            return base.getHtml(cp);
        }
    }
}
