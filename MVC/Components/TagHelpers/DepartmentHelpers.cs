using Domain.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;

namespace MVC.Components.TagHelpers
{
    public static class DepartmentHelpers
    {
        public static HtmlString CreateSpanDepartment(this IHtmlHelper html,
            string id, string spanClass, string text)
        {
            TagBuilder span = new("span");
            span.Attributes.Add("id", id);
            span.Attributes.Add("class", spanClass);
            span.InnerHtml.Append(text);

            StringWriter writer = new();
            span.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

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

        public static HtmlString CreateCollectionViewForSingle(this IHtmlHelper html, Department department)
        {
            StringWriter writer = new();
            Recursion(department).WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }

        private static TagBuilder Recursion(Department department)
        {
            TagBuilder li = new("li");

            TagBuilder spanName = new("span");
            spanName.Attributes.Add("id", department.Name);
            spanName.Attributes.Add("class", "DepartmentName");
            spanName.InnerHtml.Append(department.Name);

            li.InnerHtml.AppendHtml(spanName);

            TagBuilder spanStatus = new("span");
            spanStatus.Attributes.Add("id", department.Name + "Status");
            spanStatus.Attributes.Add("class", "DepartmentStatus");
            spanStatus.InnerHtml.Append(department.Status.ToString());

            li.InnerHtml.AppendHtml(spanStatus);

            TagBuilder ul = new("ul");

            if (department.Subdepartments.Count != 0)
            {
                foreach (var item in department.Subdepartments)
                {
                    ul.InnerHtml.AppendHtml(Recursion(item));
                }
            }

            li.InnerHtml.AppendHtml(ul);

            return li;
        }
    }
}
