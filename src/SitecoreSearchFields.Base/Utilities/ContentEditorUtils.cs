using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base.sitecore.shell.Applications.Content_Manager.Dialogs.Search;

namespace SitecoreSearchFields.Base.Utilities
{
    public static class ContentEditorUtils
    {
        public static void ShowSearchDialog(string id, string persistentFilter)
        {
            var url = Sitecore.UIUtil.GetUri($"control:{nameof(SearchDialog)}", $"id={id}&{Constants.PfilterParameter}={persistentFilter}");

            SheerResponse.ShowModalDialog(url, "1300", "700", "", true);
        }

        public static void OpenItemInTab(ID id, string language)
        {
            Item obj = Sitecore.Context.ContentDatabase.GetItem(id, Language.Parse(language));
            Sitecore.Context.ClientPage.SendMessage(null, $"contenteditor:launchtab(url={obj.ID})");
        }
    }
}