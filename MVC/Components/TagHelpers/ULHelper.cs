using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;

namespace MVC.Components.TagHelpers
{
    public static class ULHelper
    {
        // TODO
        public static HtmlString CreateUnorderedList(this IHtmlHelper html,
            Dictionary<int, string> items, string id, string listClass, string listItemClass, string listItemName)
        {
            TagBuilder ul = new("ul");
            ul.Attributes.Add("id", id);
            ul.Attributes.Add("class", listClass);

            TagBuilder li;
            foreach (var pair in items)
            {
                li = new("li");
                li.Attributes.Add("class", listItemClass);
                li.Attributes.Add("asp-for", $"Model.{listItemName}[{pair.Key}].Name");
                li.InnerHtml.Append(pair.Value);

                ul.InnerHtml.AppendHtml(li);
            }

            StringWriter writer = new();
            ul.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}
