using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EWork.Views.Admin
{ 
    public static class AdminNavPages
    {
        public static string Index => "AllJobs";

        public static string Users => "Users";
        public static string OpenedDisputes => "OpenedDisputes";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string OpenedDisputesNavClass(ViewContext viewContext) => PageNavClass(viewContext, OpenedDisputes);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
