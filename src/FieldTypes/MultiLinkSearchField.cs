using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Utilities;
using Control = Sitecore.Web.UI.HtmlControls.Control;

namespace SitecoreSearchFields.FieldTypes
{
    public class MultiLinkSearchField : Control, IContentField
    {
        private const string ItemIdParameter = "itemid";

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
                        Item obj = Sitecore.Context.ContentDatabase.GetItem(result);
                        Sitecore.Context.ClientPage.SendMessage(this, $"contenteditor:launchtab(url={obj.ID})");
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

                var url = Sitecore.UIUtil.GetUri("control:DroplinkSearch", $"id={id}&{Constants.PfilterParameter}={persistentFilter}");

                SheerResponse.ShowModalDialog(url, "1300", "700", "", true);
                args.WaitForPostBack();
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            output.Write($"<div id=\"{this.ID}\" class=\"scContentControl scTreelistEx\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
            this.RenderItems(output);
            output.Write("</div>");
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

        protected void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }
    }
}