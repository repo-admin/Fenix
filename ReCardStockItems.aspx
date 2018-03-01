<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="ReCardStockItems.aspx.cs" Inherits="Fenix.ReCardStockItems" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>Karty materiálů a zařízení</h1>
				<div id="divFilter" runat="server" visible="true" style="width:auto;">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="Typ: "></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlItemTypeFlt22" runat="server">
								</asp:DropDownList>
								&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblItemVerKitFlt" runat="server" CssClass="labels" Text="Item/Kit: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlItemVerKitFlt" runat="server">
									<asp:ListItem Value="-1" Selected="True" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Items"></asp:ListItem>
									<asp:ListItem Value="1" Text="Kits"></asp:ListItem>
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30%;">

								<asp:Label ID="lblGroupGoodsFlt" runat="server" CssClass="labels" Text="SkZboží: " ToolTip="Skupina zboží"></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlGroupGoodsFlt" runat="server" AutoPostBack="false">
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td colspan="1">
								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td colspan="1">
								<asp:Label ID="lblMaterialCodeFlt" runat="server" CssClass="labels" Text="Kod materiálu: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxMaterialCodeFlt" runat="server" MaxLength="10"></asp:TextBox>&nbsp;&nbsp;
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCardQuantityFlt" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label>&nbsp;&nbsp;<br />
								<asp:DropDownList ID="ddlCardQuantityFlt" runat="server" Width="80px">
									<asp:ListItem Value="-1" Selected="True" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="S volným mn."></asp:ListItem>
									<asp:ListItem Value="1" Text="Žádné volné mn."></asp:ListItem>
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<%--								<asp:Label ID="lblCodeFlt" runat="server" CssClass="labels" Text="Kód sestavy: "></asp:Label><br />
								<asp:TextBox ID="tbxCodeFlt" runat="server" CssClass="txt" MaxLength="5"></asp:TextBox>--%>
							</td>
							<td colspan="2">
								<%--								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>--%>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:ImageButton ID="btnExcel" runat="server" 
									CommandName="btnExcelClicked" ImageUrl="~/img/excelSmall.png" ToolTip=""
									Visible="true" OnClick="btnExcel_Click" />
								&nbsp;
							</td>
						</tr>
					</table>
				</div>
				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label>
				<br />
				<div id="divData" runat="server" style="width: auto;">
					<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemVerKitDescription" HeaderText="Typ Popis" />
							<asp:BoundField DataField="ItemOrKitId" HeaderText="ID Items/Kit" />
							<asp:BoundField DataField="GroupGoods" HeaderText="SkZ" />
							<asp:BoundField DataField="Code" HeaderText="Kód" />
							<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInteger" HeaderText="Množství" ItemStyle-Wrap="False" >
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitFreeInteger" HeaderText="Množství volné" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnConsilliationInteger" HeaderText="Množství ke schválení" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitReservedInteger" HeaderText="Množství rezervované" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitReleasedForExpeditionInteger" HeaderText="Množství uvolněné" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitExpeditedInteger" HeaderText="Množství expedováno" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="MeasuresCode" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="QualitiesCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>

</asp:Content>
