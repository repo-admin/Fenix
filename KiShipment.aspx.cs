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
using FenixHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// [Expedice] S0 - Objednávka expedice (Shipment Order)
	/// </summary>
	public partial class KiShipment : BasePage
	{
		#region Properties
				
		//seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		private int[] hideGrdDataColumns = new int[] { 4, 16, 19 };

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
				ShipmentHelper.FillDdlCompanyName(ref this.ddlCompanyName);
				ShipmentHelper.FillDdlCity(ref this.ddlCityName);
				ShipmentHelper.FillDdlMessageStatus(ref this.ddlMessageStatusFlt);
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
				this.grdData.SelectedIndex = -1;
				this.grdPager.CurrentIndex = currPageIndx;
				BC.UnbindDataFromObject(this.grdData);
				this.fillPagerData(currPageIndx);
			}
		}

		private void fillPagerData(int pageNo)
		{			
			this.pnlGvItems.Visible = false;
			BC.UnbindDataFromObject(this.gvItems);

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwCMSOsent]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "*";
			pagerData.WhereClause = this.createWhereClause();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref this.grdData, this.hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.prepareImagesInShipmentOrderList();
				BaseHelper.SetPictureDeleteOrder(ref grdData, 19, "6");
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
				this.grdPager.Visible = false; 
				this.grdData.Visible = false;
			}
		}

		private string createWhereClause()
		{
			string proW = " IsActive = 1 ";

			if (this.ddlCompanyName.SelectedValue != "-1") 
				proW += " AND [CustomerName] like '" + this.ddlCompanyName.SelectedItem.Text.Trim() + "%'";
			
			if (this.ddlCityName.SelectedValue != "-1")
				proW += " AND [CustomerCity] like '" + this.ddlCityName.SelectedItem.Text.Trim() + "%'";
			
			if (this.ddlOrderType.SelectedValue != "-1") 
				proW += " AND [OrderTypeID] = " + this.ddlOrderType.SelectedValue;
			
			if (!string.IsNullOrWhiteSpace(this.tbxDatumOdeslaniFlt.Text.Trim()))
				proW += " AND CONVERT(CHAR(8),[MessageDateOfShipment],112) = '" + base.WConvertDateToYYYYmmDD(this.tbxDatumOdeslaniFlt.Text.Trim()) + "'";
			
			if (!string.IsNullOrWhiteSpace(this.tbxDatumDodaniFlt.Text.Trim()))
				proW += " AND CONVERT(CHAR(8),[RequiredDateOfShipment],112) = '" + base.WConvertDateToYYYYmmDD(this.tbxDatumDodaniFlt.Text.Trim()) + "'";
			
			if (this.ddlMessageStatusFlt.SelectedValue != "-1")
				proW += " AND [MessageStatusId] = " + this.ddlMessageStatusFlt.SelectedValue;
			
			if (this.ddlUsersModifyFlt.SelectedValue != "-1")
				proW += " AND [ModifyUserId] = " + this.ddlUsersModifyFlt.SelectedValue;
			
			if (!string.IsNullOrWhiteSpace(this.tbxObjednavkaIDFlt.Text))
				proW += " AND ID = " + this.tbxObjednavkaIDFlt.Text.Trim();

			return proW;
		}
		
		private void prepareImagesInShipmentOrderList()
		{
			// Zamítnuto
			const string REJECT_DECISION = "2";

			ImageButton img = new ImageButton();
			foreach (GridViewRow gvr in this.grdData.Rows)
			{
				// Reconciliation
				if (gvr.Cells[16].Text == REJECT_DECISION)
				{
					img = (ImageButton)gvr.FindControl("btnS1new");
					img.Enabled = true;
					img.Visible = true;
				}
				else
				{
					img = (ImageButton)gvr.FindControl("btnS1new");
					img.Enabled = false;
					img.Visible = false;
				}
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{						
			try
			{
				string proS = string.Format("SELECT [ID], SingleOrMaster, " +
					                 "ItemVerKit, [ItemOrKitID], [ItemOrKitDescription], [ItemOrKitUnitOfMeasure], " +
				                     "[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt " +
						             "FROM [dbo].[vwShipmentOrderIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, grdData.SelectedValue);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvItems.DataSource = myDataTable.DefaultView; 
				this.gvItems.DataBind();
				this.gvItems.Visible = true;
				this.gvItems.SelectedIndex = -1;
				this.setTxbRemark();
				this.pnlGvItems.Visible = true;
			}
			catch
			{
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(1);
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";
			ClearViewControls(vwEdit);
			this.tbxKitsQuantity.Text = "1";
			this.lblErrInfo.Text = "";
			BC.UnbindDataFromObject(this.grdData, this.gvKitsOrItemsNew);

			this.mvwMain.ActiveViewIndex = 1;
			
			string proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
			" SELECT ID cValue, [Name] ctext FROM [dbo].[cdlStocks] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlStock, proS);
			this.ddlStock.SelectedValue = "2";
			// ***************************************************************************************************************
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
			" SELECT ID cValue, [CompanyName] ctext FROM [dbo].[cdlDestinationPlaces] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlDestinationPlaces, proS);
			// ***************************************************************************************************************
			BaseHelper.FillDdlGroupGoods(ref this.ddlGroupGoods);
			// ***************************************************************************************************************
			BaseHelper.FillDdlItemType(ref this.ddlItemType);
			// ***************************************************************************************************************
			proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);
			FillDdl(ref this.ddlKits, proS);
			// ***************************************************************************************************************
			proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);
			FillDdl(ref this.ddlNW, proS);

			//proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
			//	  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlMeasures] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			//FillDdl(ref this.ddlMeasures, proS);

			// ***************************************************************************************************************
			BaseHelper.FillDdlQualities(ref this.ddlKitQualities);
			// ***************************************************************************************************************
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlKitGroups] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlKitGroups, proS);
			// ***************************************************************************************************************
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				  " SELECT ID cValue,[DescriptionCz] ctext FROM [dbo].[cdlIncoterms] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlIncoterms, proS);
			this.ddlIncoterms.SelectedValue = "2";
		}

		protected void btnPridatCpeDoSoupravy_Click(object sender, EventArgs e)
		{
			addRec("KIT");
		}

		protected void btnPridatNwDoSoupravy_Click(object sender, EventArgs e)
		{
			addRec("NW");
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				bool mOk = true;
				this.lblErrInfo.Text = String.Empty;
				string errMessage = String.Empty;
				//if (this.ddlKitQualities.SelectedValue.ToString() == "-1" || this.ddlKitQualities == null) { Err += "Chybný výběr kvality Kitu <br />"; mOk = false; }
				//if (this.ddlMeasures.SelectedValue.ToString() == "-1" || this.ddlMeasures == null) { Err += "Chybný výběr měrné jednotky Kitu <br />"; mOk = false; }
				//if (string.IsNullOrWhiteSpace(this.tbxDescriptionCz.Text.Trim())) { Err += "Český popis Kitu je povinný údaj <br />"; mOk = false; }
				
				DataTable myT = new DataTable("myDt");
				ZalozTabulku(ref myT);
				
				if (ShipmentHelper.GridViewHasRows(gvKitsOrItemsNew, "CheckBoxR"))
				{
					foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
					{
						DataRow dataRow = myT.NewRow();
						
						CheckBox myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
						dataRow[0] = (myChkb.Checked) ? 1 : 0;	                    // AnoNe
						dataRow[1] = WConvertStringToInt32(gvr.Cells[1].Text);     // ID    
						dataRow[2] = WConvertStringToInt32(gvr.Cells[2].Text);     // ItemVerKit
						dataRow[3] = WConvertStringToInt32(gvr.Cells[3].Text);     // ItemOrKitID
						dataRow[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);    // ItemOrKitCode
						dataRow[5] = HttpUtility.HtmlDecode(gvr.Cells[5].Text);    // DescriptionCzItemsOrKit
						dataRow[6] = Convert.ToDecimal(gvr.Cells[6].Text);         // ItemOrKitQuantity
						dataRow[7] = WConvertStringToInt32(gvr.Cells[7].Text);     // PackageTypeId
						dataRow[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);    // cdlStocksName
						dataRow[9] = WConvertStringToInt32(gvr.Cells[9].Text);     // DestinationPlacesId
						dataRow[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);  // DestinationPlacesName
						dataRow[11] = WConvertStringToInt32(gvr.Cells[11].Text);   // DestinationPlacesContactsId
						dataRow[12] = HttpUtility.HtmlDecode(gvr.Cells[12].Text);  // DestinationPlacesContactsName
						dataRow[13] = HttpUtility.HtmlDecode(gvr.Cells[13].Text);  // DateOfExpedition
						dataRow[14] = HttpUtility.HtmlDecode(gvr.Cells[14].Text);  // IncotermsId

						myT.Rows.Add(dataRow);
					}
				}
				else
				{
					errMessage += "Obsahuje objednávka expedice nějaké položky? <br />"; mOk = false;
				}
								
				if (mOk)
				{
					if (ShipmentHelper.CheckAllRows(gvKitsOrItemsNew, "CheckBoxR") == false)				
					{
						errMessage += "Objednávka expedice obsahuje duplicitní materiál/kit<br />"; mOk = false;						
					}
				}

				if (mOk)
				{
					mOk = false;
					StringBuilder sb = new StringBuilder();
					sb.Append("<NewDataSet>");
					foreach (DataRow r in myT.Rows)
					{
						if (r[0].ToString().ToUpper() == "TRUE")
						{
							sb.Append("<Expedice>");
							sb.Append("<ID>" + r[1] + "</ID>");     // ID z tabulky [dbo].[CardStockItems]
							sb.Append("<ItemVerKit>" + r[2] + "</ItemVerKit>");
							sb.Append("<ItemOrKitID>" + r[3] + "</ItemOrKitID>");
							sb.Append("<ItemOrKitCode>" + r[4] + "</ItemOrKitCode>");
							sb.Append("<DescriptionCzItemsOrKit>" + r[5] + "</DescriptionCzItemsOrKit>");
							sb.Append("<ItemOrKitQuantity>" + r[6] + "</ItemOrKitQuantity>");
							sb.Append("<PackageTypeId>" + r[7] + "</PackageTypeId>");
							sb.Append("<cdlStocksName>" + r[8] + "</cdlStocksName>");
							sb.Append("<DestinationPlacesId>" + r[9] + "</DestinationPlacesId>");
							sb.Append("<DestinationPlacesName>" + r[10] + "</DestinationPlacesName>");
							sb.Append("<DestinationPlacesContactsId>" + r[11] + "</DestinationPlacesContactsId>");
							sb.Append("<DestinationPlacesContactsName>" + r[12] + "</DestinationPlacesContactsName>");
							sb.Append("<DateOfExpedition>" + r[13] + "</DateOfExpedition>");
							sb.Append("<IncotermsId>" + r[14] + "</IncotermsId>");
							sb.Append("</Expedice>");
						}
					}
					sb.Append("</NewDataSet>");
					
					string help = sb.ToString().Replace("{", "").Replace("}", "");
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(help);

					SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
					SqlCommand sqlComm = new SqlCommand();
					sqlComm.CommandType = CommandType.StoredProcedure;
					sqlComm.CommandText = "[dbo].[prKiSHins]";
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
					sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString());
					sqlComm.Parameters.Add("@Remark", SqlDbType.NVarChar, 4000).Value = this.tbxRemarkNew.Text.Trim();
					sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
					sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
					sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
					sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

					try
					{
						conn.Open();
						sqlComm.ExecuteNonQuery();
						if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
						{
							mOk = true;
						}

						Int32 i = WConvertStringToInt32(sqlComm.Parameters["@ReturnValue"].Value.ToString());
						if (i == -1 || i != 0) { errMessage += "Záznam <b>nebyl</b> uložen! <br />" + WConvertStringToInt32(sqlComm.Parameters["@ReturnMessage"].Value.ToString()); ; mOk = false; }

					}
					catch (Exception ex)
					{
						BC.ProcessException(ex, AppLog.GetMethodName());
						mOk = false;

						errMessage += "Záznam <b>nebyl</b> uložen! <br />";
					}
					finally
					{
						conn.Close();
						conn = null;
						sqlComm = null;
					}
				}
				else
				{
					this.lblErrInfo.Text = errMessage;
				}
				if (mOk)
				{
					Session["IsRefresh"] = "1";
					btnBack_Click(btnBack, EventArgs.Empty);
					// TODO : odeslani info mailu
				}
			}
			else
			{
				if (Session["IsRefresh"].ToString() == "1")
				{
					btnBack_Click(btnBack, EventArgs.Empty);
				}
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlNW.Items.Clear();
			this.ddlKits.Items.Clear();
			BC.UnbindDataFromObject(this.gvKitsOrItemsNew);						
			this.grdData.SelectedIndex = -1;
			this.setRemarkNewVisibility(false);
			this.fillPagerData(1);
		}

		private void ZalozTabulku(ref DataTable myDt)
		{
			DataColumn myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Boolean");
			myDataColumn.ColumnName = "AnoNe";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "Id"; // ID z CardStockItems
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
			myDataColumn.ColumnName = "ItemOrKitCode";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DescriptionCzItemsOrKit";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Decimal");
			myDataColumn.ColumnName = "ItemOrKitQuantity";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "PackageTypeId";   // 1... Master, 2...Single   ???   2014026
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "cdlStocksName";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "DestinationPlacesId";   // destinationPlaces
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DestinationPlacesName";   // destinationPlaces
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "DestinationPlacesContactsId";   // DestinationPlacesContacts
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DestinationPlacesContactsName";   // DestinationPlacesContacts ContactName
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DateOfExpedition";                // DateOfExpedition
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "IncotermsId";                    // ddlIncoterms
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);
		} // ZalozTabulku

		private void addRec(string par1)
		{
			string Err = string.Empty;
			this.lblErrInfo.Text = string.Empty;

			if (this.ddlStock.SelectedValue != "-1" && this.ddlDestinationPlaces.SelectedValue != "-1" && this.tbxDateOfExpedition.Text.Trim() != string.Empty && this.ddlIncoterms.SelectedValue != "-1")
			{
				if (this.ddlKits.SelectedValue != "-1" && par1 == "KIT" || this.ddlNW.SelectedValue != "-1" && par1 != "KIT")
				{
					//kontrola unikátnosti přidávaného zboží (NW/KIT)
					if (ShipmentHelper.GoodIsUnique(gvKitsOrItemsNew, ddlKits, ddlNW, "CheckBoxR", par1) == false)
					{
						this.lblErrInfo.Text = "V objednávce již existuje přidávaný materiál/KIT<br />"; 
						return;
					}

					DataTable myT = new DataTable("myDt");
					ZalozTabulku(ref myT);

					if (this.gvKitsOrItemsNew != null && gvKitsOrItemsNew.Rows.Count > 0)
					{
						foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
						{
							DataRow dataRow = myT.NewRow();
							CheckBox myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
							dataRow[0] = (myChkb.Checked) ? 1 : 0;	                    // AnoNe
							dataRow[1] = WConvertStringToInt32(gvr.Cells[1].Text);     // ID    
							dataRow[2] = WConvertStringToInt32(gvr.Cells[2].Text);     // ItemVerKit
							dataRow[3] = WConvertStringToInt32(gvr.Cells[3].Text);     // ItemOrKitID
							dataRow[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);    // ItemOrKitCode
							dataRow[5] = HttpUtility.HtmlDecode(gvr.Cells[5].Text);    // DescriptionCzItemsOrKit
							dataRow[6] = Convert.ToDecimal(gvr.Cells[6].Text);         // ItemOrKitQuantity
							dataRow[7] = WConvertStringToInt32(gvr.Cells[7].Text);     // PackageTypeId
							dataRow[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);    // cdlStocksName
							dataRow[9] = WConvertStringToInt32(gvr.Cells[9].Text);     // DestinationPlacesId
							dataRow[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);  // DestinationPlacesName
							dataRow[11] = WConvertStringToInt32(gvr.Cells[11].Text);   // DestinationPlacesContactsId
							dataRow[12] = HttpUtility.HtmlDecode(gvr.Cells[12].Text);  // DestinationPlacesContactsName
							dataRow[13] = HttpUtility.HtmlDecode(gvr.Cells[13].Text);  // DateOfExpedition
							dataRow[14] = HttpUtility.HtmlDecode(gvr.Cells[14].Text);  // Incoterms

							myT.Rows.Add(dataRow);
						}
					}

					string proS = string.Empty;
					if (par1 == "KIT")
					{
						proS = "SELECT ItemVerKit,[ItemOrKitID],cdlKitsCode, DescriptionCz, cdlStocksName, ItemOrKitReleasedForExpedition, ID AS CardStockItemsID  FROM [dbo].[vwCardStockItems] WHERE IsActive = 1 AND [ItemVerKit] = 1 AND id=" + this.ddlKits.SelectedValue;
					}
					else
					{
						proS = "SELECT ItemVerKit,[ItemOrKitID],Code,[DescriptionCz],cdlStocksName,ItemOrKitFree, ID AS CardStockItemsID FROM [dbo].[vwCardStockItems] WHERE IsActive = 1 AND [ItemVerKit] = 0 AND id=" + this.ddlNW.SelectedValue;
					}
					DataTable myDataTable = BC.GetDataTable(proS);
					if (myDataTable != null && myDataTable.Rows.Count == 1)
					{

						// **********************************************************************************************
						decimal sumaExpedicePozadovana = 0;
						Boolean bHelp = false;
						foreach (DataRow dr in myT.Rows)
						{
							bHelp = false;
							if (dr["ItemVerKit"].ToString() == "1")
								bHelp = true;
							if (dr["ItemOrKitID"].ToString() == myDataTable.Rows[0][1].ToString() && bHelp == Convert.ToBoolean(myDataTable.Rows[0][0]))
							{
								sumaExpedicePozadovana += Convert.ToDecimal(dr["ItemOrKitQuantity"].ToString());
							}
						}

						if (par1 == "KIT")                                                    // ItemOrKitQuantity
							sumaExpedicePozadovana += Convert.ToDecimal(this.tbxKitsQuantity.Text.Trim().Replace(".", ","));
						else
							sumaExpedicePozadovana += Convert.ToDecimal(this.tbxNwQuantity.Text.Trim().Replace(".", ","));
						// **********************************************************************************************

						if (sumaExpedicePozadovana <= Convert.ToDecimal(myDataTable.Rows[0][5].ToString().Replace(".", ",")))
						{

							DataRow dataRow = myT.NewRow();
							dataRow[0] = 1;	                                                      // AnoNe
							if (par1 == "KIT")                                                    // ItemOrKitQuantity
								dataRow[1] = WConvertStringToInt32(this.ddlKits.SelectedValue);   // ID
							else
								dataRow[1] = WConvertStringToInt32(this.ddlNW.SelectedValue);     // ID
							dataRow[2] = Convert.ToInt32(Convert.ToBoolean(myDataTable.Rows[0][0]));                  // ItemVerKit
							dataRow[3] = WConvertStringToInt32(myDataTable.Rows[0][1].ToString());   // ItemOrKitID
							dataRow[4] = myDataTable.Rows[0][2].ToString();                          // ItemOrKitCode
							dataRow[5] = myDataTable.Rows[0][3].ToString();                          // DescriptionCzItemsOrKit
							if (par1 == "KIT")                                                    // ItemOrKitQuantity
								dataRow[6] = Convert.ToDecimal(this.tbxKitsQuantity.Text.Trim().Replace(".", ","));
							else
								dataRow[6] = Convert.ToDecimal(this.tbxNwQuantity.Text.Trim().Replace(".", ","));
							dataRow[7] = 0;                                                          // PackageTypeId
							dataRow[8] = myDataTable.Rows[0][4].ToString();                          // cdlStocksName
							dataRow[9] = WConvertStringToInt32(this.ddlDestinationPlaces.SelectedValue);   // DestinationPlacesId
							dataRow[10] = this.ddlDestinationPlaces.SelectedItem.Text;                      // DestinationPlacesName
							dataRow[11] = WConvertStringToInt32(this.ddlDestinationPlacesContacts.SelectedValue);  // DestinationPlacesContactsId
							dataRow[12] = this.ddlDestinationPlacesContacts.SelectedItem.Text;                     // DestinationPlacesContactsName
							dataRow[13] = WConvertDateToYYYYmmDD(this.tbxDateOfExpedition.Text.Trim());            // DateOfExpedition
							dataRow[14] = WConvertStringToInt32(this.ddlIncoterms.SelectedValue);                  // IncotermsId

							myT.Rows.Add(dataRow);

							this.gvKitsOrItemsNew.Columns[7].Visible = true; this.gvKitsOrItemsNew.Columns[9].Visible = true; this.gvKitsOrItemsNew.Columns[11].Visible = true;
							gvKitsOrItemsNew.DataSource = myT.DefaultView;
							gvKitsOrItemsNew.DataBind();
							this.gvKitsOrItemsNew.Columns[7].Visible = false; this.gvKitsOrItemsNew.Columns[9].Visible = false; this.gvKitsOrItemsNew.Columns[11].Visible = false;
							this.setRemarkNewVisibility(true);
						}
						else
						{
							Err += "Požadované množství <b>" + sumaExpedicePozadovana + "</b> přesahuje disponibilní množství <b>" + myDataTable.Rows[0][5].ToString().Replace(".", ",") + "</b><br />";
						}
					}

				}
				else
				{
					Err += "Vyberte kit nebo item<br />";
				}
			}
			else
			{
				Err += "Zkontrolujte požadované datum a cílové místo závozu<br />";
			}

			this.lblErrInfo.Text = Err;
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			//this.pnlKiItems.Visible = false;
			this.grdData.SelectedIndex = -1;
			//this.gvKiItems.DataSource = null; this.gvKiItems.DataBind();

			this.fillPagerData(1);
		}

		protected void ddlItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			if (this.ddlItemType.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);
			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0} AND ItemType ='{1}') xx ORDER BY ctext", this.ddlStock.SelectedValue, this.ddlItemType.SelectedValue);
			}
			FillDdl(ref this.ddlNW, proS);
			this.ddlGroupGoods.SelectedValue = "-1";
		}

		protected void ddlGroupGoods_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			if (this.ddlGroupGoods.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);

			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0} AND GroupGoods ='{1}') xx ORDER BY ctext", this.ddlStock.SelectedValue, this.ddlGroupGoods.SelectedItem.Text);
			}
			FillDdl(ref this.ddlNW, proS);
			this.ddlItemType.SelectedValue = "-1";
		}

		protected void ddlKitQualities_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			// ***************************************************************************************************************
			if (this.ddlKitQualities.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);

			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0} AND ItemOrKitQuality = {1}) xx ORDER BY ctext", this.ddlStock.SelectedValue, this.ddlKitQualities.SelectedValue);
			}
			FillDdl(ref this.ddlKits, proS);
			this.ddlKitGroups.SelectedValue = "-1";
		}

		protected void ddlKitGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			// ***************************************************************************************************************
			if (this.ddlKitGroups.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);

			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0} AND GroupsId = {1}) xx ORDER BY ctext", this.ddlStock.SelectedValue, this.ddlKitGroups.SelectedValue);

			}
			FillDdl(ref this.ddlKits, proS);
			this.ddlKitQualities.SelectedValue = "-1";
		}

		protected void ddlDestinationPlaces_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				  " SELECT ID cValue,[ContactName] ctext FROM [dbo].[cdlDestinationPlacesContacts] WHERE [IsActive]=1 AND [DestinationPlacesId]={0} ) xx ORDER BY ctext", this.ddlDestinationPlaces.SelectedValue);
			FillDdl(ref this.ddlDestinationPlacesContacts, proS);
			if (this.ddlDestinationPlacesContacts.Items.Count == 2) this.ddlDestinationPlacesContacts.SelectedIndex = 1;
		}

		protected void ddlStock_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			// ***************************************************************************************************************
			proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=1 AND ItemOrKitReleasedForExpedition>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);
			FillDdl(ref this.ddlKits, proS);
			this.ddlKitQualities.SelectedValue = "-1";
			this.ddlKitGroups.SelectedValue = "-1";

			proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
			" SELECT ID cValue,LEFT([DescriptionCz]+' ('+CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext FROM [dbo].[vwCardStockItems] WHERE [IsActive]=1 AND ItemVerKit=0 AND ItemOrKitFree>0 AND cdlStocksID = {0}) xx ORDER BY ctext", this.ddlStock.SelectedValue);
			FillDdl(ref this.ddlNW, proS);
			this.ddlItemType.SelectedValue = "-1";
			this.ddlGroupGoods.SelectedValue = "-1";
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "OrderView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				OrderView(id);
			}

			if (e.CommandName == "S1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();
				this.pnlGvItems.Visible = false; this.gvItems.DataSource = null; this.gvItems.DataBind();
				this.pnlR1.Visible = true;
				Session["IsRefresh"] = "0";
				S1New(id);
			}

			if (e.CommandName == "btnExcelConfClicked")
			{
				ExcelConfClicked();
			}

			if (e.CommandName == "btnOznacit")
			{
				foreach (GridViewRow r in grdData.Rows) {
					CheckBox ckb = (CheckBox)r.FindControl("chkbExcel");
					ckb.Checked = !ckb.Checked;
				}
			}

			if (e.CommandName == "DeleteOrder")
			{
				string confirmValue = Request.Form["confirm_delete_order"];
				if (confirmValue.ToUpper() == "ANO")
				{
					BaseHelper.ProcessDeleteOrder(e, ref grdData, "6", "ShipmentOrder", Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString()));
				}
			}
		}

		protected void OrderView(int id)
		{
			try
			{
				bool mOK = true;
				bool mOKR = true;

				string proS = string.Format(" SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfShipment],[RequiredDateOfShipment],[MessageStatusId],[HeliosOrderId] " +
											" ,[CustomerID],[CustomerName],[CustomerAddress1],[CustomerAddress2],[CustomerAddress3],[CustomerCity],[CustomerZipCode],[CustomerCountryISO]     " +
											" ,[ContactID],[ContactTitle],[ContactFirstName],[ContactLastName],[ContactPhoneNumber1],[ContactPhoneNumber2],[ContactFaxNumber],[ContactEmail]  " +
											" ,[IsManually],[StockId],[IsActive],[ModifyDate],[ModifyUserId],[Remark] " +
											" FROM [dbo].[CommunicationMessagesShipmentOrdersSent]  WHERE Id = {0} ORDER BY 1,2", id);				
				DataTable dtObjHl = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHl == null || dtObjHl.Rows.Count < 1) { mOK = false; }

				proS = string.Format(" SELECT [ID],[CMSOId],[SingleOrMaster],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityReal],[ItemOrKitQuantityInt] " +
									 " ,[ItemOrKitQuantityRealInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId],[ItemOrKitQualityCode],[ItemType],[IncotermsId]" +
									 " ,[Incoterms],[PackageValue],[ShipmentOrderSource],[VydejkyId],[CardStockItemsId],[HeliosOrderRecordId],[IsActive],[ModifyDate],[ModifyUserId]" +
									 "   FROM [dbo].[vwShipmentOrderIt] WHERE CMSOId={0}", id);				
				DataTable dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjR == null || dtObjR.Rows.Count < 1) { mOK = false; }

				proS = string.Format(" SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[ShipmentOrderID],[Reconciliation],[ReconciliationYesNo]" +
									 " ,[MessageDateOfShipment],[RequiredDateOfShipment],[IsActive],ModifyDate " +
									 "  FROM [dbo].[vwShipmentConfirmationHd] where [ShipmentOrderID]={0} AND Reconciliation<>0", id);				
				DataTable dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHlCon == null || dtObjHlCon.Rows.Count < 1) { mOKR = false; }

				DataTable dtObjHlRCon = new DataTable();
				if (mOKR)
				{
					string ids = "-99";
					foreach (DataRow dr in dtObjHlCon.Rows)
					{
						ids += "," + dr[0];
					}

					proS = string.Format(" SELECT [ID],[CMSOId],[SingleOrMaster],[HeliosOrderRecordID],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[CMRSIItemQuantity]      " +
										 " ,[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId],[ItemOrKitQualityCode],[IncotermsId],[IncotermDescription]   " +
										 " ,[RealDateOfDelivery],[RealItemOrKitQuantity],[RealItemOrKitQualityID],[RealItemOrKitQuality],[Status],[KitSNs],[IsActive],[ModifyDate]" +
										 " ,[ModifyUserId],[Code],[CommunicationMessagesSentId],[ItemOrKitQuantityReal],[CardStockItemsId],[VydejkyId],[ShipmentOrderSource],RealItemOrKitQuantityInt      " +
										 "  FROM [dbo].[vwShipmentConfirmationIt] where [CMSOId] in ({0})", ids);
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
						worksheet.Cells["A1:B20000"].Style.Numberformat.Format = @"@";

						try
						{
							#region

							int radek = 1;
							// nadpis
							worksheet.Row(1).Height = 24;
							worksheet.Cells[radek, 1, radek, 9].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("S0 - Objednávka - Shipment");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							
							radek += 2;
							// hlavicka objednavky
							worksheet.Cells[radek, 1].Value = String.Format("Message ID");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0][1].ToString();  // MessageID
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Message Popis");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["MessageDescription"].ToString(); 
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Datum odeslání");							
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 6], dtObjHl.Rows[0]["MessageDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS, true);								                          
							worksheet.Cells[radek, 7].Value = String.Format("Datum požadované");							
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["RequiredDateOfShipment"], BC.DATE_TIME_FORMAT_DDMMYYY, true);							
							worksheet.Cells[radek, 9].Value = String.Format("Poznámka");
							//worksheet.Cells[radek, 9].Style.Font.Bold = true;
							worksheet.Cells[radek, 10].Value = dtObjHl.Rows[0]["Remark"].ToString(); 
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							
							radek += 1;
							worksheet.Cells[radek, 1].Value = String.Format("Cílová firma");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0]["CustomerName"].ToString();
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Město");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["CustomerCity"].ToString();
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Adresa");
							worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["CustomerAddress1"] + " - " + dtObjHl.Rows[0]["CustomerAddress2"] + " " + dtObjHl.Rows[0]["CustomerAddress3"] + "," + dtObjHl.Rows[0]["ContactPhoneNumber2"];  // ContactPhoneNumber1
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("PSČ");
							worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["CustomerZipCode"].ToString();
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 10].Value = String.Format("");
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							
							radek += 1;
							worksheet.Cells[radek, 1].Value = String.Format("Kontakt jméno");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0]["ContactFirstName"].ToString();
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Kontakt příjmení");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0]["ContactLastName"].ToString();  
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Kontakt tel.");
							worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["ContactPhoneNumber1"] + "," + dtObjHl.Rows[0]["ContactPhoneNumber2"];  
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Kontakt mail");
							worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["ContactEmail"].ToString();    
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 9].Style.Font.Bold = true;
							worksheet.Cells[radek, 10].Value = String.Format("");
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							
							radek += 2;
							// detaily objednávky
							worksheet.Cells[radek, 1].Value = String.Format("SingleOrMaster");
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 2].Value = String.Format("ItemVerKit");
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
								worksheet.Cells[radek, 1].Value = dr["SingleOrMaster"].ToString();
								worksheet.Cells[radek, 2].Value = dr["ItemVerKit"].ToString();
								worksheet.Cells[radek, 3].Value = dr["ItemOrKitID"].ToString();
								worksheet.Cells[radek, 4].Value = dr["ItemOrKitDescription"].ToString();
								worksheet.Cells[radek, 5].Value = dr["ItemOrKitQuantityInt"].ToString();
								worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 6].Value = dr["ItemOrKitQuantityRealInt"].ToString();
								worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 7].Value = dr["ItemOrKitUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 8].Value = dr["ItemOrKitQualityCode"].ToString();
								worksheet.Cells[radek, 9].Value = String.Format("");
								worksheet.Cells[radek, 10].Value = String.Format("");
								radek += 1;
							}
							#endregion
							if (mOKR)
							{
								radek += 2;
								// hlavička konfirmace
								worksheet.Cells[radek, 1, radek, 8].Merge = true;
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.Size = 14;
								worksheet.Cells[radek, 1].Value = String.Format("S1 - Konfirmace objednávky");
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
									worksheet.Cells[radek, 3].Value = String.Format("");
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
									worksheet.Cells[radek, 3].Value = String.Format("");

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

									BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dr["ModifyDate"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS);									
									worksheet.Cells[radek, 6].Value = dr["IsActive"].ToString();
									worksheet.Cells[radek, 7].Value = String.Format("");
									worksheet.Cells[radek, 8].Value = String.Format("");
									worksheet.Cells[radek, 9].Value = String.Format("");
									worksheet.Cells[radek, 10].Value = String.Format("");

									radek += 2;
									worksheet.Cells[radek, 1].Value = String.Format("SingleOrMaster");
									worksheet.Cells[radek, 1].Style.Font.Bold = true;
									worksheet.Cells[radek, 2].Value = String.Format("ItemVerKit");
									worksheet.Cells[radek, 2].Style.Font.Bold = true;
									worksheet.Cells[radek, 3].Value = String.Format("Kód");
									worksheet.Cells[radek, 3].Style.Font.Bold = true;
									worksheet.Cells[radek, 4].Value = String.Format("Popis");
									worksheet.Cells[radek, 4].Style.Font.Bold = true;
									worksheet.Cells[radek, 5].Value = String.Format("Množství");
									worksheet.Cells[radek, 5].Style.Font.Bold = true;
									worksheet.Cells[radek, 6].Value = String.Format("MJ");
									worksheet.Cells[radek, 6].Style.Font.Bold = true;
									worksheet.Cells[radek, 7].Value = String.Format("Kvalita");
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
											worksheet.Cells[radek, 1].Value = drc["SingleOrMaster"].ToString();
											worksheet.Cells[radek, 2].Value = drc["ItemVerKit"].ToString();
											worksheet.Cells[radek, 3].Value = drc["ItemOrKitID"].ToString();
											worksheet.Cells[radek, 4].Value = drc["ItemOrKitDescription"].ToString();
											worksheet.Cells[radek, 5].Value = drc["RealItemOrKitQuantityInt"].ToString();
											worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
											worksheet.Cells[radek, 6].Value = drc["ItemOrKitUnitOfMeasure"].ToString();
											worksheet.Cells[radek, 7].Value = drc["ItemOrKitQualityCode"].ToString();
											worksheet.Cells[radek, 8].Value = String.Format("");
											worksheet.Cells[radek, 9].Value = String.Format("");
											worksheet.Cells[radek, 10].Value = String.Format("");

											radek += 1;
											if (!string.IsNullOrWhiteSpace(drc["KitSNs"].ToString()))
											{
												worksheet.Cells[radek, 1].Value = String.Format("Sériová čísla");
												worksheet.Cells[radek, 1].Style.Font.Bold = true;
												radek += 1;
												string[] sn = drc["KitSNs"].ToString().Split(';');
												string[] dvojice;
												foreach (var e in sn)
												{
													dvojice = e.Split(',');
													worksheet.Cells[radek, 1].Value = dvojice[0].IsNotNullOrEmpty() ? dvojice[0].Trim() : String.Empty;
													worksheet.Cells[radek, 2].Value = dvojice[1].IsNotNullOrEmpty() ? dvojice[1].Trim() : String.Empty;
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
							//worksheet.Column(10).AutoFit();

							xls.Workbook.Properties.Title = "SO objednávka";
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
		} // OrderView

		protected void S1New(int id)
		{
			string proS = string.Format("SELECT [ID] ,SingleOrMaster " +
				",ItemVerKit,[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitUnitOfMeasure] ,[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt" +
				" FROM [dbo].[vwShipmentOrderIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, id);

			try
			{
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvR1.DataSource = myDataTable.DefaultView;
				this.gvR1.DataBind();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}
		}

		protected void btnS1Back_Click(object sender, EventArgs e)
		{
			this.pnlR1.Visible = false; 
			BC.UnbindDataFromObject(this.gvR1);
		}

		protected void btnS1Save_Click(object sender, EventArgs e)
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
					catch (Exception ex)
					{
						BC.ProcessException(ex, AppLog.GetMethodName());
						mOK = false;
					}
				}

				// zpracovani
				if (mOK && iMaSmysl > 0)
				{
					//mOK = false;
					DataTable dt = new DataTable();
					dt = BC.GetDataTable("SELECT min([MessageId]) as minMessageId FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation]", BC.FENIXRdrConnectionString);
					int iMessageID = WConvertStringToInt32(dt.Rows[0][0].ToString()) - 1;
					CultureInfo culture = new CultureInfo("cs-CZ");
					StringBuilder sb = new StringBuilder();

					sb.Append("<NewDataSet>");
					foreach (GridViewRow gvr in this.gvR1.Rows)
					{
						try
						{
							tbx = (TextBox)gvr.FindControl("tbxQuantity");
							if (!string.IsNullOrEmpty(tbx.Text.Trim()))
							{
								ii = Convert.ToInt32(tbx.Text.Trim());
								sb.Append("<item>");
								sb.Append("<ID>" + HttpUtility.HtmlDecode(gvr.Cells[0].Text).Trim() + "</ID>");
								sb.Append("<ItemQuantity>" + ii + "</ItemQuantity>");
								sb.Append("<iMessageID>" + iMessageID + "</iMessageID>");
								sb.Append("<vwCMRSentID>" + ViewState["vwCMRSentID"] + "</vwCMRSentID>");

								sb.Append("</item>");
							}
						}
						catch (Exception ex)
						{
							BC.ProcessException(ex, AppLog.GetMethodName());
							mOK = false;
						}
					}
					sb.Append("</NewDataSet>");

					string help = sb.ToString().Replace("{", "").Replace("}", "");
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(help);

					SqlConnection con = new SqlConnection();
					con.ConnectionString = BC.FENIXWrtConnectionString;
					SqlCommand com = new SqlCommand();
					com.CommandText = "prShipmentConfirmationManuallyIns";
					com.CommandType = CommandType.StoredProcedure;
					com.Connection = con;
					com.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
					com.Parameters.Add("ModifyUserId", SqlDbType.Int).Value = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString()); ;
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
					catch (Exception ex)
					{
						BC.ProcessException(ex, AppLog.GetMethodName());
						mOK = false;
					}
					finally
					{
						com = null;
						con = null;
					}
				}
				if (mOK)
				{
					Session["IsRefresh"] = "1";
					btnS1Back_Click(btnS1Back, EventArgs.Empty);
				}
				else
				{
					btnS1Back_Click(btnS1Back, EventArgs.Empty);
				}
			}
		}

		protected void ExcelConfClicked()
		{
			string ii = string.Empty;
			string proS = string.Empty;
			CheckBox chkb = new CheckBox();
			StringBuilder sb = new StringBuilder();

			foreach (GridViewRow drv in grdData.Rows)
			{
				ii = drv.Cells[2].Text;
				chkb = (CheckBox)drv.FindControl("chkbExcel");
				if (chkb.Checked) sb.Append(ii + ",");
			}

			if (!string.IsNullOrWhiteSpace(sb.ToString()))
			{
				string shipmentOrderNumbers = sb.ToString().Substring(0, (sb.ToString().Length) - 1);

				proS = String.Format("SELECT x.ID,x.[MessageId], x.[MessageTypeID], x.[MistoUrceni], x.[MessageDateOfReceipt] " +
								           ",x.[Reconciliation], x.[MessageDateOfShipment], x.[SingleOrMaster], x.[ItemVerKit] " +
								           ",x.[ItemOrKitID], x.[Popis], x.[MeJe], x.[Kvalita], x.[IncotermDescription] " +
								           ",x.[ItemOrKitQuantity] " +
								           ",SOCI.[RealItemOrKitQuantity] " +
								           ",ISNULL(SNS.SN1, '') as [SN1] " +
								           ",ISNULL(SNS.SN2, '') as [SN2] " +
								           ",x.[ConfirmationID]			 " +
									 "FROM " +
								     "( " +
								     	"SELECT	 SOS.[ID] as [ID] " +
								     					",SOS.[MessageId] as [MessageId] " +
								     					",SOS.[MessageTypeId] as [MessageTypeID]				 " +
								     					",SOS.[CustomerName] as [MistoUrceni]				 " +
								     					",SOC.[MessageDateOfReceipt] as [MessageDateOfReceipt] " +
														",SOC.[Reconciliation] as [Reconciliation] " +
								     					",SOS.[MessageDateOfShipment] as [MessageDateOfShipment]				 " +
								     					",CASE SOSI.[SingleOrMaster] WHEN 0 THEN 'Single' WHEN 1 THEN 'Master' ELSE '' END [SingleOrMaster] " +
								     					",CASE SOSI.[ItemVerKit]  WHEN 0 THEN 'Item' WHEN 1 THEN 'Kit' ELSE '' END [ItemVerKit] " +
								     					",SOSI.ItemOrKitID as [ItemOrKitID] " +
								     					",SOSI.[ItemOrKitDescription] as [Popis] " +
								     					",SOSI.[ItemOrKitUnitOfMeasure] as [MeJe] " +
								     					",SOSI.[ItemOrKitQualityCode] as [Kvalita] " +
								     					",SOSI.[Incoterms] as [IncotermDescription] " +
								     					",SOSI.[ItemOrKitQuantity] as [ItemOrKitQuantity] " +
								     					",SOC.ID AS [ConfirmationID] " +
								     	"FROM [dbo].[CommunicationMessagesShipmentOrdersSent]  SOS " +
								     	"LEFT OUTER JOIN [dbo].[CommunicationMessagesShipmentOrdersConfirmation] SOC " +
								     		"ON SOC.ShipmentOrderID = SOS.ID " +
								     	"INNER JOIN [dbo].[CommunicationMessagesShipmentOrdersSentItems] SOSI " +
								     		"ON SOSI.CMSOId = SOS.ID AND SOS.IsActive = 1 " +
								     	"WHERE SOS.Id IN ({0}) " +
								     ") x " +
								     "LEFT OUTER JOIN [dbo].[CommunicationMessagesShipmentOrdersConfirmationItems] SOCI " +
								     	"ON x.ItemOrKitID = SOCI.ItemOrKitID AND SOCI.CMSOId = x.[ConfirmationID] " +
								     "LEFT OUTER JOIN  [dbo].[CommunicationMessagesShipmentOrdersConfirmationSerNumSent] SNS  " +
								     	"ON SOCI.ID = SNS.[ShipmentOrdersItemsOrKitsID]", shipmentOrderNumbers);

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
							worksheet.Cells[radek, 1, radek, 18].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("S0 - Výpis");
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
							worksheet.Cells[radek, 6].Value = String.Format("Vyjádření");
							worksheet.Cells[radek, 7].Value = String.Format("MessageDateOfShipment");
							worksheet.Cells[radek, 8].Value = String.Format("SingleOrMaster");
							worksheet.Cells[radek, 9].Value = String.Format("ItemVerKit");
							worksheet.Cells[radek, 10].Value = String.Format("ItemOrKitID");
							worksheet.Cells[radek, 11].Value = String.Format("Popis");
							worksheet.Cells[radek, 12].Value = String.Format("MeJe");
							worksheet.Cells[radek, 13].Value = String.Format("Kvalita");
							worksheet.Cells[radek, 14].Value = String.Format("IncotermDescription");
							worksheet.Cells[radek, 15].Value = String.Format("ItemOrKitQuantity");
							worksheet.Cells[radek, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
							worksheet.Cells[radek, 16].Value = String.Format("RealItemOrKitQuantity");
							worksheet.Cells[radek, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
							worksheet.Cells[radek, 17].Value = String.Format("SN1");
							worksheet.Cells[radek, 18].Value = String.Format("SN2");

							radek += 1;
							foreach (DataRow dr in myDt.Rows)
							{
								worksheet.Cells[radek, 1].Value = dr["ID"].ToString();
								worksheet.Cells[radek, 2].Value = dr["MessageID"].ToString();
								worksheet.Cells[radek, 3].Value = dr["MessageTypeID"].ToString();
								worksheet.Cells[radek, 4].Value = dr["MistoUrceni"].ToString();																
								BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dr["MessageDateOfReceipt"], BC.DATE_TIME_FORMAT_DDMMYYY);																
								worksheet.Cells[radek, 6].Value = (BC.ExcelGetReconciliationDescription(dr["Reconciliation"].ToString())).ToUpper();
								worksheet.Cells[radek, 7].Value = dr["MessageDateOfShipment"].ToString();
								worksheet.Cells[radek, 8].Value = dr["SingleOrMaster"].ToString();
								worksheet.Cells[radek, 9].Value = dr["ItemVerKit"].ToString();
								worksheet.Cells[radek, 10].Value = dr["ItemOrKitID"].ToString();
								worksheet.Cells[radek, 11].Value = dr["Popis"].ToString();
								worksheet.Cells[radek, 12].Value = dr["MeJe"].ToString();
								worksheet.Cells[radek, 13].Value = dr["Kvalita"].ToString();
								worksheet.Cells[radek, 14].Value = dr["IncotermDescription"].ToString();
								if (dr["ItemOrKitQuantity"] != DBNull.Value)
								{
									worksheet.Cells[radek, 15].Value = Convert.ToInt32(dr["ItemOrKitQuantity"]);//dr["ItemOrKitQuantity"].ToString();
									worksheet.Cells[radek, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
									worksheet.Cells[radek, 15].Style.Numberformat.Format = "# ### ### ##0";
								}
								if (dr["RealItemOrKitQuantity"] != DBNull.Value)
								{
									worksheet.Cells[radek, 16].Value = Convert.ToInt32(dr["RealItemOrKitQuantity"]);//dr["RealItemOrKitQuantity"].ToString();
									worksheet.Cells[radek, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
									worksheet.Cells[radek, 16].Style.Numberformat.Format = "# ### ### ##0";
								}
								worksheet.Cells[radek, 17].Value = dr["SN1"].ToString();
								worksheet.Cells[radek, 18].Value = dr["SN2"].ToString();
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
														
							xls.Workbook.Properties.Title = "SO objednávka";
							xls.Workbook.Properties.Subject = "S0 - Výpis";
							xls.Workbook.Properties.Keywords = "Office Open XML";
							xls.Workbook.Properties.Category = "Sériová čísla";
							xls.Workbook.Properties.Comments = "";							
							xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";

							// save the new spreadsheet to the stream
							xls.Save();
							ms.Flush();
							ms.Seek(0, SeekOrigin.Begin);

							Response.Clear();
							Response.Buffer = true;
							Response.AddHeader("content-disposition", "attachment;filename=S0_vypis_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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
			sb = null;
		}

		protected void ExcelConfClicked_OLD()		
		{
			string ii = string.Empty;
			string proS = string.Empty;
			CheckBox chkb = new CheckBox();
			StringBuilder sb = new StringBuilder();

			foreach (GridViewRow drv in grdData.Rows)
			{
				ii = drv.Cells[2].Text;
				chkb = (CheckBox)drv.FindControl("chkbExcel");
				if (chkb.Checked) sb.Append(ii + ",");
			}

			if (!string.IsNullOrWhiteSpace(sb.ToString()))
			{
				string proW = " WHERE ShipmentOrderID in (" + sb.ToString().Substring(0, (sb.ToString().Length) - 1) + ") AND Hd.ISACTIVE=1  AND Hd.[Reconciliation]<>2 AND It.IsActive=1";
				proS = " SELECT Hd.[ID],Hd.[MessageId],Hd.[MessageTypeId],Hd.[MessageDescription],Hd.[MessageDateOfReceipt],Hd.[ShipmentOrderID],Hd.[Reconciliation],Hd.[ReconciliationYesNo]    " +
					   " ,Hd.[MessageDateOfShipment],Hd.[RequiredDateOfShipment],Hd.[IsActive],Hd.[ModifyDate]                                                                                   " +
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
							worksheet.Cells[radek, 1, radek, 17].Merge = true;
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
							worksheet.Cells[radek, 15].Value = String.Format("RealItemOrKitQuantityInt");
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
								worksheet.Cells[radek, 5].Value = dr["MessageDateOfReceipt"].ToString();
								worksheet.Cells[radek, 6].Value = dr["ShipmentOrderID"].ToString();
								worksheet.Cells[radek, 7].Value = dr["ReconciliationYesNo"].ToString();
								worksheet.Cells[radek, 8].Value = dr["MessageDateOfShipment"].ToString();
								worksheet.Cells[radek, 9].Value = dr["SingleOrMaster"].ToString();
								worksheet.Cells[radek, 10].Value = dr["ItemVerKit"].ToString();
								worksheet.Cells[radek, 11].Value = dr["ItemOrKitDescription"].ToString();
								worksheet.Cells[radek, 12].Value = dr["ItemOrKitUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 13].Value = dr["ItemOrKitQualityCode"].ToString();
								worksheet.Cells[radek, 14].Value = dr["IncotermDescription"].ToString();
								worksheet.Cells[radek, 15].Value = dr["RealItemOrKitQuantityInt"].ToString();
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
						}
					}
					// **
				}
			}
			sb = null;
		}

		/// <summary>
		/// Nastavení textboxu s poznámkou
		/// (v části 'S0 - Objednávka expedice')
		/// </summary>
		private void setTxbRemark()
		{
			this.tbxRemark.Text = ShipmentHelper.GetRemarkForID(grdData.SelectedValue);
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
		/// (v části 'S0 - Objednávka expedice')
		/// </summary>
		/// <param name="visibility"></param>
		private void setRemarkVisibility(bool visibility)
		{
			this.lblRemark.Visible = visibility;
			this.tbxRemark.Visible = visibility;
		}

		/// <summary>
		/// Nastaví viditelnost nadpisu a textboxu s poznámkou
		/// (v části 'Nový požadavek na expedici')
		/// </summary>
		/// <param name="visibility"></param>
		private void setRemarkNewVisibility(bool visibility)
		{
			lblRemarkNew.Visible = visibility;
			tbxRemarkNew.Visible = visibility;
		}
	}
}