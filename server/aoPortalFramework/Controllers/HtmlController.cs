using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.PortalFramework.Controllers {
    /// <summary>
    /// Html handling for the entire Portal system.
    /// </summary>
    class HtmlController {
        //
        public static string getRandomHtmlId(CPBaseClass cp) {
            return Guid.NewGuid().ToString().Replace("{", "").Replace("-", "").Replace("}", "").ToLowerInvariant();
        }
        //
        public static string getButton(string buttonName, string buttonValue, string buttonId, string buttonClass) {
            //afwButton 
            return "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"btn btn-primary mr-1 btn-sm " + buttonClass + "\">";
        }
        //
        public static string getButtonBar(List<string> buttons) {
            if (buttons.Count == 0) { return ""; }
            StringBuilder result = new StringBuilder();
            foreach (var button in buttons) {
                result.Append(button);
            }
            return "<div class=\"bg-white p-2\">" + result.ToString() + "</div>";
        }
        //
        public static string getButtonSection(string buttons) {
            if (string.IsNullOrEmpty(buttons)) { return ""; }
            return "<div class=\"bg-white p-2\">" + buttons + "</div>";
        }
        //
        public static string getReportDoc(CPBaseClass cp, HtmlDocRequest request) {

            string result = "";
            if (!string.IsNullOrEmpty(request.title)) {
                result += "<h2 id=\"afwTitle\">" + request.title + "</h2>";
            }
            string userErrors = cp.UserError.GetList();
            if (!string.IsNullOrEmpty(userErrors)) {
                result += "<div id=\"afwWarning\">" + request.warning + "</div>";
            }
            if (!string.IsNullOrEmpty(request.warning)) {
                result += "<div id=\"afwWarning\">" + request.warning + "</div>";
            }
            if (request.description != "") {
                result += "<p id=\"afwDescription\">" + request.description + "</p>";
            }
            if (!string.IsNullOrEmpty(request.csvDownloadFilename)) {
                result += "<p id=\"afwDescription\"><a href=\"" + cp.Http.CdnFilePathPrefix + request.csvDownloadFilename + "\">Click here</a> to download the data.</p>";
            }
            result += request.body;
            //
            // -- add padding
            if (request.includeBodyPadding) {
                result = cp.Html.div(result, "", "afwBodyPad", "");
            };
            //
            // -- add buttons
            if (!string.IsNullOrEmpty(request.buttonList)) {
                result = getButtonSection(request.buttonList) + result + getButtonSection(request.buttonList);
            }
            //
            // -- add form
            if (request.includeForm) {
                result =  cp.Html.Form(result + request.hiddenList, "", "", "", request.formActionQueryString, "");
            }
            //
            // -- add background color
            if (request.includeBodyColor) {
                result = cp.Html.div(result, "", "afwBodyColor", "");
            };
            //
            if (request.isOuterContainer) {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                result = "<div id=\"afw\">" + result + "</div>";
            }
            return result;
        }
        //
        public static string getDoc(CPBaseClass cp, string body, string csvDownloadFilename, string description, string warning, string title, bool includeBodyPadding, string buttonList, string hiddenList, bool includeForm, bool includeBodyColor, string formActionQueryString_Local, bool isOuterContainer) {
            //
            // headers
            if (!string.IsNullOrEmpty(csvDownloadFilename)) {
                body = "<p id=\"afwDescription\"><a href=\"" + cp.Http.CdnFilePathPrefix + csvDownloadFilename + "\">Click here</a> to download the data.</p>" + body;
            }
            if (description != "") {
                body = "<p id=\"afwDescription\">" + description + "</p>" + body;
            }
            if (warning != "") {
                body = "<div id=\"afwWarning\">" + warning + "</div>" + body;
            }
            if (title != "") {
                body = "<h2 id=\"afwTitle\">" + title + "</h2>" + body;
            }
            //
            // -- add padding
            if (includeBodyPadding) {
                body = cp.Html.div(body, "", "afwBodyPad", "");
            };
            //
            // -- add buttons
            if (!string.IsNullOrEmpty(buttonList)) {
                body = HtmlController.getButtonSection(buttonList) + body + HtmlController.getButtonSection(buttonList);
            }
            //
            // -- add form
            if (includeForm) {
                body = cp.Html.Form(body + hiddenList, "", "", "", formActionQueryString_Local, "");
            }
            //
            // -- add background color
            if (includeBodyColor) {
                body = cp.Html.div(body, "", "afwBodyColor", "");
            };
            //
            if (isOuterContainer) {
                cp.Doc.AddHeadJavascript(Properties.Resources.javascript);
                cp.Doc.AddHeadStyle(Properties.Resources.styles);
                body = "<div id=\"afw\">" + body + "</div>";
            }
            return body;
        }
    }
    //
    public class HtmlDocRequest {
        public string body { get; set; }
        public string csvDownloadFilename { get; set; }
        public string description { get; set; }
        public string warning { get; set; }
        public string title { get; set; }
        public bool includeBodyPadding { get; set; }
        public string buttonList { get; set; }
        public string hiddenList { get; set; }
        public bool includeForm { get; set; }
        public bool includeBodyColor { get; set; }
        public string formActionQueryString { get; set; }
        public bool isOuterContainer { get; set; }

    }
}
