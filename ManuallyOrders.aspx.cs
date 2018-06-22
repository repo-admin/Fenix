using System;
using System.Configuration;

namespace Fenix
{
	public partial class ManuallyOrders : BasePage
	{
		// [17. 07. 2017 by Michal Rezler]		
		// původní obsah stránky nahrazen výpisem o tom, že stránka je ve výstavbě
		// + odeslání informačního emailu
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			// new 17. 07. 2017
			string applicationAdmin = ConfigurationManager.AppSettings["AdminAplikace"].ToString();

			if (!String.IsNullOrEmpty(applicationAdmin))
			{
				lblUnderConstruction.Text = String.Format("{0}<br />Obraťte se na administrátora aplikace: {1}", lblUnderConstruction.Text, applicationAdmin);
			}

			// odeslání varovného emailu
			try
			{
				BC.SendMail(BC.AppTitle + " stránka: " + Request.ServerVariables["SCRIPT_NAME"].ToString(), "Uživatel: " +
					((Fenix.BasePage.User)(Session["Logistika_CURRENT_USER"])).Lastname.ToString() + "<br />" + "Požadavek na stránku ManuallyOrders.aspx" + "<br />",
					true, BC.MailWarningTo, "", "");
			}
			catch { }
			// new 17. 07. 2017
		}
	}
}