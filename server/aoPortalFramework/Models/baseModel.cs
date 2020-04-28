

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoPortal.Models
{
    public abstract class baseModel
    {
        //
        //====================================================================================================
        //-- const must be set in derived clases
        //
        //Public Const contentName As String = "" '<------ set content name
        //Public Const contentTableName As String = "" '<------ set to tablename for the primary content (used for cache names)
        //Public Const contentDataSource As String = "" '<----- set to datasource if not default
        //
        //====================================================================================================
        // -- instance properties
        public int id { get; set; }
        public string name { get; set; }
        public string ccguid { get; set; }
        public bool Active { get; set; }
        public int ContentControlID { get; set; }
        public int CreatedBy { get; set; }
        public int CreateKey { get; set; }
        public System.DateTime DateAdded { get; set; }
        public int ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public string SortOrder { get; set; }
        //
        //====================================================================================================
        private static string derivedContentName(Type derivedType)
        {
            FieldInfo fieldInfo = derivedType.GetField("contentName");
            if ((fieldInfo == null))
            {
                throw new ApplicationException("Class [" + derivedType.Name + "] must declare constant [contentName].");
            }
            else
            {
                return fieldInfo.GetRawConstantValue().ToString();
            }
        }
        //
        //====================================================================================================
        private static string derivedContentTableName(Type derivedType)
        {
            FieldInfo fieldInfo = derivedType.GetField("contentTableName");
            if ((fieldInfo == null))
            {
                throw new ApplicationException("Class [" + derivedType.Name + "] must declare constant [contentTableName].");
            }
            else
            {
                return fieldInfo.GetRawConstantValue().ToString();
            }
        }
        //
        //====================================================================================================
        private static string contentDataSource(Type derivedType)
        {
            FieldInfo fieldInfo = derivedType.GetField("contentTableName");
            if ((fieldInfo == null))
            {
                throw new ApplicationException("Class [" + derivedType.Name + "] must declare constant [contentTableName].");
            }
            else
            {
                return fieldInfo.GetRawConstantValue().ToString();
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Create an empty object. needed for deserialization
        /// </summary>
        public baseModel()
        {
            //
        }
        //
        //====================================================================================================
        /// <summary>
        /// Add a new recod to the db and open it. Starting a new model with this method will use the default values in Contensive metadata (active, contentcontrolid, etc).
        /// include callersCacheNameList to get a list of cacheNames used to assemble this response
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        protected static T @add<T>(CPBaseClass cp) where T : baseModel
        {
            T result = null;
            try
            {
                Type instanceType = typeof(T);
                string contentName = derivedContentName(instanceType);
                result = create<T>(cp, cp.Content.AddRecord(contentName));
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// return a new model with the data selected. All cacheNames related to the object will be added to the cacheNameList.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId">The id of the record to be read into the new object</param>
        protected static T create<T>(CPBaseClass cp, int recordId) where T : baseModel
        {
            T result = null;
            try
            {
                if (recordId > 0)
                {
                    Type instanceType = typeof(T);
                    string contentName = derivedContentName(instanceType);
                    CPCSBaseClass cs = cp.CSNew();
                    if (cs.Open(contentName, "(id=" + recordId.ToString() + ")", "", true, "", 9999, 1))
                    {
                        result = loadRecord<T>(cp, cs);
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordGuid"></param>
        protected static T create<T>(CPBaseClass cp, string recordGuid) where T : baseModel
        {
            T result = null;
            try
            {
                Type instanceType = typeof(T);
                string contentName = derivedContentName(instanceType);
                CPCSBaseClass cs = cp.CSNew();
                if (cs.Open(contentName, "(ccGuid=" + cp.Db.EncodeSQLText(recordGuid) + ")", "", true, "", 9999, 1))
                {
                    result = loadRecord<T>(cp, cs);
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordName"></param>
        protected static T createByName<T>(CPBaseClass cp, string recordName) where T : baseModel
        {
            T result = null;
            try
            {
                if (!string.IsNullOrEmpty(recordName))
                {
                    Type instanceType = typeof(T);
                    string contentName = derivedContentName(instanceType);
                    CPCSBaseClass cs = cp.CSNew();
                    if (cs.Open(contentName, "(name=" + cp.Db.EncodeSQLText(recordName) + ")", "id", true, "", 9999, 1))
                    {
                        result = loadRecord<T>(cp, cs);
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// open an existing object
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="cs"></param>
        private static T loadRecord<T>(CPBaseClass cp, CPCSBaseClass cs) where T : baseModel
        {
            T instance = null;
            try
            {
                if (cs.OK())
                {
                    Type instanceType = typeof(T);
                    string tableName = derivedContentTableName(instanceType);
                    instance = (T)Activator.CreateInstance(instanceType);
                    foreach (PropertyInfo resultProperty in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        switch (resultProperty.Name.ToLower())
                        {
                            case "specialcasefield":
                                break;
                            case "sortorder":
                                //
                                // -- customization for pc, could have been in default property, db default, etc.
                                string sortOrder = cs.GetText(resultProperty.Name);
                                if ((string.IsNullOrEmpty(sortOrder)))
                                {
                                    sortOrder = "9999";
                                }
                                resultProperty.SetValue(instance, sortOrder, null);
                                break;
                            default:
                                switch (resultProperty.PropertyType.Name)
                                {
                                    case "Int32":
                                        resultProperty.SetValue(instance, cs.GetInteger(resultProperty.Name), null);
                                        break;
                                    case "Boolean":
                                        resultProperty.SetValue(instance, cs.GetBoolean(resultProperty.Name), null);
                                        break;
                                    case "DateTime":
                                        resultProperty.SetValue(instance, cs.GetDate(resultProperty.Name), null);
                                        break;
                                    case "Double":
                                        resultProperty.SetValue(instance, cs.GetNumber(resultProperty.Name), null);
                                        break;
                                    default:
                                        resultProperty.SetValue(instance, cs.GetText(resultProperty.Name), null);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return instance;
        }
        //
        //====================================================================================================
        /// <summary>
        /// save the instance properties to a record with matching id. If id is not provided, a new record is created.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        protected int save(CPBaseClass cp)
        {
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                Type instanceType = this.GetType();
                string contentName = derivedContentName(instanceType);
                string tableName = derivedContentTableName(instanceType);
                if ((id > 0))
                {
                    if (!cs.Open(contentName, "id=" + id, "", true, "", 9999, 1))
                    {
                        string message = "Unable to open record in content [" + contentName + "], with id [" + id + "]";
                        cs.Close();
                        id = 0;
                        throw new ApplicationException(message);
                    }
                }
                else
                {
                    if (!cs.Insert(contentName))
                    {
                        cs.Close();
                        id = 0;
                        throw new ApplicationException("Unable to insert record in content [" + contentName + "]");
                    }
                }
                foreach (PropertyInfo resultProperty in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    switch (resultProperty.Name.ToLower())
                    {
                        case "id":
                            id = cs.GetInteger("id");
                            break;
                        case "ccguid":
                            if ((string.IsNullOrEmpty(ccguid)))
                            {
                                ccguid = Guid.NewGuid().ToString();
                            }
                            string value = null;
                            value = resultProperty.GetValue(this, null).ToString();
                            cs.SetField(resultProperty.Name, value);
                            break;
                        default:
                            switch (resultProperty.PropertyType.Name)
                            {
                                case "Int32":
                                    int integerValue = 0;
                                    int.TryParse(resultProperty.GetValue(this, null).ToString(), out integerValue);
                                    cs.SetField(resultProperty.Name, integerValue.ToString());
                                    break;
                                case "Boolean":
                                    bool booleanValue = false;
                                    bool.TryParse(resultProperty.GetValue(this, null).ToString(), out booleanValue);
                                    cs.SetField(resultProperty.Name, booleanValue.ToString());
                                    break;
                                case "DateTime":
                                    System.DateTime dateValue = default(System.DateTime);
                                    System.DateTime.TryParse(resultProperty.GetValue(this, null).ToString(), out dateValue);
                                    cs.SetField(resultProperty.Name, dateValue.ToString());
                                    break;
                                case "Double":
                                    double doubleValue = 0;
                                    double.TryParse(resultProperty.GetValue(this, null).ToString(), out doubleValue);
                                    cs.SetField(resultProperty.Name, doubleValue.ToString());
                                    break;
                                default:
                                    string stringValue = resultProperty.GetValue(this, null).ToString();
                                    cs.SetField(resultProperty.Name, stringValue);
                                    break;
                            }
                            break;
                    }
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return id;
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record by id
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>
        protected static void delete<T>(CPBaseClass cp, int recordId) where T : baseModel
        {
            try
            {
                if ((recordId > 0))
                {
                    Type instanceType = typeof(T);
                    string contentName = derivedContentName(instanceType);
                    string tableName = derivedContentTableName(instanceType);
                    cp.Content.Delete(contentName, "id=" + recordId.ToString());
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// delete an existing database record by guid
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ccguid"></param>
        protected static void delete<T>(CPBaseClass cp, string ccguid) where T : baseModel
        {
            try
            {
                if ((!string.IsNullOrEmpty(ccguid)))
                {
                    Type instanceType = typeof(T);
                    string contentName = derivedContentName(instanceType);
                    baseModel instance = create<baseModel>(cp, ccguid);
                    if ((instance != null))
                    {
                        cp.Content.Delete(contentName, "(ccguid=" + cp.Db.EncodeSQLText(ccguid) + ")");
                    }
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// pattern get a list of objects from this model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="sqlCriteria"></param>
        /// <returns></returns>
        protected static List<T> createList<T>(CPBaseClass cp, string sqlCriteria, string sqlOrderBy) where T : baseModel
        {
            List<T> result = new List<T>();
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                List<string> ignoreCacheNames = new List<string>();
                Type instanceType = typeof(T);
                string contentName = derivedContentName(instanceType);
                if ((cs.Open(contentName, sqlCriteria, sqlOrderBy, true, "", 9999, 1)))
                {
                    T instance = default(T);
                    do
                    {
                        instance = loadRecord<T>(cp, cs);
                        if ((instance != null))
                        {
                            result.Add(instance);
                        }
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the name of the record by it's id
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="recordId"></param>record
        /// <returns></returns>
        protected static string getRecordName<T>(CPBaseClass cp, int recordId) where T : baseModel
        {
            try
            {
                if ((recordId > 0))
                {
                    Type instanceType = typeof(T);
                    string tableName = derivedContentTableName(instanceType);
                    CPCSBaseClass cs = cp.CSNew();
                    if ((cs.OpenSQL("select name from " + tableName + " where id=" + recordId.ToString())))
                    {
                        return cs.GetText("name");
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the name of the record by it's guid 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ccGuid"></param>record
        /// <returns></returns>
        protected static string getRecordName<T>(CPBaseClass cp, string ccGuid) where T : baseModel
        {
            try
            {
                if ((!string.IsNullOrEmpty(ccGuid)))
                {
                    Type instanceType = typeof(T);
                    string tableName = derivedContentTableName(instanceType);
                    CPCSBaseClass cs = cp.CSNew();
                    if ((cs.OpenSQL("select name from " + tableName + " where ccguid=" + cp.Db.EncodeSQLText(ccGuid))))
                    {
                        return cs.GetText("name");
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the id of the record by it's guid 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ccGuid"></param>record
        /// <returns></returns>
        protected static int getRecordId<T>(CPBaseClass cp, string ccGuid) where T : baseModel
        {
            try
            {
                if ((!string.IsNullOrEmpty(ccGuid)))
                {
                    Type instanceType = typeof(T);
                    string tableName = derivedContentTableName(instanceType);
                    CPCSBaseClass cs = cp.CSNew();
                    if ((cs.OpenSQL("select id from " + tableName + " where ccguid=" + cp.Db.EncodeSQLText(ccGuid))))
                    {
                        return cs.GetInteger("id");
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return 0;
        }
        //
        //====================================================================================================
        protected static int getCount<T>(CPBaseClass cp, string sqlCriteria) where T : baseModel
        {
            int result = 0;
            try
            {
                Type instanceType = typeof(T);
                string tableName = derivedContentTableName(instanceType);
                CPCSBaseClass cs = cp.CSNew();
                string sql = "select count(id) as cnt from " + tableName;
                if ((!string.IsNullOrEmpty(sqlCriteria)))
                {
                    sql += " where " + sqlCriteria;
                }
                if ((cs.OpenSQL(sql)))
                {
                    result = cs.GetInteger("cnt");
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //====================================================================================================
        /// <summary>
        /// Temporary method to create a path for an uploaded. First, try the texrt value in the field. If it is empty, use this method to create the path,
        /// append the filename to the end and save it to the field, and save the file there. This path starts with the tablename and ends with a slash.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected string getUploadPath<T>(string fieldName) where T : baseModel
        {
            Type instanceType = typeof(T);
            string tableName = derivedContentTableName(instanceType);
            return tableName.ToLower() + "/" + fieldName.ToLower() + "/" + id.ToString().PadLeft(12, Convert.ToChar("0")) + "/";
        }
    }
}

