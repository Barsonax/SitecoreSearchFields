using System;
using System.IO;
using System.Web.UI;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Control = Sitecore.Web.UI.HtmlControls.Control;

namespace SitecoreSearchFields.Base.FieldTypes
{
    public abstract class CustomFieldBase : Control, IContentField
    {
        public string Source
        {
            get => GetViewStateString(nameof(Source));
            set => SetViewStateString(nameof(Source), value);
        }

        public string ItemID
        {
            get => GetViewStateString(nameof(ItemID));
            set => SetViewStateString(nameof(ItemID), value);
        }

        public string ItemLanguage
        {
            get => GetViewStateString(nameof(ItemLanguage));
            set => SetViewStateString(nameof(ItemLanguage), value);
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        protected void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

        protected bool CanWrite()
        {
            var item = Sitecore.Context.ContentDatabase.GetItem(Sitecore.Data.ID.Parse(ItemID), Language.Parse(ItemLanguage));
            return item?.Access.CanWrite() ?? false;
        }

        protected void UpdateValue(string newValue, Action<HtmlTextWriter> viewUpdate)
        {
            if (!CanWrite())
            {
                SheerResponse.Alert("You do not have write permission on this item");
                return;
            }
            string oldValue = Value;
            if (oldValue != newValue)
            {
                SetModified();
                Value = newValue;
                HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
                viewUpdate.Invoke(output);
                SheerResponse.SetInnerHtml(ID, output.InnerWriter.ToString());
            }
        }
    }
}