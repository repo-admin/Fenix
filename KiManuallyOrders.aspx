<%@ Page Title="" Language="C#" MasterPageFile="~/Kitting.master" AutoEventWireup="true" CodeBehind="KiManuallyOrders.aspx.cs" Inherits="Fenix.KiManuallyOrders" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>K0 - Objednávka kittingu</h1>
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
							    <asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
							    <asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="80px">
							    </asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp;
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

						<asp:TemplateField HeaderText="K1" Visible="false">
							<ItemTemplate>
								<asp:ImageButton ID="btnK1new" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="K1New" ImageUrl="~/img/edit.png" ToolTip="ruční K1"
									Visible="true" Enabled="false" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />                            
						</asp:TemplateField>

						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />						
                        <asp:BoundField DataField="KitID" HeaderText="Kit Id" ReadOnly="True" />
						<asp:BoundField DataField="ModelCPE" HeaderText="Model CPE" ReadOnly="True" />
                        <asp:BoundField DataField="RealCompletationDate" HeaderText="Skut. datum kompletace" ReadOnly="True" DataFormatString="{0:d}"/>
						<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" ItemStyle-Wrap="False" DataFormatString="{0:d}"/>
						<asp:BoundField DataField="KitDateOfDelivery" HeaderText="Pož. datum kompletace" ReadOnly="True" DataFormatString="{0:d}">
							<HeaderStyle Wrap="True" />
							<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="DescriptionCz" HeaderText="Satus" ReadOnly="True" />
						<asp:BoundField DataField="CompanyName" HeaderText="Komplet. firma" ReadOnly="True" />
                        <asp:BoundField DataField="KitQuantity" HeaderText="Obj. množ." ReadOnly="True" DataFormatString="{0:#}" >
                            <ItemStyle HorizontalAlign="Right" Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="KitQuantityDelivered" HeaderText="Dod. množ." ReadOnly="True" DataFormatString="{0:#}" >
                            <ItemStyle HorizontalAlign="Right" Wrap="False" />
                        </asp:BoundField>                        
                        <asp:BoundField DataField="HeliosOrderID" HeaderText="Helios obj." ReadOnly="True" />

						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnSerNumView" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="OrderView" ImageUrl="~/img/excelSmall.png" ToolTip=""
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
				<asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" Width="10px" OnRowCommand="gvItems_RowCommand">
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
						<asp:BoundField DataField="KitId" HeaderText="KitId" />
						<asp:BoundField DataField="KitDescription" HeaderText="Popis kitu" ItemStyle-Wrap="False" />
						<asp:BoundField DataField="KitQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
						<ItemStyle HorizontalAlign="Right" Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="KitQuantityDeliveredInt" HeaderText="Dod.množ." ItemStyle-Wrap="False">
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="KitQualityCode" HeaderText="Kvalita" />
						<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" ReadOnly="True" />
						<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>

					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
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
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<table width="auto">
					<tr>
						<td colspan="10" align="center">
							<h1>Ručně vkládaná objednávka</h1>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStock" runat="server" CssClass="labels" Text="Kompletační místo: "></asp:Label><br />&nbsp;
							<asp:DropDownList ID="ddlStock" runat="server">
							</asp:DropDownList>
						</td>
						<td colspan="7">
							<asp:Label ID="lblItems" runat="server" CssClass="labels" Text="Kits: " ToolTip="Zboží, zařízení"></asp:Label>
							<asp:CheckBox ID="chkbItems" runat="server" AutoPostBack="True" OnCheckedChanged="chkbItems_CheckedChanged" />&nbsp;
							<br />
							<asp:DropDownList ID="ddlItems" runat="server" OnTextChanged="ddlItems_TextChanged">
							</asp:DropDownList>
						</td>
						<td>&nbsp;<asp:Label ID="lblQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip="Objednávané množství"></asp:Label>&nbsp;<br />
							&nbsp;<asp:TextBox ID="tbxQuantity" runat="server"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblDateOfDelivery" runat="server" CssClass="labels" Text="Datum dodání: " ToolTip="Požadované datum zkompletování"></asp:Label>&nbsp;<br />
							<asp:TextBox ID="tbxDateOfDelivery" runat="server" Text="31.8.2014"></asp:TextBox>
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
						<td></td>
						<td>&nbsp;<asp:Label ID="lblHeliosOrderID" runat="server" CssClass="labels" Text="Helios PO kompletace" ToolTip="ID objednávky v Heliosu"></asp:Label><br />
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
				<asp:Label ID="lblErrInfo" runat="server" CssClass="errortext"></asp:Label>
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
						<asp:BoundField DataField="StockId" HeaderText="" ReadOnly="True">
						<ControlStyle Width="1px" />
						<FooterStyle Width="1px" />
						<HeaderStyle Width="1px" />
						<ItemStyle Width="1px" />
						</asp:BoundField>
						<asp:BoundField DataField="StockName" HeaderText="Název kompl. místa" />
						<asp:BoundField DataField="KitId" HeaderText="Kit Id" >
						<ControlStyle Width="1px" />
						</asp:BoundField>
						<asp:BoundField DataField="KitDescription" HeaderText="Kit popis" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="KitQuantity" HeaderText="Množství" ItemStyle-Wrap="False" ItemStyle-HorizontalAlign="Right" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="MeasuresId" HeaderText="MeasuresId" ItemStyle-Wrap="False" >
						<ControlStyle Width="1px" />
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeasuresCode" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="DateOfDelivery" HeaderText="DateOfDelivery" ItemStyle-Wrap="False" >
						<ItemStyle Wrap="False" />
						</asp:BoundField>
						<asp:BoundField DataField="KitQualitiesId" HeaderText="KitQualitiesId" >
						<ControlStyle Width="1px" />
						</asp:BoundField>
						<asp:BoundField DataField="KitQualitiesCode" HeaderText="KitQualitiesCode" />
						<asp:BoundField DataField="HeliosOrderID" HeaderText="HeliosOrderID" />

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
