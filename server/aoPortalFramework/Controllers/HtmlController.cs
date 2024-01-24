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
        [Obsolete("Use getRandomHtmlId with no argument.", false)]
        public static string getRandomHtmlId(CPBaseClass cp) {
            return getRandomHtmlId();
        }
        //
        public static string getRandomHtmlId() {
            return Guid.NewGuid().ToString().Replace("{", "").Replace("-", "").Replace("}", "").ToLowerInvariant();
        }
        //
        /// <summary>
        /// Create a button with a link that does not submit a form. When clicked it anchors to the link
        /// </summary>
        /// <param name="buttonCaption"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static string a(string buttonCaption, string link) {
            return a(buttonCaption,link, "", "");
        }
        //
        /// <summary>
        /// Create a button with a link that does not submit a form. When clicked it anchors to the link
        /// </summary>
        /// <param name="buttonCaption"></param>
        /// <param name="link"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public static string a(string buttonCaption, string link, string htmlId) {
            return a(buttonCaption, link, htmlId, "");
        }
        //
        /// <summary>
        /// Create a button with a link that does not submit a form. When clicked it anchors to the link
        /// </summary>
        /// <param name="buttonCaption"></param>
        /// <param name="link"></param>
        /// <param name="htmlId"></param>
        /// <param name="htmlClass"></param>
        /// <returns></returns>
        public static string a(string buttonCaption, string link, string htmlId, string htmlClass) {
            return "<a href=\"" + link + "\" id=\"" + htmlId + "\" class=\"btn btn-primary mr-1 me-1 btn-sm " + htmlClass + "\">" + buttonCaption + "</a>";
        }
        //
        /// <summary>
        /// Create a form button that submits a form.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="buttonValue"></param>
        /// <param name="buttonId"></param>
        /// <param name="buttonClass"></param>
        /// <returns></returns>
        public static string getButton(string buttonName, string buttonValue, string buttonId, string buttonClass) {
            //afwButton 
            return "<input type=\"submit\" name=\"" + buttonName + "\" value=\"" + buttonValue + "\" id=\"" + buttonId + "\" class=\"btn btn-primary mr-1 me-1 btn-sm " + buttonClass + "\">";
        }
        //
        public static string getButtonBar(List<string> buttons) {
            if (buttons.Count == 0) { return ""; }
            StringBuilder result = new StringBuilder();
            foreach (var button in buttons) {
                result.Append(button);
            }
            return "<div class=\"border-bottom bg-white p-2\">" + result.ToString() + "</div>";
        }
        //
        public static string getButtonSection(string buttons) {
            if (string.IsNullOrEmpty(buttons)) { return ""; }
            return "<div class=\"border-bottom bg-white p-2\">" + buttons + "</div>";
        }
        //
        public static string getReportDoc(CPBaseClass cp, HtmlDocRequest request) {
            string result = "";
            //
            string warningMessage = request.warningMessage;
            string userErrors = cp.Utils.ConvertHTML2Text(cp.UserError.GetList());
            if (!string.IsNullOrWhiteSpace(userErrors)) {
                warningMessage += userErrors;
            }
            //
            result += (string.IsNullOrWhiteSpace(request.title) ? "" : Constants.cr + "<h2>" + request.title + "</h2>");
            result += (string.IsNullOrWhiteSpace(request.successMessage) ? "" : Constants.cr + "<div class=\"p-3 mb-2 bg-success text-white\">" + request.successMessage + "</div>");
            result += (string.IsNullOrWhiteSpace(request.infoMessage) ? "" : Constants.cr + "<div class=\"p-3 mb-2 bg-info text-white\">" + request.infoMessage + "</div>");
            result += (string.IsNullOrWhiteSpace(request.warningMessage) ? "" : Constants.cr + "<div class=\"p-3 mb-2 bg-warning text-dark\">" + warningMessage + "</div>");
            result += (string.IsNullOrWhiteSpace(request.failMessage) ? "" : Constants.cr + "<div class=\"p-3 mb-2 bg-danger text-white\">" + request.failMessage + "</div>");
            result += (string.IsNullOrWhiteSpace(request.description) ? "" : Constants.cr + "<p>" + request.description + "</p>");
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
            if (!string.IsNullOrEmpty(request.htmlBeforeBody)) { resultBody = "<div class=\"afwBeforeHtml\">" + request.htmlBeforeBody + "</div>" + resultBody; }
            if (!string.IsNullOrEmpty(request.htmlAfterBody)) { resultBody += "<div class=\"afwAfterHtml\">" + request.htmlAfterBody + "</div>"; }
            result += resultBody;
            //
            // -- add padding
            if (request.includeBodyPadding) {
                result = cp.Html.div(result, "", "m-4", "");
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
                result = cp.Html.div(result, "", "bg-light", "");
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
        // 
        /// <summary>
        /// message displayed as a success message
        /// </summary>
        public string successMessage { get; set; } = "";
        // 
        /// <summary>
        /// message displayed as an informational message. Not a warning and not success, but something happened that the user needs to know about the results.
        /// </summary>
        public string infoMessage { get; set; } = "";
        // 
        /// <summary>
        /// message displayed as a warning message. Not an error, but an issue of some type
        /// </summary>
        public string warningMessage { get; set; } = "";
        // 
        /// <summary>
        /// message displayed as a fail message. The data is not correct
        /// </summary>
        public string failMessage { get; set; } = "";

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
        //
        [Obsolete("deprecated. Use warningMessage instead", false)]
        public string warning { get; set; }
    }
}

