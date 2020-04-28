
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoPortal {
    public class ReportListClass {
        const int columnSize = 99;
        const int rowSize = 19999;
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        struct ColumnStruct {
            public string name;
            public string caption;
            public string captionClass;
            public string cellClass;
            public bool sortable;
            public bool visible;
            public bool downloadable;
        }
        bool ReportTooLong = false;
        readonly ColumnStruct[] columns = new ColumnStruct[columnSize];
        bool formIncludeHeader = false;
        //
        int columnMax = -1;
        int columnPtr = -1;
        //
        int rowCnt = -1;
        //
        string localGuid = "";
        string localName = "";
        string localTitle = "";
        string localWarning = "";
        string localDescription = "";
        string localFrameRqs = "";
        bool localFrameRqsSet = false;
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localFormActionQueryStringSet = false;
        //string localPreForm = "";
        string localHtmlBeforeTable = "";
        string localHtmlAfterTable = "";
        string localHtmlLeftOfTable = "";
        bool localIncludeForm = false;
        bool localIsEmptyReport = true;

        //
        readonly string[,] localReportCells = new string[rowSize, columnSize];
        readonly string[,] localDownloadData = new string[rowSize, columnSize];
        readonly string[] localRowClasses = new string[rowSize];
        readonly bool[] localExcludeRowFromDownload = new bool[rowSize];
        public bool localIsOuterContainer = false;
        //
        //-------------------------------------------------
        /// <summary>
        /// The maximum number of rows allowed
        /// </summary>
        public int reportRowLimit {
            get {
                return rowSize;
            }
        }
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
        // getResult
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            StringBuilder s = new StringBuilder("");
            string row = "";
            StringBuilder rowList = new StringBuilder("");
            int rowPtr = 0;
            int colPtr = 0;
            int colPtrDownload = 0;
            string styleClass;
            string content;
            string userErrors;
            string sortLink;
            string columnSort = cp.Doc.GetText("columnSort");
            string sortField = "";
            string csvDownloadContent = "";
            DateTime rightNow = DateTime.Now;
            //
            // initialize - setup Db and/or read Db values
            //
            reportDbInit(cp);
            //
            if (!localFrameRqsSet) {
                refreshQueryString = cp.Doc.RefreshQueryString;
            }
            if (!localFormActionQueryStringSet) {
                // set locals not public property bc public also sets the includeForm
                localFormActionQueryString = localFrameRqs;
                localFormActionQueryStringSet = true;
                //formActionQueryString = localFrameRqs;
            }
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }
            //
            // headers
            //
            if (formIncludeHeader) {
                rowList = new StringBuilder("");
                for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                    if (columns[colPtr].visible) {
                        styleClass = columns[colPtr].captionClass;
                        if (styleClass != "") {
                            styleClass = " class=\"" + styleClass + "\"";
                        }
                        content = columns[colPtr].caption;
                        sortField = columns[colPtr].name;
                        if (content == "") {
                            content = "&nbsp;";
                        } else if (columns[colPtr].sortable) {
                            if (localFrameRqsSet) {
                                sortLink = "?" + localFrameRqs + "&columnSort=" + sortField;
                            } else {
                                sortLink = "?" + cp.Doc.RefreshQueryString + "&columnSort=" + sortField;
                            }
                            if (columnSort == sortField) {
                                sortLink += "Desc";
                            }
                            content = "<a href=\"" + sortLink + "\">" + content + "</a>";
                        }
                        rowList.Append(cr + "<th" + styleClass + ">" + content + "</th>");
                    }
                }
                s.Append(""
                    + cr + "<thead>"
                    + cr2 + "<tr>"
                    + indent(indent(rowList.ToString()))
                    + cr2 + "</tr>"
                    + cr + "</thead>"
                    + "");
                if (addCsvDownloadCurrentPage) {
                    colPtrDownload = 0;
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        if (columns[colPtr].downloadable) {
                            if (colPtrDownload == 0) {
                                csvDownloadContent += "\"" + columns[colPtr].caption.Replace("\"", "\"\"") + "\"";
                            } else {
                                csvDownloadContent += ",\"" + columns[colPtr].caption.Replace("\"", "\"\"") + "\"";
                            }
                            colPtrDownload += 1;
                        }
                    }
                }
            }
            //
            // body
            //
            rowList = new StringBuilder("");
            if (localIsEmptyReport) {
                styleClass = columns[0].cellClass;
                if (styleClass != "") {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                row = cr + "<td style=\"text-align:left\" " + styleClass + " colspan=\"" + (columnMax + 1) + "\">[empty]</td>";
                rowList.Append(""
                    + cr + "<tr>"
                    + indent(row)
                    + cr + "</tr>");
            } else if (ReportTooLong) {
                // -- report is too long
                styleClass = columns[0].cellClass;
                if (styleClass != "") {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                row = cr + "<td style=\"text-align:left\" " + styleClass + " colspan=\"" + (columnMax + 1) + "\">There are too many rows in this report. Please consider filtering the data.</td>";
                rowList.Append(""
                    + cr + "<tr>"
                    + indent(row)
                    + cr + "</tr>");
            } else {
                // -- display th report
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                    row = "";
                    colPtrDownload = 0;
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        styleClass = columns[colPtr].cellClass;
                        if (columns[colPtr].visible) {
                            if (styleClass != "") {
                                styleClass = " class=\"" + styleClass + "\"";
                            }
                            row += cr + "<td" + styleClass + ">" + localReportCells[rowPtr, colPtr] + "</td>";
                        }
                        if (addCsvDownloadCurrentPage && !localExcludeRowFromDownload[rowPtr]) {
                            if (columns[colPtr].downloadable) {
                                if (colPtrDownload == 0) {
                                    csvDownloadContent += Environment.NewLine;
                                } else {
                                    csvDownloadContent += ",";
                                }
                                if (!string.IsNullOrEmpty(localDownloadData[rowPtr, colPtr])) {
                                    csvDownloadContent += "\"" + localDownloadData[rowPtr, colPtr].Replace("\"", "\"\"") + "\"";
                                }
                                colPtrDownload += 1;
                            }

                        }
                    }
                    styleClass = localRowClasses[rowPtr];
                    if (rowPtr % 2 != 0) {
                        styleClass += " afwOdd";
                    }
                    if (styleClass != "") {
                        styleClass = " class=\"" + styleClass + "\"";
                    }
                    rowList.Append(""
                        + cr + "<tr" + styleClass + ">"
                        + indent(row)
                        + cr + "</tr>");
                }
            }
            string csvDownloadFilename = string.Empty;
            if (addCsvDownloadCurrentPage) {
                //
                // todo implement cp.db.CreateCsv()
                // 5.1 -- download
                CPCSBaseClass csDownloads = cp.CSNew();
                if (csDownloads.Insert("downloads")) {
                    csvDownloadFilename = csDownloads.GetFilename("filename", "export.csv");
                    cp.CdnFiles.Save(csvDownloadFilename, csvDownloadContent);
                    csDownloads.SetField("name", "Download for [" + localTitle + "], requested by [" + cp.User.Name + "]");
                    csDownloads.SetField("requestedBy", cp.User.Id.ToString());
                    csDownloads.SetField("filename", csvDownloadFilename);
                    csDownloads.SetField("dateRequested", DateTime.Now.ToString());
                    csDownloads.SetField("datecompleted", DateTime.Now.ToString());
                    csDownloads.SetField("resultmessage", "Completed");
                    csDownloads.Save();
                }
                csDownloads.Close();
            }
            s.Append(""
                + cr + "<tbody>"
                + indent(rowList.ToString())
                + cr + "</tbody>"
                + "");
            s = new StringBuilder(""
                + cr + "<table class=\"afwListReportTable\">"
                + indent(s.ToString())
                + cr + "</table>");
            if (localHtmlLeftOfTable != "") {
                s = new StringBuilder(""
                        + cr + "<div class=\"afwLeftSideHtml\">"
                        + indent(localHtmlLeftOfTable)
                        + cr + "</div>"
                        + cr + "<div class=\"afwRightSideHtml\">"
                        + indent(s.ToString())
                        + cr + "</div>"
                        + cr + "<div style=\"clear:both\"></div>"
                        + "");
            }
            if (localHtmlBeforeTable != "") {
                s = new StringBuilder(""
                    + localHtmlBeforeTable
                    + s.ToString()
                    + "");
            }
            if (localHtmlAfterTable != "") {
                s.Append(localHtmlAfterTable);
            }
            //
            // headers
            //
            if (addCsvDownloadCurrentPage && (!string.IsNullOrEmpty(csvDownloadFilename))) {
                s = new StringBuilder(cr + "<p id=\"afwDescription\"><a href=\"" + cp.Site.FilePath + csvDownloadFilename + "\">Click here</a> to download the data.</p>" + s.ToString());
            }
            if (localDescription != "") {
                s = new StringBuilder(cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + s.ToString());
            }
            if (localWarning != "") {
                s = new StringBuilder(cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + s.ToString());
            }
            if (localTitle != "") {
                s = new StringBuilder(cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + s.ToString());
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
                s = new StringBuilder(cr + cp.Html.Form(localButtonList + s.ToString() + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, ""));
                //body = ""
                //    + cr + cp.Html.Form( localButtonList + body + localHiddenList )
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            if (_includeBodyPadding) { s = new StringBuilder(cp.Html.div(s.ToString(), "", "afwBodyPad", "")); };
            if (_includeBodyColor) { s = new StringBuilder(cp.Html.div(s.ToString(), "", "afwBodyColor", "")); };
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer) {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                s = new StringBuilder(""
                    + cr + "<div id=\"afw\">"
                    + indent(s.ToString())
                    + cr + "</div>");
            }
            return s.ToString();
        }
        //
        //-------------------------------------------------
        // add html blocks
        //-------------------------------------------------
        //
        public string htmlLeftOfTable {
            get {
                return localHtmlLeftOfTable;
            }
            set {
                localHtmlLeftOfTable = value;
            }
        }
        //
        public string htmlBeforeTable {
            get {
                return localHtmlBeforeTable;
            }
            set {
                localHtmlBeforeTable = value;
            }
        }
        //
        public string htmlAfterTable {
            get {
                return localHtmlAfterTable;
            }
            set {
                localHtmlAfterTable = value;
            }
        }
        //
        //-------------------------------------------------
        /// <summary>
        /// add a form hidden
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
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
        //  Action for the form. If not set it uses the Refresh Query String
        //-------------------------------------------------
        //
        /// <summary>
        /// Set the same as the refresh query string, except exclude all form inputs within the form. For instance, if the form has a filter that include 'dateTo', add dateTo to the RQS so heading sorts retain the value, but do not add it to formAction because the input box in the filter already has that value.
        /// </summary>
        public string formActionQueryString {
            get {
                return localFormActionQueryString;
            }
            set {
                localFormActionQueryString = value;
                localFormActionQueryStringSet = true;
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
        // Refresh Query String. if not set, the cp.doc.refreshQuerystring is used
        //-------------------------------------------------
        //
        /// <summary>
        /// Include all nameValue pairs required to refresh the page if someone clicks on a header. For example, if there is a filter dateTo that is not empty, add dateTo=1/1/2000 to the RQS
        /// </summary>
        public string refreshQueryString {
            get {
                return localFrameRqs;
            }
            set {
                localFrameRqs = value;
                localFrameRqsSet = true;
            }
        }
        ////
        ////-------------------------------------------------
        //// Guid
        ////-------------------------------------------------
        ////
        public string guid {
            get {
                return localGuid;
            }
            set {
                localGuid = value;
            }
        }
        //
        //-------------------------------------------------
        // Name
        //-------------------------------------------------
        //
        public string name {
            get {
                return localName;
            }
            set {
                localName = value;
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
        // column properties
        //-------------------------------------------------
        //
        public string columnName {
            get {
                checkColumnPtr();
                return columns[columnPtr].name;
            }
            set {
                if (value != "") {
                    checkColumnPtr();
                    //formIncludeHeader = true;
                    columns[columnPtr].name = value;
                }
            }
        }
        //
        //-------------------------------------------------
        // column properties
        //-------------------------------------------------
        //
        public string columnCaption {
            get {
                checkColumnPtr();
                return columns[columnPtr].caption;
            }
            set {
                if (value != "") {
                    checkColumnPtr();
                    formIncludeHeader = true;
                    columns[columnPtr].caption = value;
                }
            }
        }
        //
        //-------------------------------------------------
        // set the caption class for this column
        //-------------------------------------------------
        //
        public string columnCaptionClass {
            get {
                checkColumnPtr();
                return columns[columnPtr].captionClass;
            }
            set {
                if (value != "") {
                    formIncludeHeader = true;
                    checkColumnPtr();
                    columns[columnPtr].captionClass = value;
                }
            }
        }
        //
        //-------------------------------------------------
        // set the cell class for this column
        //-------------------------------------------------
        //
        public string columnCellClass {
            get {
                checkColumnPtr();
                return columns[columnPtr].cellClass;
            }
            set {
                checkColumnPtr();
                columns[columnPtr].cellClass = value;
            }
        }
        //
        //-------------------------------------------------
        // set the column sortable
        //  this creates as a link on the caption
        //-------------------------------------------------
        //
        public bool columnSortable {
            get {
                checkColumnPtr();
                return columns[columnPtr].sortable;
            }
            set {
                checkColumnPtr();
                columns[columnPtr].sortable = value;
            }
        }
        //
        //-------------------------------------------------
        // set the column visible
        //  displays in report
        //-------------------------------------------------
        //
        public bool columnVisible {
            get {
                checkColumnPtr();
                return columns[columnPtr].visible;
            }
            set {
                checkColumnPtr();
                columns[columnPtr].visible = value;
            }
        }
        //
        //-------------------------------------------------
        // set the column downloadable
        //  hides from export
        //-------------------------------------------------
        //
        public bool columnDownloadable {
            get {
                checkColumnPtr();
                return columns[columnPtr].downloadable;
            }
            set {
                checkColumnPtr();
                columns[columnPtr].downloadable = value;
            }
        }
        //
        //-------------------------------------------------
        // add a column
        //-------------------------------------------------
        //
        public void addColumn() {
            if (columnPtr < columnSize) {
                columnPtr += 1;
                columns[columnPtr].name = "";
                columns[columnPtr].caption = "";
                columns[columnPtr].captionClass = "";
                columns[columnPtr].cellClass = "";
                columns[columnPtr].sortable = false;
                columns[columnPtr].visible = true;
                columns[columnPtr].downloadable = true;
                if (columnPtr > columnMax) {
                    columnMax = columnPtr;
                }
            }
        }
        //
        //-------------------------------------------------
        // add a row
        //-------------------------------------------------
        //
        public void addRow() {
            localIsEmptyReport = false;
            if (rowCnt < (rowSize + 1)) {
                rowCnt += 1;
                localExcludeRowFromDownload[rowCnt] = false;
                localRowClasses[rowCnt] = "";
            } else {
                ReportTooLong = true;
            }
            checkColumnPtr();
            columnPtr = 0;
        }
        //
        //-------------------------------------------------
        // mark this row to exclude from data download
        //-------------------------------------------------
        //
        public bool excludeRowFromDownload {
            get {
                checkColumnPtr();
                checkRowCnt();
                return localExcludeRowFromDownload[rowCnt];
            }
            set {
                checkColumnPtr();
                checkRowCnt();
                localExcludeRowFromDownload[rowCnt] = value;
            }
        }


        //
        //-------------------------------------------------
        // add a row class
        //-------------------------------------------------
        //
        public void addRowClass(string styleClass) {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            localRowClasses[rowCnt] += " " + styleClass;
        }
        //
        //-------------------------------------------------
        // populate a cell
        //-------------------------------------------------
        //
        public void setCell(string content) {
            setCell(content, content);
        }
        public void setCell(string reportContent, string downloadContent) {
            if (!ReportTooLong) {
                localIsEmptyReport = false;
                checkColumnPtr();
                checkRowCnt();
                localReportCells[rowCnt, columnPtr] = reportContent;
                localDownloadData[rowCnt, columnPtr] = downloadContent;
            }
            if (columnPtr < columnMax) {
                columnPtr += 1;
            }
        }
        //
        public void setCell(int content) => setCell(content.ToString(), content.ToString());
        public void setCell(int content, int downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        //
        public void setCell(double content) => setCell(content.ToString(), content.ToString());
        public void setCell(double content, double downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        //
        public void setCell(bool content) => setCell(content.ToString(), content.ToString());
        public void setCell(bool content, bool downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        //
        public void setCell(DateTime content) => setCell(content.ToString(), content.ToString());
        public void setCell(DateTime content, DateTime downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private string indent(string src) {
            return src.Replace(cr, cr2);
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkColumnPtr() {
            if (columnPtr < 0) {
                addColumn();
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
        // create csv download as form is build
        //-------------------------------------------------
        //
        public bool addCsvDownloadCurrentPage { get; set; } = false;
        //
        //-------------------------------------------------
        // initialize
        //  read the report and column settings from the Db
        //  if no localGuid set, sync to Db does not work
        //  if the report does not exist in teh Db, use the input values
        //-------------------------------------------------
        //
        public ReportListClass(CPBaseClass cp) {
        }
        private void reportDbInit(CPBaseClass cp) {
            try {
                int colPtr;
                string colName;
                string colCaption;
                string colSortOrder;
                string colCaptionClass;
                string colCellClass;
                bool colSortable = false;
                bool colVisible = true;
                bool colDownloadable = true;
                string textVisible = "";
                //string textDownloadable = "";
                CPCSBaseClass cs = cp.CSNew();
                int reportId;
                string sqlCriteria;
                //
                if (localGuid != "") {
                    sqlCriteria = "ccguid=" + cp.Db.EncodeSQLText(localGuid);
                    if (!cs.Open("Admin Framework Reports", sqlCriteria, "", true, "", 9999, 1)) {
                        cs.Close();
                        if (localName == "") {
                            localName = localTitle;
                        }
                        cs.Insert("Admin Framework reports");
                        cs.SetField("ccguid", localGuid);
                        cs.SetField("name", localName);
                        cs.SetField("title", localTitle);
                        //cs.SetField("description", localDescription);
                    }
                    reportId = cs.GetInteger("id");
                    localName = cs.GetText("name");
                    localTitle = cs.GetText("title");
                    //localDescription = cs.GetText("description");
                    // tmp solution for reports created with a name and no title
                    if ((localTitle == "") && (localName != "")) {
                        localTitle = localName;
                    }
                    cs.Close();
                    //
                    //
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        colCaption = columns[colPtr].caption;
                        colName = columns[colPtr].name;
                        colSortOrder = (colPtr * 10).ToString();
                        colSortOrder = colSortOrder.PadLeft(4 - colSortOrder.Length, '0');
                        colCaptionClass = columns[colPtr].captionClass;
                        colCellClass = columns[colPtr].cellClass;
                        colSortable = columns[colPtr].sortable; // not part of Db config
                        colVisible = columns[colPtr].visible;
                        colDownloadable = columns[colPtr].downloadable;
                        if (colName == "") {
                            colName = colCaption;
                        }
                        if ((colName != "") && (reportId != 0)) {
                            if (!cs.Open("Admin Framework Report Columns", "(reportId=" + reportId.ToString() + ")and(name=" + cp.Db.EncodeSQLText(colName) + ")", "id", true, "", 9999, 1)) {
                                cs.Close();
                                cs.Insert("Admin Framework Report Columns");
                                cs.SetField("reportId", reportId.ToString());
                                cs.SetField("name", colName);
                                cs.SetField("caption", colCaption);
                                cs.SetField("sortOrder", colSortOrder);
                                cs.SetField("captionClass", colCaptionClass);
                                cs.SetField("cellClass", colCellClass);
                                cs.SetField("visible", colVisible.ToString());
                                // for another day
                                //cs.SetField("downloadable", colDownloadable.ToString());
                            } else {
                                // tmp - if name but not caption, use the other
                                colCaption = cs.GetText("caption");
                                colName = cs.GetText("name");
                                if (colCaption == "") {
                                    colCaption = colName;
                                } else if (colName == "") {
                                    colName = colCaption;
                                }
                                columns[colPtr].name = colName;
                                columns[colPtr].caption = colCaption;
                                columns[colPtr].captionClass = cs.GetText("captionClass");
                                columns[colPtr].cellClass = cs.GetText("cellClass");
                                textVisible = cs.GetText("visible");
                                if (textVisible == "") {
                                    cs.SetField("visible", colVisible.ToString());
                                } else {
                                    columns[colPtr].visible = cp.Utils.EncodeBoolean(textVisible);
                                }
                                // for another day
                                //textDownloadable = cs.GetText("downloadable");
                                //if (textDownloadable == "")
                                //{
                                //    cs.SetField("downloadable", textDownloadable.ToString());
                                //}
                                //else
                                //{
                                //    columns[colPtr].visible = cp.Utils.EncodeBoolean(textDownloadable);
                                //}
                            }
                            cs.Close();
                        }
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Exception in reportListClass.init");
            }

        }

    }
}