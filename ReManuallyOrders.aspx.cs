using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.ApplicationHelpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// [Příjem zboží] R0 - objednávka nového zboží
	/// </summary>
	public partial class ReManuallyOrders : BasePage
	{
		/// <summary>
		/// Reconciliation
		/// </summary>
		private const int COLUMN_RECONCILIATION = 13;
				
		/// <summary>
		/// MessageStatusId
		/// </summary>
		private const int COLUMN_MESSAGE_STATUS_ID = 14;

		//seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		private int[] hideGrdDataColumns = new int[] { COLUMN_RECONCILIATION, COLUMN_MESSAGE_STATUS_ID };

		/// <summary>
		/// Zamítnuto
		/// </summary>
		private const string REJECTED = "2";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				ReceptionManuallyOrdersHelper.FillDdlCompanyName(ref this.ddlCompanyName);
				ReceptionManuallyOrdersHelper.FillDdlMessageStatus(ref this.ddlMessageStatusFlt);
				this.fillPagerData(1);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.DesignMode) return;

			base.OnInit(e);
			this.grdPager.ShowNewItem = false;
			this.grdPager.PageSize = 10;
			this.grdPager.Command += new CommandEventHandler(this.pagerCommand);
			this.btnSave.Click += new EventHandler(this.btnSave_Click);
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

		private void fillData()
		{
			this.fillPagerData(this.grdPager.CurrentIndex);
		}

		private void fillPagerData(int pageNo)
		{
			this.grdData.SelectedIndex = -1;
			this.gvItems.SelectedIndex = -1;
			this.pnlReConf.Visible = false;
			this.pnlItems.Visible = false;
			this.pnlR1.Visible = false;
			this.pnlDetails.Visible = false;			
			this.lblSerialNumbers.Text = String.Empty;
			BC.UnbindDataFromObject<GridView>(this.gvReConf, this.gvConfirmationItems, this.gvR1, this.gvCardStockItems, this.gvConfirmationItemsHistory);

			//string proW = "IsActive = 1 AND IsManually = 1";  // 2015-02-18
			string proW = " IsActive = 1 ";						// 2015-03-20
			if (this.ddlCompanyName.SelectedValue.ToString() != "-1") proW += " AND [ItemSupplierId] = " + this.ddlCompanyName.SelectedValue.ToString();
			if (this.ddlMessageStatusFlt.SelectedValue.ToString() != "-1") proW += " AND [MessageStatusId] = " + this.ddlMessageStatusFlt.SelectedValue.ToString();
			if (!string.IsNullOrWhiteSpace(this.tbxDatumOdeslaniFlt.Text.Trim())) proW += " AND CONVERT(CHAR(8),[MessageDateOfShipment],112) = '" + WConvertDateToYYYYmmDD(this.tbxDatumOdeslaniFlt.Text.Trim()) + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxDatumDodaniFlt.Text.Trim())) proW += " AND CONVERT(CHAR(8),[ItemDateOfDelivery],112) = '" + WConvertDateToYYYYmmDD(this.tbxDatumDodaniFlt.Text.Trim()) + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxObjednavkaIDFlt.Text)) proW += " AND ID = " + this.tbxObjednavkaIDFlt.Text.Trim();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwCMRSent]";
			pagerData.OrderBy = "ID DESC";
			pagerData.ColumnList = "[ID], [MessageId], [MessageType], [MessageDescription], [MessageDateOfShipment], [MessageStatusId], [HeliosOrderId] " + 
				                  ",[ItemSupplierId], [ItemSupplierDescription], [ItemDateOfDelivery], [IsManually], [Notice], [IsActive], [DescriptionCz] " + 
								  ",[ModifyDate], [Reconciliation]";
			pagerData.WhereClause = proW;
			
			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, this.hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.setVisibilityButtonR1manually();										
				BaseHelper.SetPictureDeleteOrder(ref grdData, COLUMN_MESSAGE_STATUS_ID, "1");

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
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
				this.grdPager.Visible = false;
				this.gvConfirmationItems.Visible = false;
				this.gvConfirmationItemsHistory.Visible = false;
				this.gvOrdersItemsHistory.Visible = false;
			}
		}

		/// <summary>
		/// Pokud je objednávka zamítnuta, lze k ní vytvořit RUČNĚ R1
		/// (viditelný/neviditelný immage button ve sloupci R1)
		/// </summary>
		private void setVisibilityButtonR1manually()
		{
			ImageButton imageButton = new ImageButton();
			foreach (GridViewRow gvr in this.grdData.Rows)
			{
				imageButton = (ImageButton)gvr.FindControl("btnR1new");
				if (imageButton != null)
				{                                          
					bool orderIsRejected = (gvr.Cells[COLUMN_RECONCILIATION].Text == REJECTED);
					imageButton.Enabled = orderIsRejected;
					imageButton.Visible = orderIsRejected;
				}
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
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReConf.DataSource = myDataTable.DefaultView; this.gvReConf.DataBind();
				this.gvReConf.SelectedIndex = -1;
			}
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}

			proS = String.Format("SELECT C.*,S.SourceCode  FROM [dbo].[vwCMRSentIt] C LEFT OUTER JOIN cdlSources S ON C.SourceId = S.Id WHERE C.[IsActive] = 1 AND C.CMSOId={0}", grdMainIdKey);
			try
			{				
				DataTable myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvItems.DataSource = myDataTable.DefaultView;
				this.gvItems.DataBind();
				this.gvReConf.Visible = true;
			}
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
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
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
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
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "OrderView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				this.OrderView(id);
			}

			if (e.CommandName == "R1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				this.R1New(id); Session["IsRefresh"] = "0";
			}

			if (e.CommandName == "DeleteOrder")
			{
				string confirmValue = Request.Form["confirm_delete_order"];
				if (confirmValue.ToUpper() == "ANO")
				{
					BaseHelper.ProcessDeleteOrder(e, ref grdData, "1", "ReceptionOrder", Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString()));
				}
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
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void btnR1Back_Click(object sender, EventArgs e)
		{
			this.pnlR1.Visible = false; this.gvR1.DataSource = null; this.gvR1.DataBind();
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
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void gvReConfOnRowCommand(object sender, GridViewCommandEventArgs e)
		{
			int id = WConvertStringToInt32(e.CommandArgument.ToString());
			this.SerNumView(id);
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
			catch(Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";
			BC.UnbindDataFromObject<GridView>(this.gvOrders, this.gvItems, this.grdData);
			this.tbxDateOfDelivery.Text = string.Empty;
			this.tbxHeliosOrderID.Text = string.Empty;
			this.tbxQuantity.Text = string.Empty;        // 2015-02-18

			this.mvwMain.ActiveViewIndex = 1;

			string proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
				   " SELECT ID cValue, [Name] ctext FROM [dbo].[cdlStocks] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlStock, proSelect);
			this.ddlStock.SelectedValue = "2"; 
			this.ddlStock.Enabled = false;
			// ***************************************************************************************************************
			proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'---'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null) xx ORDER BY ctext";
			FillDdl(ref this.ddlItems, proSelect);
			// ***************************************************************************************************************
			proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
			" SELECT ID cValue,LEFT([CompanyName],100) ctext FROM [dbo].[cdlSuppliers] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlSupplier, proSelect);
			// ***************************************************************************************************************
			BaseHelper.FillDdlGroupGoods(ref this.ddlGroupGoods);
			// ***************************************************************************************************************
			BaseHelper.FillDdlItemType(ref this.ddlItemType);
			// ***************************************************************************************************************
			proSelect = "SELECT * FROM (SELECT '-1' cValue,' Vyberte ' ctext UNION ALL " +
						" SELECT  [Id] cValue,[SourceCode] ctext FROM [dbo].[cdlSources] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlSource, proSelect);
			this.ddlSource.SelectedValue = "1";
			
			this.btnSave.Enabled= true;
		}

		protected void ddlItems_TextChanged(object sender, EventArgs e)
		{
			string proSelect = "SELECT PC, MeasuresId  FROM [dbo].[cdlItems] WHERE ID = " + ddlItems.SelectedValue.ToString();
			try
			{
				DataTable myDataTable = BC.GetDataTable(proSelect, BC.FENIXRdrConnectionString);
				this.lblMeJeValue.Text = myDataTable.Rows[0][0].ToString();
				this.lblMeJeId.Text = myDataTable.Rows[0][1].ToString();
			}
			catch
			{
				this.lblMeJeId.Text = "";
			}
		}

		protected void btnPridatDoObjednavky_Click(object sender, EventArgs e)
		{
			DataTable myT = new DataTable("myDt");
			ZalozTabulku(ref myT);
			
			if (this.gvOrders != null && gvOrders.Rows.Count > 0)
			{
				foreach (GridViewRow gvr in gvOrders.Rows)
				{
					DataRow newDataRow = myT.NewRow();
					CheckBox myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					newDataRow[0] = (myChkb.Checked) ? 1 : 0;						// AnoNe
					newDataRow[1] = Convert.ToInt32(gvr.Cells[1].Text);			// StockId
					newDataRow[2] = HttpUtility.HtmlDecode(gvr.Cells[2].Text);
					newDataRow[3] = Convert.ToInt32(gvr.Cells[3].Text);			// ItemsId
					newDataRow[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);
					newDataRow[5] = Convert.ToDecimal(gvr.Cells[5].Text);
					newDataRow[6] = HttpUtility.HtmlDecode(gvr.Cells[6].Text);    // MeasuresId
					newDataRow[7] = HttpUtility.HtmlDecode(gvr.Cells[7].Text);
					newDataRow[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);	//DateOfDelivery
					newDataRow[9] = Convert.ToInt32(gvr.Cells[9].Text);
					newDataRow[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);	//Supplier
					newDataRow[11] = Convert.ToInt32(gvr.Cells[11].Text);			//SourceId
					newDataRow[12] = HttpUtility.HtmlDecode(gvr.Cells[12].Text);  //SourceCode
					newDataRow[13] = WConvertStringToInt32(gvr.Cells[13].Text);	//HeliosOrderId
					
					myT.Rows.Add(newDataRow);
				}
			}

			DataRow Dolx = myT.NewRow();
			Dolx[0] = 1;	 // AnoNe
			Dolx[1] = Convert.ToInt32(this.ddlStock.SelectedValue.ToString());    // StockId
			Dolx[2] = this.ddlStock.SelectedItem.Text;
			Dolx[3] = Convert.ToInt32(this.ddlItems.SelectedValue.ToString()); // ItemsId
			Dolx[4] = HttpUtility.HtmlDecode(this.ddlItems.SelectedItem.Text);
			Dolx[5] = Convert.ToDecimal(this.tbxQuantity.Text.Trim().Replace(".", ","));
			Dolx[6] = this.lblMeJeId.Text;
			Dolx[7] = this.lblMeJeValue.Text;
			Dolx[8] = this.tbxDateOfDelivery.Text;  //DateOfDelivery
			Dolx[9] = Convert.ToInt32(this.ddlSupplier.SelectedValue.ToString());
			Dolx[10] = this.ddlSupplier.SelectedItem.Text;  //Supplier
			Dolx[11] = Convert.ToInt32(this.ddlSource.SelectedValue.ToString());
			Dolx[12] = this.ddlSource.SelectedItem.Text;  //SourceCode
			Dolx[13] = WConvertStringToInt32(this.tbxHeliosOrderID.Text.Trim());  //HeliosOrderId

			myT.Rows.Add(Dolx);
			this.gvOrders.Columns[1].Visible = true; this.gvOrders.Columns[2].Visible = true; this.gvOrders.Columns[3].Visible = true; this.gvOrders.Columns[6].Visible = true;
			gvOrders.DataSource = myT.DefaultView;
			gvOrders.DataBind();
			this.gvOrders.Columns[1].Visible = false; this.gvOrders.Columns[2].Visible = false; this.gvOrders.Columns[3].Visible = false; this.gvOrders.Columns[6].Visible = false;

		}

		private void ZalozTabulku(ref DataTable myDt)
		{
			DataColumn myDataColumn;

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Boolean");
			myDataColumn.ColumnName = "AnoNe";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "StockId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "StockName";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ItemsId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "ItemsDescriptionCZ";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Decimal");
			myDataColumn.ColumnName = "ItemQuantity";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "MeasuresId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "MeasuresCode";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DateOfDelivery";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "SupplierId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "SupplierNazev";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "SourceId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "SourceCode";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "HeliosOrderId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

		} // ZalozTabulku

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)  // 2015-02-18
			{
				CheckUserAcces("");
			}

			if (((Button)sender).Text == "Uložit" && ((Button)sender).Enabled == true && (Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				DataTable myT = new DataTable("myDt");
				ZalozTabulku(ref myT);


				if (this.gvOrders != null && gvOrders.Rows.Count > 0)
				{
					foreach (GridViewRow gvr in gvOrders.Rows)
					{
						DataRow Doly = myT.NewRow();
						CheckBox myChkb;
						myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
						if (myChkb.Checked)
						{
							Doly[0] = (myChkb.Checked) ? 1 : 0;	 // AnoNe
							Doly[1] = Convert.ToInt32(gvr.Cells[1].Text);    // StockId
							Doly[2] = HttpUtility.HtmlDecode(gvr.Cells[2].Text);
							Doly[3] = Convert.ToInt32(gvr.Cells[3].Text); // ItemsId
							Doly[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);
							Doly[5] = Convert.ToDecimal(gvr.Cells[5].Text);
							Doly[6] = HttpUtility.HtmlDecode(gvr.Cells[6].Text); ;
							Doly[7] = HttpUtility.HtmlDecode(gvr.Cells[7].Text);
							Doly[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);  //DateOfDelivery
							Doly[9] = Convert.ToInt32(gvr.Cells[9].Text);
							Doly[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);  //Supplier
							Doly[11] = Convert.ToInt32(gvr.Cells[11].Text);  //SourceId
							Doly[12] = HttpUtility.HtmlDecode(gvr.Cells[12].Text);  //SourceCode
							Doly[13] = WConvertStringToInt32(gvr.Cells[13].Text);  //HeliosOrderId

							myT.Rows.Add(Doly);
						}
					}

					//using (XmlWriter writer = XmlWriter.Create(@"D:\TEMP\TESTmanually.xml"))
					//{
					bool mOK = true;
					CultureInfo culture = new CultureInfo("cs-CZ");
					StringBuilder sb = new StringBuilder();
					
					sb.Append("<NewDataSet>");
					foreach (DataRow r in myT.Rows)
					{
						if (r[0].ToString().ToUpper() == "TRUE")
						{
							sb.Append("<CommunicationMessagesOrdersSentmanually>");
							sb.Append("<OrderId>" + "" + "</OrderId>");
							sb.Append("<OrderDescription>" + "" + "</OrderDescription>");
							sb.Append("<CustomerId>" + r[9].ToString() + "</CustomerId>");                // <ItemSupplierID, int,>        
							sb.Append("<CustomerName>" + r[10].ToString() + "</CustomerName>");              // <ItemSupplierDescription, nvarchar(500),>      
							sb.Append("<CustomerAddress>" + "" + "</CustomerAddress>");
							sb.Append("<CustomerZipCode>" + "" + "</CustomerZipCode>");
							sb.Append("<CustomerCountry>" + "" + "</CustomerCountry>");
							sb.Append("<CustomerPhoneNumber>" + "" + "</CustomerPhoneNumber>");
							sb.Append("<ItemVerKit>" + "0" + "</ItemVerKit>");
							sb.Append("<ItemOrKitID>" + r[3].ToString() + "</ItemOrKitID>");
							sb.Append("<ItemOrKitDescription>" + r[4].ToString() + "</ItemOrKitDescription>");
							sb.Append("<ItemOrKitQuantity>" + r[5].ToString() + "</ItemOrKitQuantity>");
							sb.Append("<ItemOrKitUnitOfMeasureId>" + r[6].ToString() + "</ItemOrKitUnitOfMeasureId>");
							sb.Append("<ItemOrKitQuality>" + "1" + "</ItemOrKitQuality>");
							sb.Append("<ItemOrKitDateOfDelivery>" + (Convert.ToDateTime(r[8].ToString())).ToString("yyyyMMdd") + "</ItemOrKitDateOfDelivery>");  //--<ItemDateOfDelivery, datetime,>
							sb.Append("<ItemType>" + "" + "</ItemType>");
							sb.Append("<Incoterms>" + "" + "</Incoterms>");
							sb.Append("<PackageType>" + "" + "</PackageType>");
							sb.Append("<ModifyUserId>" + "0" + "</ModifyUserId>");
							sb.Append("<SourceId>" + r[11].ToString() + "</SourceId>");
							sb.Append("<HeliosOrderId>" + r[13].ToString() + "</HeliosOrderId>");
							sb.Append("</CommunicationMessagesOrdersSentmanually>");
						}
					}
					sb.Append("</NewDataSet>");
					
					string help = sb.ToString().Replace("{", "").Replace("}", "");
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(help);
					
					SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
					SqlCommand sqlComm = new SqlCommand();
					sqlComm.CommandType = CommandType.StoredProcedure;
					sqlComm.CommandText = "[dbo].[prCMSOSins]";
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;  
					sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());   //2015-02-18

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
					}
					finally
					{
						//if (xmOK == false) trans.Rollback();         23.3.2012
						conn.Close();
						conn = null;
						sqlComm = null;
						sb = null;
						doc = null;
					}
					if (mOK)
					{
						try
						{
							myT = null; this.gvOrders.DataSource = null; this.gvOrders.DataBind();
						}
						catch (Exception)
						{

							throw;
						}
						this.btnSave.Enabled = false;
						this.mvwMain.ActiveViewIndex = 0;
						this.search_button.Focus();
						this.fillPagerData(1);
						Session["IsRefresh"] = "1";
					}
				}
			}
			else 
			{
				if (Session["IsRefresh"].ToString() == "1") 
				{
					this.fillPagerData(1); 
					this.mvwMain.ActiveViewIndex = 0;
					this.gvOrders.DataSource = null; this.gvOrders.DataBind();
				}
			}
		}

		protected void chkbItems_CheckedChanged(object sender, EventArgs e)
		{
			this.lblMeJeValue.Text = "";
			this.ddlItems.Items.Clear();
			string proSelect;
			if (this.chkbItems.Checked)
			{
				proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT(LTRIM([DescriptionCz]),100)+' ('+[GroupGoods]+'-'+[Code]+')' ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null) xx ORDER BY ctext";
			}
			else
			{
				proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'-'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null) xx ORDER BY ctext";
			}
			try
			{
				FillDdl(ref ddlItems, proSelect);
			}
			catch
			{
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlItems.Items.Clear();
			this.ddlSupplier.Items.Clear();						
			BC.UnbindDataFromObject<GridView>(this.gvItems);
			this.fillPagerData(1);
		}

		protected void ddlGroupGoods_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblMeJeValue.Text = "";
			string proSelect = string.Empty;
			if (this.ddlGroupGoods.SelectedValue.ToString() == "-1")
				proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'---'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null) xx ORDER BY ctext";
			else
				proSelect = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'---'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null AND GroupGoods = {0}) xx ORDER BY ctext", this.ddlGroupGoods.SelectedValue.ToString());
			FillDdl(ref this.ddlItems, proSelect);
			this.ddlItemType.SelectedValue = "-1";
		}

		protected void ddlItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblMeJeValue.Text = "";
			string proSelect = string.Empty;
			if (this.ddlItemType.SelectedValue.ToString() == "-1")
				proSelect = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'---'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null) xx ORDER BY ctext";
			else
				proSelect = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([GroupGoods]+'-'+[Code]+'---'+[DescriptionCz],100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null AND ItemType = '{0}') xx ORDER BY ctext", this.ddlItemType.SelectedValue.ToString());
			FillDdl(ref this.ddlItems, proSelect);
			this.ddlGroupGoods.SelectedValue = "-1";
		}

		private void fullDdlItems()
		{
			StringBuilder sb = new StringBuilder();

			sb = null;
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.pnlR1.Visible = false;
			this.fillPagerData(1);
		}

		protected void nic(object sender, EventArgs e)
		{ 
		
		}

		/// <summary>
		/// Export vybrané 'R0 - objednávka nového zboží' do Excelu
		/// </summary>
		/// <param name="id"></param>
		protected void OrderView(int id)
		{
			try
			{
				bool mOK = true;
				bool mOKR = true;

				string proS = string.Format("SELECT [MessageId]  ,[MessageType] ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId] ,[HeliosOrderId] ,[ItemSupplierId] ,[ItemSupplierDescription]" +
											",[ItemDateOfDelivery]  ,[IsManually]  ,[StockId]  ,[Notice]  ,[RadaDokladu] ,[PoradoveCislo] ,[RadaPlusPorCislo] ,[IsActive] ,[ModifyDate]  ,[ModifyUserId]" +
											" FROM [dbo].[CommunicationMessagesReceptionSent] WHERE Id = {0} ORDER BY 1,2", id);
				
				DataTable dtObjHl = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHl == null || dtObjHl.Rows.Count < 1)
				{
					mOK = false;
				}

				proS = string.Format("SELECT [ID] ,[CMSOId] ,[HeliosOrderId] ,[HeliosOrderRecordId] ,[ItemId] ,[GroupGoods] ,[ItemCode] ,[ItemDescription] ,[ItemQuantity] ,[ItemQuantityDelivered]" +
									 ",[MeasuresID]  ,[ItemUnitOfMeasure] ,[ItemQualityId] ,[ItemQualityCode] ,[SourceId]  ,[IsActive]  ,[ModifyDate]  ,[ModifyUserId]" +
									 " ,[ItemQuantityInt] ,[ItemQuantityDeliveredInt]  FROM [dbo].[vwCMRSentIt] WHERE CMSOId={0}", id);

				DataTable dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjR == null || dtObjR.Rows.Count < 1)
				{
					mOK = false;
				}

				proS = string.Format("SELECT [ID] ,[MessageId] ,[MessageTypeId] ,[MessageDescription] ,[MessageDateOfReceipt] ,[CommunicationMessagesSentId] ,[ItemSupplierId]" +
									 ",[ItemSupplierDescription] ,[Reconciliation] ,[IsActive] ,[ModifyDate] ,[ModifyUserId]" +
									 " FROM [dbo].[CommunicationMessagesReceptionConfirmation] where CommunicationMessagesSentId={0} AND Reconciliation<>0", id);

				DataTable dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
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
							//worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["MessageDateOfShipment"].ToString();    // MessageDateOfShipment								
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["MessageDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS, true);							
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
							//worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["ItemDateOfDelivery"].ToString();
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["ItemDateOfDelivery"], BC.DATE_TIME_FORMAT_DDMMYYY, true);							
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
									//{
									//	worksheet.Cells[radek, 4].Value = String.Format("Schváleno");
									//}
									BC.ExcelProcessReconciliation(worksheet.Cells[radek, 4], dr["Reconciliation"].ToString());

									//worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
									BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dtObjHl.Rows[0]["MessageDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS); 									
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
												string[] serialNumbers = drc["ItemSNs"].ToString().Split(',');
												foreach (var serialNumber in serialNumbers)
												{
													//worksheet.Cells[radek, 1].Value = e.ToString();
													worksheet.Cells[radek, 1].Value = BC.ExcelPrepareSerialNumber(serialNumber);
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

							xls.Workbook.Properties.Title = "R0 - objednávka nového zboží";
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
							Response.AddHeader("content-disposition", "attachment;filename=R0_Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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