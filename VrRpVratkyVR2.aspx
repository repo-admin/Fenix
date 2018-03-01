<%@ Page Title="" Language="C#" MasterPageFile="~/VratRep.master" AutoEventWireup="true" CodeBehind="VrRpVratkyVR2.aspx.cs" Inherits="Fenix.VrRpVratkyVR2" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>VR2 - Vratky příslušenství</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlDecisionFlt" runat="server">
									<%--<asp:ListItem Selected="True" Value="-1" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Bez vyjádření"></asp:ListItem>
									<asp:ListItem Value="1" Text="Schváleno"></asp:ListItem>
									<asp:ListItem Value="2" Text="Zamítnuto"></asp:ListItem>--%>
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:ImageButton ID="search_button" runat="server" AccessKey="F" AlternateText="Vyhledat" ImageUrl="~/img/search_button.png" OnClick="search_button_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
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
							<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							<asp:BoundField DataField="ModifyDate" HeaderText="Datum zápisu Message" ReadOnly="True" />
							<asp:BoundField DataField="ReconciliationAnoNe" HeaderText="Schválení" ReadOnly="True" />
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
					<cc1:Pager ID="grdPager" runat="server" />
					<br />
					<asp:Panel runat="server" ID="pnlItems" Visible="false">
						<h3>Detailní položky schvalované VR2</h3>
						<hr style="width: auto; background-color: #0000FF;"  />
						<asp:Label ID="lblErrInfo" runat="server" CssClass="errortext" Font-Bold="True"></asp:Label>
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
											<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" />
											<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
												<ItemStyle Width="10px" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemId" HeaderText="ID" />
											<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemQuantityInt" HeaderText="Expedované množ." ItemStyle-Wrap="False">
												<ItemStyle HorizontalAlign="Right" Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ItemOrKitQuality" HeaderText="Kvalita" ItemStyle-Wrap="False">
												<ItemStyle Wrap="False" />
											</asp:BoundField>
											<asp:BoundField DataField="ReturnedFrom" HeaderText="Odkud" ItemStyle-Wrap="False">
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


						</table>


					</asp:Panel>
				</div>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>

</asp:Content>
