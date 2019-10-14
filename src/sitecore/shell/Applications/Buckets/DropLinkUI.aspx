<%@ Page Language="C#" MasterPageFile="~/sitecore/shell/Applications/Buckets/ItemBucketsSearchResult.Master" AutoEventWireup="true" CodeBehind="DropLinkUI.aspx.cs" Inherits="SitecoreSearchFields.sitecore.shell.Applications.Buckets.DropLinkUI" %>
<%@ Register TagPrefix="sc" TagName="BucketSearchUI" Src="BucketSearchUI.ascx" %>
<%@ OutputCache Location="None" VaryByParam="none" %> 

<asp:Content ContentPlaceHolderID="head" runat="server">
    <%--TODO: Disabled js include--%>
    <script type="text/javascript" src="./Scripts/BucketLink.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <sc:BucketSearchUI BucketsView="Dialog" runat="server" />
</asp:Content>
