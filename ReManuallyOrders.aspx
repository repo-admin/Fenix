<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="ReManuallyOrders.aspx.cs" Inherits="Fenix.ReManuallyOrders" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>R0 - objednávka nového zboží</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblFirmaFlt" runat="server" CssClass="labels" Text="Firma: "></asp:Label>
								<br />								
                                <asp:DropDownList ID="ddlCompanyName" runat="server" CssClass="ddlfilter"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCityFlt" runat="server" CssClass="labels" Text="Město: " Visible="False"></asp:Label>
								<br />								
                                <asp:DropDownList ID="ddlCityName" runat="server" CssClass="ddlfilter" Visible="False"></asp:DropDownList>
							</td>
							<td colspan="2">
								<asp:Label ID="lblJmenoFlt" runat="server" CssClass="labels" Text="Typ obj.: " Visible="False"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlOrderType" runat="server" CssClass="ddlfilterbig" Visible="False"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:Label ID="lblStatusFlt" runat="server" CssClass="labels" Text="Status:"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlMessageStatusFlt" runat="server" CssClass="ddlfilterbig"></asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumOdeslaniFlt" runat="server" CssClass="labels" Text="Datum odesl. na ND: " ToolTip="Datum odeslání na ND"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumOdeslaniFlt" runat="server" MaxLength="10"></asp:TextBox>
								<cc2:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="tbxDatumOdeslaniFlt" CssClass="calendars"></cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumDodaniFlt" runat="server" CssClass="labels" Text="Datum pož. dodání: " ToolTip="Datum požadovaného dodání:"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumDodaniFlt" runat="server" MaxLength="10"></asp:TextBox>
								<cc2:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="tbxDatumDodaniFlt" CssClass="calendars"></cc2:CalendarExtender>
							</td>
							<td colspan="1" style="vertical-align: bottom">
								<asp:Label ID="lblUsersModifyFlt" runat="server" CssClass="labels" Text="Zadavatel: " Visible="False"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlUsersModifyFlt" runat="server" Visible="False"></asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td>
								<asp:Label ID="lblObjednavkaIDFlt" runat="server" CssClass="labels" Text="ID objednávky: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxObjednavkaIDFlt" runat="server" MaxLength="10" Width="61px"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;
								<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />
								&nbsp;
							</td>
						</tr>
					</table>				</div>
				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label>
				<br />
				<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged" OnRowCommand="grdData_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:TemplateField HeaderText="R1" Visible="true">
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
						<asp:BoundField DataField="DescriptionCz" HeaderText="Stav" ReadOnly="True" ItemStyle-Width="200"/>
						<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" />
						<asp:BoundField DataField="ItemDateOfDelivery" HeaderText="Požadované datum uskladnění" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
							<HeaderStyle Wrap="True" />
							<ItemStyle HorizontalAlign="Right" Wrap="True" />
						</asp:BoundField>
						<asp:BoundField DataField="Notice" HeaderText="Poznámka" ReadOnly="True" />
						<asp:BoundField DataField="HeliosOrderId" HeaderText="Helios" />
						<asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ReadOnly="True" />
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

                        <asp:BoundField DataField="MessageStatusId" HeaderText="Status zprávy" />

						<asp:TemplateField HeaderText="Zrušit" Visible ="true">
							<ItemTemplate>
								<asp:ImageButton ID="btnDeleteOrder" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="DeleteOrder" ImageUrl="~/img/delete_dustbin.png" ToolTip="Zrušit objednávku" OnClientClick="ConfirmDeleteOrder()"
									Visible="true" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>

					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />

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
			</asp:View>

			<asp:View ID="vwEdit" runat="server">
				<table>
					<tr>
						<td colspan="10" align="center">
							<h1>Ručně vkládaná objednávka</h1>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStock" runat="server" CssClass="labels" Text="sklad (!): " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlStock" runat="server">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Label ID="lblItemType" runat="server" CssClass="labels" Text="Typ: " ToolTip="Typ:"></asp:Label><br />
							<asp:DropDownList ID="ddlItemType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemType_SelectedIndexChanged">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Label ID="lblGroupGoods" runat="server" CssClass="labels" Text="SkZboží: " ToolTip="Skupina zboží"></asp:Label><br />
							<asp:DropDownList ID="ddlGroupGoods" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGroupGoods_SelectedIndexChanged">
							</asp:DropDownList>
						</td>
						<td colspan="5">
							<asp:Label ID="lblItems" runat="server" CssClass="labels" Text="Items (!): " ToolTip="Zboží, zařízení"></asp:Label>
							<asp:CheckBox ID="chkbItems" runat="server" AutoPostBack="True" OnCheckedChanged="chkbItems_CheckedChanged" />
							<br />
							<asp:DropDownList ID="ddlItems" runat="server" AutoPostBack="true" OnTextChanged="ddlItems_TextChanged" Width="300px">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Label ID="lblMeJe" runat="server" CssClass="labels" Text="MeJe: " ToolTip="Měrná jednotka"></asp:Label><br />
							<asp:UpdatePanel ID="UpdatePanelMeJe" runat="server" UpdateMode="Conditional">
								<ContentTemplate>
									<asp:Label ID="lblMeJeValue" runat="server" Text="" ToolTip=""></asp:Label>
								</ContentTemplate>
								<Triggers><asp:AsyncPostBackTrigger ControlID="ddlItems" EventName="TextChanged" /></Triggers>
<%--								<Triggers><asp:AsyncPostBackTrigger ControlID="ddlGroupGoods" EventName="SelectedIndexChanged" /></Triggers>
								<Triggers><asp:AsyncPostBackTrigger ControlID="ddlItemType" EventName="SelectedIndexChanged" /></Triggers>--%>
							</asp:UpdatePanel>
							<asp:Label ID="lblMeJeId" runat="server" CssClass="labels" Text="" ToolTip="" Visible="false"></asp:Label>
						</td>
						<td>&nbsp;<asp:Label ID="lblQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip="Objednávané množství"></asp:Label><br />
							&nbsp;<asp:TextBox ID="tbxQuantity" runat="server"></asp:TextBox>
						</td>

					</tr>
					<tr>
						<td colspan="9">
							<asp:Label ID="lblSupplier" runat="server" CssClass="labels" Text="Dodavatel: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlSupplier" runat="server">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Label ID="lblDateOfDelivery" runat="server" CssClass="labels" Text="Datum dodání: " ToolTip="Požadované datum dodání"></asp:Label><br />
							<asp:TextBox ID="tbxDateOfDelivery" runat="server"></asp:TextBox>
							<cc2:CalendarExtender ID="ceDateOfDelivery" runat="server" TargetControlID="tbxDateOfDelivery" CssClass="calendars"></cc2:CalendarExtender>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Button ID="btnPridatDoObjednavky" runat="server" Text="Přidat do objednávky" OnClick="btnPridatDoObjednavky_Click" />
						</td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td>
							<asp:Label ID="lblSource" runat="server" CssClass="labels" Text="Zdroj obj.: " ToolTip="zdroj objednávky může být z různých aplikací (Helios, RFA,...)"></asp:Label><br />
							<asp:DropDownList ID="ddlSource" runat="server">
							</asp:DropDownList>
						</td>
						<td>&nbsp;<asp:Label ID="lblHeliosOrderID" runat="server" CssClass="labels" Text="Id Helios, RFA, ...: " ToolTip="ID objednávky v Heliosu nebo RFA nebo..."></asp:Label><br />
							&nbsp;<asp:TextBox ID="tbxHeliosOrderID" runat="server"></asp:TextBox>
						</td>

					</tr>
					<tr>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
					</tr>
				</table>
				<br />
				<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids">
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
						<asp:BoundField DataField="StockId" HeaderText="StockId" ReadOnly="True" />
						<asp:BoundField DataField="StockName" HeaderText="StockName" />
						<asp:BoundField DataField="ItemsId" HeaderText="ItemsId" />
						<asp:BoundField DataField="ItemsDescriptionCZ" HeaderText="ItemsDescriptionCZ" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="ItemQuantity" HeaderText="ItemQuantity" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="MeasuresId" HeaderText="MeasuresId" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="MeasuresCode" HeaderText="MeasuresCode" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="DateOfDelivery" HeaderText="DateOfDelivery" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="SupplierId" HeaderText="SupplierId" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="SupplierNazev" HeaderText="SupplierNazev" ItemStyle-Wrap="False" >

						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="SourceId" HeaderText="SourceId" />
						<asp:BoundField DataField="SourceCode" HeaderText="SourceCode" />
						<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" />

					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<br />
				<asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="190px" />&nbsp;
				<asp:Button ID="btnSave" runat="server" Text="Uložit" Width="190px" />
			</asp:View>
		</asp:MultiView>
	</div>
</asp:Content>
