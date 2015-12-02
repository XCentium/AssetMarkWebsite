<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private void BindData()
	{

		IEnumerable<Item> ePictures = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Preview Link") && !string.IsNullOrWhiteSpace(item.GetText("Body")));

		if (ePictures.Count() > 0)
		{
			rImages.DataSource = ePictures;
			rImages.ItemDataBound += new RepeaterItemEventHandler(rImages_ItemDataBound);
			rImages.DataBind();


		}
		else
		{
			dColumns.Visible = false;
		}

	}

	void rImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem = e.Item.DataItem as Item;
		HtmlGenericControl dGC;
		Literal lBody;
		
		if (oItem != null)
		{
			//get controls
			lBody = (Literal)e.Item.FindControl("lBody");
			
			lBody.Text = oItem.GetText("Body");
			if (e.Item.ItemIndex == 0)
			{
				dGC = (HtmlGenericControl)e.Item.FindControl("dGC");
				dGC.Attributes["class"] = string.Concat(dGC.Attributes["class"], " inside");
			}
			
		}

	}
</script>
<div class="gc c9" id="dColumns" runat="server">
	<div class="PracticeManagement">
		<asp:Repeater runat="server" ID="rImages">
			<ItemTemplate>
				<div class="gc c3 " runat="server" id="dGC">
					<div class="section">
						<asp:Literal runat="server" ID="lBody"></asp:Literal>
					</div>
					<div class="clear">
					</div>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>
