<%@ Page Title="" Language="C#" MasterPageFile="~/Reception.master" AutoEventWireup="true" CodeBehind="VrRepaseRF1.aspx.cs" Inherits="Fenix.VrRepaseRF1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>RF1 - Schvalování RF0</h1>
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
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlDecisionFlt" runat="server">
									<%--<asp:ListItem Value="-1" Text="Vše"></asp:ListItem>
									<asp:ListItem Selected="True" Value="0" Text="Bez vyjádření"></asp:ListItem>
									<asp:ListItem Value="1" Text="Schváleno"></asp:ListItem>
									<asp:ListItem Value="2" Text="Zamítnuto"></asp:ListItem>--%>
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDatumOdeslaniFlt" runat="server" CssClass="labels" Text="Datum odeslání:" ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxDatumOdeslaniFlt" runat="server" MaxLength="12"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
								<cc2:CalendarExtender ID="CalendarExtender1" runat="server" CssClass="calendars" TargetControlID="tbxDatumOdeslaniFlt">
								</cc2:CalendarExtender>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="MessageId: " ToolTip=""></asp:Label><br />
								<asp:TextBox ID="tbxMessageIdFlt" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">                                
                                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Filtr" Width="100px" />
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
							<asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />
							<asp:BoundField DataField="DateOfShipment" HeaderText="Skutečné datum závozu" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="DateOfDelivery" HeaderText="Požadované datum závozu" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">
								<%--<ItemStyle HorizontalAlign="Right" Wrap="True" />--%>
								<HeaderStyle Wrap="True" />
							</asp:BoundField>
							<asp:BoundField DataField="RefurbishedOrderID" HeaderText="ID objednávky" ReadOnly="True" />
							<asp:BoundField DataField="ReconciliationYesNo" HeaderText="Odsouhlasení" />

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

                            <asp:BoundField DataField="Reconciliation" HeaderText="Reconcilace" ReadOnly="True" Visible="false"/>

<%--							<asp:TemplateField>
								<ItemTemplate>
									<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>' CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip="Přehled údajů o objednávce" Visible="true" />
								</ItemTemplate>
								<ItemStyle Wrap="False" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Excel">
								<ItemTemplate>
									<asp:CheckBox ID="chkbExcel" runat="server" />
								</ItemTemplate>
							</asp:TemplateField>--%>

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
											<asp:BoundField DataField="ItID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
												<ItemStyle Width="10px" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemVerKitText" HeaderText="Item/Kit" />
											<asp:BoundField DataField="ItemOrKitID" HeaderText="ID" />
											<asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Schvalované množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="COIItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitQuantityDeliveredInt" HeaderText="Již dodáno" ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="NDReceipt" HeaderText="ND doklad" ItemStyle-Wrap="False">
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
													<%--<asp:ListItem Value="2">Později</asp:ListItem>--%>
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
						</table>
						<hr style="width: auto" />
						<h3>Položky objednávky</h3>
						<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" CssClass="grids" DataKeyNames="ID" GridLines="Vertical" Width="10px">
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
						<br />

					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>

</asp:Content>
