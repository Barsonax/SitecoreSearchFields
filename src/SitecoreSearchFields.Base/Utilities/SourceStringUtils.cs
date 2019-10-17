using System;
using System.Collections.Specialized;
using System.Web;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace SitecoreSearchFields.Base.Utilities
{
    public static class SourceStringUtils
    {
        public static NameValueCollection GetSourceString(string itemId, string itemLanguage, string source)
        {
            var current = Sitecore.Context.ContentDatabase.GetItem(new ID(itemId), Language.Parse(itemLanguage));
            var lookupSourceItemsArgs = new GetLookupSourceItemsArgs
            {
                Item = current,
                Source = source
            };
            try
            {
                using (new LongRunningOperationWatcher(1000, "getLookupSourceItems pipeline[item={0}, source={1}]", current.Paths.Path, source))
                {
                    CorePipeline.Run("getLookupSourceItems", lookupSourceItemsArgs);
                }
            }
            catch (Exception ex)
            {
                throw new LookupSourceException(source, ex);
            }

            var parsedSource = HttpUtility.ParseQueryString(lookupSourceItemsArgs.Source.ToLower());
            return parsedSource;
        }
    }
}