<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/global component list/Image Link.ascx" TagName="ImageLink" %>

<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (CurrentItems == null)
        {
            CurrentItems = ContextExtension.CurrentItem.GetChildrenOfTemplate("Step Link Bottom");
        }

        if (!Page.IsPostBack)
        {
            BindData();
        }
    }

    public List<Item> CurrentItems { get; set; }

    private void BindData()
    {
        if (CurrentItems != null && CurrentItems.Count > 0)
        {
            rSteps.DataSource = CurrentItems;
            rSteps.DataBind();
        }
    }
</script>

<asp:Panel ID="panelSteps" runat="server" CssClass="panelStepsLinkBottom">
    <asp:Repeater runat="server" ID="rSteps">
        <ItemTemplate>
            <div class="StepLinkBottom">
                <div class="gc c4 inside">
                    <h3 runat="server" id="title"><%# (Container.DataItem as Item).GetText("Page", "Title") %></h3>
                    <h4 runat="server" id="subTitle"><%# (Container.DataItem as Item).GetText("Page", "Sub Title") %></h4>
                </div>
                <div class="gc c4 html">
                    <%# (Container.DataItem as Item).GetText("Page", "Body") %>
                </div>
                <div class="clear"></div>
                <Gen:ImageLink runat="server" ID="imageLink" CurrentItem="<%# Container.DataItem %>" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
