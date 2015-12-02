<%@ Page Language="C#" AutoEventWireup="true" Codepage="65001"%>
<%@ OutputCache Location="None" VaryByParam="none" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>


<%--
	This page uses FW Player from http://www.longtailvideo.com/players/. Licensing and documentation
	in TFS at $/Genworth/Documents/JW Player. This page support playback in an iFrame or shadowbox,
	the latter specified with "shadowmode=1" in the query string.
--%>

<script runat="server">
	private string sVersion = "1.0";

	string sWidth;
	string sHeight;
	string sUrl;
	string sMedia;

	private string Width { get { return sWidth ?? (sWidth = ContextExtension.CurrentItem.GetText("Video", "Width", "")); } }
	private string Height { get { return sHeight ?? (sHeight = ContextExtension.CurrentItem.GetText("Video","Height","")); } }
	private string Url { get { return sUrl ?? (sUrl = ContextExtension.CurrentItem.GetText("Video","URL","")); } }

	private string Media
	{
		get
		{
			MediaItem oItem;
			if (string.IsNullOrEmpty(sMedia) && (oItem = ContextExtension.CurrentItem.GetMediaItem()) != null)
			{
				sMedia = ItemExtension.GetMediaURL(oItem);
				sMedia = sMedia.Replace("ashx", oItem.Extension);
			}

			return sMedia;
		}
	}
	
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

		// bind using the current item
		//
		BindKeywords(oItem);
		BindVideo(oItem,Request.QueryString["shadowmode"] == "1");
	}

	private void BindKeywords(Item oItem)
	{
		Page.Title = oItem.GetText("Title","Genworth");
		MetaKeywords.Attributes.Add("content", oItem.GetText("SEO", "Meta Keywords", "Genworth"));
		MetaDescription.Attributes.Add("content", oItem.GetText("SEO", "Meta Description", "Genworth"));
	}

	private void BindVideo(Item oItem,bool bShadowMode)
	{
		String sUrl;
		StringBuilder sScript;

		// use the media URL if it exists, else the explicit URL
		//
		if (string.IsNullOrWhiteSpace(sUrl = this.Media))
		{
			sUrl = this.Url;
		}
		
		// if a YouTube URL, redirect for display in our host/parent
		//
		if (sUrl.ToLower().Contains("youtu"))
		{
			sScript = new StringBuilder(this.Url);
			sScript.Append("?rel=0&modestbranding=1&autoplay=");
			sScript.Append(bShadowMode ? "1" : "0");
			Response.Redirect(sScript.ToString(), true);
		}
		else if (!string.IsNullOrWhiteSpace(sUrl))
		{
			sScript = new StringBuilder("jwplayer('mediaspace').setup({'flashplayer': '");
            sScript.Append(Page.ResolveClientUrl("/cmscontent/flash/jwplayer.flash.swf"));
			sScript.Append("','file': '");
			sScript.Append(sUrl);

			if (!bShadowMode)
			{
				sScript.Append("','image': '");
				sScript.Append(oItem.GetImageURL("Video", "Image"));
			}
	
			sScript.Append("','width':'100%");
			sScript.Append("','height':'");
			sScript.Append(bShadowMode ? this.Height : "420");
			sScript.Append("','autostart':'");
			sScript.Append(bShadowMode ? "true":"false");
            sScript.Append("','dock': 'false','controlbar': 'over','controlbar.idlehide': 'true','stretching': 'uniform', 'base': '/cmscontent/flash/' });");
	
			Page.ClientScript.RegisterStartupScript(this.GetType(), "jwplayer", sScript.ToString(), true);
		}
	}
	
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="https://www.w3.org/1999/xhtml">
	<head id="hHead" runat="server">
		<title>AssetMark</title>
		<meta name="description" runat="server" id="MetaDescription" />
		<meta name="keywords" runat="server" id="MetaKeywords" />
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<style type="text/css">
		    body, #form
		    {
		        margin:0px;
		        background-color:#000;
		    }
		</style>
	</head>

	<script type='text/javascript' src='<%=Page.ResolveClientUrl("~/") %>cmscontent/flash/jwplayer6.js'></script>
	<script type="text/javascript">	    jwplayer.key = "Kb4hzZxVma3+83funWiZrjEYLqridcm6AnBwBQ==";</script>

	<body>
		<form id="form1" method="post" runat="server">

			<div id='mediaspace'>Loading...</div>

		</form>
	</body>

</html>

