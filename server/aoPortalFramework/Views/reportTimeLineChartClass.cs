﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contensive.Addons.PortalFramework.Controllers;
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework
{
    public class reportTimeLineChartClass
    {
        const int columnSize = 99;
        const int rowSize = 9999;
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        struct columnStruct
        {
            public string caption;
            public string captionClass;
            public string cellClass;
        }
        //
        struct rowStruct
        {
            public DateTime date;
            public string captionClass;
        }
        struct chartDataStruct
        {
            public double yValue;
            //public string clickLink;
        }
        chartDataStruct[,] chartData = new chartDataStruct[rowSize, columnSize];
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
        bool localIncludeForm = false;
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
        public bool includeBodyPadding
        {
            get
            {
                return _includeBodyPadding;
            }
            set
            {
                _includeBodyPadding = value;
            }
        }
        bool _includeBodyPadding = true;
        //
        //-------------------------------------------------
        //
        public bool includeBodyColor
        {
            get
            {
                return _includeBodyColor;
            }
            set
            {
                _includeBodyColor = value;
            }
        }
        bool _includeBodyColor = true;
        //
        //-------------------------------------------------
        //
        public string yAxisCaption
        {
            get
            {
                return localYAxisCaption;
            }
            set
            {
                localYAxisCaption = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string xAxisCaption
        {
            get
            {
                return localXAxisCaption;
            }
            set
            {
                localXAxisCaption = value;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public bool isOuterContainer
        {
            get
            {
                return localIsOuterContainer;
            }
            set
            {
                localIsOuterContainer = value;
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
                return Properties.Resources.styles;
            }
        }
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        public string javascript
        {
            get
            {
                return Properties.Resources.javascript;
            }
        }
        //
        //-------------------------------------------------
        // getResult
        //-------------------------------------------------
        //
        public string getHtml(CPBaseClass cp)
        {
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
            string chartHtmlId = "afwChart" + GnericController.getRandomHtmlId(cp);
            string captionColumn;
            //string clickLink;
            string jsonRowList = "";
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(cp.UserError.GetList());
            if (userErrors != "")
            {
                warning = userErrors;
            }
            //
            // headers
            //
            jsonData += cr + "data.addColumn('date', 'Date');";
            rowList = "";
            if (gridIncludesCaptionColumn)
            {
                rowList += cr + "<th>" + localXAxisCaption + "</th>";
            }
            for (colPtr = 0; colPtr <= columnMax; colPtr++)
            {
                styleClass = columns[colPtr].captionClass;
                if (styleClass != "")
                {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                content = columns[colPtr].caption;
                if (content == "")
                {
                    content = "&nbsp;";
                }
                rowList += cr + "<th" + styleClass + ">" + content + "</th>";
                jsonData += cr + "data.addColumn('number', '" + content + "');";
            }
            if (gridIncludeHeaderRow)
            {
                result += ""
                    + cr + "<thead>"
                    + cr2 + "<tr>"
                    + indent(indent(rowList))
                    + cr2 + "</tr>"
                    + cr + "</thead>"
                    + "";
            }
            //
            // body
            //
            rowList = "";
            jsonRowList = "";
            if (localIsEmptyReport)
            {
                styleClass = columns[0].cellClass;
                if (styleClass != "")
                {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                row = cr + "<td style=\"text-align:left\" " + styleClass + " colspan=\"" + (columnMax + 1) + "\">[empty]</td>";
                rowList += ""
                    + cr + "<tr>"
                    + indent(row)
                    + cr + "</tr>";
            }
            else
            {
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++)
                {
                    row = "";
                    //
                    // first column is the date
                    //
                    DateTime rowDate;
                    rowDate = rows[rowPtr].date;
                    jsonRow = "new Date(" + rowDate.Year + "," + (rowDate.Month-1) + "," + rowDate.Day + ")";
                    if (gridIncludesCaptionColumn)
                    {
                        captionColumn = rows[rowPtr].date.ToShortDateString();
                        if (captionColumn == "")
                        {
                            captionColumn = "&nbsp;";
                        }
                        row += cr + "<th class=\"" + rows[rowPtr].captionClass + "\">" + captionColumn + "</th>";
                    }
                    //
                    // additional columns are numeric
                    //
                    for (colPtr = 0; colPtr <= columnMax; colPtr++)
                    {
                        styleClass = columns[colPtr].cellClass;
                        if (styleClass != "")
                        {
                            styleClass = " class=\"" + styleClass + "\"";
                        }
                        row += cr + "<td" + styleClass + ">" + chartData[rowPtr, colPtr].yValue + "</td>";
                        jsonRow += "," + chartData[rowPtr, colPtr].yValue;
                    }
                    jsonRowList += "," + cr + "[" + jsonRow + "]";
                    if (rowPtr % 2 == 0)
                    {
                        styleClass = "";
                    }
                    else
                    {
                        styleClass = " class=\"afwOdd\"";
                    }
                    rowList += ""
                        + cr + "<tr" + styleClass + ">"
                        + indent(row)
                        + cr + "</tr>";
                }
            }
            if (jsonRowList != "")
            {
                jsonData += cr + "data.addRows([" + jsonRowList.Substring(1) + "]);";
            }
            result += ""
                + cr + "<tbody>"
                + indent(rowList)
                + cr + "</tbody>"
                + "";
            result = ""
                + cr + "<table class=\"afwListReportTableCollapse\">"
                + indent(result)
                + cr + "</table>";
            result = ""
                + cr + "<div class=\"afwGridCon\">"
                + indent(result)
                + cr + "</div>";
            //
            // chart
            //
            result = ""
                + cr + "<div id=\"" + chartHtmlId + "\" class=\"afwChartCon\" style=\"width:" + localChartWidth.ToString() + "px; height:" + localChartHeight.ToString() + "px;\"></div>"
                + result;
            //
            // reportCon
            //
            result = ""
                + cr + "<div class=\"afwReportCon\">"
                + indent( result )
                + cr + "</div>";
            //
            if (localHtmlLeftOfTable != "")
            {
                result = ""
                    + cr + "<div class=\"afwLeftSideHtml\">"
                    + indent(localHtmlLeftOfTable)
                    + cr + "</div>"
                    + cr + "<div class=\"afwRightSideHtml\">"
                    + indent(result)
                    + cr + "</div>"
                    + cr + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (localHtmlBeforeTable != "")
            {
                result = ""
                    + localHtmlBeforeTable
                    + result
                    + "";
            }
            if (localHtmlAfterTable != "")
            {
                result = ""
                    + result
                    + localHtmlAfterTable
                    + "";
            }
            //
            // headers
            //
            if (localDescription != "")
            {
                result = cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + result;
            }
            if (localWarning != "")
            {
                result = cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + result;
            }
            if (localTitle != "")
            {
                result = cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + result;
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
                result = cr + cp.Html.Form(localButtonList + result + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + cp.Html.Form( localButtonList + body + localHiddenList )
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            if (_includeBodyPadding) { result = cp.Html.div(result, "", "afwBodyPad", ""); };
            if (_includeBodyColor) { result = cp.Html.div(result, "", "afwBodyColor", ""); };
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer)
            {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                result = ""
                    + cr + "<div id=\"afw\">"
                    + indent(result)
                    + cr + "</div>";
            }
            returnHeadJs += ""
                + cr + "function drawChart() {"
                + cr + "var data = new google.visualization.DataTable();" 
                + jsonData
                + cr + "var options={title:'" + localTitle + "',hAxis:{title:'" + localXAxisCaption + "',titleTextStyle:{color: 'red'}},displayAnnotations: true};"
                + cr + "var chart = new google.visualization.AnnotatedTimeLine(document.getElementById('" + chartHtmlId + "'));"
                + cr + "chart.draw(data, options);"
                + cr + "}"
                + cr + "google.load(\"visualization\", \"1\", {packages:[\"annotatedtimeline\"]});"
                + cr + "jQuery(document).ready(drawChart);"
                + cr + "//google.setOnLoadCallback(drawChart);"
                + "";
            result += "<script Language=\"JavaScript\" type=\"text/javascript\">" + returnHeadJs + "</script>";
            returnHeadJs = "";
            cp.Doc.AddHeadJavascript(returnHeadJs);
            return result;
        }
        //
        //-------------------------------------------------
        // add html blocks
        //-------------------------------------------------
        //
        public string htmlLeftOfTable
        {
            get
            {
                return localHtmlLeftOfTable;
            }
            set
            {
                localHtmlLeftOfTable = value;
            }
        }
        //
        public string htmlBeforeTable
        {
            get
            {
                return localHtmlBeforeTable;
            }
            set
            {
                localHtmlBeforeTable = value;
            }
        }
        //
        public string htmlAfterTable
        {
            get
            {
                return localHtmlAfterTable;
            }
            set
            {
                localHtmlAfterTable = value;
            }
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
        public void addFormButton(string buttonValue)
        {
            addFormButton(buttonValue, "button", "", "");
        }
        public void addFormButton(string buttonValue, string buttonName)
        {
            addFormButton(buttonValue, buttonName, "", "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId)
        {
            addFormButton(buttonValue, buttonName, buttonId, "");
        }
        public void addFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass)
        {
            localButtonList += cr + "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
            localIncludeForm = true;
        }
        //
        //-------------------------------------------------
        // setForm
        //-------------------------------------------------
        //
        public string formActionQueryString
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
        // Warning
        //-------------------------------------------------
        //
        public string warning
        {
            get
            {
                return localWarning;
            }
            set
            {
                localWarning = value;
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
        // column properties
        //-------------------------------------------------
        //
        public string columnCaption
        {
            get
            {
                checkColumnPtr();
                return columns[columnPtr].caption;
            }
            set
            {
                if (value != "")
                {
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
        public string columnCaptionClass
        {
            get
            {
                checkColumnPtr();
                return columns[columnPtr].captionClass;
            }
            set
            {
                if (value != "")
                {
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
        public string columnCellClass
        {
            get
            {
                checkColumnPtr();
                return columns[columnPtr].cellClass;
            }
            set
            {
                checkColumnPtr();
                columns[columnPtr].cellClass = value;
            }
        }
        //
        //-------------------------------------------------
        // add a column
        //-------------------------------------------------
        //
        public void addColumn()
        {
            if (columnPtr < columnSize)
            {
                columnPtr += 1;
                columns[columnPtr].caption = "";
                columns[columnPtr].captionClass = "";
                columns[columnPtr].cellClass = "";
                if (columnPtr > columnMax)
                {
                    columnMax = columnPtr;
                }
            }
        }
        //
        //-------------------------------------------------
        // row Caption for grid
        //-------------------------------------------------
        //
        public DateTime rowDate
        {
            get
            {
                checkRowCnt();
                return rows[rowCnt].date;
            }
            set
            {
                checkRowCnt();
                rows[rowCnt].date = value;
                gridIncludesCaptionColumn = true;
            }
        }
        //
        //-------------------------------------------------
        // row Caption Class for grid
        //-------------------------------------------------
        //
        public string rowCaptionClass
        {
            get
            {
                checkRowCnt();
                return rows[rowCnt].captionClass;
            }
            set
            {
                if (value != "")
                {
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
        public int chartWidth
        {
            get
            {
                return localChartWidth;
            }
            set
            {
                localChartWidth = value;
            }
        }
        //
        //-------------------------------------------------
        // chart height
        //-------------------------------------------------
        //
        public int chartHeight
        {
            get
            {
                return localChartHeight;
            }
            set
            {
                localChartHeight = value;
            }
        }
        //
        //-------------------------------------------------
        // add a row
        //-------------------------------------------------
        //
        public void addRow()
        {
            localIsEmptyReport = false;
            if (rowCnt < rowSize)
            {
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
        public void setCell(double yValue)
        {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            chartData[rowCnt, columnPtr].yValue = yValue;
            //chartData[rowCnt, columnPtr].clickLink = clickLink;
            if (columnPtr < columnMax)
            {
                columnPtr += 1;
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
        //
        //-------------------------------------------------
        //
        //-------------------------------------------------
        //
        private void checkColumnPtr()
        {
            if (columnPtr < 0)
            {
                addColumn();
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
    }
}