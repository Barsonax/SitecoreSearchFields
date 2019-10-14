using System;
using System.Web;
using System.Web.UI;
using Sitecore.Buckets.Client.sitecore_modules.Shell.Sitecore.Buckets;
using Sitecore.Buckets.Pipelines.UI.ExpandIdBasedSearchFilters;
using Sitecore.Buckets.Util;
using Sitecore.Diagnostics;
using Sitecore.sitecore.admin;
using Sitecore.Web;
using SitecoreSearchFields.FieldTypes;

namespace SitecoreSearchFields.sitecore.shell.Applications.Buckets
{
    public partial class DropLinkUI : AdminPage
    {
        private string thisIsWorkBox = string.Empty;
        private string _filter = string.Empty;
        private string _globalSearchFilter = string.Empty;

        protected override void OnInit(EventArgs arguments)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            if (!Sitecore.Context.User.IsAuthenticated)
                CheckSecurity(true);
            base.OnInit(arguments);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Sitecore.Context.Language = RequestHelper.GetLanguageFromRequest();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddToHead($"<script type='text/javascript' language='javascript'>var workBox='{thisIsWorkBox}';</script>");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var query = HttpContext.Current.Request.UrlReferrer?.Query;

            var queryString = query != null ? HttpUtility.ParseQueryString(query) : HttpContext.Current.Request.QueryString;

            var id = queryString[DropLinkSearchField.IdParameter];
            var pfilter = queryString[DropLinkSearchField.PfilterParameter];
            var contentDatabase = Sitecore.Context.ContentDatabase;
            var text = string.Empty;
            try
            {
                if (contentDatabase == null)
                    return;
                var obj = contentDatabase.GetItem(id);
                var defaultQueryField = obj?.Fields[Constants.DefaultQuery];
                if (defaultQueryField != null)
                {
                    var args = new ExpandIdBasedSearchFiltersArgs(defaultQueryField.Value, contentDatabase);
                    ExpandIdBasedSearchFiltersPipeline.Run(args);
                    _filter = args.ExpandedFilters;
                }

                var defaultFilterField = obj?.Fields[Constants.DefaultFilter];
                if (defaultFilterField == null)
                    return;
                var globalFilter = defaultFilterField.Value;
                if (!string.IsNullOrEmpty(pfilter))
                {
                    globalFilter = $"{globalFilter};{pfilter}";
                }
                var args1 = new ExpandIdBasedSearchFiltersArgs(globalFilter, contentDatabase);
                ExpandIdBasedSearchFiltersPipeline.Run(args1);
                _globalSearchFilter = args1.ExpandedFilters;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Resolve Media Source", ex, this);
            }
            finally
            {
                if (!string.IsNullOrEmpty(id) && contentDatabase != null && contentDatabase.GetItem(id) != null)
                    text = contentDatabase.GetItem(id).Appearance.Icon.Replace("/sitecore/shell/themes/Standard", string.Empty);
                AddToHead(
                    $"<script type='text/javascript' language='javascript'>window.SC = window.SC || {{}}; SC.baseItemIconPath = '{WebUtil.EscapeJavascriptString(text)}'; var filterForSearch='{WebUtil.EscapeJavascriptString(_filter)}';var filterForAllSearch='{_globalSearchFilter}';</script>");
            }
        }

        protected void AddToHead(string content)
        {
            ((ItemBucketsSearchResult)Master).DynamicHeadPlaceholder.Controls.Add(new LiteralControl(content));
        }
    }
}