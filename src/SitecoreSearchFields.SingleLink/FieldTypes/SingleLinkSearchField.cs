﻿using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using SitecoreSearchFields.Base;
using SitecoreSearchFields.Base.FieldTypes;
using SitecoreSearchFields.Base.Utilities;

namespace SitecoreSearchFields.SingleLink.FieldTypes
{
    public class SingleLinkSearchField : CustomFieldBase
    {
        public string DisplayValue
        {
            get => GetViewStateString(nameof(DisplayValue));
            set
            {
                SheerResponse.SetInnerHtml(this.ID, Translate.Text(value ?? string.Empty));
                SetViewStateString(nameof(DisplayValue), value);
            }
        }

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
                case "singlelinksearch:search":
                    Sitecore.Context.ClientPage.Start(this, nameof(Search));
                    return;
                case "singlelinksearch:clear":
                    if (!string.IsNullOrEmpty(Value))
                    {
                        Value = string.Empty;
                        SetModified();
                        DisplayValue = GetDisplayValue();
                    }
                    return;
                case "singlelinksearch:open":
                    if (Sitecore.Data.ID.TryParse(Value, out ID result))
                    {
                        ContentEditorUtils.OpenItemInTab(result, ItemLanguage);
                    }
                    return;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            var displayValue = GetDisplayValue();
            this.SetWidthAndHeightStyle();
            output.Write($"<div{this.ControlAttributes}style=\"{DivStyle}\">{displayValue}</div>");
            this.RenderChildren(output);
        }

        private void Search(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult && Value.Equals(args.Result) == false)
                {
                    SetModified();
                    Value = args.Result;
                    DisplayValue = GetDisplayValue();
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

        private string GetDisplayValue()
        {
            var item = GetLinkedItem();
            if (item == null)
            {
                var value = Value;
                if (string.IsNullOrEmpty(value)) return null;
                return $"Could not find a item with id: {value}";
            }
            return item.Name;
        }

        private Item GetLinkedItem()
        {
            var value = Value;
            if (!string.IsNullOrEmpty(value))
            {
                var id = new ID(value);
                var item = Sitecore.Context.ContentDatabase.GetItem(id, Language.Parse(ItemLanguage));
                return item;
            }
            return null;
        }
    }
}