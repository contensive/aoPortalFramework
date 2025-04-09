
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
                return getPortalAddonHtml(cp, portal.id);
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
        public string getPortalAddonHtml(CPBaseClass CP, int portalId) {
            try {
                if (!CP.User.IsAdmin) {
                    return Constants.blockedMessage;
                }
                PortalDataModel portalData = PortalDataModel.create(CP, portalId);
                //string frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnSetPortalId, portalData.id.ToString(), true);
                //
                // -- Add Nav items
                PortalBuilderClass portalBuilder = new PortalBuilderClass();
                foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in portalData.featureList) {
                    PortalDataFeatureModel feature = kvp.Value;
                    if (feature.parentFeatureId == 0) {
                        //
                        // -- feature has no parent, add to nav
                        var navFlyoutList = new List<PortalBuilderSubNavItemViewModel>();
                        if (feature.addonId == 0 && feature.dataContentId == 0) {
                            //
                            // -- parent feature has no addon or content, add flyout if it has child features
                            foreach (var subFeature in feature.subFeatureList) {
                                navFlyoutList.Add(new PortalBuilderSubNavItemViewModel {
                                    subActive = true,
                                    subCaption = subFeature.heading,
                                    subIsPortalLink = false,
                                    subLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, subFeature.guid),
                                    sublinkTarget = (subFeature.dataContentId > 0) || (!string.IsNullOrEmpty(subFeature.dataContentGuid)) ? "_blank" : ""
                                });
                            }
                        }
                        portalBuilder.addNav(new PortalBuilderNavItemViewModel {
                            active = false,
                            caption = feature.heading,
                            isPortalLink = false,
                            link = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid),
                            linkTarget = (feature.dataContentId > 0) || (!string.IsNullOrEmpty(feature.dataContentGuid)) ? "_blank" : "",
                            navFlyoutList = navFlyoutList
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
                    // execute developer tool panel
                    //
                    CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, Constants.devToolGuid);
                    body = DevToolView.getDevTool(CP, portalData, CP.Doc.RefreshQueryString);
                    activeNavHeading = "Developer Tool";
                } else {
                    if (portalData.featureList.ContainsKey(dstFeatureGuid)) {
                        //
                        // add feature guid to frameRqs so if the feature uses ajax, the featureGuid will be part of it
                        // add feature guid to rqs so if an addon is used that does not support frameRqs it will work
                        //
                        PortalDataFeatureModel dstDataFeature = portalData.featureList[dstFeatureGuid];
                        //frameRqs = CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, feature.guid);
                        if (dstDataFeature.addonId != 0) {
                            //
                            // feature is an addon, execute it
                            //
                            CP.Doc.SetProperty(Constants.rnFrameRqs, CP.Doc.RefreshQueryString);
                            CP.Doc.AddRefreshQueryString(Constants.rnDstFeatureGuid, dstDataFeature.guid);
                            body = CP.Addon.Execute(dstDataFeature.addonId);
                            //
                            // -- portal title is a doc property set in each portal-builder that populates the title in the subnav.
                            portalBuilder.subNavTitle = CP.Doc.GetText("portalSubNavTitle");
                            //
                            // -- if the feature addon has sibling features, build the subnav
                            //
                            if (!string.IsNullOrEmpty(dstDataFeature.parentFeatureGuid)) {
                                if (portalData.featureList.ContainsKey(dstDataFeature.parentFeatureGuid)) {
                                    PortalDataFeatureModel dstFeatureParent = portalData.featureList[dstDataFeature.parentFeatureGuid];
                                    if (dstFeatureParent != null) {
                                        if (dstFeatureParent.addonId>0 || dstFeatureParent.dataContentId > 0) {
                                            //
                                            // -- if parent is content or addon features, show subnav (otherwise, show flyout)
                                            foreach (var dstFeatureSibling in dstFeatureParent.subFeatureList) {
                                                portalBuilder.addSubNav(new PortalBuilderSubNavItemViewModel {
                                                    subActive = true,
                                                    subCaption = dstFeatureSibling.heading,
                                                    subIsPortalLink = false,
                                                    subLink = "?" + CP.Utils.ModifyQueryString(CP.Doc.RefreshQueryString, Constants.rnDstFeatureGuid, dstFeatureSibling.guid),
                                                    sublinkTarget = (dstFeatureSibling.dataContentId > 0) || (!string.IsNullOrEmpty(dstFeatureSibling.dataContentGuid)) ? "_blank" : ""
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            //
                            //
                        } else if (dstDataFeature.dataContentId != 0) {
                            //
                            // this is a data content feature -- should not be here, link should have taken them to the content
                            //
                            CP.Response.Redirect("?cid=" + dstDataFeature.dataContentId.ToString());
                            LayoutBuilderSimple content = new LayoutBuilderSimple {
                                title = dstDataFeature.heading,
                                body = "Redirecting to content"
                            };
                            body = content.getHtml(CP);
                        } else {
                            //
                            // this is a feature list, display the feature list
                            //
                            body =  FeatureListView.getFeatureList(CP, portalData, dstDataFeature, CP.Doc.RefreshQueryString);
                        }
                        //
                        // -- set active heading
                        PortalDataFeatureModel dstFeatureRootFeature = PortalDataFeatureModel.getRootFeature(CP, dstDataFeature, portalData.featureList);
                        activeNavHeading = dstFeatureRootFeature.heading;
                        //
                        // -- if body not created, consider this a feature list
                        if (string.IsNullOrEmpty(body)) {
                            foreach (KeyValuePair<string, PortalDataFeatureModel> kvp in portalData.featureList) {
                                PortalDataFeatureModel parentFeature = kvp.Value;
                                if (parentFeature.id == dstDataFeature.parentFeatureId) {
                                    //
                                    // if feature returned empty and it is in a feature list, execute the feature list
                                    body = FeatureListView.getFeatureList(CP, portalData, parentFeature, CP.Doc.RefreshQueryString);
                                }
                            }
                        }


                        //
                        // -- add pading
                        if (dstDataFeature.addPadding) {
                            body = CP.Html.div(body, "", "afwBodyPad", "");
                        }
                    }
                }
                if (string.IsNullOrEmpty(body)) {
                    //
                    // if the feature returns blank, run the default feature
                    //
                    if (portalData.defaultFeature != null) {
                        PortalDataFeatureModel feature = portalData.defaultFeature;
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
                        //
                        // -- if no default feature set, display dashboard for this portal
                        body = CP.AdminUI.GetWidgetDashboard(portalData.guid);
                        //LayoutBuilderSimple simple = new LayoutBuilderSimple {
                        //    body = "This portal feature has no content."
                        //};
                        //body = simple.getHtml(CP);
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
    }
}