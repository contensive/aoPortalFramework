
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class managerClass : AddonBaseClass
	{
        //
        const string blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>";
        const string rnDstFeatureGuid = "dstFeatureGuid";
        const string rnFrameRqs = "frameRqs";
        const string accountManagerGuid = "{12528435-EDBF-4FBB-858F-3731447E24A3}";
        const string rnManagerId = "managerId";
        const string devToolGuid = "{13511AA1-3A58-4742-B98F-D92AF853989F}";
        //====================================================================================================
        /// <summary>
        /// Addon interface. Run addon from doc property ManagerGuid or ManagerId (form, querystring, doc.setProperty())
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp )
        {
            string managerGuid = cp.Doc.GetText("ManagerGuid");
            int managerId = cp.Doc.GetInteger("ManagerId");
            string returnHtml = "";
            if (!string.IsNullOrEmpty(managerGuid))
            {
                returnHtml = getHtml(cp, "ccguid=" + cp.Db.EncodeSQLText(managerGuid));
            }
            else if (managerId != 0)
            {
                returnHtml = getHtml(cp, "id=" + managerId.ToString());
            }
            else
            {
                returnHtml = getHtml(cp,"");
            }
            return returnHtml;
        }
        //====================================================================================================
        /// <summary>
        /// get the manager html provided the Sql select criteria for manager. Blank returns the first order by id.
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="managerSelectSqlCriteria"></param>
        /// <returns></returns>
        public string getHtml(CPBaseClass CP, string managerSelectSqlCriteria)
		{
			string returnHtml = "";
			//
			try {
				//Return "test 1"
				//Exit Function
				if (!CP.User.IsAdmin) {
					returnHtml = blockedMessage;
				} else {
					CPBlockBaseClass form = CP.BlockNew();
					string frameRqs = CP.Doc.RefreshQueryString;
					adminFramework.pageWithNavClass innerForm = new adminFramework.pageWithNavClass();
					string body = "";
					CPCSBaseClass cs = CP.CSNew();
					string dstFeatureGuid = CP.Doc.GetText(rnDstFeatureGuid);
					managerFeatureDataClass feature;
					int managerid = CP.Doc.GetInteger(rnManagerId);
                    string activeNavHeading = "";
                    string items = "";
                    string qs = "";
                    //
					// build manager
					//
                    managerDataClass manager = loadManagerFromDb(CP, managerSelectSqlCriteria);
					//
					// manager interface - add tabs
					//
					foreach (KeyValuePair<string, managerFeatureDataClass> kvp in manager.featureList) {
						feature = kvp.Value;
                        if (feature.parentFeatureId == 0)
                        {
                            innerForm.addNav();
                            innerForm.navCaption = feature.heading;
                            innerForm.navLink = "?" + CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
                        }
					}
					//
					//   execute feature, if it returns empty, display default feature
					//
					if (manager.featureList.ContainsKey(dstFeatureGuid)) {
						//
						// add feature guid to frameRqs so if the feature uses ajax, the featureGuid will be part of it
						// add feature guid to rqs so if an addon is used that does not support frameRqs it will work
						//
						feature = manager.featureList[dstFeatureGuid];
						frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
                        if (feature.guid == devToolGuid)
                        {
                            CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, devToolGuid);
                            body = getDevTool(CP, manager );
                            activeNavHeading = "Developer Tool";
                        }
                        else if (feature.addonId != 0)
                        {
                            //
                            // feature is an addon, execute it
                            //
                            CP.Doc.SetProperty(rnFrameRqs, frameRqs);
                            CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
                            body = CP.Utils.ExecuteAddon(feature.addonId.ToString());
                            if (feature.parentFeatureId == 0)
                            {
                                activeNavHeading = feature.heading;
                            }
                            else
                            {
                                foreach (KeyValuePair<string, managerFeatureDataClass> kvp in manager.featureList)
                                {
                                    managerFeatureDataClass parentFeature = kvp.Value;
                                    if (parentFeature.id == feature.parentFeatureId)
                                    {
                                        activeNavHeading = parentFeature.heading;
                                    }
                                }
                            }
                        }
                        else if (feature.dataContentId != 0)
                        {
                            //
                            // this is a data content feature -- should not be here, link should have taken them to the content
                            //
                            CP.Response.Redirect("?cid=" + feature.dataContentId.ToString() );
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
                            activeNavHeading = feature.heading;
                            formSimpleClass content = new formSimpleClass();
                            foreach ( KeyValuePair< string, managerFeatureDataClass> kvp in manager.featureList ) {
                                managerFeatureDataClass liFeature = kvp.Value;
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
                                        qs = CP.Utils.ModifyQueryString(qs, rnDstFeatureGuid, "",false );
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
                            body = content.getHtml(CP);
                        }
					}
					if (string.IsNullOrEmpty(body)) {
						//
						// if the feature turns blank, run the default feature
						//
						feature = manager.defaultFeature;
                        activeNavHeading = feature.heading;
						frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
						CP.Doc.SetProperty(rnFrameRqs, frameRqs);
						CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
						body = CP.Utils.ExecuteAddon(feature.addonId.ToString() );
					}
                    innerForm.setActiveNav(activeNavHeading);
					//
					//Assemble
					//
					innerForm.body = CP.Html.div(body, "" ,"" , "afwBodyFrame");
					innerForm.title = manager.name;
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
				CP.Site.ErrorReport(ex, "error in Contensive.Addons.aoAccountBilling.adminClass.execute");
			}
			return returnHtml;
		}
        //====================================================================================================
        /// <summary>
        /// Return a manager object read from the Db based on the manager guid argument
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="managerRecordGuid"></param>
        /// <returns></returns>
        public managerDataClass loadManagerFromDb(CPBaseClass CP, string selectSqlCriteria )
        {
            managerDataClass manager = new managerDataClass();
            manager.featureList = new Dictionary<string, managerFeatureDataClass>();
            try
            {
                //managerFeatureDataClass firstFeature = null;
                CPCSBaseClass csMan = CP.CSNew();
                CPCSBaseClass csFeature = CP.CSNew();
                string defaultConfigJson;
                if (!csMan.Open("managers", selectSqlCriteria,"id"))
                {
                    //
                    // create demo manager
                    //
                    csMan.Close();
                    manager.name = "Empty";
                    manager.guid = "";
                    manager.id = 0;
                    manager.featureList = new Dictionary<string, managerFeatureDataClass>();
                    managerFeatureDataClass feature = new managerFeatureDataClass();
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
                    // load manager
                    //
                    manager.name = csMan.GetText("name");
                    manager.guid = csMan.GetText("ccguid");
                    manager.id = csMan.GetInteger("id");
                    defaultConfigJson = csMan.GetText("defaultConfigJson");
                    csMan.Close();
                    //
                    // load features 
                    //
                    if (!csFeature.Open("manager features", "managerid=" + manager.id, "sortOrder", true ))
                    {
                        //
                        // no features found, load default manager features
                        //
                        csFeature.Close();
                        if (string.IsNullOrEmpty(defaultConfigJson))
                        {
                            //
                            // no default, fake a tab
                            //
                            manager.featureList = new Dictionary<string, managerFeatureDataClass>();
                            managerFeatureDataClass feature = new managerFeatureDataClass();
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
                            // load default and save to Db
                            //
                            System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer();
                            //string configJson = msJson.Serialize(manager);
                            manager = msJson.Deserialize<managerDataClass>(defaultConfigJson);
                            saveManagerToDb(CP, manager);
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
                            managerFeatureDataClass feature = new managerFeatureDataClass();
                            feature.id = csFeature.GetInteger("id");
                            feature.name = csFeature.GetText("name");
                            feature.heading = csFeature.GetText("heading");
                            feature.sortOrder = csFeature.GetText("sortOrder");
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
                                        cs2.SetField("ccguild", feature.addonGuid);
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
                                if (cs2.Open("manager features", "id=" + feature.parentFeatureId))
                                {
                                    feature.parentFeatureGuid = cs2.GetText("ccguid");
                                    if (string.IsNullOrEmpty(feature.parentFeatureGuid))
                                    {
                                        feature.parentFeatureGuid = CP.Utils.CreateGuid();
                                        cs2.SetField("ccguild", feature.parentFeatureGuid);
                                    }
                                }
                                cs2.Close();
                            }
                            //
                            manager.featureList.Add(feature.guid, feature);
                            if (manager.defaultFeature == null)
                            {
                                manager.defaultFeature = feature;
                            }
                            csFeature.GoNext();
                        } while (csFeature.OK());
                        csFeature.Close();
                    }
                }
                //
                // add developer tab
                //
                if (CP.User.IsDeveloper)
                {
                    managerFeatureDataClass feature = new managerFeatureDataClass();
                    feature.id = 0;
                    feature.name = "Develeper Tool";
                    feature.heading = "Developer Tool";
                    feature.guid = devToolGuid;
                    feature.addonId = 0;
                    feature.addonGuid = "";
                    feature.parentFeatureId = 0;
                    feature.parentFeatureGuid = "";
                    manager.featureList.Add(feature.guid, feature);
                    if (manager.defaultFeature == null)
                    {
                        manager.defaultFeature = feature;
                    }
                }
            }
            catch (Exception ex) 
            {
                CP.Site.ErrorReport(ex, "exception in loadManager");
            }
            return manager;
        }
        //====================================================================================================
        /// <summary>
        /// If the manager does not exist in the Db, create it and all its features based on the manager argument
        /// </summary>
        /// <param name="CP"></param>
        /// <param name="newManager"></param>
        public void saveManagerToDb(CPBaseClass CP, managerDataClass newManager)
        {
            try
            {
                CPCSBaseClass cs = CP.CSNew();
                CPCSBaseClass cs2 = CP.CSNew();
                //
                // insert or update the manager record
                //
                if (!cs.Open("managers", "ccguid=" + CP.Db.EncodeSQLText(newManager.guid)))
                {
                    cs.Close();
                    cs.Insert("managers");
                    newManager.id = cs.GetInteger("id");

                }
                newManager.id = cs.GetInteger("id");
                cs.SetField("ccguid", newManager.guid);
                cs.SetField("name", newManager.name);
                cs.Close();
                //
                // insert or update manager feature records
                //
                foreach (KeyValuePair<string, managerFeatureDataClass> kvp in newManager.featureList)
                {
                    managerFeatureDataClass feature = kvp.Value;
                    if (feature.guid != devToolGuid)
                    {
                        if (!cs.Open("manager features", "ccguid=" + CP.Db.EncodeSQLText(feature.guid))) 
                        {
                            cs.Insert("manager features");
                            cs.SetField("ccGuid", feature.guid);
                        }
                        if (cs.OK())
                        {
                            feature.id = cs.GetInteger("id");
                            cs.SetField("managerId", newManager.id.ToString());
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
                            if (newManager.defaultFeature.guid == feature.guid) 
                            {
                                newManager.defaultFeature = feature;
                            }
                        }
                        cs.Close();
                    }
                }
                //
                // lookup parent features by guid and set id
                //

                foreach (KeyValuePair<string, managerFeatureDataClass> kvp in newManager.featureList)
                {
                    managerFeatureDataClass feature = kvp.Value;
                    if (feature.guid != devToolGuid)
                    {
                        if (!string.IsNullOrEmpty(feature.parentFeatureGuid)) 
                        {
                            //
                            // get the id of the parentFeature
                            //
                            if (cs.Open("manager features", "ccguid=" + CP.Db.EncodeSQLText(feature.parentFeatureGuid )))
                            {
                                feature.parentFeatureId = cs.GetInteger("id");
                            }
                            cs.Close();
                            if (feature.parentFeatureId > 0) 
                            {
                                //
                                // set the parentFeatureId field of the current feature
                                //
                                if (cs.Open("manager features", "id=" + feature.id.ToString() ))
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
                CP.Site.ErrorReport(ex, "exception in loadManager");
            }
        }
        //====================================================================================================
        /// <summary>
        /// get a manager feature available to developers that provides tool for creating managers
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        string getDevTool(CPBaseClass CP, managerDataClass manager ) 
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
                    if (cs.Open("managers", "ccguid=" + CP.Db.EncodeSQLText(manager.guid)))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer msJson = new System.Web.Script.Serialization.JavaScriptSerializer() ;
                        string configJson = msJson.Serialize(manager);
                        cs.SetField("defaultConfigJson", configJson);
                    }
                    cs.Close();
                    
                }
                //
                // output snapshot tool
                //
                section = "<h3>Manager Snapshot</h3>";
                section += "<p>Click the snapshot button to save the current features for this manager in the manager's default configuration field.</p>";
                section += CP.Html.Button("button", "Take Snapshot");
                section += "<p>Modify Manager and Manager Features data directly</p>";
                section += "<ul>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Managers") + "\">Managers</a></li>";
                section += "<li><a href=\"?cid=" + CP.Content.GetID("Manager Features") + "\">Manager Features</a></li>";
                section += "</ul>";
                section = CP.Html.Form(section);
                content.body += section;
                //
                //
                //
                body = content.getHtml(CP);
            }
            catch (Exception ex)
            {
                CP.Site.ErrorReport(ex, "exception in loadManager");
            }
            return body;
        }
    }
	//
	// admin framework managers
	//
	public class managerDataClass
	{
        public string name;
        public string guid;
        public int id;
		public Dictionary<string, managerFeatureDataClass> featureList;
		public managerFeatureDataClass defaultFeature;
	}
	//
	public class managerFeatureDataClass
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
    }
}