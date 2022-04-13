using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    //
    /// <summary>
    /// mustache view model for page
    /// </summary>
    public class PortalBuilderViewModel {
        public string title { get; set; }
        public string warning { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public bool isOuterContainer { get; set; }
        /// <summary>
        /// this portals main features - features with no parent feature
        /// </summary>
        public List<PortalBuilderNavItemViewModel> navList { get; set; }
        public bool navListEmpty {
            get {
                return navList.Count == 0;
            }
        }
        /// <summary>
        /// current body's sibling features, this is the list to be used as subnav under the main nav
        /// </summary>
        public List<PortalBuilderSubNavItemViewModel> subNavList { get; set; }
        public bool subNavListEmpty {
            get {
                return subNavList.Count == 0;
            }
        }

    }
}
