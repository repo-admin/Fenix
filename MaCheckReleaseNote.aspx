<%@ Page Title="" Language="C#" MasterPageFile="~/Expedition.master" AutoEventWireup="true" CodeBehind="MaCheckReleaseNote.aspx.cs" Inherits="Fenix.MaCheckReleaseNote" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>Schvalování výdejek</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblIdWfFlt" runat="server" CssClass="labels" Text="Výdejka č.: "></asp:Label>
								<br />
								<asp:TextBox ID="tbxIdWfFlt" runat="server" MaxLength="20"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCompanyNameFlt" runat="server" CssClass="labels" Text="Firma: "></asp:Label>
								<br />
								<asp:TextBox ID="tbxCompanyNameFlt" runat="server" MaxLength="10"></asp:TextBox>
							</td>
							<td colspan="2">
								<asp:Label ID="lblCityFlt" runat="server" CssClass="labels" Text="Město: "></asp:Label>
								<br />
								<asp:TextBox ID="tbxCityFlt" runat="server" MaxLength="20"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="MessageId:"  Visible="False"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlMessageIdFlt" runat="server" Visible="False">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMaterialCodeFlt" runat="server" CssClass="labels" Text="Kod materiálu: " ToolTip=""></asp:Label>
								<br />
								<asp:TextBox ID="tbxMaterialCodeFlt" runat="server" MaxLength="10"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblDoneFlt" runat="server" CssClass="labels" Text="Uděláno: " ToolTip="Datum požadovaného dodání:"></asp:Label>
								<br />
								<asp:DropDownList ID="ddlDoneFlt" runat="server" >
									<asp:ListItem Selected="True" Value="-1">VŠE</asp:ListItem>
									<asp:ListItem Value="A">Ano</asp:ListItem>
									<asp:ListItem Value="N">Ne</asp:ListItem>
								</asp:DropDownList>
							</td>
							<td colspan="2">
								<%--								<asp:Label ID="lblUsersModifyFlt" runat="server" CssClass="labels" Text="Zadavatel: "></asp:Label>
								<br />
								<asp:DropDownList ID="ddlUsersModifyFlt" runat="server"></asp:DropDownList>--%>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;&nbsp;&nbsp;
								<%--								<asp:ImageButton ID="new_button" runat="server" AccessKey="N" AlternateText="Nový záznam" ImageUrl="~/img/new_button.png" OnClick="new_button_Click" />
								&nbsp;--%>

							</td>
						</tr>
					</table>
				</div>
				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label><br />
				<asp:Label ID="lblErrorUpdate" runat="server" CssClass="errortext"></asp:Label>
				<br />
				<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" AutoGenerateEditButton="True"
					OnRowEditing="grdData_RowEditing" 
					OnRowCancelingEdit="grdData_RowCancelingEdit" OnRowDataBound="grdData_RowDataBound"
					OnRowUpdating="grdData_RowUpdating" OnRowCommand="grdData_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />                    
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="IdWf" HeaderText="Č.výdejky" ReadOnly="True" />
						<asp:BoundField DataField="DoneCase" HeaderText="Souhlas(A/N)" />
						<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
						<asp:BoundField DataField="RequiredQuantities" HeaderText="Požadované množství" ReadOnly="True" />
						<asp:BoundField DataField="SuppliedQuantities" HeaderText="Dodané množství" />
						<asp:BoundField DataField="cdlItemsID" HeaderText="Kód" ReadOnly="True" />
						<asp:BoundField DataField="DescriptionCz" HeaderText="Popis" ReadOnly="True" HeaderStyle-Wrap="true">
							<HeaderStyle Wrap="True" />
						</asp:BoundField>
						<asp:BoundField DataField="cdlMeasuresCode" HeaderText="MeJe" ReadOnly="True" />
						<asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />
						<asp:BoundField DataField="ContactName" HeaderText="Kontakt" ReadOnly="True" />
						<asp:BoundField DataField="City" HeaderText="Město" ReadOnly="True" />
						<asp:BoundField DataField="Hit" HeaderText="Přeneseno" ReadOnly="True" />
                        <asp:BoundField DataField="Done" HeaderText="Done" ReadOnly="True" Visible="true" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />
				<br />

			</asp:View>
			<asp:View ID="vwEdit" runat="server">
			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>

