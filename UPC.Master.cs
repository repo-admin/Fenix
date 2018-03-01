using System;
using System.Web.UI;
using System.Xml;
using Fenix;

namespace UpcMaster
{
	public partial class UPC : MasterPage
	{
		public static string MessageText = string.Empty;

		internal bool MainNavEnabled
		{
			set
			{
				this.t_menu_tab_home.Visible = value;
				this.mnuReception.Visible = value;
				this.mnuKitting.Visible = value;
				this.mnuReport.Visible = value;
				this.mnuManagement.Visible = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				checkUserAcces("");
				this.UpcHdrCtrl.HeaderText = BC.AppTitle;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;

			this.t_menu_tab_home.NavigateUrl = GetRouteUrl("Home", new { });
			this.mnuReception.NavigateUrl = GetRouteUrl("Reception", new { });
			this.mnuKitting.NavigateUrl = GetRouteUrl("Kitting", new { });
			this.mnuExpedition.NavigateUrl = GetRouteUrl("Expedition", new { });
			this.mnuVratRep.NavigateUrl = GetRouteUrl("VratRep", new { });
			this.mnuReport.NavigateUrl = GetRouteUrl("Report", new { });
			this.mnuManagement.NavigateUrl = GetRouteUrl("Management", new { });
		}

		private void checkUserAcces(string S)
		{
			bool userHasAccess = false;
			string userLOGIN = BasePage.CurrentUser.Login;
			//  --- 2011.6.21 --- Start ---
			string cesta = "";
			cesta = Server.MapPath("App_Data") + @"\Users.xml";
			
			using (XmlReader reader = XmlReader.Create(cesta))
			{
				reader.ReadStartElement("USERS");

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "Nazev")
						{
							string s = reader.ReadString().ToString().ToUpper();
							if (s == userLOGIN.ToUpper())
							{
								userHasAccess = true;
								Session["Logistika_CURRENT_USER"] = BasePage.CurrentUser;
								Session["Logistika_ZiCyZ"] = BasePage.CurrentUser.ID_ZiCyz;
								this.lblCurrentUser.Text = BasePage.CurrentUser.Login;
								this.lblCurrentUser.ToolTip = String.Empty;
							}
						}
					}
					reader.MoveToElement();
				}
			}

			if (userHasAccess == false) 
			{ 
				Server.Transfer("NoAcces.aspx"); 
			}
		}
	}
}