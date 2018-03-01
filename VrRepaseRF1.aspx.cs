using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using FenixHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	///	RF1 - Schvalování RF0  (Potvrzení závozu na ND)  {RF0 = refurbished order  RF1 = refurbished confirmation}
	/// </summary>
	public partial class VrRepaseRF1 : BasePage
	{
		#region Properties

		/// <summary>
		/// Bez rozhodnutí
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		/// <summary>
		/// Číslo sloupce pro Refurbished Confirmation ID
		/// </summary>
		private const int COLUMN_REFURBISHED_CONFIRMATION_ID = 1;

		/// <summary>
		/// Číslo sloupce pro schválení {druhy schválení -> 0 .. bez rozhodnutí  1 .. schváleno    2 .. zamítnuto	3 .. D0 odeslána}
		/// </summary>
		private const int COLUMN_RECONCILIATION = 11;

		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				BaseHelper.FillDdlCustomerName(ref this.ddlCompanyName);
				BaseHelper.FillDdlCustomerCity(ref this.ddlCityName);
				BaseHelper.FillDdlDecision(ref this.ddlDecisionFlt);
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

		private void fillPagerData(int pageNo)
		{
			this.lblErrInfo.Text = "";

			ViewState["Filter"] = " 1=1 ";

			if (!string.IsNullOrWhiteSpace(this.tbxDatumOdeslaniFlt.Text.Trim())) ViewState["Filter"] += " AND CONVERT(CHAR(8), [MessageDateOfShipment], 112) = '" + WConvertDateToYYYYmmDD(this.tbxDatumOdeslaniFlt.Text.Trim()) + "'";
			if (this.ddlCompanyName.SelectedValue != "-1") ViewState["Filter"] += " AND [CompanyName] like '" + this.ddlCompanyName.SelectedItem.Text.Trim() + "%'";
			if (this.ddlCityName.SelectedValue != "-1") ViewState["Filter"] += " AND [City] like '" + this.ddlCityName.SelectedItem.Text.Trim() + "%'";
			if (this.ddlDecisionFlt.SelectedValue != "-1") ViewState["Filter"] += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue;
			if (!string.IsNullOrWhiteSpace(this.tbxMessageIdFlt.Text)) ViewState["Filter"] += " AND [MessageId] = " + this.tbxMessageIdFlt.Text.Trim();
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwRefurbishedConfirmationHd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = " [ID],[MessageId],[MessageTypeId],[MessageDescription],[DateOfShipment],[RefurbishedOrderID],[ReconciliationYesNo],[MessageDateOfShipment], [DateOfDelivery], [CompanyName], [City], [Reconciliation]";
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, COLUMN_RECONCILIATION);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.checkReceptionOrderConfirmation();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());				
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblErrInfo.Text = "";
			GridViewRow drv = grdData.SelectedRow;
			ViewState["vwCMRSentID"] = drv.Cells[1].Text;

			Session["IsRefresh"] = "0";
			this.pnlItems.Visible = true;
			this.gvConfirmationItems.SelectedIndex = -1;
			//this.pnlDetails.Visible = false;
			this.gvConfirmationItems.Columns[0].Visible = true;
			this.gvConfirmationItems.DataSource = null; this.gvConfirmationItems.DataBind();
			this.gvConfirmationItems.Columns[0].Visible = false;

			string proS = string.Empty;
			DataTable myDT = new DataTable();
			DataTable myDataTable = new DataTable();
			DataTable myD = new DataTable();
			try
			{
				proS = string.Format("SELECT DISTINCT [ID],[CMSOId],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity]" +
								 ",[ItemOrKitQuantityInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],ItID " +
								 ",[ItemOrKitQualityId],[ItemOrKitQualityCode],[IncotermsId],[IncotermDescription],[NDReceipt],[KitSNs],[IsActive],[ModifyDate],[ModifyUserId],[RefurbishedOrderID]     " +
								 ",[COIItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],ItemVerKitText  FROM [dbo].[vwRefurbishedConfirmationIt] WHERE IsActive = {0} AND CMSOId = {1} ORDER BY ItemVerKit,ItemOrKitID", 1, ViewState["vwCMRSentID"]);
				myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.Columns[8].Visible = true;
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView; this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;
				this.gvConfirmationItems.Columns[8].Visible = false;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "<br />proS = " +	proS);
			}
			try
			{
				proS = string.Format("SELECT cDp.[CompanyName] [CustomerName] ,cDp.[StreetName] [CustomerAddress1] ,cDp.[StreetHouseNumber] [CustomerAddress2] ,cDp.[StreetOrientationNumber] [CustomerAddress3] ,cDp.[City] [CustomerCity] ,cDp.[ZipCode] [CustomerZipCode]" +
									 ",vw.[DateOfDelivery] [RequiredDateOfShipment], cDp.IsActive, vw.RefurbishedOrderID FROM [dbo].[vwRefurbishedConfirmationHd]   vw INNER JOIN [dbo].[cdlDestinationPlaces] cDp   ON vw.[CustomerID]=cDp.ID WHERE vw.ID = {0}", grdData.SelectedValue);
				
				myDT = BC.GetDataTable(proS);
				this.lblCustomerNameValue.Text = myDT.Rows[0]["CustomerName"].ToString();
				this.lblCustomerCityValue.Text = myDT.Rows[0]["CustomerCity"].ToString();
				this.lblCustomerZipCodeValue.Text = myDT.Rows[0]["CustomerZipCode"].ToString();
				this.lblRequiredDateOfShipmentValue.Text = wConvertStringToDatedd_mm_yyyy(myDT.Rows[0]["RequiredDateOfShipment"].ToString());
				this.lblCustomerAddress1Value.Text = myDT.Rows[0]["CustomerAddress1"].ToString();
				this.lblCustomerAddress2Value.Text = myDT.Rows[0]["CustomerAddress2"].ToString();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}
			try
			{
				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered]" +
									 ",[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId]" +
									 ",[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  FROM [dbo].[vwCMRF0SentIt] WHERE [IsActive] = {0} AND [CMSOId] = {1} ORDER BY ItemVerKit,ItemOrKitID", 1, myDT.Rows[0]["RefurbishedOrderID"]);// RefurbishedOrderID

				myD = BC.GetDataTable(proS);
				this.gvItems.DataSource = myD.DefaultView; this.gvItems.DataBind();
				this.gvItems.Visible = true;
				this.gvItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}
			//******************************
			String s = HttpUtility.HtmlDecode(drv.Cells[8].Text);
			if (s == "SCHVÁLENO" || s == "ZAMÍTNUTO" || s == "D0 ODESLÁNA") this.btnDecision.Enabled = false; else this.btnDecision.Enabled = true;

			this.checkReceptionOrderConfirmation();
		}

		protected void gvConfirmationItems_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		protected void btnDecision_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				bool xmOK = false;
				Int16 iDecision;
				try
				{
					iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue);
					GridViewRow selectedRow = grdData.SelectedRow;					//2015-11-04
					if (grdData.SelectedValue != null && selectedRow != null)
					{

						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.StoredProcedure;
						sqlComm.CommandText = "[dbo].[prCMRCconsentRF]";
						sqlComm.Connection = conn;
						sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
						sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());

						sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);			//2015-11-04						
						sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 13;												//2015-11-04
						sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "RefurbishedConfirmation";		//2015-11-04

						sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());

						sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
						sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
						sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 300);
						sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

						try
						{
							conn.Open();
							sqlComm.ExecuteNonQuery();
							if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
							{
								xmOK = true;
							}
						}
						catch (Exception ex)
						{
							BC.ProcessException(ex, AppLog.GetMethodName(), "program prCMRCconsentRF");
						}
						finally
						{
							//if (xmOK == false) trans.Rollback();         23.3.2012
							conn.Close();
							conn = null;
							sqlComm = null;
						}
					}
					else
					{  // upozornit
					}
				}
				catch (Exception)
				{
					iDecision = -1;
					// musíte vybrat
				}
				if (xmOK)
				{
					this.grdData.SelectedIndex = -1;
					this.pnlItems.Visible = false;
					this.gvConfirmationItems.SelectedIndex = -1;
					Session["IsRefresh"] = "1";
					//this.pnlDetails.Visible = false;
					btnBack_Click(btnBack, EventArgs.Empty);
				}
			}

		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.gvConfirmationItems.SelectedIndex = -1;
			BC.UnbindDataFromObject(this.gvConfirmationItems);
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSnExcel_Click(object sender, EventArgs e)
		{
			ExcelConfClicked(WConvertStringToInt32(ViewState["vwCMRSentID"].ToString()));
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			//if (e.CommandName == "OrderView")
			//{
			//	int id = WConvertStringToInt32(e.CommandArgument.ToString());
			//	ViewState["vwCMRSentID"] = id.ToString();
			//	//OrderView(id);
			//	ExcelConfClicked(id);
			//}

			if (e.CommandName == "btSingleExcel")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				ExcelConfClicked(id);
			}

			if (e.CommandName == "btnExcelConfClicked")
			{
				ExcelConfClicked(0);
			}

			if (e.CommandName == "btnOznacit")
			{
				foreach (GridViewRow r in grdData.Rows)
				{
					CheckBox ckb = (CheckBox)r.FindControl("chkbExcel");
					ckb.Checked = !ckb.Checked;
				}
			}
		}

		protected void OrderView(int id)
		{
		}

		/// <summary>
		/// Export vybraného/vybraných potvrzení závozu do Excelu
		/// </summary>
		/// <param name="id"></param>
		protected void ExcelConfClicked(int id)
		{	
			string sqlSelect = this.createSqlSelectForExportToExcel(id);

			if (sqlSelect.IsNotNullOrEmpty())
			{
				DataTable myDt = BC.CreateDataTable(sqlSelect); /*BC.GetDataTable(sqlSelect);*/
				if (myDt != null && myDt.Rows.Count > 0)
				{
					// **
					MemoryStream ms = new MemoryStream();
					using (ExcelPackage xls = new ExcelPackage(ms))
					{
						ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
						worksheet.Cells["P1:Q50000"].Style.Numberformat.Format = @"@";
						try
						{
							int radek = 1;
							// nadpis
							worksheet.Row(1).Height = 24;
							worksheet.Cells[radek, 1, radek, 18].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("RF1 - Výpis");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka 
							worksheet.Cells[radek, 1].Value = String.Format("ID");
							worksheet.Cells[radek, 2].Value = String.Format("Message ID");
							worksheet.Cells[radek, 3].Value = String.Format("Message Type ID");
							worksheet.Cells[radek, 4].Value = String.Format("Message Popis");
							worksheet.Cells[radek, 5].Value = String.Format("MessageDateOfReceipt");
							worksheet.Cells[radek, 6].Value = String.Format("Id objednávky");
							worksheet.Cells[radek, 7].Value = String.Format("Vyjádření");
							worksheet.Cells[radek, 8].Value = String.Format("MessageDateOfShipment");
							worksheet.Cells[radek, 9].Value = String.Format("SingleOrMaster");
							worksheet.Cells[radek, 10].Value = String.Format("ItemVerKit");
							worksheet.Cells[radek, 11].Value = String.Format("Popis");
							worksheet.Cells[radek, 12].Value = String.Format("MeJe");
							worksheet.Cells[radek, 13].Value = String.Format("Kvalita");
							worksheet.Cells[radek, 14].Value = String.Format("IncotermDescription");
							worksheet.Cells[radek, 15].Value = String.Format("Množství");
							worksheet.Cells[radek, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
							worksheet.Cells[radek, 16].Value = String.Format("SN1");
							worksheet.Cells[radek, 17].Value = String.Format("SN2");
							
							radek += 1;
							foreach (DataRow dr in myDt.Rows)
							{
								worksheet.Cells[radek, 1].Value = dr["ID"].ToString();
								worksheet.Cells[radek, 2].Value = dr["MessageID"].ToString();
								worksheet.Cells[radek, 3].Value = dr["MessageTypeID"].ToString();
								worksheet.Cells[radek, 4].Value = dr["MessageDescription"].ToString();
								worksheet.Cells[radek, 5].Value = dr["DateOfShipment"].ToString(); // 
								worksheet.Cells[radek, 6].Value = dr["RefurbishedOrderID"].ToString();
								worksheet.Cells[radek, 7].Value = dr["ReconciliationYesNo"].ToString();
								worksheet.Cells[radek, 8].Value = dr["MessageDateOfShipment"].ToString();
								worksheet.Cells[radek, 9].Value = String.Format("");
								worksheet.Cells[radek, 10].Value = dr["ItemVerKitText"].ToString();
								worksheet.Cells[radek, 11].Value = dr["ItemOrKitDescription"].ToString();
								worksheet.Cells[radek, 12].Value = dr["ItemOrKitUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 13].Value = dr["ItemOrKitQualityCode"].ToString();
								worksheet.Cells[radek, 14].Value = dr["IncotermDescription"].ToString();
								worksheet.Cells[radek, 15].Value = dr["ItemOrKitQuantityInt"].ToString();
								worksheet.Cells[radek, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 16].Value = dr["SN1"].ToString();
								worksheet.Cells[radek, 17].Value = dr["SN2"].ToString();
								radek += 1;
							}

							worksheet.Column(1).AutoFit();
							worksheet.Column(2).AutoFit();
							worksheet.Column(3).AutoFit();
							worksheet.Column(4).AutoFit();
							worksheet.Column(5).AutoFit();
							worksheet.Column(6).AutoFit();
							worksheet.Column(7).AutoFit();
							worksheet.Column(9).AutoFit();
							worksheet.Column(10).AutoFit();
							worksheet.Column(11).AutoFit();
							worksheet.Column(12).AutoFit();
							worksheet.Column(13).AutoFit();
							worksheet.Column(14).AutoFit();
							worksheet.Column(15).AutoFit();
							worksheet.Column(16).AutoFit();
							worksheet.Column(17).AutoFit();
														
							xls.Workbook.Properties.Title = "RO objednávka";
							xls.Workbook.Properties.Subject = "Sériová čísla";
							xls.Workbook.Properties.Keywords = "Office Open XML";
							xls.Workbook.Properties.Category = "Sériová čísla";
							xls.Workbook.Properties.Comments = "";							
							xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";
														
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

						}
					}
					// **
				}
			}
		}

		/// <summary>
		/// Vytvoří select pro export zvolených potvrzení závozu
		/// (exportuje se 1 nebo více potvrzení závozu najednou)
		/// </summary>
		/// <param name="id">0 .. hromadný export vybraných závozů,  > 0 export právě 1 potvrzení závozu</param>
		/// <returns></returns>
		private string createSqlSelectForExportToExcel(int id)
		{
			string proS = String.Empty;
			string proW = String.Empty;

			if (id != 0)
			{
				// export právě 1 potvrzení závozu
				proS = "SELECT RefurbishedOrderID FROM [dbo].[CommunicationMessagesRefurbishedOrderConfirmation] " +
					   "WHERE id = " + id;

				DataTable myD = BC.GetDataTable(proS);

				//proW = " WHERE RefurbishedOrderID = " + myD.Rows[0][0] + " AND It.ISACTIVE = 1 AND [Reconciliation] <> 2";
				proW = " WHERE RefurbishedOrderID = " + myD.Rows[0][0] + " AND It.ISACTIVE = 1 AND [Reconciliation] IN (0, 1)";		//2015-10-21
			}
			else
			{
				// hromadný export (exportuje se 1 nebo více potvrzení závozu najednou) 
				string orderID = String.Empty;
				CheckBox chkb = new CheckBox();
				StringBuilder sb = new StringBuilder();

				foreach (GridViewRow drv in grdData.Rows)
				{
					orderID = drv.Cells[7].Text;
					chkb = (CheckBox)drv.FindControl("chkbExcel");
					if (chkb.Checked)
					{
						sb.Append(orderID + ",");
					}
				}

				string listOrderID = sb.ToString();
				if (listOrderID.IsNullOrEmpty())
				{
					return String.Empty;
				}

				//proW = " WHERE RefurbishedOrderID IN (" + listOrderID.Substring(0, listOrderID.Length - 1) + ") AND It.ISACTIVE = 1  AND [Reconciliation] <> 2";
				proW = " WHERE RefurbishedOrderID IN (" + listOrderID.Substring(0, listOrderID.Length - 1) + ") AND It.ISACTIVE = 1  AND [Reconciliation] IN (0, 1)";	//2015-10-21
			}

			proS = " SELECT  DISTINCT It.[ItID],It.[CMSOId],It.[ItemVerKit],It.[ItemOrKitID],It.[ItemOrKitDescription],It.[ItemOrKitQuantity] " +
				   " ,It.[ItemOrKitQuantityInt],It.[ItemOrKitUnitOfMeasureId],It.[ItemOrKitUnitOfMeasure],It.[ItemOrKitQualityId] " +
				   " ,It.[ItemOrKitQualityCode],It.[IncotermsId],It.[IncotermDescription],It.[NDReceipt],It.[KitSNs],It.[IsActive]" +
				   " ,It.[ModifyDate],It.[ModifyUserId],It.[COIItemOrKitQuantityInt],It.[ItemOrKitQuantityDeliveredInt],It.[ID] " +
				   " ,It.[MessageId],It.[MessageTypeId],It.[MessageDescription],It.[DateOfShipment],It.[RefurbishedOrderID],It.[CustomerID] " +
				   " ,It.[Reconciliation],It.[ReconciliationYesNo],It.[MessageDateOfShipment],It.[DateOfDelivery],It.[CompanyName],It.[City]" +
				   " ,SNS.SN1, SNS.SN2, ItemVerKitText" +
				   " FROM [dbo].[vwRefurbishedConfirmationIt] It " +
				   " LEFT OUTER JOIN [dbo].[CommunicationMessagesRefurbishedOrderConfirmationSerNumSent]SNS ON It.ItID = SNS.RefurbishedItemsOrKitsID "
				   + proW +
				   " ORDER BY It.[ItID] DESC, It.[ItemVerKit] ASC, It.[ItemOrKitID] ASC";

			return proS;
		}
		
		/// <summary>
		/// Vyhodnocení RF1		
		/// modrá barva   - k dané RF0 už existuje alespoň 1 RF1
		/// červená barva - nesouhlasí počty kusů objednaných a dodaných; resp. nesouhlasí počty SN (CPE x uloženo v db)
		/// </summary>
		private void checkReceptionOrderConfirmation()
		{
			foreach (GridViewRow gridViewRow in this.grdData.Rows)
			{
				if (gridViewRow.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION)
				{
					string receptionConfirmationID = gridViewRow.Cells[COLUMN_REFURBISHED_CONFIRMATION_ID].Text;
					if (RefurbishedConfirmationHelper.GetRefurbishedOrderConfirmationCount(receptionConfirmationID) > 1)
					{
						// k RF0 již existuje alespoň jedna RF1
						gridViewRow.BackColor = BC.BlueColor;
					}
					else
					{
						if ((RefurbishedConfirmationHelper.CheckRefurbishedOrderConfirmation(receptionConfirmationID) == false) || 
							(RefurbishedConfirmationHelper.CheckRefurbishedConfirmationSNsCounts(receptionConfirmationID) == false))
						{
							// nesouhlasí počty kusů objednaných a dodaných; resp. nesouhlasí počty SN (CPE x uloženo v db)
							gridViewRow.BackColor = BC.RedColor;
						}
					}
				}
			}
		}	
	}
}