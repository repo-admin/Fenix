<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaDeleteMessageReconciliation.aspx.cs" Inherits="Fenix.MaDeleteMessageReconciliation" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:MultiView ID="mvwMain" runat="server">
            <asp:View ID="vwGrid" runat="server">
                <h1>D1 - zrušené objednávky</h1>

                <div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
<%--							<td style="vertical-align: bottom;">
								<asp:Label ID="lblIDFlt" runat="server" CssClass="labels" Text="ID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>--%>

							<td style="vertical-align: bottom;">
								<asp:Label ID="lblMessageIDFlt" runat="server" CssClass="labels" Text="MessageID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxMessageIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>

							<td style="vertical-align: bottom;">
								<asp:Label ID="lblDeletedOrderIDFlt" runat="server" CssClass="labels" Text="Zruš. obj. ID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxDeletedOrderIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>

							<td style="vertical-align: bottom;">
								<asp:Label ID="lblDeletedOrderMessageIDFlt" runat="server" CssClass="labels" Text="Zruš. obj. MessageID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxDeletedOrderMessageIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>

							<td style="vertical-align: bottom;">
                                <asp:ImageButton ID="search_button" runat="server" AlternateText="Filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
							</td>                            
						</tr>
					</table>
				</div>

				<asp:Label ID="lblInfoRecordersCount" runat="server" CssClass="labels" Text=""></asp:Label>
				<br />

				<asp:GridView ID="grdData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					DataKeyNames="ID" OnSelectedIndexChanged="grdData_SelectedIndexChanged" OnRowCommand="grdData_RowCommand">
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>						
                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />                        
                        <asp:BoundField DataField="DeleteMessageDate" HeaderText="Datum přijetí/smazání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DeleteId" HeaderText="Zrušená objednávka ID" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="DeleteMessageId" HeaderText="Zrušená objednávka MessageID" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="DeleteMessageTypeDescription" HeaderText="Zrušená objednávka typ" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Source" HeaderText="Typ přenosu" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>

				<cc1:Pager ID="grdPager" runat="server" />                                
                <br />

            </asp:View>
        </asp:MultiView>
    </div>        
</asp:Content>
