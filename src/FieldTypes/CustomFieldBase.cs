using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;

namespace SitecoreSearchFields.FieldTypes
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
    }
}