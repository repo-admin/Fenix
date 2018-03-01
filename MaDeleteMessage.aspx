<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaDeleteMessage.aspx.cs" Inherits="Fenix.MaDeleteMessage" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:MultiView ID="mvwMain" runat="server">
            <asp:View ID="vwGrid" runat="server">
                <h1>D0 - zrušené objednávky</h1>

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
					<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
					<FooterStyle CssClass="gridfooter" />
					<RowStyle CssClass="gridrows" />
					<AlternatingRowStyle CssClass="gridrowalter" />
					<Columns>
						<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
						<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />
                        <asp:BoundField DataField="SentDate" HeaderText="Datum odeslání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DeleteMessageDate" HeaderText="Datum přijetí/smazání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DeleteId" HeaderText="Zrušená objednávka ID" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="DeleteMessageId" HeaderText="Zrušená objednávka MessageID" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="DeleteMessageTypeDescription" HeaderText="Zrušená objednávka typ" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />                        
                        <asp:BoundField DataField="DeletedByUserLastName" HeaderText="Objednávku zrušil" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
                        <%--skrytý sloupec (pro určení typu D0)--%>
                        <asp:BoundField DataField="DeleteMessageTypeId" HeaderText="Typ zrusene objednavky" ReadOnly="True" Visible="true" />
                        <asp:BoundField DataField="Source" HeaderText="Typ přenosu" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" />
					</Columns>
					<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				</asp:GridView>
				<cc1:Pager ID="grdPager" runat="server" />                                
                <br />

				<asp:Panel ID="pnlReceptionOrder" runat="server" Visible="false">
					<h3>Objednávka</h3>
					<asp:GridView ID="gvReceptionOrderHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />
                            <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ItemStyle-Wrap="false" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <br />
					<h3>Detail objednávky</h3>
					<asp:GridView ID="gvReceptionOrderItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="HeliosOrderRecordId" HeaderText="HeliosRecordId" ReadOnly="True" />
							<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
							<asp:BoundField DataField="ItemCode" HeaderText="Kód" />
							<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemQuantityInt" HeaderText="Obj.množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" />
							<asp:BoundField DataField="SourceCode" HeaderText="Source" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
				</asp:Panel>

                <asp:Panel ID="pnlReceptionConfirmation" runat="server" Visible="false">
                    <h3>Potvrzení</h3>
					<asp:GridView ID="gvReceptionConfirmationHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Skutečné datum uskladnění" ReadOnly="True" DataFormatString="{0:d}" />							
							<asp:BoundField DataField="ItemSupplierDescription" HeaderText="Dodavatel" ReadOnly="True" />                            
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <h3>Detail</h3>
                    <asp:GridView ID="gvReceptionConfirmationItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemID" HeaderText="ItemID" />
							<asp:BoundField DataField="GroupGoods" HeaderText="SkZb" />
							<asp:BoundField DataField="Code" HeaderText="Kód" />
							<asp:BoundField DataField="ItemDescription" HeaderText="Popis" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="DescriptionCz" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemQuantityInt" HeaderText="Naskladněné.množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="CMRSIItemQuantity" HeaderText="Objednané.množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>

							<asp:BoundField DataField="ItemUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlKittingOrder" runat="server" Visible="false">
					<h3>Objednávka</h3>
					<asp:GridView ID="gvKittingOrderHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />
                            <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="CompanyName" HeaderText="Kompletační firma" ItemStyle-Wrap="false" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <br />
                    <h3>Detail objednávky</h3>
				    <asp:GridView ID="gvKittingOrderItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
					    DataKeyNames="ID" Width="10px">
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
						    <asp:BoundField DataField="KitQualityCode" HeaderText="Kvalita" />
						    <asp:BoundField DataField="HeliosOrderId" HeaderText="HeliosOrderId" ReadOnly="True" />
						    <asp:BoundField DataField="KitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False" >
						    <ItemStyle Wrap="False" />
						    </asp:BoundField>
					    </Columns>
					    <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
				    </asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlKittingConfirmation" runat="server" Visible="false">
                    <h3>Potvrzení</h3>
					<asp:GridView ID="gvKittingConfirmationHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" >
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:CommandField ButtonType="Image" SelectImageUrl="~/img/detailSmall.png" SelectText="Položky" ShowSelectButton="true" />
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />							
                            <asp:BoundField DataField="ModelCPE" HeaderText="Model CPE" ReadOnly="True" />							
							<asp:BoundField DataField="HeliosObj" HeaderText="Helios obj." />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <h3>Detail</h3>
					<asp:GridView ID="gvKittingConfirmationItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
                </asp:Panel>
                
                <asp:Panel ID="pnlShipmentOrder" runat="server" Visible="false">
                    <h3>Objednávka</h3>
					<asp:GridView ID="gvShipmentOrderHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />
                            <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="CustomerName" HeaderText="Firma" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="CustomerCity" HeaderText="Město" ItemStyle-Wrap="false" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <br />
                    <h3>Detail objednávky</h3>
			        <asp:GridView ID="gvShipmentOrderItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							                
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="SingleOrMaster" HeaderText="S/M" />
							<asp:BoundField DataField="ItemVerKit" HeaderText="Item/kit" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlShipmentConfirmation" runat="server" Visible="false">
                    <h3>Potvrzení</h3>
                    <asp:GridView ID="gvShipmentConfirmationHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
	                    DataKeyNames="ID" >
	                    <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
	                    <FooterStyle CssClass="gridfooter" />
	                    <RowStyle CssClass="gridrows" />
	                    <AlternatingRowStyle CssClass="gridrowalter" />
	                    <Columns>                            		                    
		                    <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
		                    <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />                                                        
                            <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
		                    <asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />                            
                            <asp:BoundField DataField="City" HeaderText="Město" ReadOnly="True" />
	                    </Columns>
	                    <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
                    </asp:GridView>                    
                    <h3>Detail</h3>
					<asp:GridView ID="gvShipmentConfirmationItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
								<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="SingleOrMaster" HeaderText="S/M" />
							<asp:BoundField DataField="ItemVerKit" HeaderText="Item/kit" ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityRealInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlRefurbishedOrder" runat="server" Visible="false">
                    <h3>Objednávka</h3>
					<asp:GridView ID="gvRefurbishedOrderHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="MessageID" HeaderText="MessageID" ReadOnly="True" />
                            <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání" ReadOnly="True" HeaderStyle-Wrap="true" ItemStyle-Wrap="false" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="CompanyName" HeaderText="Firma" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="CustomerCity" HeaderText="Město" ItemStyle-Wrap="false" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                    <br />
                    <h3>Detail objednávky</h3>
					<asp:GridView ID="gvRefurbishedOrderItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
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
							<asp:BoundField DataField="ItemVerKitText" HeaderText="Item/kit">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitDescription" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
								<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
								<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlRefurbishedConfirmation" runat="server" Visible="false">
                    <h3>Potvrzení</h3>
					<asp:GridView ID="gvRefurbishedConfirmationHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" >
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />							
							<asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>                                        
                    <h3>Detail</h3>
					<asp:GridView ID="gvRefurbishedConfirmationItems" runat="server" AutoGenerateColumns="False" CssClass="grids" DataKeyNames="ID" GridLines="Vertical" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							
							<asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
							<ItemStyle Width="10px" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemVerKitText" HeaderText="Item/kit">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitID" HeaderText="id položky" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitDescription" HeaderText="DescriptionCz" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQuantityDeliveredInt" HeaderText="Dodané množ." ItemStyle-Wrap="False">
							<ItemStyle HorizontalAlign="Right" Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
							</asp:BoundField>
							<asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
							<ItemStyle Wrap="False" />
							</asp:BoundField>
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
                </asp:Panel>

                <asp:Panel ID="pnlReturnedEquipment" runat="server" Visible="false">
                    <h3>Potvrzení</h3>
					        <asp:GridView ID="gvReturnedEquipmentHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						        DataKeyNames="MessageId" >
						        <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						        <FooterStyle CssClass="gridfooter" />
						        <RowStyle CssClass="gridrows" />
						        <AlternatingRowStyle CssClass="gridrowalter" />
						        <Columns>														        
							        <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							        <asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							        <asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Datum příjmu Message" ReadOnly="True" />							
						        </Columns>
						        <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					        </asp:GridView>                                        
                </asp:Panel>

		        <asp:Panel ID="pnlReturnedItem" runat="server" Visible="false">
		            <h3>Potvrzení</h3>                    
					<asp:GridView ID="gvReturnedItemHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" >
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							    
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							<asp:BoundField DataField="ModifyDate" HeaderText="Datum zápisu Message" ReadOnly="True" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>                         
		            <h3>Detail</h3>
					<asp:GridView ID="gvReturnedItemItems" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" Width="10px">
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							
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
		        </asp:Panel>

		        <asp:Panel ID="pnlReturnedShipment" runat="server" Visible="false">
		            <h3>Potvrzení</h3>
					<asp:GridView ID="gvReturnedShipmentHeader" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						DataKeyNames="ID" >
						<HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						<FooterStyle CssClass="gridfooter" />
						<RowStyle CssClass="gridrows" />
						<AlternatingRowStyle CssClass="gridrowalter" />
						<Columns>							
							<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							<asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							<asp:BoundField DataField="MessageDescription" HeaderText="MessageDescription" ReadOnly="True" />
							<asp:BoundField DataField="ModifyDate" HeaderText="Datum příjmu Message" ReadOnly="True" />							
							<asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />
							<asp:BoundField DataField="ContactName" HeaderText="Kontakt" ReadOnly="True" />
						</Columns>
						<SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					</asp:GridView>
		        </asp:Panel>

            </asp:View>
        </asp:MultiView>
    </div>        
</asp:Content>
