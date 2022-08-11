
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.Addons.PortalFramework.Models.Domain;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class ReportListClass {
        /// <summary>
        /// add an ellipse menu entry for the current row
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public void addRowEllipseMenuItem(string name, string url) {
            if (rowEllipseMenuDict == null) { rowEllipseMenuDict = new Dictionary<int, List<EllipseMenuItem>>(); }
            if (!rowEllipseMenuDict.ContainsKey(rowCnt)) { rowEllipseMenuDict[rowCnt] = new List<EllipseMenuItem>(); }
            rowEllipseMenuDict[rowCnt].Add(new EllipseMenuItem {
                name = name,
                url = url
            });
        }
        private Dictionary<int, List<EllipseMenuItem>> rowEllipseMenuDict { get; set; } = new Dictionary<int, List<EllipseMenuItem>>();
        /// <summary>
        /// legacy constructor
        /// </summary>
        /// <param name="cp"></param>
        [Obsolete("Use parameterless constructor", false)] public ReportListClass(CPBaseClass cp) { }
        /// <summary>
        /// default constructor
        /// </summary>
        public ReportListClass() { }
        //
        /// <summary>
        /// maximum columns allowed
        /// </summary>
        const int columnSize = 99;
        //
        /// <summary>
        /// maximum rows allowed
        /// </summary>
        const int rowSize = 19999;
        //
        /// <summary>
        /// todo deprecate - use ReportListColumnClass
        /// </summary>
        struct ColumnStruct {
            public string name;
            public string caption;
            public string captionClass;
            public string cellClass;
            public bool sortable;
            public bool visible;
            public bool downloadable;
            public int columnWidthPercent;
        }
        /// <summary>
        /// when true, the report has exceeded the rowSize and future columns will populate on top of each other
        /// </summary>
        private bool ReportTooLong = false;
        /// <summary>
        /// storage for the current column
        /// </summary>
        readonly ColumnStruct[] columns = new ColumnStruct[columnSize];
        //
        /// <summary>
        /// true if the caption or captionclass has been initialized
        /// </summary>
        private bool captionIncluded = false;
        //
        /// <summary>
        /// the highest column count of any row
        /// </summary>
        private int columnMax = -1;
        //
        /// <summary>
        /// pointer to the current column
        /// </summary>
        private int columnPtr = -1;
        //
        /// <summary>
        /// the number of rows in the report
        /// </summary>
        private int rowCnt = -1;
        //
        /// <summary>
        /// list of hidden form tags
        /// </summary>
        string localHiddenList = "";
        //
        /// <summary>
        /// list of buttons added to the form
        /// </summary>
        string localButtonList = "";
        //
        /// <summary>
        /// if true, the report grid has not been populated
        /// </summary>
        bool localIsEmptyReport = true;
        //
        /// <summary>
        /// The report grid data
        /// </summary>
        readonly string[,] localReportCells = new string[rowSize, columnSize];
        //
        /// <summary>
        /// the report download data
        /// </summary>
        readonly string[,] localDownloadData = new string[rowSize, columnSize];
        //
        /// <summary>
        /// the report row styles
        /// </summary>
        readonly string[] localRowClasses = new string[rowSize];
        //
        /// <summary>
        /// if true, exclude this row from download
        /// </summary>
        readonly bool[] localExcludeRowFromDownload = new bool[rowSize];
        //
        /// <summary>
        /// if true, this report is part of a larger layout and the internal styles and js should NOT be added
        /// </summary>
        public bool localIsOuterContainer = false;
        //
        // ====================================================================================================
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        //
        //====================================================================================================
        /// <summary>
        /// The maximum number of rows allowed
        /// </summary>
        public int reportRowLimit {
            get {
                return rowSize;
            }
        }
        //
        //====================================================================================================
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
        //====================================================================================================
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
        //====================================================================================================
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
        //====================================================================================================
        //
        public string styleSheet {
            get {
                return Properties.Resources.styles;
            }
        }
        //
        //====================================================================================================
        //
        public string javascript {
            get {
                return Properties.Resources.javascript;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// render the report
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public string getHtml(CPBaseClass cp) {
            int hint = 0;
            try {
                StringBuilder rowBuilder = new StringBuilder("");
                //string classAttribute;
                //string content;
                //string userErrors;
                string sortLink;
                string columnSort = cp.Doc.GetText("columnSort");
                string csvDownloadContent = "";
                DateTime rightNow = DateTime.Now;
                hint = 10;
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
                string userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
                if (!string.IsNullOrEmpty(userErrors)) {
                    warning = userErrors;
                }
                int colPtr;
                int colPtrDownload;
                StringBuilder result = new StringBuilder("");
                hint = 20;
                //
                // headers
                //
                if (captionIncluded) {
                    rowBuilder = new StringBuilder("");
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        if (columns[colPtr].visible) {
                            string classAttribute = columns[colPtr].captionClass;
                            if (classAttribute != "") {
                                classAttribute = " class=\"" + classAttribute + "\"";
                            }
                            string content = columns[colPtr].caption;
                            string sortField = columns[colPtr].name;
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
                            string styleAttribute = "";
                            if (columns[colPtr].columnWidthPercent > 0) {
                                styleAttribute = " style=\"width:" + columns[colPtr].columnWidthPercent.ToString() + "%;\"";
                            }
                            //row += Constants.cr + "<td" + classAttribute + styleAttribute + ">" + localReportCells[rowPtr, colPtr] + "</td>";
                            rowBuilder.Append(Constants.cr + "<th" + classAttribute + styleAttribute + ">" + content + "</th>");
                        }
                    }
                    result.Append(""
                        + Constants.cr + "<thead>"
                        + Constants.cr2 + "<tr>"
                        + indent(indent(rowBuilder.ToString()))
                        + Constants.cr2 + "</tr>"
                        + Constants.cr + "</thead>"
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
                hint = 30;
                //
                // body
                //
                rowBuilder = new StringBuilder("");
                if (localIsEmptyReport) {
                    hint = 40;
                    string classAttribute = columns[0].cellClass;
                    if (classAttribute != "") {
                        classAttribute = " class=\"" + classAttribute + "\"";
                    }
                    //row = Constants.cr + "<td style=\"text-align:left\" " + classAttribute + " colspan=\"" + (columnMax + 1) + "\">[empty]</td>";
                    rowBuilder.Append(""
                        + Constants.cr + "<tr>"
                        + Constants.cr + "<td style=\"text-align:left\" " + classAttribute + " colspan=\"" + (columnMax + 1) + "\">[empty]</td>"
                        + Constants.cr + "</tr>");
                } else if (ReportTooLong) {
                    // -- report is too long
                    string classAttribute = columns[0].cellClass;
                    if (classAttribute != "") {
                        classAttribute = " class=\"" + classAttribute + "\"";
                    }
                    //row = Constants.cr + "<td style=\"text-align:left\" " + classAttribute + " colspan=\"" + (columnMax + 1) + "\">There are too many rows in this report. Please consider filtering the data.</td>";
                    rowBuilder.Append(""
                        + Constants.cr + "<tr>"
                        + Constants.cr + "<td style=\"text-align:left\" " + classAttribute + " colspan=\"" + (columnMax + 1) + "\">There are too many rows in this report. Please consider filtering the data.</td>"
                        + Constants.cr + "</tr>");
                } else {
                    hint = 50;
                    // -- display th report
                    for (int rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                        string row = "";
                        colPtrDownload = 0;
                        int colVisibleCnt = 0;
                        for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                            if (columns[colPtr].visible) {
                                colVisibleCnt++;
                                string classAttribute2 = columns[colPtr].cellClass;
                                if (!string.IsNullOrEmpty(classAttribute2)) {
                                    classAttribute2 = " class=\"" + classAttribute2 + "\"";
                                }
                                string rowContent = localReportCells[rowPtr, colPtr];
                                if ((colVisibleCnt == 1) && rowEllipseMenuDict.ContainsKey(rowPtr)) {
                                    //
                                    // -- add ellipse menu
                                    EllipseMenuDataModel ellipseMenu = new EllipseMenuDataModel {
                                        menuId = rowPtr,
                                        content = rowContent,
                                        hasMenu = true,
                                        menuList = new List<EllipseMenuDataItemModel>()
                                    };
                                    foreach (var menuItem in rowEllipseMenuDict[rowPtr]) {
                                        ellipseMenu.menuList.Add(new EllipseMenuDataItemModel {
                                            menuName = menuItem.name,
                                            menuHref = menuItem.url
                                        });
                                    }
                                    rowContent = cp.Mustache.Render(Properties.Resources.ellipseMenu, ellipseMenu);
                                }
                                row += Constants.cr + "<td" + classAttribute2 + ">" + rowContent + "</td>";
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
                        string classAttribute = localRowClasses[rowPtr];
                        if (rowPtr % 2 != 0) {
                            classAttribute += " afwOdd";
                        }
                        if (classAttribute != "") {
                            classAttribute = " class=\"" + classAttribute + "\"";
                        }
                        rowBuilder.Append(""
                            + Constants.cr + "<tr" + classAttribute + ">"
                            + indent(row)
                            + Constants.cr + "</tr>");
                    }
                }
                hint = 60;
                string csvDownloadFilename = string.Empty;
                if (addCsvDownloadCurrentPage) {
                    //
                    // todo implement cp.db.CreateCsv()
                    // 5.1 -- download
                    CPCSBaseClass csDownloads = cp.CSNew();
                    if (csDownloads.Insert("downloads")) {
                        csvDownloadFilename = csDownloads.GetFilename("filename", "export.csv");
                        cp.CdnFiles.Save(csvDownloadFilename, csvDownloadContent);
                        csDownloads.SetField("name", "Download for [" + title + "], requested by [" + cp.User.Name + "]");
                        csDownloads.SetField("requestedBy", cp.User.Id.ToString());
                        csDownloads.SetField("filename", csvDownloadFilename);
                        csDownloads.SetField("dateRequested", DateTime.Now.ToString());
                        csDownloads.SetField("datecompleted", DateTime.Now.ToString());
                        csDownloads.SetField("resultmessage", "Completed");
                        csDownloads.Save();
                    }
                    csDownloads.Close();
                }
                hint = 70;
                result.Append(""
                    + Constants.cr + "<tbody>"
                    + indent(rowBuilder.ToString())
                    + Constants.cr + "</tbody>"
                    + "");
                result = new StringBuilder(Constants.cr + "<table class=\"afwListReportTable\">" + indent(result.ToString()) + Constants.cr + "</table>");
                if (htmlLeftOfTable != "") {
                    result = new StringBuilder(""
                        + Constants.cr + "<div class=\"afwLeftSideHtml\">" + indent(htmlLeftOfTable) + Constants.cr + "</div>"
                        + Constants.cr + "<div class=\"afwRightSideHtml\">" + indent(result.ToString()) + Constants.cr + "</div>"
                        + Constants.cr + "<div style=\"clear:both\"></div>"
                        + "");
                }
                if (htmlBeforeTable != "") { result.Insert(0, "<div class=\"afwBeforeHtml\">" + htmlBeforeTable + "</div>"); }
                if (htmlAfterTable != "") { result.Append("<div class=\"afwAfterHtml\">" + htmlAfterTable + "</div>"); }
                //
                // headers
                //
                if (addCsvDownloadCurrentPage && (!string.IsNullOrEmpty(csvDownloadFilename))) {
                    result = new StringBuilder(Constants.cr + "<p id=\"afwDescription\"><a href=\"" + cp.Http.CdnFilePathPrefix + csvDownloadFilename + "\">Click here</a> to download the data.</p>" + result.ToString());
                }
                if (description != "") {
                    result = new StringBuilder(Constants.cr + "<p id=\"afwDescription\">" + description + "</p>" + result.ToString());
                }
                if (warning != "") {
                    result = new StringBuilder(Constants.cr + "<div id=\"afwWarning\">" + warning + "</div>" + result.ToString());
                }
                if (title != "") {
                    result = new StringBuilder(Constants.cr + "<h2 id=\"afwTitle\">" + title + "</h2>" + result.ToString());
                }
                hint = 80;
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
                    result = new StringBuilder(Constants.cr + cp.Html.Form(localButtonList + result.ToString() + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, ""));
                }
                if (_includeBodyPadding) { result = new StringBuilder(cp.Html.div(result.ToString(), "", "afwBodyPad", "")); };
                if (_includeBodyColor) { result = new StringBuilder(cp.Html.div(result.ToString(), "", "afwBodyColor", "")); };
                hint = 90;
                //
                // if outer container, add styles and javascript
                //
                if (localIsOuterContainer) {
                    cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                    cp.Doc.AddHeadStyle(Properties.Resources.styles);
                    result = new StringBuilder(""
                        + Constants.cr + "<div id=\"afw\">"
                        + indent(result.ToString())
                        + Constants.cr + "</div>");
                }
                //
                // -- set the optional title of the portal subnav
                if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
                return result.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "hint [" + hint + "]");
                throw;
            }
        }
        //
        //====================================================================================================
        // add html blocks
        public string htmlLeftOfTable { get; set; } = "";
        //
        //====================================================================================================
        //
        public string htmlBeforeTable { get; set; } = "";
        //
        //====================================================================================================
        //
        public string htmlAfterTable { get; set; } = "";
        //
        //====================================================================================================
        /// <summary>
        /// add a form hidden
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
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
        //====================================================================================================
        // add a form button
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
        //====================================================================================================
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
        string localFormActionQueryString = "";
        bool localFormActionQueryStringSet = false;
        //
        //====================================================================================================
        public string formId {
            get {
                return localFormId;
            }
            set {
                localFormId = value;
                localIncludeForm = true;
            }
        }
        string localFormId = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        bool localIncludeForm { get; set; } = false;
        //
        //====================================================================================================
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
        string localFrameRqs = "";
        bool localFrameRqsSet = false;
        //
        //====================================================================================================
        public string guid { get; set; } = "";
        //
        //====================================================================================================
        //
        public string name { get; set; } = "";
        //
        //====================================================================================================
        //
        public string title { get; set; } = "";
        //
        //====================================================================================================
        // Warning
        public string warning { get; set; } = "";
        //
        //====================================================================================================
        // Description
        public string description { get; set; } = "";
        //
        //====================================================================================================
        // column properties
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
        //====================================================================================================
        // column properties
        public string columnCaption {
            get {
                checkColumnPtr();
                return columns[columnPtr].caption;
            }
            set {
                if (value != "") {
                    checkColumnPtr();
                    captionIncluded = true;
                    columns[columnPtr].caption = value;
                }
            }
        }
        //
        //====================================================================================================
        // set the caption class for this column
        public string columnCaptionClass {
            get {
                checkColumnPtr();
                return columns[columnPtr].captionClass;
            }
            set {
                if (value != "") {
                    captionIncluded = true;
                    checkColumnPtr();
                    columns[columnPtr].captionClass = value;
                }
            }
        }
        //
        //====================================================================================================
        // set the cell class for this column
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
        //====================================================================================================
        // set the column sortable
        //  this creates as a link on the caption
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
        //====================================================================================================
        /// <summary>
        /// The column width percentage, 1 to 100
        /// </summary>
        public int columnWidthPercent {
            get {
                checkColumnPtr();
                return columns[columnPtr].columnWidthPercent;
            }
            set {
                checkColumnPtr();
                columns[columnPtr].columnWidthPercent = value;
            }
        }
        //
        //====================================================================================================
        // set the column visible
        //  displays in report
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
        //====================================================================================================
        // set the column downloadable
        //  hides from export
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
        //====================================================================================================
        /// <summary>
        /// Add a new blank column. After adding the column, use properties like .columnName to populate it
        /// </summary>
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
                columns[columnPtr].columnWidthPercent = 0;
                if (columnPtr > columnMax) {
                    columnMax = columnPtr;
                }
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Add a new column populated with the values provided
        /// </summary>
        public void addColumn(ReportListColumnClass column) {
            addColumn();
            columnCaption = column.caption;
            columnCaptionClass = column.captionClass;
            columnCellClass = column.cellClass;
            columnDownloadable = column.downloadable;
            columnName = column.name;
            columnSortable = column.sortable;
            columnVisible = column.visible;
            columnWidthPercent = column.columnWidthPercent;
        }
        //
        //====================================================================================================
        /// <summary>
        /// add a new row. After adding a row, add columns and populate them
        /// </summary>
        public void addRow() {
            localIsEmptyReport = false;
            if (rowCnt < rowSize) {
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
        //====================================================================================================
        // mark this row to exclude from data download
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
        //====================================================================================================
        // add a row class
        public void addRowClass(string styleClass) {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            localRowClasses[rowCnt] += " " + styleClass;
        }
        //
        //====================================================================================================
        // populate a cell
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
        //====================================================================================================
        //
        private string indent(string src) {
            return src.Replace(Constants.cr, Constants.cr2);
        }
        //
        //====================================================================================================
        //
        private void checkColumnPtr() {
            if (columnPtr < 0) {
                addColumn();
            }
        }
        //
        //====================================================================================================
        //
        private void checkRowCnt() {
            if (rowCnt < 0) {
                addRow();
            }
        }
        //
        //====================================================================================================
        // create csv download as form is build
        public bool addCsvDownloadCurrentPage { get; set; } = false;
        //
        //====================================================================================================
        // initialize
        //  read the report and column settings from the Db
        //  if no localGuid set, sync to Db does not work
        //  if the report does not exist in teh Db, use the input values
        //public ReportListClass(CPBaseClass cp) {
        //}
        //private void reportDbInit(CPBaseClass cp) {
        //    try {
        //        int colPtr;
        //        string colName;
        //        string colCaption;
        //        string colSortOrder;
        //        string colCaptionClass;
        //        string colCellClass;
        //        bool colSortable = false;
        //        bool colVisible = true;
        //        bool colDownloadable = true;
        //        string textVisible = "";
        //        //string textDownloadable = "";
        //        CPCSBaseClass cs = cp.CSNew();
        //        int reportId;
        //        string sqlCriteria;
        //        //
        //        if (guid != "") {
        //            sqlCriteria = "ccguid=" + cp.Db.EncodeSQLText(guid);
        //            if (!cs.Open("Admin Framework Reports", sqlCriteria, "", true, "", 9999, 1)) {
        //                cs.Close();
        //                if (name == "") {
        //                    name = title;
        //                }
        //                cs.Insert("Admin Framework reports");
        //                cs.SetField("ccguid", guid);
        //                cs.SetField("name", name);
        //                cs.SetField("title", title);
        //                //cs.SetField("description", localDescription);
        //            }
        //            reportId = cs.GetInteger("id");
        //            name = cs.GetText("name");
        //            title = cs.GetText("title");
        //            //localDescription = cs.GetText("description");
        //            // tmp solution for reports created with a name and no title
        //            if ((title == "") && (name != "")) {
        //                title = name;
        //            }
        //            cs.Close();
        //            //
        //            //
        //            for (colPtr = 0; colPtr <= columnMax; colPtr++) {
        //                colCaption = columns[colPtr].caption;
        //                colName = columns[colPtr].name;
        //                colSortOrder = (colPtr * 10).ToString();
        //                colSortOrder = colSortOrder.PadLeft(4 - colSortOrder.Length, '0');
        //                colCaptionClass = columns[colPtr].captionClass;
        //                colCellClass = columns[colPtr].cellClass;
        //                colSortable = columns[colPtr].sortable; // not part of Db config
        //                colVisible = columns[colPtr].visible;
        //                colDownloadable = columns[colPtr].downloadable;
        //                if (colName == "") {
        //                    colName = colCaption;
        //                }
        //                if ((colName != "") && (reportId != 0)) {
        //                    if (!cs.Open("Admin Framework Report Columns", "(reportId=" + reportId.ToString() + ")and(name=" + cp.Db.EncodeSQLText(colName) + ")", "id", true, "", 9999, 1)) {
        //                        cs.Close();
        //                        cs.Insert("Admin Framework Report Columns");
        //                        cs.SetField("reportId", reportId.ToString());
        //                        cs.SetField("name", colName);
        //                        cs.SetField("caption", colCaption);
        //                        cs.SetField("sortOrder", colSortOrder);
        //                        cs.SetField("captionClass", colCaptionClass);
        //                        cs.SetField("cellClass", colCellClass);
        //                        cs.SetField("visible", colVisible.ToString());
        //                        // for another day
        //                        //cs.SetField("downloadable", colDownloadable.ToString());
        //                    } else {
        //                        // tmp - if name but not caption, use the other
        //                        colCaption = cs.GetText("caption");
        //                        colName = cs.GetText("name");
        //                        if (colCaption == "") {
        //                            colCaption = colName;
        //                        } else if (colName == "") {
        //                            colName = colCaption;
        //                        }
        //                        columns[colPtr].name = colName;
        //                        columns[colPtr].caption = colCaption;
        //                        columns[colPtr].captionClass = cs.GetText("captionClass");
        //                        columns[colPtr].cellClass = cs.GetText("cellClass");
        //                        textVisible = cs.GetText("visible");
        //                        if (textVisible == "") {
        //                            cs.SetField("visible", colVisible.ToString());
        //                        } else {
        //                            columns[colPtr].visible = cp.Utils.EncodeBoolean(textVisible);
        //                        }
        //                        // for another day
        //                        //textDownloadable = cs.GetText("downloadable");
        //                        //if (textDownloadable == "")
        //                        //{
        //                        //    cs.SetField("downloadable", textDownloadable.ToString());
        //                        //}
        //                        //else
        //                        //{
        //                        //    columns[colPtr].visible = cp.Utils.EncodeBoolean(textDownloadable);
        //                        //}
        //                    }
        //                    cs.Close();
        //                }
        //            }
        //        }
        //    } catch (Exception ex) {
        //        cp.Site.ErrorReport(ex, "Exception in reportListClass.init");
        //    }

        //}

    }
    //
    //====================================================================================================
    /// <summary>
    /// The data used to build a column
    /// </summary>
    public class ReportListColumnClass {
        public string name { get; set; }
        public string caption { get; set; }
        public string captionClass { get; set; }
        public string cellClass { get; set; }
        public bool sortable { get; set; } = false;
        public bool visible { get; set; } = true;
        public bool downloadable { get; set; } = false;
        /// <summary>
        /// set as an integer between 1 and 100. This value will be added as the width of the column in a style tag
        /// </summary>
        public int columnWidthPercent { get; set; } = 10;
    }
    //
    public class EllipseMenuItem {
        public string name { get; set; }
        public string url { get; set; }
    }

}