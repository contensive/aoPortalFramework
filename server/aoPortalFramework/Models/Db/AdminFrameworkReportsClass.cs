
using System.Collections.Generic;

namespace Contensive.Addons.PortalFramework {
    public class AdminFrameworkReportsClass {
        //
        //-------------------------------------------------
        // Report name
        //-------------------------------------------------
        //
        public string name { get; set; } = "";
        //
        //-------------------------------------------------
        // Report title
        //-------------------------------------------------
        //
        public string title { get; set; } = "";
        //
        //-------------------------------------------------
        // Report description
        //-------------------------------------------------
        //
        public string description { get; set; } = "";
        //
        //-------------------------------------------------
        // Report guid
        //-------------------------------------------------
        //
        public string guid { get; set; } = "";
        //
        public ICollection<AdminFrameworkReportColumnClass> column { get; set; }
    }

}