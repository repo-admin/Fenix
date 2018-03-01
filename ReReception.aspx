<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="ReReception.aspx.cs" Inherits="Fenix.ReReception" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<asp:UpdatePanel ID="upnlMain" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
		<ContentTemplate>

			<asp:MultiView ID="mvwMain" runat="server">
				<asp:View ID="vwMain" runat="server">
				</asp:View>
				<asp:View ID="vwEdit" runat="server">
				</asp:View>
			</asp:MultiView>
			<asp:UpdateProgress ID="uprogMain" runat="server" AssociatedUpdatePanelID="upnlMain" DisplayAfter="250" style="margin-top: 15px;">
				<ProgressTemplate>
					<asp:Image ID="imgWait" runat="server" ImageUrl="~/img/wait.gif" />
				</ProgressTemplate>
			</asp:UpdateProgress>
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
