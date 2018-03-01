using System;
using System.Linq;
using System.Web;
using sd = System.Drawing;

namespace Fenix
{
	public partial class Home : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{				
			this.setMainNavigationVisibility(true);			
			
			BC.SetLabel(ref this.lblInfo, String.Empty, sd.Color.Black);

			if (HttpContext.Current.Session["Logistika_CURRENT_USER"] == null) 
			{ 
				Session["Logistika_CURRENT_USER"] = CurrentUser; 
			}

			#region Not used

			//User user = (User)HttpContext.Current.Session["Logistika_CURRENT_USER"];

			//int ZiCyzId = UPC.WebUtils.Users.User.LogonUserId;
			//if ((ZiCyzId != user.ID_ZiCyz || user.ID_ZiCyz < 1) || (!user.R && !user.W && !user.Admin))
			//{
			//	try
			//	{
			//		if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogonUser"].ToString()))
			//		{						
			//			BC.SetLabel(ref this.lblInfo, "<b>Fenix</b>:<br />Nemáte oprávnění přístupu do aplikace", sd.Color.Red);
			//			setMainNavigationVisibility(false);
			//		}
			//	}
			//	catch (Exception)
			//	{ 
			//		string help = "<b>FENIX</b>:<br />Nemáte oprávnění přístupu do aplikace<br />Obraťte se na administrátora aplikace: <b>" ;

			//		if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminAplikace"].ToString()))
			//		{
			//			help += ConfigurationManager.AppSettings["AdminAplikace"].ToString() + "</b>";
			//		}
			//		BC.SetLabel(ref this.lblInfo, help, sd.Color.Red);
			//		setMainNavigationVisibility(false);
			//	}
			//}

			#endregion
			
			Server.Transfer("HomeReconciliation.aspx");
		}
		
		/// <summary>
		/// Nastavení viditelnosti hlavní navigace stránky
		/// </summary>
		/// <param name="visibility"></param>
		private void setMainNavigationVisibility(bool visibility)
		{
			var mainNavigation = Master.FindControl("ulMainNav");
			mainNavigation.Visible = visibility;
		}

		#region Test
		/// <summary>
		/// Test EF 6.x
		/// </summary>
		private void testInternalDocumentSaveInEF6()
		{
			InternalDocuments internalDocument = new InternalDocuments();
			internalDocument.ItemVerKit = false;
			internalDocument.ItemOrKitID = 111;
			internalDocument.ItemOrKitUnitOfMeasureId = 1;
			internalDocument.ItemOrKitQualityId = 1;


			internalDocument.ItemOrKitFreeBefore = 10;
			internalDocument.ItemOrKitFreeAfter = 11;

			internalDocument.ItemOrKitUnConsilliationBefore = 12;
			internalDocument.ItemOrKitUnConsilliationAfter = 12;

			internalDocument.ItemOrKitReservedBefore = 13;
			internalDocument.ItemOrKitUnConsilliationAfter = 13;

			internalDocument.ItemOrKitReleasedForExpeditionBefore = 14;
			internalDocument.ItemOrKitReleasedForExpeditionAfter = 14;

			internalDocument.ItemOrKitExpeditedBefore = 100;
			internalDocument.ItemOrKitExpeditedAfter = 101;


			internalDocument.StockId = 2;
			internalDocument.InternalDocumentsSourceId = 1;
			internalDocument.IsActive = true;
			internalDocument.ModifyDate = DateTime.Now;
			internalDocument.ModifyUserId = 1084;

			using (var db = new FenixEntities())
			{
				try
				{
					db.Configuration.LazyLoadingEnabled = false;
					db.Configuration.ProxyCreationEnabled = false;
					db.InternalDocuments.Add(internalDocument);
					db.SaveChanges();
				}

				catch
				{
					foreach (var validationResults in db.GetValidationErrors())
					{
						foreach (var error in validationResults.ValidationErrors)
						{
							Console.WriteLine(
												"Entity Property: {0}, Error {1}",
												error.PropertyName,
												error.ErrorMessage);
						}
					}
					throw;
				}
			}
		}

		#endregion
	}
}