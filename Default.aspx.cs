using System;

namespace Fenix
{
	public partial class Default : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.RedirectToRoute("Home");
		}
	}
}