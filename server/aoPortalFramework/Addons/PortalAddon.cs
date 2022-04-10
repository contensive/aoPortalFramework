
using System;
using System.Collections.Generic;
using Contensive.Addons.PortalFramework.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Models.Db;
using Models.Domain;

namespace Contensive.Addons.PortalFramework {
    public class PortalAddon : AddonBaseClass {
        //====================================================================================================
        /// <summary>
        /// Addon interface. Run addon from doc property PortalGuid or PortalId (form, querystring, doc.setProperty())
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                //
                // get portal to display 
                string instanceId = cp.Doc.GetText("instanceid");
                string visitPropertyName = "portalForInstance" + instanceId;
                PortalModel portal = null;
                int portalId = cp.Doc.GetInteger(Constants.rnSetPortalId);
                if (!portalId.Equals(0)) { portal = DbBaseModel.create<PortalModel>(cp, portalId); }
                if (portal == null) {
                    //
                    // -- no setPortalId, try guid
                    string portalGuid = cp.Doc.GetText(Constants.rnSetPortalGuid);
                    if (!string.IsNullOrEmpty(portalGuid)) { portal = DbBaseModel.create<PortalModel>(cp, portalGuid); }
                    if (portal == null) {
                        //
                        // -- no setPortalGuid, try visit property
                        portalId = cp.Visit.GetInteger(visitPropertyName);
                        if (!portalId.Equals(0)) { portal = DbBaseModel.create<PortalModel>(cp, portalId); }
                        if (portal == null) {
                            //
                            // no visit property, try portal guid argument
                            portalGuid = cp.Doc.GetText("PortalGuid");
                            if (!string.IsNullOrEmpty(portalGuid)) { portal = DbBaseModel.create<PortalModel>(cp, portalGuid); }
                            if (portal == null) {
                                //
                                // try value from addon argument
                                portalId = cp.Doc.GetInteger("Portal");
                                if (!portalId.Equals(0)) { portal = DbBaseModel.create<PortalModel>(cp, portalId); }
                            }
                        }
                    }

                }
                cp.Doc.AddRefreshQueryString(Constants.rnSetPortalId, portal.id);
                cp.Visit.SetProperty(visitPropertyName, portal.id);
                return getHtml(cp, portal.id);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "portalClass exception");
                return "";
            }
        }
        //====================================================================================================
        /// <summary>
        /// get the portal html. portalId=0  returns the first order by id.
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="portalSelectSqlCriteria"></param>
        /// <returns></returns>
        public string getHtml(CPBaseClass CP, int portalId) {
            try {
                if (!CP.User.IsAdmin) {
                    return Constants.blockedMessage;
                }
                PortalDataModel portalData = PortalDataModel.create(CP, portalId);
                //string frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnSetPortalId, portalData.id.ToString(), true);
                //
                // -- Add Nav items
                PortalBuilderClass portalBuilder = new PortalBuilderClass();
                PortalDataFeatureModel feature;
                foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in portalData.featureList) {
                    feature = kvp.Value;
                    if (feature.parentFeatureId == 0) {
                        //
                        // -- feature has no parent, add to nav
                        var subNav = new List<PortalBuilderDataSubNavItemModel>();
                        if (feature.addonId == 0 && feature.dataContentId == 0) {
                            //
                            // -- feature has no addon or content, add flyout if it has child features
                            foreach (var subFeature in feature.subFeatureList) {
                                subNav.Add(new PortalBuilderDataSubNavItemModel {
                                    subActive = true,
                                    subCaption = subFeature.heading,
                                    subIsPortalLink = false,
                                    subLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, subFeature.guid),
                                    sublinkTarget = (subFeature.dataContentId > 0) || (!string.IsNullOrEmpty(subFeature.dataContentGuid)) ? "_blank" : ""
                                });
                            }
                        }
                        portalBuilder.addNav(new PortalBuilderDataNavItemModel {
                            active = false,
                            caption = feature.heading,
                            isPortalLink = false,
                            link = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid),
                            navFlyoutList = subNav,
                            linkTarget = (feature.dataContentId > 0) || (!string.IsNullOrEmpty(feature.dataContentGuid)) ? "_blank" : ""
                        });
                        portalBuilder.navCaption = feature.heading;
                        portalBuilder.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                    }
                }
                //
                // add developer nav item
                //
                if (CP.User.IsDeveloper) {
                    if (portalData.featureList.ContainsKey(Constants.devToolGuid)) {
                        CP.Site.ErrorReport("loadPortalFromDb, the portal [" + portalData.name + "] appears to have the devTool feature saved in either the Db features or the defaultConfig. This is not allowed.");
                    } else {
                        portalBuilder.addNav();
                        portalBuilder.navCaption = "Developer Tool";
                        portalBuilder.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, Constants.devToolGuid);
                    }
                }
                //
                // add linked features nav items
                //
                if (portalData.linkedPortals.Count > 0) {
                    foreach (PortalDataModel linkedPortal in portalData.linkedPortals) {
                        portalBuilder.addNav();
                        portalBuilder.navCaption = linkedPortal.name;
                        portalBuilder.navLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnSetPortalId, linkedPortal.id.ToString());
                    }
                }
                string body = "";
                string dstFeatureGuid = CP.Doc.GetText(Constants.rnDstFeatureGuid);
                string activeNavHeading = "";
                //
                //   execute feature, if it returns empty, display default feature
                //
                if (dstFeatureGuid == Constants.devToolGuid) {
                    //
                    // execute developer tools
                    //
                    CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, Constants.devToolGuid);
                    body = getDevTool(CP, portalData, CP.Doc.RefreshQueryString);
                    activeNavHeading = "Developer Tool";
                } else {
                    if (portalData.featureList.ContainsKey(dstFeatureGuid)) {
                        //
                        // add feature guid to frameRqs so if the feature uses ajax, the featureGuid will be part of it
                        // add feature guid to rqs so if an addon is used that does not support frameRqs it will work
                        //
                        feature = portalData.featureList[dstFeatureGuid];
                        //frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                        if (feature.addonId != 0) {
                            //
                            // feature is an addon, execute it
                            //
                            CP.Doc.SetProperty(Constants.rnFrameRqs, CP.Doc.RefreshQueryString);
                            CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, feature.guid);
                            body = CP.Addon.Execute(feature.addonId);
                            //
                            // -- if the feature addon has sibling features, build the subnav
                            //
                            foreach (var testFeature in feature.subFeatureList) {
                                portalBuilder.addSubNav(new PortalBuilderDataSubNavItemModel {
                                    subActive = true,
                                    subCaption = testFeature.heading,
                                    subIsPortalLink = false,
                                    subLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, testFeature.guid),
                                    sublinkTarget = (testFeature.dataContentId > 0) || (!string.IsNullOrEmpty(testFeature.dataContentGuid)) ? "_blank" : ""
                                });
                            }
                            //
                            //
                        } else if (feature.dataContentId != 0) {
                            //
                            // this is a data content feature -- should not be here, link should have taken them to the content
                            //
                            CP.Response.Redirect("?cid=" + feature.dataContentId.ToString());
                            FormSimpleClass content = new FormSimpleClass {
                                title = feature.heading,
                                body = "Redirecting to content"
                            };
                            body = content.getHtml(CP);
                        } else {
                            //
                            // this is a feature list, display the feature list
                            //
                            body = getFeatureList(CP, portalData, feature, CP.Doc.RefreshQueryString);
                        }
                        //
                        // set active heading
                        //
                        if (feature.parentFeatureId == 0) {
                            activeNavHeading = feature.heading;
                        } else {
                            foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in portalData.featureList) {
                                PortalDataFeatureModel parentFeature = kvp.Value;
                                if (parentFeature.id == feature.parentFeatureId) {
                                    activeNavHeading = parentFeature.heading;
                                    //
                                    // if feature returned empty and it is in a feature list, execute the feature list
                                    //
                                    if (string.IsNullOrEmpty(body)) {
                                        body = getFeatureList(CP, portalData, parentFeature, CP.Doc.RefreshQueryString);
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
                    if (portalData.defaultFeature != null) {
                        feature = portalData.defaultFeature;
                        activeNavHeading = feature.heading;
                        //frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                        CP.Doc.SetProperty(Constants.rnFrameRqs, CP.Doc.RefreshQueryString);
                        CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, feature.guid);
                        body = CP.Addon.Execute(feature.addonId);
                        if (feature.addPadding) {
                            body = CP.Html.div(body, "", "afwBodyPad", "");
                        }
                    }
                    if (string.IsNullOrEmpty(body)) {
                        FormSimpleClass simple = new FormSimpleClass {
                            body = "This portal feature has no content."
                        };
                        body = simple.getHtml(CP);
                    }
                }
                portalBuilder.setActiveNav(activeNavHeading);
                //
                //Assemble
                //
                portalBuilder.body = CP.Html.div(body, "", "", "afwBodyFrame");
                portalBuilder.title = portalData.name;
                portalBuilder.isOuterContainer = true;
                string returnHtml = portalBuilder.getHtml(CP);
                //
                // assemble body
                //
                CP.Doc.AddHeadStyle(Properties.Resources.styles);
                CP.Doc.AddHeadJavascript("var afwFrameRqs='" + CP.Doc.RefreshQueryString + "';");
                CP.Doc.AddHeadJavascript(Properties.Resources.javascript);
                //
                return returnHtml;
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "portalClass");
                throw;
            }
        }
        //====================================================================================================
        /// <summary>
        /// get a portal feature available to developers that provides tool for creating portals
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        string getDevTool(CPBaseClass CP, PortalDataModel portal, string frameRqs) {
            try {
                string section;
                //
                // this is a feature list, display the feature list
                //
                FormSimpleClass content = new FormSimpleClass {
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
                return content.getHtml(CP);
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
                throw;
            }
        }
        //====================================================================================================
        /// <summary>
        /// create a feature list box
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        string getFeatureList(CPBaseClass cp, PortalDataModel portal, PortalDataFeatureModel feature, string frameRqs) {
            try {
                //string activeNavHeading = feature.heading;
                string items = "";
                foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in portal.featureList) {
                    PortalDataFeatureModel liFeature = kvp.Value;
                    if ((liFeature.parentFeatureId == feature.id) && (liFeature.parentFeatureId != 0)) {
                        string featureHeading = liFeature.heading;
                        if (string.IsNullOrEmpty(featureHeading)) {
                            featureHeading = liFeature.name;
                        }
                        if (liFeature.dataContentId != 0) {
                            //
                            // -- display content button if content is not developeronly, or if this is a developer
                            ContentModel featureContent = DbBaseModel.create<ContentModel>(cp, liFeature.dataContentId);
                            if (featureContent != null) {
                                if ((!featureContent.developerOnly) || (cp.User.IsDeveloper)) {
                                    string qs = frameRqs;
                                    qs = cp.Utils.ModifyQueryString(qs, "addonid", "", false);
                                    qs = cp.Utils.ModifyQueryString(qs, Constants.rnDstFeatureGuid, "", false);
                                    qs = cp.Utils.ModifyQueryString(qs, "cid", liFeature.dataContentId.ToString());
                                    items += "<li><a target=\"_blank\" href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                                }
                            }
                        } else {
                            string qs = frameRqs;
                            qs = cp.Utils.ModifyQueryString(qs, Constants.rnDstFeatureGuid, liFeature.guid);
                            items += "<li><a href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                        }
                    }
                }
                FormSimpleClass content = new FormSimpleClass {
                    title = feature.heading,
                    body = "<ul class=\"afwButtonList\">" + items + "</ul>"
                };
                return content.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "portalClass.getFeatureList exception");
                throw;
            }
        }
    }
}