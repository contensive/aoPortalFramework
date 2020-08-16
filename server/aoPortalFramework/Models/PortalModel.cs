
using Contensive.Models.Db;

namespace Models.Db {
    public class PortalModel : Contensive.Models.Db.DbBaseModel {
        //
        public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Portals", "ccPortals", "default", false);
        //
        //====================================================================================================
        //
        // -- instance properties
        public string defaultConfigJson { get; set; }
        //
        public int defaultFeatureId { get; set; }
    }
}
