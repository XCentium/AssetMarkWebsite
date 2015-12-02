<%@ Control Language="c#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Security" %>
<%@ Import Namespace="AM.ContentService.Common.Entities" %>
<%@ Import Namespace="AM.ContentService.Client.Controller" %>
<script runat="server">

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            VerifyActiveDate();
        }
    }

    private void VerifyActiveDate()
    {
        int currentMonth = DateTime.Today.Month;
        int currentDay = DateTime.Today.Day;

        if (currentMonth == 1)
        {
            if (currentDay >= 1 && currentDay <= 15)
            {
                Excel.Enabled = false;
                Excel.CssClass = "disable";
                PDF.Enabled = false;
                PDF.CssClass = "disable";
            }
        }
    }
    
    protected void Excel_Click(object sender, EventArgs e)
    {
        string[] agentIds = Authorization.CurrentAuthorization.AgentIds;
        EntityList list = new EntityList();

        if (agentIds.Length > 0)
        {
            list.Add(BuildDocumentParameter(agentIds, "EXCEL", "RMDReport_Excel"));
            GetDocumentContent(list, "Excel");
        }
    }

    protected void PDF_Click(object sender, EventArgs e)
    {
        string[] agentIds = Authorization.CurrentAuthorization.AgentIds;
        EntityList list = new EntityList();

        if (agentIds.Length > 0)
        {
            list.Add(BuildDocumentParameter(agentIds, "PDF", "RMDReport_PDF"));
            GetDocumentContent(list, "PDF");
        }
    }

    private AM.ContentService.Common.Entities.Document BuildDocumentParameter(string[] agentId, string contentType, string reportName)
    {
        AM.ContentService.Common.Entities.Document doc = new AM.ContentService.Common.Entities.Document();        
        bool isRMDOnly = radioValues.SelectedValue == "1";
        
        doc.Attributes = new string[] { "AgentId=" + string.Join(",", agentId), "RMDOnly=" + isRMDOnly.ToString() };
        doc.ContentLocation = AM.ContentService.Common.Types.Constants.ContentLocType.ReportServer;
        doc.ReportServerType = AM.ContentService.Common.Types.Constants.ReportServerType.Web;
        doc.ContentType = contentType;
        doc.Name = reportName;
        
        return doc;
    }

    private void GetDocumentContent(EntityList list, string format)
    {
        try
        {
            string userName = GetUserName();
            string todayString = DateTime.Today.ToString("yyyyMMdd");
        
            this.Response.Clear();
            this.Response.ClearHeaders();
            this.Response.ClearContent();

            switch (format.ToUpper())
            {
                case "PDF":
                    this.Response.ContentType = "Application/PDF";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=\"RMD_" + userName + "_" + todayString + ".pdf\"");
                    break;
                case "EXCEL":
                    this.Response.ContentType = "application/x-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=\"RMD_" + userName + "_" + todayString + ".xls\"");
                    break;
            }
        
            AM.ContentService.Common.Response.StreamResponse streamResponse = null;
            ContentController controller = new ContentController();
            streamResponse = controller.GetDocumentContents(list, false, false);
            CopyStream(streamResponse.FileContent, Response.OutputStream);
        }
        catch (Exception ex)
        {
            Sitecore.Diagnostics.Log.Error(string.Format("ERROR: {0}, Stacktrace: {1}", ex.Message, ex.StackTrace), this);
        }
    }

    public void CopyStream(System.IO.Stream readStream, System.IO.Stream writeStream)
    {
        int bytesToRead = 4096;
        long total = 0L;
        long count = bytesToRead;
        byte[] byteArray = new byte[bytesToRead + 1];

        try
        {
            while ((0 < count))
            {
                count = readStream.Read(byteArray, 0, bytesToRead);
                total += count;
                writeStream.Write(byteArray, 0, (int)count);
            }
            writeStream.Flush();
            readStream.Close();
        }
        catch (System.IO.IOException ex)
        {
            throw ex;
        }
        finally
        {
            readStream.Close();
            readStream.Dispose();
        }
    }

    private string GetUserName()
    {
        string name = Authorization.CurrentAuthorization.UserName;
        string cleanName = Regex.Replace(name, "[^a-zA-Z0-9_.& ]+", "", RegexOptions.Compiled);
        return cleanName;
    }


    
</script>


<style type="text/css">
    body.ModalWindowBody
    {
        background:#fff;
    }
</style>
<script type="text/javascript" src="/CMSContent/Scripts/RmdReport.js"></script>
<div class="rmdReport">
	<sc:Placeholder ID="Placeholder3" Key="Title" runat="server" />	
    <div class="rmdReportContainer">
	    <sc:Placeholder ID="Placeholder2" Key="Top" runat="server" />
        <div class="rmdReportRadioListContent">
            <div>
                <asp:RadioButtonList runat="server" CssClass="rmdRadioButtonList" RepeatLayout="UnorderedList" ID="radioValues">
                    <asp:ListItem Text="Display all clients with a required minimum distribution as calculated by AssetMark Trust*" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Display all clients with a retirement account held at AssetMark Trust" Value="2"></asp:ListItem>
                </asp:RadioButtonList>
                <div class="clear"></div>
                </div>
            <p>Please note, RMD files are not available between January 1 and January 15</p>
            <p>Choose your report format:</p>
            <asp:LinkButton ID="Excel" runat="server" OnClick="Excel_Click" />
            <asp:LinkButton ID="PDF" runat="server" OnClick="PDF_Click" />
            <div class="clear"></div>
        </div>
	    <sc:Placeholder ID="Placeholder1" Key="Bottom" runat="server" />
    </div>
    <p id="gw-dialog-close-p">
        <a href="#" id="gw-dialog-close"><img src="/CMSContent/Images/close.jpg" /></a> 
    </p>
</div>