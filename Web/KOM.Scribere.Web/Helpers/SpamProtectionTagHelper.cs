using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace iSolicitude.Web.Helpers;

[HtmlTargetElement("form")]
public class SpamProtectionTagHelper : TagHelper
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Method { get; set; }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Method != null && !string.Equals(Method, "get", StringComparison.OrdinalIgnoreCase))
        {
            output.PostContent.AppendHtml("<input type=\"text\" style=\"display:inline;height:1px;left:-10000px;overflow:hidden;position:absolute;width:1px;\" name=\"Website\" value=\"\" />");
            output.PostContent.AppendHtml($"<input type=\"hidden\" name=\"SpamProtectionTimeStamp\" value=\"{DateTimeOffset.Now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)}\" />");
        }

        return Task.CompletedTask;
    }
}