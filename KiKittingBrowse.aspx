<%@ Page Title="" Language="C#" MasterPageFile="~/Kitting.master" AutoEventWireup="true" CodeBehind="KiKittingBrowse.aspx.cs" Inherits="Fenix.KiKittingBrowse" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>K - Přehled objednávek</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="MessageId: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxMessageIdFlt" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageStatusFlt" runat="server" CssClass="labels" Text="MessageStatus: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlMessageStatusFlt" runat="server">
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblReconciliationFlt" runat="server" CssClass="labels" Text="Schválení: " ToolTip="" Visible="false"></asp:Label><br />
								<asp:DropDownList ID="ddlReconciliationFlt" runat="server" Visible="false">
									<asp:ListItem Value="-1" Selected="True" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="1" Text="Schválené"></asp:ListItem>
									<asp:ListItem Value="2" Text="Zamítnuté"></asp:ListItem>
								</asp:DropDownList>
							</td>
							<td colspan="2">
								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>
							</td>
						</tr>

						<tr>
							<td style="vertical-align: bottom; height: 30px;">
								<%--<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="excelSmall_button" runat="server" ImageUrl="~/img/excelSmall.png" OnClick="excelSmall_button_Click" Height="30px" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="excelSmall_button" runat="server" ImageUrl="~/img/excelSmall.png" OnClick="excelSmall_button_Click" Height="30px" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="search_button" runat="server" AccessKey="F" AlternateText="Vyhledat" ImageUrl="~/img/search_button.png" OnClick="search_button_Click" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Filtr" />
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
							<asp:TemplateField HeaderText="K1">
								<ItemTemplate>
									<asp:ImageButton ID="btnK1new" runat="server" CommandArgument='<%# Eval("ID") %>'
										CommandName="K1New" ImageUrl="~/img/edit.png" ToolTip="ruční K1"
										Visible="true" Enabled="false" />
								</ItemTemplate>
								<ItemStyle Wrap="False" />
							</asp:TemplateField>
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="MessageTypeID" HeaderText="MessageType" ReadOnly="True" />
							<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True"  />
							<asp:BoundField DataField="KitDateOfDelivery" HeaderText="Pož. datum kompletace" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
								<HeaderStyle Wrap="True" />
								<ItemStyle HorizontalAlign="Right" Wrap="True" />
							</asp:BoundField>
							<asp:BoundField DataField="DescriptionCz" HeaderText="Satus" ReadOnly="True" />
							<asp:BoundField DataField="CompanyName" HeaderText="Kompletač.firma" ReadOnly="True" />
							<asp:BoundField DataField="ModifyDate" HeaderText="Editováno" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:TemplateField>
								<ItemTemplate>
									<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
										CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce"
										Visible="true" />
								</ItemTemplate>
								<ItemStyle Wrap="False" />
							</asp:TemplateField>
							<asp:BoundField DataField="Reconciliation" HeaderText="Reconciliation" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
					<asp:Panel ID="pnlItems" runat="server" Visible="false">
						<h3>Detail objednávky</h3>
						<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="ID" Width="10px" OnSelectedIndexChanged="gvItems_SelectedIndexChanged">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
								<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									<ItemStyle Width="10px" />
								</asp:BoundField>
								<asp:BoundField DataField="KitId" HeaderText="KitId" />
								<asp:BoundField DataField="KitDescription" HeaderText="Popis kitu" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQuantityDeliveredInt" HeaderText="Dod.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQualityCode" HeaderText="Kvalita" />
								<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" ReadOnly="True" />
								<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
					</asp:Panel>
					<br />
					<asp:Panel ID="pnlCardStockItems" runat="server" Visible="false">
						<h3>Skladová karta KITu</h3>
						<asp:GridView ID="gvCardStockItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
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
								<asp:BoundField DataField="Code" HeaderText="Kód" />
								<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Množství" ItemStyle-Wrap="False" Visible="false">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemOrKitFreeInt" HeaderText="Množství volné" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemOrKitUnConsilliationInt" HeaderText="Množství ke schválení" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemOrKitReservedInt" HeaderText="Množství rezervované" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemOrKitReleasedForExpeditionInt" HeaderText="Množství uvolněné" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="MeasuresCode" HeaderText="MeJe" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQualitiesCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
					</asp:Panel>
					<br />
				    <asp:Panel ID="pnlK1" runat="server" Visible="false">
						<asp:GridView ID="gvK1" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="ID" Width="10px">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									<ItemStyle Width="10px" />
								</asp:BoundField>
								<asp:BoundField DataField="KitId" HeaderText="KitId" />
								<asp:BoundField DataField="KitDescription" HeaderText="Popis kitu" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:TemplateField HeaderText="K1 - dod.množství">
									<ItemTemplate>
										<asp:TextBox ID="tbxQuantity" runat="server" MaxLength="9"></asp:TextBox>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="KitQuantityDeliveredInt" HeaderText="Dod.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitQualityCode" HeaderText="Kvalita" />
								<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" ReadOnly="True" />
								<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="MeasuresID" HeaderText="MeasuresID" />
								<asp:BoundField DataField="KitQualityId" HeaderText="KitQualityId" />

							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						<br />
						<asp:Button ID="btnK1Save" runat="server" Text="Vytvoř K1" Width="100px" OnClick="btnK1Save_Click" />&nbsp;&nbsp;
						<asp:Button ID="btnK1Back" runat="server" Text="Zpět" Width="100px" OnClick="btnK1Back_Click" />
					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>

</asp:Content>
