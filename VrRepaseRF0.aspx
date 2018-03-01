<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="VrRepaseRF0.aspx.cs" Inherits="Fenix.VrRepaseRF0" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>RF0 - Refurbished Order</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblFirmaFlt" runat="server" CssClass="labels" Text="Firma: "></asp:Label>
								<br />								
                                <asp:DropDownList ID="ddlCompanyName" runat="server" CssClass="ddlfilter"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCityFlt" runat="server" CssClass="labels" Text="Město: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlCityName" runat="server" CssClass="ddlfilter"></asp:DropDownList>
							</td>
							<td colspan="2">
								<asp:Label ID="lblJmenoFlt" runat="server" CssClass="labels" Text="Příjmení: " Visible="False"></asp:Label>
								<br />
								<asp:TextBox ID="tbxJmenoFlt" runat="server" MaxLength="20" Visible="False"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:Label ID="lblStatusFlt" runat="server" CssClass="labels" Text="Status:"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlMessageStatusFlt" runat="server" Width="200px">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumOdeslaniFlt" runat="server" CssClass="labels" Text="Datum odesl. na ND: " ToolTip="Datum odeslání na ND"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumOdeslaniFlt" runat="server" MaxLength="12"></asp:TextBox>
                                <cc2:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="tbxDatumOdeslaniFlt" CssClass="calendars">
                                </cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumDodaniFlt" runat="server" CssClass="labels" Text="Datum pož. dodání: " ToolTip="Datum požadovaného dodání:"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumDodaniFlt" runat="server" MaxLength="12"></asp:TextBox>
                                <cc2:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="tbxDatumDodaniFlt" CssClass="calendars" >
                                </cc2:CalendarExtender>
							</td>
							<td colspan="2">
								<asp:Label ID="lblUsersModifyFlt" runat="server" CssClass="labels" Text="Zadavatel: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlUsersModifyFlt" runat="server"></asp:DropDownList>								
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
				<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged" OnRowCommand="grdData_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:TemplateField HeaderText="RF1">
							<ItemTemplate>
								<asp:ImageButton ID="btnRF1new" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="RF1New" ImageUrl="~/img/edit.png" ToolTip="ruční S1"
									Visible="true" Enabled="false" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
						<asp:BoundField DataField="MessageTypeID" HeaderText="MessageType" ReadOnly="True" />
						<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
						<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" DataFormatString="{0:d}" ItemStyle-Wrap="false" />
						<asp:BoundField DataField="DateOfDelivery" HeaderText="Pož. datum dodání" ReadOnly="True" DataFormatString="{0:d}">
							<HeaderStyle Wrap="True" />
							<ItemStyle HorizontalAlign="Right" Wrap="false" />
						</asp:BoundField>
						<asp:BoundField DataField="DescriptionCz" HeaderText="Satus" ReadOnly="True" />
						<asp:BoundField DataField="CustomerDescription" HeaderText="Firma" ReadOnly="True" />
						<asp:BoundField DataField="CustomerCity" HeaderText="Město" />
						<asp:BoundField DataField="StreetName" HeaderText="Ulice" />
						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce"
									Visible="true" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Excel" >
                            <HeaderTemplate>
								<asp:ImageButton ID="btnExcelConf" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="btnExcelConfClicked" ImageUrl="~/img/excelSmall.png" ToolTip="Suma zaškrtnutých confirmací"
									Visible="true"  Height="20px" Width="20px"/>
								<asp:Button ID="btnOznacit" runat="server" Text="*" CommandName="btnOznacit" Height="20px" Width="20px"/>
                              </HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox ID="chkbExcel" runat="server" />
							</ItemTemplate>
							<HeaderStyle Wrap="False" />
						</asp:TemplateField>
						<asp:BoundField DataField="Reconciliation" HeaderText="Reconciliation" />
                        <asp:BoundField DataField="ModifyUserLastName" HeaderText="Zadavatel" />

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
				<asp:Panel ID="pnlGvItems" Visible="false" runat="server">
					<h3>Detail</h3>
					<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<%--<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />--%>
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemVerKitText" HeaderText="Item/kit">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitDescription" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityDeliveredInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
				</asp:Panel>
				<asp:Panel ID="pnlR1" runat="server">
					<table>
						<tr>
							<td>
								<table>
									<tr>
										<td valign="bottom">
											<asp:Label ID="Label7" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxCustomerNameValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="Label8" runat="server" CssClass="labels" Text="Město: " ToolTip=""></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxCustomerCityValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
									</tr>
									<tr>
										<td valign="bottom">
											<asp:Label ID="Label9" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxCustomerAddress1Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="Label10" runat="server" CssClass="labels" Text="Č.popisné: " ToolTip=""></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxCustomerAddress2Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
									</tr>
									<tr>
										<td valign="bottom">
											<asp:Label ID="Label12" runat="server" CssClass="labels" Text="PSČ: " ToolTip=""></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxCustomerZipCodeValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="Label13" runat="server" CssClass="labels" Text="Pož.datum dodání: "></asp:Label></td>
										<td valign="bottom">
											<asp:Label ID="lblxRequiredDateOfShipmentValue" runat="server" CssClass="plaintext" Text="" ToolTip="" DataFormatString="{0:d}" Font-Bold="True"></asp:Label></td>
									</tr>
								</table>

							</td>
						</tr>
						<tr>
							<td>
								<asp:GridView ID="gvRF1" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
									DataKeyNames="ID" Width="10px">
									<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
									<FooterStyle CssClass="gridfooter" />
									<RowStyle CssClass="gridrows" />
									<AlternatingRowStyle CssClass="gridrowalter" />
									<Columns>
										<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
											<ItemStyle Width="10px" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemVerKitText" HeaderText="Item/kit">
											<ItemStyle HorizontalAlign="Right" Wrap="False" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
											<ItemStyle Wrap="False" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemOrKitDescription" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
											<ItemStyle Wrap="False" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
											<ItemStyle HorizontalAlign="Right" Wrap="False" />
										</asp:BoundField>
										<asp:TemplateField HeaderText="RF1 - dod.množství">
											<ItemTemplate>
												<asp:TextBox ID="tbxQuantity" runat="server" MaxLength="9"></asp:TextBox>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:BoundField DataField="ItemOrKitQuantityDeliveredInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
											<ItemStyle HorizontalAlign="Right" Wrap="False" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
											<ItemStyle Wrap="False" />
										</asp:BoundField>
										<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
											<ItemStyle Wrap="False" />
										</asp:BoundField>
									</Columns>
									<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
								</asp:GridView>
							</td>
						</tr>
					</table>
					<br />
					<asp:Button ID="btnRF1Save" runat="server" Text="Vytvoř RF1" Width="100px" OnClick="btnRF1Save_Click" />&nbsp;&nbsp;
						<asp:Button ID="btnRF1Back" runat="server" Text="Zpět" Width="100px" OnClick="btnRF1Back_Click" />
				</asp:Panel>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<table>
					<tr>
						<td colspan="10" align="center">
							<h1>Nový požadavek na naskladnění (repase)</h1>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStock" runat="server" CssClass="labels" Text="Na sklad: "></asp:Label><br />
							<asp:DropDownList ID="ddlStock" runat="server">
							</asp:DropDownList>
						</td>
						<td colspan="3">
							<asp:Label ID="lblDestinationPlaces" runat="server" CssClass="labels" Text="Odkud místo: "></asp:Label><br />
							<asp:DropDownList ID="ddlDestinationPlaces" runat="server">
							</asp:DropDownList>
						</td>
						<td colspan="3">
							<br />
						</td>
						<td>
							<br />
						</td>
						<%--						<td>
							<asp:Label ID="lblKitQualities" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlKitQualities" runat="server" Width="60px">
							</asp:DropDownList>
						</td>--%>
						<%--						<td>
							<asp:Label ID="lblCardStockItems" runat="server" CssClass="labels" Text="Název (český): " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlCardStockItems" runat="server" Width="60px">
							</asp:DropDownList>
						</td>--%>
						<td colspan="3">
							<asp:Label ID="lblDateOfDelivery" runat="server" CssClass="labels" Text="Datum dodání: " ToolTip="Požadované datum expediceí"></asp:Label><br />
							<asp:TextBox ID="tbxDateOfDelivery" runat="server" Text="31.8.2014"></asp:TextBox>
							<cc2:CalendarExtender ID="ceDateOfDelivery" runat="server" CssClass="calendars" TargetControlID="tbxDateOfDelivery">
							</cc2:CalendarExtender>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<table>
								<tr>
									<td>
										<asp:Label ID="lblKitGroups" runat="server" CssClass="labels" Text="Skupina: " ToolTip=""></asp:Label><br />
										<asp:DropDownList ID="ddlKitGroups" runat="server" Width="70px" AutoPostBack="True" OnSelectedIndexChanged="ddlKitGroups_SelectedIndexChanged">
										</asp:DropDownList>
									</td>
									<td>
										<asp:Label ID="lblKitQualities" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
										<asp:DropDownList ID="ddlKitQualities" runat="server" Width="60px" AutoPostBack="True" OnSelectedIndexChanged="ddlKitQualities_SelectedIndexChanged" ToolTip="Toto slouží pro filtraci">
										</asp:DropDownList>
									</td>
									<td>
										<asp:Label ID="lblKitQuality" runat="server" Text="Kvalita: " Font-Bold="True" ForeColor="#993300"></asp:Label><br />
										<asp:DropDownList ID="ddlKitQuality" runat="server" Width="100px" Enabled="False" ToolTip="Toto se zapisuje do RF0">
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
						<td colspan="5">
							<table>
								<tr>
									<td>
										<asp:Label ID="lblItemType" runat="server" CssClass="labels" Text="Typ: " ToolTip="Typ:"></asp:Label>&nbsp;&nbsp;<br />
										<asp:DropDownList ID="ddlItemType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemType_SelectedIndexChanged">
										</asp:DropDownList>
									</td>
									<td>
										<asp:Label ID="lblGroupGoods" runat="server" CssClass="labels" Text="SkZboží: " ToolTip="Skupina zboží"></asp:Label>&nbsp;&nbsp;<br />
										<asp:DropDownList ID="ddlGroupGoods" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGroupGoods_SelectedIndexChanged">
										</asp:DropDownList>
									</td>
									<td>
										<asp:Label ID="lblItemQuality" runat="server" Text="Kvalita: " ToolTip="Toto se zapisuje do RF0" Font-Bold="True" ForeColor="#993300"></asp:Label><br />
										<asp:DropDownList ID="ddlItemQuality" runat="server" Width="100px">
										</asp:DropDownList>
									</td>

								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<asp:Label ID="lblKits" runat="server" CssClass="labels" Text="Kits: "></asp:Label><br />
							<asp:DropDownList ID="ddlKits" runat="server" Width="350px" AutoPostBack="True" OnSelectedIndexChanged="ddlKits_SelectedIndexChanged">
							</asp:DropDownList><br />
							<asp:Label ID="lblKitsQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="tbxKitsQuantity" runat="server" Text="1"></asp:TextBox>

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
							<asp:Button ID="btnPridatCpeDoSoupravy" runat="server" Text="Přidat do objednávky" OnClick="btnPridatCpeDoSoupravy_Click" />
						</td>
						<td colspan="5">
							<asp:Button ID="btnPridatNwDoSoupravy" runat="server" Text="Přidat do objednávky" OnClick="btnPridatNwDoSoupravy_Click" />
						</td>
					</tr>
				</table>
				<asp:Label ID="lblErrInfo" runat="server" CssClass="errortext"></asp:Label>
				<br />
				<asp:GridView ID="gvKitsOrItemsNew" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids">
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
						<asp:BoundField DataField="ID" HeaderText="ID" />
						<asp:BoundField DataField="ItemVerKit" HeaderText="Kit/Item" ReadOnly="True" />
						<asp:BoundField DataField="ItemOrKitID" HeaderText="ID Kit/Item" ReadOnly="True" />
						<asp:BoundField DataField="ItemOrKitCode" HeaderText="Kód" ReadOnly="True" />
						<asp:BoundField DataField="DescriptionCzItemsOrKit" HeaderText="Popis" ReadOnly="True" />
						<asp:BoundField DataField="ItemOrKitQuantity" HeaderText="Množství" ReadOnly="True" />
						<asp:BoundField DataField="PackageTypeId" HeaderText="Typ packaging" />
						<asp:BoundField DataField="cdlStocksName" HeaderText="Sklad - kam" />
						<asp:BoundField DataField="DestinationPlacesId" HeaderText="Odkud Id" />
						<asp:BoundField DataField="DestinationPlacesName" HeaderText="Odkud název" />
						<asp:BoundField DataField="DestinationPlacesContactsId" HeaderText="Od koho Id" />
						<asp:BoundField DataField="DestinationPlacesContactsName" HeaderText="Od koho jméno" />
						<asp:BoundField DataField="DateOfDelivery" HeaderText="DateOfDelivery" />
						<asp:BoundField DataField="StockId" HeaderText="StockId" />

						<asp:BoundField DataField="QualityID" HeaderText="QualityID" />
						<asp:BoundField DataField="QualityText" HeaderText="QualityText" />

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
