using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class FormNameValueRowsClass {
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
        // ====================================================================================================
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        //
        //-------------------------------------------------
        // get
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            string result = "";
            string rowName;
            string rowValue;
            //
            // add user errors
            //
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (!string.IsNullOrEmpty(userErrors)) {
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
            for (int rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                //
                // check for fieldSetOpens
                //
                for (int fieldSetPtrx = 0; fieldSetPtrx <= fieldSetMax; fieldSetPtrx++) {
                    if (fieldSets[fieldSetPtrx].rowOpen == rowPtr) {
                        result += Constants.cr + "<fieldset class=\"afwFieldSet\">";
                        if (fieldSets[fieldSetPtrx].caption != "") {
                            result += Constants.cr + "<legend>" + fieldSets[fieldSetPtrx].caption + "</legend>";
                        }
                    }
                }
                //
                // -- name value row
                string nameValueRow = "";
                rowName = (string.IsNullOrWhiteSpace(rows[rowPtr].name) ? "&nbsp;" : rows[rowPtr].name);
                nameValueRow += cp.Html.div(rowName, "", "afwFormRowName", "");
                rowValue = (string.IsNullOrWhiteSpace(rows[rowPtr].value) ? "&nbsp;" : rows[rowPtr].value);
                nameValueRow += cp.Html.div(rowValue, "", "afwFormRowValue", "");
                result += cp.Html.div(nameValueRow, "", "afwFormRow", rows[rowPtr].htmlId);
                //
                // -- help row
                if (!string.IsNullOrEmpty(rows[rowPtr].help)) {
                    string helpRow = cp.Html.div("", "", "afwFormRowName", "");
                    rowValue = "<small class=\"text-muted afwFormRowValuehelp\">" + rows[rowPtr].help + "</small>";
                    helpRow += cp.Html.div(rowValue, "", "afwFormRowHelp", "");
                    result += cp.Html.div(helpRow, "", "afwFormRow", rows[rowPtr].htmlId);
                }
                //
                // check for fieldSetCloses
                //
                for (int fieldSetPtrx = fieldSetMax; fieldSetPtrx >= 0; fieldSetPtrx--) {
                    if (fieldSets[fieldSetPtrx].rowClose == rowPtr) {
                        result += Constants.cr + "</fieldset>";
                    }
                }
            }
            //
            // headers
            //
            if (localDescription != "") {
                result = Constants.cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + result;
            }
            if (localWarning != "") {
                result = Constants.cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + result;
            }
            if (localTitle != "") {
                result = Constants.cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + result;
            }
            //
            // add form
            //
            if (localIncludeForm) {
                if (localButtonList != "") {
                    localButtonList = ""
                        + Constants.cr + "<div class=\"afwButtonCon\">"
                        + indent(localButtonList)
                        + Constants.cr + "</div>";
                }
                result = Constants.cr + cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
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
                    + Constants.cr + "<div id=\"afw\">"
                    + indent(result)
                    + Constants.cr + "</div>";
            }
            //
            // -- set the optional title of the portal subnav
            cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle);
            return result;
        }
        //
        //-------------------------------------------------
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string Name, string Value) {
            localHiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
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
            localButtonList += Constants.cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
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
            return src.Replace(Constants.cr, Constants.cr2);
        }
    }
}
