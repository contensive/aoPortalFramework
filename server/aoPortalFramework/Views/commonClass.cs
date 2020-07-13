
using Contensive.BaseClasses;

namespace Contensive.Addons.PortalFramework {
    public static class AfwStyles {
        public const string afwWidth10 = "afwWidth10";
        public const string afwWidth20 = "afwWidth20";
        public const string afwWidth30 = "afwWidth30";
        public const string afwWidth40 = "afwWidth40";
        public const string afwWidth50 = "afwWidth50";
        public const string afwWidth60 = "afwWidth60";
        public const string afwWidth70 = "afwWidth70";
        public const string afwWidth80 = "afwWidth80";
        public const string afwWidth90 = "afwWidth90";
        public const string afwWidth100 = "afwWidth100";
        //
        public const string afwWidth10px = "afwWidth10px";
        public const string afwWidth20px = "afwWidth20px";
        public const string afwWidth30px = "afwWidth30px";
        public const string afwWidth40px = "afwWidth40px";
        public const string afwWidth50px = "afwWidth50px";
        public const string afwWidth60px = "afwWidth60px";
        public const string afwWidth70px = "afwWidth70px";
        public const string afwWidth80px = "afwWidth80px";
        public const string afwWidth90px = "afwWidth90px";
        //
        public const string afwWidth100px = "afwWidth100px";
        public const string afwWidth200px = "afwWidth200px";
        public const string afwWidth300px = "afwWidth300px";
        public const string afwWidth400px = "afwWidth400px";
        public const string afwWidth500px = "afwWidth500px";
        //
        public const string afwMarginLeft100px = "afwMarginLeft100px";
        public const string afwMarginLeft200px = "afwMarginLeft200px";
        public const string afwMarginLeft300px = "afwMarginLeft300px";
        public const string afwMarginLeft400px = "afwMarginLeft400px";
        public const string afwMarginLeft500px = "afwMarginLeft500px";
        //
        public const string afwTextAlignRight = "afwTextAlignRight";
        public const string afwTextAlignLeft = "afwTextAlignLeft";
        public const string afwTextAlignCenter = "afwTextAlignCenter";
    }
    static class CommonClass {
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
        public static int getPageNumber(CPBaseClass cp) {
            int pageNumber = cp.Utils.EncodeInteger(cp.Doc.GetProperty(rnPageNumber, "1"));
            int setPageNumber = cp.Utils.EncodeInteger(cp.Doc.GetProperty(rnSetPageNumber, ""));
            if (setPageNumber != 0) {
                pageNumber = setPageNumber;
            }
            if (pageNumber == 0) {
                pageNumber = 1;
            }
            return pageNumber;

        }
        //
        //=========================================================================================================
        //   GetPageSize
        //=========================================================================================================
        //
        public static int getPageSize(CPBaseClass cp) {
            int pageSize = cp.Doc.GetInteger(rnPageSize, 20);
            int setPageSize = cp.Doc.GetInteger(rnSetPageSize);
            if (setPageSize != 0) {
                pageSize = setPageSize;
            }
            if (pageSize == 0) {
                pageSize = 20;
            }
            return pageSize;

        }

    }
}
