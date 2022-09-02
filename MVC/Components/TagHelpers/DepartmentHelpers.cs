using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;

namespace MVC.Components.TagHelpers
{
    public static class DepartmentHelpers
    {
        public static HtmlString CreateUnorderedList(this IHtmlHelper html,
            string ulId, string ulClass, List<string> items,
            string listItemClass, string hiddenInputName, string hiddenInputClass)
        {
            TagBuilder ul = new("ul");
            ul.Attributes.Add("id", ulId);
            ul.Attributes.Add("class", ulClass);

            TagBuilder li;
            TagBuilder hiddenInput;
            for (int i = 0; i < items.Count; i++)
            {
                li = new("li");
                li.Attributes.Add("id", i.ToString());
                li.Attributes.Add("class", listItemClass);

                hiddenInput = new("input");
                hiddenInput.Attributes.Add("type", "hidden");
                hiddenInput.Attributes.Add("name", hiddenInputName + $"[{i}]"); // collection property name + index
                hiddenInput.Attributes.Add("class", hiddenInputClass);
                hiddenInput.Attributes.Add("value", items[i]);

                li.InnerHtml.Append(items[i]);
                li.InnerHtml.AppendHtml(hiddenInput);

                ul.InnerHtml.AppendHtml(li);
            }

            StringWriter writer = new();
            ul.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}
