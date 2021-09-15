
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
        /// <summary>
        /// only used after the import of a json data set. NOT populated during feature load
        /// </summary>
        public string addonGuid { get; set; }
        public int dataContentId { get; set; }
        /// <summary>
        /// only used after the import of a json data set. NOT populated during feature load
        /// </summary>
        public string dataContentGuid { get; set; }
        public string sortOrder { get; set; }
        public bool addPadding { get; set; }
    }
}
