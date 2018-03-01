<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaInternalDocuments.aspx.cs" Inherits="Fenix.MaInternalDocuments" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:MultiView ID="mvwMain" runat="server">
            <asp:View ID="vwGrid" runat="server">
                <h1>Interní doklady</h1>

                <div id="divFilter" runat="server" visible="true">
					<table style="border: thin solid #0000FF;">
						<tr>
							<td style="vertical-align: bottom;">
								<asp:Label ID="lblIDFlt" runat="server" CssClass="labels" Text="ID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
							</td>

							<td style="vertical-align: bottom;">
								<asp:Label ID="lblItemOrKitIDFlt" runat="server" CssClass="labels" Text="Item/kit ID: " ToolTip=""></asp:Label>&nbsp;<br />
								<asp:TextBox ID="tbxItemOrKitIDFlt" runat="server" MaxLength="20"></asp:TextBox>&nbsp;&nbsp;
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
					DataKeyNames="ID" >
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>						
                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="ItemVerKitText" HeaderText="Item  kit" ReadOnly="True" />
                        <asp:BoundField DataField="ItemOrKitID" HeaderText="Item/kit ID" ReadOnly="True" HeaderStyle-Wrap="true" />
                        <asp:BoundField DataField="MeasureCode" HeaderText="MeJe" ReadOnly="True" HeaderStyle-Wrap="true" />
                        <asp:BoundField DataField="QualityCode" HeaderText="Kvalita" ReadOnly="True" HeaderStyle-Wrap="true" />
                        <asp:BoundField DataField="ItemOrKitQuantityBefore" HeaderText="Množ. celkem před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitQuantityAfter" HeaderText="Množ. celkem po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitFreeBefore" HeaderText="Množ. volné před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitFreeAfter" HeaderText="Množ. volné po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitUnConsilliationBefore" HeaderText="Množ. ke schválení před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitUnConsilliationAfter" HeaderText="Množ. ke schválení po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitReservedBefore" HeaderText="Množ. rezervované před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitReservedAfter" HeaderText="Množ. rezervované po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitReleasedForExpeditionBefore" HeaderText="Množ. uvolněné před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitReleasedForExpeditionAfter" HeaderText="Množ. uvolněné po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitExpeditedBefore" HeaderText="Množ. exped. před" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="ItemOrKitExpeditedAfter" HeaderText="Množ. exped. po" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0}" />
                        <asp:BoundField DataField="StockName" HeaderText="Sklad" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="ModifyDate" HeaderText="Datum editace" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>

				<cc1:Pager ID="grdPager" runat="server" />                                
                <br />

            </asp:View>
        </asp:MultiView>
    </div>        
</asp:Content>
