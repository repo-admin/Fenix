<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaCardStockItems.aspx.cs" Inherits="Fenix.MaCardStockItems" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<%@ Register Src="CardStockDetail.ascx" TagName="CardStockDetail" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:MultiView ID="mvwMain" runat="server">
			<asp:View ID="vwGrid" runat="server">
				<h1>Stav skladu</h1>
				<div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblMessageIdFlt" runat="server" CssClass="labels" Text="Typ: "></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlItemTypeFlt22" runat="server">
								</asp:DropDownList>
								&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblItemVerKitFlt" runat="server" CssClass="labels" Text="Item/Kit: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlItemVerKitFlt" runat="server">
									<asp:ListItem Value="-1" Selected="True" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Items"></asp:ListItem>
									<asp:ListItem Value="1" Text="Kits"></asp:ListItem>
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom; height: 30%;">

								<asp:Label ID="lblGroupGoodsFlt" runat="server" CssClass="labels" Text="SkZboží: " ToolTip="Skupina zboží"></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlGroupGoodsFlt" runat="server" AutoPostBack="false">
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td colspan="1">
								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td colspan="1">
								<asp:Label ID="lblMaterialCodeFlt" runat="server" CssClass="labels" Text="Kod materiálu: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxMaterialCodeFlt" runat="server" MaxLength="10"></asp:TextBox>&nbsp;&nbsp;
							</td>
						</tr>

						<tr>
							<td colspan="2" style="vertical-align: bottom; height: 30%;">
								<asp:Label ID="lblQuantityFlt" runat="server" CssClass="labels" Text="Množství:" ToolTip=""></asp:Label>
                                <br />
								<asp:DropDownList ID="ddlQuantityTypeFlt" runat="server">
									<asp:ListItem Value="-1" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Množství volné"></asp:ListItem>
									<asp:ListItem Value="1" Text="Množství ke schválení"></asp:ListItem>
									<asp:ListItem Value="2" Text="Množství rezervované"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="Množství uvolněné"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="Množství záporné kdekoliv"></asp:ListItem>
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30%;">
							</td>
							<td>
								<asp:Label ID="lblTrideniFlt" runat="server" CssClass="labels" Text="Třídění: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlTrideniFlt" runat="server" Width="60px">									
                                    <asp:ListItem Value="ItemOrKitID">ID Item/kit</asp:ListItem>
									<asp:ListItem Value="Code">Kód</asp:ListItem>
									<asp:ListItem Value="QualitiesCode,DescriptionCz">Kvalita</asp:ListItem>
									<asp:ListItem Value="DescriptionCz,QualitiesCode">Název</asp:ListItem>
								</asp:DropDownList>
							</td>
							<td style="vertical-align: bottom; height: 30px;">
								<asp:ImageButton ID="search_button" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
								&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:ImageButton ID="btnExcel" runat="server" 
									CommandName="btnExcelClicked" ImageUrl="~/img/excelSmall.png" ToolTip=""
									Visible="true" OnClick="btnExcel_Click" />
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
						<asp:TemplateField><ItemTemplate><asp:ImageButton ID="btnEdit" runat="server" CommandArgument='<%# Eval("ID") %>'
									CommandName="btnEdit" ImageUrl="~/img/edit.png" ToolTip="ruční S1"
									Visible="true" /></ItemTemplate><ItemStyle Wrap="False" /></asp:TemplateField>
						<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True"><ItemStyle Width="10px" /></asp:BoundField>
						<asp:BoundField DataField="ItemVerKitDescription" HeaderText="Typ Popis" />
						<asp:BoundField DataField="ItemOrKitId" HeaderText="ID Items/Kit" />
						<asp:BoundField DataField="GroupGoods" HeaderText="SkZ" />
						<asp:BoundField DataField="Code" HeaderText="Kód" />
						<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitQuantityInteger" HeaderText="Množství" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitFreeInteger" HeaderText="Množství volné" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitUnConsilliationInteger" HeaderText="Množství ke schválení" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitReservedInteger" HeaderText="Množství rezervované" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitReleasedForExpeditionInteger" HeaderText="Množství uvolněné" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="ItemOrKitExpeditedInteger" HeaderText="Množství expedováno" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="MeasuresCode" HeaderText="MeJe" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
						<asp:BoundField DataField="QualitiesCode" HeaderText="Kvalita" ItemStyle-Wrap="False"><ItemStyle HorizontalAlign="Right" Wrap="False" /></asp:BoundField>
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />
				<br />
			</asp:View>
			<asp:View ID="vwEdit" runat="server">
				<br />
				<asp:Label ID="lblErr" runat="server" CssClass="errortext" Text="" ToolTip=""></asp:Label>
				<uc1:CardStockDetail runat="server" ID="CardStockDetail1" />

				<asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="190px" CausesValidation="False" />&nbsp;
				<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Uložit" Width="190px" />
			</asp:View>
		</asp:MultiView>
	</div>
</asp:Content>
