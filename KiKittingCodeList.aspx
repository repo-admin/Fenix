<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="KiKittingCodeList.aspx.cs" Inherits="Fenix.KiKittingCodeList" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>Číselník kitů</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDescriptionCzFlt" runat="server" CssClass="labels" Text="Popis sestavy: "></asp:Label><br />
								<asp:TextBox ID="tbxDescriptionCzFlt" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCodeFlt" runat="server" CssClass="labels" Text="Kód sestavy: "></asp:Label><br />
								<asp:TextBox ID="tbxCodeFlt" runat="server" CssClass="txt" MaxLength="5"></asp:TextBox>
							</td>
						<td colspan="1">
							<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
							</asp:DropDownList>
						</td>
						<td colspan="1">
							<asp:Label ID="lblIsActive" runat="server" CssClass="labels" Text="Aktivita: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlIsActive" runat="server" Width="60px">
								<asp:ListItem Value="-1" Text="Vše"></asp:ListItem>
								<asp:ListItem Value="1" Text="Aktivní" Selected="True"></asp:ListItem>
								<asp:ListItem Value="0" Text="Neaktivní"></asp:ListItem>
							</asp:DropDownList>
						</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />
								&nbsp;
							</td>
						</tr>
					</table>
				</div>
				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label>
				<br />
				<div id="divData" runat="server" style="width: auto;">
					<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged" OnRowCommand="grdData_RowCommand">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="Code" HeaderText="Kód CRM" />
							<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ReadOnly="True" />
							<asp:BoundField DataField="DescriptionEng" HeaderText="DescriptionEng" ReadOnly="True" />
							<asp:BoundField DataField="IsSent" HeaderText="Posláno na ND" ReadOnly="True" />
							<asp:BoundField DataField="SentDate" HeaderText="Datum odeslání na ND" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="MeasuresCode" HeaderText="MeJe" />
							<asp:BoundField DataField="KitQualitiesCode" HeaderText="Kvalita" />
							<asp:BoundField DataField="Packaging" HeaderText="Packaging" />
							<asp:BoundField DataField="GroupsCode" HeaderText="Skupina" />
							<asp:BoundField DataField="IsActive" HeaderText="Aktivita" />
						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnZmenaAktivity" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="ZmenaAktivity" ImageUrl="~/img/invert.png" ToolTip="Mění aktivitu"
									Visible="true" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
					<asp:Panel ID="pnlKiItems" runat="server" Visible="false">
						<h3>Detail sestavy</h3>
						<asp:GridView ID="gvKiItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="cdlKitsItemsID" OnSelectedIndexChanged="gvKiItems_SelectedIndexChanged">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
								<asp:BoundField DataField="cdlKitsItemsID" HeaderText="ID" ReadOnly="True" />
								<asp:BoundField DataField="ItemVerKitText" HeaderText="Kit/Item" ReadOnly="True" />
								<asp:BoundField DataField="ItemOrKitID" HeaderText="ID Kit/Item" ReadOnly="True" />
								<asp:BoundField DataField="ItemCode" HeaderText="Kód" />
								<asp:BoundField DataField="DescriptionCzItemsOrKit" HeaderText="Popis" ReadOnly="True" />
								<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Množství" ReadOnly="True"  ItemStyle-HorizontalAlign="Right" />
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						<cc1:Pager ID="grdPagerKiItems" runat="server" />
					</asp:Panel>
					<br />
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<table>
					<tr>
						<td colspan="10" align="center">
							<h1>Nová sestava</h1>
						</td>
					</tr>
					<tr>
						<td colspan="1">
							<asp:Label ID="lblCode" runat="server" CssClass="labels" Text="Kód(CRM)"></asp:Label><br />
							<asp:TextBox ID="tbxCode" runat="server" MaxLength="5" Width="55px"></asp:TextBox>
						</td>
						<td colspan="2">
							<asp:Label ID="lblDescriptionCz" runat="server" CssClass="labels" Text="Název (český): " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxDescriptionCz" runat="server" MaxLength="55" Width="240px"></asp:TextBox>
						</td>
						<td colspan="2">
							<asp:Label ID="lblDescriptionEng" runat="server" CssClass="labels" Text="Název (anglický): " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxDescriptionEng" runat="server" MaxLength="55" Width="240px"></asp:TextBox>
						</td>
						<td colspan="2">
							<asp:Label ID="lblMeasures" runat="server" CssClass="labels" Text="MeJe: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlMeasures" runat="server" Width="65px">
							</asp:DropDownList>

						</td>
						<td colspan="1">
							<asp:Label ID="lblKitQualities" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlKitQualities" runat="server" Width="65px">
							</asp:DropDownList>
						</td>
						<td colspan="1">
							<asp:Label ID="lblPackaging" runat="server" CssClass="labels" Text="Packaging: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxPackaging" runat="server" MaxLength="5" Width="55px"></asp:TextBox>
						</td>
						<td colspan="1">
							<asp:Label ID="lblKitGroups" runat="server" CssClass="labels" Text="Skupina: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlKitGroups" runat="server" Width="70px">
							</asp:DropDownList>
						</td>


					</tr>
					<tr>
						<td colspan="5">
							<asp:Label ID="lblCPE" runat="server" CssClass="labels" Text="Zařízení: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlCPE" runat="server" Width="350px">
							</asp:DropDownList><br />
							<asp:Label ID="lblCpeQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxCpeQuantity" runat="server" Text="1"></asp:TextBox>

						</td>

						<td colspan="5">
							<asp:Label ID="lblNW" runat="server" CssClass="labels" Text="Materiál: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlNW" runat="server" Width="350px">
							</asp:DropDownList><br />
							<asp:Label ID="lblNwQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxNwQuantity" runat="server" Text="1"></asp:TextBox>
						</td>

					</tr>
					<tr>
						<td colspan="5">
							<asp:Button ID="btnPridatCpeDoSoupravy" runat="server" Text="Přidat do soupravy" OnClick="btnPridatCpeDoSoupravy_Click" />
						</td>
						<td colspan="5">
							<asp:Button ID="btnPridatNwDoSoupravy" runat="server" Text="Přidat do soupravy" OnClick="btnPridatNwDoSoupravy_Click" />
						</td>
					</tr>
				</table>
				<asp:Label ID="lblErrInfo" runat="server" CssClass="errortext"></asp:Label>
				<br />
				<asp:GridView ID="gvItemsNew" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:TemplateField HeaderText="Uložit">
							<ItemTemplate>
								<asp:CheckBox ID="CheckBoxR" runat="server" Checked='<%# Bind("AnoNe") %>' />
							</ItemTemplate>
							<ItemStyle HorizontalAlign="Right" Width="50px" />
						</asp:TemplateField>
						<asp:BoundField DataField="ItemVerKit" HeaderText="Kit/Item" ReadOnly="True" />
						<asp:BoundField DataField="ItemOrKitID" HeaderText="ID Kit/Item" ReadOnly="True" />
						<asp:BoundField DataField="ItemGroupGoods" HeaderText="Skupina zboží" ReadOnly="True" />
						<asp:BoundField DataField="ItemCode" HeaderText="Kód" ReadOnly="True" />
						<asp:BoundField DataField="DescriptionCzItemsOrKit" HeaderText="Popis" ReadOnly="True" />
						<asp:BoundField DataField="ItemOrKitQuantity" HeaderText="Množství" ReadOnly="True" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<br />
				<asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="190px" />&nbsp;
				<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Uložit" Width="190px" />
			</asp:View>
		</asp:MultiView>

	</div>


</asp:Content>
