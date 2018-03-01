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
	/// [Příjem zboží] Objednávka závozu/naskladnění repasovaného zboží na ND (RF0 - Refurbished Order)
	/// </summary>
	public partial class VrRepaseRF0 : BasePage
	{
		/// <summary>
		/// MessageStatusId
		/// </summary>
		private const int COLUMN_MESSAGE_STATUS_ID = 16;
				
		//seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		private int[] hideGrdDataColumns = new int[] { 4, 5, 14, COLUMN_MESSAGE_STATUS_ID };

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
				BaseHelper.FillDdlMessageStatuses(ref this.ddlMessageStatusFlt);
				BaseHelper.FillDdlUserModify(ref this.ddlUsersModifyFlt);
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
				this.grdData.SelectedIndex = -1;				
				BC.UnbindDataFromObject<GridView>(this.grdData);
				this.grdPager.CurrentIndex = currPageIndx;
				this.fillPagerData(currPageIndx);
			}
		}

		private void fillPagerData(int pageNo)
		{
			this.pnlR1.Visible = false;
			this.pnlGvItems.Visible = false; 
			BC.UnbindDataFromObject<GridView>(this.gvRF1, this.gvItems);
						
			string proW = " IsActive = 1";
			
			if (this.ddlCityName.SelectedValue.ToString() != "-1") proW += " AND [CustomerCity] like '" + this.ddlCityName.SelectedItem.Text.Trim() + "%'";
			if (this.ddlCompanyName.SelectedValue.ToString() != "-1") proW += " AND [CompanyName] like '" + this.ddlCompanyName.SelectedItem.Text.Trim() + "%'";						
			if (this.ddlUsersModifyFlt.SelectedValue.ToString() != "-1") proW += " AND [ModifyUserId] = " + this.ddlUsersModifyFlt.SelectedValue.ToString();
			if (!string.IsNullOrWhiteSpace(this.tbxDatumOdeslaniFlt.Text.Trim())) proW += " AND CONVERT(CHAR(8),[MessageDateOfShipment], 112) = '" + WConvertDateToYYYYmmDD(this.tbxDatumOdeslaniFlt.Text.Trim()) + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxDatumDodaniFlt.Text.Trim())) proW += " AND CONVERT(CHAR(8),[DateOfDelivery], 112) = '" + WConvertDateToYYYYmmDD(this.tbxDatumDodaniFlt.Text.Trim()) + "'";
			if (this.ddlMessageStatusFlt.SelectedValue.ToString() != "-1") proW += " AND [MessageStatusId] = " + this.ddlMessageStatusFlt.SelectedValue.ToString();
						
			string proS = "*";
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwCMRF0Sent]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = proS;
			pagerData.WhereClause = proW;

			try
			{				
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, this.hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

				ImageButton img = new ImageButton();
				foreach (GridViewRow gvr in this.grdData.Rows)
				{
					if (gvr.Cells[14].Text == "2")
					{
						img = (ImageButton)gvr.FindControl("btnRF1new");
						img.Enabled = true;
						img.Visible = true;
					}
					else
					{
						img = (ImageButton)gvr.FindControl("btnRF1new");
						img.Enabled = false;
						img.Visible = false;
					}
				}
								
				BaseHelper.SetPictureDeleteOrder(ref grdData, COLUMN_MESSAGE_STATUS_ID, "12");
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
				this.grdPager.Visible = false; 
				this.grdData.Visible = false;
			}			
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridViewRow drv = grdData.SelectedRow;
			ViewState["vwCMRSentID"] = drv.Cells[2].Text;

			this.pnlR1.Visible = false;
			this.gvRF1.SelectedIndex = -1; 
			BC.UnbindDataFromObject<GridView>(this.gvRF1);

			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered]" +
									 ",[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId]" +
									 ",[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  FROM [dbo].[vwCMRF0SentIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, grdData.SelectedValue);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvItems.DataSource = myDataTable.DefaultView; 
				this.gvItems.DataBind();
				this.gvItems.Visible = true;
				this.gvItems.SelectedIndex = -1;
				this.pnlGvItems.Visible = true;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";
			ClearViewControls(vwEdit);
			this.tbxKitsQuantity.Text = "1";
			this.lblErrInfo.Text = "";
			BC.UnbindDataFromObject<GridView>(this.grdData, this.gvKitsOrItemsNew);

			this.mvwMain.ActiveViewIndex = 1;

			String proS = string.Empty;

			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ! ' ctext UNION ALL " +
			" SELECT ID cValue, [Name] ctext FROM [dbo].[cdlStocks] WHERE [IsActive]=1) xx ORDER BY ctext";
			FillDdl(ref this.ddlStock, proS);
			try
			{ this.ddlStock.SelectedValue = "2"; }
			catch (Exception)
			{ this.ddlStock.SelectedValue = "-1"; }
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
						" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwKitsHd] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			FillDdl(ref this.ddlKits, proS);
			// ***************************************************************************************************************
			proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwItems] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			FillDdl(ref this.ddlNW, proS);
			// ***************************************************************************************************************
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlQualities] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlKitQualities, proS);
			FillDdl(ref this.ddlItemQuality, proS);
			proS = "SELECT * FROM (SELECT '-1' cValue,' ' ctext UNION ALL " +
				  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlQualities] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlKitQuality, proS);			
			// ***************************************************************************************************************
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlKitGroups] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlKitGroups, proS);
			// ***************************************************************************************************************
			//proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
			//	  " SELECT ID cValue,[DescriptionCz] ctext FROM [dbo].[cdlIncoterms] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			//FillDdl(ref this.ddlIncoterms, proS);
			//this.ddlIncoterms.SelectedValue = "2";
		}

		protected void btnPridatCpeDoSoupravy_Click(object sender, EventArgs e)
		{
			AddRec("KIT");
		}

		protected void btnPridatNwDoSoupravy_Click(object sender, EventArgs e)
		{
			AddRec("NW");
		}

		private void AddRec(string par1)
		{
			string Err = string.Empty;
			this.lblErrInfo.Text = string.Empty;
			if (this.ddlStock.SelectedValue != "-1" && this.ddlDestinationPlaces.SelectedValue != "-1" && this.tbxDateOfDelivery.Text.Trim() != string.Empty && (this.ddlKitQuality.SelectedValue.ToString() != "-1" && par1 == "KIT" || this.ddlItemQuality.SelectedValue.ToString() != "-1" && par1 == "NW"))
			{
				if (this.ddlKits.SelectedValue != "-1" && par1 == "KIT" || this.ddlNW.SelectedValue != "-1" && par1 != "KIT")
				{
					DataTable myT = new DataTable("myDt");
					ZalozTabulku(ref myT);
					ZpracujTabulku(ref myT);

					string proS = string.Empty;
					if (par1 == "KIT")
					{
						proS = string.Format("SELECT 1 ItemVerKit,[ID] [ItemOrKitID],[Code],[DescriptionCz],[DescriptionEng],[MeasuresId],[MeasuresCode],[KitQualitiesId],[KitQualitiesCode]" +
							",[IsSent],[SentDate],[GroupsId],[Packaging],[IsActive],[ModifyDate],[ModifyUserId]" +
							"  FROM [dbo].[cdlKits] WHERE IsActive = 1 AND  id={0}", this.ddlKits.SelectedValue.ToString());
					}
					else
					{
						proS = string.Format("SELECT 0 ItemVerKit, ID [ItemOrKitID],Code,[DescriptionCz],[DescriptionEng],[MeasuresId],[ItemTypesId],[PackagingId],[GroupsId],[ItemType],[PC],[Packaging],[IsSent],[SentDate]" +
							 ",[ItemTypeDesc1],[ItemTypeDesc2],[IsActive],[ModifyDate],[ModifyUserId] FROM [dbo].[cdlItems] WHERE IsActive = 1 AND id={0}", this.ddlNW.SelectedValue.ToString());
					}

					DataTable myDataTable = BC.GetDataTable(proS);

					if (myDataTable != null && myDataTable.Rows.Count == 1)
					{
						// **********************************************************************************************

						DataRow Dolx = myT.NewRow();
						Dolx[0] = 1;	                                                      // AnoNe
						if (par1 == "KIT")                                                    // ItemOrKitQuantity
							Dolx[1] = WConvertStringToInt32(this.ddlKits.SelectedValue.ToString());   // ID
						else
							Dolx[1] = WConvertStringToInt32(this.ddlNW.SelectedValue.ToString());     // ID
						Dolx[2] = Convert.ToInt32(Convert.ToBoolean(myDataTable.Rows[0][0]));         // ItemVerKit
						Dolx[3] = WConvertStringToInt32(myDataTable.Rows[0][1].ToString());   // ItemOrKitID
						Dolx[4] = myDataTable.Rows[0][2].ToString();                          // ItemOrKitCode
						Dolx[5] = myDataTable.Rows[0][3].ToString();                          // DescriptionCzItemsOrKit
						if (par1 == "KIT")                                                    // ItemOrKitQuantity
							Dolx[6] = Convert.ToDecimal(this.tbxKitsQuantity.Text.Trim().Replace(".", ","));
						else
							Dolx[6] = Convert.ToDecimal(this.tbxNwQuantity.Text.Trim().Replace(".", ","));
						Dolx[7] = 0;                                                          // PackageTypeId
						Dolx[8] = this.ddlStock.SelectedItem.Text;                            // cdlStocksName
						Dolx[9] = WConvertStringToInt32(this.ddlDestinationPlaces.SelectedValue);    // DestinationPlacesId
						Dolx[10] = this.ddlDestinationPlaces.SelectedItem.Text;                      // DestinationPlacesName
						//Dolx[11] = WConvertStringToInt32(this.ddlDestinationPlacesContacts.SelectedValue);  // DestinationPlacesContactsId
						//Dolx[12] = this.ddlDestinationPlacesContacts.SelectedItem.Text;                     // DestinationPlacesContactsName
						Dolx[13] = WConvertDateToYYYYmmDD(this.tbxDateOfDelivery.Text.Trim());                     // DateOfDelivery
						Dolx[14] = WConvertStringToInt32(this.ddlStock.SelectedValue);                             // cdlStocksId                // IncotermsId

						if (par1 == "KIT")
						{
							Dolx[15] = WConvertStringToInt32(this.ddlKitQuality.SelectedValue);   // QualityID
							Dolx[16] = this.ddlKitQuality.SelectedItem.Text;  // QualityText
						}
						else
						{
							Dolx[15] = WConvertStringToInt32(this.ddlItemQuality.SelectedValue);   // QualityID
							Dolx[16] = this.ddlItemQuality.SelectedItem.Text;  // QualityText

						}
						myT.Rows.Add(Dolx);
						
						this.gvKitsOrItemsNew.Columns[7].Visible = true; this.gvKitsOrItemsNew.Columns[9].Visible = true; this.gvKitsOrItemsNew.Columns[11].Visible = true;
						gvKitsOrItemsNew.DataSource = myT.DefaultView;
						gvKitsOrItemsNew.DataBind();
						this.gvKitsOrItemsNew.Columns[7].Visible = false; this.gvKitsOrItemsNew.Columns[9].Visible = false; this.gvKitsOrItemsNew.Columns[11].Visible = false;
					}
				}
				else
				{
					Err += "Vyberte kit nebo item<br />";
				}
			}
			else
			{
				Err += "Zkontrolujte požadované datum, zdrojové místo závozu, sklad a kvalitu<br />";
			}
			this.lblErrInfo.Text = Err;
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlNW.Items.Clear();
			this.ddlKits.Items.Clear();			
			this.grdData.SelectedIndex = -1;
			BC.UnbindDataFromObject<GridView>(this.gvKitsOrItemsNew);
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		private void ZalozTabulku(ref DataTable myDt)
		{
			DataColumn myDataColumn;

			myDataColumn = new DataColumn();
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
			myDataColumn.ColumnName = "DateOfDelivery";                // DateOfExpedition
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "StockId";                    // ddlStock
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "QualityID";                    // QualityID
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "QualityText";                // QualityText
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);
		} 

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				bool mOk = true;
				this.lblErrInfo.Text = "";
				string Err = string.Empty;
				
				DataTable myT = new DataTable("myDt");
				ZalozTabulku(ref myT);
				ZpracujTabulku(ref myT);
				if (!(myT != null && myT.Rows.Count > 0))
				{
					Err += "Obsahuje objednávka expedice nějaké položky? <br />"; mOk = false;
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
							sb.Append("<RF0>");
							sb.Append("<ID>" + r[1].ToString() + "</ID>");     // ID z tabulky [dbo].[CardStockItems]
							sb.Append("<ItemVerKit>" + r[2].ToString() + "</ItemVerKit>");
							sb.Append("<ItemOrKitID>" + r[3].ToString() + "</ItemOrKitID>");
							sb.Append("<ItemOrKitCode>" + r[4].ToString() + "</ItemOrKitCode>");
							sb.Append("<DescriptionCzItemsOrKit>" + r[5].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</DescriptionCzItemsOrKit>");
							sb.Append("<ItemOrKitQuantity>" + r[6].ToString() + "</ItemOrKitQuantity>");
							sb.Append("<PackageTypeId>" + r[7].ToString() + "</PackageTypeId>");
							sb.Append("<cdlStocksName>" + r[8].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</cdlStocksName>");
							sb.Append("<DestinationPlacesId>" + r[9].ToString() + "</DestinationPlacesId>");
							sb.Append("<DestinationPlacesName>" + r[10].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</DestinationPlacesName>");
							sb.Append("<DestinationPlacesContactsId>" + r[11].ToString() + "</DestinationPlacesContactsId>");
							sb.Append("<DestinationPlacesContactsName>" + r[12].ToString() + "</DestinationPlacesContactsName>");
							sb.Append("<DateOfDelivery>" + r[13].ToString() + "</DateOfDelivery>");
							sb.Append("<StockId>" + r[14].ToString() + "</StockId>");
							sb.Append("<QualityId>" + r[15].ToString() + "</QualityId>");
							sb.Append("<QualityText>" + r[16].ToString() + "</QualityText>");
							sb.Append("</RF0>");
						}
					}
					sb.Append("</NewDataSet>");


					string help = sb.ToString().Replace("{", "").Replace("}", "");

					XmlDocument doc = new XmlDocument();
					doc.LoadXml(help);
					SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
					SqlCommand sqlComm = new SqlCommand();
					sqlComm.CommandType = CommandType.StoredProcedure;
					sqlComm.CommandText = "[dbo].[prRORF0ins]";  // *********
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
					sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString());

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
						if (i == -1 || i != 0) { Err += "Záznam <b>nebyl</b> uložen! <br />" + WConvertStringToInt32(sqlComm.Parameters["@ReturnMessage"].Value.ToString()); ; mOk = false; }

					}
					catch (Exception ex)
					{
						BC.ProcessException(ex, AppLog.GetMethodName());
						mOk = false;
						Err += "Záznam <b>nebyl</b> uložen! <br />";
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
					this.lblErrInfo.Text = Err;
				}

				if (mOk)
				{
					Session["IsRefresh"] = "1";
					btnBack_Click(btnBack, EventArgs.Empty);
				}
				else
				{
					if (Session["IsRefresh"].ToString() == "1")
					{
						btnBack_Click(btnBack, EventArgs.Empty);
					}
				}
			}
			else
			{
				btnBack_Click(btnBack, EventArgs.Empty);
			}			
		} 

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "OrderView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();

				this.pnlR1.Visible = false;
				OrderView(id);
			}

			if (e.CommandName == "RF1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMRSentID"] = id.ToString();

				this.pnlGvItems.Visible = false;
				this.pnlR1.Visible = true;
				RF1New(id);
				Session["IsRefresh"] = "0";
			}

			if (e.CommandName == "btnExcelConfClicked")
			{
				ExcelConfClicked();
			}

			if (e.CommandName == "btnOznacit")
			{
				foreach (GridViewRow r in grdData.Rows)
				{
					CheckBox ckb = (CheckBox)r.FindControl("chkbExcel");
					ckb.Checked = !ckb.Checked;
				}
			}

			if (e.CommandName == "DeleteOrder")
			{
				string confirmValue = Request.Form["confirm_delete_order"];
				if (confirmValue.ToUpper() == "ANO")
				{
					BaseHelper.ProcessDeleteOrder(e, ref grdData, "12", "RefurbishedOrder", Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString()));
				}
			}
		}

		protected void OrderView(int id)
		{
			ExcelConf(id.ToString());
		}

		protected void ddlKitGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			
			if (this.ddlKitGroups.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwKitsHd] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwKitsHd] WHERE [IsActive]=1  AND GroupsId = {0} ) xx ORDER BY ctext", this.ddlKitGroups.SelectedValue);
			}
			FillDdl(ref this.ddlKits, proS);
			this.ddlKitQualities.SelectedValue = "-1";
		}

		protected void ddlKitQualities_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			// ***************************************************************************************************************
			if (this.ddlKitQualities.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwKitsHd] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwKitsHd] WHERE [IsActive]=1  AND KitQualitiesId = {0} ) xx ORDER BY ctext", this.ddlKitQualities.SelectedValue);
			}
			FillDdl(ref this.ddlKits, proS);
			this.ddlKitGroups.SelectedValue = "-1";
		}  // ddlKitQualities_SelectedIndexChanged

		protected void ddlItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			if (this.ddlItemType.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwItems] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwItems] WHERE [IsActive]=1 AND [ItemType] ='{0}') xx ORDER BY ctext", this.ddlItemType.SelectedValue);
			}
			FillDdl(ref this.ddlNW, proS);
			this.ddlGroupGoods.SelectedValue = "-1";
		} // ddlItemType_SelectedIndexChanged

		protected void ddlGroupGoods_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = string.Empty;
			if (this.ddlGroupGoods.SelectedValue == "-1")
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwItems] WHERE [IsActive]=1 ) xx ORDER BY ctext", "");
			}
			else
			{
				proS = string.Format("SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
							" SELECT ID cValue,LEFT([Code]+' - '+[DescriptionCz],100) ctext FROM [dbo].[vwItems] WHERE [IsActive]=1 AND GroupGoods ='{0}') xx ORDER BY ctext", this.ddlGroupGoods.SelectedItem.Text);
			}
			FillDdl(ref this.ddlNW, proS);
			this.ddlItemType.SelectedValue = "-1";
		}  // ddlGroupGoods_SelectedIndexChanged

		private void ZpracujTabulku(ref DataTable myDt)
		{

			if (this.gvKitsOrItemsNew != null && gvKitsOrItemsNew.Rows.Count > 0)
			{
				foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
				{
					DataRow Doly = myDt.NewRow();
					CheckBox myChkb;
					myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					Doly[0] = (myChkb.Checked) ? 1 : 0;	                    // AnoNe
					Doly[1] = WConvertStringToInt32(gvr.Cells[1].Text);     // ID    
					Doly[2] = WConvertStringToInt32(gvr.Cells[2].Text);     // ItemVerKit
					Doly[3] = WConvertStringToInt32(gvr.Cells[3].Text);     // ItemOrKitID
					Doly[4] = HttpUtility.HtmlDecode(gvr.Cells[4].Text);    // ItemOrKitCode
					Doly[5] = HttpUtility.HtmlDecode(gvr.Cells[5].Text);    // DescriptionCzItemsOrKit
					Doly[6] = Convert.ToDecimal(gvr.Cells[6].Text);         // ItemOrKitQuantity
					Doly[7] = WConvertStringToInt32(gvr.Cells[7].Text);     // PackageTypeId
					Doly[8] = HttpUtility.HtmlDecode(gvr.Cells[8].Text);    // cdlStocksName
					Doly[9] = WConvertStringToInt32(gvr.Cells[9].Text);     // DestinationPlacesId
					Doly[10] = HttpUtility.HtmlDecode(gvr.Cells[10].Text);  // DestinationPlacesName
					Doly[11] = WConvertStringToInt32(gvr.Cells[11].Text);   // DestinationPlacesContactsId
					Doly[12] = HttpUtility.HtmlDecode(gvr.Cells[12].Text);  // DestinationPlacesContactsName
					Doly[13] = HttpUtility.HtmlDecode(gvr.Cells[13].Text);  // DateOfDelivery
					Doly[14] = HttpUtility.HtmlDecode(gvr.Cells[14].Text);  // cdlStocksId
					Doly[15] = WConvertStringToInt32(gvr.Cells[15].Text);   // QualityID
					Doly[16] = HttpUtility.HtmlDecode(gvr.Cells[16].Text);  // QualityText
					myDt.Rows.Add(Doly);
				}
			}

		}

		protected void ddlKits_SelectedIndexChanged(object sender, EventArgs e)
		{
			string proS = "SELECT [KitQualitiesId] FROM [dbo].[cdlKits] WHERE ID =  " + this.ddlKits.SelectedValue.ToString();
			DataTable myT = new DataTable();
			myT = BC.GetDataTable(proS);
			try
			{
				this.ddlKitQuality.SelectedValue = myT.Rows[0][0].ToString();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}
		}

		protected void RF1New(int id)
		{
			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered]" +
									 ",[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId]" +
									 ",[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  FROM [dbo].[vwCMRF0SentIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, id);

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvRF1.Columns[8].Visible = true;
				this.gvRF1.DataSource = myDataTable.DefaultView; this.gvRF1.DataBind();
				this.gvRF1.SelectedIndex = -1;
				this.gvRF1.Columns[8].Visible = false;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
			}

			proS = string.Format("SELECT TOP 1 cDp.[CompanyName] [CustomerName] ,cDp.[StreetName] [CustomerAddress1] ,cDp.[StreetHouseNumber] [CustomerAddress2] ,cDp.[StreetOrientationNumber] [CustomerAddress3] ,cDp.[City] [CustomerCity] ,cDp.[ZipCode] [CustomerZipCode]" +
								 ",vw.[DateOfDelivery] [RequiredDateOfShipment], cDp.IsActive " +
								 "FROM [dbo].[vwRefurbishedConfirmationHd]   vw INNER JOIN [dbo].[cdlDestinationPlaces] cDp   ON vw.[CustomerID]=cDp.ID WHERE vw.[RefurbishedOrderID] = {0}", id);

			DataTable myDT;
			myDT = BC.GetDataTable(proS);
			this.lblxCustomerNameValue.Text = myDT.Rows[0]["CustomerName"].ToString();
			this.lblxCustomerCityValue.Text = myDT.Rows[0]["CustomerCity"].ToString();
			this.lblxCustomerZipCodeValue.Text = myDT.Rows[0]["CustomerZipCode"].ToString();
			this.lblxRequiredDateOfShipmentValue.Text = wConvertStringToDatedd_mm_yyyy(myDT.Rows[0]["RequiredDateOfShipment"].ToString());
			this.lblxCustomerAddress1Value.Text = myDT.Rows[0]["CustomerAddress1"].ToString();
			this.lblxCustomerAddress2Value.Text = myDT.Rows[0]["CustomerAddress2"].ToString();


		}

		protected void btnRF1Save_Click(object sender, EventArgs e)
		{
			if (Session["IsRefresh"].ToString() == "0" || Session["IsRefresh"] == null)
			{
				// kontrola hodnot
				bool mOK = true; string Err = string.Empty; int ii = 0; int iMaSmysl = 0;
				TextBox tbx = new TextBox();
				foreach (GridViewRow gvr in this.gvRF1.Rows)
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
					dt = BC.GetDataTable("SELECT min([MessageId]) as minMessageId FROM [dbo].[CommunicationMessagesRefurbishedOrderConfirmation]", BC.FENIXRdrConnectionString);
					int iMessageID = WConvertStringToInt32(dt.Rows[0][0].ToString()) - 1;
					CultureInfo culture = new CultureInfo("cs-CZ");
					StringBuilder sb = new StringBuilder();

					sb.Append("<NewDataSet>");
					foreach (GridViewRow gvr in this.gvRF1.Rows)
					{
						try
						{
							tbx = (TextBox)gvr.FindControl("tbxQuantity");
							if (!string.IsNullOrEmpty(tbx.Text.Trim()))
							{
								ii = Convert.ToInt32(tbx.Text.Trim());
								sb.Append("<item>");
								sb.Append("<ID>" + HttpUtility.HtmlDecode(gvr.Cells[0].Text).Trim() + "</ID>");
								sb.Append("<ItemQuantity>" + ii.ToString() + "</ItemQuantity>");
								sb.Append("<iMessageID>" + iMessageID.ToString() + "</iMessageID>");
								sb.Append("<vwCMRSentID>" + ViewState["vwCMRSentID"].ToString() + "</vwCMRSentID>");

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
					com.CommandText = "prRefurbishedConfirmationManuallyIns";
					com.CommandType = CommandType.StoredProcedure;
					com.Connection = con;
					com.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
					com.Parameters.Add("ModifyUserId", SqlDbType.Int);
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
						BC.ProcessException(ex, AppLog.GetMethodName(), "program prRefurbishedConfirmationManuallyIns");
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
					btnRF1Back_Click(btnRF1Back, EventArgs.Empty);
				}
				else
				{
					btnRF1Back_Click(btnRF1Back, EventArgs.Empty);
				}
			}
		}

		protected void btnRF1Back_Click(object sender, EventArgs e)
		{			
			this.pnlR1.Visible = false;
			this.grdData.SelectedIndex = -1;
			this.gvRF1.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
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
			try
			{
				string setIds = sb.ToString().Substring(0, (sb.ToString().Length) - 1);
				ExcelConf(setIds);
			}
			catch (Exception)
			{
			}
			finally { sb = null; }
		}

		protected void ExcelConf(String setIds)
		{
			string ii = string.Empty;
			string proS = string.Empty;
			//CheckBox chkb = new CheckBox();
			//StringBuilder sb = new StringBuilder();
			//foreach (GridViewRow drv in grdData.Rows)
			//{
			//	ii = drv.Cells[2].Text;
			//	chkb = (CheckBox)drv.FindControl("chkbExcel");
			//	if (chkb.Checked) sb.Append(ii + ",");
			//}
			if (!string.IsNullOrWhiteSpace(setIds))
			{
				string proW = " WHERE RefurbishedOrderID in (" + setIds + ") AND It.ISACTIVE=1  AND [Reconciliation] IN (0, 1)";
				proS = "SELECT It.ID [ItID],It.[CMSOId],It.[ItemVerKit],It.[ItemOrKitID],It.[ItemOrKitDescription],It.[ItemOrKitQuantity]" +
					",CAST(It.[ItemOrKitQuantity] AS INT) ItemOrKitQuantityInt ,It.[ItemOrKitUnitOfMeasureId],It.[ItemOrKitUnitOfMeasure]              " +
					",It.[ItemOrKitQualityId]  ,It.[ItemOrKitQualityCode],It.[IncotermsId],It.[IncotermDescription],It.[NDReceipt]                     " +
					",It.[KitSNs],It.[IsActive] ,It.[ModifyDate],It.[ModifyUserId],CAST(COI.ItemOrKitQuantity AS INT) COIItemOrKitQuantityInt           " +
					",CAST(COI.ItemOrKitQuantityDelivered AS INT) ItemOrKitQuantityDeliveredInt ,C.[ID]  ,C.[MessageId],C.[MessageTypeId]             " +
					",C.[MessageDescription],C.[DateOfShipment],C.[RefurbishedOrderID],C.[CustomerID]                                                  " +
					",C.[Reconciliation] " +
					",O.[MessageDateOfShipment],O.[DateOfDelivery],cDP.[CompanyName],cDP.[City] ,SN1, SN2,                                             " +
					"CASE It.[ItemVerKit] WHEN 0 THEN 'Item' WHEN 1 THEN 'Kit' ELSE '?' END  ItemVerKitText                                            " +
					"FROM [dbo].[CommunicationMessagesRefurbishedOrderConfirmationItems] It  " +
					"INNER JOIN [dbo].[CommunicationMessagesRefurbishedOrderConfirmation] C  " +
					"ON It.CMSOId = C.ID  " +
					"INNER JOIN [dbo].[CommunicationMessagesRefurbishedOrder]   O  " +
					"ON C.RefurbishedOrderID = O.ID  " +
					"LEFT OUTER JOIN [dbo].[cdlDestinationPlaces] cDP                        " +
					"ON O.[CustomerID] = cDP.ID " +
					" LEFT OUTER JOIN [dbo].[CommunicationMessagesRefurbishedOrderItems]  COI                          " +
					" ON O.ID = COI.CMSOId AND It.[ItemVerKit] = COI.ItemVerKit AND It.[ItemOrKitID] = COI.ItemOrKitID " +
					"AND It.[ItemOrKitQualityId] = COI.ItemOrKitQualityId                                              " +
					"LEFT OUTER JOIN [dbo].[CommunicationMessagesRefurbishedOrderConfirmationSerNumSent]SNS            " +
					"ON It.ID = SNS.RefurbishedItemsOrKitsID " + proW + " ORDER BY ID,ItID ";
				DataTable myDt = new DataTable();
				myDt = BC.GetDataTable(proS);
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
								worksheet.Cells[radek, 7].Value = BC.ExcelGetReconciliationDescription(dr["Reconciliation"].ToString()).ToUpper();
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
							worksheet.Column(11).AutoFit();
							worksheet.Column(12).AutoFit();
							worksheet.Column(13).AutoFit();
							worksheet.Column(14).AutoFit();
							worksheet.Column(15).AutoFit();
							worksheet.Column(16).AutoFit();
							worksheet.Column(17).AutoFit();


							//worksheet.Cells["A1"].Value = "Sériová čísla";
							//worksheet.Cells["A1"].Style.Font.Bold = true;
							//worksheet.Cells["A1"].Style.Font.UnderLine = true;

							//worksheet.Cells["A2"].Value = "SN1";
							//worksheet.Cells["B2"].Value = "SN2";
														
							// lets set the header text 
							//worksheet.HeaderFooter.OddHeader.CenteredText = "Tinned Goods Sales";
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

						}
					}

					// **
				}
			}
			//sb = null;
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}
	}
}