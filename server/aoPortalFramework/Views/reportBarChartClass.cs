
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class ReportBarChartClass {
        const int columnSize = 99;
        const int rowSize = 9999;
        //
        struct columnStruct {
            public string caption;
            public string captionClass;
            public string cellClass;
        }
        //
        struct rowStruct {
            public string caption;
            public string captionClass;
        }
        struct barDataStruct {
            public int height;
            public string clickLink;
        }
        barDataStruct[,] barData = new barDataStruct[rowSize, columnSize];
        columnStruct[] columns = new columnStruct[columnSize];
        rowStruct[] rows = new rowStruct[rowSize];
        //
        bool gridIncludeHeaderRow = false;
        bool gridIncludesCaptionColumn = false;
        //
        int columnMax = -1;
        int columnPtr = -1;
        //
        int rowCnt = -1;
        //
        string localTitle = "";
        string localWarning = "";
        string localDescription = "";
        //string localFrameRqs = "";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        //string localPreForm = "";
        string localHtmlBeforeTable = "";
        string localHtmlAfterTable = "";
        string localHtmlLeftOfTable = "";
        //
        //-------------------------------------------------
        /// <summary>
        /// If true, the resulting html is wrapped in a form element whose action returns execution back to this addon where is it processed here in the same code.
        /// consider a pattern that blocks the include form if this layout is called form the portal system, where the portal methods create the entire strucuture
        /// </summary>
        bool localIncludeForm { get; set; } = false;
        bool localIsEmptyReport = true;
        //int localPageSize = 20;
        //int localPageNumber = 1;
        //
        string localXAxisCaption = "";
        string localYAxisCaption = "";
        //
        int localChartWidth = 600;
        int localChartHeight = 500;
        //
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
        public string yAxisCaption {
            get {
                return localYAxisCaption;
            }
            set {
                localYAxisCaption = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string xAxisCaption {
            get {
                return localXAxisCaption;
            }
            set {
                localXAxisCaption = value;
            }
        }
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
        // ====================================================================================================
        /// <summary>
        /// Optional. If set, this value will populate the title in the subnav of the portalbuilder
        /// </summary>
        public string portalSubNavTitle { get; set; }
        //
        //-------------------------------------------------
        // getResult
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp) {
            string result = "";
            string row = "";
            string rowList = "";
            int rowPtr = 0;
            int colPtr = 0;
            string styleClass;
            string content;
            string userErrors;
            string returnHeadJs = "";
            string jsonData = "";
            string jsonRow = "";
            string chartHtmlId = "afwChart" + HtmlController.getRandomHtmlId();
            string captionColumn;
            string clickLink;
            //
            // add user errors
            //
            userErrors = cp.Utils.ConvertHTML2Text(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }
            //
            // headers
            //
            jsonRow = "'" + localXAxisCaption + "'";
            rowList = "";
            if (gridIncludesCaptionColumn) {
                rowList += Constants.cr + "<th>" + localXAxisCaption + "</th>";
            }
            for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                styleClass = columns[colPtr].captionClass;
                if (styleClass != "") {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                content = columns[colPtr].caption;
                if (content == "") {
                    content = "&nbsp;";
                }
                rowList += Constants.cr + "<th" + styleClass + ">" + content + "</th>";
                jsonRow += ",'" + content + "'";
            }
            jsonData = Constants.cr + "[" + jsonRow + "]";
            if (gridIncludeHeaderRow) {
                result += ""
                    + Constants.cr + "<thead>"
                    + Constants.cr2 + "<tr>"
                    + indent(indent(rowList))
                    + Constants.cr2 + "</tr>"
                    + Constants.cr + "</thead>"
                    + "";
            }
            //
            // body
            //
            rowList = "";
            if (localIsEmptyReport) {
                styleClass = columns[0].cellClass;
                if (styleClass != "") {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                row = Constants.cr + "<td style=\"text-align:left\" " + styleClass + " colspan=\"" + (columnMax + 1) + "\">[empty]</td>";
                rowList += ""
                    + Constants.cr + "<tr>"
                    + indent(row)
                    + Constants.cr + "</tr>";
            } else {
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                    row = "";
                    //
                    // first column is a text caption
                    //
                    jsonRow = "'" + rows[rowPtr].caption + "'";
                    if (gridIncludesCaptionColumn) {
                        captionColumn = rows[rowPtr].caption;
                        if (captionColumn == "") {
                            captionColumn = "&nbsp;";
                        }
                        row += Constants.cr + "<th class=\"" + rows[rowPtr].captionClass + "\">" + captionColumn + "</th>";
                    }
                    //
                    // additional columns are numeric
                    //
                    for (colPtr = 0; colPtr <= columnMax; colPtr++) {
                        styleClass = columns[colPtr].cellClass;
                        if (styleClass != "") {
                            styleClass = " class=\"" + styleClass + "\"";
                        }
                        clickLink = barData[rowPtr, colPtr].clickLink;
                        if (clickLink == "") {
                            row += Constants.cr + "<td" + styleClass + ">" + barData[rowPtr, colPtr].height + "</td>";
                        } else {
                            row += Constants.cr + "<td" + styleClass + " onclick=\"location.href='" + clickLink + "';\" style=\"cursor:pointer;\">" + barData[rowPtr, colPtr].height + "</td>";
                        }
                        jsonRow += "," + barData[rowPtr, colPtr].height;
                    }
                    jsonData += Constants.cr + ",[" + jsonRow + "]";
                    if (rowPtr % 2 == 0) {
                        styleClass = "";
                    } else {
                        styleClass = " class=\"afwOdd\"";
                    }
                    rowList += ""
                        + Constants.cr + "<tr" + styleClass + ">"
                        + indent(row)
                        + Constants.cr + "</tr>";
                }
            }
            result += ""
                + Constants.cr + "<tbody>"
                + indent(rowList)
                + Constants.cr + "</tbody>"
                + "";
            result = ""
                + Constants.cr + "<table class=\"afwListReportTableCollapse\">"
                + indent(result)
                + Constants.cr + "</table>";
            result = ""
                + Constants.cr + "<div class=\"afwGridCon\">"
                + indent(result)
                + Constants.cr + "</div>";
            //
            // bar chart
            //
            result = Constants.cr + "<div id=\"" + chartHtmlId + "\" class=\"afwChartCon\" style=\"width:" + localChartWidth.ToString() + "px; height:" + localChartHeight.ToString() + "px;\"></div>" + result;
            //
            if (localHtmlLeftOfTable != "") {
                result = ""
                    + Constants.cr + "<div class=\"afwLeftSideHtml\">"
                    + indent(localHtmlLeftOfTable)
                    + Constants.cr + "</div>"
                    + Constants.cr + "<div class=\"afwRightSideHtml\">"
                    + indent(result)
                    + Constants.cr + "</div>"
                    + Constants.cr + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (localHtmlBeforeTable != "") {
                result = ""
                    + localHtmlBeforeTable
                    + result
                    + "";
            }
            if (localHtmlAfterTable != "") {
                result = ""
                    + result
                    + localHtmlAfterTable
                    + "";
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
            if (_includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
            if (localIncludeForm) {
                //if (localButtonList != "") {
                //    localButtonList = ""
                //        + Constants.cr + "<div class=\"afwButtonCon\">"
                //        + indent(localButtonList)
                //        + Constants.cr + "</div>";
                //}
                result = Constants.cr + cp.Html.Form(result + HtmlController.getButtonSection(localButtonList) + localHiddenList, "", "", "", localFormActionQueryString, "");
            }
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
            returnHeadJs += ""
                + Constants.cr + "google.load(\"visualization\", \"1\", {packages:[\"corechart\"]});"
                + Constants.cr + "google.setOnLoadCallback(drawChart);"
                + Constants.cr + "function drawChart() {"
                + Constants.cr + "var data = google.visualization.arrayToDataTable([" + jsonData + "]);"
                + Constants.cr + "var options={title:'" + localTitle + "',hAxis:{title:'" + localXAxisCaption + "',titleTextStyle:{color: 'red'}}};"
                + Constants.cr + "var chart = new google.visualization.ColumnChart(document.getElementById('" + chartHtmlId + "'));"
                + Constants.cr + "chart.draw(data, options);"
                + Constants.cr + "}";
            result += "<script Language=\"JavaScript\" type=\"text/javascript\">" + returnHeadJs + "</script>";
            returnHeadJs = "";
            //
            // -- set the optional title of the portal subnav
            if (!string.IsNullOrEmpty(portalSubNavTitle)) { cp.Doc.SetProperty("portalSubNavTitle", portalSubNavTitle); }
            return result;
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
            localButtonList += HtmlController.getButton(buttonName, buttonValue, buttonId, buttonClass);
            //localButtonList += Constants.cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
            localIncludeForm = true;
        }
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
        public string columnCaption {
            get {
                checkColumnPtr();
                return columns[columnPtr].caption;
            }
            set {
                if (value != "") {
                    checkColumnPtr();
                    gridIncludeHeaderRow = true;
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
                    gridIncludeHeaderRow = true;
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
        // add a column
        //-------------------------------------------------
        //
        public void addColumn() {
            if (columnPtr < columnSize) {
                columnPtr += 1;
                columns[columnPtr].caption = "";
                columns[columnPtr].captionClass = "";
                columns[columnPtr].cellClass = "";
                if (columnPtr > columnMax) {
                    columnMax = columnPtr;
                }
            }
        }
        //
        //-------------------------------------------------
        // row Caption for grid
        //-------------------------------------------------
        //
        public string rowCaption {
            get {
                checkRowCnt();
                return rows[rowCnt].caption;
            }
            set {
                if (value != "") {
                    checkRowCnt();
                    rows[rowCnt].caption = value;
                    gridIncludesCaptionColumn = true;
                }
            }
        }
        //
        //-------------------------------------------------
        // row Caption Class for grid
        //-------------------------------------------------
        //
        public string rowCaptionClass {
            get {
                checkRowCnt();
                return rows[rowCnt].captionClass;
            }
            set {
                if (value != "") {
                    checkRowCnt();
                    rows[rowCnt].captionClass = value;
                    gridIncludesCaptionColumn = true;
                }
            }
        }
        //
        //-------------------------------------------------
        // chart width
        //-------------------------------------------------
        //
        public int chartWidth {
            get {
                return localChartWidth;
            }
            set {
                localChartWidth = value;
            }
        }
        //
        //-------------------------------------------------
        // chart height
        //-------------------------------------------------
        //
        public int chartHeight {
            get {
                return localChartHeight;
            }
            set {
                localChartHeight = value;
            }
        }
        //
        //-------------------------------------------------
        // add a row
        //-------------------------------------------------
        //
        public void addRow() {
            localIsEmptyReport = false;
            if (rowCnt < rowSize) {
                rowCnt += 1;
            }
            checkColumnPtr();
            columnPtr = 0;
        }
        //
        //-------------------------------------------------
        // populate a cell
        //-------------------------------------------------
        //
        public void setCell(int barHeight, string clickLink) {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            barData[rowCnt, columnPtr].height = barHeight;
            barData[rowCnt, columnPtr].clickLink = clickLink;
            if (columnPtr < columnMax) {
                columnPtr += 1;
            }
        }
        public void setCell(int barHeight) {
            setCell(barHeight, "");
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private string indent(string src) {
            return src.Replace(Constants.cr, Constants.cr2);
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
    }
}