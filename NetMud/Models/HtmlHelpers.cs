﻿using NetMud.Data.Architectural.EntityBase;
using NetMud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace NetMud.Models
{
    public static class MvcHtmlHelpers
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            TagBuilder description = GetDescriptionHtml(metadata.Description);

            if (description == null)
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create(description.ToString());
        }

        public static MvcHtmlString DescriptiveLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return DescriptiveLabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DescriptiveLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            TagBuilder description = GetDescriptionHtml(metadata.Description);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (string.IsNullOrWhiteSpace(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (description == null)
            {
                tag.SetInnerText(labelText);
                return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
            }
            else
            {
                TagBuilder labelSpan = new TagBuilder("span");
                labelSpan.SetInnerText(labelText);

                string outputHtml = tag.ToString(TagRenderMode.StartTag)
                                    + labelSpan.ToString(TagRenderMode.Normal)
                                    + description.ToString(TagRenderMode.Normal)
                                    + tag.ToString(TagRenderMode.EndTag);

                return MvcHtmlString.Create(outputHtml);
            }
        }

        public static MvcHtmlString EditorForMany<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable<TValue>>> expression, object additionalViewData, int currentCount = 0, string templateName = "")
        {
            string fieldName = html.NameFor(expression).ToString();
            IEnumerable<TValue> items = expression.Compile()(html.ViewData.Model);

            if (items.Count() == 0)
            {
                items = new List<TValue>()
                {
                    DataUtility.InsantiateThing<TValue>(typeof(EntityTemplatePartial).Assembly)
                };
            }

            string templateNameOverride = string.IsNullOrWhiteSpace(templateName) ? html.ViewData.ModelMetadata.TemplateHint : templateName;
            return MvcHtmlString.Create(string.Concat(items.Select((item, i) =>
                html.EditorFor(m => item, templateNameOverride, string.Format("[{0}]", i + currentCount), additionalViewData))).Replace(Environment.NewLine, "").Replace('\u000A', ' ').Trim());
        }

        public static MvcHtmlString EmptyEditorForMany<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable<TValue>>> expression, object additionalViewData, int currentCount = 0, string templateName = "")
        {
            string fieldName = html.NameFor(expression).ToString();
            List<TValue> plusOne = new List<TValue>
            {
                DataUtility.InsantiateThing<TValue>(typeof(EntityTemplatePartial).Assembly)
            };

            string templateNameOverride = string.IsNullOrWhiteSpace(templateName) ? html.ViewData.ModelMetadata.TemplateHint : templateName;
            return MvcHtmlString.Create(string.Concat(plusOne.Select((item, i) =>
                html.EditorFor(m => item, templateNameOverride, string.Format("[{0}]", i + currentCount), additionalViewData))).Replace(Environment.NewLine, "").Replace('\u000A', ' ').Trim());
        }

        private static TagBuilder GetDescriptionHtml(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            TagBuilder descTag = new TagBuilder("span");
            descTag.AddCssClass("glyphicon glyphicon-question-sign helpTip");
            descTag.Attributes.Add(new KeyValuePair<string, string>("title", description));

            return descTag;
        }
    }
}