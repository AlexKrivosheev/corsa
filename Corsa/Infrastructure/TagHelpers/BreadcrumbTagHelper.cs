using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Corsa.Infrastructure.TagHelpers
{

    public class BreadCrumbItem
    {
        private string link;

        public BreadCrumbItem(string title, string link, bool active = false)
        {
            this.Title = title;
            this.Link = link;
            this.Active = active;
        }

        public BreadCrumbItem(string title)
        {
            this.Title = title;
            this.Link = string.Empty;
            this.Active = true;
        }


        public string Title { get; set; }

        public string Link { get; set; }

        public bool Active { get; set; }
    }

    


    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("breadcrumb", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbTagHelper : TagHelper
    {
        public BreadCrumbItem[] Items { get; set; } = new BreadCrumbItem[0];
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";

            TagBuilder htmlContentBuilder = new TagBuilder("ol");
            htmlContentBuilder.AddCssClass("breadcrumb");
            htmlContentBuilder.AddCssClass("crs-breadcrumb");

            foreach (var item in Items)
            {
                htmlContentBuilder.InnerHtml.AppendHtml($"<li class='crs-breadcrumb__item breadcrumb-item {(item.Active?"active":"")}'><a href='{item.Link}'>{item.Title}</a></li>");
            }

            output.Content.AppendHtml(htmlContentBuilder);


            output.TagMode = TagMode.StartTagAndEndTag;
        }

    }
}
