using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.PortalFramework.Controllers {
    class HtmlController {
        //
        public static string getRandomHtmlId( CPBaseClass cp ) {
            return Guid.NewGuid().ToString().Replace("{", "").Replace("-", "").Replace("}", "").ToLowerInvariant();
        }
        //
        public static string  getButton(string buttonName, string buttonValue, string buttonId, string buttonClass) {
            return "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"afwButton " + buttonClass + "\">";
        }
    }
}
