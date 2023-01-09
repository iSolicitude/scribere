namespace KOM.Scribere.Web.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
    public static IEnumerable<SelectListItem> GetEnumNameSelectList<TEnum>(this IHtmlHelper htmlHelper)
        where TEnum : struct
    {
        var selectList = new List<SelectListItem>();
        var enumType = typeof(TEnum);

        foreach (var value in Enum.GetValues(enumType))
        {
            string name = value.ToString();

            var selectListItem = new SelectListItem
            {
                Text = enumType.GetField(name).GetCustomAttribute<DisplayAttribute>()?.Name ?? name,
                Value = name,
            };

            selectList.Add(selectListItem);
        }

        return selectList;
    }
}