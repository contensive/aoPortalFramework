
using Contensive.Models.Db;

namespace Models.Domain {
    //
    public class PortalFeatureDataClass {
        public int id { get; set; }
        public string name { get; set; }
        public string heading { get; set; }
        public int parentFeatureId { get; set; }
        public string parentFeatureGuid { get; set; }
        public string guid { get; set; }
        public int addonId { get; set; }
        public int dataContentId { get; set; }
        public string dataContentGuid { get; set; }
        public string addonGuid { get; set; }
        public string sortOrder { get; set; }
        public bool addPadding { get; set; }
    }
}
