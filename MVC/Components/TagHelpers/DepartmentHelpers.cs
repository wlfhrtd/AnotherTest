﻿using Domain.Models;
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

        #region TRASH
        //public static HtmlString CreateCollectionViewForSingle(this IHtmlHelper html, Department department)
        //{
        //    StringBuilder sb = new(2048);

        //    sb.Append(Recursion(department));

        //    return new HtmlString(sb.ToString());
        //}

        //private static string Recursion(Department department)
        //{
        //    StringBuilder stringBuilder = new(2048);

        //    stringBuilder.AppendLine("<li>");
        //    stringBuilder.AppendLine($"<span id=\"{department.Name}\" class=\"DepartmentName\">");
        //    stringBuilder.AppendLine(department.Name);
        //    stringBuilder.AppendLine("</span>");
        //    stringBuilder.AppendLine($"<span id=\"{department.Name + "Status"}\" class=\"DepartmentStatus\">");
        //    stringBuilder.AppendLine(department.Status.ToString());
        //    stringBuilder.AppendLine("</span>");
        //    stringBuilder.AppendLine("<ul>");

        //    if (department.Subdepartments.Count != 0)
        //    {
        //        foreach (var item in department.Subdepartments)
        //        {
        //            stringBuilder.AppendLine(Recursion(item));
        //        }

        //    }

        //    stringBuilder.AppendLine("</ul>");
        //    stringBuilder.AppendLine("</li>");

        //    return stringBuilder.ToString().TrimEnd();
        //}
        #endregion
    }
}
