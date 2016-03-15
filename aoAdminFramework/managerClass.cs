
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
	public class managerClass
	{
        //
        const string blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>";
        const string rnDstFeatureGuid = "dstFeatureGuid";
        const string rnFrameRqs = "frameRqs";
        const string accountManagerGuid = "{12528435-EDBF-4FBB-858F-3731447E24A3}";
        const string rnManagerId = "managerId";
        //
		public string getHtml(CPBaseClass CP, string managerRecordGuid)
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
					int defaultFeatureId;
					managerFeatureDataClass firstFeature = null;
					CPCSBaseClass csMan = CP.CSNew();
					int managerid = CP.Doc.GetInteger(rnManagerId);
                    //
					// build manager
					//
					managerDataClass manager = new managerDataClass();
					manager.featureList = new Dictionary<string, managerFeatureDataClass>();
                    if (!csMan.Open("managers", "ccguid=" + CP.Db.EncodeSQLText(managerRecordGuid)))
                    {
						csMan.Close();
						csMan.Insert("managers");
                        csMan.SetField("ccguid", managerRecordGuid);
                        csMan.SetField("name", "Sample Manager");
                    }
					manager.name = csMan.GetText("name");
					manager.id = csMan.GetInteger("id");
					defaultFeatureId = csMan.GetInteger("defaultFeatureId");
					csMan.Close();
					if (!csMan.Open("manager features", "managerid=" + manager.id, "sortOrder", true , "name,ccGuid,addonId")) {
						csMan.Insert("manager features");
						csMan.SetField("managerid", manager.id.ToString());
						csMan.SetField("name", "Sample Feature");
						csMan.Save();
					}
					do {
						feature = new managerFeatureDataClass();
						feature.name = csMan.GetText("name");
						feature.guid = csMan.GetText("ccguid");
						if (string.IsNullOrEmpty(feature.guid)) {
							feature.guid = CP.Utils.CreateGuid();
							csMan.SetField("ccguid", feature.guid);
						}
						feature.addonId = csMan.GetInteger("addonId");
						manager.featureList.Add(feature.guid, feature);
						if (firstFeature == null) {
							firstFeature = feature;
						}
						if (csMan.GetInteger("id") == defaultFeatureId) {
							manager.defaultFeature = feature;
						}
						csMan.GoNext();
					} while (csMan.OK());
					csMan.Close();
					//
					if (manager.defaultFeature == null) {
						manager.defaultFeature = firstFeature;
					}
					//
					// manager interface - add tabs
					//
					foreach (KeyValuePair<string, managerFeatureDataClass> kvp in manager.featureList) {
						feature = kvp.Value;
						innerForm.addNav();
						innerForm.navCaption = feature.name;
						innerForm.navLink = "?" + CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
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
						CP.Doc.SetProperty(rnFrameRqs, frameRqs);
						CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
						body = CP.Utils.ExecuteAddon( feature.addonId.ToString() );
					}
					if (string.IsNullOrEmpty(body)) {
						//
						// if the feature turns blank, run the default feature
						//
						feature = manager.defaultFeature;
						frameRqs = CP.Utils.ModifyQueryString(frameRqs, rnDstFeatureGuid, feature.guid);
						CP.Doc.SetProperty(rnFrameRqs, frameRqs);
						CP.Doc.AddRefreshQueryString(rnDstFeatureGuid, feature.guid);
						body = CP.Utils.ExecuteAddon(feature.addonId.ToString() );
					}
					innerForm.setActiveNav(feature.name);
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
					CP.Doc.AddHeadStyle(innerForm.styleSheet);
					CP.Doc.AddHeadJavascript("var afwFrameRqs='" + frameRqs + "';");
				}
			} catch (Exception ex) {
				CP.Site.ErrorReport(ex, "error in Contensive.Addons.aoAccountBilling.adminClass.execute");
			}
			return returnHtml;
		}
	}
	//
	// admin framework managers
	//
	public class managerDataClass
	{
		public string name;
		public int id;
		public Dictionary<string, managerFeatureDataClass> featureList;
		public managerFeatureDataClass defaultFeature;
	}
	//
	public class managerFeatureDataClass
	{
		public string name;
		public string guid;
		public int addonId;
	}
}