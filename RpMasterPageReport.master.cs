using System;
using System.Web.UI;

namespace Fenix
{
	public partial class RpMasterPageReport : MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		internal bool MainNavEnabled
		{
			set
			{
				this.mnuRpExpedice.Visible = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.mnuRpExpedice.NavigateUrl = GetRouteUrl("mnuRpExpedice", new { });
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
	}
}