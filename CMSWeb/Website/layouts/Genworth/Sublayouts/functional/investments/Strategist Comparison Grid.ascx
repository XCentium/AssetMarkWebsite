<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	private Item oComparePage;

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
		string sCode;
		Item oSelectedAllocationApproach;
		Repeater[] oRepeaters;
		Item[] oItems;
		Item oDocument;
		string sPath;

		//assume there is no file to download
		hDownload.Visible = false;

		//get the code
		sCode = (Request.QueryString["AllocationApproach"] ?? string.Empty).Trim();

		//get the selected allocation approach
		if (!string.IsNullOrEmpty(sCode))
		{
			//temporarily load the items
			oItems = Genworth.SitecoreExt.Constants.Investments.Items.AllocationApproachFolderItem.GetChildrenOfTemplate("Asset Allocation Approach").Where(oAllocationApproach => oAllocationApproach["Code"] == sCode).ToArray();
			//	Genworth.SitecoreExt.Constants.Investments.Items.InvestmentsRootItem.Axes.SelectItems(new StringBuilder(@"Asset Allocation Approaches//*[@@TemplateName=""Asset Allocation Approach"" and @Code=""").Append(sCode).Append(@"""]").ToString());

			//get the selected allocation approach
			oSelectedAllocationApproach = oItems != null ? oItems.FirstOrDefault() : null;

			//do we have a selected allocation approach?
			if (oSelectedAllocationApproach != null)
			{
				//create the array of repeaters
				oRepeaters = new Repeater[]{
                    rStrategists,
                    rInvestmentMandate,
					rPhilosophies,
					rFrequencies,
					rMagnitudes,
					rCores,
					rSatellites,
					rInternationalExposures,
					rBetas,
					rAlphas,
					rTypes,
					rStrategys,
					rFeeSensitivitys,
					rTaxSensitivityOptions,
					rAccountMinimums,
					rTypeOfOrganizations,
					rPortfolioConstructs
				};

				//get the items
				string sId=oSelectedAllocationApproach.ID.ToString();
				oItems = Genworth.SitecoreExt.Constants.Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist).SelectMany(oStrategist => oStrategist.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy).Where(oStrategy => oStrategy["Allocation Approach"] == sId)).ToArray();
					//Genworth.SitecoreExt.Constants.Investments.Items.InvestmentsRootItem.Axes.SelectItems(new StringBuilder(@"strategists//*[@@TemplateName=""Strategist""]//*[@@TemplateName=""Strategy"" AND contains(@Allocation Approach, """).Append(oSelectedAllocationApproach.ID.ToString()).Append(@""")]").ToString());

				//do we have enough items to bind?
				if (oItems != null)
				{
					//loop over the repeaters
					foreach (Repeater rItems in oRepeaters)
					{
						//get the strategies for the selected allocation approach
						rItems.DataSource = oItems;
						rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
						rItems.DataBind();
					}

					//do we have a download?
					if ((oDocument = oSelectedAllocationApproach.GetListItem("Documents", "Strategist Comparison")) != null)
					{
						//does the document have a file?
						if (!string.IsNullOrEmpty(sPath = oDocument.GetImageURL("Document", "File")))
						{
							//set the download path
							hDownload.Attributes.Add("href", sPath);
							hDownload.Visible = true;
							hDownload.Target = "_blank";
						}
					}
				}
				else
				{
					//hide the items since we do not have enough items
					pItems.Visible = false;
				}
			}
			else
			{
				//hide the items since we do not have enough items
				pItems.Visible = false;
			}
		}
		else
		{
			//hide the items since we do not have enough items
			pItems.Visible = false;
		}
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		Literal lText;
		Item[] oSolutionItems;
		Image iLogo;
		string sLogo;

		//get the item
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the text
			lText = (Literal)e.Item.FindControl("lText");
          
			//depending on which repeater was used, output the field
			switch (((Repeater)sender).ID.ToLower())
			{
				case "rstrategists":
					iLogo = (Image)e.Item.FindControl("iLogo");
					sLogo = oItem.Parent.GetField("Strategist", "Logo").GetImageURL();
					if (!string.IsNullOrWhiteSpace(sLogo))
						iLogo.Attributes.Add("src",string.Format("{0}?mw=100&mh=40",sLogo));
					else
						iLogo.Visible = false;
					break;
                case "rinvestmentmandate":
                    lText.Text = oItem.GetText("Strategy", "Investment Mandate");
                    break;
				case "rphilosophies":
					lText.Text = oItem.GetText("Strategy", "Philosophy");
					break;
				case "rfrequencies":
					lText.Text = oItem.GetText("Strategy", "Frequency");
					break;
				case "rmagnitudes":
					lText.Text = oItem.GetText("Strategy", "Magnitude");
					break;
				case "rcores":
					lText.Text = oItem.GetText("Strategy", "Asset Class Exposures Maintained Core");
					break;
				case "rsatellites":
					lText.Text = oItem.GetText("Strategy", "Asset Class Exposures Maintained Satellite");									
					break;
				case "rinternationalexposures":
					lText.Text = oItem.GetText("Strategy", "International Exposure");
					break;
				case "rbetas":
					lText.Text = oItem.GetText("Strategy", "Alternative Exposure Beta");
					break;
				case "ralphas":
					lText.Text = oItem.GetText("Strategy", "Alternative Exposure Alpha");
					break;
				case "rtypes":
					//get the solution items for this strategy
					if ((oSolutionItems = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).ToArray()) != null)
					{
						//bind the solution types
						lText.Text = string.Join(" / ", oSolutionItems.SelectMany(oSolution => oSolution.GetMultilistItems("Solution", "Type").GetItemsOfTemplate("Solution Type")).Select(oSolutionType => oSolutionType.GetText("Code")).Distinct().ToArray());
					}
					break;
				case "rstrategys":
					//get the solution items for this strategy
					if ((oSolutionItems = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).ToArray()) != null)
					{
						//bind the strategies
						lText.Text = string.Join(" / ", oSolutionItems.SelectMany(oSolution => oSolution.GetMultilistItems("Solution", "Strategy")).Select(oStrategy => oStrategy.DisplayName).Distinct().ToArray());
					}
					break;
				case "rfeesensitivitys":
					lText.Text = oItem.GetText("Strategy", "Fee Sensitivity");
					break;
				case "rtaxsensitivityoptions":
					lText.Text = oItem.GetText("Strategy", "Tax Sensitivity Option");
					break;
				case "raccountminimums":
					lText.Text = oItem.GetMultiLineText("Strategy", "Account Minimum");
					break;
				case "rtypeoforganizations":
					lText.Text = oItem.GetText("Strategy", "Organization Type");
					break;
				case "rportfolioconstructs":
					lText.Text = oItem.GetText("Strategy", "Portfolio Construct");
					break;
			}
		}
	}
</script>
<asp:PlaceHolder ID="pItems" runat="server">
<div class="performance-table-wrapper">
	<div class="tab-header">
		<h1>
			<span class="float-right"><a id="aPrintPdf" href="JavaScript: window.print();" class="icon print-icon">
				Print</a>
				<img src="/CMSContent/Images/gridHeader_div.png" />
				<asp:HyperLink ID="hDownload" runat="server" Text="Download" CssClass="icon download-icon"  />
			</span>
			Key Attributes
		</h1>
	</div>
</div>


		<div class="comparison-table-scroll-window">
			<table class="comparison-table" width="100%" cellspacing="1" cellpadding="0" border="0">
				<thead>
					<tr>
						<th colspan="2" class="primary">
							<label>
								Strategist</label>
						</th>
						<asp:Repeater ID="rStrategists" runat="server">
							<ItemTemplate>
								<th class="odd">
									<asp:Image runat="server" ID="iLogo" />
								</th>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<th class="even">
									<asp:Image runat="server" ID="iLogo" />
								</th>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
				</thead>
				<tbody>
                 <tr>
						<td colspan="2" class="primary">
							<label>
								Investment Mandate</label>
						</td>
						<asp:Repeater ID="rInvestmentMandate" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>

					<tr>
						<td rowspan="3" class="primary">
							<label>
								Asset Allocation Approach</label>
						</td>
						<td class="secondary">
							<label>
								Philosophy</label>
						</td>
						<asp:Repeater ID="rPhilosophies" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span><span>
										<asp:Literal ID="lText" runat="server" /></span></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td class="secondary">
							<label>
								Frequency</label>
						</td>
						<asp:Repeater ID="rFrequencies" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td class="secondary">
							<label>
								Magnitude</label>
						</td>
						<asp:Repeater ID="rMagnitudes" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td rowspan="2">
							<label>
								Asset Class Exposures Maintained</label>
						</td>
						<td class="secondary">
							<label>
								Core</label>
						</td>
						<asp:Repeater ID="rCores" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td class="secondary">
							<label>
								Satellite
							</label>
						</td>
						<asp:Repeater ID="rSatellites" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								International Exposure</label>
						</td>
						<asp:Repeater ID="rInternationalExposures" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td rowspan="2" class="primary">
							<label>
								Alternative Exposure</label>
						</td>
						<td class="secondary">
							<label>
								Beta</label>
						</td>
						<asp:Repeater ID="rBetas" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td class="secondary">
							<label>
								Alpha</label>
						</td>
						<asp:Repeater ID="rAlphas" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td rowspan="2" class="primary">
							<label>
								Implementation Vehicles
							</label>
						</td>
						<td class="secondary">
							<label>
								Type</label>
						</td>
						<asp:Repeater ID="rTypes" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td class="secondary">
							<label>
								Strategy</label>
						</td>
						<asp:Repeater ID="rStrategys" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								Fee Sensitivity</label>
						</td>
						<asp:Repeater ID="rFeeSensitivitys" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								Tax Sensitivity Options</label>
						</td>
						<asp:Repeater ID="rTaxSensitivityOptions" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								Account Minimum</label>
						</td>
						<asp:Repeater ID="rAccountMinimums" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								Type of Organization</label>
						</td>
						<asp:Repeater ID="rTypeOfOrganizations" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
					<tr>
						<td colspan="2" class="primary">
							<label>
								Portfolio Construct</label>
						</td>
						<asp:Repeater ID="rPortfolioConstructs" runat="server">
							<ItemTemplate>
								<td class="odd">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<td class="even">
									<span>
										<asp:Literal ID="lText" runat="server" /></span>
								</td>
							</AlternatingItemTemplate>
						</asp:Repeater>
					</tr>
				</tbody>
			</table>
		</div>

</asp:PlaceHolder>
