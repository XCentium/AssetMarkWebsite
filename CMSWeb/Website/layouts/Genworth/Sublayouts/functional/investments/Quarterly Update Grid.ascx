<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<script runat="server">
	private IEnumerable<string> oQuarterlyTitles;
	private Dictionary<string, Item> oQuarterlyItems;
	private const string sValidExtension = "pdf doc docx ppt pptx";
	private string sDate=string.Empty;
	private string sResearchLink = string.Empty;
    private Item oReseachItem;
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
		Item oItem = ContextExtension.CurrentItem;
		List<Category> oCategories = oItem.GetChildrenOfTemplate("Quarterly Update Category").Select(oItemCategory => new Category(oItemCategory)).ToList();
		oReseachItem = oItem.Parent.Parent.GetChildrenOfTemplate("Research Grid Page").SingleOrDefault();
		if (oReseachItem != null)
			sResearchLink = oReseachItem.GetURL();
		oQuarterlyTitles = oCategories.SelectMany(oCategory => oCategory.Subcategories.SelectMany(oSubcategory => oSubcategory.QuarterlyItems.Select(oQuartely => oQuartely.DisplayName))).Distinct().OrderBy(oResult2 => oResult2);
		rContent.DataSource = oCategories;
		rContent.ItemDataBound += new RepeaterItemEventHandler(rContent_ItemDataBound);
		rContent.DataBind();
	}

	void rContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem rItem = e.Item;
		Category oCategory = rItem.DataItem as Category;
		if (oCategory != null)
		{

			HtmlTableCell tHeader = rItem.FindControl("tHeader") as HtmlTableCell;
			Repeater rHeader = rItem.FindControl("rHeader") as Repeater;
			Repeater rRow = rItem.FindControl("rRow") as Repeater;
			tHeader.InnerText = oCategory.Title;
			rHeader.DataSource = oQuarterlyTitles;
			rHeader.ItemDataBound += new RepeaterItemEventHandler(rHeader_ItemDataBound);
			rHeader.DataBind();
			rRow.DataSource = oCategory.Subcategories;
			rRow.ItemDataBound += new RepeaterItemEventHandler(rRow_ItemDataBound);
			rRow.DataBind();
		}
	}

	void rRow_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem rItem = e.Item;
		SubCategory oSubCategory = rItem.DataItem as SubCategory;
		if (oSubCategory != null)
		{
			HtmlTableCell tSubcategoryTitle = rItem.FindControl("tSubcategoryTitle") as HtmlTableCell;
			Repeater rColumn = rItem.FindControl("rColumn") as Repeater;

			tSubcategoryTitle.InnerText = oSubCategory.Title;
			rColumn.DataSource = oQuarterlyTitles;
			oQuarterlyItems =oSubCategory.QuarterlyItems.ToDictionary(oKey =>oKey.DisplayName);
			rColumn.ItemDataBound += new RepeaterItemEventHandler(rColumn_ItemDataBound);
			rColumn.DataBind();

		}
	}

	void rColumn_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		string sKey =  e.Item.DataItem as string;
		Item oMediaItem;
		PlaceHolder oPlaceHolder;
		if (!string.IsNullOrEmpty(sKey))
		{
			if (!oQuarterlyItems.ContainsKey(sKey))
			{
				((PlaceHolder)e.Item.FindControl("pEmpty")).Visible = true;
				return;
			}
			Item oItem = oQuarterlyItems[sKey];
			Literal lMessage = e.Item.FindControl("lMessage") as Literal;
			
			IEnumerable<Item> oFiles = oItem.GetMultilistItems("Items").Where(oFile => IsValidQuarterLink(oFile));
			
			if (oFiles.Count() > 0)
			{
				DateTime? oDate = oItem.GetField("Date").GetDate();
				if (oDate.HasValue)
				{
					sDate = oDate.Value.ToShortDateString();
				}
				Repeater rFiles = e.Item.FindControl("rFiles") as Repeater;
				rFiles.DataSource = oFiles;
				rFiles.ItemDataBound += new RepeaterItemEventHandler(rFiles_ItemDataBound);
				rFiles.DataBind();
				
			}
			else
			{
			
				oPlaceHolder = e.Item.FindControl("pOther") as PlaceHolder;
				oPlaceHolder.Visible = true;
				if (oItem.GetText("Include View Research") == "1")
				{
					HyperLink hViewResearch = e.Item.FindControl("hViewResearch") as HyperLink;
					hViewResearch.Visible = true;
					hViewResearch.NavigateUrl = string.Format("{0}?{1}", sResearchLink,oItem.GetText("View Research Filter"));
                    oReseachItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hViewResearch);
				}
				string sMessege;
				if (string.IsNullOrEmpty((sMessege = oItem.GetText("Message"))))
				{
					DateTime? oDate = oItem.GetField("Date").GetDate();
					if (oDate.HasValue)
					{
						sMessege = oDate.Value.ToShortDateString();
					}
					else
						sMessege = "TBA";
				}
				lMessage.Text = sMessege;


			}

		}
	}

	void rFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HyperLink hDocument;
		Item oDocumentItem;
		MediaItem oMediaItem;

		oDocumentItem = e.Item.DataItem as Item;

		if (oDocumentItem != null)
		{
			hDocument = e.Item.FindControl("hDocumentLink") as HyperLink;
			
			if (oDocumentItem.InstanceOfTemplate("Link"))
			{
				oDocumentItem.ConfigureHyperlink(hDocument);
				hDocument.Text = oDocumentItem.DisplayName;
				hDocument.CssClass = "htm-icon icon";
			}
			else if (oDocumentItem.InstanceOfTemplate("Document Base"))
			{
				oMediaItem = (MediaItem)((FileField)oDocumentItem.GetField("Document", "File")).MediaItem;
				
				switch (oMediaItem.Extension.ToLower())
				{
					case "pdf":
						hDocument.Text = "PDF";
						hDocument.CssClass = "pdf-icon icon";
						break;
					case "doc":
					case "docx":
						hDocument.Text = "DOC";
						hDocument.CssClass = "doc-icon icon";
						break;
					case "ppt":
					case "pptx":
						hDocument.Text = "PPT";
						hDocument.CssClass = "ppt-icon icon";
						break;
					default:
						return;
				}
				hDocument.NavigateUrl = string.Concat("~/", ((Item)oMediaItem).GetMediaURL());
			}

			hDocument.Visible = true;

            oDocumentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hDocument);
		}
		else
			if (e.Item.ItemType == ListItemType.Header)
			{
				Literal lDate;
				lDate = (Literal)e.Item.FindControl("lDate");
				lDate.Text = sDate;
				sDate = string.Empty;
			}
		
	}
	/// <summary>
	/// Set all header values
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void rHeader_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		string sTitle = e.Item.DataItem as string;
		if (sTitle != null)
		{
			HtmlTableCell tQuarter = e.Item.FindControl("tQuarter") as HtmlTableCell;
			tQuarter.InnerText = sTitle;
		}
	}

	/// <summary>
	/// The item has to have a valid document associated or be a link item
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	bool IsValidQuarterLink(Item oItem)
	{
		MediaItem oMediaItem;

		return (oItem.InstanceOfTemplate("Link"))
			|| ((oItem.InstanceOfTemplate("Document Base")
				&& (oMediaItem = (MediaItem)((FileField)oItem.GetField("Document", "File")).MediaItem) != null
				&& sValidExtension.Contains(oMediaItem.Extension.ToLower())));
	}

	private class Category
	{
		Item oItem;

		IEnumerable<SubCategory> oSubcategories;
		public string Title
		{
			get
			{
				return oItem.GetText("Title");
			}
		}
		public IEnumerable<SubCategory> Subcategories
		{
			get
			{
				return oSubcategories;
			}
		}
		public Category(Item oItem)
		{
			this.oItem = oItem;
			oSubcategories = oItem.GetChildrenOfTemplate("Quarterly Update").Select(oSubCategorieItem => new SubCategory(oSubCategorieItem));

		}
		public static implicit operator Category(Item oItem)
		{
			return new Category(oItem);
		}
	}
	private class SubCategory
	{
		Item oItem;
		List<Item> oQuarterlyItems;
		public List<Item> QuarterlyItems
		{
			get
			{
				return oQuarterlyItems;
			}
		}
		public string Title
		{
			get
			{
				return oItem.GetText("Title");
			}
		}
		public SubCategory(Item oItem)
		{
			this.oItem = oItem;
			oQuarterlyItems = oItem.GetChildrenOfTemplate("Quarter");
		}
		public static implicit operator SubCategory(Item oItem)
		{
			return new SubCategory(oItem);
		}
	}
</script>
<asp:Repeater runat="server" ID="rContent">
	<HeaderTemplate>
		<table class="filter-results-table" width="100%" cellpadding="0px" cellspacing="0px">
	</HeaderTemplate>
	<ItemTemplate>
		<thead>
			<tr>
				<th class="first" width="*" runat="server" id="tHeader">
				</th>
				<asp:Repeater runat="server" ID="rHeader">
					<ItemTemplate>
						<th class="date center" width="15%" runat="server" id="tQuarter">
						</th>
					</ItemTemplate>
				</asp:Repeater>
			</tr>
		</thead>
		<asp:Repeater runat="server" ID="rRow">
			<HeaderTemplate>
				<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="first" runat="server" id="tSubcategoryTitle" />
					
					<asp:Repeater runat="server" ID="rColumn">
						<ItemTemplate>
							<asp:Repeater runat="server" ID="rFiles">
								<HeaderTemplate>
									<td class="docs date left">
									 <center><asp:Literal runat="server" ID="lDate"></asp:Literal></center>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:HyperLink ID="hDocumentLink" runat="server" Visible="false" target="_blank"></asp:HyperLink>
									<a runat="server" ID="hPDF" class="pdf-icon icon" Visible="false" target="_blank">PDF</a>
									<a ID="hDOC" runat="server" class="doc-icon icon" Visible="false" target="_blank">DOC</a>
								</ItemTemplate>
								<SeparatorTemplate>
									<img src="/CMSContent/Images/docs-divider.png" /></SeparatorTemplate>
								<FooterTemplate>
								
									</td></FooterTemplate>
							</asp:Repeater>
							
							<asp:PlaceHolder runat="server" ID="pOther" Visible="false">
								<td class="date right">
								<asp:Literal runat="server" ID="lMessage"></asp:Literal>
									<br />
									<asp:HyperLink runat="server" ID="hViewResearch" Text="View Research" Visible="false"></asp:HyperLink>
								</td>
							</asp:PlaceHolder>
							<asp:PlaceHolder runat="server" ID="pEmpty" Visible ="false">
							<td></td>
							</asp:PlaceHolder>
						</ItemTemplate>
					</asp:Repeater>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</tbody>
			</FooterTemplate>
		</asp:Repeater>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
