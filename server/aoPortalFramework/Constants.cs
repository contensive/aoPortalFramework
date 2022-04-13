using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.PortalFramework {
    class Constants {
        public static string cr { get; } = System.Environment.NewLine + "\t";
        public static string cr2 { get; } = cr + "\t";
        //
        public const string guidLayoutPageWithNav = "{7B4BEE74-A4A1-4641-9745-25960AFD398F}";
        public const string nameLayoutPageWithNav = "AdminUI Page With Nav Layout";
        public const string pathFilenameLayoutAdminUIPageWithNav = "portalframework\\AdminUIPageWithNavLayout.html";
        //
        public const string guidLayoutAdminUITwoColumnLeft = "{6B0B5593-49A9-45A9-AF64-9A14B34ACB44}";
        public const string nameLayoutAdminUITwoColumnLeft = "AdminUI Two Column Left";
        public const string pathFilenameLayoutAdminUITwoColumnLeft = "portalframework\\AdminUITwoColumnLeftLayout.html";
        //
        public const string guidLayoutAdminUITwoColumnRight = "{41C1F5F9-9AAC-418D-8C05-8B558A02BAF2}";
        public const string nameLayoutAdminUITwoColumnRight = "AdminUI Two Column Right";
        public const string pathFilenameLayoutAdminUITwoColumnRight = "portalframework\\AdminUITwoColumnRightLayout.html";
        //
        public const string blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>";
        public const string rnDstFeatureGuid = "dstFeatureGuid";
        public const string rnFrameRqs = "frameRqs";
        public const string accountPortalGuid = "{12528435-EDBF-4FBB-858F-3731447E24A3}";
        public const string rnPortalId = "portalId";
        public const string devToolGuid = "{13511AA1-3A58-4742-B98F-D92AF853989F}";
        public const string rnSetPortalId = "setPortalId";
        public const string rnSetPortalGuid = "setPortalGuid";
    }
}
