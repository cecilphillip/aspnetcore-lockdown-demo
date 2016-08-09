namespace DayCare.Web.TagHelpers
{
    using System.Collections.Generic;
    using System.Text;
    using Models;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.AspNetCore.Http;

    [HtmlTargetElement("timeline")]
    public class TimeLineTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _ctxAccessor;

        private const string ListTimeTemplate =
        @"<li class=""{3}"">
            <div class=""timeline-badge light-blue accent-3""><i class=""material-icons dp48"">comment</i></div>
            <div class=""timeline-panel amber lighten-4"">
                <div class=""timeline-heading"">
                    <span class=""timeline-title"">{0}</span>
                    <p><small><i class=""material-icons dp48"">schedule</i> {1}</small></p>
                </div>
                <div class=""timeline-body"">
                    <p>{2}</p>
                </div>
            </div>
        </li>";

        public IEnumerable<ChildActivity> Activities { get; set; }

        public TimeLineTagHelper(IHttpContextAccessor ctxAccessor)
        {
            _ctxAccessor = ctxAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.TagMode = TagMode.StartTagAndEndTag;

            bool invert = false;
            var sb = new StringBuilder();

            foreach (var childActivity in Activities)
            {
                sb.AppendFormat(ListTimeTemplate,
                    childActivity.Title,
                    childActivity.Ocurred.ToString("g"),
                    childActivity.Notes,
                    invert ? "timeline-inverted" : "");

                invert = !invert;
            }

            output.Attributes.SetAttribute("class", "timeline");
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
