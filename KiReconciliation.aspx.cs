using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using Fenix.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// K1 - Schválení kittingu (schválení příjmu kitů)
	/// </summary>
	public partial class KiReconciliation : BasePage
	{
		#region Properties

		/// <summary>
		/// Bez rozhodnutí
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		/// <summary>
		/// Schváleno
		/// <value> = "1"</value>
		/// </summary>
		private const string YES_DECISION = "1";

		/// <summary>
		/// Číslo sloupce pro Refurbished Confirmation ID
		/// </summary>
		private const int COLUMN_KITTING_CONFIRMATION_ID = 1;

		/// <summary>
		/// Číslo sloupce pro schválení {druhy schválení -> 0 .. bez rozhodnutí  1 .. schváleno    2 .. zamítnuto	3 .. D0 odeslána}
		/// </summary>
		private const int COLUMN_RECONCILIATION = 13;

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
				BaseHelper.FillDdlKitCode(ref this.ddlModelCPE);
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
			ViewState["Filter"] = " 1=1 ";
			if (this.ddlDecisionFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue.ToString();
			if (this.ddlModelCPE.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [ModelCPE] like '" + this.ddlModelCPE.SelectedItem.Text.Trim() + "%'";
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwKitConfirmationHd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = " [ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [KitOrderID], [Reconciliation], " +
								   " [MessageDateOfShipment], [KitDateOfDelivery], [HeliosObj], [ModelCPE], [OrderedKitQuantity], [DeliveredKitQuantity], [ReconciliationText]";
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();				
				pagerData.FillObject(ref grdData, COLUMN_RECONCILIATION);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.setObjectsForExportToExcel();
				this.checkKittingOrderConfirmation();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		/// <summary>
		/// Nastavení objektů grdData souvisejících s exportem do Excelu
		/// (exportovat lze pouze schválenou/é K1)
		/// </summary>
		private void setObjectsForExportToExcel()
		{			
			foreach (GridViewRow gvr in grdData.Rows)
			{
				try
				{
					CheckBox ckb = (CheckBox)gvr.FindControl("chkbExcel");
					ImageButton ib = (ImageButton)gvr.FindControl("btnSerNumView");

					if (gvr.Cells[COLUMN_RECONCILIATION].Text == YES_DECISION)
					{
						ckb.Visible = true;
						ib.ImageUrl = ResolveUrl("~/img/excelSmall.png");
						ib.Enabled = true;
					}
					else
					{
						ckb.Visible = false;
						ib.ImageUrl = ResolveUrl("~/img/excelSmallGrey.png");
						ib.Enabled = false;
					}
				}
				catch(Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());					
				}
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlItems.Visible = true;
			this.gvConfirmationItems.SelectedIndex = -1;
			this.pnlDetails.Visible = false;
			BC.UnbindDataFromObject<GridView>(this.gvConfirmationItems, this.gvCardStockItems, this.gvCardStockItems2);
									
			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID],[CMSOId],[HeliosOrderID] ,[HeliosOrderRecordId] ,[KitId] ,[KitDescription],[DescriptionCz],[KitQuantity],[CMRSIItemQuantity]"+
									 ",[KitQuantityInt],[CMRSIItemQuantityInt],[KitUnitOfMeasure]  ,[KitQualityId] ,[IsActive] ,[ModifyDate],[ModifyUserId],[CommunicationMessagesSentId],[Code]" +
									 " FROM [dbo].[vwKitConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, grdData.SelectedValue);
				 
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView; 
				this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;
			}
			catch
			{
			}

			if (this.ddlDecisionFlt.SelectedValue.ToString() == WITHOUT_DECISION) 
			{ 
				this.btnDecision.Enabled = true; 
				this.rdblDecision.Enabled = true; 
			} 
			else 
			{ 
				this.btnDecision.Enabled = false; 
				this.rdblDecision.Enabled = false; 
			}

			this.checkKittingOrderConfirmation();
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "btSingleExcel")
			{
				ExcelKittingReconciliationSummary(e.CommandArgument.ToString());
			}

			if (e.CommandName == "btnExcelConfClicked")
			{
				ExcelKittingReconciliationSummary("0");
			}

			if (e.CommandName == "btnOznacit")
			{
				foreach (GridViewRow r in grdData.Rows)
				{
					CheckBox ckb = (CheckBox)r.FindControl("chkbExcel");
					if (ckb.Visible)
					{
						ckb.Checked = !ckb.Checked;
					}
				}
			}
		}

		protected void gvConfirmationItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlDetails.Visible = true;
			BC.UnbindDataFromObject<GridView>(this.gvCardStockItems, this.gvCardStockItems2);
			
			GridViewRow gvr = gvConfirmationItems.SelectedRow;

			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [cdlKitsItemsID] ,[cdlKitsID],CASE [ItemVerKit] WHEN 1 THEN 'Kit' ELSE 'Item' END ItemVerKit,[ItemOrKitID],[ItemCode],[DescriptionCzKit],[DescriptionCzItemsOrKit],[ItemOrKitQuantity],[ItemOrKitQuantityInt] ,[PackageType]" +
						  " FROM [dbo].[vwKitsIt] WHERE [IsActive] = {0} AND cdlKitsID = {1}", 1, gvr.Cells[2].Text);
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.gvKiItems.DataSource = myDataTable.DefaultView; this.gvKiItems.DataBind();
				this.gvKiItems.Visible = true;
				this.gvKiItems.SelectedIndex = -1;
			}
			catch
			{
			}

			try
			{
				proS = string.Format("SELECT ID,[cdlStocksName],[ItemVerKitDescription],[Code] ,[DescriptionCz],[ItemOrKitQuantity]"+
					",[ItemOrKitFree],[ItemOrKitUnConsilliation] ,[ItemOrKitReserved] ,[ItemOrKitReleasedForExpedition]" +
					", ItemOrKitQuantityInt,[ItemOrKitFreeInt],[ItemOrKitUnConsilliationInt] ,[ItemOrKitReservedInt] ,[ItemOrKitReleasedForExpeditionInt]" +
					" ,MeasuresCode,KitQualitiesCode" +
						  " FROM [dbo].[vwCardStockItemsK] WHERE [IsActive] = {0} AND ItemOrKitId = {1}", 1, gvr.Cells[2].Text);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvCardStockItems.DataSource = myDataTable.DefaultView; this.gvCardStockItems.DataBind();
			}
			catch
			{
			}
			try
			{
				proS = string.Format("SELECT CardStockItemsID ,[ItemOrKitId] ,DescriptionCz ,[MeasureCode] ,[ItemOrKitQuantity],ItemVerKitDescription, Name" +
									 ",[ItemOrKitQuality]  ,[ItemOrKitFree]  ,[ItemOrKitUnConsilliation] ,[ItemOrKitReserved] ,[ItemOrKitReleasedForExpedition], GroupGoods,Code " +
				                     ",ItemOrKitQuantityInt,[ItemOrKitFreeInt],[ItemOrKitUnConsilliationInt] ,[ItemOrKitReservedInt] ,[ItemOrKitReleasedForExpeditionInt]" +
									 " FROM [dbo].[vwKitCardStokItems] WHERE [StockId] = {0} AND [cdlKitsID] = {1}", 2, gvr.Cells[2].Text);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvCardStockItems2.DataSource = myDataTable.DefaultView; this.gvCardStockItems2.DataBind();
			}
			catch
			{
			}

		}

		protected void btnDecision_Click(object sender, EventArgs e)
		{
			bool xmOK = false;
			Int16 iDecision;
			try
			{
				iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue.ToString());
				GridViewRow selectedRow = grdData.SelectedRow;								//2015-11-04
				if (grdData.SelectedValue != null && selectedRow != null)
				{
					SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
					SqlCommand sqlComm = new SqlCommand();
					sqlComm.CommandType = CommandType.StoredProcedure;
					sqlComm.CommandText = "[dbo].[prCMRCconsentK]";
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
					sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());

					sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);			//2015-11-04						
					sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 4;												//2015-11-04
					sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "KittingConfirmation";			//2015-11-04

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
					catch (Exception)
					{
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
				this.pnlDetails.Visible = false;

				this.fillPagerData(BC.PAGER_FIRST_PAGE);
			}
			//}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.gvConfirmationItems.SelectedIndex = -1;
			this.pnlDetails.Visible = false;

			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		/// <summary>
		/// Export sériových čísel z detailu zvoleného potvrzení kittingu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSnExcel_Click(object sender, EventArgs e)
		{
			try
			{
				bool mOK = true;
				string proS = string.Format("SELECT  [ID],[CMSOId],[KitID],[KitDescription],[KitQuantity],[KitSNs] " +
				 " FROM [dbo].[CommunicationMessagesKittingsConfirmationItems]" +
				 " WHERE CMSOId = {0} ORDER BY 1,2", grdData.SelectedValue);
				
				DataTable dt = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dt == null || dt.Rows.Count < 1) { mOK = false; }
				if (mOK)
				{
					string sns = string.Empty;
					foreach (DataRow dr in dt.Rows)
					{
						if (!string.IsNullOrWhiteSpace(dr["KitSNs"].ToString()))
						{
							if (!string.IsNullOrEmpty(sns)) sns += ";";
							sns += dr["KitSNs"].ToString();
						}
					}

					if (!string.IsNullOrWhiteSpace(sns))
					{
						string[] dvojice = sns.Split(';');
						
						MemoryStream ms = new MemoryStream();
						using (ExcelPackage xls = new ExcelPackage(ms))
						{
							ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");

							try
							{
								int radek = 3;
								foreach (var par in dvojice)
								{
									string[] sn = par.Split(',');
									string adresaA = String.Format("A{0}", radek);
									string adresaB = String.Format("B{0}", radek);
									worksheet.Cells[adresaA].Value = sn[0].IsNotNullOrEmpty() ? sn[0].Trim() : String.Empty;
									worksheet.Cells[adresaB].Value = sn[1].IsNotNullOrEmpty() ? sn[1].Trim() : String.Empty;
									radek++;
								}

								worksheet.Cells["A1:B10000"].Style.Numberformat.Format = @"@";
								worksheet.Column(1).AutoFit();
								worksheet.Column(2).AutoFit();
								worksheet.Column(3).AutoFit();

								worksheet.Cells["A1"].Value = "Sériová čísla";
								worksheet.Cells["A1"].Style.Font.Bold = true;
								worksheet.Cells["A1"].Style.Font.UnderLine = true;

								worksheet.Cells["A2"].Value = "SN1";
								worksheet.Cells["B2"].Value = "SN2";
								
								xls.Workbook.Properties.Title = "Sériová čísla";
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
								throw;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				// TODO
				throw;
			}
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		/// <summary>
		/// Export vybraných kitting confirmation/s do Excelu
		/// </summary>
		/// <param name="rowID"></param>
		protected void ExcelKittingReconciliationSummary(string rowID)
		{
			string kittingConfirmationIDList = this.getSelectedKittingConfirmationIDList(rowID);

			if (kittingConfirmationIDList.IsNotNullOrEmpty())
			{
				string proS = String.Format("SELECT KCHd.[ID], KCHd.[KitOrderID], cKits.[ID] AS KitID, KCHd.[ModelCPE], KCHd.[MessageDateOfReceipt] AS SkutDatKompletace, " +
											"	KCHd.[MessageDateOfShipment] AS DatOdeslMessage, KCHd.[KitDateOfDelivery] as PozDatKompletace,  CMRCI.KitSNs " +											
											"FROM [dbo].[vwKitConfirmationHd]										KCHd " +
											"LEFT OUTER JOIN [dbo].[CommunicationMessagesKittingsConfirmationItems]	CMRCI " +
												"ON KCHd.ID = CMRCI.CMSOId " +
											"LEFT OUTER JOIN [dbo].[cdlKits]										cKits " +
												"ON CMRCI.KitID = cKits.ID " +
											"WHERE KCHd.[ID] IN ({0}) " +
											"ORDER BY KCHd.[ID] DESC", kittingConfirmationIDList);

				DataTable myDt = BC.GetDataTable(proS);
				if (myDt != null && myDt.Rows.Count > 0)
				{
					// **
					MemoryStream ms = new MemoryStream();
					using (ExcelPackage xls = new ExcelPackage(ms))
					{
						ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
						worksheet.Cells["A1:J2"].Style.Numberformat.Format = @"@";
						try
						{
							int radek = 1;
							// nadpis
							worksheet.Row(1).Height = 24;
							worksheet.Cells[radek, 1, radek, 7].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("K1 - Výpis");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka 
							worksheet.Cells[radek, 1].Value = String.Format("Kit ID");
							worksheet.Cells[radek, 2].Value = String.Format("Model CPE");
							worksheet.Cells[radek, 3].Value = String.Format("Sk. datum kompletace");
							worksheet.Cells[radek, 4].Value = String.Format("Datum odeslání message");							
							worksheet.Cells[radek, 5].Value = String.Format("Pož. datum kompletace");
							worksheet.Cells[radek, 6].Value = String.Format("SN1");
							worksheet.Cells[radek, 7].Value = String.Format("SN2");
							worksheet.Cells[radek, 1, radek, 7].Style.Font.Bold = true;

							radek += 1;
							foreach (DataRow dr in myDt.Rows)
							{
								string kitID = dr["KitID"].ToString();
								string modelCPE = dr["ModelCPE"].ToString();
								string skutDatKompletace = ((DateTime)dr["SkutDatKompletace"]).ToString("dd.MM.yyyy");
								string datOdeslMessage = ((DateTime)dr["DatOdeslMessage"]).ToString("dd.MM.yyyy");
								string pozDatKompletace = ((DateTime)dr["PozDatKompletace"]).ToString("dd.MM.yyyy");

								this.setCommonExcelKitingReconciliationColumns(worksheet, radek, kitID, modelCPE, skutDatKompletace, datOdeslMessage, pozDatKompletace);

								string sns = dr["KitSNs"].ToString();
								if (sns.IsNotNullOrEmpty())
								{
									string[] dvojice = sns.Split(';');
									for (int i = 0; i < dvojice.Length; i++)
									{
										string[] sn = dvojice[i].Split(',');
										string adresaA = String.Format("F{0}", radek);
										string adresaB = String.Format("G{0}", radek);
										worksheet.Cells[adresaA].Value = sn[0].IsNotNullOrEmpty() ? sn[0].Trim() : String.Empty;
										worksheet.Cells[adresaA].Style.Numberformat.Format = @"@";
										worksheet.Cells[adresaB].Value = sn[1].IsNotNullOrEmpty() ? sn[1].Trim() : String.Empty;
										worksheet.Cells[adresaB].Style.Numberformat.Format = @"@";
										if (i < dvojice.Length - 1)
										{											
											radek++;
											this.setCommonExcelKitingReconciliationColumns(worksheet, radek, kitID, modelCPE, skutDatKompletace, datOdeslMessage, pozDatKompletace);
										}										
									}
								}
								radek++;
							}
																					
							worksheet.Column(1).AutoFit();
							worksheet.Column(2).AutoFit();
							worksheet.Column(3).AutoFit();
							worksheet.Column(4).AutoFit();
							worksheet.Column(5).AutoFit();
							worksheet.Column(6).AutoFit();
							worksheet.Column(7).AutoFit();
														
							xls.Workbook.Properties.Title = "K1 přehled";
							xls.Workbook.Properties.Subject = "K1 přehled";
							xls.Workbook.Properties.Keywords = "Office Open XML";
							xls.Workbook.Properties.Category = "K1 přehled";
							xls.Workbook.Properties.Comments = "";							
							xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";
														
							xls.Save();
							ms.Flush();
							ms.Seek(0, SeekOrigin.Begin);

							Response.Clear();
							Response.Buffer = true;
							Response.AddHeader("content-disposition", "attachment;filename=K1_prehled_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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

		private void setCommonExcelKitingReconciliationColumns(ExcelWorksheet worksheet, int radek, string kitID, string modelCPE, string skutDatKompletace, string datOdeslMessage, string pozDatKompletace)
		{
			worksheet.Cells[radek, 1].Value = kitID;
			worksheet.Cells[radek, 1].Style.Numberformat.Format = @"@";
			worksheet.Cells[radek, 2].Value = modelCPE;
			worksheet.Cells[radek, 3].Value = skutDatKompletace;
			worksheet.Cells[radek, 3].Style.Numberformat.Format = "dd.mm.yyyy";
			worksheet.Cells[radek, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			worksheet.Cells[radek, 4].Value = datOdeslMessage;
			worksheet.Cells[radek, 4].Style.Numberformat.Format = "dd.mm.yyyy";
			worksheet.Cells[radek, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			worksheet.Cells[radek, 5].Value = pozDatKompletace;
			worksheet.Cells[radek, 5].Style.Numberformat.Format = "dd.mm.yyyy";
			worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
		}

		/// <summary>
		/// Vytvoří seznam kitting confirmation ID, které se mají exportovat do Excelu
		/// </summary>
		/// <param name="rowID"></param>
		/// <returns>seznam vybraných čísel potvrzení kittingu (kitting confirmation ID) oddělený čárkou</returns>
		private string getSelectedKittingConfirmationIDList(string rowID)
		{
			string shipmentOrderIDs = String.Empty;
			CheckBox chkb = new CheckBox();
			StringBuilder sb = new StringBuilder();

			if (rowID == "0")
			{
				// hromadný export (exportuje se 1 nebo více potvrzení kittingu najednou) 
				foreach (GridViewRow drv in grdData.Rows)
				{
					shipmentOrderIDs = drv.Cells[1].Text;
					chkb = (CheckBox)drv.FindControl("chkbExcel");
					if (chkb.Checked)
					{
						sb.Append(shipmentOrderIDs + ",");
					}
				}
			}
			else
			{
				// export právě 1 potvrzení kittingu
				sb.Append(rowID + ",");
			}

			return sb.ToString().IsNotNullOrEmpty() ? sb.ToString().Substring(0, (sb.ToString().Length) - 1) : String.Empty;
		}

		/// <summary>
		/// Vyhodnocení K1
		/// modrá barva   - k dané K0 už existuje alespoň 1 K1
		/// červená barva - nesouhlasí počty kusů objednaných a dodaných; resp. nesouhlasí počty SN (CPE x uloženo v db)
		/// </summary>
		private void checkKittingOrderConfirmation()
		{
			foreach (GridViewRow gridViewRow in this.grdData.Rows)
			{
				if (gridViewRow.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION)
				{
					string kittingConfirmationID = gridViewRow.Cells[COLUMN_KITTING_CONFIRMATION_ID].Text;
					if (KittingReconciliationHelper.GetKittingOrderConfirmationCount(kittingConfirmationID) > 1)
					{
						// ke K0 již existuje alespoň jedna K1
						gridViewRow.BackColor = BC.BlueColor;
					}
					else
					{
						if ((KittingReconciliationHelper.CheckKittingOrderConfirmation(kittingConfirmationID) == false) || 
							(KittingReconciliationHelper.CheckKittingConfirmationSNsCounts(kittingConfirmationID) == false))
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