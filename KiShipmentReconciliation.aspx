<%@ Page Title="" Language="C#" MasterPageFile="~/Expedition.master" AutoEventWireup="true" CodeBehind="KiShipmentReconciliation.aspx.cs" Inherits="Fenix.KiShipmentReconciliation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>S1 - Schválení expedice</h1>
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
							<td colspan="2" style="vertical-align: bottom; height: 35%;">
								<asp:Label ID="lblOrderType" runat="server" CssClass="labels" Text="Typ obj.: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlOrderType" runat="server" CssClass="ddlfiltermorethanbig"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlDecisionFlt" runat="server">
									<%--<asp:ListItem Value="-1" Text="Vše"></asp:ListItem>
									<asp:ListItem Selected="True" Value="0" Text="Bez vyjádření"></asp:ListItem>
									<asp:ListItem Value="1" Text="Schváleno"></asp:ListItem>
									<asp:ListItem Value="2" Text="Zamítnuto"></asp:ListItem>--%>
								</asp:DropDownList>
							</td>
							<td>
								<asp:Label ID="lblObjednavkaIDFlt" runat="server" CssClass="labels" Text="ID objednávky: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxObjednavkaIDFlt" runat="server" MaxLength="10"></asp:TextBox>&nbsp;&nbsp;
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumZavozuFlt" runat="server" CssClass="labels" Text="Skutečné dat. závozu od: " ToolTip="Datum expedice od"></asp:Label><br />
								<asp:TextBox ID="tbxDatumZavozuFlt" runat="server" MaxLength="10"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
								<cc2:CalendarExtender ID="CalendarExtender1" runat="server" CssClass="calendars" TargetControlID="tbxDatumZavozuFlt">
								</cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumZavozuFltDo" runat="server" CssClass="labels" Text="Skutečné dat. závozu do: " ToolTip="Datum expedice do"></asp:Label><br />
								<asp:TextBox ID="tbxDatumZavozuFltDo" runat="server" MaxLength="10"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
								<cc2:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="calendars" TargetControlID="tbxDatumZavozuFltDo">
								</cc2:CalendarExtender>
							</td>							
                            <td style="vertical-align: bottom; height: 5%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="MessageId: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxMessageIdFlt" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 5%;">								
                                &nbsp;&nbsp;<asp:LinkButton ID="lnkBtnDoFltByDescr" runat="server" CssClass="linkButton" OnClick="lnkBtnDoFltByDescr_Click">Popis:</asp:LinkButton><br />
								&nbsp;&nbsp;<asp:TextBox ID="tbxDescriptionFlt" runat="server" CssClass="txt" MaxLength="3000"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom">
								<asp:Label ID="lblUsersModifyFlt" runat="server" CssClass="labels" Text="Zadavatel: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlUsersModifyFlt" runat="server"></asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30px;">								
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />							    
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
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />                                                        
                            <asp:BoundField DataField="OrderTypeDescription" HeaderText="Typ obj." ReadOnly="True" />
							<asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />                            
                            <asp:BoundField DataField="City" HeaderText="Město" ReadOnly="True" />                            							
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="RequiredDateOfShipment" HeaderText="Požadované datum závozu" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">								
								<HeaderStyle Wrap="True" />
							</asp:BoundField>
							<asp:BoundField DataField="ReconciliationYesNo" HeaderText="Odsouhlasení" />
							<asp:BoundField DataField="ShipmentOrderID" HeaderText="ID objednávky" />

						    <asp:TemplateField>
							    <ItemTemplate>
								    <asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
									    CommandName="btSingleExcel" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce"
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

                            <asp:BoundField DataField="ModifyUserLastName" HeaderText="Zadavatel" />
                            <asp:BoundField DataField="OrderTypeID" HeaderText="OrderTypeID" Visible="true"/>
                            <asp:BoundField DataField="Reconciliation" HeaderText="Reconciliation" Visible="true"/>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />

					<br />
					<asp:Panel ID="pnlItems" runat="server" Visible="false">
						<h3>Detailní položky schvalované Confirmace</h3>
						<hr style="width: auto; background-color: #0000FF;" />
						<asp:Label ID="lblErrInfo" runat="server" CssClass="errortext" Font-Bold="True"></asp:Label>
						<table>
							<tr>
								<td>
									<table>
										<tr>
											<td valign="bottom">
												<asp:Label ID="Label1" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblCustomerNameValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="Label2" runat="server" CssClass="labels" Text="Město: " ToolTip=""></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblCustomerCityValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
										</tr>
										<tr>
											<td valign="bottom">
												<asp:Label ID="Label3" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblCustomerAddress1Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="Label4" runat="server" CssClass="labels" Text="Č.popisné: " ToolTip=""></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblCustomerAddress2Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
										</tr>
										<tr>
											<td valign="bottom">
												<asp:Label ID="Label5" runat="server" CssClass="labels" Text="PSČ: " ToolTip=""></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblCustomerZipCodeValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="Label6" runat="server" CssClass="labels" Text="Pož.datum dodání: "></asp:Label></td>
											<td valign="bottom">
												<asp:Label ID="lblRequiredDateOfShipmentValue" runat="server" CssClass="plaintext" Text="" ToolTip="" DataFormatString="{0:d}" Font-Bold="True"></asp:Label></td>
										</tr>
									</table>

								</td>
							</tr>
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
											<asp:BoundField DataField="SingleOrMaster" HeaderText="S/M" />
											<asp:BoundField DataField="ItemVerKit" HeaderText="Item/Kit" />
											<asp:BoundField DataField="ItemOrKitID" HeaderText="ID" />
											<asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="RealItemOrKitQuantityInt" HeaderText="Expedované množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="CMSOSIItemOrKitQuantity" HeaderText="Objednané.množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitQuantityRealInt" HeaderText="Již expedováno" ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="Code" HeaderText="Kvalita" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="IncotermDescription" HeaderText="Incoterm" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
										</Columns>
										<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td nowrap="nowrap">
									<table>
										<tr>
											<td valign="bottom">
												<asp:RadioButtonList ID="rdblDecision" runat="server" RepeatDirection="Horizontal">													
													<asp:ListItem Value="1">Schvaluji</asp:ListItem>
													<%--<asp:ListItem Value="2">ZAMÍTÁM !</asp:ListItem>--%>
                                                    <asp:ListItem Value="3">ZAMÍTÁM !</asp:ListItem>
												</asp:RadioButtonList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
											</td>
											<td valign="top">
												<asp:Button ID="btnDecision" runat="server" Text="Rozhodnutí" OnClick="btnDecision_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
											</td>
											<td valign="top">
												<asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="Zpět" Width="100px" />&nbsp;&nbsp;&nbsp;&nbsp;
											</td>
											<td valign="top">
												<asp:Button ID="btnSnExcel" runat="server" OnClick="btnSnExcel_Click" Text="SN excel" Width="100px" />
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td nowrap="nowrap">
									<h3>Položky objednávky</h3>
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
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblRemark" runat="server" CssClass="labels" Text="Poznámka:"></asp:Label><br />
                                    <asp:TextBox ID="tbxRemark" runat="server" MaxLength="4000" Height="60px" TextMode="MultiLine" Width="99%"></asp:TextBox>
                                </td>
                            </tr>
						</table>
						<%--<hr style="width: auto" />--%>
                        
						<%--						<asp:Panel ID="pnlDetails" runat="server" Visible="false">
							<h3>Složení KITu</h3>
							<asp:GridView ID="gvKiItems" runat="server" AutoGenerateColumns="False" CssClass="grids" DataKeyNames="cdlKitsItemsID" GridLines="Vertical">
								<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
								<FooterStyle CssClass="gridfooter" />
								<RowStyle CssClass="gridrows" />
								<AlternatingRowStyle CssClass="gridrowalter" />
								<Columns>
									<asp:BoundField DataField="cdlKitsItemsID" HeaderText="ID" ReadOnly="True" />
									<asp:BoundField DataField="ItemVerKit" HeaderText="Kit/Item" ReadOnly="True" />
									<asp:BoundField DataField="ItemOrKitID" HeaderText="ID Kit/Item" ReadOnly="True" />
									<asp:BoundField DataField="ItemCode" HeaderText="Kód" />
									<asp:BoundField DataField="DescriptionCzItemsOrKit" HeaderText="Popis" ReadOnly="True" />
									<asp:BoundField DataField="ItemOrKitQuantity" HeaderText="Množství" ReadOnly="True" />
								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
							<br />
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
									<asp:BoundField DataField="MeasuresCode" HeaderText="MeJe" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="KitQualitiesCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
							<br />
							<h3>Příslušné skladové karty ITEMů</h3>
							<asp:GridView ID="gvCardStockItems2" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
								DataKeyNames="CardStockItemsID" Width="10px">
								<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
								<FooterStyle CssClass="gridfooter" />
								<RowStyle CssClass="gridrows" />
								<AlternatingRowStyle CssClass="gridrowalter" />
								<Columns>
									<asp:BoundField DataField="CardStockItemsID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
										<ItemStyle Width="10px" />
									</asp:BoundField>
									<asp:BoundField DataField="ItemVerKitDescription" HeaderText="Typ Popis" />
									<asp:BoundField DataField="ItemOrKitId" HeaderText="Item" />
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
									<asp:BoundField DataField="MeasureCode" HeaderText="MeJe" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="Name" HeaderText="Sklad" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>

								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
						</asp:Panel>--%>
					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">

				<table width="80%">
					<tr>
						<td colspan="10" align="center">
							<h1>Filtrace dle popisu</h1>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<asp:Label ID="lblKitsFltByDescr" runat="server" CssClass="labels" Text="Kits: "></asp:Label><br />
							<asp:DropDownList ID="ddlKitsFltByDescr" runat="server" Width="350px">
							</asp:DropDownList>
                            <br />
						</td>
						<td colspan="5">
							<asp:Label ID="lblMaterialFltByDescr" runat="server" CssClass="labels" Text="Materiál: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlMaterialFltByDescr" runat="server" Width="350px">
							</asp:DropDownList>
                            <br />
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<asp:Button ID="btnAddKitIntoFltByDescr" runat="server" Text="Přidat" OnClick="btnAddKitIntoFltByDescr_Click" />
						</td>
						<td colspan="5">
							<asp:Button ID="btnAddMaterialIntoFltByDescr" runat="server" Text="Přidat" OnClick="btnAddMaterialIntoFltByDescr_Click" />
						</td>
					</tr>
				</table>

				<asp:Label ID="lblErrorFltByDescr" runat="server" CssClass="errortext"></asp:Label>
				<br />

                <table width="80%">
                    <tr>
                        <td align="center">
				            <asp:GridView ID="gvKitsOrItemsNew" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids">
					            <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
					            <FooterStyle CssClass="gridfooter" />
					            <RowStyle CssClass="gridrows" />
					            <AlternatingRowStyle CssClass="gridrowalter" />
					            <Columns>
						            <asp:TemplateField HeaderText="Filtr">
							            <ItemTemplate>
								            <asp:CheckBox ID="CheckBoxR" runat="server" Checked='<%# Bind("AnoNe") %>' />
							            </ItemTemplate>
							            <ItemStyle HorizontalAlign="Right" Width="50px" />
						            </asp:TemplateField>						            
						            <asp:BoundField DataField="ItemVerKit" HeaderText="Kit/Item" ReadOnly="True" />
						            <asp:BoundField DataField="ItemOrKitID" HeaderText="ID Kit/Item" ReadOnly="True" />
                                    <asp:BoundField DataField="ItemOrKitCode" HeaderText="Kód" />
						            <asp:BoundField DataField="DescriptionCzItemsOrKit" HeaderText="Popis" ReadOnly="True" />
					            </Columns>
					            <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">				            
				            <asp:Button ID="btnBackFltByDescr" runat="server" Text="Zpět" OnClick="btnBackFltByDescr_Click" Width="190px" />&nbsp;
				            <asp:Button ID="btnSaveFltByDescr" runat="server" Text="Filtrovat" OnClick="btnSaveFltByDescr_Click" Width="190px" />
                        </td>
                    </tr>
                </table>
                
			</asp:View>
		</asp:MultiView>
	</div>
</asp:Content>
