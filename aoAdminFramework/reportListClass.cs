
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
    public class reportListClass
    {
        const int columnSize = 99;
        const int rowSize = 9999;
        const string cr = "\r\n\t";
        const string cr2 = cr + "\t";
        //
        struct columnStruct
        {
            public string name;
            public string caption;
            public string captionClass;
            public string cellClass;
            public bool sortable;
        }
        columnStruct[] columns = new columnStruct[columnSize];
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
        //string localPreForm = "";
        string localHtmlBeforeTable = "";
        string localHtmlAfterTable = "";
        string localHtmlLeftOfTable = "";
        bool localIncludeForm = false;
        bool localIsEmptyReport = true;
        //int localPageSize = 20;
        //int localPageNumber = 1;
        //
        string[,] cells = new string[rowSize, columnSize];
        //
        string[] rowClasses = new string[rowSize];
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
            string s = "";
            string row = "";
            string rowList = "";
            int rowPtr = 0;
            int colPtr = 0;
            string styleClass;
            string content;
            string userErrors;
            string sortLink;
            string columnSort = cp.Doc.GetText("columnSort");
            string sortField="";
            //
            // initialize - setup Db and/or read Db values
            //
            // init(cp);
            //
            // add user errors
            //
            userErrors = cp.Utils.EncodeText(  cp.UserError.GetList());
            if ( userErrors!="")
            {
                warning = userErrors;
            }
            //
            // headers
            //
            if (formIncludeHeader)
            {
                rowList = "";
                for (colPtr = 0; colPtr <= columnMax; colPtr++)
                {
                    styleClass = columns[colPtr].captionClass;
                    if (styleClass != "")
                    {
                        styleClass = " class=\"" + styleClass + "\"";
                    }
                    content = columns[colPtr].caption;
                    sortField = content;
                    if (content == "")
                    {
                        content = "&nbsp;";
                    }
                    else if (columns[colPtr].sortable)
                    {
                        if (localFrameRqsSet ) 
                        {
                            sortLink = "?" + localFrameRqs + "&columnSort=" + sortField;
                        }
                        else
                        {
                            sortLink = "?" + cp.Doc.RefreshQueryString + "&columnSort=" + sortField;
                        }
                        if (columnSort == sortField)
                        {
                            sortLink += "Desc";
                        }
                        content = "<a href=\"" + sortLink + "\">" + content + "</a>";
                    }
                    rowList += cr + "<th" + styleClass + ">" + content + "</th>";
                }
                s += ""
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
            if (localIsEmptyReport)
            {
                styleClass = columns[0].cellClass;
                if (styleClass != "")
                {
                    styleClass = " class=\"" + styleClass + "\"";
                }
                row = cr + "<td style=\"text-align:left\" " + styleClass + " colspan=\"" + (columnMax+1) + "\">[empty]</td>";
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
                    for (colPtr = 0; colPtr <= columnMax; colPtr++)
                    {
                        styleClass = columns[colPtr].cellClass;
                        if (styleClass != "")
                        {
                            styleClass = " class=\"" + styleClass + "\"";
                        }
                        row += cr + "<td" + styleClass + ">" + cells[rowPtr, colPtr] + "</td>";
                    }
                    styleClass = rowClasses[rowPtr];
                    if (rowPtr % 2 != 0)
                    {
                        styleClass += " afwOdd";
                    }
                    if (styleClass != "" )
                    {
                        styleClass = " class=\"" + styleClass + "\"";
                    }
                    rowList += ""
                        + cr + "<tr" + styleClass + ">"
                        + indent(row)
                        + cr + "</tr>";
                }
            }
            s += ""
                + cr + "<tbody>"
                + indent(rowList)
                + cr + "</tbody>"
                + "";
            s = ""
                + cr + "<table class=\"afwListReportTable\">"
                + indent(s)
                + cr + "</table>";
            if (localHtmlLeftOfTable != "")
            {
                s = ""
                    + cr + "<div class=\"afwLeftSideHtml\">"
                    + indent(localHtmlLeftOfTable)
                    + cr + "</div>"
                    + cr + "<div class=\"afwRightSideHtml\">"
                    + indent(s)
                    + cr + "</div>"
                    + cr + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (localHtmlBeforeTable != "")
            {
                s = ""
                    + localHtmlBeforeTable
                    + s
                    + "";
            }
            if (localHtmlAfterTable != "")
            {
                s = ""
                    + s
                    + localHtmlAfterTable
                    + "";
            }
            //
            // headers
            //
            if (localDescription != "")
            {
                s = cr + "<p id=\"afwDescription\">" + localDescription + "</p>" + s;
            }
            if (localWarning != "")
            {
                s = cr + "<div id=\"afwWarning\">" + localWarning + "</div>" + s;
            }
            if (localTitle != "")
            {
                s = cr + "<h2 id=\"afwTitle\">" + localTitle + "</h2>" + s;
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
                s = cr + cp.Html.Form(localButtonList + s + localButtonList + localHiddenList, "", "", "", localFormActionQueryString, "");
                //body = ""
                //    + cr + cp.Html.Form( localButtonList + body + localHiddenList )
                //    + cr + "<form action=\"" + localFormAction + "\" method=\"post\" enctype=\"MULTIPART/FORM-DATA\">"
                //    + indent(localButtonList + body + localHiddenList)
                //    + cr + "</form>";
            }
            s = cp.Html.div(s, "", "afwBodyPad", "");
            s = cp.Html.div(s, "", "afwBodyColor", "");
            //
            // if outer container, add styles and javascript
            //
            if (localIsOuterContainer)
            {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                s = ""
                    + cr + "<div id=\"afw\">"
                    + indent(s)
                    + cr + "</div>";
            }
            return s;
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
                return localFrameRqs;
            }
            set
            {
                localFrameRqs = value;
                localFrameRqsSet = true;
            }
        }
        //
        //-------------------------------------------------
        // Guid
        //-------------------------------------------------
        //
        public string guid
        {
            get
            {
                return localGuid;
            }
            set
            {
                localGuid = value;
            }
        }
        //
        //-------------------------------------------------
        // Name
        //-------------------------------------------------
        //
        public string name
        {
            get
            {
                return localName;
            }
            set
            {
                localName = value;
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
        public string columnName
        {
            get
            {
                checkColumnPtr();
                return columns[columnPtr].name;
            }
            set
            {
                if (value != "")
                {
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
        // set the column sortable
        //  this creates as a link on the caption
        //-------------------------------------------------
        //
        public bool columnSortable
        {
            get
            {
                checkColumnPtr();
                return columns[columnPtr].sortable;
            }
            set
            {
                checkColumnPtr();
                columns[columnPtr].sortable = value;
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
                columns[columnPtr].name = "";
                columns[columnPtr].caption = "";
                columns[columnPtr].captionClass = "";
                columns[columnPtr].cellClass = "";
                columns[columnPtr].sortable = false;
                if (columnPtr > columnMax)
                {
                    columnMax = columnPtr;
                }
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
        // add a row class
        //-------------------------------------------------
        //
        public void addRowClass( string styleClass )
        {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            rowClasses[rowCnt] += " " + styleClass;
        }
        //
        //-------------------------------------------------
        // populate a cell
        //-------------------------------------------------
        //
        public void setCell(string content)
        {
            localIsEmptyReport = false;
            checkColumnPtr();
            checkRowCnt();
            cells[rowCnt, columnPtr] = content;
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
        //
        //-------------------------------------------------
        // initialize
        //  read the report and column settings from the Db
        //  if no localGuid set, sync to Db does not work
        //  if the report does not exist in teh Db, use the input values
        //-------------------------------------------------
        //
        public reportListClass(CPBaseClass cp)
        {
            try
            {
                int colPtr;
                string colName;
                string colCaption;
                string colSortOrder;
                string colCaptionClass;
                string colCellClass;
                bool colSortable;
                CPCSBaseClass cs = cp.CSNew();
                int reportId;
                string sqlCriteria;
                //
                if (localGuid != "")
                {
                    sqlCriteria = "ccguid=" + cp.Db.EncodeSQLText(localGuid);
                    if (!cs.Open("Admin Framework Reports", sqlCriteria))
                    {
                        cs.Close();
                        if(localName=="")
                        {
                            localName=localTitle;
                        }
                        cs.Insert("Admin Framework reports");
                        cs.SetField("ccguid", localGuid);
                        cs.SetField("name", localName );
                        cs.SetField("title", localTitle);
                        cs.SetField("description", localDescription);
                    }
                    reportId = cs.GetInteger("id");
                    localName = cs.GetText("name");
                    localTitle = cs.GetText("title");
                    localDescription = cs.GetText("description");
                    // tmp solution for reports created with a name and no title
                    if((localTitle=="")&&(localName!=""))
                    {
                        localTitle = localName;
                    }
                    cs.Close();
                    //
                    //
                    for (colPtr = 0; colPtr <= columnMax; colPtr++)
                    {
                        colCaption = columns[colPtr].caption;
                        colName = columns[colPtr].name;
                        colSortOrder = (colPtr * 10).ToString();
                        colSortOrder = colSortOrder.PadLeft(4 - colSortOrder.Length, '0');
                        colCaptionClass = columns[colPtr].captionClass;
                        colCellClass = columns[colPtr].cellClass;
                        colSortable = columns[colPtr].sortable; // not part of Db config
                        if (colName == "")
                        {
                            colName = colCaption;
                        }
                        if ((colName != "") && (reportId != 0))
                        {
                            if (!cs.Open("Admin Framework Report Columns", "(reportId=" + reportId.ToString() + ")and(name=" + cp.Db.EncodeSQLText(colName) + ")", "id"))
                            {
                                cs.Close();
                                cs.Insert("Admin Framework Report Columns");
                                cs.SetField("reportId", reportId.ToString());
                                cs.SetField("name", colName);
                                cs.SetField("caption", colCaption);
                                cs.SetField("sortOrder", colSortOrder);
                                cs.SetField("captionClass", colCaptionClass);
                                cs.SetField("cellClass", colCellClass);
                            }
                            else
                            {
                                // tmp - if name but not caption, use the other
                                colCaption = cs.GetText("caption");
                                colName = cs.GetText("name");
                                if (colCaption == "")
                                {
                                    colCaption = colName;
                                }
                                else if (colName == "")
                                {
                                    colName = colCaption;
                                }
                                columns[colPtr].name = colName;
                                columns[colPtr].caption = colCaption;
                                columns[colPtr].captionClass = cs.GetText("captionClass");
                                columns[colPtr].cellClass = cs.GetText("cellClass");
                            }
                            cs.Close();
                        }
                    }
                }
            } 
            catch(Exception ex) 
            {
                cp.Site.ErrorReport(ex, "Exception in reportListClass.init");
            }

        }
    }
}