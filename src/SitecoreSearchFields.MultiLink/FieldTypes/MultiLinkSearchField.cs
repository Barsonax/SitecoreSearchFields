using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base.FieldTypes;
using SitecoreSearchFields.Base.Utilities;
using SitecoreSearchFields.SingleLink.FieldTypes;

namespace SitecoreSearchFields.MultiLink.FieldTypes
{
    public class MultiLinkSearchField : CustomFieldBase
    {
        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
            if (message["id"] != ID)
            {
                return;
            }

            switch (message.Name)
            {
                case "linkfield:search":
                    Sitecore.Context.ClientPage.Start(this, nameof(Search));
                    return;
                case "linkfield:delete":
                    string itemid = message[SingleLinkSearchField.ItemIdParameter];
                    string[] values = Value.Split('|');
                    string newValue = string.Join("|", values.Where(x => x != itemid));
                    UpdateValue(newValue);
                    return;
                case "linkfield:open":
                    if (Sitecore.Data.ID.TryParse(message[SingleLinkSearchField.ItemIdParameter], out ID result))
                    {
                        ContentEditorUtils.OpenItemInTab(result, ItemLanguage);
                    }
                    return;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            output.Write($"<div id=\"{ID}\" class=\"scContentControl scTreelistEx\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
            RenderItems(output);
            output.Write("</div>");
        }

        private void Search(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult && Value.Equals(args.Result) == false)
                {
                    string value = Value;
                    string newValue = string.IsNullOrEmpty(value) ? args.Result : $"{value}|{args.Result}";
                    UpdateValue(newValue);
                }
            }
            else
            {
                NameValueCollection parameters = SourceStringUtils.GetSourceString(ItemID, ItemLanguage, Source);
                ContentEditorUtils.ShowSearchDialog(parameters);

                args.WaitForPostBack();
            }
        }

        private void UpdateValue(string newValue)
        {
            UpdateValue(newValue, output => RenderItems(output));
        }

        private void RenderItems(HtmlTextWriter output)
        {
            string[] values = Value.Split('|');
            foreach (string value in values)
            {
                SingleLinkSearchField.RenderItem(output, value, ItemLanguage, ID);
            }
        }
    }
}