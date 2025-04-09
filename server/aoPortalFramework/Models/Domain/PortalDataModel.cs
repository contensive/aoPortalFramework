
using Contensive.BaseClasses;
using Models.Domain;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework.Models {
    //
    // admin framework portals
    //
    public class PortalDataModel {
        public string name;
        public string guid;
        public int id;
        public Dictionary<string, PortalDataFeatureModel> featureList;
        public PortalDataFeatureModel defaultFeature;
        public List<PortalDataModel> linkedPortals;
        //====================================================================================================
        /// <summary>
        /// Return a portal object read from the Db based on the portal guid argument
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="portalRecordGuid"></param>
        /// <returns></returns>
        public static PortalDataModel create(CPBaseClass CP, int portalId) {
            try {
                PortalDataModel result = new PortalDataModel {
                    featureList = new Dictionary<string, PortalDataFeatureModel>()
                };
                using (CPCSBaseClass portalCs = CP.CSNew()) {
                    string defaultConfigJson;
                    if (!portalCs.Open("portals", (portalId == 0 ? "" : "id=" + portalId), "id", true, "", 9999, 1)) {
                        //
                        // -- no portals found, create demo portal
                        portalCs.Close();
                        result.name = "Empty";
                        result.guid = "";
                        result.id = 0;
                        result.featureList = new Dictionary<string, PortalDataFeatureModel>();
                        PortalDataFeatureModel feature = new PortalDataFeatureModel {
                            addonId = 0,
                            dataContentId = 0,
                            guid = "",
                            heading = "Sample",
                            id = 0,
                            name = "Demo",
                            parentFeatureId = 0,
                            sortOrder = ""
                        };
                        return result;
                    }
                    //
                    // -- load selected portal
                    result.name = portalCs.GetText("name");
                    result.guid = portalCs.GetText("ccguid");
                    result.id = portalCs.GetInteger("id");
                    int portalDefaultFeatureId = portalCs.GetInteger("defaultFeatureId");
                    defaultConfigJson = portalCs.GetText("defaultConfigJson");
                    portalCs.Close();
                    //
                    // -- portal links
                    result.linkedPortals = new List<PortalDataModel>();
                    string sql = "select p.id,p.ccguid,p.name from ccPortals p left join ccPortalLinks l on l.toPortalId=p.id where p.active>0 and l.fromPortalId=" + result.id;
                    if (portalCs.OpenSQL(sql)) {
                        do {
                            int linkedPortalId = portalCs.GetInteger("id");
                            if (!result.id.Equals(linkedPortalId)) {
                                PortalDataModel linkedPortal = new PortalDataModel {
                                    id = linkedPortalId,
                                    name = portalCs.GetText("name")
                                };
                                result.linkedPortals.Add(linkedPortal);
                            }
                            portalCs.GoNext();
                        } while (portalCs.OK());
                    }
                    portalCs.Close();
                    //
                    // -- load features and subfeatures
                    string featureSql = ""
                        + " select"
                        + " f.id,f.name,f.heading,f.sortOrder,f.addPadding,f.ccguid,f.addonId,f.dataContentId,f.parentFeatureId,"
                        + " sub.id as subId,sub.name as subName,sub.heading as subheading,sub.sortOrder as subSortOrder,sub.addPadding as subAddPadding,sub.ccguid as subccGuid,sub.addonId as subAddonId,sub.dataContentId as subdatacontentid,sub.parentFeatureId as subparentFeatureId"
                        + " from ccPortalFeatures f left join ccPortalFeatures sub on sub.parentFeatureId=f.id "
                        + " where (f.active>0) and ((sub.active is null)or(sub.active>0))and f.portalid=" + portalId
                        + " order by f.sortOrder,f.name,sub.name";
                    using (CPCSBaseClass csFeature = CP.CSNew()) {
                        if (csFeature.OpenSQL(featureSql)) {
                            //
                            // -- load features from Db
                            int lastFeatureId = 0;
                            PortalDataFeatureModel feature = null;
                            do {
                                if (lastFeatureId != csFeature.GetInteger("id")) {
                                    //
                                    // -- new feature, load the feature
                                    feature = new PortalDataFeatureModel {
                                        id = csFeature.GetInteger("id"),
                                        name = csFeature.GetText("name"),
                                        heading = (string.IsNullOrEmpty(csFeature.GetText("heading")) ? csFeature.GetText("name") : csFeature.GetText("heading")),
                                        sortOrder = csFeature.GetText("sortOrder"),
                                        addPadding = csFeature.GetBoolean("addPadding"),
                                        guid = csFeature.GetText("ccguid"),
                                        addonId = csFeature.GetInteger("addonId"),
                                        dataContentId = csFeature.GetInteger("dataContentId"),
                                        parentFeatureId = csFeature.GetInteger("parentFeatureId"),
                                        subFeatureList = new List<PortalDataFeatureModel>(),
                                        addonGuid = "",
                                        dataContentGuid = "",
                                        parentFeatureGuid = ""
                                    };
                                    //
                                    // -- add the new feature to the list
                                    string featureGuid = csFeature.GetText("ccguid");
                                    if (result.featureList.ContainsKey(featureGuid)) {
                                        //
                                        // -- data error, duplicate features
                                        CP.Site.ErrorReport("Portal Error, 2 portal features with the same guid [" + featureGuid + "]");
                                    } else {
                                        result.featureList.Add(featureGuid, feature);
                                        //if (result.defaultFeature == null) {
                                        //    result.defaultFeature = feature;
                                        //}
                                        if (portalDefaultFeatureId == feature.id) {
                                            result.defaultFeature = feature;
                                        }
                                    }
                                }
                                //
                                // -- load subfeatures
                                int subFeatureId = csFeature.GetInteger("subid");
                                if (subFeatureId > 0) {
                                    PortalDataFeatureModel subFeature = new PortalDataFeatureModel {
                                        id = subFeatureId,
                                        name = csFeature.GetText("subname"),
                                        heading = (string.IsNullOrEmpty(csFeature.GetText("subheading")) ? csFeature.GetText("subname") : csFeature.GetText("subheading")),
                                        sortOrder = csFeature.GetText("subsortOrder"),
                                        addPadding = csFeature.GetBoolean("subaddPadding"),
                                        guid = csFeature.GetText("subccguid"),
                                        addonId = csFeature.GetInteger("subaddonId"),
                                        dataContentId = csFeature.GetInteger("subdataContentId"),
                                        parentFeatureId = csFeature.GetInteger("subparentFeatureId"),
                                    };
                                    feature.subFeatureList.Add(subFeature);
                                }
                                lastFeatureId = csFeature.GetInteger("id");
                                csFeature.GoNext();
                            } while (csFeature.OK());
                            csFeature.Close();
                            //
                            // -- populate parentFeatureGuid from parentFeatureId
                            foreach (var kvp in result.featureList) {
                                PortalDataFeatureModel testFeature = kvp.Value;
                                if ((testFeature.parentFeatureId > 0) && string.IsNullOrEmpty(testFeature.parentFeatureGuid)) {
                                    foreach ( var searchFeature in  result.featureList ) {
                                        if (testFeature.parentFeatureId==searchFeature.Value.id) {
                                            //
                                            testFeature.parentFeatureGuid = searchFeature.Value.guid;
                                            break;
                                        }
                                    }
                                }
                            }
                            return result;
                        }
                    }
                    //
                    // -- no features found, load default portal features
                    if (string.IsNullOrEmpty(defaultConfigJson)) {
                        //
                        // no default, fake a tab
                        //
                        result.featureList = new Dictionary<string, PortalDataFeatureModel>();
                        PortalDataFeatureModel feature = new PortalDataFeatureModel {
                            addonId = 0,
                            dataContentId = 0,
                            guid = "",
                            heading = "Sample",
                            id = 0,
                            name = "Demo",
                            parentFeatureId = 0,
                            sortOrder = "",
                            addPadding = false
                        };
                        return result;
                    }
                    //
                    // legacy - load features from portal record alone
                    //
                    System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = msJson.Deserialize<PortalDataModel>(defaultConfigJson);
                    savePortalToDb(CP, result);
                    //
                    using (CPCSBaseClass featureCs = CP.CSNew()) {
                        if (!string.IsNullOrEmpty(result.defaultFeature.guid)) {
                            if (featureCs.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(result.defaultFeature.guid), "", true, "", 9999, 1)) {
                                result.defaultFeature = loadPortalFeatureFromCs(CP, featureCs);
                            }
                            featureCs.Close();
                        }
                    }
                    return result;
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
                throw;
            }
        }
        //====================================================================================================
        /// <summary>
        /// load a feature from the current row of a contentSet
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="csFeature"></param>
        /// <returns></returns>
        public static PortalDataFeatureModel loadPortalFeatureFromCs(CPBaseClass CP, CPCSBaseClass csFeature) {
            PortalDataFeatureModel feature = new PortalDataFeatureModel();
            try {
                feature.id = csFeature.GetInteger("id");
                feature.name = csFeature.GetText("name");
                feature.heading = csFeature.GetText("heading");
                feature.sortOrder = csFeature.GetText("sortOrder");
                feature.addPadding = csFeature.GetBoolean("addPadding");
                if (string.IsNullOrEmpty(feature.heading)) {
                    feature.heading = feature.name;
                }
                feature.guid = csFeature.GetText("ccguid");
                if (string.IsNullOrEmpty(feature.guid)) {
                    feature.guid = CP.Utils.CreateGuid();
                    csFeature.SetField("ccguid", feature.guid);
                }
                //
                feature.addonId = csFeature.GetInteger("addonId");
                feature.dataContentId = csFeature.GetInteger("dataContentId");
                feature.parentFeatureId = csFeature.GetInteger("parentFeatureId");
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
                throw;
            }
            return feature;
        }
        //====================================================================================================
        /// <summary>
        /// If the portal does not exist in the Db, create it and all its features based on the portal argument
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="newPortal"></param>
        public static void savePortalToDb(CPBaseClass CP, PortalDataModel newPortal) {
            try {
                CPCSBaseClass cs = CP.CSNew();
                CPCSBaseClass cs2 = CP.CSNew();
                //
                // insert or update the portal record
                //
                if (!cs.Open("portals", "ccguid=" + CP.Db.EncodeSQLText(newPortal.guid), "", true, "", 9999, 1)) {
                    cs.Close();
                    cs.Insert("portals");
                    newPortal.id = cs.GetInteger("id");

                }
                newPortal.id = cs.GetInteger("id");
                cs.SetField("ccguid", newPortal.guid);
                cs.SetField("name", newPortal.name);
                cs.Close();
                //
                // insert or update portal feature records
                //
                foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in newPortal.featureList) {
                    PortalDataFeatureModel feature = kvp.Value;
                    if (feature.guid != Constants.devToolGuid) {
                        if (!cs.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(feature.guid), "", true, "", 9999, 1)) {
                            cs.Insert("portal features");
                            cs.SetField("ccGuid", feature.guid);
                        }
                        if (cs.OK()) {
                            feature.id = cs.GetInteger("id");
                            cs.SetField("portalId", newPortal.id.ToString());
                            cs.SetField("name", feature.name);
                            cs.SetField("heading", feature.heading);
                            cs.SetField("sortOrder", feature.sortOrder);
                            if (feature.addonId == 0 && !string.IsNullOrEmpty(feature.addonGuid)) {
                                //
                                // lookup addon by guid, set addonid
                                //
                                if (cs2.Open("add-ons", "ccguid=" + CP.Db.EncodeSQLText(feature.addonGuid), "", true, "", 9999, 1)) {
                                    cs.SetField("addonId", cs2.GetInteger("id").ToString());
                                }
                                cs2.Close();
                            }
                            if (feature.dataContentId == 0 && !string.IsNullOrEmpty(feature.dataContentGuid)) {
                                //
                                // save dataContentId based on dataContentGuid
                                //
                                if (cs2.Open("content", "ccguid=" + CP.Db.EncodeSQLText(feature.dataContentGuid), "", true, "", 9999, 1)) {
                                    feature.dataContentId = cs2.GetInteger("id");
                                    cs.SetField("dataContentId", feature.dataContentId.ToString());
                                }
                                cs2.Close();
                            }
                            if (newPortal.defaultFeature.guid == feature.guid) {
                                newPortal.defaultFeature = feature;
                            }
                        }
                        cs.Close();
                    }
                }
                //
                // lookup parent features by guid and set id
                //

                foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in newPortal.featureList) {
                    PortalDataFeatureModel feature = kvp.Value;
                    if (feature.guid != Constants.devToolGuid) {
                        if (feature.parentFeatureId == 0 && !string.IsNullOrEmpty(feature.parentFeatureGuid)) {
                            //
                            // get the id of the parentFeature
                            //
                            if (cs.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(feature.parentFeatureGuid), "", true, "", 9999, 1)) {
                                feature.parentFeatureId = cs.GetInteger("id");
                            }
                            cs.Close();
                            if (feature.parentFeatureId > 0) {
                                //
                                // set the parentFeatureId field of the current feature
                                //
                                if (cs.Open("portal features", "id=" + feature.id.ToString(), "", true, "", 9999, 1)) {
                                    cs.SetField("parentFeatureId", feature.parentFeatureId.ToString());
                                }
                                cs.Close();
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
                throw;
            }
        }
    }
}
