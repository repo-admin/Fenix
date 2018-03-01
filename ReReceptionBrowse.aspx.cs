using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.ApplicationHelpers;
using FenixHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// Zásobování - R0 přehled objednávek
	/// </summary>
	public partial class ReReceptionBrowse : BasePage
	{
		private const int HIDE_COLUMN = 12;

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
				
				string proSelect = "SELECT * FROM (SELECT '-1' cValue,' Vše' ctext UNION ALL " +
								   " SELECT ID cValue, [CompanyName]+' (' + City + ')'  ctext FROM [dbo].[cdlSuppliers]) xx ORDER BY ctext";
				FillDdl(ref this.ddlSuppliersFlt, proSelect);
				
				this.ddlReconciliationFlt.Visible = false;
				this.lblReconciliationFlt.Visible = false;
				//proSelect = "SELECT * FROM (SELECT '-1' cValue,' Vše' ctext UNION ALL " +
				//				   " SELECT  '0' ID, 'Neodsouhlaseno' ctext  UNION ALL  SELECT '1' ID , 'Odsouhlaseno'  ctext  UNION ALL  SELECT '2' ID , 'Zamítnuto'  ctext) xx ORDER BY ctext";
				//FillDdl(ref this.ddlReconciliationFlt, proSelect);
				fillPagerData(1);
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
				this.grdData.SelectedIndex = -1;
				this.pnlReConf.Visible = false;
				this.pnlItems.Visible = false;
			}
		}

		private void fillPagerData(int pageNo)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlReConf.Visible = false;
			this.pnlItems.Visible = false;
			this.pnlR1.Visible = false;
			this.pnlDetails.Visible = false;
			this.gvItems.SelectedIndex = -1;
			this.lblSerialNumbers.Text = String.Empty;
			BC.UnbindDataFromObject<GridView>(this.gvReConf, this.gvConfirmationItems, this.gvR1, this.gvCardStockItems, this.gvConfirmationItemsHistory);
			
			string proW = "IsActive=1";
			if (this.ddlMessageStatusFlt.SelectedValue != "-1") proW += " AND [MessageStatusId]=" + this.ddlMessageStatusFlt.SelectedValue;
			if (this.ddlSuppliersFlt.SelectedValue != "-1") proW += " AND [ItemSupplierId]=" + this.ddlSuppliersFlt.SelectedValue;
			//if (this.ddlReconciliationFlt.SelectedValue != "-1") proW += " AND Reconciliation=" + this.ddlReconciliationFlt.SelectedValue;

			string proS = "[ID] ,[MessageId] ,[MessageType] ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId] ,[HeliosOrderId] ,[ItemSupplierId]" +
						  ",[ItemSupplierDescription]   ,[ItemDateOfDelivery]   ,[IsManually]   ,[Notice]  ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId]  ,[DescriptionCz], HeliosObj ,[Reconciliation]";

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwCMRSent]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = proS;
			pagerData.WhereClause = proW;

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, HIDE_COLUMN);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

				ImageButton img = new ImageButton();
				foreach (GridViewRow gvr in this.grdData.Rows)
				{
					if (gvr.Cells[12].Text == "2")
					{
						img = (ImageButton)gvr.FindControl("btnR1new");
						img.Enabled = true;
						img.Visible = true;
					}
					else
					{
						img = (ImageButton)gvr.FindControl("btnR1new");
						img.Enabled = false;
						img.Visible = false;
					}
				}

				if (pagerData.ItemCount == 0)
				{ 
					this.grdPager.Visible = false; 
					this.gvConfirmationItems.Visible = false; 
					this.gvConfirmationItemsHistory.Visible = false; 
					this.gvOrdersItemsHistory.Visible = false; 
					this.gvReConf.Visible = false; 
				}
				else 
				{ 
					this.grdPager.Visible = true; 
					this.gvConfirmationItems.Visible = false; 
					this.gvConfirmationItemsHistory.Visible = false; 
					this.gvOrdersItemsHistory.Visible = false; 
				}
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());

				this.grdPager.Visible = false; 
				this.gvConfirmationItems.Visible = false; 
				this.gvConfirmationItemsHistory.Visible = false; 
				this.gvOrdersItemsHistory.Visible = false;
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlReConf.Visible = true;
			this.pnlItems.Visible = false;
			this.pnlR1.Visible = false;
			this.gvItems.SelectedIndex = -1;
			this.lblSerialNumbers.Text = String.Empty;
			this.pnlDetails.Visible = false;
			BC.UnbindDataFromObject<GridView>(this.gvReConf, this.gvConfirmationItems, this.gvR1, this.gvCardStockItems, this.gvConfirmationItemsHistory);
			
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[CommunicationMessagesSentId],[ItemSupplierId],[ItemSupplierDescription],[Reconciliation],ReconciliationYesNo,[MessageDateOfShipment],[ItemDateOfDelivery],HeliosOrderId, Notice" +
						  " FROM [dbo].[vwReceptionConfirmationHd] WHERE [IsActive] = {0} AND CommunicationMessagesSentId = {1}", 1, grdMainIdKey);
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.gvReConf.DataSource = myDataTable.DefaultView; this.gvReConf.DataBind();
				this.gvReConf.SelectedIndex = -1;
			}
			catch
			{
			}

			proS = String.Format("SELECT C.*,S.SourceCode  FROM [dbo].[vwCMRSentIt] C LEFT OUTER JOIN cdlSources S ON C.SourceId = S.Id WHERE C.[IsActive] = 1 AND C.CMSOId={0}", grdMainIdKey);
			try
			{
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvItems.DataSource = myDataTable.DefaultView;
				this.gvItems.DataBind();
				this.gvReConf.Visible = true;
			}
			catch
			{
			}
		}

		protected void gvConfirmationItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlDetails.Visible = true;
			BC.UnbindDataFromObject<GridView>(this.gvCardStockItems, this.gvConfirmationItemsHistory);

			GridViewRow gvr = gvConfirmationItems.SelectedRow;

			string proS = string.Empty;
			try
			{
				// jen nové 13.8.2014
				proS = string.Format("SELECT DISTINCT ID,[cdlStocksName],[ItemVerKitDescription],[GroupGoods],[Code] ,[DescriptionCz]" +
						  ",[ItemOrKitQuantity],[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger] ,[ItemOrKitReservedInteger]" +
						  " ,[ItemOrKitReleasedForExpeditionInteger] ,PC" +
						  " FROM [dbo].[vwCardStockItems] WHERE [IsActive] = {0} AND ItemOrKitId = {1} AND ItemOrKitQuality={2}", 1, gvr.Cells[2].Text, 1);
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.gvCardStockItems.Columns[5].Visible = true;
				this.gvCardStockItems.DataSource = myDataTable.DefaultView; this.gvCardStockItems.DataBind();
				this.gvCardStockItems.Columns[5].Visible = false;
			}
			catch
			{
			}

			try
			{
				proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[ItemDescription] ,[ItemQuantity], ItemQuantityInt  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[IsActive] ,[ModifyDate] ,[ModifyUserId],[GroupGoods],[Code],[DescriptionCz] " +
						  " , NDReceipt FROM [dbo].[vwReceptionConfirmationIt] WHERE [IsActive] = {0} AND ItemID = {1}", 1, gvr.Cells[2].Text);
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItemsHistory.DataSource = myDataTable.DefaultView; this.gvConfirmationItemsHistory.DataBind();
				this.gvConfirmationItemsHistory.Visible = true;
			}
			catch
			{
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.pnlR1.Visible = false;
			fillPagerData(1);
		}

		protected void gvReConf_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlItems.Visible = true;
			this.lblSerialNumbers.Text = String.Empty;
			this.gvConfirmationItems.SelectedIndex = -1;
			this.pnlDetails.Visible = false;
			BC.UnbindDataFromObject<GridView>(this.gvConfirmationItems, this.gvCardStockItems, this.gvConfirmationItemsHistory);
						
			try
			{
				string proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[ItemDescription] ,[ItemQuantity] ,ItemQuantityInt ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[IsActive] ,[ModifyDate] ,[ModifyUserId],[GroupGoods],[Code],[DescriptionCz],CMRSIItemQuantity " +
						  " ,NDReceipt FROM [dbo].[vwReceptionConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, gvReConf.SelectedValue);
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView; this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;
				this.gvConfirmationItems.Visible = true;
			}
			catch
			{
			}
		}

		protected void gvReConfOnRowCommand(object sender, GridViewCommandEventArgs e)
		{
			int id = WConvertStringToInt32(e.CommandArgument.ToString());
			SerNumView(id);
		}

		protected void SerNumView(int id)
		{
			this.lblSerialNumbers.Text = String.Empty;
			string proS = string.Empty;
			try
			{
				proS = "DECLARE @SN AS nvarchar(max); SELECT @SN = ISNULL(@SN,'') + ItemSNs FROM [dbo].[CommunicationMessagesReceptionConfirmationItems] WHERE [IsActive]=1 AND CMSOId = " + id.ToString() + "; SELECT @SN";
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				this.lblSerialNumbers.Text = "SN: " + myDataTable.Rows[0][0].ToString();
			}
			catch
			{
			}
		}  // SerNum

		protected void OrderView(int id)
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
				if (dtObjHl == null || dtObjHl.Rows.Count < 1) 
				{ 
					mOK = false; 
				}

				proS = string.Format("SELECT [ID] ,[CMSOId] ,[HeliosOrderId] ,[HeliosOrderRecordId] ,[ItemId] ,[GroupGoods] ,[ItemCode] ,[ItemDescription] ,[ItemQuantity] ,[ItemQuantityDelivered]" +
									 ",[MeasuresID]  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[ItemQualityCode] ,[SourceId]  ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId]" +
									 " ,[ItemQuantityInt] ,[ItemQuantityDeliveredInt]  FROM [dbo].[vwCMRSentIt] WHERE CMSOId={0}", id);
				DataTable dtObjR = new DataTable();
				dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjR == null || dtObjR.Rows.Count < 1) 
				{ 
					mOK = false; 
				}

				proS = string.Format("SELECT [ID] ,[MessageId] ,[MessageTypeId] ,[MessageDescription] ,[MessageDateOfReceipt] ,[CommunicationMessagesSentId] ,[ItemSupplierId]" +
									 ",[ItemSupplierDescription] ,[Reconciliation] ,[IsActive] ,[ModifyDate] ,[ModifyUserId]" +
									 " FROM [dbo].[CommunicationMessagesReceptionConfirmation] where CommunicationMessagesSentId={0} AND Reconciliation<>0", id);
				DataTable dtObjHlCon = new DataTable();
				dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHlCon == null || dtObjHlCon.Rows.Count < 1) 
				{ 
					mOKR = false; 
				}

				DataTable dtObjHlRCon = new DataTable();
				if (mOKR)
				{
					string ids = "-99";
					foreach (DataRow dr in dtObjHlCon.Rows)
					{
						ids += "," + dr[0].ToString();
					}

					proS = string.Format("SELECT [ID] ,[CMSOId] ,[ItemID] ,[GroupGoods] ,[Code] ,[ItemDescription] ,[DescriptionCz] ,[ItemQuantity] ,[CMRSIItemQuantity]  ,[ItemUnitOfMeasure]" +
										 ",[ItemQualityId] ,[IsActive]  ,[ModifyDate] ,[ModifyUserId] ,[CommunicationMessagesSentId] ,[NDReceipt], ItemSns,ItemQuantityInt " +
										 " FROM [dbo].[vwReceptionConfirmationIt] where [CMSOId] in ({0})", ids);
					dtObjHlRCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
					if (dtObjHlRCon == null || dtObjHlRCon.Rows.Count < 1) 
					{ 
						mOKR = false; 
					}
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
							worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["ItemSupplierDescription"].ToString();  // ItemSupplierDescription
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Datum odeslání");
							worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["MessageDateOfShipment"].ToString();    // MessageDateOfShipment	
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 10].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							radek += 1;
							// identifikace objednavky v Heliosu
							worksheet.Cells[radek, 1].Value = String.Format("Řada dokladů");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0]["RadaDokladu"].ToString();  // RadaDokladu
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Pořadové číslo");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["PoradoveCislo"].ToString();  // PoradoveCislo
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format(""); // String.Format("HeliosOrderId");
							worksheet.Cells[radek, 6].Value = String.Format(""); // dtObjHl.Rows[0]["HeliosOrderId"].ToString();  // HeliosOrderId
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Pož. datum naskladnění");
							worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();
							worksheet.Cells[radek, 8].Style.Numberformat.Format = "yyy-mm-dd";
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
							worksheet.Cells[radek, 9].Style.Font.Bold = true;
							worksheet.Cells[radek, 10].Value = String.Format("");  //dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();      // ItemDateOfDelivery
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
								worksheet.Cells[radek, 6].Value = dr["ItemQuantityDeliveredInt"].ToString();
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
								worksheet.Cells[radek, 1].Value = String.Format("R1 - Confirmace objednávky");
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
									//	worksheet.Cells[radek, 4].Value = String.Format("Schváleno");
									BC.ExcelProcessReconciliation(worksheet.Cells[radek, 4], dr["Reconciliation"].ToString());

									worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
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
													worksheet.Cells[radek, 1].Value = e.ToString();
													//worksheet.Cells[radek, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
													//worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.SlateBlue);
													radek += 1;
												}
											}
										}
									}

								}

							}






							//foreach (var par in dvojice)
							//{
							//	string[] sn = par.Split(',');
							//	string adresaA = String.Format("A{0}", radek);
							//	string adresaB = String.Format("B{0}", radek);
							//	worksheet.Cells[adresaA].Value = sn[0];
							//	worksheet.Cells[adresaB].Value = sn[1];
							//	radek++;
							//}

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
							xls.Workbook.Properties.Title = "RO objednávka";
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


		} // OrderView

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			
			if (e.CommandName == "OrderView") { 
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				OrderView(id);
			}
			if (e.CommandName == "R1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				R1New(id); Session["IsRefresh"] = "0";
			}
		}

		protected void R1New(int id)
		{
			this.pnlR1.Visible = true;
			string proS = String.Format("SELECT C.*,S.SourceCode  FROM [dbo].[vwCMRSentIt] C LEFT OUTER JOIN cdlSources S ON C.SourceId = S.Id WHERE C.[IsActive] = 1 AND C.CMSOId={0}", id);
			try
			{
				this.pnlItems.Visible = false; this.gvItems.DataSource = null; this.gvItems.DataBind();
				this.pnlReConf.Visible = false; this.gvReConf.DataSource = null; this.gvReConf.DataBind();
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				//12-17
				this.gvR1.Columns[12].Visible = true; this.gvR1.Columns[13].Visible = true; this.gvR1.Columns[14].Visible = true; this.gvR1.Columns[15].Visible = true; this.gvR1.Columns[16].Visible = true;
				this.gvR1.DataSource = myDataTable.DefaultView;
				this.gvR1.DataBind();
				this.gvR1.Columns[12].Visible = false; this.gvR1.Columns[13].Visible = false; this.gvR1.Columns[14].Visible = false; this.gvR1.Columns[15].Visible = false; this.gvR1.Columns[16].Visible = false;
			}
			catch
			{
			}
		}

		protected void btnR1Back_Click(object sender, EventArgs e)
		{
			this.pnlR1.Visible = false; this.gvR1.DataSource = null; this.gvR1.DataBind();
		}

		protected void btnR1Save_Click(object sender, EventArgs e)
		{
			if (Session["IsRefresh"].ToString() == "0" || Session["IsRefresh"] == null)
			{
				// kontrola hodnot
				bool mOK = true; string Err = string.Empty; int ii = 0; int iMaSmysl = 0;
				TextBox tbx = new TextBox();
				foreach (GridViewRow gvr in this.gvR1.Rows)
				{
					try
					{
						tbx = (TextBox)gvr.FindControl("tbxQuantity");
						if (!string.IsNullOrEmpty(tbx.Text.Trim()))
						{
							ii = Convert.ToInt32(tbx.Text.Trim());
							iMaSmysl += ii;
						}
					}
					catch (Exception)
					{
						mOK = false;
					}
				}

				// zpracovani
				if (mOK && iMaSmysl > 0)
				{
					//mOK = false;
					DataTable dt = new DataTable();
					dt = BC.GetDataTable("SELECT min([MessageId]) as minMessageId FROM [dbo].[CommunicationMessagesReceptionConfirmation]", BC.FENIXRdrConnectionString);
					int iMessageID = WConvertStringToInt32(dt.Rows[0][0].ToString()) - 1;
					CultureInfo culture = new CultureInfo("cs-CZ");
					StringBuilder sb = new StringBuilder();

					dt = BC.GetDataTable("SELECT [ID] ,[HeliosOrderId] ,[ItemSupplierId] ,[ItemSupplierDescription] FROM [dbo].[vwCMRSent] WHERE id=" + ViewState["vwCMRSentID"].ToString(), BC.FENIXRdrConnectionString);
					
					sb.Append("<NewDataSet>");
					sb.Append("<CommunicationMessagesReceptionConfirmation>");
					sb.Append("<ID>-1</ID>");
					sb.Append("<MessageID>" + iMessageID.ToString() + "</MessageID>");
					sb.Append("<MessageTypeID>2</MessageTypeID>");
					sb.Append("<MessageTypeDescription>ReceptionConfirmation</MessageTypeDescription>");
					sb.Append("<MessageDateOfReceipt>" + DateTime.Today.ToString("yyyy-MM-dd") + "</MessageDateOfReceipt>");
					sb.Append("<ReceptionOrderID>" + dt.Rows[0][0].ToString() + "</ReceptionOrderID>");
					sb.Append("<HeliosOrderID>" + dt.Rows[0][1].ToString() + "</HeliosOrderID>");   //
					sb.Append("<ItemSupplierID>" + dt.Rows[0][2].ToString() + "</ItemSupplierID>");
					sb.Append("<ItemSupplierDescription>" + HttpUtility.HtmlDecode(dt.Rows[0][3].ToString()) + "</ItemSupplierDescription>");
					sb.Append("<items>");
					foreach (GridViewRow gvr in this.gvR1.Rows)
					{
						try
						{
							tbx = (TextBox)gvr.FindControl("tbxQuantity");
							if (!string.IsNullOrEmpty(tbx.Text.Trim()))
							{
								ii = Convert.ToInt32(tbx.Text.Trim());
								sb.Append("<item>");
								sb.Append("<HeliosOrderRecordID>" + HttpUtility.HtmlDecode(gvr.Cells[1].Text).Trim() + "</HeliosOrderRecordID>");
								sb.Append("<ItemID>" + gvr.Cells[11].Text + "</ItemID>");
								sb.Append("<ItemDescription>" + HttpUtility.HtmlDecode(gvr.Cells[5].Text) + "</ItemDescription>");
								sb.Append("<ItemQuantity>" + ii.ToString() + "</ItemQuantity>");
								sb.Append("<ItemUnitOfMeasureID>" + gvr.Cells[12].Text + "</ItemUnitOfMeasureID>");
								sb.Append("<ItemUnitOfMeasure>" + HttpUtility.HtmlDecode(gvr.Cells[13].Text) + "</ItemUnitOfMeasure>");
								sb.Append("<ItemQualityID>" + gvr.Cells[14].Text + "</ItemQualityID>");
								sb.Append("<ItemQuality>" + HttpUtility.HtmlDecode(gvr.Cells[15].Text) + "</ItemQuality>");
								sb.Append("</item>");
							}
						}
						catch (Exception)
						{
							mOK = false;
						}
					}
					sb.Append("</items>");
					sb.Append("</CommunicationMessagesReceptionConfirmation>");
					sb.Append("</NewDataSet>");


					string help = sb.ToString().Replace("{", "").Replace("}", "");
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(help);

					SqlConnection con = new SqlConnection();
					con.ConnectionString = BC.FENIXWrtConnectionString;
					SqlCommand com = new SqlCommand();
					com.CommandText = "prCMRCins";
					com.CommandType = CommandType.StoredProcedure;
					com.Connection = con;
					com.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
					com.Parameters.Add("@ReturnValue", SqlDbType.Int);
					com.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
					com.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2000);
					com.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

					try
					{
						con.Open();
						com.ExecuteNonQuery();
						if (com.Parameters["@ReturnValue"].Value.ToString() != "0")
						{
							mOK = false;
							Err = com.Parameters["@ReturnMessage"].Value.ToString();
						}


					}
					catch (Exception)
					{
						mOK = false;
					}
					finally
					{
						com = null;

					}
				}
				if (mOK)
				{
					Session["IsRefresh"] = "1";
					btnR1Back_Click(btnR1Back, EventArgs.Empty);

				}
			}
			else
			{
				btnR1Back_Click(btnR1Back, EventArgs.Empty);
			}

		}
	}
}