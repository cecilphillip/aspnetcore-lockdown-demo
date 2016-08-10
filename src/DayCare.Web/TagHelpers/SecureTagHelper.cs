namespace DayCare.Web.TagHelpers
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System.Linq;
    using System;

    [HtmlTargetElement(Attributes = RoleAttributeName)]
    public class SecureTagHelper : TagHelper
    {
        private const string RoleAttributeName = "secure-roles";

        [HtmlAttributeName(RoleAttributeName)]
        public string Roles { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Roles))
            {
                var user = ViewContext.HttpContext.User;
                var roles = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var allowed = roles.Any(r => user.IsInRole(r));
                if (allowed)
                {
                    base.Process(context, output);
                    return;
                }
            }
            output.SuppressOutput();
        }
    }
}
