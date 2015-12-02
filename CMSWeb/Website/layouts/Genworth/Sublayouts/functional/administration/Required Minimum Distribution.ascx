<%@ Control Language="c#" %>
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
            BindData();
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
                btnExcel.Enabled = false;
                btnExcel.CssClass = "button-gray";
                btnPDF.Enabled = false;
                btnPDF.CssClass = "button-gray";
            }
        }
    }

    void btnExcel_Click(object sender, EventArgs e)
    {
        string[] agentIds = Authorization.CurrentAuthorization.AgentIds;
        EntityList list = new EntityList();

        if (agentIds.Length > 0)
        {
            list.Add(BuildExcelDocumentParameter(agentIds));
            GetDocumentContent(list, "Excel");
        }
    }

    private void btnPDF_Click(object sender, EventArgs e)
    {
        string[] agentIds = Authorization.CurrentAuthorization.AgentIds;
        EntityList list = new EntityList();

        if (agentIds.Length > 0)
        {
            list.Add(BuildPDFDocumentParameter(agentIds));
            GetDocumentContent(list, "PDF");
        }
    }

    private void GetDocumentContent(EntityList list, string format)
    {
        AM.ContentService.Common.Response.StreamResponse streamResponse = null;
        System.IO.MemoryStream fileStream = null;
        string userName = string.Empty;
        string todayString = DateTime.Today.ToString("yyyyMMdd");
        bool zipIt = false;

        try
        {
            userName = GetUserName();
            ContentController controller = new ContentController();
            streamResponse = controller.GetDocumentContents(list, zipIt, false);
                
            if (streamResponse.ResponseStatus.Status)
            {
                using (fileStream = new System.IO.MemoryStream())
                {
                    CopyStream(streamResponse.FileContent, fileStream);
                    this.Response.Clear();
                    this.Response.ClearHeaders();
                    if (controller.ReadSuccess && fileStream.Length > 0)
                    {
                        if (zipIt)
                        {
                            this.Response.ContentType = "Application/x-zip-compressed";
                            this.Response.AddHeader("Content-Disposition", "attachment; filename=\"RMD_" + userName + "_" + todayString + ".zip\"");
                        }
                        else
                        {
                            Document doc = list[0] as Document;
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
                        }

                        int bufferSize = 4096;
                        int bytesRead = 0;
                        byte[] byteBuffer = new byte[bufferSize];

                        fileStream.Position = 0;
                        bytesRead = fileStream.Read(byteBuffer, 0, bufferSize);

                        while (bytesRead > 0)
                        {
                            if (this.Response.IsClientConnected)
                            {
                                this.Response.OutputStream.Write(byteBuffer, 0, bytesRead);
                                this.Response.Flush();
                                bytesRead = fileStream.Read(byteBuffer, 0, bufferSize);
                            }
                            else
                            {
                                bytesRead = -1;
                            }
                        }
                        this.Response.Close();
                    }
                }
            }
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

    private Document BuildExcelDocumentParameter(string[] agentId)
    {
        Document doc = BuildDocumentParameter(agentId);
        doc.ContentType = "EXCEL";
        doc.Name = "RMDReport_Excel";
        return doc;
    }

    private Document BuildPDFDocumentParameter(string[] agentId)
    {
        Document doc = BuildDocumentParameter(agentId);
        doc.ContentType = "PDF";
        doc.Name = "RMDReport_PDF";
        return doc;
    }

    private Document BuildDocumentParameter(string[] agentId)
    {
        Document doc = new Document();
        StringBuilder sb = new StringBuilder();
        doc.Attributes = new string[] { "AgentId=" + string.Join(",", agentId) };
        doc.ContentLocation = AM.ContentService.Common.Types.Constants.ContentLocType.ReportServer;
        doc.ReportServerType = AM.ContentService.Common.Types.Constants.ReportServerType.Web;
        return doc;
    }

    private void BindData()
    {
        lIntro.Text = ContextExtension.CurrentItem.GetText("Page", "Summary");
		lBody.Text = ContextExtension.CurrentItem.GetText("Page", "Body");

        lInstructions.Text = "<p>Please note, RMD files are not available between January 1 and January 15.</p>";
	
		hPreviousPage.NavigateUrl = "javascript:history.go(-1)";
    }
</script>
<div class="toptxt">
    <asp:Literal runat="server" ID="lIntro" ></asp:Literal>
    <table width="60%" height="100%" cellpadding="5" cellspacing="5" border="0">
        <tr>
            <td width="30%"></td>
            <td colspan="2" style="font-weight: bold; text-align: center;" width="65%" >Choose your report format:</td>
        </tr>
		<tr>
			<td width="30%">
				<div class="">
					<asp:Literal ID="lInstructions" runat="server" />
				</div>
			</td>
            <td width="15%" valign="top" align="center">
				<asp:Button CssClass="formButton" ID="btnExcel" Text=" Excel " runat="server" OnClick="btnExcel_Click" />
			</td>
            <td width="10%" valign="top"> 
                <asp:Button CssClass="formButton" ID="btnPDF" Text="   PDF   " runat="server" OnClick="btnPDF_Click" />                
            </td>
        </tr>
    </table>
    <br />
	<asp:Literal runat="server" ID="lBody" ></asp:Literal>
</div>
<p>
	<asp:HyperLink ID="hPreviousPage" runat="server">BACK TO PREVIOUS PAGE</asp:HyperLink>
</p>