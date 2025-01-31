
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.Addons.PortalFramework.Models.Domain;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    //
    /// <summary>
    /// Create a layout with a data grid
    /// </summary>
    public class ReportListClass : LayoutBuilderBaseClass {
        //
        // ====================================================================================================
        // constructors
        //
        /// <summary>
        /// prefered constructor
        /// </summary>
        /// <param name="cp"></param>
        [Obsolete("Deprecated. Use cp.AdminUI.CreateLayoutBuilderList()", false)]
        public ReportListClass(CPBaseClass cp) { 
            this.cp = cp;
        }
        //
        /// <summary>
        /// legacy constructor, without cp. (cp needed for pagination)
        /// </summary>
        [Obsolete("Deprecated. Use cp.AdminUI.CreateLayoutBuilderList()", false)]
        public ReportListClass() { }
        //
        // ====================================================================================================
        // privates
        /// <summary>
        /// used for pagination and export
        /// </summary>
        private CPBaseClass cp { get; }
        //
        //
        // ====================================================================================================
        // publics
        //
        /// <summary>
        /// if set true, the pageSie and pageNumber will control pagination
        /// The grid will include pagination controls, and the client application should read pageSize and pageNumber when setting up the query
        /// </summary>
        public bool allowPagination { get; set; }
        //
        /// <summary>
        /// Only valid if allowPagination is set to true.
        /// Set the pageSize used by default.
        /// The user may select a different page size.
        /// </summary>
        public int paginationPageSizeDefault { 
            get {
                if(_paginationPageSizeDefault!= null ) { return (int)_paginationPageSizeDefault; }
                _paginationPageSizeDefault = 50;
                return (int)_paginationPageSizeDefault;
            }
            set {
                _paginationPageSizeDefault = value;
            } 
        }
        private int? _paginationPageSizeDefault;
        //
        /// <summary>
        /// if allowPagination false, this will will be 9999999. 
        /// If allowPagination true, this is the number of rows in the display, and should be used as the pageSize in the query
        /// </summary>
        public int paginationPageSize {
            get {
                if (_paginationPageSize != null) { return (int)_paginationPageSize; }
                _paginationPageSize = paginationPageSizeDefault;
                return (int)_paginationPageSize;
            }
            set {
                if (value < 1) { value = 1; }
                if (value > 9999999) { value = 9999999; }
                _paginationPageSize = value;
            }
        }
        private int? _paginationPageSize;
        //
        /// <summary>
        /// The 0-based page number being displayed
        /// if allowPagination false, this will will be 0 (the first page). 
        /// If allowPagination true, this is the page shown in the display, and should be used as the pageNumber in the query
        /// </summary>
        public int paginationPageNumber {
            get {
                if (_paginationPageNumber != null) { return (int)_paginationPageNumber; }
                if (!allowPagination || cp == null) {
                    return 0;
                }
                _paginationPageNumber = cp.Request.GetInteger("paginationPageNumber");
                return (int)_paginationPageSize;
            }
            set {
                if (value < 0) { value = 0; }
                if (value > 9999999) { value = 9999999; }
                _paginationPageNumber = value;
            }
        }
        private int? _paginationPageNumber;
        //
        /// <summary>
        /// Add an ellipse menu entry for the current row
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
        //
        // --- privates
        //
        /// <summary>
        /// The report grid data
        /// </summary>
        private readonly string[,] localReportCells = new string[rowSize, columnSize];
        //
        /// <summary>
        /// the report download data
        /// </summary>
        private readonly string[,] localDownloadData = new string[rowSize, columnSize];
        //
        /// <summary>
        /// add indent to the source
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private string indent(string src) {
            return src.Replace(Constants.cr, Constants.cr2);
        }
        //
        /// <summary>
        /// check if first column has been added. If not add the first column.
        /// </summary>
        private void checkColumnPtr() {
            if (columnPtr < 0) {
                addColumn();
            }
        }
        //
        /// <summary>
        /// check if the first row has been added. if not, add it
        /// </summary>
        private void checkRowCnt() {
            if (rowCnt < 0) {
                addRow();
            }
        }
        //
        /// <summary>
        /// list of elipse menu items added to the rightmost column
        /// </summary>
        private Dictionary<int, List<EllipseMenuItem>> rowEllipseMenuDict { get; set; } = new Dictionary<int, List<EllipseMenuItem>>();
        //
        /// <summary>
        /// maximum columns allowed
        /// </summary>
        private const int columnSize = 99;
        //
        /// <summary>
        /// maximum rows allowed
        /// </summary>
        private const int rowSize = 19999;
        //
        /// <summary>
        /// todo deprecate - use ReportListColumnClass
        /// </summary>
        private struct ColumnStruct {
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
        private readonly ColumnStruct[] columns = new ColumnStruct[columnSize];
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
        //====================================================================================================
        /// <summary>
        /// create csv download as form is build
        /// </summary>
        public bool addCsvDownloadCurrentPage { get; set; } = false;
        ////
        //// ====================================================================================================
        ///// <summary>
        ///// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        ///// </summary>
        //public string portalSubNavTitle { get; set; }
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
        ////
        ////====================================================================================================
        ////
        //public bool includeBodyPadding {
        //    get {
        //        return includeBodyPadding_Local;
        //    }
        //    set {
        //        includeBodyPadding_Local = value;
        //    }
        //}
        //private bool includeBodyPadding_Local = true;
        ////
        ////====================================================================================================
        ////
        //public bool includeBodyColor {
        //    get {
        //        return includeBodyColor_Local;
        //    }
        //    set {
        //        includeBodyColor_Local = value;
        //    }
        //}
        //private bool includeBodyColor_Local = true;
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// if true, this report is part of a larger layout and the internal styles and js should NOT be added
        ///// </summary>
        //public bool isOuterContainer {
        //    get {
        //        return localIsOuterContainer;
        //    }
        //    set {
        //        localIsOuterContainer = value;
        //    }
        //}
        //private bool localIsOuterContainer = false;
        ////
        ////====================================================================================================
        ////
        //public string styleSheet {
        //    get {
        //        return Properties.Resources.styles;
        //    }
        //}
        ////
        ////====================================================================================================
        ////
        //public string javascript {
        //    get {
        //        return Properties.Resources.javascript;
        //    }
        //}
        //
        //====================================================================================================
        //
        /// <summary>
        /// render the report
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public new string getHtml(CPBaseClass cp) {
            int hint = 0;
            try {
                ////
                //// -- set the optional title of the portal subnav
                //if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
                //
                StringBuilder rowBuilder = new StringBuilder("");
                string columnSort = cp.Doc.GetText("columnSort");
                string csvDownloadContent = "";
                DateTime rightNow = DateTime.Now;
                hint = 10;
                //
                // add user errors
                //
                string userErrors = cp.Utils.ConvertHTML2Text(cp.UserError.GetList());
                if (!string.IsNullOrEmpty(userErrors)) {
                    warningMessage += userErrors;
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
                    string refreshQueryString = (!string.IsNullOrEmpty(base.refreshQueryString) ? base.refreshQueryString : cp.Doc.RefreshQueryString);
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
                                string sortLink;
                                sortLink = "?" + base.refreshQueryString + "&columnSort=" + sortField;
                                if (columnSort == sortField) {
                                    sortLink += "Desc";
                                }
                                content = "<a href=\"" + sortLink + "\">" + content + "</a>";
                            }
                            string styleAttribute = "";
                            if (columns[colPtr].columnWidthPercent > 0) {
                                styleAttribute = " style=\"width:" + columns[colPtr].columnWidthPercent.ToString() + "%;\"";
                            }
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
                    rowBuilder.Append(""
                        + Constants.cr + "<tr>"
                        + Constants.cr + "<td style=\"text-align:left\" colspan=\"" + (columnMax + 1) + "\">[empty]</td>"
                        + Constants.cr + "</tr>");
                } else if (ReportTooLong) {
                    //
                    // -- report is too long
                    string classAttribute = columns[0].cellClass;
                    if (classAttribute != "") {
                        classAttribute = " class=\"" + classAttribute + "\"";
                    }
                    rowBuilder.Append(""
                        + Constants.cr + "<tr>"
                        + Constants.cr + "<td style=\"text-align:left\" " + classAttribute + " colspan=\"" + (columnMax + 1) + "\">There are too many rows in this report. Please consider filtering the data.</td>"
                        + Constants.cr + "</tr>");
                } else {
                    hint = 50;
                    //
                    // -- if ellipse needed, determine last visible column
                    int colPtrLastVisible = -1;
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        if (columns[colPtr].visible) {
                            colPtrLastVisible = colPtr;
                        }
                    }
                    //
                    // -- output the grid
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
                                if ((colPtrLastVisible == colPtr) && rowEllipseMenuDict.ContainsKey(rowPtr)) {
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
                base.body = result.ToString();
                return base.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "hint [" + hint + "]");
                throw;
            }
        }
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// An html block added to the left of the table. Typically used for filters.
        ///// </summary>
        //public string htmlLeftOfTable { get; set; } = "";
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// An html block added above the table. Typically used for filters.
        ///// </summary>
        //public string htmlBeforeTable { get; set; } = "";
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// An html block added below the table. Typically used for filters.
        ///// </summary>
        //public string htmlAfterTable { get; set; } = "";
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// set true will block the addition of a form tag around the final html.
        ///// The form tag is automatically added if you add a button, hidden or formActionRefreshString
        ///// </summary>
        //public bool blockFormTag { get; set; } = false;
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// Add a form hidden element. 
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="Name"></param>
        ///// <param name="Value"></param>
        //public void addFormHidden(string Name, string Value) {
        //    hiddenList += Constants.cr + "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
        //    includeForm = includeForm || !string.IsNullOrEmpty(Value);
        //}
        ///// <summary>
        ///// Add a form hidden.
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void addFormHidden(string name, int value) => addFormHidden(name, value.ToString());
        ///// <summary>
        ///// Add a form hidden.
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void addFormHidden(string name, double value) => addFormHidden(name, value.ToString());
        ///// <summary>
        ///// Add a form hidden.
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void addFormHidden(string name, DateTime value) => addFormHidden(name, value.ToString());
        ///// <summary>
        ///// Add a form hidden.
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        //public void addFormHidden(string name, bool value) => addFormHidden(name, value.ToString());
        //
        /// <summary>
        /// local list of hidden form elements
        /// </summary>
        //private string hiddenList = "";
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// Add a form button. 
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="buttonValue"></param>
        //public void addFormButton(string buttonValue) {
        //    addFormButton(buttonValue, "button", "", "");
        //}
        ///// <summary>
        ///// Add a form button. 
        ///// Adding a form button also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="buttonValue"></param>
        ///// <param name="buttonName"></param>
        //public void addFormButton(string buttonValue, string buttonName) {
        //    addFormButton(buttonValue, buttonName, "", "");
        //}
        ///// <summary>
        ///// Add a form button. 
        ///// Adding a form button also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="buttonValue"></param>
        ///// <param name="buttonName"></param>
        ///// <param name="buttonId"></param>
        //public void addFormButton(string buttonValue, string buttonName, string buttonId) {
        //    addFormButton(buttonValue, buttonName, buttonId, "");
        //}
        ///// <summary>
        ///// Add a form button. 
        ///// Adding also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        ///// <param name="buttonValue"></param>
        ///// <param name="buttonName"></param>
        ///// <param name="buttonId"></param>
        ///// <param name="buttonClass"></param>
        //public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
        //    buttonList += HtmlController.getButton(buttonName, buttonValue, buttonId, buttonClass);
        //    includeForm = true;
        //}
        //private string buttonList = "";
        ////
        ////====================================================================================================
        ////
        ///// <summary>
        ///// Set the same as the refresh query string, except exclude all form inputs within the form. For instance, if the form has a filter that include 'dateTo', add dateTo to the RQS so heading sorts retain the value, but do not add it to formAction because the input box in the filter already has that value.
        ///// Setting also wraps the html with a form tag. Block the form tag with blockFormTag.
        ///// </summary>
        //public string formActionQueryString {
        //    get {
        //        return formActionQueryString_Local;
        //    }
        //    set {
        //        formActionQueryString_Local = value;
        //        localFormActionQueryStringSet = true;
        //        includeForm = !string.IsNullOrEmpty(value);
        //    }
        //}
        //private string formActionQueryString_Local = "";
        //private bool localFormActionQueryStringSet = false;
        //
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        private bool includeForm { get; set; } = false;
        ////
        ////====================================================================================================
        ///// <summary>
        ///// Include all nameValue pairs required to refresh the page if someone clicks on a header. For example, if there is a filter dateTo that is not empty, add dateTo=1/1/2000 to the RQS
        ///// </summary>
        //public string refreshQueryString {
        //    get {
        //        return refreshQueryString_Local;
        //    }
        //    set {
        //        refreshQueryString_Local = value;
        //        refreshQueryStringSet_Local = true;
        //    }
        //}
        //private string refreshQueryString_Local = "";
        //private bool refreshQueryStringSet_Local = false;
        //
        //====================================================================================================
        /// <summary>
        /// deprecated. Had previously been the guid of the saved report record.
        /// </summary>
        [Obsolete("deprecated. Had previously been the guid of the saved report record.", false)]
        public string guid { get; set; } = "";
        //
        //====================================================================================================
        /// <summary>
        /// deprecated. Had previously been the name of the saved report record.
        /// </summary>
        [Obsolete("deprecated. Had previously been the name of the saved report record.", false)]
        public string name { get; set; } = "";
        ////
        ////====================================================================================================
        ///// <summary>
        ///// The headline of the generated html document
        ///// </summary>
        //public string title { get; set; } = "";
        ////
        ////====================================================================================================
        ///// <summary>
        ///// Optional warning text displayed at the top of the html document. Typically used to warn the user of an issue.
        ///// </summary>
        //public string warning { get; set; } = "";
        ////
        ////====================================================================================================
        ///// <summary>
        ///// Optional description displayed below the title at the top of the html document
        ///// </summary>
        //public string description { get; set; } = "";
        //
        //====================================================================================================
        /// <summary>
        /// The name of the current column.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// <summary>
        /// The caption displayed in the top row of the table.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// <summary>
        /// Optional class to be added to the caption for the current column.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// <summary>
        /// Optional class to be added to each cell in this column.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        //
        /// <summary>
        /// If true, the column caption will be linked and if the user clicks it, the form will redraw giving the calling code the opportunity to sort data accordingly.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// Optional integer percentagle added to the caption for this column. 1 to 100.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
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
        // 
        /// <summary>
        /// Default true. If set false, the column will not display but will be exported in the csv.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        // 
        /// <summary>
        /// set the column downloadable
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// Add a new blank column. After adding the column, use properties like .columnName to populate it.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
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
        /// Add a new column populated with the values provided.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
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
        /// add a new row. After adding a row, add columns and populate them.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
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
        private bool localIsEmptyReport = true;
        //
        //====================================================================================================
        // 
        /// <summary>
        /// mark this row to exclude from data download.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// <summary>
        /// if true, exclude this row from download
        /// </summary>
        private readonly bool[] localExcludeRowFromDownload = new bool[rowSize];
        //
        //====================================================================================================
        // 
        /// <summary>
        /// Set the html class for the current row.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        /// <param name="styleClass"></param>
        public void addRowClass(string styleClass) {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            localRowClasses[rowCnt] += " " + styleClass;
        }
        //
        /// <summary>
        /// the report row styles
        /// </summary>
        private readonly string[] localRowClasses = new string[rowSize];
        //
        //====================================================================================================
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        /// <param name="content"></param>
        public void setCell(string content) {
            setCell(content, content);
        }
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
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
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(int content) => setCell(content.ToString(), content.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(int content, int downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        //
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(double content) => setCell(content.ToString(), content.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(double content, double downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(bool content) => setCell(content.ToString(), content.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(bool content, bool downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(DateTime content) => setCell(content.ToString(), content.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(DateTime? content) => setCell((content == null) ? "" : content.ToString(), content.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(DateTime content, DateTime downloadContent) => setCell(content.ToString(), downloadContent.ToString());
        // 
        /// <summary>
        /// populate a cell.
        /// To define a column, first call addColumn(), then set its name, caption, captionclass, cellclass, visible, sortable, width, downloadable. When columns are defined, use addRow() to create a row, then addCell() repeately to create a cell for each column.
        /// </summary>
        public void setCell(DateTime? content, DateTime downloadContent) => setCell((content == null) ? "" : content.ToString(), downloadContent.ToString());
        //
        //====================================================================================================
        //
        // -- deprecated
        //
        /// <summary>
        /// deprecated. use addHidden() to add a formId hidden tag
        /// </summary>
        [Obsolete("deprecated. use addHidden() to add a formId hidden tag", false)]
        public string formId {
            get {
                return localFormId_Local;
            }
            set {
                localFormId_Local = value;
                includeForm = !string.IsNullOrEmpty(value);
            }
        }
        private string localFormId_Local = "";
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