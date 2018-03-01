using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using FenixHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// Expedice repase
	/// </summary>
	public partial class KiRepase01 : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				this.ddlReconciliationFlt.Visible = true;
				string proSelect = "SELECT * FROM (SELECT '-1' cValue,' Vše' ctext UNION ALL " +
								   " SELECT  '0' ID, 'Neodsouhlaseno' ctext  UNION ALL  SELECT '1' ID , 'Odsouhlaseno'  ctext  UNION ALL  SELECT '2' ID , 'Zamítnuto'  ctext) xx ORDER BY ctext";
				FillDdl(ref this.ddlReconciliationFlt, proSelect);
				fillData(1);
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
				this.fillData(currPageIndx);
				this.grdData.SelectedIndex = -1;
				this.pnlItems.Visible = false;
			}
		}

		private void fillData(int pageNo)
		{
			string proW = "1=1";
			if (this.ddlReconciliationFlt.SelectedValue != "-1") proW += " AND Reconciliation=" + this.ddlReconciliationFlt.SelectedValue;

			string proS = "[ID] ,[MessageId] ,[MessageTypeId] ,[MessageDescription] ,[MessageDateOfShipment] ,[CustomerID]" +
						  ",[CustomerDescription]   ,[Quantity]   ,[Reconciliation] , Case Reconciliation When 1 THEN 'Schváleno' When 2 Then 'Zamítnuto' ELSE '?' END ReconciliationText ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId] ";

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[CommunicationMessagesRefurbishRE1]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = proS;
			pagerData.WhereClause = proW;
			
			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string reconsilition = grdData.SelectedRow.Cells[7].Text;
			if (reconsilition == "?") this.btnDecision.Enabled = true; else this.btnDecision.Enabled = false;

			this.pnlItems.Visible = true;
		}

		protected void btnDecision_Click(object sender, EventArgs e)
		{
			bool mOk = true;
			string proU = string.Format("UPDATE [dbo].[CommunicationMessagesRefurbishRE1] SET Reconciliation = {0} WHERE ID = {1}", this.rdblDecision.SelectedValue.ToString(),grdData.SelectedValue.ToString() );

			SqlCommand com = new SqlCommand();
			SqlConnection con = new SqlConnection(BC.FENIXWrtConnectionString);
			com.CommandType = CommandType.Text;
			com.CommandText = proU;
			com.Connection = con;

			try
			{
				con.Open();
				com.ExecuteNonQuery();
			}
			catch (Exception)
			{
				mOk = false;
			}
			finally
			{
     
				con.Close();
				con = null;
				com = null;
			}
			if (mOk) btnBack_Click(btnDecision, EventArgs.Empty);

		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			//int id = WConvertStringToInt32(grdData.DataKeys[int.Parse(e.CommandArgument.ToString())].Value.ToString()); 
			//int id = WConvertStringToInt32(e.CommandArgument.ToString());
			if (e.CommandName == "OrderView") { 
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				OrderView(id);
			}
		}

		protected void OrderView(int id)
		{
			try
			{
				bool mOK = true;
                string proS = string.Format("SELECT [ID] ,[MessageId] ,[MessageTypeId] ,[MessageDescription] ,[MessageDateOfShipment] ,[CustomerID]" +
						  ",[CustomerDescription]   ,[Quantity]   ,[Reconciliation]  ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId] FROM " +
						  " [dbo].[CommunicationMessagesRefurbishRE1] WHERE [IsActive] = 1 AND [ID] = {0} ORDER BY 1", id);
				DataTable dtRE1 = new DataTable();
				dtRE1 = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtRE1 == null || dtRE1.Rows.Count < 1) { mOK = false; }

				proS = string.Format("SELECT [SN1] ,[SN2]" +
											" FROM [dbo].[CommunicationMessagesRefurbishRE1SerNum] WHERE [IsActive] = 1 AND [RefurbishRe1ID] = {0} ", id);
				DataTable dtRE1Sn = new DataTable();
				dtRE1Sn = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtRE1Sn == null || dtRE1Sn.Rows.Count < 1) { mOK = false; }
				if (mOK)
				{

					MemoryStream ms = new MemoryStream();
					using (ExcelPackage xls = new ExcelPackage(ms))
					{
						ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
						worksheet.Cells["A1:H3"].Style.Numberformat.Format = @"@";
						worksheet.Cells["A1:A20000"].Style.Numberformat.Format = @"@";

						try
						{
							int radek = 1;
							// nadpis
							worksheet.Row(1).Height = 24;
							worksheet.Cells[radek, 1, radek, 10].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("RE1 - REPASE");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka objednavky
							worksheet.Cells[radek, 1].Value = String.Format("Message ID");
							worksheet.Cells[radek, 2].Value = dtRE1.Rows[0][0].ToString();  // MessageID
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Message Popis");
							worksheet.Cells[radek, 4].Value = dtRE1.Rows[0][2].ToString();  // MessageDescription
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Dodavatel");
							worksheet.Cells[radek, 6].Value = dtRE1.Rows[0]["CustomerDescription"].ToString();  // CustomerDescription
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Datum odeslání");
							worksheet.Cells[radek, 8].Value = dtRE1.Rows[0]["MessageDateOfShipment"].ToString();    // MessageDateOfShipment	
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("Množství");
							worksheet.Cells[radek, 10].Value = dtRE1.Rows[0]["Quantity"].ToString() ;  //dtRE1.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							radek += 2;

							worksheet.Cells[radek, 1].Value = String.Format("SN1");
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 2].Value = String.Format("SN2");
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							radek += 1;
							DataRow drc ;
							for (int ii = 0; ii <= dtRE1Sn.Rows.Count - 1; ii++)
							{
								drc = dtRE1Sn.Rows[ii];
										
											worksheet.Cells[radek, 1].Value = drc[0].ToString();
											worksheet.Cells[radek, 2].Value = drc[1].ToString();
											radek += 1;
									}

							//worksheet.Cells["A1:B10000"].Style.Numberformat.Format = @"@";
							worksheet.Column(1).AutoFit();
							worksheet.Column(2).AutoFit();
							worksheet.Column(3).AutoFit();
							worksheet.Column(4).AutoFit();
							worksheet.Column(5).AutoFit();
							worksheet.Column(6).AutoFit();
							worksheet.Column(7).AutoFit();
							worksheet.Column(9).AutoFit();
							worksheet.Column(10).AutoFit();


							//worksheet.Cells["A1"].Value = "Sériová čísla";
							//worksheet.Cells["A1"].Style.Font.Bold = true;
							//worksheet.Cells["A1"].Style.Font.UnderLine = true;

							//worksheet.Cells["A2"].Value = "SN1";
							//worksheet.Cells["B2"].Value = "SN2";




							// lets set the header text 
							worksheet.HeaderFooter.OddHeader.CenteredText = "Tinned Goods Sales";
							// add the page number to the footer plus the total number of pages
							worksheet.HeaderFooter.OddFooter.RightAlignedText =
								string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
							// add the sheet name to the footer
							worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
							// add the file path to the footer
							worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

							//// change the sheet view to show it in page layout mode
							//worksheet.View.PageLayoutView = true;

							// set some core property values
							xls.Workbook.Properties.Title = "RE1 zaslání k repasi";
							xls.Workbook.Properties.Subject = "Sériová čísla";
							xls.Workbook.Properties.Keywords = "Office Open XML";
							xls.Workbook.Properties.Category = "Sériová čísla";
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
							// TODO
							throw;
						}
					}

				}
			}
			catch
			{

			}


		} //OrderView

		protected void search_button_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.mvwMain.ActiveViewIndex = 0;
			this.pnlItems.Visible = false;
			fillData(1);
		} 
	}
}