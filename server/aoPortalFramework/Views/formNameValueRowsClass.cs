using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;
using System;
using System.Collections;

namespace Contensive.Addons.PortalFramework {
    public class FormNameValueRowsClass {
        //
        /// <summary>
        /// the maximum number of fields allowed
        /// </summary>
        const int fieldSetSize = 999;
        //
        /// <summary>
        /// the number of fields used in this form
        /// </summary>
        private int fieldSetMax = -1;
        //
        /// <summary>
        /// the current field being updated
        /// </summary>
        private int fieldSetPtr = -1;
        //
        /// <summary>
        /// the structure of data saved to each field
        /// </summary>
        struct fieldSetStruct {
            public string caption;
            public int rowOpen;
            public int rowClose;
        }
        //
        /// <summary>
        /// fieldsets are used to group fields visually with ah html fieldset element
        /// </summary>
        private readonly fieldSetStruct[] fieldSets = new fieldSetStruct[fieldSetSize];
        //
        /// <summary>
        /// fieldsets are used to group fields visually with ah html fieldset element
        /// </summary>
        private Stack fieldSetPtrStack = new Stack();
        //
        /// <summary>
        /// the max number of row
        /// </summary>
        const int rowSize = 999;
        //
        /// <summary>
        /// the current row
        /// </summary>
        private int rowCnt = -1;
        //
        /// <summary>
        /// the structure of stored rows
        /// </summary>
        struct rowStruct {
            public string name;
            public string value;
            public string help;
            public string htmlId;
        }
        //
        /// <summary>
        /// the stored rows to be rendered
        /// </summary>
        rowStruct[] rows = new rowStruct[rowSize];
        //
        private string localBody = "";
        private string localFormId = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        private bool includeForm { get; set; } = false;
        //
        // ====================================================================================================
        //
        public bool includeBodyPadding { get; set; }
        //
        // ====================================================================================================
        //
        public bool includeBodyColor { get; set; }
        //
        // ====================================================================================================
        //
        public bool isOuterContainer { get; set; }
        //
        // ====================================================================================================
        //
        public string title { get; set; }
        //
        // ====================================================================================================
        //
        public string warning { get; set; }
        //
        // ====================================================================================================
        //
        public string description { get; set; }
        //
        // ====================================================================================================
        //
        public string styleSheet {
            get {
                return Properties.Resources.styles;
            }
        }
        //
        // ====================================================================================================
        //
        public string javascript {
            get {
                return Properties.Resources.javascript;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// start a new html fieldset
        /// </summary>
        /// <param name="caption"></param>
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
        // ====================================================================================================
        /// <summary>
        /// close  fieldset. creates an html fieldset around the field elements
        /// </summary>
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
        // ====================================================================================================
        /// <summary>
        /// Render the stored structure to an html form
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public string getHtml(CPBaseClass cp) {
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            //
            string result = "";
            string rowName;
            string rowValue;
            //
            // -- add user errors
            string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (!string.IsNullOrEmpty(userErrors)) {
                warning = userErrors;
            }
            //
            // -- add body
            result += localBody;
            for (int rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                //
                // -- check for fieldSetOpens
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
            // -- construct report
            HtmlDocRequest request = new HtmlDocRequest() {
                body = result.ToString(),
                includeBodyPadding = includeBodyPadding,
                includeBodyColor = includeBodyColor,
                buttonList = buttonList,
                csvDownloadFilename = "",
                description = description,
                formActionQueryString = formActionQueryString,
                hiddenList = hiddenList,
                includeForm = includeForm,
                isOuterContainer = isOuterContainer,
                title = title,
                warning = warning
            };
            return HtmlController.getReportDoc(cp, request);
        }
        //
        //-------------------------------------------------
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string name, string value) {
            hiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + name + "\" value=\"" + value + "\">";
            includeForm = true;
        }
        private string hiddenList = "";
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
            buttonList += HtmlController.getButton(buttonName, buttonValue, buttonId, buttonClass);
            includeForm = true;
        }
        private string buttonList = "";
        //
        //-------------------------------------------------
        // setForm
        //-------------------------------------------------
        //
        public string formActionQueryString {
            get {
                return localFormActionQueryString;
            }
            set {
                localFormActionQueryString = value;
                includeForm = !string.IsNullOrEmpty(value);
            }
        }
        private string localFormActionQueryString = "";
        //
        //

        public string formId {
            get {
                return localFormId;
            }
            set {
                localFormId = value;
                includeForm = !string.IsNullOrEmpty(value);
            }
        }
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
