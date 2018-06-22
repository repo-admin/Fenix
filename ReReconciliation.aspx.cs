using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using Fenix.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// Zásobování - R1 schválení příjmu položek
	/// </summary>
	public partial class ReReconciliation : BasePage
	{
		#region Properties

		/// <summary>
		/// Bez rozhodnutí
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		/// <summary>
		/// Číslo sloupce pro Reception Confirmation ID
		/// </summary>
		private const int COLUMN_RECEPTION_CONFIRMATION_ID = 1;

		/// <summary>
		/// Číslo sloupce pro Reception Order ID
		/// </summary>
		private const int COLUMN_SHIPMENT_ORDER_ID = 6;

		/// <summary>
		/// Číslo sloupce pro schválení {druhy schválení -> 0 .. bez rozhodnutí  1 .. schváleno    2 .. zamítnuto	3 .. D0 odeslána}
		/// </summary>
		private const int COLUMN_RECONCILIATION = 10;
		
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
				BaseHelper.FillDdlMessageStatuses(ref this.ddlMessageStatusFlt);
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
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwReceptionConfirmationHd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = " [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[CommunicationMessagesSentId],[ItemSupplierId],[ItemSupplierDescription],[Reconciliation],[MessageDateOfShipment],[ItemDateOfDelivery],HeliosObj,[HeliosOrderId]";
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref this.grdData, COLUMN_RECONCILIATION);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);				
				this.checkReceptionOrderConfirmation();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}			
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlItems.Visible = true;
			this.gvConfirmationItems.SelectedIndex = -1;
			this.pnlDetails.Visible = false;
			BC.UnbindDataFromObject<GridView>(this.gvConfirmationItems, this.gvCardStockItems, this.gvConfirmationItemsHistory);
						
			try
			{
				string proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[ItemDescription] ,[ItemQuantityInt]  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[IsActive] ,[ModifyDate] ,[ModifyUserId],[GroupGoods],[Code],[DescriptionCz],CMRSIItemQuantity " +
						  " FROM [dbo].[vwReceptionConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, grdData.SelectedValue);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView; 
				this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;
			}
			catch
			{
			}

			this.checkReceptionOrderConfirmation();

			//******************************
			//String s = HttpUtility.HtmlDecode(drv.Cells[7].Text);
			//if (this.ddlDecisionFlt.SelectedValue.ToString() != "-1") { this.btnDecision.Enabled = false; this.rdblDecision.Enabled = false; } else { this.btnDecision.Enabled = true; this.rdblDecision.Enabled = true; }
			if (this.ddlDecisionFlt.SelectedValue.ToString() == "0") { this.btnDecision.Enabled = true; this.rdblDecision.Enabled = true; } else { this.btnDecision.Enabled = false; this.rdblDecision.Enabled = false; }
		}

		protected void gvConfirmationItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlDetails.Visible = true;
			BC.UnbindDataFromObject<GridView>(this.gvCardStockItems, this.gvConfirmationItemsHistory);

			GridViewRow gvr = gvConfirmationItems.SelectedRow;

			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT ID,[cdlStocksName],[ItemVerKitDescription],[GroupGoods],[Code] ,[DescriptionCz]"+
					",[ItemOrKitQuantity],[ItemOrKitFree],[ItemOrKitUnConsilliation] ,[ItemOrKitReserved] ,[ItemOrKitReleasedForExpedition] ,PC" +
					",[ItemOrKitQuantityInteger],[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger] ,[ItemOrKitReservedInteger] ,[ItemOrKitReleasedForExpeditionInteger]" +
                    " FROM [dbo].[vwCardStockItems] WHERE [IsActive] = {0} AND ItemOrKitId = {1}", 1, gvr.Cells[2].Text);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvCardStockItems.DataSource = myDataTable.DefaultView; 
				this.gvCardStockItems.DataBind();
			}
			catch
			{
			}

			try
			{
				proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[ItemDescription] ,[ItemQuantityInt]  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[IsActive] ,[ModifyDate] ,[ModifyUserId],[GroupGoods],[Code],[DescriptionCz] " +
						  " FROM [dbo].[vwReceptionConfirmationIt] WHERE [IsActive] = {0} AND ItemID = {1}", 1, gvr.Cells[2].Text);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItemsHistory.DataSource = myDataTable.DefaultView; 
				this.gvConfirmationItemsHistory.DataBind();
			}
			catch
			{
			}
		}

		protected void btnDecision_Click(object sender, EventArgs e)
		{
			bool mOK = false;
			Int16 iDecision;
			try
			{
				iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue.ToString());
				GridViewRow selectedRow = grdData.SelectedRow;
				if (grdData.SelectedValue != null && selectedRow != null)
				{

					SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
					SqlCommand sqlComm = new SqlCommand();
					sqlComm.CommandType = CommandType.StoredProcedure;
					sqlComm.CommandText = "[dbo].[prCMRCconsent]";
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
					sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());

					sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);			//2015-11-03
					sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 2;												//2015-11-03
					sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "ReceptionConfirmation";		//2015-11-03

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
							mOK = true;
						}
					}
					catch (Exception)
					{
						mOK = false;
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
			if (mOK)
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

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int idC = WConvertStringToInt32(e.CommandArgument.ToString()); 
			//int idC = WConvertStringToInt32(e.CommandArgument.ToString());  // toto je View z confirmace    !!!!!!!
			if (e.CommandName == "OrderView")
			{ // 1

				string proS = string.Format("SELECT [CommunicationMessagesSentId] FROM CommunicationMessagesReceptionConfirmation WHERE id={0}",idC);
				DataTable dt = new DataTable();
				dt = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dt != null || dt.Rows.Count == 1) 
				{
					int id = WConvertStringToInt32(dt.Rows[0][0].ToString());
					OrderView(id, idC);
				}
				
			}  //1
		}

		protected void OrderView(int id, int idC)
		{
			try
			{
				bool mOK = true;
				bool mOKR = true;

				string proS = string.Format("SELECT [MessageId]  ,[MessageType] ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId] ,[HeliosOrderId] ,[ItemSupplierId] ,[ItemSupplierDescription]" +
											",[ItemDateOfDelivery]  ,[IsManually]  ,[StockId]  ,[Notice]  ,[RadaDokladu] ,[PoradoveCislo] ,[RadaPlusPorCislo] ,[IsActive] ,[ModifyDate]  ,[ModifyUserId]" +
											" FROM [dbo].[CommunicationMessagesReceptionSent] WHERE Id = {0} ORDER BY 1,2", id);
				DataTable dtObjHl = new DataTable();
				dtObjHl = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHl == null || dtObjHl.Rows.Count < 1) { mOK = false; }

				proS = string.Format("SELECT [ID] ,[CMSOId] ,[HeliosOrderId] ,[HeliosOrderRecordId] ,[ItemId] ,[GroupGoods] ,[ItemCode] ,[ItemDescription] ,[ItemQuantityInt] ,[ItemQuantityDelivered]" +
									 ",[MeasuresID]  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[ItemQualityCode] ,[SourceId]  ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId]" +
									 " FROM [dbo].[vwCMRSentIt] WHERE CMSOId={0}", id);
				DataTable dtObjR = new DataTable();
				dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjR == null || dtObjR.Rows.Count < 1) { mOK = false; }

				proS = string.Format("SELECT [ID] ,[MessageId] ,[MessageTypeId] ,[MessageDescription] ,[MessageDateOfReceipt] ,[CommunicationMessagesSentId] ,[ItemSupplierId]" +
									 ",[ItemSupplierDescription] ,[Reconciliation] ,[IsActive] ,[ModifyDate] ,[ModifyUserId]" +
									 " FROM [dbo].[CommunicationMessagesReceptionConfirmation] where ID={0}", idC);
				DataTable dtObjHlCon = new DataTable();
				dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHlCon == null || dtObjHlCon.Rows.Count < 1) { mOKR = false; }

				DataTable dtObjHlRCon = new DataTable();
				if (mOKR)
				{
					string ids = "-99";
					foreach (DataRow dr in dtObjHlCon.Rows)
					{
						ids += "," + dr[0].ToString();
					}

					proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[GroupGoods] ,[Code] ,[ItemDescription] ,[DescriptionCz] ,[ItemQuantityInt] ,[CMRSIItemQuantity]  ,[ItemUnitOfMeasure]" +
										 ",[ItemQualityId] ,[IsActive]  ,[ModifyDate] ,[ModifyUserId] ,[CommunicationMessagesSentId] ,[NDReceipt], ItemSns " +
										 " FROM [dbo].[vwReceptionConfirmationIt] where [CMSOId] in ({0})", ids);
					dtObjHlRCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
					if (dtObjHlRCon == null || dtObjHlRCon.Rows.Count < 1) { mOKR = false; }
				}
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
							worksheet.Cells[radek, 1, radek, 8].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("R0 - Objednávka");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka objednavky
							worksheet.Cells[radek, 1].Value = String.Format("Message ID");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0][0].ToString();  // MessageID
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Message Popis");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0][2].ToString();  // MessageDescription
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Dodavatel");
							worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["ItemSupplierDescription"].ToString();  
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Datum odeslání");
							//worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["MessageDateOfShipment"].ToString();    
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["MessageDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS, true);														  
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 10].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							radek += 1;
							// identifikace objednavky v Heliosu
							worksheet.Cells[radek, 1].Value = String.Format("Řada dokladů");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0]["RadaDokladu"].ToString();  
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Pořadové číslo");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["PoradoveCislo"].ToString();  
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format(""); 
							worksheet.Cells[radek, 6].Value = String.Format(""); 
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Pož. datum naskladnění");
							//worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["ItemDateOfDelivery"], BC.DATE_TIME_FORMAT_DDMMYYY, true);							
							worksheet.Cells[radek, 9].Value = String.Format(""); 
							worksheet.Cells[radek, 9].Style.Font.Bold = true;
							worksheet.Cells[radek, 10].Value = String.Format("");
							worksheet.Cells[radek, 10].Style.Font.Bold = true;

							radek += 2;
							// detaily objednávky
							worksheet.Cells[radek, 1].Value = String.Format("HeliosOrderRecordId");
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 2].Value = String.Format("SkZ");
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Kód");
							worksheet.Cells[radek, 3].Style.Font.Bold = true;
							worksheet.Cells[radek, 4].Value = String.Format("Popis");
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Objed. množství");
							worksheet.Cells[radek, 5].Style.Font.Bold = true;
							worksheet.Cells[radek, 6].Value = String.Format("Dodané množství");
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("MJ");
							worksheet.Cells[radek, 7].Style.Font.Bold = true;
							worksheet.Cells[radek, 8].Value = String.Format("Kvalita");
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 9].Style.Font.Bold = true;
							worksheet.Cells[radek, 10].Value = String.Format("");
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							radek += 1;

							foreach (DataRow dr in dtObjR.Rows)
							{
								worksheet.Cells[radek, 1].Value = dr["HeliosOrderRecordId"].ToString();
								worksheet.Cells[radek, 2].Value = dr["GroupGoods"].ToString();
								worksheet.Cells[radek, 3].Value = dr["ItemCode"].ToString();
								worksheet.Cells[radek, 4].Value = dr["ItemDescription"].ToString();
								worksheet.Cells[radek, 5].Value = dr["ItemQuantityInt"].ToString();
								worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 6].Value = dr["ItemQuantityDelivered"].ToString();
								worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 7].Value = dr["ItemUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 8].Value = dr["ItemQualityCode"].ToString();
								worksheet.Cells[radek, 9].Value = String.Format("");
								worksheet.Cells[radek, 10].Value = String.Format("");
								radek += 1;
							}
							if (mOKR)
							{
								radek += 2;
								// hlavička konfirmace
								worksheet.Cells[radek, 1, radek, 8].Merge = true;
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.Size = 14;
								worksheet.Cells[radek, 1].Value = String.Format("R0 - Confirmace objednávky");
								worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
								worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
								worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

								foreach (DataRow dr in dtObjHlCon.Rows)
								{
									radek += 1;
									worksheet.Cells[radek, 1].Value = String.Format("Message Id");
									worksheet.Cells[radek, 1].Style.Font.Bold = true;
									worksheet.Cells[radek, 2].Value = String.Format("Message popis");
									worksheet.Cells[radek, 2].Style.Font.Bold = true;
									worksheet.Cells[radek, 3].Value = String.Format("Dodavatel");
									worksheet.Cells[radek, 3].Style.Font.Bold = true;
									worksheet.Cells[radek, 4].Value = String.Format("Odsouhlasení");
									worksheet.Cells[radek, 4].Style.Font.Bold = true;
									worksheet.Cells[radek, 5].Value = String.Format("Datum zapsání do Fenixu");
									worksheet.Cells[radek, 5].Style.Font.Bold = true;
									worksheet.Cells[radek, 6].Value = String.Format("Aktivita");
									worksheet.Cells[radek, 6].Style.Font.Bold = true;
									worksheet.Cells[radek, 7].Value = String.Format("");
									worksheet.Cells[radek, 7].Style.Font.Bold = true;
									worksheet.Cells[radek, 8].Value = String.Format("");
									worksheet.Cells[radek, 8].Style.Font.Bold = true;
									worksheet.Cells[radek, 9].Value = String.Format("");
									worksheet.Cells[radek, 9].Style.Font.Bold = true;
									worksheet.Cells[radek, 10].Value = String.Format("");
									worksheet.Cells[radek, 10].Style.Font.Bold = true;
									radek += 1;
									worksheet.Cells[radek, 1].Value = dr["MessageId"].ToString();
									worksheet.Cells[radek, 2].Value = dr["MessageDescription"].ToString();
									worksheet.Cells[radek, 3].Value = dr["ItemSupplierDescription"].ToString();

									//if (dr["Reconciliation"].ToString() == "2")
									//{
									//	worksheet.Cells[radek, 4].Style.Fill.PatternType = ExcelFillStyle.LightUp;
									//	worksheet.Cells[radek, 4].Style.Fill.BackgroundColor.SetColor(Color.Red);
									//	worksheet.Cells[radek, 4].Value = String.Format("Zamítnuto");
									//}
									//else
									//{ 
									//	worksheet.Cells[radek, 4].Value = String.Format("Schváleno"); 
									//}
									BC.ExcelProcessReconciliation(worksheet.Cells[radek, 4], dr["Reconciliation"].ToString());
																		
									//worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
									BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dr["ModifyDate"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS);																		
									worksheet.Cells[radek, 6].Value = dr["IsActive"].ToString();
									worksheet.Cells[radek, 7].Value = String.Format("");
									worksheet.Cells[radek, 8].Value = String.Format("");
									worksheet.Cells[radek, 9].Value = String.Format("");
									worksheet.Cells[radek, 10].Value = String.Format("");
									
									radek += 2;
									worksheet.Cells[radek, 1].Value = String.Format("Item Id");
									worksheet.Cells[radek, 1].Style.Font.Bold = true;
									worksheet.Cells[radek, 2].Value = String.Format("SkZ");
									worksheet.Cells[radek, 2].Style.Font.Bold = true;
									worksheet.Cells[radek, 3].Value = String.Format("Kód");
									worksheet.Cells[radek, 3].Style.Font.Bold = true;
									worksheet.Cells[radek, 4].Value = String.Format("Popis");
									worksheet.Cells[radek, 4].Style.Font.Bold = true;
									worksheet.Cells[radek, 5].Value = String.Format("Množství");
									worksheet.Cells[radek, 5].Style.Font.Bold = true;
									worksheet.Cells[radek, 6].Value = String.Format("MJ");
									worksheet.Cells[radek, 6].Style.Font.Bold = true;
									worksheet.Cells[radek, 7].Value = String.Format("Doklad ND");
									worksheet.Cells[radek, 7].Style.Font.Bold = true;
									worksheet.Cells[radek, 8].Value = String.Format("");
									worksheet.Cells[radek, 8].Style.Font.Bold = true;
									worksheet.Cells[radek, 9].Value = String.Format("");
									worksheet.Cells[radek, 9].Style.Font.Bold = true;
									worksheet.Cells[radek, 10].Value = String.Format("");
									worksheet.Cells[radek, 10].Style.Font.Bold = true;
									radek += 1;
									
									for (int ii = 0; ii <= dtObjHlRCon.Rows.Count - 1; ii++)
									{
										DataRow drc = dtObjHlRCon.Rows[ii];

										if (dr["ID"].ToString() == drc["CMSOId"].ToString())
										{
											worksheet.Cells[radek, 1].Value = drc["ItemID"].ToString();
											worksheet.Cells[radek, 2].Value = drc["GroupGoods"].ToString();
											worksheet.Cells[radek, 3].Value = drc["Code"].ToString();
											worksheet.Cells[radek, 4].Value = drc["ItemDescription"].ToString();
											worksheet.Cells[radek, 5].Value = drc["ItemQuantityInt"].ToString();
											worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
											worksheet.Cells[radek, 6].Value = drc["ItemUnitOfMeasure"].ToString();
											worksheet.Cells[radek, 7].Value = drc["NDReceipt"].ToString();
											worksheet.Cells[radek, 8].Value = String.Format("");
											worksheet.Cells[radek, 9].Value = String.Format("");
											worksheet.Cells[radek, 10].Value = String.Format("");
											radek += 1;
											if (!string.IsNullOrWhiteSpace(drc["ItemSNs"].ToString()))
											{
												worksheet.Cells[radek, 1].Value = String.Format("Sériová čísla");
												worksheet.Cells[radek, 1].Style.Font.Bold = true;
												radek += 1;
												string[] sn = drc["ItemSNs"].ToString().Split(',');
												foreach (var e in sn)
												{
													worksheet.Cells[radek, 1].Value = e.IsNotNullOrEmpty() ? e.Trim() : String.Empty;
													//worksheet.Cells[radek, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
													//worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.SlateBlue);
													radek += 1;
												}
											}
										}
									}
								}
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
							throw;
						}
					}
				}
			}
			catch
			{
			}
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlItems.Visible = false;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		/// <summary>
		/// Vyhodnocení R1
		/// modrá barva   - k dané R0 už existuje alespoň 1 R1
		/// červená barva - nesouhlasí počty kusů objednaných a dodaných	; resp. nesouhlasí počty SN (CPE x uloženo v db)	
		/// </summary>
		private void checkReceptionOrderConfirmation()
		{			
			foreach (GridViewRow gridViewRow in this.grdData.Rows)
			{
				if (gridViewRow.Cells[COLUMN_RECONCILIATION].Text == WITHOUT_DECISION)
				{
					string receptionConfirmationID = gridViewRow.Cells[COLUMN_RECEPTION_CONFIRMATION_ID].Text;
					if (ReceptionConfirmationHelper.GetReceptionOrderConfirmationCount(receptionConfirmationID) > 1)
					{
						// k R0 již existuje alespoň jedna R1
						gridViewRow.BackColor = BC.BlueColor;
					}
					else
					{
						if ((ReceptionConfirmationHelper.CheckReceptionConfirmationBalance(receptionConfirmationID) == false) ||
							(ReceptionConfirmationHelper.CheckReceptionConfirmationSNsCounts(receptionConfirmationID) == false))
						{
							// nesouhlasí počty kusů objednaných a dodaných, resp. nesouhlasí počty SN (CPE x uloženo v db)
							gridViewRow.BackColor = BC.RedColor;
						}
					}
				}
			}
		}
	}
}