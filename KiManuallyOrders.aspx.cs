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
	/// [Kitting] K0 - objednávka kittingu (sestavy)
	/// !! sloupec K1 (ruční vytvoření K1) je skrytý !!
	/// </summary>
	public partial class KiManuallyOrders : BasePage
	{
		/// <summary>
		/// Reconciliation
		/// </summary>
		private const int COLUMN_RECONCILIATION = 15;

		/// <summary>
		/// MessageStatusId
		/// </summary>
		private const int COLUMN_MESSAGE_STATUS_ID = 16;

		//seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		private int[] hideGrdDataColumns = new int[] { COLUMN_RECONCILIATION, COLUMN_MESSAGE_STATUS_ID };

		/// <summary>
		/// Zamítnuto
		/// <value> = 2</value>
		/// </summary>
		private const string REJECTED = "2";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				this.prepareDdlQualities();
				BaseHelper.FillDdlKitCode(ref this.ddlModelCPE);
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
			string proW = " IsActive = 1 ";
			if (this.ddlModelCPE.SelectedValue.ToString() != "-1") proW += " AND [ModelCPE] like '" + this.ddlModelCPE.SelectedItem.Text.Trim() + "%'";
			
			string proS = "*";
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwCMKSent]";
			pagerData.OrderBy = "ID DESC";
			pagerData.ColumnList = proS;
			pagerData.WhereClause = proW;

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, this.hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.setVisibilityButtonK1manually();				
				BaseHelper.SetPictureDeleteOrder(ref grdData, COLUMN_MESSAGE_STATUS_ID, "3");
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		/// <summary>
		/// Pokud je objednávka zamítnuta, lze k ní vytvořit RUČNĚ K1
		/// </summary>
		private void setVisibilityButtonK1manually()
		{
			ImageButton img = new ImageButton();
			foreach (GridViewRow gvr in this.grdData.Rows)
			{
				if (gvr.Cells[COLUMN_RECONCILIATION].Text == REJECTED)
				{
					img = (ImageButton)gvr.FindControl("btnK1new");
					img.Enabled = true;
					img.Visible = true;
				}
				else
				{
					img = (ImageButton)gvr.FindControl("btnK1new");
					img.Enabled = false;
					img.Visible = false;
				}
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();

			this.gvItems.Visible = true;
			this.pnlK1.Visible = false;
			this.gvItems.SelectedIndex = -1;

			string proSelect = String.Format("SELECT [ID] ,[KitId],[KitDescription],[KitQuantity],[KitQuantityDelivered] " +
			                                 ",[MeasuresID]  ,[KitUnitOfMeasure] ,[KitQualityId] ,[KitQualityCode]  ,[HeliosOrderID] " +
			                                 ",[CardStockItemsId],KitQuantityInt,KitQuantityDeliveredInt FROM [dbo].[vwCMKSentItems] " + 
											 "WHERE [IsActive] = 1 AND CMSOId={0}", grdMainIdKey);
			try
			{
				DataTable myDataTable = BC.GetDataTable(proSelect, BC.FENIXRdrConnectionString);
				this.gvItems.DataSource = myDataTable.DefaultView;
				this.gvItems.DataBind();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";
			ClearViewControls(vwEdit);
			this.mvwMain.ActiveViewIndex = 1;
						
			this.lblErrInfo.Text = "";
			this.gvOrders.DataSource = null; this.gvOrders.DataBind();
			this.grdData.DataSource = null; this.grdData.DataBind();
			this.pnlK1.Visible = false;
			
			string proS = string.Empty;
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
			 	   " SELECT ID cValue, CAST(Code AS CHAR(10))+' - '+[DescriptionCz] ctext FROM [dbo].[cdlKits] WHERE [IsActive]=1 AND KitQualitiesId = 1) xx ORDER BY ctext"; // KitQualitiesId = 1  jen nový 20140819
			FillDdl(ref this.ddlItems, proS);

			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
			       " SELECT ID cValue, [Name] ctext FROM [dbo].[cdlStocks] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlStock, proS);

			try
			{
				this.ddlStock.SelectedValue = "2";
			}
			catch (Exception ex)
			{
				this.ddlStock.SelectedValue = "-1";
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void chkbItems_CheckedChanged(object sender, EventArgs e)
		{
		}

		protected void ddlItems_TextChanged(object sender, EventArgs e)
		{
		}

		protected void btnPridatDoObjednavky_Click(object sender, EventArgs e)
		{			
			bool mOk = true;			
			string Err = string.Empty;
			this.lblErrInfo.Text = string.Empty;

			// kontrola uplnosti dat
			if (this.ddlStock.SelectedValue.ToString() == "-1" || this.ddlStock == null) { Err += "Chybný výběr kompletačního místa <br />"; mOk = false; }
			if (this.ddlItems.SelectedValue.ToString() == "-1" || this.ddlItems == null) { Err += "Chybný výběr kitu <br />"; mOk = false; }
			if (string.IsNullOrWhiteSpace(this.tbxQuantity.Text.Trim())) { Err += "Schází množství <br />"; mOk = false; }
			else
			{
				if (WConvertStringToInt32(this.tbxQuantity.Text.Trim()) == -1) { Err += "Množství není celé číslo <br />"; mOk = false; }
			}
			if (string.IsNullOrWhiteSpace(this.tbxDateOfDelivery.Text.Trim())) { Err += "Schází datum požadované kompletace <br />"; mOk = false; }
			else
			{
				if (WConvertDateToYYYYmmDD(this.tbxDateOfDelivery.Text.Trim()) == "") { Err += "Chybné datum požadované kompletace <br />"; mOk = false; }
			}
			
			#region  Kontrola disponibilního množství v dané chvíli

			if (mOk)
			{
				try
				{
					double d = 0;
					DataTable myDataTable;


					DataTable dtKitItems;
					string proS = string.Format("SELECT [cdlKitsItemsID],[cdlKitsID],[ItemVerKit],[ItemOrKitID],[ItemCode],[DescriptionCzKit] " +
						",[DescriptionCzItemsOrKit],[ItemOrKitQuantity] ,[PackageType]  FROM [dbo].[vwKitsIt] WHERE IsActive=1 AND [cdlKitsID] = {0}", this.ddlItems.SelectedValue.ToString());
					dtKitItems = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);

					if (dtKitItems != null && dtKitItems.Rows.Count > 0)
					{
						foreach (DataRow dr in dtKitItems.Rows)
						{
							// TODO
							// =====

							proS = string.Format("SELECT [ID] ,[cdlKitsId],[ItemVerKit],[ItemOrKitId],[ItemGroupGoods],[ItemCode],[ItemOrKitQuantity],[PackageType],[Code],[DescriptionCz]" +
														",[StockId],[Name],[ItemOrKitFree],[ItemOrKitReserved] ,[ItemOrKitUnConsilliation] ,[ItemOrKitReleasedForExpedition],[ItemOrKitQuality]" +
														" FROM [dbo].[vwKitsItemCardStokItems]  WHERE [ItemOrKitQuality] = 1 AND [StockId]={0} AND cdlKitsId={1} AND ItemOrKitId={2}", this.ddlStock.SelectedValue.ToString(), this.ddlItems.SelectedValue.ToString(), dr["ItemOrKitID"].ToString());
							myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);

							if (myDataTable != null && myDataTable.Rows.Count > 0)
							{
								foreach (DataRow drx in myDataTable.Rows)
								{
									d = WConvertStringToDouble(drx["ItemOrKitQuantity"].ToString()) * WConvertStringToDouble(this.tbxQuantity.Text.Trim());
									if (d > WConvertStringToDouble(drx["ItemOrKitFree"].ToString()))
									{
										Err += "<b>Nedostatečný počet volných jednotek pro kompletaci:</b><br />Item Code=<b>" + drx["ItemCode"].ToString() + "</b> " + dr["DescriptionCzItemsOrKit"].ToString() +
											"<br /> -požadované množství=" + d.ToString() + "<br /> -volné množství=" + drx["ItemOrKitFree"].ToString() + " <br />"; mOk = false;
									}
								}
							}
							else
							{
								Err += "<b>Žádné volné jednotky pro kompletaci:</b><br />Item Code=<b>" + dr["ItemCode"].ToString() + "</b> " + dr["DescriptionCzItemsOrKit"].ToString() +
									"<br /> -požadované množství=" + this.tbxQuantity.Text.Trim() + "<br /> -volné množství= 0,000" + " <br />"; mOk = false;
							}
						}
					}
				}
				catch
				{
					Err += "Chyba při kontrole volného množství  <br />"; mOk = false;
				}
			}

			#endregion

			if (mOk)
			{
				AddRec("");
			}
			this.lblErrInfo.Text = Err;
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlItems.Items.Clear();                                // ddl kitů
			this.gvOrders.DataSource = null; this.gvOrders.DataBind();
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				DataTable myT = new DataTable("myDt");
				ZalozTabulku(ref myT);

				bool mOK = true;

				if (this.gvOrders != null && gvOrders.Rows.Count > 0)
				{
					foreach (GridViewRow gvr in gvOrders.Rows)
					{
						DataRow Doly = myT.NewRow();
						CheckBox myChkb;
						myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
						if (myChkb.Checked)
						{
							try
							{
								Doly[0] = (myChkb.Checked) ? 1 : 0;	                         // AnoNe
								Doly[1] = WConvertStringToInt32(gvr.Cells[1].Text);          // StockId
								Doly[2] = HttpUtility.HtmlDecode(gvr.Cells[2].Text);         // StockName
								Doly[3] = WConvertStringToInt32(gvr.Cells[3].Text);          // KitId
								Doly[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);         // DescriptionCZ
								Doly[5] = WConvertStringToInt32(gvr.Cells[5].Text);          // KitQuantity
								Doly[6] = WConvertStringToInt32(gvr.Cells[6].Text);          // MeasuresId
								Doly[7] = HttpUtility.HtmlDecode(gvr.Cells[7].Text);         // MeasuresCode
								Doly[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);         // DateOfDelivery
								Doly[9] = HttpUtility.HtmlDecode(gvr.Cells[9].Text);         // KitQualitiesId
								Doly[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);       // KitQualitiesCode
								Doly[11] = HttpUtility.HtmlDecode(gvr.Cells[11].Text);       // HeliosOrderID
								Doly[12] = 0; // HttpUtility.HtmlDecode(gvr.Cells[12].Text);       // CardStockItemsId

								myT.Rows.Add(Doly);

							}
							catch (Exception)
							{
								mOK = false;
							}
						}
					}
					if (mOK)
					{
						CultureInfo culture = new CultureInfo("cs-CZ");
						StringBuilder sb = new StringBuilder();

						sb.Append("<NewDataSet>");
						foreach (DataRow r in myT.Rows)
						{
							if (r[0].ToString().ToUpper() == "TRUE")
							{
								sb.Append("<CommunicationMessagesSentKitManually>");
								sb.Append("<ItemVerKit>" + "1" + "</ItemVerKit>");
								sb.Append("<StockID>" + r[1].ToString() + "</StockID>");
								sb.Append("<StockName>" + r[2].ToString() + "</StockName>");
								sb.Append("<ItemOrKitID>" + r[3].ToString() + "</ItemOrKitID>");
								sb.Append("<ItemOrKitDescription>" + r[4].ToString() + "</ItemOrKitDescription>");
								sb.Append("<ItemOrKitQuantity>" + r[5].ToString() + "</ItemOrKitQuantity>");
								sb.Append("<ItemOrKitUnitOfMeasureID>" + r[6].ToString() + "</ItemOrKitUnitOfMeasureID>");
								sb.Append("<ItemOrKitMeasuresCode>" + r[7].ToString() + "</ItemOrKitMeasuresCode>");
								sb.Append("<ItemOrKitDateOfDelivery>" + (Convert.ToDateTime(r[8].ToString())).ToString("yyyyMMdd") + "</ItemOrKitDateOfDelivery>");  //--<ItemDateOfDelivery, datetime,>
								sb.Append("<ItemOrKitQualityID>" + r[9].ToString() + "</ItemOrKitQualityID>");
								sb.Append("<KitQualitiesCode>" + r[10].ToString() + "</KitQualitiesCode>");
								sb.Append("<HeliosOrderID>" + r[11].ToString() + "</HeliosOrderID>");
								sb.Append("<ModifyUserId>" + Session["Logistika_ZiCyZ"].ToString() + "</ModifyUserId>");
								sb.Append("</CommunicationMessagesSentKitManually>");
							}
						}
						sb.Append("</NewDataSet>");

						string help = sb.ToString().Replace("{", "").Replace("}", "");
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(help);

						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.StoredProcedure;
						sqlComm.CommandText = "[dbo].[prCMSOKins]";
						sqlComm.Connection = conn;
						sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;

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
								mOK = true;
							}
						}
						catch (Exception)
						{
							mOK = false;
						}
						finally
						{
							conn.Close();
							conn = null;
							sqlComm = null;
						}

						if (mOK)
						{
							myT = null; sb = null; doc = null;
							this.mvwMain.ActiveViewIndex = 0;
							this.search_button.Focus();
							BC.UnbindDataFromObject<GridView>(this.gvOrders);
							this.fillPagerData(BC.PAGER_FIRST_PAGE);
							Session["IsRefresh"] = "1";
						}
					}
				}
				else
				{
					if (Session["IsRefresh"].ToString() == "1")
					{
						this.fillPagerData(BC.PAGER_FIRST_PAGE);
						this.mvwMain.ActiveViewIndex = 0;
						BC.UnbindDataFromObject<GridView>(this.gvOrders);
					}
				}
			}
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
			myDataColumn.ColumnName = "KitId";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "KitDescription";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "KitQuantity";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "MeasuresID";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "KitUnitOfMeasure";
			myDt.Columns.Add(myDataColumn);
			
			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "DateOfDelivery";
			myDt.Columns.Add(myDataColumn);
			
			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "KitQualitiesId";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "KitQualitiesCode";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "HeliosOrderID";
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "CardStockItemsId";
			myDt.Columns.Add(myDataColumn);
		}

		private void AddRec(string par1)
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
					Doly[0] = (myChkb.Checked) ? 1 : 0;	                         // AnoNe
					Doly[1] = WConvertStringToInt32(gvr.Cells[1].Text);          // StockId
					Doly[2] = HttpUtility.HtmlDecode(gvr.Cells[2].Text);         // StockName
					Doly[3] = WConvertStringToInt32(gvr.Cells[3].Text);          // KitId
					Doly[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);         // DescriptionCZ
					Doly[5] = WConvertStringToInt32(gvr.Cells[5].Text);          // KitQuantity
					Doly[6] = WConvertStringToInt32(gvr.Cells[6].Text);          // MeasuresId
					Doly[7] = HttpUtility.HtmlDecode(gvr.Cells[7].Text);         // MeasuresCode
					Doly[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);         // DateOfDelivery
					Doly[9] = HttpUtility.HtmlDecode(gvr.Cells[9].Text);         // KitQualitiesId
					Doly[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);       // KitQualitiesCode
					Doly[11] = HttpUtility.HtmlDecode(gvr.Cells[11].Text);       // HeliosOrderID
					Doly[12] = 0; // HttpUtility.HtmlDecode(gvr.Cells[12].Text);       // CardStockItemsId

					myT.Rows.Add(Doly);
				}
			}

			string proS = string.Empty;
			proS = "SELECT [Code] ,[DescriptionCz] ,[DescriptionEng] ,[MeasuresId] ,[MeasuresCode] ,[KitQualitiesId] ,[KitQualitiesCode] FROM [dbo].[cdlKits] WHERE id = " + this.ddlItems.SelectedValue.ToString();
			DataTable myDataTable;
			myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
			if (myDataTable != null && myDataTable.Rows.Count == 1)
			{
				DataRow Dolx = myT.NewRow();
				Dolx[0] = 1;	                                                                  // AnoNe
				Dolx[1] = WConvertStringToInt32(this.ddlStock.SelectedValue.ToString());          // StockId  (1 LICA, 2 ND)
				Dolx[2] = this.ddlStock.SelectedItem.Text;                                        // StockName
				Dolx[3] = WConvertStringToInt32(this.ddlItems.SelectedValue.ToString());          // KitId
				Dolx[4] = this.ddlItems.SelectedItem.ToString();                                  // DescriptionCZ
				Dolx[5] = this.tbxQuantity.Text.Trim();                                           // KitQuantity
				Dolx[6] = WConvertStringToInt32(myDataTable.Rows[0][3].ToString());               // MeasuresId
				Dolx[7] = myDataTable.Rows[0][4].ToString();                                      // MeasuresCode
				Dolx[8] = this.tbxDateOfDelivery.Text.Trim();                                     // DateOfDelivery
				Dolx[9] = WConvertStringToInt32(myDataTable.Rows[0][5].ToString());               // KitQualitiesId
				Dolx[10] = myDataTable.Rows[0][6].ToString();                                     // KitQualitiesCode
				Dolx[11] = WConvertStringToInt32(this.tbxHeliosOrderID.Text.Trim());              // HeliosOrderID
				Dolx[12] = 0;// ViewState["CardStockItemsId"].ToString();                         // CardStockItemsId

				myT.Rows.Add(Dolx);
				ViewState["CardStockItemsId"] = "";
			}
			this.gvOrders.Columns[1].Visible = true; this.gvOrders.Columns[3].Visible = true;
			this.gvOrders.DataSource = myT.DefaultView; this.gvOrders.DataBind();
			this.gvOrders.Columns[1].Visible = false; this.gvOrders.Columns[3].Visible = false;
		}

		protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "OrderView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMKSentID"] = id.ToString();
				this.OrderView(id);
			}

			if (e.CommandName == "K1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMKSentID"] = id.ToString();
				this.K1New(id);
			}

			if (e.CommandName == "DeleteOrder")
			{
				string confirmValue = Request.Form["confirm_delete_order"];
				if (confirmValue.ToUpper() == "ANO")
				{
					BaseHelper.ProcessDeleteOrder(e, ref grdData, "3", "KittingOrder", Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString()));
				}
			}
		}

		protected void OrderView(int id)
		{
			this.gvItems.Visible = true;

			try
			{
				bool mOK = true;
				bool mOKR = true;

				string proS = string.Format("SELECT  [ID] ,[MessageId] ,[MessageTypeID] ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId] ,[KitDateOfDelivery] ,[StockId] ,[IsActive]" +
											" ,[ModifyDate] ,[ModifyUserId] ,[CompanyName] ,[DescriptionCz] ,[Code] ,[HeliosOrderID]" +
											" FROM [dbo].[vwCMKSent] WHERE Id = {0} ORDER BY 1,2", id);
				
				DataTable dtObjHl = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHl == null || dtObjHl.Rows.Count < 1) { mOK = false; }

				proS = string.Format("SELECT CI.[ID] ,CI.[CMSOId] ,CI.[HeliosOrderID] ,CI.[HeliosOrderRecordId] ,CI.[KitId] ,CI.[KitDescription] ,CI.[KitQuantityInt] ,CI.[KitQuantityDeliveredInt] " +
									 ",CI.[MeasuresID] ,CI.[KitUnitOfMeasure]  ,CI.[KitQualityId] ,CI.[KitQualityCode] ,CI.[CardStockItemsId] ,CI.[IsActive] ,CI.[ModifyDate] ,CI.[ModifyUserId], K.[Code]" +
									 " FROM [dbo].[vwCMKSentItems]  CI INNER JOIN cdlKits  K ON CI.[KitId]=K.Id WHERE CMSOId={0}", id);
				
				DataTable dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjR == null || dtObjR.Rows.Count < 1) { mOK = false; }

				proS = string.Format("SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[KitOrderID],[Reconciliation],[IsActive],[ModifyDate],[ModifyUserId]" +
									 " FROM [dbo].[CommunicationMessagesKittingsConfirmation] where [KitOrderID]={0} AND Reconciliation<>0", id);

				DataTable dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dtObjHlCon == null || dtObjHlCon.Rows.Count < 1) { mOKR = false; }

				DataTable dtObjHlRCon = new DataTable();
				if (mOKR)
				{
					string ids = "-99";
					foreach (DataRow dr in dtObjHlCon.Rows)
					{
						ids += "," + dr[0].ToString();
					}

					proS = string.Format("SELECT KI.[ID],KI.[CMSOId],KI.[KitID],KI.[KitDescription],KI.[KitQuantityInt],KI.[KitUnitOfMeasure],KI.[KitQualityId],KI.[KitSNs],KI.[IsActive]" +
										 ",KI.[ModifyDate],KI.[ModifyUserId],K.Code" +
										 " FROM [dbo].[vwKitConfirmationIt]  KI INNER JOIN cdlKits  K ON KI.[KitId]=K.Id where [CMSOId] in ({0})", ids);
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
							int radek = 1;
							// nadpis
							worksheet.Row(1).Height = 24;
							worksheet.Cells[radek, 1, radek, 8].Merge = true;
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 1].Style.Font.Size = 14;
							worksheet.Cells[radek, 1].Value = String.Format("K0 - Objednávka");
							worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
							worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
							worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
							radek += 2;
							// hlavicka objednavky
							worksheet.Cells[radek, 1].Value = String.Format("Message ID");
							worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0][1].ToString();						// MessageID
							worksheet.Cells[radek, 2].Style.Font.Bold = true;
							worksheet.Cells[radek, 3].Value = String.Format("Message Popis");
							worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0][4].ToString();						// MessageDescription
							worksheet.Cells[radek, 4].Style.Font.Bold = true;
							worksheet.Cells[radek, 5].Value = String.Format("Kompletační místo");
							worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["CompanyName"].ToString();			// ItemSupplierDescription
							worksheet.Cells[radek, 6].Style.Font.Bold = true;
							worksheet.Cells[radek, 7].Value = String.Format("Datum dodání");
							//worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["KitDateOfDelivery"].ToString();	
							BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 8], dtObjHl.Rows[0]["KitDateOfDelivery"], BC.DATE_TIME_FORMAT_DDMMYYY);
							worksheet.Cells[radek, 8].Style.Font.Bold = true;
							worksheet.Cells[radek, 9].Value = String.Format("");
							worksheet.Cells[radek, 10].Value = String.Format("");
							worksheet.Cells[radek, 10].Style.Font.Bold = true;
							radek += 1;

							// detaily objednávky
							worksheet.Cells[radek, 1].Value = String.Format("HeliosOrderRecordId");
							worksheet.Cells[radek, 1].Style.Font.Bold = true;
							worksheet.Cells[radek, 2].Value = String.Format("Id Kitu");
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
								worksheet.Cells[radek, 2].Value = dr["KitId"].ToString();
								worksheet.Cells[radek, 3].Value = dr["Code"].ToString();
								worksheet.Cells[radek, 4].Value = dr["KitDescription"].ToString();
								worksheet.Cells[radek, 5].Value = dr["KitQuantityInt"].ToString();
								worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 6].Value = dr["KitQuantityDeliveredInt"].ToString();
								worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
								worksheet.Cells[radek, 7].Value = dr["KitUnitOfMeasure"].ToString();
								worksheet.Cells[radek, 8].Value = dr["KitQualityCode"].ToString();
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
								worksheet.Cells[radek, 1].Value = String.Format("K1 - Confirmace objednávky");
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
									//	worksheet.Cells[radek, 4].Value = String.Format("Schváleno");
									BC.ExcelProcessReconciliation(worksheet.Cells[radek, 4], dr["Reconciliation"].ToString());

									//worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
									BC.ExcelFillCellWithDateTime(worksheet.Cells[radek, 5], dr["ModifyDate"], BC.DATE_TIME_FORMAT_DDMMYYY_HHMMSS);
									worksheet.Cells[radek, 6].Value = dr["IsActive"].ToString();
									worksheet.Cells[radek, 7].Value = String.Format("");
									worksheet.Cells[radek, 8].Value = String.Format("");
									worksheet.Cells[radek, 9].Value = String.Format("");
									worksheet.Cells[radek, 10].Value = String.Format("");
									radek += 2;

									worksheet.Cells[radek, 1].Value = String.Format("Kit Id");
									worksheet.Cells[radek, 1].Style.Font.Bold = true;
									worksheet.Cells[radek, 2].Value = String.Format("Kód");
									worksheet.Cells[radek, 2].Style.Font.Bold = true;
									worksheet.Cells[radek, 3].Value = String.Format("Popis");
									worksheet.Cells[radek, 3].Style.Font.Bold = true;
									worksheet.Cells[radek, 4].Value = String.Format("Množství");
									worksheet.Cells[radek, 4].Style.Font.Bold = true;
									worksheet.Cells[radek, 5].Value = String.Format("MJ");
									worksheet.Cells[radek, 5].Style.Font.Bold = true;
									worksheet.Cells[radek, 6].Value = String.Format("");
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

									for (int ii = 0; ii <= dtObjHlRCon.Rows.Count - 1; ii++)
									{
										DataRow drc = dtObjHlRCon.Rows[ii];

										if (dr["ID"].ToString() == drc["CMSOId"].ToString())
										{
											worksheet.Cells[radek, 1].Value = drc["KitID"].ToString();
											worksheet.Cells[radek, 2].Value = drc["Code"].ToString();
											worksheet.Cells[radek, 3].Value = drc["KitDescription"].ToString();
											worksheet.Cells[radek, 4].Value = drc["KitQuantityInt"].ToString();
											worksheet.Cells[radek, 5].Value = drc["KitUnitOfMeasure"].ToString();
											//worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
											worksheet.Cells[radek, 6].Value = String.Format("");
											worksheet.Cells[radek, 7].Value = String.Format("");
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
													dvojice = e.ToString().Split(',');
													worksheet.Cells[radek, 1].Value = BC.ExcelPrepareSerialNumber(dvojice[0]);
													worksheet.Cells[radek, 2].Value = BC.ExcelPrepareSerialNumber(dvojice[1]);
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
														
							worksheet.HeaderFooter.OddHeader.CenteredText = "K0 - objednávka kittingu";							
							worksheet.HeaderFooter.OddFooter.RightAlignedText =
								string.Format("Strana {0} z {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
														
							xls.Workbook.Properties.Title = "K0 - objednávka kittingu";
							xls.Workbook.Properties.Subject = "Sériová čísla";
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
							Response.AddHeader("content-disposition", "attachment;filename=K0_Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
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

		protected void K1New(int id)
		{
			this.pnlK1.Visible = true;
			string proS = String.Format("SELECT [ID],[CMSOId],[KitId],[KitDescription],[KitQuantity],[KitQuantityDelivered],[KitQuantityInt] " +
										", [KitQuantityDeliveredInt],[MeasuresID],[KitUnitOfMeasure],[KitQualityId],[KitQualityCode] " +
										", [HeliosOrderID],[HeliosOrderRecordId],[CardStockItemsId],[IsActive],[ModifyDate],[ModifyUserId] " +
										" FROM [dbo].[vwCMKSentItems] C WHERE C.[IsActive] = 1 AND C.CMSOId={0}", id);
			try
			{				
				this.gvItems.Visible = false;
				BC.UnbindDataFromObject<GridView>(this.gvItems);

				DataTable myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvK1.Columns[9].Visible = true; this.gvK1.Columns[10].Visible = true;
				this.gvK1.DataSource = myDataTable.DefaultView;
				this.gvK1.DataBind();
				this.gvK1.Columns[9].Visible = false; this.gvK1.Columns[10].Visible = false;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}
		}

		protected void btnK1Back_Click(object sender, EventArgs e)
		{
			this.pnlK1.Visible = false;
			BC.UnbindDataFromObject<GridView>(this.gvK1);
		}

		protected void btnK1Save_Click(object sender, EventArgs e)
		{
			// kontrola hodnot
			bool mOK = true; string Err = string.Empty; int ii = 0; int iMaSmysl = 0;
			TextBox tbx = new TextBox();
			foreach (GridViewRow gvr in this.gvK1.Rows)
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
			}  // foreach (GridViewRow gvr in this.gvK1.Rows)

			// zpracovani
			if (mOK && iMaSmysl > 0)
			{
				//mOK = false;
				DataTable dt = new DataTable();
				dt = BC.GetDataTable("SELECT min([MessageId]) as minMessageId FROM [dbo].[CommunicationMessagesKittingsConfirmation]", BC.FENIXRdrConnectionString);
				int iMessageID = WConvertStringToInt32(dt.Rows[0][0].ToString()) - 1;
				CultureInfo culture = new CultureInfo("cs-CZ");
				StringBuilder sb = new StringBuilder();

				dt = BC.GetDataTable(" SELECT [ID],[MessageId],[MessageTypeID],[MessageDescription],[MessageDateOfShipment],[MessageStatusId]" +
						 " ,[KitDateOfDelivery],[StockId],[IsActive],[ModifyDate],[ModifyUserId],[CompanyName],[DescriptionCz]   " +
						 " ,[Code],[HeliosOrderID],[Reconciliation]                                                              " +
						 "  FROM [dbo].[vwCMKSent] WHERE id=" + ViewState["vwCMKSentID"].ToString(), BC.FENIXRdrConnectionString);

				sb.Append("<NewDataSet>");
				sb.Append("<CommunicationMessagesKittingConfirmation>");
				sb.Append("<ID>-1</ID>");
				sb.Append("<MessageID>" + iMessageID.ToString() + "</MessageID>");
				sb.Append("<MessageTypeID>4</MessageTypeID>");
				sb.Append("<MessageTypeDescription>KittingConfirmation</MessageTypeDescription>");
				sb.Append("<MessageDateOfReceipt>" + DateTime.Today.ToString("yyyy-MM-dd") + "</MessageDateOfReceipt>");
				sb.Append("<KitOrderID>" + dt.Rows[0][0].ToString() + "</KitOrderID>");
				sb.Append("<HeliosOrderID>" + dt.Rows[0][14].ToString() + "</HeliosOrderID>");   //

				foreach (GridViewRow gvr in this.gvK1.Rows)
				{
					try
					{
						tbx = (TextBox)gvr.FindControl("tbxQuantity");
						if (!string.IsNullOrEmpty(tbx.Text.Trim()))
						{
							ii = Convert.ToInt32(tbx.Text.Trim());
							sb.Append("<KitID>" + gvr.Cells[1].Text + "</KitID>");
							sb.Append("<KitDescription>" + HttpUtility.HtmlDecode(gvr.Cells[2].Text) + "</KitDescription>");
							sb.Append("<KitQuantity>" + ii.ToString() + "</KitQuantity>");
							sb.Append("<KitUnitOfMeasureID>" + gvr.Cells[9].Text + "</KitUnitOfMeasureID>");
							sb.Append("<KitUnitOfMeasure>" + HttpUtility.HtmlDecode(gvr.Cells[8].Text) + "</KitUnitOfMeasure>");
							sb.Append("<KitQualityID>" + gvr.Cells[10].Text + "</KitQualityID>");
							sb.Append("<KitQuality>" + HttpUtility.HtmlDecode(gvr.Cells[6].Text) + "</KitQuality>");
							sb.Append("<NDReceipt></NDReceipt>");
						}
					}
					catch (Exception)
					{
						mOK = false;
					}
				}
				sb.Append("</CommunicationMessagesKittingConfirmation>");
				sb.Append("</NewDataSet>");

				string help = sb.ToString().Replace("{", "").Replace("}", "");
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(help);
				
				SqlConnection con = new SqlConnection();
				con.ConnectionString = BC.FENIXWrtConnectionString;
				SqlCommand com = new SqlCommand();
				com.CommandText = "prCMRCKins";
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
				catch (Exception ex)
				{
					mOK = false;
					BC.ProcessException(ex, AppLog.GetMethodName(), "help = " + help);
				}
				finally
				{
					com = null;
				}
			}

			if (mOK)
			{
				btnK1Back_Click(btnK1Back, EventArgs.Empty);
			}			
		}

		/// <summary>
		/// Naplní ddlQuailities seznamem kvalit, vybere kvalitu NEW a disabluje ddlQuailities
		/// (NEW - zatím jediná kvalita při objednávce kitu => tato filtrace, zatím, nedává smysl)
		/// </summary>
		private void prepareDdlQualities()
		{
			BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);

			try
			{
				ddlKitQualitiesFlt.SelectedIndex = ddlKitQualitiesFlt.Items.IndexOf(ddlKitQualitiesFlt.Items.FindByText("NEW"));
			}
			catch
			{
			}

			this.ddlKitQualitiesFlt.Enabled = false;
		}
	}
}