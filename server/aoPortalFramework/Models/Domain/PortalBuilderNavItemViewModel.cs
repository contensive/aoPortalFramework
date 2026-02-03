using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    /// <summary>
    /// This represents a single item in the main nav
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
        /// <summary>
        /// if the main nav has a flyout, this is the list of items in the flyout
        /// </summary>
        public List<PortalBuilderSubNavItemViewModel> navFlyoutList { get; set; } = new List<PortalBuilderSubNavItemViewModel>();
        /// <summary>
        /// if true, there is no flyout to this main nav
        /// </summary>
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
