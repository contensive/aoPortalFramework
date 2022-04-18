using Contensive.BaseClasses;
using System.Collections.Generic;

namespace Models.Domain {
    //
    public class PortalDataFeatureModel {
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
        public List<PortalDataFeatureModel> subFeatureList { get; set; }
        //
        // ====================================================================================================
        /// <summary>
        /// return the Root feature of the current feature
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="currentFeature"></param>
        /// <returns></returns>
        public static PortalDataFeatureModel getRootFeature(CPBaseClass cp, PortalDataFeatureModel currentFeature, Dictionary<string,PortalDataFeatureModel> featureList) {
            return getRootFeature(cp, currentFeature, featureList, 5);
        }
        //
        public static PortalDataFeatureModel getRootFeature(CPBaseClass cp, PortalDataFeatureModel currentFeature, Dictionary<string, PortalDataFeatureModel> featureDict, int recursionCnt) {
            try {
                if(recursionCnt<=0) { return currentFeature; }
                if (currentFeature.parentFeatureId == 0) { return currentFeature; }
                foreach (var parentFeatureKvp in featureDict) {
                    if (parentFeatureKvp.Value.id == currentFeature.parentFeatureId) {
                        if (parentFeatureKvp.Value.parentFeatureId == 0) { return parentFeatureKvp.Value; }
                        return getRootFeature(cp, parentFeatureKvp.Value, featureDict, --recursionCnt);
                    };
                }
                return currentFeature;
            } catch (System.Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}
