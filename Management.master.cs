using System;
using System.Web.UI;

namespace Fenix
{
	public partial class Management : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		internal bool MainNavEnabled
		{
			set
			{
				this.mnuMaDestPlaces.Visible = value;
				this.mnuMaCardStockItems.Visible = value;
				this.mnuKittingCodeList.Visible = value;
				this.mnuMaHistoryMovesSN.Visible = value;
				this.mnuMaInternalMovements.Visible = value;
				this.mnuMaInternalDocuments.Visible = value;
				//this.mnuMaDeletedOrders.Visible = value;					//delete via Email
				this.mnuMaDeleteMessage.Visible = value;						//delete via XML
				this.mnuMaDeleteMessageReconciliation.Visible = value;		//delete via XML
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuMaDestPlaces.NavigateUrl = GetRouteUrl("mnuMaDestPlaces", new { });
			this.mnuMaCardStockItems.NavigateUrl = GetRouteUrl("mnuMaCardStockItems", new { });
			this.mnuKittingCodeList.NavigateUrl = GetRouteUrl("KittingCodeList", new { });			
			this.mnuMaHistoryMovesSN.NavigateUrl = GetRouteUrl("mnuMaHistoryMovesSN", new { });
			this.mnuMaInternalMovements.NavigateUrl = GetRouteUrl("mnuMaInternalMovements", new { });			
			this.mnuMaInternalDocuments.NavigateUrl = GetRouteUrl("mnuMaInternalDocuments", new { });	
			//this.mnuMaDeletedOrders.NavigateUrl = GetRouteUrl("mnuMaDeletedOrders", new { });								//delete via Email			
			this.mnuMaDeleteMessage.NavigateUrl = GetRouteUrl("mnuMaDeleteMessage", new { });								//delete via XML			
			this.mnuMaDeleteMessageReconciliation.NavigateUrl = GetRouteUrl("mnuMaDeleteMessageReconciliation", new { });	//delete via XML
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
	}
}