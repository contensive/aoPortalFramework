
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class portalClass : AddonBaseClass
	{
        //
        const string blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>";
        const string rnDstFeatureGuid = "dstFeatureGuid";
        const string rnFrameRqs = "frameRqs";
        const string accountPortalGuid = "{12528435-EDBF-4FBB-858F-3731447E24A3}";
        const string rnPortalId = "portalId";
        const string devToolGuid = "{13511AA1-3A58-4742-B98F-D92AF853989F}";
        const string rnSetPortalId = "setPortalId";
        const string rnSetPortalGuid = "setPortalGuid";
        //====================================================================================================
        /// <summary>
        /// Addon interface. Run addon from doc property PortalGuid or PortalId (form, querystring, doc.setProperty())
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp )
        {
            string returnHtml = "";
            try
            {
                string portalGuid;
                string instanceId;
                int portalId;
                string visitPropertyName;
                //
                // get portal to display 
                //
                instanceId = cp.Doc.GetText("instanceid");
                visitPropertyName = "portalForInstance" + instanceId;
                portalId = cp.Doc.GetInteger(rnSetPortalId);
                if (portalId != 0)
                {
                    cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                }
                else
                {
                    portalGuid = cp.Doc.GetText(rnSetPortalGuid);
                    if (!string.IsNullOrEmpty(portalGuid))
                    {
                        CPCSBaseClass cs = cp.CSNew();
                        if (cs.Open("portals", "ccguid=" + cp.Db.EncodeSQLText(portalGuid), "id", true, "id"))
                        {
                            portalId = cs.GetInteger("id");
                        }
                        cs.Close();
                        cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                    }
                    else
                    {
                        portalId = cp.Visit.GetInteger(visitPropertyName);
                        if (portalId == 0)
                        {
                            // try portal guid argument
                            portalGuid = cp.Doc.GetText("PortalGuid");
                            if (!string.IsNullOrEmpty(portalGuid))
                            {
                                //
                                // try guid, comes from addon argument list in distributed addons
                                //
                                CPCSBaseClass cs = cp.CSNew();
                                if (cs.Open("portals", "ccguid=" + cp.Db.EncodeSQLText(portalGuid), "id", true, "id"))
                                {
                                    portalId = cs.GetInteger("id");
                                }
                                cs.Close();
                            }
                            if (portalId == 0)
                            {
                                //
                                // try value from addon argument
                                //
                                portalId = cp.Doc.GetInteger("Portal");
                                if (portalId == 0)
                                {
                                    //
                                    // use the first portal in the list
                                    //
                                    CPCSBaseClass cs = cp.CSNew();
                                    if (cs.Open("portals", "", "id", true, "id"))
                                    {
                                        portalId = cs.GetInteger("id");
                                    }
                                    cs.Close();
                                }
                            }
                            if (portalId != 0)
                            {
                                cp.Visit.SetProperty(visitPropertyName, portalId.ToString());
                            }
                        }
                    }
                }
                if (portalId != 0)
                {
                    returnHtml = getHtml(cp, "id=" + portalId.ToString());
                }
                else
                {
                    returnHtml = getHtml(cp, "");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "adminFramework.portalClass exception");
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
        public string getHtml(CPBaseClass CP, string portalSelectSqlCriteria)
		{
			string returnHtml = "";
			//
			try {
				if (!CP.User.IsAdmin) {
					returnHtml = blockedMessage;
				} else {
					CPBlockBaseClass form = CP.BlockNew();
					string frameRqs = CP.Doc.RefreshQueryString;
					adminFramework.pageWithNavClass innerForm = new adminFramework.pageWithNavClass();
					string body = "";
					CPCSBaseClass cs = CP.CSNew();
					string dstFeatureGuid = CP.Doc.GetText(rnDstFeatureGuid);
					portalFeatureDataClass feature;
					int portalid = CP.Doc.GetInteger(rnPortalId);
                    string activeNavHeading = "";
                    //
					// build portal
					//
                    portalDataClass portal = loadPortalFromDb(CP, portalSelectSqlCriteria);
                    frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnSetPortalId, portal.id.ToString(), true);
					//
					// portal interface - add tabs
					//
					foreach (KeyValuePair<string, portalFeatureDataClass> kvp in portal.featureList) {
						feature = kvp.Value;
                        if (feature.parentFeatureId == 0)
                        {
                            innerForm.addNav();
                            innerForm.navCaption = feature.heading;
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
                        }
					}
                    //
                    // add developer tab
                    //
                    if (CP.User.IsDeveloper)
                    {
                        if (portal.featureList.ContainsKey(devToolGuid))
                        {
                            CP.Site.ErrorReport("loadPortalFromDb, the portal [" + portal.name + "] appears to have the devTool feature saved in either the Db features or the defaultConfig. This is not allowed.");
                        }
                        else
                        {
                            innerForm.addNav();
                            innerForm.navCaption = "Developer Tool";
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, devToolGuid);
                        }
                    }
                    //
                    // add linked features
                    //
                    if (portal.linkedPortals.Count > 0)
                    {
                        foreach(portalDataClass linkedPortal in portal.linkedPortals)
                        {
                            innerForm.addNav();
                            innerForm.navCaption = linkedPortal.name;
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(frameRqs, rnSetPortalId, linkedPortal.id.ToString() );
                        }
                    }
                    //
					//   execute feature, if it returns empty, display default feature
					//
                    if (dstFeatureGuid == devToolGuid)
                    {
                        //
                        // execute developer tools
                        //
                        CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, devToolGuid);
                        body = getDevTool(CP, portal, frameRqs );
                        activeNavHeading = "Developer Tool";
                    }
                    else
                    {
                        if (portal.featureList.ContainsKey(dstFeatureGuid))
                        {
                            //
                            // add feature guid to frameRqs so if the feature uses ajax, the featureGuid will be part of it
                            // add feature guid to rqs so if an addon is used that does not support frameRqs it will work
                            //
                            feature = portal.featureList[dstFeatureGuid];
                            frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
                            if (feature.addonId != 0)
                            {
                                //
                                // feature is an addon, execute it
                                //
                                CP.Doc.SetProperty(rnFrameRqs, frameRqs);
                                CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
                                body = CP.Utils.ExecuteAddon(feature.addonId.ToString());
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
                            }
                            else if (feature.dataContentId != 0)
                            {
                                //
                                // this is a data content feature -- should not be here, link should have taken them to the content
                                //
                                CP.Response.Redirect("?cid=" + feature.dataContentId.ToString());
                                formSimpleClass content = new formSimpleClass();
                                content.title = feature.heading;
                                content.body = "Redirecting to content";
                                body = content.getHtml(CP);
                            }
                            else
                            {
                                //
                                // this is a feature list, display the feature list
                                //
                                body = getFeatureList(CP, portal, feature, frameRqs);
                            }
                            //
                            // set active heading
                            //
                            if (feature.parentFeatureId == 0)
                            {
                                activeNavHeading = feature.heading;
                            }
                            else
                            {
                                foreach (KeyValuePair<string, portalFeatureDataClass> kvp in portal.featureList)
                                {
                                    portalFeatureDataClass parentFeature = kvp.Value;
                                    if (parentFeature.id == feature.parentFeatureId)
                                    {
                                        activeNavHeading = parentFeature.heading;
                                        //
                                        // if feature returned empty and it is in a feature list, execute the feature list
                                        //
                                        if (string.IsNullOrEmpty(body))
                                        {
                                            body = getFeatureList(CP, portal, parentFeature, frameRqs);
                                        }
                                    }
                                }
                            }
                            if (feature.addPadding)
                            {
                                body = CP.Html.div(body, "", "afwBodyPad", "");
                            }
                        }
                    }
					if (string.IsNullOrEmpty(body)) {
						//
						// if the feature turns blank, run the default feature
						//
                        if (portal.defaultFeature!=null)
                        {
                            feature = portal.defaultFeature;
                            activeNavHeading = feature.heading;
                            frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
                            CP.Doc.SetProperty(rnFrameRqs, frameRqs);
                            CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
                            body = CP.Utils.ExecuteAddon(feature.addonId.ToString());
                            if (feature.addPadding)
                            {
                                body = CP.Html.div(body, "", "afwBodyPad", "");
                            }
                        }
                        if(string.IsNullOrEmpty(body))
                        {
                            formSimpleClass simple = new formSimpleClass();
                            simple.body = "This portal feature has no content.";
                            body = simple.getHtml(CP);
                        }
					}
                    innerForm.setActiveNav(activeNavHeading);
					//
					//Assemble
					//
					innerForm.body = CP.Html.div(body, "" ,"" , "afwBodyFrame");
					innerForm.title = portal.name;
					innerForm.isOuterContainer = true;
					returnHtml = innerForm.getHtml(CP);
					//
					// assemble body
					//
					CP.Doc.AddHeadStyle(Properties.Resources.styles);
                    CP.Doc.AddHeadJavascript("var afwFrameRqs='" + frameRqs + "';");
                    CP.Doc.AddHeadJavascript(Properties.Resources.javascript);
                }
			} catch (Exception ex) {
				CP.Site.ErrorReport(ex, "portalClass");
			}
			return returnHtml;
		}
        //====================================================================================================
        /// <summary>
        /// Return a portal object read from the Db based on the portal guid argument
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="portalRecordGuid"></param>
        /// <returns></returns>
        public portalDataClass loadPortalFromDb(CPBaseClass CP, string selectSqlCriteria )
        {
            portalDataClass portal = new portalDataClass();
            portal.featureList = new Dictionary<string, portalFeatureDataClass>();
            try
            {
                //portalFeatureDataClass firstFeature = null;
                CPCSBaseClass csMan = CP.CSNew();
                CPCSBaseClass csFeature = CP.CSNew();
                string defaultConfigJson;
                if (!csMan.Open("portals", selectSqlCriteria,"id"))
                {
                    //
                    // create demo portal
                    //
                    csMan.Close();
                    portal.name = "Empty";
                    portal.guid = "";
                    portal.id = 0;
                    portal.featureList = new Dictionary<string, portalFeatureDataClass>();
                    portalFeatureDataClass feature = new portalFeatureDataClass();
                    feature.addonId = 0;
                    feature.dataContentId = 0;
                    feature.guid = "";
                    feature.heading = "Sample";
                    feature.id = 0;
                    feature.name = "Demo";
                    feature.parentFeatureId = 0;
                    feature.sortOrder = "";
                }
                else
                {
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
                    portal.linkedPortals = new List<portalDataClass>();
                    string sql = "select p.id,p.ccguid,p.name from ccPortals p left join ccPortalLinks l on l.toPortalId=p.id where l.fromPortalId=" + portal.id;
                    if (csMan.OpenSQL(sql))
                    {
                        do
                        {
                            portalDataClass linkedPortal = new portalDataClass();
                            linkedPortal.id = csMan.GetInteger("id");
                            //linkedPortal.guid = csMan.GetText( "ccguid");
                            //if (string.IsNullOrEmpty(linkedPortal.guid))
                            //{
                            //    linkedPortal.guid = CP.Utils.CreateGuid();
                            //    CP.Db.ExecuteSQL("update ccPortals set ccguid='"+linkedPortal.guid+"' where id=" + csMan.GetInteger("id"));
                            //}
                            linkedPortal.name = csMan.GetText("name");
                            portal.linkedPortals.Add( linkedPortal);
                            csMan.GoNext();
                        } while (csMan.OK());
                    }
                    csMan.Close();
                    //
                    // load features 
                    //
                    if (!csFeature.Open("portal features", "portalid=" + portal.id, "sortOrder", true ))
                    {
                        //
                        // no features found, load default portal features
                        //
                        csFeature.Close();
                        if (string.IsNullOrEmpty(defaultConfigJson))
                        {
                            //
                            // no default, fake a tab
                            //
                            portal.featureList = new Dictionary<string, portalFeatureDataClass>();
                            portalFeatureDataClass feature = new portalFeatureDataClass();
                            feature.addonId = 0;
                            feature.dataContentId = 0;
                            feature.guid = "";
                            feature.heading = "Sample";
                            feature.id = 0;
                            feature.name = "Demo";
                            feature.parentFeatureId = 0;
                            feature.sortOrder = "";
                            feature.addPadding = false;
                        }
                        else
                        {
                            //
                            // load default and save to Db
                            //
                            System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer();
                            //string configJson = msJson.Serialize(portal);
                            portal = msJson.Deserialize<portalDataClass>(defaultConfigJson);
                            savePortalToDb(CP, portal);
                            //
                            if (!string.IsNullOrEmpty(portal.defaultFeature.guid))
                            {
                                if (csFeature.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(portal.defaultFeature.guid)))
                                {
                                    portal.defaultFeature = loadPortalFeatureFromCs(CP, csFeature);
                                }
                                csFeature.Close();
                            }
                        }
                    }
                    else
                    {
                        //
                        // load features from Db
                        //
                        CPCSBaseClass cs2 = CP.CSNew();
                        do
                        {
                            portalFeatureDataClass feature = loadPortalFeatureFromCs(CP, csFeature);
                            portal.featureList.Add(csFeature.GetText("ccguid"), feature);
                            if (portal.defaultFeature == null)
                            {
                                portal.defaultFeature = feature;
                            }
                            if (portalDefaultFeatureId == feature.id)
                            {
                                portal.defaultFeature = feature;
                            }
                            csFeature.GoNext();
                        } while (csFeature.OK());
                        csFeature.Close();
                    }
                }
            }
            catch (Exception ex) 
            {
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
        public portalFeatureDataClass loadPortalFeatureFromCs(CPBaseClass CP, CPCSBaseClass csFeature)
        {
            portalFeatureDataClass feature = new portalFeatureDataClass();
            try
            {
                CPCSBaseClass cs2 = CP.CSNew();
                //
                feature.id = csFeature.GetInteger("id");
                feature.name = csFeature.GetText("name");
                feature.heading = csFeature.GetText("heading");
                feature.sortOrder = csFeature.GetText("sortOrder");
                feature.addPadding = csFeature.GetBoolean("addPadding");
                if (string.IsNullOrEmpty(feature.heading))
                {
                    feature.heading = feature.name;
                }
                feature.guid = csFeature.GetText("ccguid");
                if (string.IsNullOrEmpty(feature.guid))
                {
                    feature.guid = CP.Utils.CreateGuid();
                    csFeature.SetField("ccguid", feature.guid);
                }
                //
                feature.addonId = csFeature.GetInteger("addonId");
                if (feature.addonId != 0)
                {
                    if (cs2.Open("add-ons", "id=" + feature.addonId))
                    {
                        feature.addonGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.addonGuid))
                        {
                            feature.addonGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguid", feature.addonGuid);
                        }
                    }
                    cs2.Close();
                }
                //
                feature.dataContentId = csFeature.GetInteger("dataContentId");
                if (feature.dataContentId != 0)
                {
                    if (cs2.Open("content", "id=" + feature.dataContentId))
                    {
                        feature.dataContentGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.dataContentGuid))
                        {
                            feature.dataContentGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguild", feature.dataContentGuid);
                        }
                    }
                    cs2.Close();
                }
                //
                feature.parentFeatureId = csFeature.GetInteger("parentFeatureId");
                if (feature.parentFeatureId != 0)
                {
                    if (cs2.Open("portal features", "id=" + feature.parentFeatureId))
                    {
                        feature.parentFeatureGuid = cs2.GetText("ccguid");
                        if (string.IsNullOrEmpty(feature.parentFeatureGuid))
                        {
                            feature.parentFeatureGuid = CP.Utils.CreateGuid();
                            cs2.SetField("ccguid", feature.parentFeatureGuid);
                        }
                    }
                    cs2.Close();
                }
                //
            }
            catch (Exception ex) 
            {
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
        public void savePortalToDb(CPBaseClass CP, portalDataClass newPortal)
        {
            try
            {
                CPCSBaseClass cs = CP.CSNew();
                CPCSBaseClass cs2 = CP.CSNew();
                //
                // insert or update the portal record
                //
                if (!cs.Open("portals", "ccguid=" + CP.Db.EncodeSQLText(newPortal.guid)))
                {
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
                foreach (KeyValuePair<string, portalFeatureDataClass> kvp in newPortal.featureList)
                {
                    portalFeatureDataClass feature = kvp.Value;
                    if (feature.guid != devToolGuid)
                    {
                        if (!cs.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(feature.guid))) 
                        {
                            cs.Insert("portal features");
                            cs.SetField("ccGuid", feature.guid);
                        }
                        if (cs.OK())
                        {
                            feature.id = cs.GetInteger("id");
                            cs.SetField("portalId", newPortal.id.ToString());
                            cs.SetField("name", feature.name);
                            cs.SetField("heading", feature.heading);
                            cs.SetField("sortOrder", feature.sortOrder);
                            if (!string.IsNullOrEmpty(feature.addonGuid))
                            {
                                //
                                // lookup addon by guid, set addonid
                                //
                                if (cs2.Open("add-ons", "ccguid=" + CP.Db.EncodeSQLText(feature.addonGuid )))
                                {
                                    cs.SetField("addonId", cs2.GetInteger("id").ToString() );
                                }
                                cs2.Close();
                            }
                            if (!string.IsNullOrEmpty(feature.dataContentGuid))
                            {
                                //
                                // save dataContentId based on dataContentGuid
                                //
                                if (cs2.Open("content", "ccguid=" + CP.Db.EncodeSQLText(feature.dataContentGuid)))
                                {
                                    feature.dataContentId = cs2.GetInteger("id");
                                    cs.SetField("dataContentId", feature.dataContentId.ToString()  );
                                }
                                cs2.Close();
                            }
                            if (newPortal.defaultFeature.guid == feature.guid) 
                            {
                                newPortal.defaultFeature = feature;
                            }
                        }
                        cs.Close();
                    }
                }
                //
                // lookup parent features by guid and set id
                //

                foreach (KeyValuePair<string, portalFeatureDataClass> kvp in newPortal.featureList)
                {
                    portalFeatureDataClass feature = kvp.Value;
                    if (feature.guid != devToolGuid)
                    {
                        if (!string.IsNullOrEmpty(feature.parentFeatureGuid)) 
                        {
                            //
                            // get the id of the parentFeature
                            //
                            if (cs.Open("portal features", "ccguid=" + CP.Db.EncodeSQLText(feature.parentFeatureGuid )))
                            {
                                feature.parentFeatureId = cs.GetInteger("id");
                            }
                            cs.Close();
                            if (feature.parentFeatureId > 0) 
                            {
                                //
                                // set the parentFeatureId field of the current feature
                                //
                                if (cs.Open("portal features", "id=" + feature.id.ToString() ))
                                {
                                    cs.SetField("parentFeatureId", feature.parentFeatureId.ToString());
                                }
                                cs.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
            }
        }
        //====================================================================================================
        /// <summary>
        /// get a portal feature available to developers that provides tool for creating portals
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        string getDevTool(CPBaseClass CP, portalDataClass portal, string frameRqs ) 
        {
            string body = "Hello World";
            try
            {
                string section;
                //
                // this is a feature list, display the feature list
                //
                formSimpleClass content = new formSimpleClass();
                content.title = "Developer Tool";
                content.body = "";
                //
                // process snapshot tool
                //
                if(CP.Doc.GetText("button")=="Take Snapshot")
                {
                    CPCSBaseClass cs = CP.CSNew();
                    if (cs.Open("portals", "ccguid=" + CP.Db.EncodeSQLText(portal.guid)))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer() ;
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
                section = CP.Html.Form(section,"","","",frameRqs);
                content.body += section;
                //
                //
                //
                body = content.getHtml(CP);
            }
            catch (Exception ex)
            {
                CP.Site.ErrorReport(ex, "exception in loadPortal");
            }
            return body;
        }
        //====================================================================================================
        /// <summary>
        /// create a feature list box
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        string getFeatureList(CPBaseClass CP, portalDataClass portal,  portalFeatureDataClass feature, string frameRqs )
        {
            string returnBody = "";
            string items = "";
            string qs;
            try
            {
                string activeNavHeading;
                activeNavHeading = feature.heading;
                formSimpleClass content = new formSimpleClass();
                foreach (KeyValuePair<string, portalFeatureDataClass> kvp in portal.featureList)
                {
                    portalFeatureDataClass liFeature = kvp.Value;
                    if ((liFeature.parentFeatureId == feature.id) && (liFeature.parentFeatureId != 0))
                    {
                        string featureHeading = liFeature.heading;
                        if (string.IsNullOrEmpty(featureHeading))
                        {
                            featureHeading = liFeature.name;
                        }
                        if (liFeature.dataContentId != 0)
                        {
                            qs = frameRqs;
                            qs = CP.Utils.ModifyQueryString(qs, "addonid", "", false);
                            qs = CP.Utils.ModifyQueryString(qs, rnDstFeatureGuid, "", false);
                            qs = CP.Utils.ModifyQueryString(qs, "cid", liFeature.dataContentId.ToString());
                            items += "<li><a target=\"_blank\" href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                        }
                        else
                        {
                            qs = frameRqs;
                            qs = CP.Utils.ModifyQueryString(qs, rnDstFeatureGuid, liFeature.guid);
                            items += "<li><a href=\"?" + qs + "\">" + featureHeading + "</a></li>";
                        }
                    }
                }
                content.title = feature.heading;
                content.body = "<ul class=\"afwButtonList\">" + items + "</ul>";
                returnBody = content.getHtml(CP);
            }
            catch (Exception ex)
            {
                CP.Site.ErrorReport(ex, "adminFramework.portalClass.getFeatureList exception");
            }
            return returnBody;
        }
    }
	//
	// admin framework portals
	//
	public class portalDataClass
	{
        public string name;
        public string guid;
        public int id;
        //public string defaultFeatureGuid;
		public Dictionary<string, portalFeatureDataClass> featureList;
		public portalFeatureDataClass defaultFeature;
        public List<portalDataClass> linkedPortals;
	}
	//
	public class portalFeatureDataClass
	{
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