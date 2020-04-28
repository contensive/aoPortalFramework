using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contensive.Addons.aoPortal
{
    public class adminFrameworkReportColumnClass
    {
        string _name;
        string _sortOrder;
        string _captionClass;
        string _cellClass;
        //
        //-------------------------------------------------
        // column name
        //-------------------------------------------------
        //
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        //
        //-------------------------------------------------
        // column _sortOrder
        //-------------------------------------------------
        //
        public string sortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }
        //
        //-------------------------------------------------
        // column _captionClass
        //-------------------------------------------------
        //
        public string captionClass
        {
            get
            {
                return _captionClass;
            }
            set
            {
                _captionClass = value;
            }
        }
        //
        //-------------------------------------------------
        // column _cellClass
        //-------------------------------------------------
        //
        public string cellClass
        {
            get
            {
                return _cellClass;
            }
            set
            {
                _cellClass = value;
            }
        }
    }
    public class adminFrameworkReportsClass
    {
        string _title = "";
        string _name = "";
        string _guid = "";
        string _description = "";
        ICollection<adminFrameworkReportColumnClass> _columnCollection;
        //
        //-------------------------------------------------
        // Report name
        //-------------------------------------------------
        //
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        //
        //-------------------------------------------------
        // Report title
        //-------------------------------------------------
        //
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        //
        //-------------------------------------------------
        // Report description
        //-------------------------------------------------
        //
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        //
        //-------------------------------------------------
        // Report guid
        //-------------------------------------------------
        //
        public string guid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }
        //
        public ICollection<adminFrameworkReportColumnClass> column
        {
            get
            {
                return _columnCollection;
            }
            set
            {
                _columnCollection = value;
            }
        }
    }
}
