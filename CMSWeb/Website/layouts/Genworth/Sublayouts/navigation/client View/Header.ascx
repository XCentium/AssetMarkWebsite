<%@ Control Language="c#" AutoEventWireup="true" %>
<div class="header client-header">
	<div id="logoContainer">
		<img src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/fpo-advisor-logo.png" />
	</div>
	<span id="welcomeText" class="headerText">Welcome Victoria Emanuelson<span class="advisor">Advisor:
		Gary Smith</span></span>
	<ul id="userMenu" class="headerText headerList">
		<li>Manager Role •</li>
		<li><a href="#">Logout</a></li>
	</ul>
</div>
<sc:Placeholder  Key="Content" runat="server" />
