<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaDestPlaces.aspx.cs" Inherits="Fenix.MaDestPlaces" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>Cílové destinace</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblCompanyNameFlt" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label>
								<br />
								<asp:TextBox ID="tbxCompanyNameFlt" runat="server" CssClass="txt" MaxLength="5"></asp:TextBox>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblStreetFlt" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label>
								<br />
								<asp:TextBox ID="tbxStreetFlt" runat="server" CssClass="txt" MaxLength="10"></asp:TextBox>
							</td>
							<td colspan="2">
								<%--								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>--%>
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
					DataKeyNames="ID" OnRowCommand="grdData_RowCommand" OnSelectedIndexChanged="grdData_SelectedIndexChanged">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnUpdate" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="RecordUpdate" ImageUrl="~/img/edit.png" ToolTip="Náhled/Schválení" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="OrganisationNumber" HeaderText="Č.organizace" ReadOnly="True" />
						<asp:BoundField DataField="CompanyName" HeaderText="Název" ReadOnly="True" />
						<asp:BoundField DataField="City" HeaderText="Město" ReadOnly="True" />
						<asp:BoundField DataField="StreetName" HeaderText="Ulice" ReadOnly="True" />
						<asp:BoundField DataField="StreetOrientationNumber" HeaderText="Č. orientační" ReadOnly="True" />
						<asp:BoundField DataField="StreetHouseNumber" HeaderText="Č. popisné" ReadOnly="True" />
						<asp:BoundField DataField="ZipCode" HeaderText="PSČ" ReadOnly="True" />
						<asp:BoundField DataField="ICO" HeaderText="IČ" ReadOnly="True" />
						<asp:BoundField DataField="DIC" HeaderText="DIČ" ReadOnly="True" />
						<asp:BoundField DataField="Type" HeaderText="Type" ReadOnly="True" />
						<asp:BoundField DataField="CountryISO" HeaderText="ISO" ReadOnly="True" />
						<asp:BoundField DataField="IsActive" HeaderText="IsActive" ReadOnly="True" />

					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />
				<br />
				<br />
				<asp:GridView ID="gvContacts" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" OnRowCommand="gvContacts_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:TemplateField>
							<ItemTemplate>
								<asp:ImageButton ID="btnUpdate" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="RecordUpdate" ImageUrl="~/img/edit.png" ToolTip="Náhled/Schválení" />
							</ItemTemplate>
							<ItemStyle Wrap="False" />
						</asp:TemplateField>
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="FirstName" HeaderText="Jméno" ReadOnly="True" />
						<asp:BoundField DataField="LastName" HeaderText="Příjmení" ReadOnly="True" />
						<asp:BoundField DataField="PhoneNumber" HeaderText="Telefon" ReadOnly="True" />
						<asp:BoundField DataField="ContactEmail" HeaderText="Email" ReadOnly="True" />
						<asp:BoundField DataField="Type" HeaderText="Type" ReadOnly="True" />
						<asp:BoundField DataField="IsActive" HeaderText="IsActive" ReadOnly="True" />

					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<br />
				<asp:Label ID="lblErr" runat="server" CssClass="errortext" Text="" ToolTip=""></asp:Label>
				<table>
					<tr>
						<td>
							<asp:Label ID="lblID" runat="server" CssClass="labels" Text="ID: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxID" runat="server" Enabled="False"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblOrganisationNumber" runat="server" CssClass="labels" Text="Č.org.: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxOrganisationNumber" runat="server" MaxLength="15"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblCompanyName" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbxCompanyName" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxCompanyName" runat="server" MaxLength="150" Width="200px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblCity" runat="server" CssClass="labels" Text="Město: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbxCity" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxCity" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStreetName" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbxStreetName" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxStreetName" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblStreetOrientationNumber" runat="server" CssClass="labels" Text="Č.orientační: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxStreetOrientationNumber" runat="server" MaxLength="15"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblStreetHouseNumber" runat="server" CssClass="labels" Text="Č.popisné: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="tbxStreetHouseNumber" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxStreetHouseNumber" runat="server" MaxLength="35"></asp:TextBox>

						</td>
						<td>
							<asp:Label ID="lblZipCode" runat="server" CssClass="labels" Text="PSČ: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="tbxZipCode" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxZipCode" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblIdCountry" runat="server" CssClass="labels" Text="Země: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxIdCountry" runat="server"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblType" runat="server" CssClass="labels" Text="Typ: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxType" runat="server" MaxLength="10"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblICO" runat="server" CssClass="labels" Text="IČ: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxICO" runat="server" MaxLength="15"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblDIC" runat="server" CssClass="labels" Text="DIČ: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxDIC" runat="server" MaxLength="15"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblCountryISO" runat="server" CssClass="labels" Text="ISO: " ToolTip=""></asp:Label>
							<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbxCountryISO" ErrorMessage="!!!" Font-Bold="True" ForeColor="#FF3300">!!!</asp:RequiredFieldValidator>
						</td>
						<td>
							<asp:TextBox ID="tbxCountryISO" runat="server" MaxLength="3" Width="40px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="lblIsActive" runat="server" CssClass="labels" Text="Aktivita: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:CheckBox ID="chkbIsActive" runat="server" />
						</td>
					</tr>
				</table>
				<asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="190px" CausesValidation="False" />&nbsp;
				<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Uložit" Width="190px" />
				<br />
				<asp:Label ID="lblErrorUpdate" runat="server" CssClass="errortext" Text="" ToolTip=""></asp:Label>
				<br />

				<asp:GridView ID="gvContactsUpdate" runat="server" AutoGenerateColumns="False" AutoGenerateEditButton="True" 
					CssClass="grids" DataKeyNames="ID" GridLines="Vertical" OnRowCommand="gvContacts_RowCommand"
					 OnRowEditing="gvContactsUpdate_RowEditing" OnRowCancelingEdit="gvContactsUpdate_RowCancelingEdit" 
					OnRowUpdating="gvContactsUpdate_RowUpdating">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
						<asp:BoundField DataField="FirstName" HeaderText="Jméno" />
						<asp:BoundField DataField="LastName" HeaderText="Příjmení" />
						<asp:BoundField DataField="PhoneNumber" HeaderText="Telefon" />
						<asp:BoundField DataField="ContactEmail" HeaderText="Email" />
						<asp:BoundField DataField="Type" HeaderText="Type" />
						<asp:BoundField DataField="IsActive" HeaderText="IsActive" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<br />
				<table>
					<tr>
						<td colspan="4" align="center">
							<asp:Label ID="Label10" runat="server" CssClass="labels" Text="Nový kontakt" ToolTip="" Font-Size="14px"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="Label1" runat="server" CssClass="labels" Text="Jméno: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxJmenoN" runat="server" MaxLength="35" Width="220px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="Label2" runat="server" CssClass="labels" Text="Příjmení: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxPrijmeniN" runat="server" MaxLength="35" Width="220px"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="Label3" runat="server" CssClass="labels" Text="Město: " ToolTip="" Visible="false"></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxMestoN" runat="server" MaxLength="100" Visible="false" Width="220px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="Label4" runat="server" CssClass="labels" Text="Ulice: " ToolTip="" Visible="false"></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxUliceN" runat="server" MaxLength="100" Visible="false" Width="220px"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="Label5" runat="server" CssClass="labels" Text="PSČ: " ToolTip="" Visible="false"></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxPSCN" runat="server" MaxLength="5" Visible="false"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="Label6" runat="server" CssClass="labels" Text="Č.p./Č.or.: " ToolTip="" Visible="false"></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxCisPopis" runat="server" MaxLength="5" Visible="false"></asp:TextBox><asp:TextBox ID="tbxCisOr" runat="server" MaxLength="5" Visible="false" Width="93px"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="Label7" runat="server" CssClass="labels" Text="Mail: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxMailN" runat="server" MaxLength="150" Width="220px"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="Label8" runat="server" CssClass="labels" Text="Telefon: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxTelefonN" runat="server" MaxLength="13"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="Label9" runat="server" CssClass="labels" Text="Typ: " ToolTip=""></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="tbxTypN" runat="server" MaxLength="10"></asp:TextBox>
						</td>
						<td>
							<asp:Label ID="Label11" runat="server" CssClass="labels" Text="Pohlaví:"></asp:Label>
						</td>
						<td>

							<asp:DropDownList ID="ddlPohlaví" runat="server">
								<asp:ListItem Value="1">Muž</asp:ListItem>
								<asp:ListItem Value="2">Žena</asp:ListItem>
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td colspan="4" align="center">
						<asp:Button ID="btnSaveContact" runat="server" Text=" Ulož kontakt " CausesValidation="false" OnClick="btnSaveContact_Click" />
						</td>
					</tr>

				</table>

			</asp:View>
		</asp:MultiView>

	</div>
</asp:Content>
