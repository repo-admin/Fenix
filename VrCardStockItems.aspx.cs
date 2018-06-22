using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	/// <summary>
	/// Vratky-Repase -> Karty materiálů a zařízení
	/// </summary>
	public partial class VrCardStockItems : BasePage
	{
		private const int HIDE_COLUMN = 6;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);
				BaseHelper.FillDdlGroupGoods(ref this.ddlGroupGoodsFlt);
				BaseHelper.FillDdlItemType(ref this.ddlItemTypeFlt22);
				this.mvwMain.ActiveViewIndex = 0;	
				this.fillPagerData(BC.PAGER_FIRST_PAGE);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.DesignMode) return;

			base.OnInit(e);
			this.grdPager.ShowNewItem = false;
			this.grdPager.PageSize = 10;
			this.grdPager.Command += new CommandEventHandler(this.pagerCommand);
		}

		private void pagerCommand(object sender, CommandEventArgs e)
		{
			int currPageIndx = Convert.ToInt32(e.CommandArgument);
			if (currPageIndx == -1)
			{
				// Novy zaznam
				//this.OnNewRecord(sender, e);
			}
			else
			{
				this.grdPager.CurrentIndex = currPageIndx;
				this.fillPagerData(currPageIndx);
			}
		}

		private void fillPagerData()
		{
			this.fillPagerData(this.grdPager.CurrentIndex);
		}

		private void fillPagerData(int pageNo)
		{			
			string proW = "IsActive=1";
			if (this.ddlKitQualitiesFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemOrKitQuality]=" + this.ddlKitQualitiesFlt.SelectedValue.ToString();
			if (this.ddlItemVerKitFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemVerKit]= CAST(" + this.ddlItemVerKitFlt.SelectedValue.ToString() + " AS BIT)";
			if (this.ddlGroupGoodsFlt.SelectedValue.ToString() != "-1") proW += " AND [GroupGoods]= '" + this.ddlGroupGoodsFlt.SelectedValue.ToString() + "'";
			if (this.ddlItemTypeFlt22.SelectedValue.ToString() != "-1") proW += " AND [ItemType]= '" + this.ddlItemTypeFlt22.SelectedValue.ToString() + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(ItemOrKitID AS VARCHAR(50))," + delka.ToString() + ") = " + this.tbxMaterialCodeFlt.Text.Trim();
			}

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwCardStockItems]";
			pagerData.OrderBy = "ItemVerKit";
			pagerData.ColumnList = "*";
			pagerData.WhereClause = proW;

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, HIDE_COLUMN);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);			
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}			
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void excelSmall_button_Click(object sender, ImageClickEventArgs e)
		{

			//string proS = "SELECT [ID],[ItemVerKit],[ItemVerKitDescription],[GroupGoods],[Code],[DescriptionCz],[ItemOrKitID],[ItemOrKitQuantityInteger],[QualitiesCode]"+
			//			  ",[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger],[ItemOrKitReservedInteger],[ItemOrKitReleasedForExpeditionInteger],[ItemOrKitExpeditedInteger]"+		
			//			  ",[MeasuresCode],[ItemType],[PC],[cdlStocksName],[cdlKitsCode],[cdlKitGroupsCode]  FROM [dbo].[vwCardStockItems]";
			//DataTable dtObjHl = new DataTable();
			//dtObjHl = getDataTable(proS, FENIXRdrConnectionString);
			//if (!(dtObjHl == null || dtObjHl.Rows.Count < 1)) {


			//		System.IO.MemoryStream ms = new System.IO.MemoryStream();
			//		using (ExcelPackage xls = new ExcelPackage(ms))
			//		{
			//			ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
			//			worksheet.Cells["A1:H3"].Style.Numberformat.Format = @"@";
			//			worksheet.Cells["A1:A20000"].Style.Numberformat.Format = @"@";

			//try
			//{
			//	int radek = 1;
			//	// nadpis
			//	worksheet.Row(1).Height = 24;
			//	worksheet.Cells[radek, 1, radek, 8].Merge = true;
			//	worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 1].Style.Font.Size = 14;
			//	worksheet.Cells[radek, 1].Value = String.Format("Skladové karty");
			//	worksheet.Cells[radek, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightUp;
			//	worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
			//	worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
			//	radek += 2;
			//	// hlavicka objednavky
			//	worksheet.Cells[radek, 1].Value = String.Format("ID");
			//	worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 2].Value = String.Format("Item/kit");
			//	worksheet.Cells[radek, 2].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 5].Value = String.Format("Dodavatel");
			//	worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["ItemSupplierDescription"].ToString();  // ItemSupplierDescription
			//	worksheet.Cells[radek, 6].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 7].Value = String.Format("Datum odeslání");
			//	worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["MessageDateOfShipment"].ToString();    // MessageDateOfShipment	
			//	worksheet.Cells[radek, 8].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 9].Value = String.Format("");
			//	worksheet.Cells[radek, 10].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
			//	worksheet.Cells[radek, 10].Style.Font.Bold = true;
			//	radek += 1;
			//	// identifikace objednavky v Heliosu
			//	worksheet.Cells[radek, 1].Value = String.Format("Řada dokladů");
			//	worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0]["RadaDokladu"].ToString();  // RadaDokladu
			//	worksheet.Cells[radek, 2].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 3].Value = String.Format("Pořadové číslo");
			//	worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["PoradoveCislo"].ToString();  // PoradoveCislo
			//	worksheet.Cells[radek, 4].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 5].Value = String.Format(""); // String.Format("HeliosOrderId");
			//	worksheet.Cells[radek, 6].Value = String.Format(""); // dtObjHl.Rows[0]["HeliosOrderId"].ToString();  // HeliosOrderId
			//	worksheet.Cells[radek, 6].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 7].Value = String.Format("Pož. datum naskladnění");
			//	worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();
			//	worksheet.Cells[radek, 8].Style.Numberformat.Format = "yyy-mm-dd";
			//	worksheet.Cells[radek, 8].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 9].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
			//	worksheet.Cells[radek, 9].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 10].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
			//	worksheet.Cells[radek, 10].Style.Font.Bold = true;
			//	radek += 2;
			//	// detaily objednávky
			//	worksheet.Cells[radek, 1].Value = String.Format("HeliosOrderRecordId");
			//	worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 2].Value = String.Format("SkZ");
			//	worksheet.Cells[radek, 2].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 3].Value = String.Format("Kód");
			//	worksheet.Cells[radek, 3].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 4].Value = String.Format("Popis");
			//	worksheet.Cells[radek, 4].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 5].Value = String.Format("Objed. množství");
			//	worksheet.Cells[radek, 5].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 6].Value = String.Format("Dodané množství");
			//	worksheet.Cells[radek, 6].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 7].Value = String.Format("MJ");
			//	worksheet.Cells[radek, 7].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 8].Value = String.Format("Kvalita");
			//	worksheet.Cells[radek, 8].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 9].Value = String.Format("");
			//	worksheet.Cells[radek, 9].Style.Font.Bold = true;
			//	worksheet.Cells[radek, 10].Value = String.Format("");
			//	worksheet.Cells[radek, 10].Style.Font.Bold = true;
			//	radek += 1;

			//	foreach (DataRow dr in dtObjR.Rows)
			//	{
			//		worksheet.Cells[radek, 1].Value = dr["HeliosOrderRecordId"].ToString();
			//		worksheet.Cells[radek, 2].Value = dr["GroupGoods"].ToString();
			//		worksheet.Cells[radek, 3].Value = dr["ItemCode"].ToString();
			//		worksheet.Cells[radek, 4].Value = dr["ItemDescription"].ToString();
			//		worksheet.Cells[radek, 5].Value = dr["ItemQuantity"].ToString();
			//		worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			//		worksheet.Cells[radek, 6].Value = dr["ItemQuantityDelivered"].ToString();
			//		worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			//		worksheet.Cells[radek, 7].Value = dr["ItemUnitOfMeasure"].ToString();
			//		worksheet.Cells[radek, 8].Value = dr["ItemQualityCode"].ToString();
			//		worksheet.Cells[radek, 9].Value = String.Format("");
			//		worksheet.Cells[radek, 10].Value = String.Format("");
			//		radek += 1;
			//	}
			//	if (mOKR)
			//	{
			//		radek += 2;
			//		// hlavička konfirmace
			//		worksheet.Cells[radek, 1, radek, 8].Merge = true;
			//		worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//		worksheet.Cells[radek, 1].Style.Font.Size = 14;
			//		worksheet.Cells[radek, 1].Value = String.Format("R0 - Confirmace objednávky");
			//		worksheet.Cells[radek, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightUp;
			//		worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
			//		worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

			//		foreach (DataRow dr in dtObjHlCon.Rows)
			//		{
			//			radek += 1;
			//			worksheet.Cells[radek, 1].Value = String.Format("Message Id");
			//			worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 2].Value = String.Format("Message popis");
			//			worksheet.Cells[radek, 2].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 3].Value = String.Format("Dodavatel");
			//			worksheet.Cells[radek, 3].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 4].Value = String.Format("Odsouhlasení");
			//			worksheet.Cells[radek, 4].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 5].Value = String.Format("Datum zapsání do Fenixu");
			//			worksheet.Cells[radek, 5].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 6].Value = String.Format("Aktivita");
			//			worksheet.Cells[radek, 6].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 7].Value = String.Format("");
			//			worksheet.Cells[radek, 7].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 8].Value = String.Format("");
			//			worksheet.Cells[radek, 8].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 9].Value = String.Format("");
			//			worksheet.Cells[radek, 9].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 10].Value = String.Format("");
			//			worksheet.Cells[radek, 10].Style.Font.Bold = true;
			//			radek += 1;
			//			worksheet.Cells[radek, 1].Value = dr["MessageId"].ToString();
			//			worksheet.Cells[radek, 2].Value = dr["MessageDescription"].ToString();
			//			worksheet.Cells[radek, 3].Value = dr["ItemSupplierDescription"].ToString();

			//			if (dr["Reconciliation"].ToString() == "2")
			//			{
			//				worksheet.Cells[radek, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightUp;
			//				worksheet.Cells[radek, 4].Style.Fill.BackgroundColor.SetColor(Color.Red);
			//				worksheet.Cells[radek, 4].Value = String.Format("Zamítnuto");
			//			}
			//			else
			//				worksheet.Cells[radek, 4].Value = String.Format("Schváleno");
			//			worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
			//			worksheet.Cells[radek, 6].Value = dr["IsActive"].ToString();
			//			worksheet.Cells[radek, 7].Value = String.Format("");
			//			worksheet.Cells[radek, 8].Value = String.Format("");
			//			worksheet.Cells[radek, 9].Value = String.Format("");
			//			worksheet.Cells[radek, 10].Value = String.Format("");
			//			radek += 2;
			//			worksheet.Cells[radek, 1].Value = String.Format("Item Id");
			//			worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 2].Value = String.Format("SkZ");
			//			worksheet.Cells[radek, 2].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 3].Value = String.Format("Kód");
			//			worksheet.Cells[radek, 3].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 4].Value = String.Format("Popis");
			//			worksheet.Cells[radek, 4].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 5].Value = String.Format("Množství");
			//			worksheet.Cells[radek, 5].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 6].Value = String.Format("MJ");
			//			worksheet.Cells[radek, 6].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 7].Value = String.Format("Doklad ND");
			//			worksheet.Cells[radek, 7].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 8].Value = String.Format("");
			//			worksheet.Cells[radek, 8].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 9].Value = String.Format("");
			//			worksheet.Cells[radek, 9].Style.Font.Bold = true;
			//			worksheet.Cells[radek, 10].Value = String.Format("");
			//			worksheet.Cells[radek, 10].Style.Font.Bold = true;
			//			radek += 1;



			//			for (int ii = 0; ii <= dtObjHlRCon.Rows.Count - 1; ii++)
			//			{
			//				DataRow drc = dtObjHlRCon.Rows[ii];

			//				if (dr["ID"].ToString() == drc["CMSOId"].ToString())
			//				{
			//					worksheet.Cells[radek, 1].Value = drc["ItemID"].ToString();
			//					worksheet.Cells[radek, 2].Value = drc["GroupGoods"].ToString();
			//					worksheet.Cells[radek, 3].Value = drc["Code"].ToString();
			//					worksheet.Cells[radek, 4].Value = drc["ItemDescription"].ToString();
			//					worksheet.Cells[radek, 5].Value = drc["ItemQuantity"].ToString();
			//					worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			//					worksheet.Cells[radek, 6].Value = drc["ItemUnitOfMeasure"].ToString();
			//					worksheet.Cells[radek, 7].Value = drc["NDReceipt"].ToString();
			//					worksheet.Cells[radek, 8].Value = String.Format("");
			//					worksheet.Cells[radek, 9].Value = String.Format("");
			//					worksheet.Cells[radek, 10].Value = String.Format("");
			//					radek += 1;
			//					if (!string.IsNullOrWhiteSpace(drc["ItemSNs"].ToString()))
			//					{
			//						worksheet.Cells[radek, 1].Value = String.Format("Sériová čísla");
			//						worksheet.Cells[radek, 1].Style.Font.Bold = true;
			//						radek += 1;
			//						string[] sn = drc["ItemSNs"].ToString().Split(',');
			//						foreach (var e in sn)
			//						{
			//							worksheet.Cells[radek, 1].Value = e.ToString();
			//							//worksheet.Cells[radek, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
			//							//worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.SlateBlue);
			//							radek += 1;
			//						}
			//					}
			//				}
			//			}

			//		}

			//	}






			//	//foreach (var par in dvojice)
			//	//{
			//	//	string[] sn = par.Split(',');
			//	//	string adresaA = String.Format("A{0}", radek);
			//	//	string adresaB = String.Format("B{0}", radek);
			//	//	worksheet.Cells[adresaA].Value = sn[0];
			//	//	worksheet.Cells[adresaB].Value = sn[1];
			//	//	radek++;
			//	//}

			//	//worksheet.Cells["A1:B10000"].Style.Numberformat.Format = @"@";
			//	worksheet.Column(1).AutoFit();
			//	worksheet.Column(2).AutoFit();
			//	worksheet.Column(3).AutoFit();
			//	worksheet.Column(4).AutoFit();
			//	worksheet.Column(5).AutoFit();
			//	worksheet.Column(6).AutoFit();
			//	worksheet.Column(7).AutoFit();
			//	worksheet.Column(9).AutoFit();
			//	worksheet.Column(10).AutoFit();


			//	//worksheet.Cells["A1"].Value = "Sériová čísla";
			//	//worksheet.Cells["A1"].Style.Font.Bold = true;
			//	//worksheet.Cells["A1"].Style.Font.UnderLine = true;

			//	//worksheet.Cells["A2"].Value = "SN1";
			//	//worksheet.Cells["B2"].Value = "SN2";




			//	// lets set the header text 
			//	worksheet.HeaderFooter.OddHeader.CenteredText = "Tinned Goods Sales";
			//	// add the page number to the footer plus the total number of pages
			//	worksheet.HeaderFooter.OddFooter.RightAlignedText =
			//		string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
			//	// add the sheet name to the footer
			//	worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
			//	// add the file path to the footer
			//	worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

			//	//// change the sheet view to show it in page layout mode
			//	//worksheet.View.PageLayoutView = true;

			//	// set some core property values
			//	xls.Workbook.Properties.Title = "RO objednávka";
			//	xls.Workbook.Properties.Subject = "Sériová čísla";
			//	xls.Workbook.Properties.Keywords = "Office Open XML";
			//	xls.Workbook.Properties.Category = "Sériová čísla";
			//	xls.Workbook.Properties.Comments = "";
			//	// set some extended property values
			//	xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";

			//	// save the new spreadsheet to the stream
			//	xls.Save();
			//	ms.Flush();
			//	ms.Seek(0, System.IO.SeekOrigin.Begin);

			//	Response.Clear();
			//	Response.Buffer = true;
			//	Response.AddHeader("content-disposition", "attachment;filename=Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
			//	Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			//	Response.Charset = "";
			//	EnableViewState = false;

			//	Response.BinaryWrite(ms.ToArray());
			//	ms.Close();
			//	Response.End();
			//}

			//catch (Exception)
			//{
			//	// TODO
			//	throw;
			//}
			//}


		}
	}
}