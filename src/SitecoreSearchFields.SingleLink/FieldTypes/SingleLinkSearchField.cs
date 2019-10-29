using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base.FieldTypes;
using SitecoreSearchFields.Base.Utilities;

namespace SitecoreSearchFields.SingleLink.FieldTypes
{
    public class SingleLinkSearchField : CustomFieldBase
    {
        public const string ItemIdParameter = "itemid";

        private const string DivStyle = "box-sizing: border-box;" +
                                     "display: block;" +
                                     "width: 100 %;" +
                                     "min-height: 34px;" +
                                     "padding: 8px 12px;" +
                                     "font-size: 12px;" +
                                     "line-height: 1.42857143;" +
                                     "color: #474747;" +
                                     "background-color: #ffffff;" +
                                     "background-image: none;" +
                                     "border: 1px solid #cccccc;" +
                                     "border-radius: 2px;" +
                                     "box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);" +
                                     "transition: border-color ease -in-out .15s, box-shadow ease -in-out .15s;";

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
                    UpdateValue(string.Empty);
                    return;
                case "linkfield:open":
                    if (Sitecore.Data.ID.TryParse(Value, out ID result))
                    {
                        ContentEditorUtils.OpenItemInTab(result, ItemLanguage);
                    }
                    return;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            output.Write($"<div{this.ControlAttributes}style=\"{DivStyle}\">");
            RenderItem(output, Value, ItemLanguage, ID);
            output.Write("</div>");
        }

        private void Search(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult && Value.Equals(args.Result) == false)
                {
                    string newValue = args.Result;
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

        public static void RenderItem(HtmlTextWriter output, string value, string language, string controlId)
        {
            if (string.IsNullOrEmpty(value)) return;
            Item obj = Sitecore.Context.ContentDatabase.GetItem(value, Language.Parse(language));
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
                RenderButtons(output, value, controlId);
                imageBuilder.Render(output);
                output.Write(obj.GetUIDisplayName());
                output.Write("</div>");
            }
        }

        private static void RenderButtons(HtmlTextWriter output, string value, string controlId)
        {
            string openEvent = $"linkfield:open({ItemIdParameter}={value}";
            string deleteEvent = $"linkfield:delete({ItemIdParameter}={value}";

            RenderButton(output, openEvent, "Office/16x16/magnifying_glass.png", controlId);
            RenderButton(output, deleteEvent, "Office/16x16/delete.png", controlId);

            output.Write("|");
        }

        private static void RenderButton(HtmlTextWriter output, string clickEvent, string imageSrc, string controlId)
        {
            ImageBuilder imageBuilder = new ImageBuilder();
            imageBuilder.Width = 16;
            imageBuilder.Height = 16;
            imageBuilder.Margin = "0px 8px 0px 0px";
            imageBuilder.Align = "absmiddle";
            imageBuilder.Src = imageSrc;
            imageBuilder.Style = "cursor: pointer;";
            imageBuilder.OnClick = $"javascript:return scForm.postEvent(this,event,'{clickEvent},id={controlId})')";

            output.Write("<span>");
            imageBuilder.Render(output);
            output.Write("</span>");
        }

        private void UpdateValue(string newValue)
        {
            UpdateValue(newValue, output => RenderItem(output, Value, ItemLanguage, ID));
        }
    }
}
