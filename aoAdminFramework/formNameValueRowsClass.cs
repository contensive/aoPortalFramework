using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class formNameValueRowsClass
    {
        //
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        const int fieldSetSize = 999;
        int fieldSetMax = -1;
        int fieldSetPtr = -1;
        struct fieldSetStruct
        {
            public string caption;
            public int rowOpen;
            public int rowClose;
        }
        fieldSetStruct[] fieldSets = new fieldSetStruct[fieldSetSize];
        Stack fieldSetPtrStack = new Stack();


        //
        const int rowSize = 999;
        int rowCnt = -1;
        struct rowStruct
        {
            public string name;
            public string value;
            public string htmlId;
        }
        rowStruct[] rows = new rowStruct[rowSize];

        //
        string localBody = "";
        string localDescription = "";
        string localTitle = "";
        string localRqs = "";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localIncludeForm = false;
        //
        //-------------------------------------------------
        // Title
        //-------------------------------------------------
        //
        public string title
        {
            get
            {
                return localTitle;
            }
            set
            {
                localTitle = value;
            }
        }
        //
        //-------------------------------------------------
        // Description
        //-------------------------------------------------
        //
        public string description
        {
            get
            {
                return localDescription;
            }
            set
            {
                localDescription = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string styleSheet
        {
            get
            {
                return adminFramework.Resource1.styles.ToString();
            }
        }
        //
        //-------------------------------------------------
        // open a FieldSet
        //-------------------------------------------------
        //
        public void openFieldSet(string caption)
        {
            fieldSetPtrStack.Push(fieldSetPtr);
            if (fieldSetMax < fieldSetSize)
            {
                fieldSetMax += 1;
            }
            fieldSetPtr = fieldSetMax;
            fieldSets[fieldSetPtr].caption = caption;
            fieldSets[fieldSetPtr].rowOpen = rowCnt+1;
        }
        //
        //-------------------------------------------------
        // close a FieldSet
        //-------------------------------------------------
        //
        public void closeFieldSet()
        {
            if (fieldSetPtr >= 0)
            {
                fieldSets[fieldSetPtr].rowClose = rowCnt;
            }
            if (fieldSetPtrStack.Count > 0)
            {
                fieldSetPtr = (int)fieldSetPtrStack.Pop();
            }
        }
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp)
        {
            string s = "";
            int rowPtr = 0;
            int fieldSetPtr = 0;
            string row = "";
            string rowName;
            string rowValue;
            //
            //
            //
            if (localBody != "")
            {
                s += localBody;
                /*
                body += ""
                    + cr + "<div class=\"afwBodyColor\">"
                    + indent(localBody)
                    + cr + "</div>";
                */
            }
            for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++)
            {
                //
                // check for fieldSetOpens
                //
                for (fieldSetPtr = 0; fieldSetPtr <= fieldSetMax; fieldSetPtr++)
                {
                    if (fieldSets[fieldSetPtr].rowOpen == rowPtr)
                    {
                        s += cr + "<fieldset class=\"afwFieldSet\">";
                        if (fieldSets[fieldSetPtr].caption != "")
                        {
                            s += cr + "<legend>" + fieldSets[fieldSetPtr].caption + "</legend>";
                        }
                    }
                }
                row = "";
                rowName = rows[rowPtr].name;
                if (rowName == "") rowName = "&nbsp;";
                row += cr + cp.Html.div(rowName, "", "afwFormRowName", "");
                rowValue = rows[rowPtr].value;
                if (rowValue == "") rowValue = "&nbsp;";
                row += cr + cp.Html.div(rowValue, "", "afwFormRowValue", "");
                s += cr + cp.Html.div(row, "", "afwFormRow", rows[rowPtr].htmlId);
                //
                // check for fieldSetCloses
                //
                for (fieldSetPtr = fieldSetMax; fieldSetPtr >= 0; fieldSetPtr--)
                {
                    if (fieldSets[fieldSetPtr].rowClose == rowPtr)
                    {
                        s += cr + "</fieldset>";
                    }
                }
            }
            //
            // headers
            //
            if (localDescription != "")
            {
                s = cr + "<p class=\"afwDescription\">" + localDescription + "</p>" + s;
            }
            if (localTitle != "")
            {
                s = cr + "<h2 class=\"afwTitle\">" + localTitle + "</h2>" + s;
            }
            //
            // add form
            //
            if (localIncludeForm)
            {
                if (localButtonList != "")
                {
                    localButtonList = ""
                        + cr + "<div class=\"afwButtonCon\">" 
                        + indent(localButtonList)
                        + cr + "</div>";
                }
                s = cr + cp.Html.Form(localButtonList + s + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            //
            // body padding and color
            //
            s = cp.Html.div(s, "",  "afwBodyPad", "");
            s = cp.Html.div(s, "", "afwBodyColor", "");
            return s;
        }
        //
        //-------------------------------------------------
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string Name, string Value)
        {
            localHiddenList += cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // add a form button
        //-------------------------------------------------
        //
        public void addFormButton(string buttonValue)
        {
            addFormButton(buttonValue, "button", "");
        }
        public void addFormButton(string buttonValue, string buttonName)
        {
            addFormButton(buttonValue, buttonName, "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId)
        {
            localButtonList += cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // setForm
        //-------------------------------------------------
        //
        public string formAction
        {
            get
            {
                return localFormActionQueryString;
            }
            set
            {
                localFormActionQueryString = value;
                localIncludeForm = true;
            }
        }
        public string formId
        {
            get
            {
                return localFormId;
            }
            set
            {
                localFormId = value;
                localIncludeForm = true;
            }
        }
        //
        //-------------------------------------------------
        // Refresh Query String
        //-------------------------------------------------
        //
        public string refreshQueryString
        {
            get
            {
                return localRqs;
            }
            set
            {
                localRqs = value;
            }
        }
        //
        //-------------------------------------------------
        // body
        //-------------------------------------------------
        //
        public string body
        {
            get
            {
                return localBody;
            }
            set
            {
                localBody = value;
            }
        }
        //
        //-------------------------------------------------
        // add a row
        //-------------------------------------------------
        //
        public void addRow()
        {
            if (rowCnt < rowSize)
            {
                rowCnt += 1;
                rows[rowCnt].name = "";
                rows[rowCnt].value = "";
                rows[rowCnt].htmlId = "";
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowHtmlId
        {
            get
            {
                checkRowCnt();
                return rows[rowCnt].htmlId;
            }
            set
            {
                checkRowCnt();
                rows[rowCnt].htmlId = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowName
        {
            get
            {
                checkRowCnt();
                return rows[rowCnt].name;
            }
            set
            {
                checkRowCnt();
                rows[rowCnt].name = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowValue
        {
            get
            {
                checkRowCnt();
                return rows[rowCnt].value;
            }
            set
            {
                checkRowCnt();
                rows[rowCnt].value = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkRowCnt()
        {
            if (rowCnt < 0)
            {
                addRow();
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private string indent(string src)
        {
            return src.Replace(cr, cr2);
        }
    }
}
