using System;
using System.Web;
using System.Web.UI;
using Sitecore.Buckets.Client.sitecore_modules.Shell.Sitecore.Buckets;
using Sitecore.Buckets.Pipelines.UI.ExpandIdBasedSearchFilters;
using Sitecore.Buckets.Util;
using Sitecore.Diagnostics;
using Sitecore.sitecore.admin;
using Sitecore.Data.Items;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.Data;

namespace SitecoreSearchFields.Base.sitecore.shell.Applications.Buckets
{
    public partial class SearchView : AdminPage
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
            var parameters = HttpContext.Current.Request.QueryString;

            var id = parameters[Constants.IdParameter];
            var contentDatabase = Sitecore.Context.ContentDatabase;
            var text = string.Empty;
            try
            {
                if (contentDatabase == null)
                    return;
                var obj = contentDatabase.GetItem(id);

                _filter = GetDefaultFilter(obj, parameters);
                _globalSearchFilter = GetPersistentFilter(obj, parameters);
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
                    $"<script type='text/javascript' language='javascript'>window.SC = window.SC || {{}}; SC.baseItemIconPath = '{HttpUtility.JavaScriptStringEncode(text)}'; var filterForSearch='{HttpUtility.JavaScriptStringEncode(_filter)}';var filterForAllSearch='{HttpUtility.JavaScriptStringEncode(_globalSearchFilter)}';</script>");
            }
        }

        private static string GetDefaultFilter(Item obj, NameValueCollection parameters)
        {
            var defaultFilter = parameters[Constants.DfilterParameter];
            var defaultBucketFilter = obj?.Fields[Sitecore.Buckets.Util.Constants.DefaultQuery];
            var filter = JoinIgnoreEmpty(defaultBucketFilter?.Value, defaultFilter);

            return !string.IsNullOrWhiteSpace(filter) ? ExpandIdBasedSearchFilter(obj.Database, filter) : string.Empty;
        }

        private static string GetPersistentFilter(Item obj, NameValueCollection parameters)
        {
            var persistentFilter = parameters[Constants.PfilterParameter];
            var persistentBucketFilter = obj?.Fields[Sitecore.Buckets.Util.Constants.DefaultFilter];
            var filter = JoinIgnoreEmpty(persistentBucketFilter?.Value, persistentFilter);

            return !string.IsNullOrWhiteSpace(filter) ? ExpandIdBasedSearchFilter(obj.Database, filter) : string.Empty;
        }

        private static string ExpandIdBasedSearchFilter(Database database, string filter)
        {
            var args1 = new ExpandIdBasedSearchFiltersArgs(filter, database);
            ExpandIdBasedSearchFiltersPipeline.Run(args1);
            return args1.ExpandedFilters;
        }

        private static string JoinIgnoreEmpty(params string[] args)
        {
            return string.Join(";", args.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        protected void AddToHead(string content)
        {
            ((ItemBucketsSearchResult)Master).DynamicHeadPlaceholder.Controls.Add(new LiteralControl(content));
        }
    }
}