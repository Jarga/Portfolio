using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace PortfolioOne.Core.Utilities.HtmlHelpers
{
    public static class FormHelpers
    {
        public static HtmlString CustomTextBoxFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string placeholderText)
        {
            return FormFieldFrom(helper.TextBoxFor(expression, new { placeholder = placeholderText, @class = "form-control" }), helper.ValidationMessageFor(expression));
        }
        public static HtmlString CustomTextAreaFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string placeholderText)
        {
            return FormFieldFrom(helper.TextAreaFor(expression, new { placeholder = placeholderText, @class = "form-control" }), helper.ValidationMessageFor(expression));
        }
        public static HtmlString FormFieldFrom(HtmlString formElement, HtmlString validationElement)
        {
            return new TagHelper("div", "form-group control-group")
            {
                InnerHtml = new TagHelper("div", "controls")
                {
                    InnerHtml = new TagHelper("div", "help-block")
                    {
                        InnerHtml = new TagHelper("ul")
                        {
                            InnerHtml = new TagHelper("li")
                            {
                                InnerHtml = validationElement.ToString()
                            }.ToString()
                        }.ToString()
                    }.ToString() + formElement.ToString()
                }.ToString()
            }.ToHtmlString();
        }
    }
}