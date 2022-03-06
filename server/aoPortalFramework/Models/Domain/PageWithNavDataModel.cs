using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    //
    /// <summary>
    /// mustache view model for page
    /// </summary>
    public class PageWithNavDataModel {
        public string title { get; set; }
        public string warning { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public bool isOuterContainer { get; set; }
        public List<PageWithNavDataNavItemModel> navList { get; set; }
        public bool navListEmpty { get; set; }
    }
}
