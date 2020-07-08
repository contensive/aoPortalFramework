
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public class ReportPieChartClass {
        const int columnSize = 2;
        const int rowSize = 9999;
        //
        struct columnStruct {
            public string caption;
            public double value;
        }
        //
        int rowCnt = -1;
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        bool localIncludeForm = false;
        bool localIsEmptyReport = true;
        //
        columnStruct[] row = new columnStruct[rowSize];
        //
        bool gridIncludesCaptionColumn = false;
        string localgridValueClass = "";
        string localFilterListItems = "";
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
        public bool includeBodyColor { get; set; } = true;

        //
        //-------------------------------------------------
        //
        public bool isOuterContainer { get; set; } = false;
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
            string result = "";
            string gridRowList = "";
            int rowPtr = 0;
            string userErrors;
            string jsonData = "";
            string chartHtmlId = "afwChart" + (new Random()).Next(10000, 99999);
            double total = 0;
            string grid = "";
            string gridRow = "";
            string gridRowClass = "";
            string captionColumn = "";
            string percentText = "";

            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "") {
                warning = userErrors;
            }

            //--- imported from bar chart
            //
            // headers
            //
            gridRowList += "<th>" + gridCaptionHeader + "</th>";
            gridRowList += "<th>" + gridValueHeader + "</th>";
            gridRowList += "<th>%</th>";
            gridRowList = ""
                + "<thead>"
                + "<tr>"
                + gridRowList
                + "</tr>"
                + "</thead>"
                + "";
            //---
            //
            // body
            //
            if (localIsEmptyReport) {
            } else {
                //
                // -- first row of data is the captions
                jsonData += ",['" + gridCaptionHeader + "','" + gridValueHeader + "']";
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                    //
                    // -- calcuate the totals
                    total += row[rowPtr].value;
                }
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++) {
                    gridRow = "";
                    //
                    // -- first column is a text caption
                    if (gridIncludesCaptionColumn) {
                        captionColumn = row[rowPtr].caption;
                        if (captionColumn == "") {
                            captionColumn = "&nbsp;";
                        }
                        gridRow += "<th class=\"" + gridCaptionClass + "\">" + captionColumn + "</th>";
                    }
                    //
                    // -- additional columns are numeric
                    gridRow += "<td class=\"" + localgridValueClass + "\">" + row[rowPtr].value.ToString() + "</td>";
                    if (total > 0) {
                        percentText = (row[rowPtr].value / total).ToString("p1");
                    } else {
                        percentText = "n/a";
                    }
                    gridRow += "<td class=\"" + localgridValueClass + "\">" + percentText + "</td>";
                    if (rowPtr % 2 == 0) {
                        gridRowClass = "";
                    } else {
                        gridRowClass = " class=\"afwOdd\"";
                    }
                    gridRowList += ""
                        + "<tr" + gridRowClass + ">"
                        + gridRow
                        + "</tr>";
                    jsonData += ",['" + row[rowPtr].caption + "', " + row[rowPtr].value + "]";
                }
            }
            if (jsonData != "") {
                jsonData = jsonData.Substring(1);
                gridRowList += ""
                    + "<tfoot>"
                    + "<tr>"
                    + "<th class=\"" + gridCaptionClass + "\">Total</th>"
                    + "<td class=\"" + gridValueClass + "\">" + total + "</td>"
                    + "<td class=\"" + gridValueClass + "\">&nbsp;</td>"
                    + "</tr>"
                    + "</tfoot>"
                    + "";
            }
            grid += ""
                + "<tbody>"
                + gridRowList
                + "</tbody>"
                + "";
            grid = ""
                + "<table class=\"afwListReportTableCollapse\">"
                + grid
                + "</table>";
            grid = ""
                + "<div class=\"afwGridCon\">"
                + grid
                + "</div>";
            result = ""
                + "<div class=\"afwChartCon\" style=\"width:" + chartWidth + "px;\">"
                + "<div id=\"" + chartHtmlId + "\"></div>"
                + "</div>"
                + "<div class=\"afwGridCon\">"
                + grid
                + "</div>";
            result = ""
                + "<div class=\"afwPieCon\">"
                + result
                + "</div>"
                + "";
            if (localFilterListItems != "") {
                htmlLeftOfTable += ""
                    + cp.Html.ul("<h3>Filter Options</h3>" + localFilterListItems, "", "afwFilterList")
                    + "";
            }
            if (htmlLeftOfTable != "") {
                result = ""
                    + "<div class=\"afwLeftSideHtml\">"
                    + htmlLeftOfTable
                    + "</div>"
                    + "<div class=\"afwRightSideHtml\">"
                    + result
                    + "</div>"
                    + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (htmlBeforeTable != "") {
                result = ""
                    + htmlBeforeTable
                    + result
                    + "";
            }
            if (htmlAfterTable != "") {
                result = ""
                    + result
                    + htmlAfterTable
                    + "";
            }
            //
            // headers
            //
            if (description != "") {
                result = "<p id=\"afwDescription\">" + description + "</p>" + result;
            }
            if (warning != "") {
                result = "<div id=\"afwWarning\">" + warning + "</div>" + result;
            }
            if (title != "") {
                result = "<h2 id=\"afwTitle\">" + title + "</h2>" + result;
            }
            //
            // add form
            //
            if (localIncludeForm) {
                if (localButtonList != "") {
                    localButtonList = ""
                        + "<div class=\"afwButtonCon\">"
                        + (localButtonList)
                        + "</div>";
                }
                result = cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
            }
            if (_includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
            if (includeBodyColor) { result = cp.Html.div(result, "", "afwBodyColor", ""); };
            string returnHeadJs = "<script type=\"text/javascript\" src=\"https://www.gstatic.com/charts/loader.js\"></script>";
            returnHeadJs += ""
                + "<script Language=\"JavaScript\" type=\"text/javascript\">"
                + "google.charts.load('current', {'packages':['corechart']});"
                + "google.charts.setOnLoadCallback(drawChart);"
                + "function drawChart() {"
                + "var data = google.visualization.arrayToDataTable([" + jsonData + "]);"
                + "var options = {'title':'" + chartTitle + "','width':" + chartWidth + ",'height':" + chartHeight + "};"
                + "var chart = new google.visualization.PieChart(document.getElementById('" + chartHtmlId + "'));"
                + "chart.draw(data, options);"
                + "}"
                + "</script>";
            result += returnHeadJs;
            returnHeadJs = "";
            //
            // if outer container, add styles and javascript
            //
            if (isOuterContainer) {
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                result = ""
                    + "<div id=\"afw\">"
                    + result
                    + "</div>";
            }
            return result;
        }
        //
        //-------------------------------------------------
        // add html blocks
        //-------------------------------------------------
        //
        public string htmlLeftOfTable { get; set; } = "";
        //
        public string htmlBeforeTable { get; set; } = "";
        //
        public string htmlAfterTable { get; set; } = "";
        //
        //-------------------------------------------------
        // add a form hidden
        //-------------------------------------------------
        //
        public void addFormHidden(string Name, string Value) {
            localHiddenList += "<input type=\"hidden\" name=\"" + Name + "\" value=\"" + Value + "\">";
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
            localButtonList += "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
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
        // Title
        //-------------------------------------------------
        //
        public string title { get; set; } = "";
        //
        //-------------------------------------------------
        // Title
        //-------------------------------------------------
        //
        public string chartTitle { get; set; } = "";
        //
        //-------------------------------------------------
        // Warning
        //-------------------------------------------------
        //
        public string warning { get; set; } = "";

        // 
        //-------------------------------------------------
        /// <summary>
        /// Description
        /// </summary>
        public string description { get; set; } = "";
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
        // chart width
        //-------------------------------------------------
        //
        public int chartWidth { get; set; } = 600;
        //
        //-------------------------------------------------
        // chart height
        //-------------------------------------------------
        //
        public int chartHeight { get; set; } = 500;
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string gridCaptionHeader { get; set; } = "Slice";
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string gridValueHeader { get; set; } = "Size";
        //
        //-------------------------------------------------
        // row Caption for grid
        //-------------------------------------------------
        //
        public string rowCaption {
            get {
                checkRowCnt();
                return row[rowCnt].caption;
            }
            set {
                if (value != "") {
                    checkRowCnt();
                    row[rowCnt].caption = value;
                    gridIncludesCaptionColumn = true;
                    localIsEmptyReport = false;
                }
            }
        }
        //
        //-------------------------------------------------
        // row Caption Class for grid
        //-------------------------------------------------
        //
        public string gridCaptionClass { get; set; } = "";
        //
        //-------------------------------------------------
        // row Value
        //-------------------------------------------------
        //
        public double rowValue {
            get {
                checkRowCnt();
                return row[rowCnt].value;
            }
            set {
                checkRowCnt();
                row[rowCnt].value = value;
                gridIncludesCaptionColumn = true;
                localIsEmptyReport = false;
            }
        }
        //
        //-------------------------------------------------
        // row Caption Class for grid
        //-------------------------------------------------
        //
        public string gridValueClass {
            get {
                return localgridValueClass;
            }
            set {
                if (value != "") {
                    localgridValueClass = value;
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
            if (rowCnt < rowSize) {
                rowCnt += 1;
            }
        }
        //
        //-------------------------------------------------
        // add a filter caption
        //-------------------------------------------------
        //
        public void addFilterCaption(string caption) {
            localFilterListItems += "<li class=\"afwFilterCaption\">" + caption + "</li>";
        }
        //
        //-------------------------------------------------
        // add a filter input
        //-------------------------------------------------
        //
        public void addFilterInput(string formInput) {
            localFilterListItems += "<li class=\"afwFilterInput\">" + formInput + "</li>";
        }
    }
}