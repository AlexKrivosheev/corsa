using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Corsa.Infrastructure
{
    public static class WebHelper
    {

        private static Dictionary<string, string> _controllerVocabulary = new Dictionary<string, string>
        {
            {"Home","Home"}

        };

        private static Dictionary<string, string> _actionVocabulary = new Dictionary<string, string>
        {
            {"Index",""},
            {"Edit","Editing of "},
            {"Create","Editing of "}
        };

        private static readonly HtmlContentBuilder _emptyBuilder = new HtmlContentBuilder();

        public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper, string title)
        {          
            string controllerName = helper.ViewContext.RouteData.Values["controller"].ToString();
            string actionName = helper.ViewContext.RouteData.Values["action"].ToString();

            if (_controllerVocabulary.ContainsKey(controllerName))
            {
                controllerName = _controllerVocabulary[controllerName];
            }

            if (_actionVocabulary.ContainsKey(actionName))
            {
                actionName = _actionVocabulary[actionName];
            }

            var breadcrumb = new HtmlContentBuilder()
                                .AppendHtml("<ol class='breadcrumb'><li>")
                                .AppendHtml(helper.ActionLink("Project", "Index", "Home"))
                                .AppendHtml("</li><li>")
                                .AppendHtml(helper.ActionLink(controllerName.Titleize(),
                                                          "Index", controllerName))
                                .AppendHtml("</li>");

            if (!string.IsNullOrEmpty(title))
            {
                breadcrumb.AppendHtml("<li>")
                .AppendHtml(title)
                .AppendHtml("</li>");
            }
            
            if (helper.ViewContext.RouteData.Values["action"].ToString() != "Index")
            {
                breadcrumb.AppendHtml("<li>")
                   .AppendHtml(helper.ActionLink(actionName.Titleize(), actionName, controllerName))
                .AppendHtml("</li>");
            }

            return breadcrumb.AppendHtml("</ol>");
        }

        public static byte[] GetFavicon(string url)
        {
            try
            {
                var targeturl = new Uri(url);
                var client = new System.Net.WebClient();
                var builder = new UriBuilder(targeturl.Scheme, targeturl.Host, targeturl.Port, "favicon.ico");
                return client.DownloadData(builder.Uri);
            }
            catch (Exception exc)
            {
                return null;
            }

            return null;
        }
    }
}
