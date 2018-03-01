<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="ReReceptionBrowse.aspx.cs" Inherits="Fenix.ReReceptionBrowse" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>R - Přehled objednávek</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="MessageId: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxMessageIdFlt" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageStatusFlt" runat="server" CssClass="labels" Text="MessageStatus: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlMessageStatusFlt" runat="server" Width="150px">
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblSuppliers" runat="server" CssClass="labels" Text="Dodavatelé: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlSuppliersFlt" runat="server" Width="150px">
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblReconciliationFlt" runat="server" CssClass="labels" Text="Schválení: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlReconciliationFlt" runat="server">
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
							<td style="vertical-align: bottom; height: 30px;" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;
									<%--<asp:ImageButton ID="search_button" runat="server" AccessKey="F" AlternateText="Vyhledat" ImageUrl="~/img/search_button.png" OnClick="search_button_Click" />--%>&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Filtr" Width="100px"/>
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
							<asp:TemplateField HeaderText="R1">
								<ItemTemplate>
									<asp:ImageButton ID="btnR1new" runat="server" CommandArgument='<%# Eval("ID") %>'
										CommandName="R1New" ImageUrl="~/img/edit.png" ToolTip="ruční R1"
										Visible="true" Enabled="false" />
								</ItemTemplate>
								<ItemStyle Wrap="False" />
							</asp:TemplateField>
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True"  />
							<asp:BoundField DataField="ItemDateOfDelivery" HeaderText="Požadované datum dodání" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
								<%--<ItemStyle HorizontalAlign="Right" Wrap="True" />--%>
								<HeaderStyle Wrap="True" />
							</asp:BoundField>
							<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ReadOnly="True" />
							<asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ReadOnly="True" />
							<asp:BoundField DataField="HeliosObj" HeaderText="Helios obj." />
							<asp:BoundField DataField="ModifyDate" DataFormatString="{0:d}" HeaderText="Editováno" />
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

					<asp:Label ID="Label1" runat="server" CssClass="labels" Text=""></asp:Label>
					<br />
					<asp:Panel ID="pnlReConf" runat="server" Visible="false">
						<h3>Detail objednávky</h3>
						<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="ID" Width="10px">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/text.png" SelectText="Detail" ShowSelectButton="true" ItemStyle-Width="10px">
									<ItemStyle Width="10px" />
								</asp:CommandField>
								<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									<ItemStyle Width="10px" />
								</asp:BoundField>
								<asp:BoundField DataField="HeliosOrderRecordId" HeaderText="HeliosRecordId" ReadOnly="True" />
								<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
								<asp:BoundField DataField="ItemCode" HeaderText="Kód" />
								<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemQuantityDeliveredInt" HeaderText="Dod.množ." ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" HorizontalAlign="Right" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">

									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" />
								<asp:BoundField DataField="SourceCode" HeaderText="Source" />
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						<br />
						<h3>Confirmace</h3>

						<asp:GridView ID="gvReConf" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="ID" OnSelectedIndexChanged="gvReConf_SelectedIndexChanged" Width="804px" OnRowCommand="gvReConfOnRowCommand">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
								<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
								<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
								<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
								<%--<asp:BoundField DataField="DescriptionCz" HeaderText="Stav" ReadOnly="True" />--%>
								<asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Obdrženo z ND" ReadOnly="True" DataFormatString="{0:d}" />
								<%--<asp:BoundField DataField="ItemDateOfDelivery" HeaderText="Požadované datum uskladnění" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
									<ItemStyle HorizontalAlign="Right" Wrap="True" />
								</asp:BoundField>--%>
								<asp:BoundField DataField="ReconciliationYesNo" HeaderText="Odsouhlaseno" ReadOnly="True" />
								<asp:BoundField DataField="Notice" HeaderText="Poznámka" ReadOnly="True" />
								<asp:BoundField DataField="HeliosOrderId" HeaderText="Helios" />
								<asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ReadOnly="True" />
								<asp:TemplateField>
									<ItemTemplate>
										<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
											CommandName="SerNumView" ImageUrl="~/img/excelSmall.png" ToolTip="Sériová čísla"
											Visible="true" />
									</ItemTemplate>
									<ItemStyle Wrap="False" />
								</asp:TemplateField>

							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						<cc1:Pager ID="Pager1" runat="server" />
						<asp:Label ID="lblSerialNumbers" runat="server" ForeColor="#cc0000" Text="" Enabled="False" Font-Bold="True"></asp:Label>

					</asp:Panel>
					<br />
					<asp:Panel ID="pnlItems" runat="server" Visible="false">
						<h3>Detailní položky confirmace</h3>
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
											<asp:BoundField DataField="ItemQuantityInt" HeaderText="Naskladněné.množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="CMRSIItemQuantity" HeaderText="Objednané.množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="NDReceipt" HeaderText="ND doklad" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
										</Columns>
										<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
									</asp:GridView>
								</td>
								<td>&nbsp;</td>
							</tr>
						</table>
						<asp:Panel ID="pnlDetails" runat="server" Visible="false">
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
									<asp:BoundField DataField="PC" HeaderText="MeJe" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
							<br />
							<table id="tblConvirmace" width="100%">
								<tr>
									<td style="width: 50%">
										<h3>Historie confirmací</h3>
										<asp:GridView ID="gvConfirmationItemsHistory" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
											DataKeyNames="ID" Width="10px">
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
												<asp:BoundField DataField="ItemQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
													<ItemStyle HorizontalAlign="Right" Wrap="False" />
												</asp:BoundField>
												<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
													<ItemStyle Wrap="False" />
												</asp:BoundField>
											</Columns>
											<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
										</asp:GridView>
									</td>
									<td style="width: 50%; visibility: hidden;">
										<h3>Historie objednávek</h3>
										<asp:GridView ID="gvOrdersItemsHistory" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
											DataKeyNames="ID" Width="10px">
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
									</td>
								</tr>
							</table>
						</asp:Panel>
					</asp:Panel>
					<br />
					<asp:Panel ID="pnlR1" runat="server" Visible="false">
						<asp:GridView ID="gvR1" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							DataKeyNames="ID" Width="10px">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
<%--								<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/text.png" SelectText="Detail" ShowSelectButton="true" ItemStyle-Width="10px">
									<ItemStyle Width="10px" />
								</asp:CommandField>--%>
								<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									<ItemStyle Width="10px" />
								</asp:BoundField>
								<asp:BoundField DataField="HeliosOrderRecordId" HeaderText="HeliosRecordId" ReadOnly="True" />
								<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
								<asp:BoundField DataField="ItemCode" HeaderText="Kód" />
								<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemQuantity" HeaderText="Obj.množ." ItemStyle-Wrap="False">
									<ItemStyle HorizontalAlign="Right" Wrap="False" />
								</asp:BoundField>
								<asp:TemplateField HeaderText="R1 - dod.množství">
									<ItemTemplate>
										<asp:TextBox ID="tbxQuantity" runat="server" MaxLength="9"></asp:TextBox>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:BoundField DataField="ItemQuantityDeliveredInt" HeaderText="Dod.množ." ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" />
								<asp:BoundField DataField="SourceCode" HeaderText="Source" />
								<asp:BoundField DataField="ItemId" HeaderText="ItemId" />
								<asp:BoundField DataField="MeasuresID" HeaderText="MeasuresID" />
								<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="ItemUnitOfMeasure" />
								<asp:BoundField DataField="ItemQualityId" HeaderText="ItemQualityId" />
								<asp:BoundField DataField="ItemQualityCode" HeaderText="ItemQualityCode" />
								<asp:BoundField DataField="CMSOId" HeaderText="CMSOId" />
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						<br />
						<asp:Button ID="btnR1Save" runat="server" Text="Vytvoř R1" Width="100px" OnClick="btnR1Save_Click" />&nbsp;&nbsp;
						<asp:Button ID="btnR1Back" runat="server" Text="Zpět" Width="100px" OnClick="btnR1Back_Click" />
					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>
