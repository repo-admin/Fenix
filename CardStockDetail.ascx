<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CardStockDetail.ascx.cs" Inherits="Fenix.CardStockDetail" %>


<table>
	<tr>
		<td colspan="4" align="center">
			<asp:Label ID="lblStockName" runat="server" CssClass="labels" Font-Bold="True" Font-Size="16px" ForeColor="Navy"></asp:Label>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblID" runat="server" CssClass="labels" Text="ID: " ToolTip=""></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxID" runat="server" Enabled="False" Width="100px"></asp:TextBox>
		</td>
		<td>
			<asp:Label ID="lblOrganisationNumber" runat="server" CssClass="labels" Text="Item/Kit:"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemVerKit" runat="server" MaxLength="15" Width="100px" ReadOnly="True"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblItemOrKitID" runat="server" CssClass="labels" Text="Item/Kit ID"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitID" runat="server" MaxLength="150" Width="100px" ReadOnly="True"></asp:TextBox>
		</td>
		<td>
			<asp:Label ID="lblCity" runat="server" CssClass="labels" Text="Název:"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxDescriptionCz" runat="server" MaxLength="100" Width="250px" ReadOnly="True"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblStreetName" runat="server" CssClass="labels" Text="Kvalita/MeJe:"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitQuality" runat="server" MaxLength="100" Width="150px" ReadOnly="True"></asp:TextBox>
		</td>
		<td>
			<asp:Label ID="lblStreetOrientationNumber" runat="server" CssClass="labels" Text="SK/Kód: "></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxGroopsAndCode" runat="server" MaxLength="15" ReadOnly="True"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td colspan="4" align="center">
			<asp:Label ID="Label1" runat="server" CssClass="labels" Text="M n o ž s t v í" Font-Bold="True" Font-Size="14px" ForeColor="#993333"></asp:Label>&nbsp;
			<asp:Label ID="lblRelease" runat="server" Text=" k uvolnění" Visible="False" ForeColor="#993333"  Font-Size="14px" Font-Bold="True"></asp:Label>&nbsp;
			<asp:TextBox ID="tbxRelease" runat="server" Visible="False" BackColor="#ffffff"  Font-Bold="True"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblItemOrKitFree" runat="server" CssClass="labels" Text="volné: " ></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitFree" runat="server" MaxLength="18"></asp:TextBox>

		</td>
		<td>
			<asp:Label ID="lblItemOrKitUnConsilliation" runat="server" CssClass="labels" Text="k odsouhlasení: " ></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitUnConsilliation" runat="server" MaxLength="18"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblItemOrKitReserved" runat="server" CssClass="labels" Text="rezervované" ></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitReserved" runat="server"></asp:TextBox>
		</td>
		<td>
			<asp:Label ID="lblItemOrKitReleasedForExpedition" runat="server" CssClass="labels" Text="uvolněno k expedici: " ></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxItemOrKitReleasedForExpedition" runat="server" MaxLength="10"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td style="height: 26px">
			<asp:Label ID="lblItemOrKitExpedited" runat="server" CssClass="labels" Text="expedováno: " ></asp:Label>
		</td>
		<td style="height: 26px">
			<asp:TextBox ID="tbxItemOrKitExpedited" runat="server" MaxLength="15"></asp:TextBox>
		</td>
		<td style="height: 26px"></td>
		<td style="height: 26px"></td>
	</tr>
	<tr>
		<td>
			<asp:Label ID="lblModifyDate" runat="server" CssClass="labels" Text="Editováno:" ToolTip="Datum poslední aktualizace záznamu"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxModifyDate" runat="server" Width="150px" ReadOnly="True"></asp:TextBox>
		</td>
		<td>
			<asp:Label ID="lblModifyUserId" runat="server" CssClass="labels" Text="Editoval:" ToolTip="Poslední, kdo editoval záznamu"></asp:Label>
		</td>
		<td>
			<asp:TextBox ID="tbxModifyUserId" runat="server" Width="200px" ReadOnly="True"></asp:TextBox>
		</td>

	</tr>
	<tr>
		<td></td>
		<td></td>
		<td>
			<asp:Label ID="lblIsActive" runat="server" CssClass="labels" Text="Aktivita: " ToolTip=""></asp:Label>
		</td>
		<td>
			<asp:CheckBox ID="chkbIsActive" runat="server" Enabled="False" />
		</td>
	</tr>
</table>

