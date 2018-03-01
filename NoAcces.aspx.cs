using System;
using System.Configuration;
using System.Web.UI;

namespace Fenix
{
	public partial class NoAcces : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{	
			lblNoAccess.Text = String.Format("{0}<br />Nemáte oprávnění ke vstupu do aplikace", BC.AppTitle);

			string applicationAdmin = ConfigurationManager.AppSettings["AdminAplikace"].ToString();

			if (!String.IsNullOrEmpty(applicationAdmin))
			{
				lblNoAccess.Text = String.Format("{0}.<br />Obraťte se na administrátora aplikace: {1}", lblNoAccess.Text, applicationAdmin);
			}
		}
	}
}