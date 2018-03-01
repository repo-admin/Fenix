<%@ Page Title="" Language="C#" MasterPageFile="~/UPC.Master" AutoEventWireup="true" CodeBehind="HomeReconciliation.aspx.cs" Inherits="Fenix.HomeReconciliation" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="BaseHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BaseMainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>R1 - Schválení příjmu zboží</h1>
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
							<td style="vertical-align: bottom; height: 30%;"></td>
						</tr>

						<tr>
							<td style="vertical-align: bottom; height: 30px;">
								<%--<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="excelSmall_button" runat="server" ImageUrl="~/img/excelSmall.png" OnClick="excelSmall_button_Click" Height="30px" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="search_button" runat="server" AccessKey="F" AlternateText="Vyhledat" ImageUrl="~/img/search_button.png" OnClick="search_button_Click" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
						</tr>

					</table>
				</div>
				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label>
				<br />
				<div id="divData" runat="server" style="width:auto;">
				<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged" OnRowCommand="grdData_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
						<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
						<asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Skutečné datum uskladnění" ReadOnly="True" DataFormatString="{0:d}" />
						<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
						<asp:BoundField DataField="ItemDateOfDelivery" HeaderText="Požadované datum dodání" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
							<%--<ItemStyle HorizontalAlign="Right" Wrap="True" />--%>
						<HeaderStyle Wrap="True" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ReadOnly="True" />
						<asp:BoundField DataField="HeliosObj" HeaderText="Helios obj." />
								<asp:TemplateField>
									<ItemTemplate>
										<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
											CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce a konfirmaci"
											Visible="true" />
									</ItemTemplate>
									<ItemStyle Wrap="False" />
								</asp:TemplateField>
                        <asp:BoundField DataField="Reconciliation" HeaderText="Reconcilace" ReadOnly="True" Visible="false"/>
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />
				<br />
				<asp:panel ID="pnlItems" runat="server" visible="false"> 
				<h3>Detailní položky schvalované Confirmace</h3>
				<table>
					<tr>
						<td>
							<asp:GridView ID="gvConfirmationItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
								DataKeyNames="ID" OnSelectedIndexChanged="gvConfirmationItems_SelectedIndexChanged" Width="10px">
								<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
								<FooterStyle CssClass="gridfooter" />
								<RowStyle CssClass="gridrows" />
								<AlternatingRowStyle CssClass="gridrowalter" />
								<Columns>
									<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
									<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
										<ItemStyle Width="10px" />
									</asp:BoundField>
									<asp:BoundField DataField="ItemID" HeaderText="ItemID" />
									<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
									<asp:BoundField DataField="Code" HeaderText="Kód" />
									<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
										<ItemStyle Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
										<ItemStyle Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="ItemQuantity" HeaderText="Naskladněné.množ." ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="CMRSIItemQuantity" HeaderText="Objednané.množ." ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
										<ItemStyle Wrap="False" />
									</asp:BoundField>
								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
						</td></tr>
					<tr>
						<td nowrap="nowrap">
							<table>
								<tr>
									<td style="bottom: auto">
										<asp:RadioButtonList ID="rdblDecision" runat="server" RepeatDirection="Horizontal">
											<asp:ListItem Value="1">Schvaluji</asp:ListItem>
                                            <%--<asp:ListItem Value="2">ZAMÍTÁM !</asp:ListItem>--%>
											<asp:ListItem Value="3">ZAMÍTÁM !</asp:ListItem>
										</asp:RadioButtonList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									</td>
									<td style="bottom: auto">
										<asp:Button ID="btnDecision" runat="server" Text="Rozhodněte" OnClick="btnDecision_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
									</td>
									<td style="bottom: auto">
										<asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="Zpět" Width="100px" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
					<asp:panel ID="pnlDetails" runat="server" visible="false"> 
				<br />
				<h3>Příslušné skladové karty</h3>
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
						<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
						<asp:BoundField DataField="Code" HeaderText="Kód" />
						<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemOrKitQuantity" HeaderText="Množství" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemOrKitFree" HeaderText="Množství volné" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemOrKitUnConsilliation" HeaderText="Množství ke schválení" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemOrKitReserved" HeaderText="Množství rezervované" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemOrKitReleasedForExpedition" HeaderText="Množství uvolněné" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="PC" HeaderText="MeJe" ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<br />
				<h3>Historie Confirmací</h3>
				<asp:GridView ID="gvConfirmationItemsHistory" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID"  Width="10px">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
							<ItemStyle Width="10px" />
						</asp:BoundField>
						<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
						<asp:BoundField DataField="Code" HeaderText="Kód" />
						<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemQuantity" HeaderText="Obj.množ." ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
						</asp:BoundField>
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
						</asp:panel>
				</asp:panel>
</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>
