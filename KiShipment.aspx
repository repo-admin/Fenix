<%@ Page Title="" Language="C#" MasterPageFile="~/Expedition.master" AutoEventWireup="true" CodeBehind="KiShipment.aspx.cs" Inherits="Fenix.KiShipment" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>S0 - Objednávka expedice</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblFirmaFlt" runat="server" CssClass="labels" Text="Firma: "></asp:Label>
								<br />								
                                <asp:DropDownList ID="ddlCompanyName" runat="server" CssClass="ddlfilter"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblCityFlt" runat="server" CssClass="labels" Text="Město: "></asp:Label>
								<br />								
                                <asp:DropDownList ID="ddlCityName" runat="server" CssClass="ddlfilter"></asp:DropDownList>
							</td>
							<td colspan="2" style="vertical-align: bottom; height: 40%;">
								<asp:Label ID="lblJmenoFlt" runat="server" CssClass="labels" Text="Typ obj.: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlOrderType" runat="server" CssClass="ddlfilterbig"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblStatusFlt" runat="server" CssClass="labels" Text="Status:"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlMessageStatusFlt" runat="server" CssClass="ddlfilterbig"></asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblDatumOdeslaniFlt" runat="server" CssClass="labels" Text="Datum odesl. na ND: " ToolTip="Datum odeslání na ND"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumOdeslaniFlt" runat="server" MaxLength="10"></asp:TextBox>
								<cc2:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="tbxDatumOdeslaniFlt" CssClass="calendars"></cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblDatumDodaniFlt" runat="server" CssClass="labels" Text="Datum pož. dodání: " ToolTip="Datum požadovaného dodání:"></asp:Label>
								<br />
								<asp:TextBox ID="tbxDatumDodaniFlt" runat="server" MaxLength="10"></asp:TextBox>
								<cc2:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="tbxDatumDodaniFlt" CssClass="calendars"></cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblUsersModifyFlt" runat="server" CssClass="labels" Text="Zadavatel: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlUsersModifyFlt" runat="server"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 20%;">
								<asp:Label ID="lblObjednavkaIDFlt" runat="server" CssClass="labels" Text="ID objednávky: " ToolTip=""></asp:Label>
                                <br />
								<asp:TextBox ID="tbxObjednavkaIDFlt" runat="server" MaxLength="10" Width="100px"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;
								<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />								
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
						<asp:TemplateField HeaderText="S1" Visible="true">
							<ItemTemplate>
								<asp:ImageButton ID="btnS1new" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="S1New" ImageUrl="~/img/edit.png" ToolTip="ruční S1"
									Visible="true" Enabled="false" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
						<asp:BoundField DataField="MessageTypeID" HeaderText="MessageType" ReadOnly="True" />
                        <asp:BoundField DataField="OrderTypeDescription" HeaderText="Typ obj." ReadOnly="True" />
						<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" />
						<asp:BoundField DataField="RequiredDateOfShipment" HeaderText="Pož. datum dodání" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
							<HeaderStyle Wrap="True" />
							<ItemStyle HorizontalAlign="Right" Wrap="True" />
						</asp:BoundField>
						<asp:BoundField DataField="DescriptionCz" HeaderText="Satus" ReadOnly="True" />
						<asp:BoundField DataField="CustomerName" HeaderText="Firma" ReadOnly="True" />
						<asp:BoundField DataField="CustomerCity" HeaderText="Město" />
						<asp:BoundField DataField="ContactLastName" HeaderText="Příjmení" />
						<asp:BoundField DataField="ContactFirstName" HeaderText="Jméno" Visible="False" />
						<asp:BoundField DataField="ContactPhoneNumber1" HeaderText="Telefon" />
						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce"
									Visible="true" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Excel">
							<HeaderTemplate>
								<asp:ImageButton ID="btnExcelConf" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="btnExcelConfClicked" ImageUrl="~/img/excelSmall.png" ToolTip="Suma zaškrtnutých confirmací"
									Visible="true" Height="20px" Width="20px" />
								<asp:Button ID="btnOznacit" runat="server" Text="*" CommandName="btnOznacit" Height="20px" Width="20px"/>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox ID="chkbExcel" runat="server" />
							</ItemTemplate>
							<HeaderStyle Wrap="False" />
						</asp:TemplateField>
						<asp:BoundField DataField="Reconciliation" HeaderText="Reconciliation" />
						<asp:BoundField DataField="IdWf" HeaderText="Č.výdejky" />
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
                    <table>
                        <tr><td><h3>Detail</h3></td></tr>
                        <tr>
                            <td>
					            <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						            DataKeyNames="ID" Width="10px">
						            <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						            <FooterStyle CssClass="gridfooter" />
						            <RowStyle CssClass="gridrows" />
						            <AlternatingRowStyle CssClass="gridrowalter" />
						            <Columns>							                
							            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								            <ItemStyle Width="10px" />
							            </asp:BoundField>
							            <asp:BoundField DataField="SingleOrMaster" HeaderText="S/M" />
							            <asp:BoundField DataField="ItemVerKit" HeaderText="Item/kit" ItemStyle-Wrap="False">
								            <ItemStyle HorizontalAlign="Right" Wrap="False" />
							            </asp:BoundField>
							            <asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
								            <ItemStyle Wrap="False" />
							            </asp:BoundField>
							            <asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
								            <ItemStyle Wrap="False" />
							            </asp:BoundField>
							            <asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
								            <ItemStyle HorizontalAlign="Right" Wrap="False" />
							            </asp:BoundField>
							            <asp:BoundField DataField="ItemOrKitQuantityRealInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
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
                        <tr><td><br /></td></tr>                            
                        <tr>
                            <td>
                                <asp:Label ID="lblRemark" runat="server" CssClass="labels" Text="Poznámka:"></asp:Label><br />
                                <asp:TextBox ID="tbxRemark" runat="server" MaxLength="4000" Height="60px" TextMode="MultiLine" Width="99%"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
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
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="SingleOrMaster" HeaderText="S/M" />
							<asp:BoundField DataField="ItemVerKit" HeaderText="Item/kit" ItemStyle-Wrap="False">
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
							<asp:TemplateField HeaderText="S1 - dod.množství">
								<ItemTemplate>
									<asp:TextBox ID="tbxQuantity" runat="server" MaxLength="9"></asp:TextBox>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="ItemOrKitQuantityRealInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
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
					<br />
					<asp:Button ID="btnS1Save" runat="server" Text="Vytvoř S1" Width="100px" OnClick="btnS1Save_Click" />&nbsp;&nbsp;
						<asp:Button ID="btnS1Back" runat="server" Text="Zpět" Width="100px" OnClick="btnS1Back_Click" />
				</asp:Panel>

			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<table>
					<tr>
						<td colspan="10" align="center">
							<h1>Nový požadavek na expedici</h1>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStock" runat="server" CssClass="labels" Text="Výdejní místo: "></asp:Label><br />
							<asp:DropDownList ID="ddlStock" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStock_SelectedIndexChanged">
							</asp:DropDownList>
						</td>
						<td colspan="3">
							<asp:Label ID="lblDestinationPlaces" runat="server" CssClass="labels" Text="Cílové místo: "></asp:Label><br />
							<asp:DropDownList ID="ddlDestinationPlaces" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDestinationPlaces_SelectedIndexChanged">
							</asp:DropDownList>
						</td>
						<td colspan="3">
							<asp:Label ID="lblDestinationPlacesContacts" runat="server" CssClass="labels" Text="Kontakt: "></asp:Label><br />
							<asp:DropDownList ID="ddlDestinationPlacesContacts" runat="server">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Label ID="lblIncoterms" runat="server" CssClass="labels" Text="Incoterms: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlIncoterms" runat="server" Width="70px">
							</asp:DropDownList>
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
							<asp:Label ID="lblDateOfExpedition" runat="server" CssClass="labels" Text="Datum dodání: " ToolTip="Požadované datum expediceí"></asp:Label><br />
							<asp:TextBox ID="tbxDateOfExpedition" runat="server" Text="31.8.2014"></asp:TextBox>
							<cc2:CalendarExtender ID="ceDateOfExpedition" runat="server" CssClass="calendars" TargetControlID="tbxDateOfExpedition">
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
										<asp:DropDownList ID="ddlKitQualities" runat="server" Width="60px" AutoPostBack="True" OnSelectedIndexChanged="ddlKitQualities_SelectedIndexChanged">
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
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<asp:Label ID="lblKits" runat="server" CssClass="labels" Text="Kits: "></asp:Label><br />
							<asp:DropDownList ID="ddlKits" runat="server" Width="350px">
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

                <table>
                    <tr>
                        <td>
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
						            <asp:BoundField DataField="cdlStocksName" HeaderText="Sklad - odkud" />
						            <asp:BoundField DataField="DestinationPlacesId" HeaderText="Kam Id" />
						            <asp:BoundField DataField="DestinationPlacesName" HeaderText="Kam název" />
						            <asp:BoundField DataField="DestinationPlacesContactsId" HeaderText="Komu Id" />
						            <asp:BoundField DataField="DestinationPlacesContactsName" HeaderText="Komu jméno" />
						            <asp:BoundField DataField="DateOfExpedition" HeaderText="DateOfExpedition" />
						            <asp:BoundField DataField="IncotermsId" HeaderText="IncotermsId" />
					            </Columns>
					            <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				            </asp:GridView>
                        </td>
                    </tr>
                    <tr><td><br /></td></tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblRemarkNew" runat="server" CssClass="labels" Text="Poznámka:" Visible="false"></asp:Label><br />
                            <asp:TextBox ID="tbxRemarkNew" runat="server" MaxLength="4000" Height="60px" TextMode="MultiLine" Width="100%" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr><td><br /></td></tr>
                    <tr>
                        <td>				            
				            <asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="190px" />&nbsp;
				            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Uložit" Width="190px" />
                        </td>
                    </tr>
                </table>

			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>
