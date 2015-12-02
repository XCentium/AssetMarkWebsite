<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>


<script runat="server">
	private const string Topic = "Topic";
	private string sItemURL;
	private ID oSelectedItem;
	private int iLevel = 1;
	
	
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
		Item oCurrentItem = ContextExtension.CurrentItem;
		string sTopicRequest;
		Guid oItemID;
		
		// get the main topics
		//
		List<Item> oChildrenList = oCurrentItem.GetChildrenOfTemplate("Document Library");
		if (oChildrenList != null && oChildrenList.Count > 0)
		{
			sItemURL = oCurrentItem.GetURL();
			sTopicRequest = Request.QueryString[Topic];
			if (!string.IsNullOrWhiteSpace(sTopicRequest) && Guid.TryParse(sTopicRequest, out oItemID))
			{
				oSelectedItem = new ID(oItemID);
				oCurrentItem = ContextExtension.CurrentDatabase.GetItem(oSelectedItem);
			}
			else
			{
				oSelectedItem = oChildrenList[0].ID;
				oCurrentItem = oChildrenList[0];
			}

			rTopicList.DataSource = oChildrenList;
			rTopicList.ItemDataBound += new RepeaterItemEventHandler(rTopicList_ItemDataBound);
			rTopicList.DataBind();
			if (oCurrentItem != null)
			{
				rDescription.DataSource = oCurrentItem.Axes.GetDescendants().Where(item => item == null ? false : item.InstanceOfTemplate(new string[] { "Video", "Document Base" }));
				rDescription.ItemDataBound += new RepeaterItemEventHandler(rDescription_ItemDataBound);
				rDescription.DataBind();
			}
		}
	}

	void rDescription_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Literal lDescription;
		Literal lTime;
		Image iTopicIcon;
		HtmlTableRow trRow;

		Item oItem = e.Item.DataItem as Item;
		if (oItem != null)
		{
			trRow = (HtmlTableRow)e.Item.FindControl("trRow");
			iTopicIcon = (Image)e.Item.FindControl("iTopicIcon");
			lDescription = (Literal)e.Item.FindControl("lDescription");
			lTime = (Literal)e.Item.FindControl("lTime");
			
			// bind the controls
			//
			if (oItem.InstanceOfTemplate("Video"))
			{
				lDescription.Text = oItem.GetText("Description");
				trRow.Attributes.Add("href", oItem.GetURL(true));
				trRow.Attributes.Add("class", "first inline-video");
				iTopicIcon.ImageUrl = "~/CMSContent/Images/icon_video.png";
			}
			else if (oItem.InstanceOfTemplate("Document Base"))
			{
				lDescription.Text = oItem.DisplayName;
				trRow.Attributes.Add("target", "_blank");
				trRow.Attributes.Add("class", "first inline-video isDocument");
				trRow.Attributes.Add("href", ((FileField)oItem.GetField("Document", "File")).MediaItem.GetMediaURL(String.Empty));
				iTopicIcon.ImageUrl = "~/CMSContent/Images/icon_pdf.png";
			}
			
			int iMinutes;
			if (int.TryParse(oItem.GetText("Length in Minutes"), out iMinutes))
			{
				TimeSpan oMinutes = new TimeSpan(0, iMinutes, 0);
				lTime.Text = oMinutes.ToString("%h\\:mm");
			}
		}

	}

	void rTopicList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HyperLink hTopicItem;
		Repeater rTopicList;
		
		Item oTopic = e.Item.DataItem as Item;
		List<Item> oItemChildren;
		
		
		if (oTopic != null)
		{
			hTopicItem = (HyperLink)e.Item.FindControl("hTopicItem");
			rTopicList = (Repeater)e.Item.FindControl("rTopicList");
			hTopicItem.Text = oTopic.GetText("Title");

			oItemChildren = oTopic.GetChildrenOfTemplate("Document Library");
			if (oSelectedItem == oTopic.ID)
			{
				HtmlGenericControl lList = (HtmlGenericControl)e.Item.FindControl("lList");
				lList.Attributes.Add("class", "selected");
			}
			else
			{
				hTopicItem.NavigateUrl = string.Format("{0}?{1}={2}", sItemURL, Topic, oTopic.ID.ToShortID());
			}
			
			if (oItemChildren.Count > 0)
			{
				iLevel++;
				rTopicList.ItemTemplate = this.rTopicList.ItemTemplate;
				rTopicList.DataSource = oTopic.GetChildrenOfTemplate("Document Library");
				rTopicList.ItemDataBound += new RepeaterItemEventHandler(rTopicList_ItemDataBound);
				rTopicList.DataBind();
			}

		}

	}
</script>

<table width="100%" border="0" cellpadding="0" cellspacing="0">
	<thead>
		<tr>
			<th width="200px" class="first">
				Topic
			</th>
			<th width="30px">
				Type
			</th>
			<th width="*">
				Description
			</th>
			<th width="40px" class="last">
				Length
			</th>
		</tr>
	</thead>
	<tbody>
		<tr class="first">
			<td rowspan="10">
				<!-- START TOPIC LIST -->
				<asp:Repeater ID="rTopicList" runat="server">
					<HeaderTemplate>
						<ul class="topic-list">
							<asp:Literal ID="lUlist" runat="server"></asp:Literal>
					</HeaderTemplate>
					<ItemTemplate>
						<li runat="server" id="lList">
							<asp:HyperLink runat="server" ID="hTopicItem" />
							<asp:Repeater ID="rTopicList" runat="server">
								<HeaderTemplate>
									<ul>
								</HeaderTemplate>
								<FooterTemplate>
									</ul>
								</FooterTemplate>
							</asp:Repeater>
						</li>
					</ItemTemplate>
					<FooterTemplate>
						</ul>
					</FooterTemplate>
				</asp:Repeater>
				<!-- END TOPIC LIST -->
			</td>
			<td colspan="3">
				<!-- START DOCUMENT LIST -->
				<asp:Repeater runat="server" ID="rDescription">
					<HeaderTemplate>
						<table class="document-list" width="100%" border="0" cellpadding="0" cellspacing="0">
					</HeaderTemplate>
					<ItemTemplate>
						<tr id="trRow" runat="server">
							<td width="20px" class="doc-icon">
								<asp:Image ID="iTopicIcon" runat="server" />
							</td>
							<td width="*" class="doc-description">
								<div><asp:Literal ID="lDescription" runat="server" /></div>
							</td>
							<td width="30px" class="doc-length">
								<asp:Literal runat="server" ID="lTime"></asp:Literal>
							</td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
						</table>
					</FooterTemplate>
				</asp:Repeater>
				<!-- END DOCUMENT LIST -->
			</td>
		</tr>
	</tbody>
</table>
