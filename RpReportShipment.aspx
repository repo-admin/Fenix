<%@ Page Title="" Language="C#" MasterPageFile="~/RpMasterPageReport.master" AutoEventWireup="true" CodeBehind="RpReportShipment.aspx.cs" Inherits="Fenix.RpReportShipment" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:MultiView ID="mvwMain" runat="server">
            <asp:View ID="vwGrid" runat="server">
                <h1>Report expedice</h1>
                <table>
                    <tr>  
                        <td style="height: 30%;">
                            <asp:Label ID="lblDateFrom" runat="server" CssClass="labels" Text="Datum Od:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txbDateFrom" runat="server" MaxLength="12"></asp:TextBox>
                            <cc2:CalendarExtender ID="txbDateFrom_CalendarExtender" runat="server" TargetControlID="txbDateFrom" CssClass="calendars">
                            </cc2:CalendarExtender>
                        </td>
                        <td style="width: 70px;">
                        </td>  
                        <td style="height: 30%;">
                            <asp:Label ID="lblDateTo" runat="server" CssClass="labels" Text="Datum Do:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txbDateTo" runat="server" MaxLength="12"></asp:TextBox>
                            <cc2:CalendarExtender ID="txbDateTo_CalendarExtender" runat="server" TargetControlID="txbDateTo" CssClass="calendars">
                            </cc2:CalendarExtender>
                        </td>
                        <td style="width: 70px;">
                        </td>
                        <td>
                            &nbsp;
                            <br />
                            <asp:Button ID="btnExport" runat="server" Text="Vytvořit report" Width="190px" OnClick="btnExport_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Label ID="lblErrInfo" runat="server" CssClass="errortext"></asp:Label>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>