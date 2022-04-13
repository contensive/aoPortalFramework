using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    /// <summary>
    /// mustache view model for list of nav
    /// </summary>
    public class PortalBuilderNavItemViewModel {
        /// <summary>
        /// the displayed text on teh nav
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// if present, this link goes on the nav
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// if true, the view is currently on this nav
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// if true, this nav goes to another portal
        /// </summary>
        public bool isPortalLink { get; set; }
        //
        public List<PortalBuilderSubNavItemViewModel> navFlyoutList { get; set; } = new List<PortalBuilderSubNavItemViewModel>();
        public bool navFlyoutListEmpty {
            get {
                return navFlyoutList.Count == 0;
            }
        }
        /// <summary>
        /// _blank for links outside of portal, else empty
        /// </summary>
        public string linkTarget { get; set; }
    }
}
