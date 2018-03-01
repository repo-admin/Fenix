using System;
using System.Web.UI;

namespace Fenix
{
	public partial class Kitting : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
		internal bool MainNavEnabled
		{
			set
			{

				this.mnuKittingManuallyOrder.Visible = value;
				this.mnuKittingBrowse.Visible = value;
				this.mnuKittingReconciliation.Visible = value;
				this.mnuKittingApproval.Visible = value;
				this.mnuKittingCardStockItems.Visible = value;


			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuKittingManuallyOrder.NavigateUrl = GetRouteUrl("KittingManuallyOrder", new { });			
			this.mnuKittingReconciliation.NavigateUrl = GetRouteUrl("KittingReconciliation", new { });

			this.mnuKittingBrowse.NavigateUrl = GetRouteUrl("KittingBrowse", new { });						// skrytá položka
			this.mnuKittingApproval.NavigateUrl = GetRouteUrl("KittingApproval", new { });					// skrytá položka
			this.mnuKittingCardStockItems.NavigateUrl = GetRouteUrl("KittingCardStockItems", new { });		// skrytá položka			
		}

		protected override void OnPreRender(EventArgs e)
		{
			//string fullTitle = "Albert: Uživatel ";
			//Control ctrl = this.Master.FindControl("UpcHdrCtrl");
			//if (ctrl != null)
			//{
			//	((HeaderControl)ctrl).HeaderText = fullTitle;
			//}
			//this.Page.Title = fullTitle;

			base.OnPreRender(e);
		}
	}
}