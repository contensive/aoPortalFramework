using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoPortal {
    public class formNameValueRowsClass {
        //
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        const int fieldSetSize = 999;
        int fieldSetMax = -1;
        int fieldSetPtr = -1;
        struct fieldSetStruct {
            public string caption;
            public int rowOpen;
            public int rowClose;
        }
        fieldSetStruct[] fieldSets = new fieldSetStruct[fieldSetSize];
        Stack fieldSetPtrStack = new Stack();


        //
        const int rowSize = 999;
        int rowCnt = -1;
        struct rowStruct {
            public string name;
            public string value;
            public string help;
            public string htmlId;
        }
        rowStruct[] rows = new rowStruct[rowSize];

        //
        string localBody = "";
        string localDescription = "";
        string localWarning = "";
        string localTitle = "";
        //string localFrameRqs = "";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localIncludeForm = false;
        bool localIsOuterContainer = false;
        //
        //-------------------------------------------------
        //
        public bool includeBodyPadding {
            get {
                return _includeBodyPadding;
            }
            set {
                _includeBodyPadding = value;
            }
        }
        bool _includeBodyPadding = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyColor {
            get {
                return _includeBodyColor;
            }
            set {
                _includeBodyColor = value;
            }
        }
        bool _includeBodyColor = true;
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public bool isOuterContainer {
            get {
                return localIsOuterContainer;
            }
            set {
                localIsOuterContainer = value;
            }
        }
        //
        //-------------------------------------------------
        // Title
        //-------------------------------------------------
        //
        public string title {
            get {
                return localTitle;
            }
            set {
                localTitle = value;
            }
        }
        //
        //-------------------------------------------------
        // Warning
        //-------------------------------------------------
        //
        public string warning {
            get {
                return localWarning;
            }
            set {
                localWarning = value;
            }
        }
        //
        //-------------------------------------------------
        // Description
        //-------------------------------------------------
        //
        public string description {
            get {
                return localDescription;
            }
            set {
                localDescription = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string styleSheet {
            get {
                return Properties.Resources.styles;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string javascript {
            get {
                return Properties.Resources.javascript;
            }
        }
        //
        //-------------------------------------------------
        // open a FieldSet
        //-------------------------------------------------
        //
        public void openFieldSet(string caption) {
            fieldSetPtrStack.Push(fieldSetPtr);
            if (fieldSetMax < fieldSetSize) {
                fieldSetMax += 1;
            }
            fieldSetPtr = fieldSetMax;
            fieldSets[fieldSetPtr].caption = caption;
            fieldSets[fieldSetPtr].rowOpen = rowCnt + 1;
        }
        //
        //-------------------------------------------------
        // close a FieldSet
        //-------------------------------------------------
        //
        public void closeFieldSet() {
            if (fieldSetPtr >= 0) {
                fieldSets[fieldSetPtr].rowClose = rowCnt;
            }
            if (fieldSetPtrStack.Count > 0) {
                fieldSetPtr = (int)fieldSetPtrStack.Pop();
            }
        }
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            string result = "";
            int rowPtr = 0;
            int fieldSetPtr = 0;
            string row = "";
            string rowName;
            string rowValue;
            string userErrors;
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }
            //
            //
            //
            if (localBody != "") {
                result += localBody;
                /*
                body += ""
                    + cr + "<div class=\"afwBodyColor\">"
                    + indent(localBody)
                    + cr + "</div>";
                */
            }
            for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                //
                // check for fieldSetOpens
                //
                for (fieldSetPtr = 0; fieldSetPtr <= fieldSetMax; fieldSetPtr++) {
                    if (fieldSets[fieldSetPtr].rowOpen == rowPtr) {
                        result += cr + "<fieldset class=\"afwFieldSet\">";
                        if (fieldSets[fieldSetPtr].caption != "") {
                            result += cr + "<legend>" + fieldSets[fieldSetPtr].caption + "</legend>";
                        }
                    }
                }
                row = "";
                rowName = rows[rowPtr].name;
                if (rowName == "") rowName = "&nbsp;";
                row += cr + cp.Html.div(rowName, "", "afwFormRowName", "");
                rowValue = rows[rowPtr].value;
                if (rowValue == "") rowValue = "&nbsp;";
                if (!string.IsNullOrEmpty(rows[rowPtr].help)) rowValue += cr + cp.Html.div(rows[rowPtr].help, "", "afwFormRowValuehelp");
                row += cr + cp.Html.div(rowValue, "", "afwFormRowValue", "");
                result += cr + cp.Html.div(row, "", "afwFormRow", rows[rowPtr].htmlId);
                //
                // check for fieldSetCloses
                //
                for (fieldSetPtr = fieldSetMax; fieldSetPtr >= 0; fieldSetPtr--) {
                    if (fieldSets[fieldSetPtr].rowClose == rowPtr) {
                        result += cr + "</fieldset>";
                    }
                }
            }
            //
            // headers
            //
            if (localDescription != "") {
                result = cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + result;
            }
            if (localWarning != "") {
                result = cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + result;
            }
            if (localTitle != "") {
                result = cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + result;
            }
            //
            // add form
            //
            if (localIncludeForm) {
                if (localButtonList != "") {
                    localButtonList = ""
                        + cr + "<div class=\"afwButtonCon\">"
                        + indent(localButtonList)
                        + cr + "</div>";
                }
                result = cr + cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            //
            // body padding and color
            //
            if (_includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
            if (_includeBodyColor) { result = cp.Html.div(result, "", "afwBodyColor", ""); };
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer) {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                result = ""
                    + cr + "<div id=\"afw\">"
                    + indent(result)
                    + cr + "</div>";
            }
            return result;
        }
        //
        //-------------------------------------------------
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string Name, string Value) {
            localHiddenList += cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
            localIncludeForm = true;
        }
        //
        public void addFormHidden(string name, int value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, double value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, DateTime value) => addFormHidden(name, value.ToString());
        //
        public void addFormHidden(string name, bool value) => addFormHidden(name, value.ToString());
        //
        //-------------------------------------------------
        // add a form button
        //-------------------------------------------------
        //
        public void addFormButton(string buttonValue) {
            addFormButton(buttonValue, "button", "", "");
        }
        public void addFormButton(string buttonValue, string buttonName) {
            addFormButton(buttonValue, buttonName, "", "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId) {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
            localButtonList += cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // setForm
        //-------------------------------------------------
        //
        public string formAction {
            get {
                return localFormActionQueryString;
            }
            set {
                localFormActionQueryString = value;
                localIncludeForm = true;
            }
        }
        public string formId {
            get {
                return localFormId;
            }
            set {
                localFormId = value;
                localIncludeForm = true;
            }
        }
        //
        //-------------------------------------------------
        // Refresh Query String
        //-------------------------------------------------
        //
        //public string refreshQueryString
        //{
        //    get
        //    {
        //        return localFrameRqs;
        //    }
        //    set
        //    {
        //        localFrameRqs = value;
        //    }
        //}
        //
        //-------------------------------------------------
        // body
        //-------------------------------------------------
        //
        public string body {
            get {
                return localBody;
            }
            set {
                localBody = value;
            }
        }
        //
        //-------------------------------------------------
        // add a row
        //-------------------------------------------------
        //
        public void addRow() {
            if (rowCnt < rowSize) {
                rowCnt += 1;
                rows[rowCnt].name = "";
                rows[rowCnt].value = "";
                rows[rowCnt].help = "";
                rows[rowCnt].htmlId = "";
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowHtmlId {
            get {
                checkRowCnt();
                return rows[rowCnt].htmlId;
            }
            set {
                checkRowCnt();
                rows[rowCnt].htmlId = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowName {
            get {
                checkRowCnt();
                return rows[rowCnt].name;
            }
            set {
                checkRowCnt();
                rows[rowCnt].name = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string rowValue {
            get {
                checkRowCnt();
                return rows[rowCnt].value;
            }
            set {
                checkRowCnt();
                rows[rowCnt].value = value;
            }
        }
        //
        //-------------------------------------------------
        //
        public string rowHelp {
            get {
                checkRowCnt();
                return rows[rowCnt].help;
            }
            set {
                checkRowCnt();
                rows[rowCnt].help = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkRowCnt() {
            if (rowCnt < 0) {
                addRow();
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private string indent(string src) {
            return src.Replace(cr, cr2);
        }
    }
}
