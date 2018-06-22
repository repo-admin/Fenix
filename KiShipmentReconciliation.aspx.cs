using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using Fenix.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// S1 - Schválení expedice (závozu)	
	/// </summary>
	public partial class KiShipmentReconciliation : BasePage
	{
		#region Properties

		/// <summary>
		/// Bez rozhodnutí
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		/// <summary>
		/// Objednávka CPE
		/// </summary>
		private const string ORDER_CPE = "1";

		/// <summary>
		/// Objednávka materiálu
		/// </summary>
		private const string ORDER_MATERIAL = "2";

		/// <summary>
		/// Číslo sloupce pro Shipmet Order ID
		/// </summary>
		private const int COLUMN_SHIPMENT_ORDER_ID = 9;

		/// <summary>
		/// Číslo sloupce pro typ objednávky {1 .. objednávka CPE    2 .. objednávka materiálu}
		/// </summary>		
		private const int COLUMN_ORDER_TYPE_ID = 13;

		/// <summary>
		/// Číslo sloupce pro schválení {druhy schválení -> 0 .. bez rozhodnutí  1 .. schváleno    2 .. zamítnuto	3 .. D0 odeslána}
		/// </summary>
		private const int COLUMN_RECONCILIATION = 14;

		/// <summary>
		/// seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		/// </summary>
		private readonly int[] hideGrdDataColumns = new int[] { COLUMN_ORDER_TYPE_ID, COLUMN_RECONCILIATION };

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
				BaseHelper.FillDdlOrderType(ref this.ddlOrderType);
				BaseHelper.FillDdlUserModify(ref this.ddlUsersModifyFlt);
				BaseHelper.FillDdlDecision(ref this.ddlDecisionFlt);
				ShipmentReconciliationHelper.FillDdlCompanyName(ref this.ddlCompanyName);
				ShipmentReconciliationHelper.FillDdlCity(ref this.ddlCityName);
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
			ViewState["Filter"] = this.createWhereClause();
			this.lblErrInfo.Text = "";

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwShipmentConfirmationHd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "*";
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

				this.checkShipmentOrderConfirmation();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblErrInfo.Text = "";
			GridViewRow drv = this.grdData.SelectedRow;
			Session["IsRefresh"] = "0";
			this.pnlItems.Visible = true;
			this.gvConfirmationItems.SelectedIndex = -1;
			BC.UnbindDataFromObject(this.gvConfirmationItems);

			DataTable myDT = new DataTable();
			DataTable myDataTable = new DataTable();
			DataTable myD = new DataTable();
			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID] ,[CMSOId],[SingleOrMaster],[HeliosOrderRecordID],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[CMRSIItemQuantity],[ItemOrKitUnitOfMeasureId]" +
									 ",[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId],[ItemOrKitQualityCode],[IncotermsId],[IncotermDescription],[RealDateOfDelivery],[RealItemOrKitQuantity]" +
									 ",[RealItemOrKitQualityID],[RealItemOrKitQuality],[Status],[KitSNs],[IsActive],[ModifyDate],[ModifyUserId],[Code],[CommunicationMessagesSentId]" +
									 ",[ItemOrKitQuantityReal],[CardStockItemsId],[VydejkyId],[ShipmentOrderSource],RealItemOrKitQuantityInt,CMRSIItemQuantityInt" +
									 ",ItemOrKitQuantityRealInt,CMSOSIItemOrKitQuantity FROM [dbo].[vwShipmentConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, grdData.SelectedValue);

				myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView;
				this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS = " + proS);
			}

			proS = string.Format("SELECT CMSOS.[CustomerName] ,CMSOS.[CustomerAddress1] ,CMSOS.[CustomerAddress2] ,CMSOS.[CustomerAddress3] ,CMSOS.[CustomerCity] ,CMSOS.[CustomerZipCode],CMSOS.[RequiredDateOfShipment], CMSOS.IsActive" +
				   ", vw.ShipmentOrderID FROM [dbo].[vwShipmentConfirmationHd]     vw INNER JOIN [dbo].[CommunicationMessagesShipmentOrdersSent]  CMSOS   ON vw.ShipmentOrderID=CMSOS.ID WHERE vW.ID = {0}", grdData.SelectedValue);

			myDT = BC.GetDataTable(proS);
			this.lblCustomerNameValue.Text = myDT.Rows[0]["CustomerName"].ToString();
			this.lblCustomerCityValue.Text = myDT.Rows[0]["CustomerCity"].ToString();
			this.lblCustomerZipCodeValue.Text = myDT.Rows[0]["CustomerZipCode"].ToString();
			this.lblRequiredDateOfShipmentValue.Text = wConvertStringToDatedd_mm_yyyy(myDT.Rows[0]["RequiredDateOfShipment"].ToString());
			this.lblCustomerAddress1Value.Text = myDT.Rows[0]["CustomerAddress1"].ToString();
			this.lblCustomerAddress2Value.Text = myDT.Rows[0]["CustomerAddress2"].ToString();

			try
			{
				proS = string.Format("SELECT [ID] ,SingleOrMaster " +
					",ItemVerKit,[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitUnitOfMeasure] ,[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt" +
						  "  FROM [dbo].[vwShipmentOrderIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, myDT.Rows[0]["ShipmentOrderID"]);

				myD = BC.GetDataTable(proS);
				this.gvItems.DataSource = myD.DefaultView;
				this.gvItems.DataBind();
				this.gvItems.Visible = true;
				this.gvItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS = " + proS);
			}

			this.btnDecision.Enabled = (drv.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION);

			this.checkShipmentOrderConfirmation();
			//this.setApproveChoice(drv);  na zadost logistiky (T.Gubricka) zruseno 13. 12. 2017
			this.setTxbRemarkReadOnly(drv.Cells[COLUMN_SHIPMENT_ORDER_ID].Text);
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
					GridViewRow selectedRow = grdData.SelectedRow;					//2015-11-09
					if (grdData.SelectedValue != null && selectedRow != null)
					{
						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand
						{
							CommandType = CommandType.StoredProcedure,
							CommandText = "[dbo].[prCMRCconsentS]",
							Connection = conn
						};
						sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
						sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());

						sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);			//2015-11-09						
						sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 7;												//2015-11-09
						sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "ShipmentConfirmation";		//2015-11-09
						
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
					Session["IsRefresh"] = "1";
					//this.pnlDetails.Visible = false;
					this.fillPagerData(BC.PAGER_FIRST_PAGE);
				}
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.gvConfirmationItems.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSnExcel_Click(object sender, EventArgs e)
		{
			try
			{
				this.lblErrInfo.Text = "";
				bool mOK = true;
				string proS = string.Format("SELECT  [ID],[CMSOId],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[KitSNs] " +
				 " FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmationItems]" +
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
									worksheet.Cells[adresaA].Value = BC.ExcelPrepareSerialNumber(sn[0]);
									worksheet.Cells[adresaB].Value = BC.ExcelPrepareSerialNumber(sn[1]);
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
																
								xls.Workbook.Properties.Title = "S1 - Sériová čísla";
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
								Response.AddHeader("content-disposition", "attachment;filename=S1_Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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
					else this.lblErrInfo.Text = "Žádné SN";
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

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "btSingleExcel")
			{
				ExcelConfClicked(e.CommandArgument.ToString());
			}

			if (e.CommandName == "btnExcelConfClicked")
			{
				ExcelConfClicked("0");
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

		/// <summary>
		/// Aktivace filtrace dle popisu kitu/itemu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lnkBtnDoFltByDescr_Click(object sender, EventArgs e)
		{
			base.ClearViewControls(vwEdit);
			this.lblErrorFltByDescr.Text = "";
			BC.UnbindDataFromObject(this.grdData, this.gvKitsOrItemsNew);
			BaseHelper.FillDdlKitDescription(ref this.ddlKitsFltByDescr);
			ShipmentReconciliationHelper.FillDdlItemDescriptionWithoutMaterial(ref this.ddlMaterialFltByDescr);
			this.mvwMain.ActiveViewIndex = 1;
		}

		/// <summary>
		/// Přidání popisu kitu do filtrace dle popisu kitu/itemu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnAddKitIntoFltByDescr_Click(object sender, EventArgs e)
		{
			addRecFltByDescr("KIT");
		}

		/// <summary>
		/// Přidání popisu itemu do filtrace dle popisu kitu/itemu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnAddMaterialIntoFltByDescr_Click(object sender, EventArgs e)
		{
			addRecFltByDescr("MATERIAL");
		}

		/// <summary>
		/// Návrat z filtrace dle popisu kitu/itemu - bez výběru
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnBackFltByDescr_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlKitsFltByDescr.Items.Clear();
			this.ddlMaterialFltByDescr.Items.Clear();
			BC.UnbindDataFromObject(this.gvKitsOrItemsNew);
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		/// <summary>
		/// Návrat z filtrace dle popisu kitu/itemu - s výběrem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSaveFltByDescr_Click(object sender, EventArgs e)
		{
			string error = string.Empty;
			this.lblErrorFltByDescr.Text = string.Empty;

			if (ShipmentHelper.GridViewHasRows(gvKitsOrItemsNew, "CheckBoxR"))
			{
				string listIDforFltByDescr = string.Empty;
				string listDescrForFltByDescr = string.Empty;
				foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
				{
					CheckBox myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					if (myChkb.Checked)
					{
						listIDforFltByDescr += gvr.Cells[2].Text + ",";
						listDescrForFltByDescr += gvr.Cells[4].Text + ", ";
					}
				}
				ViewState["IDforFltByDescr"] = listIDforFltByDescr.Substring(0, listIDforFltByDescr.Length - 1);
				ViewState["DescrforFltByDescr"] = listDescrForFltByDescr.Substring(0, listDescrForFltByDescr.Length - 2);				
				this.btnBackFltByDescr_Click(btnBack, EventArgs.Empty);
				this.tbxDescriptionFlt.Text = ViewState["DescrforFltByDescr"].ToString();
			}
			else
			{
				error += "Nutno zadat alespoň 1 Kit, nebo materiál<br />";
			}

			this.lblErrorFltByDescr.Text = error;
		}

		/// <summary>
		/// Export zvolené objednávky/zvolených objednávek do Excelu 
		/// (2015-08-25 provedeno na zaklade pozadavku D. Vavry)
		/// </summary>
		/// <param name="rowID"></param>
		protected void ExcelConfClicked(string rowID)
		{
			string shipmentOrderIDList = this.getSelectedShipmentOrderIDList(rowID);

			if (shipmentOrderIDList.IsNotNullOrEmpty())
			{
				//string proW = " WHERE ShipmentOrderID in (" + shipmentOrderIDList + ") AND Hd.ISACTIVE=1  AND Hd.[Reconciliation]<>2 AND It.IsActive=1";
				string proW = " WHERE ShipmentOrderID in (" + shipmentOrderIDList + ") AND Hd.ISACTIVE=1  AND It.IsActive=1";	//2015-08-25
				string proS = " SELECT Hd.[ID],Hd.[MessageId],Hd.[MessageTypeId],Hd.[MessageDescription],Hd.[MessageDateOfReceipt],Hd.[ShipmentOrderID],Hd.[Reconciliation],Hd.[ReconciliationYesNo]     " +
							   " ,Hd.[MessageDateOfShipment],Hd.[RequiredDateOfShipment],Hd.[IsActive],Hd.[ModifyDate],Hd.CompanyName                                                                    " +
							   " ,It.[ID]  ItID,It.[SingleOrMaster],It.[HeliosOrderRecordID],It.[ItemVerKit],It.[ItemOrKitID],It.[ItemOrKitDescription],It.[CMRSIItemQuantity],It.[CMRSIItemQuantityInt] " +
							   " ,It.[ItemOrKitUnitOfMeasureId],It.[ItemOrKitUnitOfMeasure],It.[ItemOrKitQualityId],It.[ItemOrKitQualityCode],It.[IncotermsId],It.[IncotermDescription]                  " +
							   " ,It.[RealDateOfDelivery],It.[RealItemOrKitQuantity],It.[RealItemOrKitQuantityInt],It.[RealItemOrKitQualityID],It.[RealItemOrKitQuality],It.[Status]                     " +
							   " ,It.[IsActive],It.[ModifyDate],It.[ModifyUserId],It.[Code],It.[CommunicationMessagesSentId],It.[ItemOrKitQuantityReal],It.[CardStockItemsId]                            " +
							   " ,It.[VydejkyId],[ShipmentOrderSource], [KitSNs], SN1, SN2 " +
							   " FROM [dbo].[vwShipmentConfirmationHd]       Hd " +
							   " INNER JOIN [dbo].[vwShipmentConfirmationIt] It  ON Hd.ID = It.CMSOId " +
							   " LEFT OUTER JOIN  [dbo].[CommunicationMessagesShipmentOrdersConfirmationSerNumSent] SNS ON It.ID = SNS.[ShipmentOrdersItemsOrKitsID]" + proW;

				DataTable myDt = BC.GetDataTable(proS);
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
							worksheet.Cells[radek, 1, radek, 19].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("S1 - Výpis");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka 
							worksheet.Cells[radek, 1].Value = String.Format("ID");
							worksheet.Cells[radek, 2].Value = String.Format("Message ID");
							worksheet.Cells[radek, 3].Value = String.Format("Message Type ID");
							worksheet.Cells[radek, 4].Value = String.Format("Místo určení");
							worksheet.Cells[radek, 5].Value = String.Format("MessageDateOfReceipt");
							worksheet.Cells[radek, 6].Value = String.Format("Id objednávky");
							worksheet.Cells[radek, 7].Value = String.Format("Vyjádření");
							worksheet.Cells[radek, 8].Value = String.Format("MessageDateOfShipment");
							worksheet.Cells[radek, 9].Value = String.Format("SingleOrMaster");
							worksheet.Cells[radek, 10].Value = String.Format("ItemVerKit");
							worksheet.Cells[radek, 11].Value = String.Format("ItemOrKitID");
							worksheet.Cells[radek, 12].Value = String.Format("Popis");
							worksheet.Cells[radek, 13].Value = String.Format("MeJe");
							worksheet.Cells[radek, 14].Value = String.Format("Kvalita");
							worksheet.Cells[radek, 15].Value = String.Format("IncotermDescription");
							worksheet.Cells[radek, 16].Value = String.Format("RealItemOrKitQuantityInt");
							worksheet.Cells[radek, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
							worksheet.Cells[radek, 17].Value = String.Format("SN1");
							worksheet.Cells[radek, 18].Value = String.Format("SN2");

							radek += 1;
							foreach (DataRow dr in myDt.Rows)
							{
								worksheet.Cells[radek, 1].Value = dr["ID"].ToString();
								worksheet.Cells[radek, 2].Value = dr["MessageID"].ToString();
								worksheet.Cells[radek, 3].Value = dr["MessageTypeID"].ToString();
								worksheet.Cells[radek, 4].Value = dr["CompanyName"].ToString();
								//worksheet.Cells[radek, 5].Value = dr["MessageDateOfReceipt"].ToString();
								BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dr["MessageDateOfReceipt"], BC.DATE_TIME_FORMAT_DDMMYYY, ExcelHorizontalAlignment.Center);
								worksheet.Cells[radek, 6].Value = dr["ShipmentOrderID"].ToString();
								worksheet.Cells[radek, 7].Value = dr["ReconciliationYesNo"].ToString();
								//worksheet.Cells[radek, 8].Value = dr["MessageDateOfShipment"].ToString();
								BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dr["MessageDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS);
								worksheet.Cells[radek, 9].Value = dr["SingleOrMaster"].ToString();
								worksheet.Cells[radek, 10].Value = dr["ItemVerKit"].ToString();
								worksheet.Cells[radek, 11].Value = dr["ItemOrKitID"].ToString();
								worksheet.Cells[radek, 12].Value = dr["ItemOrKitDescription"].ToString();
								worksheet.Cells[radek, 13].Value = dr["ItemOrKitUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 14].Value = dr["ItemOrKitQualityCode"].ToString();
								worksheet.Cells[radek, 15].Value = dr["IncotermDescription"].ToString();
								worksheet.Cells[radek, 16].Value = dr["RealItemOrKitQuantityInt"].ToString();
								worksheet.Cells[radek, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 17].Value = BC.ExcelPrepareSerialNumber(dr["SN1"]);
								worksheet.Cells[radek, 18].Value = BC.ExcelPrepareSerialNumber(dr["SN2"]);
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
							worksheet.Column(16).AutoFit();
							worksheet.Column(17).AutoFit();
							worksheet.Column(18).AutoFit();
														
							xls.Workbook.Properties.Title = "S1 - Schválení expedice";
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
							Response.AddHeader("content-disposition", "attachment;filename=S1_Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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
			shipmentOrderIDList = null;
		}

		/// <summary>
		/// Vytvoří seznam shipment order ID, které se mají exportovat do Excelu
		/// </summary>
		/// <param name="rowID"></param>
		/// <returns>seznam vybraných čísel objednávek (shipment order id) oddělený čárkou</returns>
		private string getSelectedShipmentOrderIDList(string rowID)
		{
			string shipmentOrderIDs = String.Empty;
			CheckBox chkb = new CheckBox();
			StringBuilder sb = new StringBuilder();

			if (rowID == "0")
			{
				// hromadný export (exportuje se 1 nebo více objednávek najednou) 
				foreach (GridViewRow drv in grdData.Rows)
				{
					shipmentOrderIDs = drv.Cells[9].Text;
					chkb = (CheckBox)drv.FindControl("chkbExcel");
					if (chkb.Checked)
					{
						sb.Append(shipmentOrderIDs + ",");
					}
				}
			}
			else
			{
				// export právě 1 objednávky
				foreach (GridViewRow drv in grdData.Rows)
				{
					if (drv.Cells[1].Text == rowID)
					{
						shipmentOrderIDs = drv.Cells[9].Text;
						sb.Append(shipmentOrderIDs + ",");
						break;
					}
				}
			}

			return sb.ToString().IsNotNullOrEmpty() ? sb.ToString().Substring(0, (sb.ToString().Length) - 1) : String.Empty;
		}

		/// <summary>
		/// Vyhodnocení S1
		/// modrá barva   - k odpovídající S0 už existuje alespoň 1 S1
		/// červená barva - nesouhlasí počty kusů objednaných a dodaných; resp. nesouhlasí počty SN (CPE x uloženo v db)
		/// </summary>
		private void checkShipmentOrderConfirmation()
		{
			const int COLUMN_SHIPMENT_CONFIRMATION_ID = 1;

			foreach (GridViewRow gridViewRow in this.grdData.Rows)
			{
				if (gridViewRow.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION)
				{
					string s1ID = gridViewRow.Cells[COLUMN_SHIPMENT_CONFIRMATION_ID].Text;
					if (ShipmentReconciliationHelper.GetShipmentOrderConfirmationCount(s1ID) > 1)
					{
						// k S0 již existuje alespoň jedna S1
						gridViewRow.BackColor = BC.BlueColor;
					}
					else
					{
						if ((ShipmentReconciliationHelper.CheckShipmentOrderConfirmation(s1ID) == false) || 
							(ShipmentReconciliationHelper.CheckShipmentConfirmationSNsCounts(s1ID) == false))
						{
							// nesouhlasí počty kusů objednaných a dodaných; resp. nesouhlasí počty SN (CPE x uloženo v db)
							gridViewRow.BackColor = BC.RedColor;
						}
					}
				}
			}
		}

		/// <summary>
		/// Nastavení (ne)přístupnosti volby 'Schvaluji'
		/// (nepřístupné pro neschválenou materiálovou S1, jestliže k odpovídající S0 už existuje alespoň 1 S1)
		/// </summary>
		/// <param name="drv"></param>
		private void setApproveChoice(GridViewRow drv)
		{
			const int ITEM_APPROVE = 0;
			rdblDecision.Items[ITEM_APPROVE].Enabled = true;

			if (drv.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION &&
				drv.Cells[COLUMN_ORDER_TYPE_ID].Text == ORDER_MATERIAL &&
				ShipmentReconciliationHelper.GetShipmentOrderConfirmationCount(drv.Cells[1].Text) > 1)
			{
				rdblDecision.Items[ITEM_APPROVE].Enabled = false;
			}
		}

		/// <summary>
		/// Nastavení textboxu s poznámkou
		/// (data jsou z 'S0 - Objednávka expedice')
		/// </summary>
		private void setTxbRemarkReadOnly(object sipmentOrderID)
		{
			this.tbxRemark.Text = ShipmentHelper.GetRemarkForID(sipmentOrderID);
			if (this.tbxRemark.Text.IsNotNullOrEmpty())
			{
				this.tbxRemark.Enabled = false;
				this.tbxRemark.BackColor = Color.White;
				this.tbxRemark.ForeColor = Color.Black;
				this.setRemarkVisibility(true);
			}
			else
			{
				this.setRemarkVisibility(false);
			}
		}

		/// <summary>
		/// Nastaví viditelnost nadpisu a textboxu s poznámkou
		/// (data jsou z 'S0 - Objednávka expedice')
		/// </summary>
		/// <param name="visibility"></param>
		private void setRemarkVisibility(bool visibility)
		{
			this.lblRemark.Visible = visibility;
			this.tbxRemark.Visible = visibility;
		}

		/// <summary>
		/// Vytvoření podmínky WHERE (pro pager data - filtrace)
		/// </summary>
		private string createWhereClause()
		{
			string whereClause = " 1=1 ";

			if (this.ddlDecisionFlt.SelectedValue != "-1")
				whereClause += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue;

			if (!string.IsNullOrWhiteSpace(tbxDatumZavozuFlt.Text))
				try
				{
					whereClause += " AND [MessageDateOfReceipt] >='" + (Convert.ToDateTime(tbxDatumZavozuFlt.Text.Trim())).ToString("yyyyMMdd") + "'";
				}
				catch (Exception)
				{
				}

			if (!string.IsNullOrWhiteSpace(tbxDatumZavozuFltDo.Text))
				try
				{
					whereClause += " AND [MessageDateOfReceipt] <='" + (Convert.ToDateTime(tbxDatumZavozuFltDo.Text.Trim())).ToString("yyyyMMdd") + "'";
				}
				catch (Exception)
				{
				}

			if (!string.IsNullOrWhiteSpace(this.tbxObjednavkaIDFlt.Text))
				whereClause += " AND [ShipmentOrderID] = " + this.tbxObjednavkaIDFlt.Text.Trim();

			if (this.ddlCompanyName.SelectedValue != "-1")
				whereClause += " AND [CompanyName] like '" + this.ddlCompanyName.SelectedItem.Text.Trim() + "%'";

			if (this.ddlCityName.SelectedValue != "-1")
				whereClause += " AND [City] like '" + this.ddlCityName.SelectedItem.Text.Trim() + "%'";

			if (this.ddlOrderType.SelectedValue != "-1")
				whereClause += " AND [OrderTypeID] = " + this.ddlOrderType.SelectedValue;

			if (!string.IsNullOrWhiteSpace(this.tbxMessageIdFlt.Text))
				whereClause += " AND [MessageId] = " + this.tbxMessageIdFlt.Text.Trim();

			if (this.ddlUsersModifyFlt.SelectedValue != "-1")
				whereClause += " AND [ModifyUserId] = " + this.ddlUsersModifyFlt.SelectedValue;

			// filtrace dle popisu zboží
			if (this.tbxDescriptionFlt.Text.Trim().IsNotNullOrEmpty() && ViewState["IDforFltByDescr"] != null)
			{
				whereClause +=
					string.Format(" AND ID IN " +
                                 "( " +
                                   "SELECT DISTINCT(CMSOC.[ID]) " +
                                   "FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation] CMSOC " +
                                   "LEFT OUTER JOIN [dbo].[CommunicationMessagesShipmentOrdersConfirmationItems] CMSOCI " +
                                 	  "ON CMSOC.ID = CMSOCI.CMSOId " +
                                   "WHERE CMSOCI.[ItemOrKitID] IN ({0}) " +
                                 " )", 
								 ViewState["IDforFltByDescr"]);
			}

			return whereClause;
		}

		/// <summary>
		/// Přidá popis kitu/itemu do filtrace podle popisu
		/// </summary>
		/// <param name="par1"></param>
		private void addRecFltByDescr(string par1)
		{
			string error = string.Empty;
			this.lblErrorFltByDescr.Text = string.Empty;

			if (this.ddlKitsFltByDescr.SelectedValue != "-1" && par1 == "KIT" || this.ddlMaterialFltByDescr.SelectedValue != "-1" && par1 == "MATERIAL")
			{
				if (ShipmentReconciliationHelper.GoodIsUniqueInFltByDescr(gvKitsOrItemsNew, this.ddlKitsFltByDescr, this.ddlMaterialFltByDescr, "CheckBoxR", par1) == false)
				{
					this.lblErrorFltByDescr.Text = "Ve filtraci dle popisu přidávaný materiál/KIT již existuje<br />";
					return;
				}

				DataTable myT = new DataTable("myDt");
				this.createTableFltByDescr(ref myT);

				if (this.gvKitsOrItemsNew != null && gvKitsOrItemsNew.Rows.Count > 0)
				{
					foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
					{
						DataRow newDataRow = myT.NewRow();
						CheckBox myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
						newDataRow[0] = (myChkb.Checked) ? 1 : 0;						// AnoNe						
						newDataRow[1] = WConvertStringToInt32(gvr.Cells[1].Text);		// ItemVerKit
						newDataRow[2] = WConvertStringToInt32(gvr.Cells[2].Text);		// ItemOrKitID
						newDataRow[3] = HttpUtility.HtmlDecode(gvr.Cells[3].Text);		// ItemOrKitCode
						newDataRow[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);		// DescriptionCzItemsOrKit
						myT.Rows.Add(newDataRow);
					}
				}

				string proS = string.Empty;
				if (par1 == "KIT")
				{
					proS = string.Format("SELECT [ID], [Code], [DescriptionCz], [IsActive], [ModifyDate], [ModifyUserId] FROM [dbo].[cdlKits] WHERE ID = {0} AND [IsActive] = 1", this.ddlKitsFltByDescr.SelectedValue);
				}
				else
				{
					proS = string.Format("SELECT [ID], [Code], [DescriptionCz], [IsActive], [ModifyDate], [ModifyUserId] FROM [dbo].[cdlItems] WHERE ID = {0} AND [IsActive] = 1", this.ddlMaterialFltByDescr.SelectedValue);
				}
				DataTable myDataTable = BC.GetDataTable(proS);

				if (myDataTable != null && myDataTable.Rows.Count > 0)
				{
					DataRow addDataRow = myT.NewRow();
					addDataRow[0] = 1;																		// AnoNe
					addDataRow[1] = par1 == "KIT" ? 1 : 0;													// ItemVerKit
					addDataRow[2] = WConvertStringToInt32(myDataTable.Rows[0][0].ToString());				// ItemOrKitID
					addDataRow[3] = myDataTable.Rows[0][1].ToString();										// ItemOrKitCode				
					addDataRow[4] = myDataTable.Rows[0][2].ToString();										// DescriptionCzItemsOrKit

					myT.Rows.Add(addDataRow);

					gvKitsOrItemsNew.DataSource = myT.DefaultView;
					gvKitsOrItemsNew.DataBind();
				}
			}
			else
			{
				error += "Vyberte kit nebo item<br />";
			}

			this.lblErrorFltByDescr.Text = error;
		}

		/// <summary>
		/// Založí tabulku pro filtraci dle popisu
		/// </summary>
		/// <param name="myDt"></param>
		private void createTableFltByDescr(ref DataTable myDt)
		{
			DataColumn myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Boolean");
			myDataColumn.ColumnName = "AnoNe";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ItemVerKit";  //  kit 1, item 0
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ItemOrKitId";   // id kitu nebo itemu
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "ItemOrKitCode";	// kód kitu/itemu			
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DescriptionCzItemsOrKit";	// popis kitu/itemu			
			myDt.Columns.Add(myDataColumn);
		}
	}
}