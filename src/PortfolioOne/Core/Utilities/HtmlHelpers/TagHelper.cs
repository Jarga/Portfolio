using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PortfolioOne.Core.Utilities.HtmlHelpers
{
    public class TagHelper
    {
        TagBuilder Tag { get; set; }

        public object Attributes
        {
            get { return Tag.Attributes; }
            set
            {
                if (value != null)
                {
                    Tag.MergeAttributes(ToDictionary(value));
                }
            }
        }

        public string InnerHtml
        {
            get { return Tag.InnerHtml; }
            set { Tag.InnerHtml = value; }
        }

        public TagHelper(string tagName)
        {
            Tag = new TagBuilder(tagName);
        }

        public TagHelper(string tagName, string className)
        {
            Tag = new TagBuilder(tagName);
            Tag.AddCssClass(className);
        }

        public void MergeAttributes(object obj)
        {
            Tag.MergeAttributes(ToDictionary(obj));
        }

        public string ToString(TagRenderMode renderMode = TagRenderMode.Normal)
        {
            return Tag.ToString(renderMode);
        }

        public HtmlString ToHtmlString(TagRenderMode renderMode = TagRenderMode.Normal)
        {
            return new HtmlString(Tag.ToString(renderMode));
        }

        private static IDictionary<string, string> ToDictionary(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] props = type.GetProperties();
            return props.ToDictionary(p => p.Name.Replace("_", "-"), p => p.GetValue(obj).ToString());
        }
    }
}