<%@ Control Language="C#" EnableTheming="false"  Inherits="Subtext.Web.UI.Controls.MyLinks" %>

<ul>
	<li><asp:HyperLink Runat="server" NavigateUrl="~/Default.aspx" Text="Home" ID="HomeLink" /></li>
	<li><asp:HyperLink Runat="server" NavigateUrl="~/Archives.aspx" Text="Archives" ID="Archives" /></li>
	<li><asp:HyperLink AccessKey="9" Runat="server" NavigateUrl="~/Contact.aspx" Text="Contact" ID="ContactLink" /></li>
	<li><asp:HyperLink Runat="server" Text="Admin" ID="Admin" /></li>
</ul>