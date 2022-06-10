using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models.Domain {
    // 
    // ====================================================================================================
    /// <summary>
    ///     ''' data to populate HamburburgerMenuLayout
    ///     ''' </summary>
    public class EllipseMenuDataModel {
        public int menuId { get; set; }
        public string content { get; set; }
        public bool hasMenu { get; set; }
        public List<EllipseMenuDataItemModel> menuList { get; set; }
    }
    public class EllipseMenuDataItemModel {
        public string menuName { get; set; }
        public string menuHref { get; set; }
    }
}
