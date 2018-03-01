using System;
using System.Web.UI;

namespace Fenix
{
	public partial class VratRep : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
		internal bool MainNavEnabled
		{
			set
			{
				this.mnuKittingReturns01.Visible = value;
				//this.mnuKittingRepase01.Visible = value;
				this.mnuVrCardStockItems.Visible = value;
				this.mnuVrRepaseVR2.Visible = value;
				this.mnuVrRepaseVR3.Visible = false;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuKittingReturns01.NavigateUrl = GetRouteUrl("KittingReturns", new { }); // VR1
			//this.mnuKittingRepase01.NavigateUrl = GetRouteUrl("KittingRepase01", new { });
			this.mnuVrCardStockItems.NavigateUrl = GetRouteUrl("VrCardStockItems", new { });
			this.mnuVrRepaseVR2.NavigateUrl = GetRouteUrl("VrVratkyVR2", new { });
			this.mnuVrRepaseVR3.NavigateUrl = GetRouteUrl("VrVratkyVR3", new { });
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
	}
}