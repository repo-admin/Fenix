<%@ Page Title="" Language="C#" MasterPageFile="~/Kitting.master" AutoEventWireup="true" CodeBehind="KiReconciliation.aspx.cs" Inherits="Fenix.KiReconciliation" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>K1 - Schválení kittingu</h1>
				<div id="divFilter" runat="server" visible="true">

					<table style="border: thin solid #0000FF;">
						<tr>
						    <td style="vertical-align: bottom; height: 30%;">
							    <asp:Label ID="lblModelCPE" runat="server" CssClass="labels" Text="Model CPE: " ToolTip=""></asp:Label><br />
							    <asp:DropDownList ID="ddlModelCPE" runat="server" Width="80px">
							    </asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp;
						    </td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlDecisionFlt" runat="server">
									<%--<asp:ListItem Text="Vše" Value="-1"></asp:ListItem>
									<asp:ListItem Selected="True" Text="Bez vyjádření" Value="0"></asp:ListItem>
									<asp:ListItem Text="Schváleno" Value="1"></asp:ListItem>
									<asp:ListItem Text="Zamítnuto" Value="2"></asp:ListItem>--%>
								</asp:DropDownList>
							</td>
						</tr>

						<tr>
							<td style="vertical-align: bottom; height: 30px;">
								&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;
								&nbsp;&nbsp;&nbsp;&nbsp;
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
                            <asp:BoundField DataField="ModelCPE" HeaderText="Model CPE" ReadOnly="True" />
							<asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Skutečné datum kompletace" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							<asp:BoundField DataField="KitDateOfDelivery" HeaderText="Požadované datum kompletace" ReadOnly="True" DataFormatString="{0:d}">								
								<HeaderStyle Wrap="True" />
							</asp:BoundField>
                            <asp:BoundField DataField="OrderedKitQuantity" HeaderText="Obj. množ." ReadOnly="True" DataFormatString="{0:#}" >
                                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DeliveredKitQuantity" HeaderText="Dod. množ." ReadOnly="True" DataFormatString="{0:#}" >
                                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                            </asp:BoundField>
							<asp:BoundField DataField="HeliosObj" HeaderText="Helios obj." />
                            <asp:BoundField DataField="ReconciliationText" HeaderText="Rozhodnutí" ReadOnly="True" />
						    <asp:TemplateField>
							    <ItemTemplate>
								    <asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
									    CommandName="btSingleExcel" ImageUrl="~/img/excelSmall.png" ToolTip=""
									    Visible="true" />
							    </ItemTemplate>
							    <ItemStyle Wrap="False" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="Excel">
							    <HeaderTemplate>
								    <asp:ImageButton ID="btnExcelConf" runat="server" CommandArgument='<%# Eval("ID") %>'
									    CommandName="btnExcelConfClicked" ImageUrl="~/img/excelSmall.png" ToolTip=""
									    Visible="true" Height="20px" Width="20px" />
								    <asp:Button ID="btnOznacit" runat="server" Text="*" CommandName="btnOznacit" Height="20px" Width="20px"/>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:CheckBox ID="chkbExcel" runat="server" />
							    </ItemTemplate>
							    <HeaderStyle Wrap="False" />
						    </asp:TemplateField>
                            <asp:BoundField DataField="Reconciliation" HeaderText="Reconciliation" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
					<asp:Panel ID="pnlItems" runat="server" Visible="false">
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
											<asp:BoundField DataField="KitID" HeaderText="KitID" />
											<asp:BoundField DataField="KitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="CMRSIItemQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="KitQuantityInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="Code" HeaderText="Kvalita" ItemStyle-Wrap="False">
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
						<asp:Panel ID="pnlDetails" runat="server" Visible="false">
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
									<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Množství" ReadOnly="True" />
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
									<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Množství" ItemStyle-Wrap="False">
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
									<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Množství" ItemStyle-Wrap="False">
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
									<asp:BoundField DataField="MeasureCode" HeaderText="MeJe" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>
									<asp:BoundField DataField="Name" HeaderText="Sklad" ItemStyle-Wrap="False">
										<ItemStyle HorizontalAlign="Right" Wrap="False" />
									</asp:BoundField>

								</Columns>
								<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
							</asp:GridView>
						</asp:Panel>
					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>

</asp:Content>
