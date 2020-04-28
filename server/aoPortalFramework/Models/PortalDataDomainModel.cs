
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.aoPortal.Models {
    //
    // admin framework portals
    //
    public class PortalDataClass {
        public string name;
        public string guid;
        public int id;
        public Dictionary<string, PortalFeatureDataClass> featureList;
        public PortalFeatureDataClass defaultFeature;
        public List<PortalDataClass> linkedPortals;
        //====================================================================================================
        /// <summary>
        /// Return a portal object read from the Db based on the portal guid argument
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="portalRecordGuid"></param>
        /// <returns></returns>
        public static PortalDataClass loadPortalFromDb(CPBaseClass CP, string selectSqlCriteria) {
            PortalDataClass portal = new PortalDataClass {
                featureList = new Dictionary<string, PortalFeatureDataClass>()
            };
            try {
                //portalFeatureDataClass firstFeature = null;
                CPCSBaseClass csMan = CP.CSNew();
                CPCSBaseClass csFeature = CP.CSNew();
                string defaultConfigJson;
                if (!csMan.Open("portals", selectSqlCriteria, "id", true, "", 9999, 1)) {
                    //
                    // create demo portal
                    //
                    csMan.Close();
                    portal.name = "Empty";
                    portal.guid = "";
                    portal.id = 0;
                    portal.featureList = new Dictionary<string, PortalFeatureDataClass>();
                    PortalFeatureDataClass feature = new PortalFeatureDataClass {
                        addonId = 0,
                        dataContentId = 0,
                        guid = "",
                        heading = "Sample",
                        id = 0,
                        name = "Demo",
                        parentFeatureId = 0,
                        sortOrder = ""
                    };
                } else {
                    //
                    // load portal
                    //
                    portal.name = csMan.GetText("name");
                    portal.guid = csMan.GetText("ccguid");
                    portal.id = csMan.GetInteger("id");
                    int portalDefaultFeatureId = csMan.GetInteger("defaultFeatureId");
                    defaultConfigJson = csMan.GetText("defaultConfigJson");
                    csMan.Close();
                    //
                    // load portal links
                    //
                    portal.linkedPortals = new List<PortalDataClass>();
                    string sql = "select p.id,p.ccguid,p.name from ccPortals p where p.active>0 order by p.name";
                    //string sql = "select p.id,p.ccguid,p.name from ccPortals p left join ccPortalLinks l on l.toPortalId=p.id where l.fromPortalId=" + portal.id;
                    if (csMan.OpenSQL(sql)) {
                        do {
                            int portalId = csMan.GetInteger("id");
                            if (!portal.id.Equals(portalId)) {
                                PortalDataClass linkedPortal = new PortalDataClass {
                                    id = portalId,
                                    name = csMan.GetText("name")
                                };
                                portal.linkedPortals.Add(linkedPortal);
                            }
                            csMan.GoNext();
                        } while (csMan.OK());
                    }
                    csMan.Close();
                    //
                    // load features 
                    //
                    if (!csFeature.Open("portal features", "portalid=" + portal.id, "sortOrder", true, "", 9999, 1)) {
                        //
                        // no features found, load default portal features
                        //
                        csFeature.Close();
                        if (string.IsNullOrEmpty(defaultConfigJson)) {
                            //
                            // no default, fake a tab
                            //
                            portal.featureList = new Dictionary<string, PortalFeatureDataClass>();
                            PortalFeatureDataClass feature = new PortalFeatureDataClass {
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
                        } else {
                            //
                            // load default and save to Db
                            //
                            System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer();
                            //string configJson = msJson.Serialize(portal);
                            portal = msJson.Deserialize<PortalDataClass>(defaultConfigJson);
                            savePortalToDb(CP, portal);
                            //
                            if (!string.IsNullOrEmpty(portal.defaultFeature.guid)) {
                                if (csFeature.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(portal.defaultFeature.guid), "", true, "", 9999, 1)) {
                                    portal.defaultFeature = loadPortalFeatureFromCs(CP, csFeature);
                                }
                                csFeature.Close();
                            }
                        }
                    } else {
                        //
                        // load features from Db
                        //
                        CPCSBaseClass cs2 = CP.CSNew();
                        do {
                            PortalFeatureDataClass feature = loadPortalFeatureFromCs(CP, csFeature);
                            portal.featureList.Add(csFeature.GetText("ccguid"), feature);
                            if (portal.defaultFeature == null) {
                                portal.defaultFeature = feature;
                            }
                            if (portalDefaultFeatureId == feature.id) {
                                portal.defaultFeature = feature;
                            }
                            csFeature.GoNext();
                        } while (csFeature.OK());
                        csFeature.Close();
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
            }
            return portal;
        }
        //====================================================================================================
        /// <summary>
        /// load a feature from the current row of a contentSet
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="csFeature"></param>
        /// <returns></returns>
        public static PortalFeatureDataClass loadPortalFeatureFromCs(CPBaseClass CP, CPCSBaseClass csFeature) {
            PortalFeatureDataClass feature = new PortalFeatureDataClass();
            try {
                CPCSBaseClass cs2 = CP.CSNew();
                //
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
                if (feature.addonId != 0) {
                    if (cs2.Open("add-ons", "id=" + feature.addonId, "", true, "", 9999, 1)) {
                        feature.addonGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.addonGuid)) {
                            feature.addonGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguid", feature.addonGuid);
                        }
                    }
                    cs2.Close();
                }
                //
                feature.dataContentId = csFeature.GetInteger("dataContentId");
                if (feature.dataContentId != 0) {
                    if (cs2.Open("content", "id=" + feature.dataContentId, "", true, "", 9999, 1)) {
                        feature.dataContentGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.dataContentGuid)) {
                            feature.dataContentGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguild", feature.dataContentGuid);
                        }
                    }
                    cs2.Close();
                }
                //
                feature.parentFeatureId = csFeature.GetInteger("parentFeatureId");
                if (feature.parentFeatureId != 0) {
                    if (cs2.Open("portal features", "id=" + feature.parentFeatureId, "", true, "", 9999, 1)) {
                        feature.parentFeatureGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.parentFeatureGuid)) {
                            feature.parentFeatureGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguid", feature.parentFeatureGuid);
                        }
                    }
                    cs2.Close();
                }
                //
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
            }
            return feature;
        }
        //====================================================================================================
        /// <summary>
        /// If the portal does not exist in the Db, create it and all its features based on the portal argument
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="newPortal"></param>
        public static void savePortalToDb(CPBaseClass CP, PortalDataClass newPortal) {
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
                foreach (KeyValuePair<string, PortalFeatureDataClass> kvp in newPortal.featureList) {
                    PortalFeatureDataClass feature = kvp.Value;
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
                            if (!string.IsNullOrEmpty(feature.addonGuid)) {
                                //
                                // lookup addon by guid, set addonid
                                //
                                if (cs2.Open("add-ons", "ccguid=" + CP.Db.EncodeSQLText(feature.addonGuid), "", true, "", 9999, 1)) {
                                    cs.SetField("addonId", cs2.GetInteger("id").ToString());
                                }
                                cs2.Close();
                            }
                            if (!string.IsNullOrEmpty(feature.dataContentGuid)) {
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

                foreach (KeyValuePair<string, PortalFeatureDataClass> kvp in newPortal.featureList) {
                    PortalFeatureDataClass feature = kvp.Value;
                    if (feature.guid != Constants.devToolGuid) {
                        if (!string.IsNullOrEmpty(feature.parentFeatureGuid)) {
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
            }
        }
    }
}
