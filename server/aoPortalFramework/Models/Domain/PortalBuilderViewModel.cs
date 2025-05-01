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
        /// <summary>
        /// true ifthe main nav has no items
        /// </summary>
        public bool navListEmpty {
            get {
                return navList.Count == 0;
            }
        }
        /// <summary>
        /// if for example the user clicks on the Account entry in the main nav, this Account section will display. The accoutn section has several subsections. Those subsections are listed here.
        /// </summary>
        public List<PortalBuilderSubNavItemViewModel> subNavList { get; set; }
        /// <summary>
        /// The bootstrap brand element of the subnav
        /// </summary>
        public string subNavTitle { get; set; }
        public bool subNavListEmpty {
            get {
                return subNavList.Count == 0;
            }
        }

    }
}
