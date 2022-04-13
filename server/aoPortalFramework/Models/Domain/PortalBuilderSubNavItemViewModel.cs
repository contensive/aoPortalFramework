namespace Contensive.Addons.PortalFramework.Models {
    public class PortalBuilderSubNavItemViewModel {
        /// <summary>
        /// the displayed text on teh nav
        /// </summary>
        public string subCaption { get; set; }
        /// <summary>
        /// if present, this link goes on the nav
        /// </summary>
        public string subLink { get; set; }
        /// <summary>
        /// if true, the view is currently on this nav
        /// </summary>
        public bool subActive { get; set; }
        /// <summary>
        /// if true, this nav goes to another portal
        /// </summary>
        public bool subIsPortalLink { get; set; }
        /// <summary>
        /// _blank for links outside of portal, else empty
        /// </summary>
        public string sublinkTarget { get; set; }
    }
}
