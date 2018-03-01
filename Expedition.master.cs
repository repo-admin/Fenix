using System;
using System.Web.UI;

namespace Fenix
{
	public partial class Expedition : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		internal bool MainNavEnabled
		{
			set
			{
				this.mnuKittingShipment.Visible = value;
				this.mnuKittingShipmentReconciliation.Visible = value;
				this.mnuMaCheckReleaseNote.Visible = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuKittingShipment.NavigateUrl = GetRouteUrl("KittingShipment", new { });
			this.mnuKittingShipmentReconciliation.NavigateUrl = GetRouteUrl("KittingShipmentReconciliation", new { });
			this.mnuMaCheckReleaseNote.NavigateUrl = GetRouteUrl("mnuMaCheckReleaseNote", new { });
		}
	}
}