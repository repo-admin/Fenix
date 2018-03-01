<%@ Page Title="" Language="C#" MasterPageFile="~/Management.master" AutoEventWireup="true" CodeBehind="MaHistoryMovesSN.aspx.cs" Inherits="Fenix.MaHistoryMovesSN" %>
<%@ Register Assembly="UpcWebControls" Namespace="UPC.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">    
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:MultiView ID="mvwMain" runat="server">
                    <asp:View ID="vwGrid" runat="server">                
                    <h1>Historie pohybů SN</h1>
                    <div id="divFilter" runat="server" visible="true">
                        <table>
                            <tr>
                                <td>
                                    <table style="border: thin solid #0000FF;">
						                <tr>
							                <td style="vertical-align: bottom; height: 30%;">
								                <asp:Label ID="lblSN" runat="server" CssClass="labels" Text="SN:" ToolTip=""></asp:Label>
                                                <br />
								                <asp:TextBox ID="txtSN" runat="server" CssClass="txt" MaxLength="50"></asp:TextBox>
							                </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                            </td>
							                <td style="vertical-align: bottom; height: 30%;">
								                <asp:Label ID="lblDecisionFlt" runat="server" CssClass="labels" Text="Rozhodnutí: " ToolTip=""></asp:Label><br />
								                <asp:DropDownList ID="ddlDecisionFlt" runat="server">
									                <%--<asp:ListItem Selected="True" Value="-1" Text="Vše"></asp:ListItem>
									                <asp:ListItem Value="0" Text="Bez vyjádření"></asp:ListItem>
									                <asp:ListItem Value="1" Text="Schváleno"></asp:ListItem>
									                <asp:ListItem Value="2" Text="Zamítnuto"></asp:ListItem>--%>
								                </asp:DropDownList>
							                </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                            </td>
							                <td style="vertical-align: bottom; height: 30px;">								
								                <asp:ImageButton ID="btnSearch" runat="server" AlternateText="filtrace záznamů" ImageUrl="~/img/search_button.png" OnClick="btnSearch_Click" />
							                </td>
						                </tr>
                                    </table>
                                </td>                            
                                <td>
                                    <asp:UpdateProgress runat="server" id="updateProgress1">
                                        <ProgressTemplate>
                                            <table>
                                                <tr>
                                                    <td valign="middle">
                                                        <asp:ImageButton ID="btnSearch22" runat="server" AlternateText=" " ImageUrl="~/img/ajax-loader.gif" />  
                                                    </td>
                                                    <td valign="middle">
                                                        &nbsp;<b>Načítání dat ...</b>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>            
                                </td>                                
                            </tr>
                        </table>
                    </div>
                    <asp:Label ID="lblErr" runat="server" CssClass="errortext"></asp:Label>
                    <asp:Label ID="lblInfo" runat="server" CssClass="labels" Text=""></asp:Label>
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
							    <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							    <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
                                <asp:BoundField DataField="ItemId" HeaderText="ItemId" ReadOnly="True" Visible="false" />
                                <asp:BoundField DataField="TypeShortcut" HeaderText="Typ" ReadOnly="True" />
                                <asp:BoundField DataField="TypeDescription" HeaderText="Popis" ReadOnly="True" />
                                <asp:BoundField DataField="ReceiptDate" HeaderText="Datum příjmu message" ReadOnly="True" DataFormatString="{0:dd/MM/yyyy hh:mm:ss}" />
							    <asp:BoundField DataField="DecisionText" HeaderText="Odsouhlasení" />					
						    </Columns>
						    <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					    </asp:GridView>
                        <cc1:Pager ID="grdPager" runat="server" />
                        <br />

                        <asp:Panel ID="pnlKittingConfirmation" runat="server" Visible="false">                        
                            <h3>K1</h3>
                            <asp:GridView ID="grdKittingConfirmationData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						        DataKeyNames="ID" style="width: 760px;" >
						        <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						        <FooterStyle CssClass="gridfooter" />
						        <RowStyle CssClass="gridrows" />
						        <AlternatingRowStyle CssClass="gridrowalter" />
						        <Columns>							    
							        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							        <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />							
                                    <asp:BoundField DataField="ModelCPE" HeaderText="Model CPE" ReadOnly="True" />
							        <asp:BoundField DataField="MessageDateOfReceipt" HeaderText="Skutečné datum kompletace" ReadOnly="True" DataFormatString="{0:d}" />
							        <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							        <asp:BoundField DataField="KitDateOfDelivery" HeaderText="Požadované datum kompletace" ReadOnly="True" DataFormatString="{0:d}">								
								        <HeaderStyle Wrap="True" />
							        </asp:BoundField>
                                    <asp:BoundField DataField="OrderedKitQuantity" HeaderText="Obj. množ." ReadOnly="True" DataFormatString="{0:#}" >
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DeliveredKitQuantity" HeaderText="Dod. množ." ReadOnly="True" DataFormatString="{0:#}" >
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                                    </asp:BoundField>
							        <asp:BoundField DataField="HeliosObj" HeaderText="Helios obj." />                                
						        </Columns>
						        <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					        </asp:GridView>

                            <h3>K1 Detail</h3>

                            <asp:GridView ID="grdKittingConfirmationDetail" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							    DataKeyNames="ID" Width="146px">
							    <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							    <FooterStyle CssClass="gridfooter" />
							    <RowStyle CssClass="gridrows" />
							    <AlternatingRowStyle CssClass="gridrowalter" />
							    <Columns>								
								    <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									    <ItemStyle Width="10px" />
								    </asp:BoundField>
								    <asp:BoundField DataField="KitID" HeaderText="KitID" />
								    <asp:BoundField DataField="KitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
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

                        <asp:Panel ID="pnlShipmentConfirmation" runat="server" Visible="false">
                            <h3>S1</h3>
                            <asp:GridView ID="grdShipmentConfirmationData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids" DataKeyNames="ID">
						        <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						        <FooterStyle CssClass="gridfooter" />
						        <RowStyle CssClass="gridrows" />
						        <AlternatingRowStyle CssClass="gridrowalter" />
						        <Columns>                                							    
							        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							        <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />                                                        
                                    <asp:BoundField DataField="OrderTypeDescription" HeaderText="Typ obj." ReadOnly="True" />
							        <asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />                            
                                    <asp:BoundField DataField="City" HeaderText="Město" ReadOnly="True" />                            							
							        <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							        <asp:BoundField DataField="RequiredDateOfShipment" HeaderText="Požadované datum závozu" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">								
								        <HeaderStyle Wrap="True" />
							        </asp:BoundField>
							        <asp:BoundField DataField="ReconciliationYesNo" HeaderText="Odsouhlasení" />
							        <asp:BoundField DataField="ShipmentOrderID" HeaderText="ID objednávky" />
                                    <asp:BoundField DataField="ModifyUserLastName" HeaderText="Zadavatel" />
						        </Columns>
						        <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					        </asp:GridView>              
                                  
                            <h3>S1 Detail</h3>
						
                            <table>
							    <tr>
								    <td valign="bottom">
									    <asp:Label ID="lblScName" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCustomerNameValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCity" runat="server" CssClass="labels" Text="Město: " ToolTip=""></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCustomerCityValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
							    </tr>
							    <tr>
								    <td valign="bottom">
									    <asp:Label ID="lblScStreet" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCustomerAddress1Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScStrretNumber" runat="server" CssClass="labels" Text="Č.popisné: " ToolTip=""></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCustomerAddress2Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
							    </tr>
							    <tr>
								    <td valign="bottom">
									    <asp:Label ID="lblScZipCode" runat="server" CssClass="labels" Text="PSČ: " ToolTip=""></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScCustomerZipCodeValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScRequiredDateOfShipment" runat="server" CssClass="labels" Text="Pož.datum dodání: "></asp:Label></td>
								    <td valign="bottom">
									    <asp:Label ID="lblScRequiredDateOfShipmentValue" runat="server" CssClass="plaintext" Text="" ToolTip="" DataFormatString="{0:d}" Font-Bold="True"></asp:Label></td>
							    </tr>
						    </table>
								
						    <asp:GridView ID="grdShipmentConfirmationDetail" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
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
								    <asp:BoundField DataField="ItemVerKit" HeaderText="Item/Kit" />
								    <asp:BoundField DataField="ItemOrKitID" HeaderText="ID" />
								    <asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="RealItemOrKitQuantityInt" HeaderText="Expedované množ." ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="CMSOSIItemOrKitQuantity" HeaderText="Objednané.množ." ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitQuantityRealInt" HeaderText="Již expedováno" ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="Code" HeaderText="Kvalita" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="IncotermDescription" HeaderText="Incoterm" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
							    </Columns>
							    <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
						    </asp:GridView>                        
                        </asp:Panel>

                        <asp:Panel ID="pnlRefurbishedConfirmation" runat="server" Visible="false">
                            <h3>RF1</h3>

                            <asp:GridView ID="grdRefurbishedConfirmationData" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
						        DataKeyNames="ID">
						        <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" />
						        <FooterStyle CssClass="gridfooter" />
						        <RowStyle CssClass="gridrows" />
						        <AlternatingRowStyle CssClass="gridrowalter" />
						        <Columns>							    
							        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
							        <asp:BoundField DataField="MessageId" HeaderText="MessageId" ReadOnly="True" />
							        <asp:BoundField DataField="CompanyName" HeaderText="Firma" ReadOnly="True" />
							        <asp:BoundField DataField="DateOfShipment" HeaderText="Skutečné datum závozu" ReadOnly="True" DataFormatString="{0:d}" />
							        <asp:BoundField DataField="MessageDateOfShipment" HeaderText="Datum odeslání message" ReadOnly="True" DataFormatString="{0:d}" />
							        <asp:BoundField DataField="DateOfDelivery" HeaderText="Požadované datum závozu" ReadOnly="True" DataFormatString="{0:d}" HeaderStyle-Wrap="true">								
								        <HeaderStyle Wrap="True" />
							        </asp:BoundField>
							        <asp:BoundField DataField="RefurbishedOrderID" HeaderText="ID objednávky" ReadOnly="True" />
							        <asp:BoundField DataField="ReconciliationYesNo" HeaderText="Odsouhlasení" />
						        </Columns>
						        <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					        </asp:GridView>

                            <h3>RF1 Detail</h3>

                            <table>
	                            <tr>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfName" runat="server" CssClass="labels" Text="Název: " ToolTip=""></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCustomerNameValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCity" runat="server" CssClass="labels" Text="Město: " ToolTip=""></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCustomerCityValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
	                            </tr>
	                            <tr>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfStreet" runat="server" CssClass="labels" Text="Ulice: " ToolTip=""></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCustomerAddress1Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfStrretNumber" runat="server" CssClass="labels" Text="Č.popisné: " ToolTip=""></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCustomerAddress2Value" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
	                            </tr>
	                            <tr>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfZipCode" runat="server" CssClass="labels" Text="PSČ: " ToolTip=""></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfCustomerZipCodeValue" runat="server" CssClass="plaintext" Text="" ToolTip="" Font-Bold="True"></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfRequiredDateOfShipment" runat="server" CssClass="labels" Text="Pož.datum dodání: "></asp:Label></td>
		                            <td valign="bottom">
			                            <asp:Label ID="lblRfRequiredDateOfShipmentValue" runat="server" CssClass="plaintext" Text="" ToolTip="" DataFormatString="{0:d}" Font-Bold="True"></asp:Label></td>
	                            </tr>
                            </table>

                            <asp:GridView ID="grdRefurbishedConfirmationDetail" runat="server" AutoGenerateColumns="False" GridLines="Vertical" CssClass="grids"
							    DataKeyNames="ID" Width="10px">
							    <HeaderStyle CssClass="gridheader" HorizontalAlign="Left" Width="100%" />
							    <FooterStyle CssClass="gridfooter" />
							    <RowStyle CssClass="gridrows" />
							    <AlternatingRowStyle CssClass="gridrowalter" />
							    <Columns>								
								    <asp:BoundField DataField="ItID" HeaderText="ID" ItemStyle-Width="10px" ReadOnly="True">
									    <ItemStyle Width="10px" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemVerKitText" HeaderText="Item/Kit" />
								    <asp:BoundField DataField="ItemOrKitID" HeaderText="ID" />
								    <asp:BoundField DataField="ItemOrKitDescription" HeaderText="Popis" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitQuantityInt" HeaderText="Schvalované množ." ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="COIItemOrKitQuantityInt" HeaderText="Objednané množ." ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitQuantityDeliveredInt" HeaderText="Již dodáno" ItemStyle-Wrap="False">
									    <ItemStyle HorizontalAlign="Right" Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitUnitOfMeasure" HeaderText="MeJe" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="ItemOrKitQualityCode" HeaderText="Kvalita" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
								    <asp:BoundField DataField="NDReceipt" HeaderText="ND doklad" ItemStyle-Wrap="False">
									    <ItemStyle Wrap="False" />
								    </asp:BoundField>
							    </Columns>
							    <SelectedRowStyle BackColor="#FF9966" Font-Bold="True" />
					        </asp:GridView>
                        </asp:Panel>
                    </div>
                    </asp:View>        
                </asp:MultiView>
            </ContentTemplate>
        </asp:UpdatePanel>        
    </div>
</asp:Content>
