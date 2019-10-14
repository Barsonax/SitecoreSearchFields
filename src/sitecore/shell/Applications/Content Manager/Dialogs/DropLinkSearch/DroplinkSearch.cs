using System;
using System.Web;
using Sitecore.Web;

namespace SitecoreSearchFields.sitecore.shell.Applications.Content_Manager.Dialogs.DropLinkSearch
{
    public class DroplinkSearch : Sitecore.Web.UI.Pages.DialogForm
    {
        public Sitecore.Web.UI.HtmlControls.Edit ItemLink { get; set; }
        public Sitecore.Web.UI.HtmlControls.Frame Search { get; set; }

        protected string Id => WebUtil.ExtractUrlParm("id", HttpContext.Current.Request.Url.Query);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var sourceUri = $"/sitecore/shell/Applications/Buckets/DropLinkUI.aspx?id={Id}";
            Search.SourceUri = sourceUri;
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Sitecore.Web.UI.Sheer.SheerResponse.SetDialogValue(ItemLink.Value);
            base.OnOK(sender, args);
        }
    }
}