
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework.Models
{
    public class contentModel : baseModel
    {
        //
        //====================================================================================================
        //-- const
        //------ set content name
        public const string contentName = "content";
        //------ set to tablename for the primary content (used for cache names)
        public const string contentTableName = "ccContent";
        //
        //====================================================================================================
        // -- instance properties
        public bool AdminOnly { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowContentChildTool { get; set; }
        public bool AllowContentTracking { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowMetaContent { get; set; }
        public bool AllowTopicRules { get; set; }
        public bool AllowWorkflowAuthoring { get; set; }
        public int AuthoringTableID { get; set; }
        public int ContentCategoryID { get; set; }
        public int ContentTableID { get; set; }
        public int DefaultSortMethodID { get; set; }
        public bool DeveloperOnly { get; set; }
        public string DropDownFieldList { get; set; }
        public int EditorGroupID { get; set; }
        public int IconHeight { get; set; }
        public string IconLink { get; set; }
        public int IconSprites { get; set; }
        public int IconWidth { get; set; }
        public int InstalledByCollectionID { get; set; }
        public bool IsBaseContent { get; set; }
        public int ParentID { get; set; }
        //
        //====================================================================================================
        public static contentModel @add(CPBaseClass cp)
        {
            return @add<contentModel>(cp);
        }
        //
        //====================================================================================================
        public static contentModel create(CPBaseClass cp, int recordId)
        {
            return create<contentModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static contentModel create(CPBaseClass cp, string recordGuid)
        {
            return create<contentModel>(cp, recordGuid);
        }
        //
        //====================================================================================================
        public static contentModel createByName(CPBaseClass cp, string recordName)
        {
            return createByName<contentModel>(cp, recordName);
        }
        //
        //====================================================================================================
        public void save(CPBaseClass cp)
        {
            base.save(cp);
        }
        //
        //====================================================================================================
        public static void delete(CPBaseClass cp, int recordId)
        {
            delete<contentModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static void delete(CPBaseClass cp, string ccGuid)
        {
            delete<contentModel>(cp, ccGuid);
        }
        //
        //====================================================================================================
        public static List<contentModel> createList(CPBaseClass cp, string sqlCriteria, string sqlOrderBy = "id")
        {
            return createList<contentModel>(cp, sqlCriteria, sqlOrderBy);
        }
        //
        //====================================================================================================
        public static string getRecordName(CPBaseClass cp, int recordId)
        {
            return baseModel.getRecordName<contentModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static string getRecordName(CPBaseClass cp, string ccGuid)
        {
            return baseModel.getRecordName<contentModel>(cp, ccGuid);
        }
        //
        //====================================================================================================
        public static int getRecordId(CPBaseClass cp, string ccGuid)
        {
            return baseModel.getRecordId<contentModel>(cp, ccGuid);
        }

        //
        //====================================================================================================
        //
        public contentModel Clone(CPBaseClass cp)
        {
            contentModel result = (contentModel)this.Clone();
            result.id = cp.Content.AddRecord(contentName);
            result.ccguid = cp.Utils.CreateGuid();
            result.save(cp);
            return result;
        }
        //
        //====================================================================================================
        //
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
