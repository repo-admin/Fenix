using System;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace Fenix
{
	public class Global : HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
			{
				Path = "~/scripts/jquery-1.4.1.min.js",
				DebugPath = "~/scripts/jquery-1.4.1.js",
				CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.1.min.js",
				CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.1.js"
			});
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			// Zásobování[Recepce] (přehled objednávek recepce, ruční objednávka recepce, 
			//                      karty materiálů a zařízení, schválení příjmu položek)
			routes.MapPageRoute("Home", "Home", "~/Home.aspx");
			routes.MapPageRoute("Reception", "Reception", "~/ReReception.aspx");
			routes.MapPageRoute("ReceptionManuallyOrder", "ReceptionManuallyOrder", "~/ReManuallyOrders.aspx");
			routes.MapPageRoute("ReceptionBrowse", "ReceptionBrowse", "~/ReReceptionBrowse.aspx");
			routes.MapPageRoute("CardStockItems", "CardStockItems", "~/ReCardStockItems.aspx");
			routes.MapPageRoute("ReceptionReconciliation", "ReceptionReconciliation", "~/ReReconciliation.aspx");

			// Sestavy[Kity] (Přehled objednávek sestav, Ruční objednávka sestavy, Číselník sestav
			//                Schválení příjmu kitů, Karty materiálů a zařízení, Objednávka expedice, 
			//                Schválení expedice, Uvolnění kitů)
			routes.MapPageRoute("Kitting", "Kitting", "~/KiKitting.aspx");
			routes.MapPageRoute("KittingManuallyOrder", "KittingManuallyOrder", "~/KiManuallyOrders.aspx");
			routes.MapPageRoute("KittingBrowse", "KittingBrowse", "~/KiKittingBrowse.aspx");
			routes.MapPageRoute("KittingCodeList", "KittingCodeList", "~/KiKittingCodeList.aspx");
			routes.MapPageRoute("KittingReconciliation", "KittingReconciliation", "~/KiReconciliation.aspx");
			routes.MapPageRoute("KittingApproval", "KittingApproval", "~/KiApproval.aspx");
			routes.MapPageRoute("KittingCardStockItems", "KittingCardStockItems", "~/KiCardStockItems.aspx");
			routes.MapPageRoute("KittingShipment", "KittingShipment", "~/KiShipment.aspx");
			routes.MapPageRoute("KittingShipmentReconciliation", "KittingShipmentReconciliation", "~/KiShipmentReconciliation.aspx");
			routes.MapPageRoute("KittingReturns", "KittingReturns", "~/KiReturns.aspx");
			//routes.MapPageRoute("KittingRepase01", "KittingRepase01", "~/KiRepase01.aspx");
			routes.MapPageRoute("KittingRelease", "KittingRelease", "~/KiRelease.aspx");  // "~/Test.aspx"

			routes.MapPageRoute("Expedition", "Expedition", "~/ExExpedition.aspx"); 
			
			routes.MapPageRoute("VratRep", "VratRep", "~/VrRpVratRep.aspx");
			routes.MapPageRoute("VrRepaseRF0", "VrRepaseRF0", "~/VrRepaseRF0.aspx");
			routes.MapPageRoute("VrRepaseRF1", "VrRepaseRF1", "~/VrRepaseRF1.aspx");
			routes.MapPageRoute("VrCardStockItems", "VrCardStockItems", "~/VrCardStockItems.aspx");
			routes.MapPageRoute("VrVratkyVR3", "VrVratkyVR3", "~/VrRpVratkyVR3.aspx");
			routes.MapPageRoute("VrVratkyVR2", "VrVratkyVR2", "~/VrRpVratkyVR2.aspx");

			//Reporty (Expedice)
			routes.MapPageRoute("Report", "Report", "~/RpReport.aspx");			
			routes.MapPageRoute("mnuRpExpedice", "mnuRpExpedice", "~/RpReportShipment.aspx");
			                     
			//Správa
			routes.MapPageRoute("Management", "Management", "~/MaManagement.aspx");
			routes.MapPageRoute("mnuMaDestPlaces", "mnuMaDestPlaces", "~/MaDestPlaces.aspx");
			routes.MapPageRoute("mnuMaCardStockItems", "mnuMaCardStockItems", "~/MaCardStockItems.aspx");
			routes.MapPageRoute("mnuMaCheckReleaseNote", "mnuMaCheckReleaseNote", "~/MaCheckReleaseNote.aspx");
			routes.MapPageRoute("mnuMaHistoryMovesSN", "mnuMaHistoryMovesSN", "~/MaHistoryMovesSN.aspx");
			routes.MapPageRoute("mnuMaInternalMovements", "mnuMaInternalMovements", "~/MaInternalMovements.aspx");
			
			routes.MapPageRoute("mnuMaInternalDocuments", "mnuMaInternalDocuments", "~/MaInternalDocuments.aspx");

			routes.MapPageRoute("mnuMaDeletedOrders", "mnuMaDeletedOrders", "~/MaDeletedOrders.aspx");													//via Email
			routes.MapPageRoute("mnuMaDeleteMessage", "mnuMaDeleteMessage", "~/MaDeleteMessage.aspx");													//via XML
			routes.MapPageRoute("mnuMaDeleteMessageReconciliation", "mnuMaDeleteMessageReconciliation", "~/MaDeleteMessageReconciliation.aspx");			//via XML			
		}
	}
}