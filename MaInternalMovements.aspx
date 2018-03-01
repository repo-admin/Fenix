<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaInternalMovements.aspx.cs" Inherits="Fenix.MaInternalMovements" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:MultiView ID="mvwMain" runat="server">
            <asp:View ID="vwGrid" runat="server">
                <h1>Interní pohyby</h1>

                <div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblItemOrKitIDFlt" runat="server" CssClass="labels" Text="ID Item/Kit: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxItemOrKitIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblKitQualitiesFlt" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlKitQualitiesFlt" runat="server" Width="60px">
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblItemVerKitFlt" runat="server" CssClass="labels" Text="Item/Kit: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:DropDownList ID="ddlItemVerKitFlt" runat="server">
									<asp:ListItem Value="-1" Selected="True" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Items"></asp:ListItem>
									<asp:ListItem Value="1" Text="Kits"></asp:ListItem>
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								<asp:DropDownList ID="ddlDecisionFlt" runat="server">
									<asp:ListItem Selected="True" Value="-1" Text="Vše"></asp:ListItem>
									<asp:ListItem Value="0" Text="Bez rozhodnutí"></asp:ListItem>
									<asp:ListItem Value="1" Text="Schváleno"></asp:ListItem>
									<asp:ListItem Value="2" Text="Zamítnuto"></asp:ListItem>
								</asp:DropDownList>&nbsp;&nbsp;
							</td>
							<td style="vertical-align: bottom;">
                                <asp:ImageButton ID="search_button" runat="server" AlternateText="Filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
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
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Datum vytvoření" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="ItemVerKitDescription" HeaderText="Item/Kit" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="ItemOrKitID" HeaderText="ID Item/Kit" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Description" HeaderText="Popis" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="IternalMovementQuantityInteger" HeaderText="Množství" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="MeasureCode" HeaderText="MeJe" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="QualityCode" HeaderText="Kvalita" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="MovementTypeDescription" HeaderText="Typ" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="MovementsDecisionDescription" HeaderText="Schváleno" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="CreatedUserLastName" HeaderText="Vytvořil" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Remark" HeaderText="Poznámka" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="MovementsAddSubBaseAbbrev" HeaderText="+/-" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" /> 
                        <%--skryté sloupce (pro filtraci atd...)--%>
                        <asp:BoundField DataField="ItemVerKit" HeaderText="ItemVerKit" Visible="true" />
                        <asp:BoundField DataField="QualityID" HeaderText="QualityID" Visible="true"/>
                        <asp:BoundField DataField="MovementsDecisionID" HeaderText="MovementsDecisionID" Visible="true"/>
                        <asp:BoundField DataField="MovementsTypeID" HeaderText="MovementsTypeID" Visible="true"/>
                        <asp:BoundField DataField="MovementsAddSubBaseID" HeaderText="MovementsAddSubBaseID" Visible="true"/>
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />                                
                <br />
                <asp:Panel ID="pnlDecision" runat="server" Visible="false">					
					<table>
						<tr>
							<td valign="bottom">
								<asp:RadioButtonList ID="rdblDecision" runat="server" RepeatDirection="Horizontal">													
									<asp:ListItem Value="1">Schvaluji</asp:ListItem>
									<asp:ListItem Value="2">ZAMÍTÁM !</asp:ListItem>
								</asp:RadioButtonList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
							<td valign="top">
								<asp:Button ID="btnDecision" runat="server" Text="Rozhodnutí" OnClick="btnDecision_Click" />
							</td>
						</tr>
					</table>
                </asp:Panel>
            </asp:View>

            <asp:View ID="vwEdit" runat="server">

                <h1>Nový požadavek na interní pohyb</h1>
                <table width="110%">
					<tr>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblItemVerKitNew" runat="server" CssClass="labels" Text="ID Item: " ToolTip=""></asp:Label>&nbsp;<br />
                            <asp:TextBox ID="txtItemVerKitID" runat="server" Text="1"></asp:TextBox><br />
						</td>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblKits" runat="server" CssClass="labels" Text="Kits: "></asp:Label><br />
							<asp:DropDownList ID="ddlKits" runat="server" Width="350px">
							</asp:DropDownList><br />
						</td>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblNW" runat="server" CssClass="labels" Text="Materiál: " ToolTip=""></asp:Label><br />
							<asp:DropDownList ID="ddlNW" runat="server" Width="350px">
							</asp:DropDownList><br />
						</td>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblKitQualitiesNewIM" runat="server" CssClass="labels" Text="Kvalita: " ToolTip=""></asp:Label>&nbsp;<br />
							<asp:DropDownList ID="ddlKitQualitiesNewIM" runat="server" Width="60px">
							</asp:DropDownList><br />
						</td>

						<td style="vertical-align: bottom;">
							<asp:Label ID="lblAddSubBase" runat="server" CssClass="labels" Text="+/-" ToolTip=""></asp:Label>&nbsp;<br />
							<asp:DropDownList ID="ddlAddSubBase" runat="server" Width="120px">
							</asp:DropDownList><br />
						</td>						
                        						
					</tr>					
					<tr>					
						<td style="vertical-align: bottom;">
                            <asp:Label ID="lblItemVerKitNewMn" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="txtItemVerKitQuantity" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
						</td>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblKitsQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="txtKitsQuantity" runat="server"></asp:TextBox>
						</td>
						<td style="vertical-align: bottom;">
							<asp:Label ID="lblNwQuantity" runat="server" CssClass="labels" Text="Množství: " ToolTip=""></asp:Label><br />
							<asp:TextBox ID="txtNwQuantity" runat="server" Text="1"></asp:TextBox>
						</td>
						<td style="vertical-align: bottom;">
							<br />
						</td>

						<td style="vertical-align: bottom;">
							<br />
						</td>					

					</tr>
                </table>
                
                <asp:Label ID="lblErrInfo" runat="server" CssClass="errortext"></asp:Label>
                <br />
                
                <table width="900px;">
                    <tr>
                        <td style="text-align:center;">
							<asp:Label ID="lblRemark" runat="server" CssClass="labels" Text="Poznámka: " ToolTip=""></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
							<asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Width="390px"></asp:TextBox>
                        </td>
                    </tr>
                </table>

                <br />
                <table width="900px;">
                    <tr>
                        <td style="text-align:center;">
                            <asp:Button ID="btnDeficiency" runat="server" Text="Manko" OnClick="btnDeficiency_Click" Width="190px" ForeColor="Red"/>&nbsp;                        
                            <asp:Button ID="btnSurplus" runat="server" Text="Přebytek" OnClick="btnSurplus_Click" Width="190px" ForeColor="Blue" />&nbsp;&nbsp;&nbsp;&nbsp;                        
                            <asp:Button ID="btnBack" runat="server" Text="Zpět" OnClick="btnBack_Click" Width="85px" />                        
                        </td>
                    </tr>
                </table>
            </asp:View>

        </asp:MultiView>
    </div>

    <asp:HiddenField ID="MessageText" Value="" runat="server" ClientIDMode="Static"/>
    
</asp:Content>
