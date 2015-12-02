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
            CurrentItems = ContextExtension.CurrentItem.GetChildrenOfTemplate("Bottom Slide Panel");
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
            rSteps.DataSource = CurrentItems.Take(3);
            rSteps.DataBind();
        }
    }
    
    

    private string ClassName(int index)
    {
        string response = "gc c4 Column" + index;
        if (index == 0)
        {
            response += " inside";
        }
        return response;
    }

    protected void rSteps_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem = e.Item.DataItem as Item;
        var image = e.Item.FindControl("image") as Image;
        var imageBody = e.Item.FindControl("imageBody") as Image;
        oItem.ConfigImage("Header", image);
        oItem.ConfigImage("Body", imageBody);
    }
</script>

<div id="BottomSlidePanel">
    <asp:Repeater runat="server" ID="rSteps" OnItemDataBound="rSteps_ItemDataBound">
        <ItemTemplate>
            <div class='<%# ClassName(Container.ItemIndex) %>'>
                <div class="panel">
                    <asp:Image ID="image" runat="server" />
                    <h5><%# (Container.DataItem as Item).GetText("Page", "Title") %></h5>
                    <h6><%# (Container.DataItem as Item).GetText("Page", "Sub Title") %></h6>
                    <div class="clear"></div>
                </div>
                <div class="body">
                    <asp:Image ID="imageBody" runat="server" />
                    <div class="container"><%# (Container.DataItem as Item).GetText("Page", "Body") %></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="clear"></div>
</div>

<script type="text/javascript">
    $(function () {
        Genworth.Planning.BottomSlidePanel();
    });
</script>
