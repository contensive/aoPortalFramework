
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.Addons.aoPortal.Models;
using Contensive.BaseClasses;
using Constants = Contensive.Addons.aoPortal.Constants;

namespace Contensive.Addons.aoPortal {
    public class PortalClass : AddonBaseClass {
        //====================================================================================================
        /// <summary>
        /// Addon interface. Run addon from doc property PortalGuid or PortalId (form, querystring, doc.setProperty())
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            string returnHtml = "";
            try {
                //
                // get portal to display 
                string instanceId = cp.Doc.GetText("instanceid");
                string visitPropertyName = "portalForInstance" + instanceId;
                int portalId = cp.Doc.GetInteger(Constants.rnSetPortalId);
                if (portalId != 0) {
                    cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                } else {
                    string portalGuid = cp.Doc.GetText(Constants.rnSetPortalGuid);
                    if (!string.IsNullOrEmpty(portalGuid)) {
                        CPCSBaseClass cs = cp.CSNew();
                        if (cs.Open("portals", "ccguid=" + cp.Db.EncodeSQLText(portalGuid), "id", true, "id", 9999, 1)) {
                            portalId = cs.GetInteger("id");
                        }
                        cs.Close();
                        cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                    } else {
                        portalId = cp.Visit.GetInteger(visitPropertyName);
                        if (portalId == 0) {
                            // try portal guid argument
                            portalGuid = cp.Doc.GetText("PortalGuid");
                            if (!string.IsNullOrEmpty(portalGuid)) {
                                //
                                // try guid, comes from addon argument list in distributed addons
                                //
                                CPCSBaseClass cs = cp.CSNew();
                                if (cs.Open("portals", "ccguid=" + cp.Db.EncodeSQLText(portalGuid), "id", true, "id", 9999, 1)) {
                                    portalId = cs.GetInteger("id");
                                }
                                cs.Close();
                            }
                            if (portalId == 0) {
                                //
                                // try value from addon argument
                                //
                                portalId = cp.Doc.GetInteger("Portal");
                                if (portalId == 0) {
                                    //
                                    // use the first portal in the list
                                    //
                                    CPCSBaseClass cs = cp.CSNew();
                                    if (cs.Open("portals", "", "id", true, "id", 9999, 1)) {
                                        portalId = cs.GetInteger("id");
                                    }
                                    cs.Close();
                                }
                            }
                            if (portalId != 0) {
                                cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                            }
                        }
                    }
                }
                if (portalId != 0) {
                    returnHtml = getHtml(cp, "id=" + portalId.ToString());
                } else {
                    returnHtml = getHtml(cp, "");
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "portalClass exception");
            }
            return returnHtml;
        }
        //====================================================================================================
        /// <summary>
        /// get the portal html provided the Sql select criteria for portal. Blank returns the first order by id.
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="portalSelectSqlCriteria"></param>
        /// <returns></returns>
        public string getHtml(CPBaseClass CP, string portalSelectSqlCriteria) {
            string returnHtml = "";
            //
            try {
                if (!CP.User.IsAdmin) {
                    returnHtml = Constants.blockedMessage;
                } else {
                    //
                    // build portal
                    //string frameRqs = CP.Doc.RefreshQueryString;
                    PortalFeatureDataClass feature;
                    PortalDataClass portal = PortalDataClass.loadPortalFromDb(CP, portalSelectSqlCriteria);
                    string frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnSetPortalId, portal.id.ToString(), true);
                    Contensive.Addons.aoPortal.pageWithNavClass innerForm = new Contensive.Addons.aoPortal.pageWithNavClass();
                    //
                    // portal interface - add tabs
                    //
                    foreach (KeyValuePair<string, PortalFeatureDataClass> kvp in portal.featureList) {
                        feature = kvp.Value;
                        if (feature.parentFeatureId == 0) {
                            innerForm.addNav();
                            innerForm.navCaption = feature.heading;
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                        }
                    }
                    //
                    // add developer tab
                    //
                    if (CP.User.IsDeveloper) {
                        if (portal.featureList.ContainsKey(Constants.devToolGuid)) {
                            CP.Site.ErrorReport("loadPortalFromDb, the portal [" + portal.name + "] appears to have the devTool feature saved in either the Db features or the defaultConfig. This is not allowed.");
                        } else {
                            innerForm.addNav();
                            innerForm.navCaption = "Developer Tool";
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, Constants.devToolGuid);
                        }
                    }
                    //
                    // add linked features
                    //
                    if (portal.linkedPortals.Count > 0) {
                        foreach (PortalDataClass linkedPortal in portal.linkedPortals) {
                            innerForm.addNav();
                            innerForm.navCaption = linkedPortal.name;
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnSetPortalId, linkedPortal.id.ToString());
                        }
                    }
                    string body = "";
                    //CPCSBaseClass cs = CP.CSNew();
                    string dstFeatureGuid = CP.Doc.GetText(Constants.rnDstFeatureGuid);
                    //int portalid = CP.Doc.GetInteger(rnPortalId);
                    string activeNavHeading = "";
                    //
                    //   execute feature, if it returns empty, display default feature
                    //
                    if (dstFeatureGuid == Constants.devToolGuid) {
                        //
                        // execute developer tools
                        //
                        CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, Constants.devToolGuid);
                        body = getDevTool(CP, portal, CP.Doc.RefreshQueryString);
                        activeNavHeading = "Developer Tool";
                    } else {
                        if (portal.featureList.ContainsKey(dstFeatureGuid)) {
                            //
                            // add feature guid to frameRqs so if the feature uses ajax, the featureGuid will be part of it
                            // add feature guid to rqs so if an addon is used that does not support frameRqs it will work
                            //
                            feature = portal.featureList[dstFeatureGuid];
                            frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                            if (feature.addonId != 0) {
                                //
                                // feature is an addon, execute it
                                //
                                CP.Doc.SetProperty(Constants.rnFrameRqs, CP.Doc.RefreshQueryString);
                                CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, feature.guid);
                                body = CP.Addon.Execute(feature.addonId);
                                //if (feature.parentFeatureId == 0)
                                //{
                                //    activeNavHeading = feature.heading;
                                //}
                                //else
                                //{
                                //    foreach (KeyValuePair<string, portalFeatureDataClass> kvp in portal.featureList)
                                //    {
                                //        portalFeatureDataClass parentFeature = kvp.Value;
                                //        if (parentFeature.id == feature.parentFeatureId)
                                //        {
                                //            activeNavHeading = parentFeature.heading;
                                //            //
                                //            // if feature returned empty and it is in a feature list, execute the feature list
                                //            //
                                //            if (string.IsNullOrEmpty(body))
                                //            {
                                //                body = getFeatureList(CP, portal, parentFeature, frameRqs);
                                //            }
                                //        }
                                //    }
                                //}
                            } else if (feature.dataContentId != 0) {
                                //
                                // this is a data content feature -- should not be here, link should have taken them to the content
                                //
                                CP.Response.Redirect("?cid=" + feature.dataContentId.ToString());
                                formSimpleClass content = new formSimpleClass {
                                    title = feature.heading,
                                    body = "Redirecting to content"
                                };
                                body = content.getHtml(CP);
                            } else {
                                //
                                // this is a feature list, display the feature list
                                //
                                body = getFeatureList(CP, portal, feature, CP.Doc.RefreshQueryString);
                            }
                            //
                            // set active heading
                            //
                            if (feature.parentFeatureId == 0) {
                                activeNavHeading = feature.heading;
                            } else {
                                foreach (KeyValuePair<string, PortalFeatureDataClass> kvp in portal.featureList) {
                                    PortalFeatureDataClass parentFeature = kvp.Value;
                                    if (parentFeature.id == feature.parentFeatureId) {
                                        activeNavHeading = parentFeature.heading;
                                        //
                                        // if feature returned empty and it is in a feature list, execute the feature list
                                        //
                                        if (string.IsNullOrEmpty(body)) {
                                            body = getFeatureList(CP, portal, parentFeature, CP.Doc.RefreshQueryString);
                                        }
                                    }
                                }
                            }
                            if (feature.addPadding) {
                                body = CP.Html.div(body, "", "afwBodyPad", "");
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(body)) {
                        //
                        // if the feature turns blank, run the default feature
                        //
                        if (portal.defaultFeature != null) {
                            feature = portal.defaultFeature;
                            activeNavHeading = feature.heading;
                            frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                            CP.Doc.SetProperty(Constants.rnFrameRqs, CP.Doc.RefreshQueryString);
                            CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, feature.guid);
                            body = CP.Utils.ExecuteAddon(feature.addonId.ToString());
                            if (feature.addPadding) {
                                body = CP.Html.div(body, "", "afwBodyPad", "");
                            }
                        }
                        if (string.IsNullOrEmpty(body)) {
                            formSimpleClass simple = new formSimpleClass {
                                body = "This portal feature has no content."
                            };
                            body = simple.getHtml(CP);
                        }
                    }
                    innerForm.setActiveNav(activeNavHeading);
                    //
                    //Assemble
                    //
                    innerForm.body = CP.Html.div(body, "", "", "afwBodyFrame");
                    innerForm.title = portal.name;
                    innerForm.isOuterContainer = true;
                    returnHtml = innerForm.getHtml(CP);
                    //
                    // assemble body
                    //
                    CP.Doc.AddHeadStyle(Properties.Resources.styles);
                    CP.Doc.AddHeadJavascript("var afwFrameRqs='" + CP.Doc.RefreshQueryString + "';");
                    CP.Doc.AddHeadJavascript(Properties.Resources.javascript);
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "portalClass");
            }
            return returnHtml;
        }
        //====================================================================================================
        /// <summary>
        /// get a portal feature available to developers that provides tool for creating portals
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        string getDevTool(CPBaseClass CP, PortalDataClass portal, string frameRqs) {
            string body = "Hello World";
            try {
                string section;
                //
                // this is a feature list, display the feature list
                //
                formSimpleClass content = new formSimpleClass {
                    title = "Developer Tool",
                    body = ""
                };
                //
                // process snapshot tool
                //
                if (CP.Doc.GetText("button") == "Take Snapshot") {
                    CPCSBaseClass cs = CP.CSNew();
                    if (cs.Open("portals", "ccguid=" + CP.Db.EncodeSQLText(portal.guid), "", true, "", 9999, 1)) {
                        System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer();
                        string configJson = msJson.Serialize(portal);
                        cs.SetField("defaultConfigJson", configJson);
                    }
                    cs.Close();

                }
                //
                // output snapshot tool
                //
                section = "<h3>Portal Snapshot</h3>";
                section += "<p>Click the snapshot button to save the current features for this portal in the portal's default configuration field.</p>";
                section += CP.Html.Button("button", "Take Snapshot");
                section += "<p>Modify Portal and Portal Features data directly</p>";
                section += "<ul>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Portals") + "\">Portals</a></li>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Portal Features") + "\">Portal Features</a></li>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Admin Framework Reports") + "\">Admin Framework Reports</a></li>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Admin Framework Report Columns") + "\">Admin Framework Report Columns</a></li>";
                section += "</ul>";
                section = CP.Html.Form(section, "", "", "", frameRqs);
                content.body += section;
                //
                //
                //
                body = content.getHtml(CP);
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
            }
            return body;
        }
        //====================================================================================================
        /// <summary>
        /// create a feature list box
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        string getFeatureList(CPBaseClass cp, PortalDataClass portal, PortalFeatureDataClass feature, string frameRqs) {
            string returnBody = "";
            string items = "";
            string qs;
            try {
                string activeNavHeading;
                activeNavHeading = feature.heading;
                formSimpleClass content = new formSimpleClass();
                foreach (KeyValuePair<string, PortalFeatureDataClass> kvp in portal.featureList) {
                    PortalFeatureDataClass liFeature = kvp.Value;
                    if ((liFeature.parentFeatureId == feature.id) && (liFeature.parentFeatureId != 0)) {
                        string featureHeading = liFeature.heading;
                        if (string.IsNullOrEmpty(featureHeading)) {
                            featureHeading = liFeature.name;
                        }
                        if (liFeature.dataContentId != 0) {
                            //
                            // -- display content button if content is not developeronly, or if this is a developer
                            Models.ContentModel featureContent = Models.ContentModel.create(cp, liFeature.dataContentId);
                            if (featureContent != null) {
                                if ((!featureContent.developerOnly) || (cp.User.IsDeveloper)) {
                                    qs = frameRqs;
                                    qs = cp.Utils.ModifyQueryString(qs, "addonid", "", false);
                                    qs = cp.Utils.ModifyQueryString(qs, Constants.rnDstFeatureGuid, "", false);
                                    qs = cp.Utils.ModifyQueryString(qs, "cid", liFeature.dataContentId.ToString());
                                    items += "<li><a target=\"_blank\" href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                                }
                            }
                        } else {
                            qs = frameRqs;
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnDstFeatureGuid, liFeature.guid);
                            items += "<li><a href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                        }
                    }
                }
                content.title = feature.heading;
                content.body = "<ul class=\"afwButtonList\">" + items + "</ul>";
                returnBody = content.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "portalClass.getFeatureList exception");
            }
            return returnBody;
        }
    }
    //
    public class PortalFeatureDataClass {
        public int id;
        public string name;
        public string heading;
        public int parentFeatureId;
        public string parentFeatureGuid;
        public string guid;
        public int addonId;
        public int dataContentId;
        public string dataContentGuid;
        public string addonGuid;
        public string sortOrder;
        public bool addPadding;
    }
}