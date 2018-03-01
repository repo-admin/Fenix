using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UPC.WebControls;
using UPC.WebUtils.Users;

namespace Fenix
{
	public class BasePage : Page
	{
		#region Structs

		[Serializable]
		new public struct User
		{
			//Kódy jsou: Logistik administrator - 1077, Logistik network -1078, Logistik CPE - 1079, Logistik report - 1080, Logistik nahled - 1081.
			public int ID_ZiCyz { get; set; }
			public string Login { get; set; }
			public string Lastname { get; set; }
			public string Firstname { get; set; }
			public Nullable<int> Region { get; set; }
			public bool LogistikNetwork { get; set; }
			public bool LogistikCPE { get; set; }
			public bool LogistikReport { get; set; }
			public bool LogistikNahled { get; set; }
			public bool R { get; set; }
			public bool W { get; set; }
			public bool Admin { get; set; }     
			public bool IS_ACTIVE { get; set; }
			public DateTime EDIT_DATE { get; set; }
			public int EDIT_ID_USER { get; set; }
			public string COMMENT { get; set; }
		}

		#endregion

		#region Properties
		
		// TODO
		internal static User CurrentUser
		{
			get
			{
				User user = new User();
				object currUser = HttpContext.Current.Session["FENIX_CURRENT_USER"];
				user.Login = UPC.WebUtils.Users.User.LogonUserName;
				if (currUser == null)
				{
					//user.Admin = UPC.WebUtils.Users.User.IsValidForApp(1024);
					//user.R = UPC.WebUtils.Users.User.IsValidForApp(1025);
					//user.W = UPC.WebUtils.Users.User.IsValidForApp(1026);

					Zicyz zcz = new Zicyz();
					if (zcz != null)
					{
						user.Login = zcz.LoginName;
						user.ID_ZiCyz = zcz.ZcId;
						user.Lastname = zcz.LastName.Trim();
						user.Firstname = zcz.FirstName.Trim();
					}
					else
					{
						user.ID_ZiCyz = -1;
						user.Login = "";
						user.Lastname = "!";
						user.Firstname = "!";
					}

					HttpContext.Current.Session["FENIX_CURRENT_USER"] = user;
				}
				else
				{
					try { user = (User)currUser; }
					catch { user = new User(); }
				}

				#region NEPOUŽITO

				//		try
				//		{
				//			if (BC.IsInternet)
				//			{
				//				if (currUser == null) return user;
				//				else { try { return (User)currUser; } catch { return user; } }
				//			}
				//			else
				//			{
				//				if (currUser == null)
				//				{
				//					user.Admin = UPC.WebUtils.Users.User.IsValidForApp(1024);
				//					user.R = UPC.WebUtils.Users.User.IsValidForApp(1025);
				//					user.W = UPC.WebUtils.Users.User.IsValidForApp(1026);

				//					UPC.WebUtils.Users.Zicyz zcz = new UPC.WebUtils.Users.Zicyz();
				//					if (zcz != null)
				//					{
				//						user.Login = zcz.LoginName;
				//						user.ID_ZiCyz = zcz.ZcId;
				//						user.Lastname = zcz.LastName.ToString().Trim();
				//						user.Firstname = zcz.FirstName.ToString().Trim();
				//					}
				//					else
				//					{
				//						user.ID_ZiCyz = -1;
				//						user.Login = "";
				//						user.Lastname = "!";
				//						user.Firstname = "!";
				//					}

				//					HttpContext.Current.Session["FENIX_CURRENT_USER"] = user;
				//				}
				//				else
				//				{
				//					try { user = (User)currUser; }
				//					catch { user = new User(); }
				//				}

				//				if (user.ID_ZiCyz > 0)
				//				{
				//					object authToken = HttpContext.Current.Session["AuthTokenValue"];
				//					if (authToken == null)
				//					{
				//						try
				//						{
				//							Token tkn = new Token(BC.SecretKey, "AuthToken");
				//							HttpContext.Current.Session["AuthTokenValue"] = tkn.Compute(user.ID_ZiCyz.ToString(), DateTime.Now.AddSeconds(BC.ValidityPeriod));
				//						}
				//						catch { }
				//					}
				//				}
				//				else
				//				{
				//					if (HttpContext.Current.Session["AuthTokenValue"] != null) HttpContext.Current.Session.Remove("AuthTokenValue");
				//				}
				//			}
				//		}
				//		catch (Exception)
				//		{
				//		} 

				#endregion

				return user;
			}
		}

		#endregion

		protected void SetGridPager(Pager pg)
		{
			pg.PageClause = "Stránka";
			pg.BackToFirstClause = "První stránka";
			pg.BackToPageClause = "Předchozí stránka";
			pg.GoToPageClause = "Jít na stránku";
			pg.GoToLastClause = "Poslední stránka";
			pg.NextToPageClause = "Následující stránka";
			pg.NewItemClause = "Přidat nový záznam";
			pg.RecordsPerPageClause = "Záznamů na stránce";

			ListItemCollection li = new ListItemCollection();
			li.Add(new ListItem("5", "5"));
			li.Add(new ListItem("10", "10"));
			li.Add(new ListItem("15", "15"));
			li.Add(new ListItem("20", "20"));
			li.Add(new ListItem("50", "50"));
			li.Add(new ListItem("100", "100"));
			li.Add(new ListItem("200", "200"));

			pg.PageSizeItems = li;
			pg.PageSize = 15;
			pg.ShowPageSize = true;
		}

		internal static void FillDdl(ref DropDownList ddl, string proS)
		{
			try
			{
				ddl.Items.Clear();
				DataTable myDataTable = BC.GetDataTable(proS);
				ddl.DataSource = myDataTable.DefaultView;
				ddl.DataValueField = "cValue";
				ddl.DataTextField = "ctext";
				ddl.DataBind();
				ddl.SelectedValue = "-1";
			}
			catch
			{
			}
		}

		#region  prevody a dalsi

		internal Int64 WConvertStringToInt64(string value)
		{
			try
			{
				return Convert.ToInt64(value);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		internal Int32 WConvertStringToInt32(string value)
		{
			try
			{
				return Convert.ToInt32(value);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		internal double WConvertStringToDouble(string value)
		{
			try
			{
				return Convert.ToDouble(value);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		internal string WConvertStringToStringXx(string value, Int32 delka)
		{
			string s = string.Empty;
			Int32 iDelka = value.Length;
			if (iDelka > delka) return value.Substring(0, delka - 1).Replace(">", "~");
			else return value.Replace(">", "~");
		}

		internal string WConvertDateToYYYYmmDD(string value)
		{
			try
			{
				string cValue = value.Replace(".", ",");
				return Convert.ToDateTime(cValue).ToString("yyyyMMdd");
			}
			catch (Exception)
			{
				return "19000101";
			}

		}

		internal string wConvertStringToDatedd_mm_yyyy(string value)
		{
			try
			{
				string sDate = (Convert.ToDateTime(value)).ToString("dd.MM.yyyy");
				if (sDate == "01.01.1900") sDate = "";
				return sDate;
			}
			catch (Exception)
			{
				return "";
			}

		}

		internal void EnabledTrueFalseView(View vw, bool b)
		{
			foreach (Control c in vw.Controls)
			{
				if (c.GetType().Name == "TextBox")
				{
					((TextBox)c).Enabled = b;
					((TextBox)c).Font.Bold = !b;
				}
				else
					if (c.GetType().Name == "DropDownList")
					{
						((DropDownList)c).Enabled = b;
						((DropDownList)c).Font.Bold = !b;
					}
					else
						if (c.GetType().Name == "CheckBox") ((CheckBox)c).Enabled = b;
						else
							if (c.GetType().Name == "RadioButtonList") ((RadioButtonList)c).Enabled = b;
			}
		}

		internal void ClearViewControls(Control vw)
		{
			foreach (Control c in vw.Controls)
			{
				if (c.GetType().Name == "TextBox") ((TextBox)c).Text = string.Empty;
				if (c.GetType().Name == "DropDownList") ((DropDownList)c).Items.Clear();
				if (c.GetType().Name == "CheckBox") ((CheckBox)c).Checked = false; ;
				//if (c.GetType().Name == "RadioButtonList") ((RadioButtonList)c).Enabled = b;
			}
		}

		internal string TreatmentsString(string s)
		{
			string result;
			if (string.IsNullOrEmpty(s)) result = " "; else result = s;
			return result;
		}

		#endregion

		protected virtual void onBtnSaveClick()
		{
		}
	
		// *******
		public void ExcelView(string proSelect)
		{
			DataTable d = new DataTable();
			d = BC.GetDataTable(proSelect, BC.FENIXRdrConnectionString);
			MemoryStream ms = new MemoryStream();
			using (ExcelPackage xls = new ExcelPackage(ms))
			{
				ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
				worksheet.Cells["A1:P3"].Style.Numberformat.Format = @"@";      // ?
				try
				{
					int radek = 1;
					// nadpis
					worksheet.Row(1).Height = 24;
					worksheet.Cells[radek, 1, radek, 15].Merge = true;
					worksheet.Cells[radek, 1].Style.Font.Bold = true;
					worksheet.Cells[radek, 1].Style.Font.Size = 14;
					worksheet.Cells[radek, 1].Value = String.Format("Skladové karty");
					worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
					worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
					worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
					radek += 2;
					// hlavička
					worksheet.Cells[radek, 1].Value = String.Format("Id");
					worksheet.Cells[radek, 1].Style.Font.Bold = true;
					worksheet.Cells[radek, 2].Value = String.Format("Zboží");
					worksheet.Cells[radek, 2].Style.Font.Bold = true;
					worksheet.Cells[radek, 3].Value = String.Format("Kód");
					worksheet.Cells[radek, 3].Style.Font.Bold = true;
					worksheet.Cells[radek, 4].Value = String.Format("Popis");
					worksheet.Cells[radek, 4].Style.Font.Bold = true;
					worksheet.Cells[radek, 5].Value = String.Format("Kvalita");
					worksheet.Cells[radek, 5].Style.Font.Bold = true;
					worksheet.Cells[radek, 6].Value = String.Format("MJ");
					worksheet.Cells[radek, 6].Style.Font.Bold = true;
					worksheet.Cells[radek, 7].Value = String.Format("Mn.volné");
					worksheet.Cells[radek, 7].Style.Font.Bold = true;
					worksheet.Cells[radek, 8].Value = String.Format("Mn.ke schválení");
					worksheet.Cells[radek, 8].Style.Font.Bold = true;
					worksheet.Cells[radek, 9].Value = String.Format("Mn.rezervované");
					worksheet.Cells[radek, 9].Style.Font.Bold = true;
					worksheet.Cells[radek, 10].Value = String.Format("Mn.uvolněné");
					worksheet.Cells[radek, 10].Style.Font.Bold = true;
					worksheet.Cells[radek, 11].Value = String.Format("Mn.expedované");
					worksheet.Cells[radek, 11].Style.Font.Bold = true;
					worksheet.Cells[radek, 12].Value = String.Format("Typ");
					worksheet.Cells[radek, 12].Style.Font.Bold = true;
					worksheet.Cells[radek, 13].Value = String.Format("PC");
					worksheet.Cells[radek, 13].Style.Font.Bold = true;
					worksheet.Cells[radek, 14].Value = String.Format("Packaking");
					worksheet.Cells[radek, 14].Style.Font.Bold = true;
					worksheet.Cells[radek, 15].Value = String.Format("Aktivita");
					worksheet.Cells[radek, 15].Style.Font.Bold = true;
					//worksheet.Cells[radek, 16].Value = String.Format("");
					//worksheet.Cells[radek, 16].Style.Font.Bold = true;
					radek += 1;

					foreach (DataRow r in d.Rows)
					{
						worksheet.Cells[radek, 1].Value = r["Id"].ToString();
						//worksheet.Cells[radek, 1].Style.Font.Bold = true;
						worksheet.Cells[radek, 2].Value = r["ItemVerKitDescription"].ToString();
						//worksheet.Cells[radek, 2].Style.Font.Bold = true;
						worksheet.Cells[radek, 3].Value = r["ItemOrKitID"].ToString();
						//worksheet.Cells[radek, 3].Style.Font.Bold = true;
						worksheet.Cells[radek, 4].Value = r["DescriptionCz"].ToString();
						//worksheet.Cells[radek, 4].Style.Font.Bold = true;
						worksheet.Cells[radek, 5].Value = r["QualitiesCode"].ToString();
						//worksheet.Cells[radek, 5].Style.Font.Bold = true;
						worksheet.Cells[radek, 6].Value = r["MeasuresCode"].ToString();
						//worksheet.Cells[radek, 6].Style.Font.Bold = true;
						worksheet.Cells[radek, 7].Value = r["ItemOrKitFreeInteger"].ToString();
						worksheet.Cells[radek, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 8].Value = r["ItemOrKitUnConsilliationInteger"].ToString();
						worksheet.Cells[radek, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 9].Value = r["ItemOrKitReservedInteger"].ToString();
						worksheet.Cells[radek, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 10].Value = r["ItemOrKitReleasedForExpeditionInteger"].ToString();
						worksheet.Cells[radek, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 11].Value = r["ItemOrKitExpeditedInteger"].ToString();
						worksheet.Cells[radek, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 12].Value = r["ItemType"].ToString();
						//worksheet.Cells[radek, 12].Style.Font.Bold = true;
						worksheet.Cells[radek, 13].Value = r["PC"].ToString();
						//worksheet.Cells[radek, 13].Style.Font.Bold = true;
						worksheet.Cells[radek, 14].Value = r["Packaging"].ToString();
						worksheet.Cells[radek, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 15].Value = r["IsActive"].ToString();
						//worksheet.Cells[radek, 15].Style.Font.Bold = true;
						radek += 1;
					}

					worksheet.Column(1).AutoFit();
					worksheet.Column(2).AutoFit();
					worksheet.Column(3).AutoFit();
					worksheet.Column(4).AutoFit();
					worksheet.Column(5).AutoFit();
					worksheet.Column(6).AutoFit();
					worksheet.Column(7).AutoFit();
					worksheet.Column(8).AutoFit();
					worksheet.Column(9).AutoFit();
					worksheet.Column(10).AutoFit();
					worksheet.Column(11).AutoFit();
					worksheet.Column(12).AutoFit();
					worksheet.Column(13).AutoFit();
					worksheet.Column(14).AutoFit();
					worksheet.Column(15).AutoFit();

					// set some core property values
					xls.Workbook.Properties.Title = "Skladové karty";
					xls.Workbook.Properties.Subject = "Skladové karty";
					xls.Workbook.Properties.Keywords = "Office Open XML";
					xls.Workbook.Properties.Category = "Skladové karty";
					xls.Workbook.Properties.Comments = "";
					// set some extended property values
					xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";

					// save the new spreadsheet to the stream
					xls.Save();
					ms.Flush();
					ms.Seek(0, SeekOrigin.Begin);

					Response.Clear();
					Response.Buffer = true;
					Response.AddHeader("content-disposition", "attachment;filename=Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.Charset = "";
					EnableViewState = false;

					Response.BinaryWrite(ms.ToArray());
					ms.Close();
					Response.End();
				}
				catch (Exception)
				{
					// TODO : jak vyresit exception vznikajici volanim prikazu Response.End();
				}
			}

			////

		}
		// ******
		protected void CheckUserAcces(string par)
		{
			bool userHasAccess = false;
			string currentUserLogin = CurrentUser.Login;
			string cesta = Server.MapPath("App_Data") + @"\Users.xml";

			using (XmlReader reader = XmlReader.Create(cesta))
			{
				reader.ReadStartElement("USERS");

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "Nazev")
						{
							string login = reader.ReadString().ToUpper();
							if (login == currentUserLogin.ToUpper())
							{
								userHasAccess = true;
								Session["Logistika_CURRENT_USER"] = CurrentUser;
								Session["Logistika_ZiCyZ"] = CurrentUser.ID_ZiCyz;
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