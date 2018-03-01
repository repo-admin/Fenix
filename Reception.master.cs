using System;
using System.Web.UI;

namespace Fenix
{
	public partial class Reception : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		internal bool MainNavEnabled
		{
			set
			{
				this.mnuReceptionManuallyOrder.Visible = value;
				this.mnuReceptionBrowse.Visible = value;
				this.mnuCardStockItems.Visible = value;
				this.mnuReceptionReconciliation.Visible = value;
				this.mnuKittingRelease.Visible = value;
				this.mnuVrRepaseRF0.Visible = value;
				this.mnuVrRepaseRF1.Visible = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuReceptionManuallyOrder.NavigateUrl = GetRouteUrl("ReceptionManuallyOrder", new { });
			this.mnuVrRepaseRF0.NavigateUrl = GetRouteUrl("VrRepaseRF0", new { });
			this.mnuReceptionReconciliation.NavigateUrl = GetRouteUrl("ReceptionReconciliation", new { });
			this.mnuVrRepaseRF1.NavigateUrl = GetRouteUrl("VrRepaseRF1", new { });
			this.mnuKittingRelease.NavigateUrl = GetRouteUrl("KittingRelease", new { });

			this.mnuReceptionBrowse.NavigateUrl = GetRouteUrl("ReceptionBrowse", new { });
			this.mnuCardStockItems.NavigateUrl = GetRouteUrl("CardStockItems", new { });
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
	}
}