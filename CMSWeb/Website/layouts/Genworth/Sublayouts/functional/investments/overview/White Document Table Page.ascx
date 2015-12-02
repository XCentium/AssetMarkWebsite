<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
	
	private List<Item> _contextItem;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			_contextItem = ContextExtension.CurrentItem.GetChildrenAndItemsOfTemplate("Brochure White Paper");
			BindData();
		}
	}

	private void BindData()
	{
		if (_contextItem != null)
		{
			rDocs.DataSource = _contextItem;
			rDocs.DataBind();
		}
		else
		{
			white_document.Visible = false;
		}
	}

	protected void rDocs_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			Item contentItem = e.Item.DataItem as Item;
			var header = e.Item.FindControl("header") as Literal;
			var date = e.Item.FindControl("date") as HtmlTableCell;
			var strategy = e.Item.FindControl("strategy") as HtmlTableCell;
			var body = e.Item.FindControl("body") as HtmlGenericControl;
			var ppt = e.Item.FindControl("ppt") as HyperLink;
			var pdf = e.Item.FindControl("pdf") as HyperLink;
			var linkList = e.Item.FindControl("linkList") as Panel;

			header.Text = contentItem.GetText("title");
			date.InnerText = contentItem.GetField("Timing", "Date").GetDateString("M/dd/yyyy", string.Empty);
			strategy.InnerText = contentItem.GetText("Sub Title");
			body.InnerHtml = contentItem.GetText("body");

			ppt.Visible = contentItem.ConfigureDocumentHyperlink(ppt, "Documents", "PPT") != null;
			pdf.Visible = contentItem.ConfigureDocumentHyperlink(pdf, "Documents", "PDF") != null;

            //Following the shared folder level pattern for files, Omniture tags are applied to these hyperlinks.
            var parent = contentItem.GetParentItems().Where(p => p.InstanceOfTemplate("Web Page")).FirstOrDefault();
            if (parent != null)
            {
                contentItem.ConfigureOmnitureControl(parent, ppt);
                contentItem.ConfigureOmnitureControl(parent, pdf);
            }
            
			linkList.Visible = ppt.Visible || pdf.Visible;
		}
	}
</script>

<div class="white_document_page" id="white_document" runat="server">
	<sc:Placeholder runat="server" Key="content" />

	<asp:Repeater runat="server" ID="rDocs" OnItemDataBound="rDocs_ItemDataBound">
		<HeaderTemplate>
			<table class="white_document_table">
				<thead>
					<tr>
						<th>Title</th>
						<th>Strategist</th>
						<th>Date</th>
					</tr>
				</thead>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="title">
					<h3>
						<asp:Literal runat="server" ID="header" />
					</h3>
					<div runat="server" id="body" class="body html"></div>
					<asp:Panel runat="server" ID="linkList" CssClass="linkList">
						<asp:HyperLink runat="server" ID="ppt"><img src="/CMSContent/Images/DocumentIcons/ppt_icon.png" /> Download PPT</asp:HyperLink>
						<asp:HyperLink runat="server" ID="pdf"><img src="/CMSContent/Images/DocumentIcons/pdf_icon.png" /> Download PDF</asp:HyperLink>
					</asp:Panel>
				</td>
				<td runat="server" class="strategist" id="strategy" />
				<td runat="server" class="date" id="date" />
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>
</div>
