<%@ Page Title="" Language="C#" MasterPageFile="~/Kitting.master" AutoEventWireup="true" CodeBehind="KiApproval.aspx.cs" Inherits="Fenix.KiApproval" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>K2 - Přehled kitů</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<br />
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
									<asp:ListItem Value="0" Text="Schválené"></asp:ListItem>
									<asp:ListItem Value="1" Text="Zamítnuté"></asp:ListItem>
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
						DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
								<asp:TemplateField HeaderText="Uložit">
									<ItemTemplate>
										<asp:CheckBox ID="CheckBoxR" runat="server" Checked='<%# Bind("AnoNe") %>' />
									</ItemTemplate>
									<ItemStyle HorizontalAlign="Right" Width="50px" />
								</asp:TemplateField>
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="MessageId" HeaderText="Message ID" />
							<asp:BoundField DataField="MessageDescription" HeaderText="Message" />
							<asp:BoundField DataField="ModifyDate" HeaderText="Editováno" ItemStyle-Wrap="False" DataFormatString="{0:d}">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="Released" HeaderText="Uvolněno" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
					<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Uložit" Width="190px" />
					<br />
					<asp:Panel ID="pnlEdit" runat="server" Visible="false">
						<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids">
							<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							<FooterStyle CssClass="gridfooter" />
							<RowStyle CssClass="gridrows" />
							<AlternatingRowStyle CssClass="gridrowalter" />
							<Columns>
								<asp:BoundField DataField="KitID" HeaderText="Kit" ReadOnly="True">
								</asp:BoundField>
								<asp:BoundField DataField="KitDescription" HeaderText="Kit popis">
								</asp:BoundField>
								<asp:BoundField DataField="KitQuantity" HeaderText="Množství" ItemStyle-Wrap="False">
									<ItemStyle Wrap="False" />
								</asp:BoundField>
								<asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								</asp:BoundField>
								<asp:BoundField DataField="KitQuality" HeaderText="Kvalita" ItemStyle-Wrap="False">
								</asp:BoundField>
							</Columns>
							<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						</asp:GridView>
						

					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>
