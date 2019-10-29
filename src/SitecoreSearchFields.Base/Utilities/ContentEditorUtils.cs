using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base.sitecore.shell.Applications.Content_Manager.Dialogs.Search;
using System.Collections.Specialized;

namespace SitecoreSearchFields.Base.Utilities
{
    public static class ContentEditorUtils
    {
        public static void ShowSearchDialog(NameValueCollection parameters)
        {
            var url = Sitecore.UIUtil.GetUri($"control:{nameof(SearchDialog)}", parameters.ToString());

            SheerResponse.ShowModalDialog(url, "1300", "700", "", true);
        }

        public static void OpenItemInTab(ID id, string language)
        {
            Item item = Sitecore.Context.ContentDatabase.GetItem(id, Language.Parse(language));
            if (item == null)
            {
                SheerResponse.Alert($"Item with {id} was not found");
                return;
            }

            if (!item.Access.CanRead())
            {
                SheerResponse.Alert("You do not have read permission on this item");
                return;
            }
            Sitecore.Context.ClientPage.SendMessage(null, $"contenteditor:launchtab(id={item.ID},la={language})");
        }
    }
}