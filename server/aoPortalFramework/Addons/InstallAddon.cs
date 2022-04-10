
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.PortalFramework {
    public class InstallAddon : AddonBaseClass {
        //====================================================================================================
        /// <summary>
        /// Addon interface. Run addon from doc property PortalGuid or PortalId (form, querystring, doc.setProperty())
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                //
                // -- delete layout records so they will repopulate with collection file
                cp.Db.ExecuteNonQuery("delete from cclayouts where ccguid=" + cp.Db.EncodeSQLText(Constants.guidLayoutPortal));
                //
                return "";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return "";
            }
        }
    }
}