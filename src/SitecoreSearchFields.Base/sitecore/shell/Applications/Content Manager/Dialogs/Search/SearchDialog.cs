using System;
using System.Web;

namespace SitecoreSearchFields.Base.sitecore.shell.Applications.Content_Manager.Dialogs.Search
{
    public class SearchDialog : Sitecore.Web.UI.Pages.DialogForm
    {
        public Sitecore.Web.UI.HtmlControls.Edit ItemLink { get; set; }
        public Sitecore.Web.UI.HtmlControls.Frame Search { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var sourceUri = $"/sitecore/shell/Applications/Buckets/SearchView.aspx?{HttpContext.Current.Request.Url.Query}";
            Search.SourceUri = sourceUri;
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Sitecore.Web.UI.Sheer.SheerResponse.SetDialogValue(ItemLink.Value);
            base.OnOK(sender, args);
        }
    }
}