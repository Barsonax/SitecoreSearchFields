using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base;
using SitecoreSearchFields.Base.FieldTypes;
using SitecoreSearchFields.Base.Utilities;

namespace SitecoreSearchFields.MultiLink.FieldTypes
{
    public class MultiLinkSearchField : CustomFieldBase
    {
        private const string ItemIdParameter = "itemid";

        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
            if (message["id"] != ID)
            {
                return;
            }

            switch (message.Name)
            {
                case "multilinksearch:search":
                    Sitecore.Context.ClientPage.Start(this, nameof(Search));
                    return;
                case "multilinksearch:open":
                    if (Sitecore.Data.ID.TryParse(message[ItemIdParameter], out ID result))
                    {
                        ContentEditorUtils.OpenItemInTab(result);
                    }
                    return;
                case "multilinksearch:delete":
                    var itemid = message[ItemIdParameter];
                    var values = Value.Split('|');
                    var newValue = string.Join("|", values.Where(x => x != itemid));
                    UpdateValue(newValue);
                    return;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            output.Write($"<div id=\"{this.ID}\" class=\"scContentControl scTreelistEx\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
            this.RenderItems(output);
            output.Write("</div>");
        }

        private void Search(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult && Value.Equals(args.Result) == false)
                {
                    var value = Value;
                    var newValue = string.IsNullOrEmpty(value) ? args.Result : $"{value}|{args.Result}";
                    UpdateValue(newValue);
                }
            }
            else
            {
                var source = SourceStringUtils.GetSourceString(ItemID, ItemLanguage, Source);
                var id = source[Constants.IdParameter];
                var persistentFilter = source[Constants.PfilterParameter];

                ContentEditorUtils.ShowSearchDialog(id, persistentFilter);

                args.WaitForPostBack();
            }
        }

        private void UpdateValue(string newValue)
        {
            var oldValue = Value;
            if (oldValue != newValue)
            {
                SetModified();
                Value = newValue;
                HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
                this.RenderItems(output);
                SheerResponse.SetInnerHtml(this.ID, output.InnerWriter.ToString());
            }
        }

        private void RenderItems(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            var values = this.Value.Split('|').Where(x => !string.IsNullOrEmpty(x));
            foreach (var value in values)
            {
                Item obj = Sitecore.Context.ContentDatabase.GetItem(value, Language.Parse(this.ItemLanguage));
                ImageBuilder imageBuilder = new ImageBuilder();
                imageBuilder.Width = 16;
                imageBuilder.Height = 16;
                imageBuilder.Margin = "0px 4px 0px 0px";
                imageBuilder.Align = "absmiddle";
                if (obj == null)
                {
                    imageBuilder.Src = "Applications/16x16/forbidden.png";
                    output.Write("<div>");
                    imageBuilder.Render(output);
                    output.Write(Translate.Text("Item not found: {0}", (object)HttpUtility.HtmlEncode(value)));
                    output.Write("</div>");
                }
                else
                {
                    imageBuilder.Src = obj.Appearance.Icon;
                    output.Write("<div title=\"" + obj.Paths.ContentPath + "\">");
                    RenderButtons(output, value);
                    imageBuilder.Render(output);
                    output.Write(obj.GetUIDisplayName());
                    output.Write("</div>");
                }
            }
        }

        private void RenderButtons(HtmlTextWriter output, string value)
        {
            var openEvent = $"multilinksearch:open({ItemIdParameter}={value}";
            var deleteEvent = $"multilinksearch:delete({ItemIdParameter}={value}";

            RenderButton(output, openEvent, "Office/16x16/magnifying_glass.png");

            RenderButton(output, deleteEvent, "Office/16x16/delete.png");

            output.Write("|");
        }

        private void RenderButton(HtmlTextWriter output, string clickEvent, string imageSrc)
        {
            ImageBuilder imageBuilder = new ImageBuilder();
            imageBuilder.Width = 16;
            imageBuilder.Height = 16;
            imageBuilder.Margin = "0px 8px 0px 0px";
            imageBuilder.Align = "absmiddle";
            imageBuilder.Src = imageSrc;
            imageBuilder.Style = "cursor: pointer;";
            imageBuilder.OnClick =
                $"javascript:return scForm.postEvent(this,event,'{clickEvent},id={this.ID})')";

            output.Write("<span>");
            imageBuilder.Render(output);
            output.Write("</span>");
        }
    }
}