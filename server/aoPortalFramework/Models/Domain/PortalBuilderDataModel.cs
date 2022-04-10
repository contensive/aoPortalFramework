using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    //
    /// <summary>
    /// mustache view model for page
    /// </summary>
    public class PortalBuilderDataModel {
        public string title { get; set; }
        public string warning { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public bool isOuterContainer { get; set; }
        /// <summary>
        /// this portals main features - features with no parent feature
        /// </summary>
        public List<PortalBuilderDataNavItemModel> navList { get; set; }
        /// <summary>
        /// current body's sibling features, this is the list to be used as subnav under the main nav
        /// </summary>
        public List<PortalBuilderDataSubNavItemModel> subNavList { get; set; }
        public bool navListEmpty { get; set; }
    }
}
