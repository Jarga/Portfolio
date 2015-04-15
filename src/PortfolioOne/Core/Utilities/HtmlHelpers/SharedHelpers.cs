using Microsoft.AspNet.Mvc.Rendering;
using System;

namespace PortfolioOne.Core.Utilities.HtmlHelpers
{
    public static class SharedHelpers
    {
        public static bool IsDebug(this IHtmlHelper helper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}