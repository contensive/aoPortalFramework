
//using Microsoft.VisualBasic;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Text;
//using Contensive.BaseClasses;

//namespace adminFramework.Models
//{
//    public class _blankModel : baseModel
//    {
//        //
//        //====================================================================================================
//        //-- const
//        //------ set content name
//        public const string contentName = "tables";
//        //------ set to tablename for the primary content (used for cache names)
//        public const string contentTableName = "ccTables";
//        //
//        //====================================================================================================
//        // -- instance properties
//        public int dataSourceID { get; set; }
//        //<------ replace this with a list all model fields not part of the base model
//        //
//        //====================================================================================================
//        public static _blankModel @add(CPBaseClass cp)
//        {
//            return @add<_blankModel>(cp);
//        }
//        //
//        //====================================================================================================
//        public static _blankModel create(CPBaseClass cp, int recordId)
//        {
//            return create<_blankModel>(cp, recordId);
//        }
//        //
//        //====================================================================================================
//        public static _blankModel create(CPBaseClass cp, string recordGuid)
//        {
//            return create<_blankModel>(cp, recordGuid);
//        }
//        //
//        //====================================================================================================
//        public static _blankModel createByName(CPBaseClass cp, string recordName)
//        {
//            return createByName<_blankModel>(cp, recordName);
//        }
//        ////
//        ////====================================================================================================
//        //public void save(CPBaseClass cp)
//        //{
//        //    base.save(cp);
//        //}
//        //
//        //====================================================================================================
//        public static void delete(CPBaseClass cp, int recordId)
//        {
//            delete<_blankModel>(cp, recordId);
//        }
//        //
//        //====================================================================================================
//        public static void delete(CPBaseClass cp, string ccGuid)
//        {
//            delete<_blankModel>(cp, ccGuid);
//        }
//        //
//        //====================================================================================================
//        public static List<_blankModel> createList(CPBaseClass cp, string sqlCriteria, string sqlOrderBy = "id")
//        {
//            return createList<_blankModel>(cp, sqlCriteria, sqlOrderBy);
//        }
//        //
//        //====================================================================================================
//        public static string getRecordName(CPBaseClass cp, int recordId)
//        {
//            return baseModel.getRecordName<_blankModel>(cp, recordId);
//        }
//        //
//        //====================================================================================================
//        public static string getRecordName(CPBaseClass cp, string ccGuid)
//        {
//            return baseModel.getRecordName<_blankModel>(cp, ccGuid);
//        }
//        //
//        //====================================================================================================
//        public static int getRecordId(CPBaseClass cp, string ccGuid)
//        {
//            return baseModel.getRecordId<_blankModel>(cp, ccGuid);
//        }

//        //
//        //====================================================================================================
//        //
//        public _blankModel clone(CPBaseClass cp)
//        {
//            _blankModel result = (_blankModel)this.clone();
//            result.id = cp.Content.AddRecord(contentName);
//            result.ccguid = cp.Utils.CreateGuid();
//            result.save(cp);
//            return result;
//        }
//        //
//        //====================================================================================================
//        //
//        public object clone()
//        {
//            return this.MemberwiseClone();
//        }
//    }
//}
