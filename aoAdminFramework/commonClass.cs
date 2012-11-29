using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace adminFramework
{
     static  class commonClass
    {
        //
        public const string rnPageNumber = "pageNumber";
        public const string rnSetPageNumber = "setPageNumber";
        public const string rnPageSize = "pageSize";
        public const string rnSetPageSize = "setPageSize";
        //
        //=========================================================================================================
        //   GetPageNumber
        //=========================================================================================================
        //
        public static int getPageNumber(CPBaseClass cp)
        {
            int pageNumber = cp.Utils.EncodeInteger(cp.Doc.GetProperty(rnPageNumber, "1"));
            int setPageNumber = cp.Utils.EncodeInteger(  cp.Doc.GetProperty(rnSetPageNumber,""));
            if (setPageNumber!=0) 
            {
                pageNumber = setPageNumber;
            }
            if (pageNumber==0)
            {
                pageNumber=1;
            }
            return pageNumber;

        }
        //
        //=========================================================================================================
        //   GetPageSize
        //=========================================================================================================
        //
        public static int getPageSize(CPBaseClass cp)
        {
            int pageSize = cp.Utils.EncodeInteger(cp.Doc.GetProperty(rnPageSize, "20"));
            int setPageSize = cp.Utils.EncodeInteger(cp.Doc.GetProperty(rnSetPageSize, ""));
            if (setPageSize != 0)
            {
                pageSize = setPageSize;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            return pageSize;

        }

    }
}
