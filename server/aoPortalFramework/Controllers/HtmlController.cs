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
            if (!string.IsNullOrEmpty(request.warning)) {
                result += "<div id=\"afwWarning\">" + request.warning + "</div>";
            }
            string userErrors = cp.UserError.GetList();
            if (!string.IsNullOrEmpty(userErrors)) {
                result += "<div id=\"afwWarning\">" + userErrors + "</div>";
            }
            if (!string.IsNullOrEmpty(request.description )) {
                result += "<p id=\"afwDescription\">" + request.description + "</p>";
            }
            if (!string.IsNullOrEmpty(request.csvDownloadFilename)) {
                result += "<p id=\"afwDescription\"><a href=\"" + cp.Http.CdnFilePathPrefix + request.csvDownloadFilename + "\">Click here</a> to download the data.</p>";
            }
            string resultBody = request.body;
            if (!string.IsNullOrEmpty(request.htmlLeftOfBody)) {
                resultBody = ""
                    + "<div class=\"afwLeftSideHtml\">" + request.htmlLeftOfBody + "</div>"
                    + "<div class=\"afwRightSideHtml\">" + resultBody + "</div>"
                    + "<div style=\"clear:both\"></div>"
                    + "";
            }
            if (!string.IsNullOrEmpty(request.htmlBeforeBody )) { resultBody = "<div class=\"afwBeforeHtml\">" + request.htmlBeforeBody + "</div>" + resultBody; }
            if (!string.IsNullOrEmpty(request.htmlAfterBody )) { resultBody += "<div class=\"afwAfterHtml\">" + request.htmlAfterBody + "</div>"; }
            result += resultBody;
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
            if (request.includeForm && !request.blockFormTag) {
                string action = !string.IsNullOrEmpty(request.formActionQueryString) ? request.formActionQueryString : (!string.IsNullOrEmpty(request.refreshQueryString) ? request.refreshQueryString : cp.Doc.RefreshQueryString); 
                result = cp.Html.Form(result + request.hiddenList, "", "", "", action, "");
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
    }
    //
    public class HtmlDocRequest {
        /// <summary>
        /// The body of the document
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// if not empty, this download will be included below the description
        /// </summary>
        public string csvDownloadFilename { get; set; }
        /// <summary>
        /// if text description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// an optional warning at the top
        /// </summary>
        public string warning { get; set; }
        /// <summary>
        /// the document headline
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// if true, include padding around the doc (but not the buttons)
        /// </summary>
        public bool includeBodyPadding { get; set; }
        /// <summary>
        /// A list of htm tags that will be placed in the button sections
        /// </summary>
        public string buttonList { get; set; }
        /// <summary>
        /// a list of html tags that will be placed at the end of the form
        /// </summary>
        public string hiddenList { get; set; }
        /// <summary>
        /// if true a form will be added
        /// </summary>
        public bool includeForm { get; set; }
        /// <summary>
        /// if true, background color will be added
        /// </summary>
        public bool includeBodyColor { get; set; }
        /// <summary>
        /// the querystring to be added to the optional form
        /// </summary>
        public string formActionQueryString { get; set; }
        /// <summary>
        /// the querystring that will be used as the basis for links to the view
        /// </summary>
        public string refreshQueryString { get; set; }    
        /// <summary>
        /// if true, the outer htmlid, styles and javascript will be added
        /// </summary>
        public bool isOuterContainer { get; set; }
        /// <summary>
        /// html elements that will be displayed to the left of the body
        /// </summary>
        public string htmlLeftOfBody { get; set; }
        /// <summary>
        /// html elements that will be displayed before the body
        /// </summary>
        public string htmlBeforeBody { get; set; }
        /// <summary>
        /// html elements that will be displayed after the body
        /// </summary>
        public string htmlAfterBody { get; set; }
        /// <summary>
        /// if true, the form tag will not be added
        /// </summary>
        public bool blockFormTag { get; set; }

    }
}

