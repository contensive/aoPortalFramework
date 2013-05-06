
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class reportPieChartClass
    {
        const int columnSize = 2;
        const int rowSize = 9999;
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        struct columnStruct
        {
            public string caption;
            public double value;
        }
        //columnStruct[] columns = new columnStruct[columnSize];
        //bool formIncludeHeader = false;
        //
        //int columnMax = -1;
        //int columnPtr = -1;
        //
        int rowCnt = -1;
        //
        string localTitle = "";
        string localChartTitle = "";
        string localWarning = "";
        string localDescription = "";
        string localRqs = "";
        string localHiddenList = "";
        string localButtonList = "";
        string localFormId = "";
        string localFormActionQueryString = "";
        //string localPreForm = "";
        string localHtmlBeforeContent = "";
        string localHtmlAfterContent = "";
        string localHtmlLeftOfContent = "";
        bool localIncludeForm = false;
        bool localIsEmptyReport = true;
        //int localPageSize = 20;
        //int localPageNumber = 1;
        //
        columnStruct[] row = new columnStruct[rowSize];
        //
        bool localIsOuterContainer = false;
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
                //return adminFramework.Resource1.styles.ToString();
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
            string returnHtml = "";
            string returnHeadJs = "";
            string rowList = "";
            int rowPtr = 0;
            string userErrors;
            string jsonData = "";
            string chartHtmlId = "afwChart" + (new Random()).Next(10000,99999);
            double total = 0;

            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(  cp.UserError.GetList());
            if ( userErrors!="")
            {
                warning = userErrors;
            }
            //
            // body
            //
            rowList = "";
            if (localIsEmptyReport)
            {
            }
            else
            {
                for (rowPtr = 0; rowPtr <= rowCnt; rowPtr++)
                {
                    rowList += ""
                        + cr + "<div class=\"afwPieDataRow\">"
                        + cr2 + "<div class=\"afwPieDataValue\">" + row[rowPtr].value + "</div>"
                        + cr2 + "<div class=\"afwPieDataCaption\">" + row[rowPtr].caption + "</div>"
                        + cr + "</div>"
                        + "";
                    total += row[rowPtr].value;
                    jsonData += ",['" + row[rowPtr].caption + "', " + row[rowPtr].value + "]";
                }
            }
            if (jsonData != "")
            {
                jsonData = jsonData.Substring(1);
                rowList += ""
                    + cr + "<div class=\"afwPieTotalRow\">"
                    + cr2 + "<div class=\"afwPieDataValue\">" + total + "</div>"
                    + cr2 + "<div class=\"afwPieDataCaption\">Total</div>"
                    + cr + "</div>"
                    + "";
            }
            returnHtml += ""
                + cr + "<div id=\"" + chartHtmlId + "\" class=\"afwPieChart\"></div>"
                + cr + "<div class=\"afwPieData\">"
                + indent(rowList)
                + cr + "</div>"
                + "";
            returnHtml = ""
                + cr + "<div class=\"afwPieCon\">"
                + indent(returnHtml)
                + cr + "</div>"
                + "";
            if (localHtmlLeftOfContent != "")
            {
                returnHtml = ""
                    + cr + "<div class=\"afwLeftSideHtml\">"
                    + indent(localHtmlLeftOfContent)
                    + cr + "</div>"
                    + cr + "<div class=\"afwRightSideHtml\">"
                    + indent(returnHtml)
                    + cr + "</div>"
                    + cr + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (localHtmlBeforeContent != "")
            {
                returnHtml = ""
                    + localHtmlBeforeContent
                    + returnHtml
                    + "";
            }
            if (localHtmlAfterContent != "")
            {
                returnHtml = ""
                    + returnHtml
                    + localHtmlAfterContent
                    + "";
            }
            //
            // headers
            //
            if (localDescription != "")
            {
                returnHtml = cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + returnHtml;
            }
            if (localWarning != "")
            {
                returnHtml = cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + returnHtml;
            }
            if (localTitle != "")
            {
                returnHtml = cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + returnHtml;
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
                returnHtml = cr + cp.Html.Form(localButtonList + returnHtml + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + cp.Html.Form( localButtonList + body + localHiddenList )
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            returnHtml = cp.Html.div(returnHtml, "", "afwBodyPad", "");
            returnHtml = cp.Html.div(returnHtml, "", "afwBodyColor", "");
            returnHeadJs += ""
                + cr + "google.load('visualization', '1.0', {'packages':['corechart']});"
                + cr + "google.setOnLoadCallback(drawChart);"
                + cr + "function drawChart() {"
                + cr + "var data = new google.visualization.DataTable();"
                + cr + "data.addColumn('string', 'Topping');"
                + cr + "data.addColumn('number', 'Slices');"
                + cr + "data.addRows([" + jsonData + "]);"
                + cr + "var options = {'title':'" + localChartTitle + "','width':500,'height':400};"
                + cr + "var chart = new google.visualization.PieChart(document.getElementById('" + chartHtmlId + "'));"
                + cr + "chart.draw(data, options);"
                + cr + "}"
                + cr + "";
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer)
            {
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                returnHtml = ""
                    + cr + "<div id=\"afw\">"
                    + indent(returnHtml)
                    + cr + "</div>";
            }
            //cp.Doc.AddHeadTag("<script type=\"text/javascript\" src=\"https://www.google.com/jsapi\"></script>");
            cp.Doc.AddHeadJavascript(returnHeadJs);
            return returnHtml;
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
                return localHtmlLeftOfContent;
            }
            set
            {
                localHtmlLeftOfContent = value;
            }
        }
        //
        public string htmlBeforeTable
        {
            get
            {
                return localHtmlBeforeContent;
            }
            set
            {
                localHtmlBeforeContent = value;
            }
        }
        //
        public string htmlAfterTable
        {
            get
            {
                return localHtmlAfterContent;
            }
            set
            {
                localHtmlAfterContent = value;
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
        // Title
        //-------------------------------------------------
        //
        public string chartTitle
        {
            get
            {
                return localChartTitle;
            }
            set
            {
                localChartTitle = value;
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
        ////
        ////-------------------------------------------------
        //// column properties
        ////-------------------------------------------------
        ////
        //public string columnCaption
        //{
        //    get
        //    {
        //        checkColumnPtr();
        //        return columns[columnPtr].caption;
        //    }
        //    set
        //    {
        //        if (value != "")
        //        {
        //            checkColumnPtr();
        //            formIncludeHeader = true;
        //            columns[columnPtr].caption = value;
        //        }
        //    }
        //}
        ////
        ////-------------------------------------------------
        //// set the caption class for this column
        ////-------------------------------------------------
        ////
        //public string columnCaptionClass
        //{
        //    get
        //    {
        //        checkColumnPtr();
        //        return columns[columnPtr].captionClass;
        //    }
        //    set
        //    {
        //        if (value != "")
        //        {
        //            formIncludeHeader = true;
        //            checkColumnPtr();
        //            columns[columnPtr].captionClass = value;
        //        }
        //    }
        //}
        ////
        ////-------------------------------------------------
        //// set the cell class for this column
        ////-------------------------------------------------
        ////
        //public string columnCellClass
        //{
        //    get
        //    {
        //        checkColumnPtr();
        //        return columns[columnPtr].cellClass;
        //    }
        //    set
        //    {
        //        checkColumnPtr();
        //        columns[columnPtr].cellClass = value;
        //    }
        //}
        ////
        ////-------------------------------------------------
        //// add a column
        ////-------------------------------------------------
        ////
        //public void addColumn()
        //{
        //    if (columnPtr < columnSize)
        //    {
        //        columnPtr += 1;
        //        columns[columnPtr].caption = "";
        //        columns[columnPtr].captionClass = "";
        //        columns[columnPtr].cellClass = "";
        //        if (columnPtr > columnMax)
        //        {
        //            columnMax = columnPtr;
        //        }
        //    }
        //}
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
            //columnPtr = 0;
        }
        //
        //-------------------------------------------------
        // populate a row caption
        //-------------------------------------------------
        //
        public void setRowCaption(string content)
        {
            localIsEmptyReport = false;
            checkRowCnt();
            row[rowCnt].caption = content;
        }
        //
        //-------------------------------------------------
        // populate a row value
        //-------------------------------------------------
        //
        public void setRowValue(double  content)
        {
            localIsEmptyReport = false;
            checkRowCnt();
            row[rowCnt].value = content;
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
        ////
        ////-------------------------------------------------
        ////
        ////-------------------------------------------------
        ////
        //private void checkColumnPtr()
        //{
        //    if (columnPtr < 0)
        //    {
        //        addColumn();
        //    }
        //}
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