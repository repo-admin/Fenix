using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	/// <summary>
	/// [Správa] Číselník kitů
	/// </summary>
	public partial class KiKittingCodeList : BasePage
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
				BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);
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
				this.pnlKiItems.Visible = false;
				this.gvKiItems.DataSource = null; this.gvKiItems.DataBind();
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
			string proW = "1=1";
			if (this.ddlIsActive.SelectedValue.ToString() != "-1") proW += " AND IsActive=" + this.ddlIsActive.SelectedValue.ToString();
			if (!string.IsNullOrWhiteSpace(this.tbxCodeFlt.Text.Trim())) proW += " AND CODE='" + this.tbxCodeFlt.Text.Trim() + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxDescriptionCzFlt.Text.Trim())) proW += " AND DescriptionCz like '" + this.tbxDescriptionCzFlt.Text.Trim() + "%'";
			if (this.ddlKitQualitiesFlt.SelectedValue.ToString() != "-1") proW += " AND [KitQualitiesId] =" + this.ddlKitQualitiesFlt.SelectedValue.ToString();
			string proS = "[ID] ,Code ,[DescriptionCz] ,[DescriptionEng] ,[IsSent]  ,[SentDate], [MeasuresCode], [KitQualitiesCode],Packaging,GroupsCode,IsActive";

			PagerData pagerData = new PagerData();			
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwKitsHd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = proS;
			pagerData.WhereClause = proW;

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

				gvKiItems.Visible = (pagerData.ItemCount > 0 ? true : false);
			}
			catch (Exception ex)
			{				
				this.grdPager.Visible = false; 
				this.gvKiItems.Visible = false;
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}			
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlKiItems.Visible = true;
			BC.UnbindDataFromObject<GridView>(this.gvKiItems);

			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [cdlKitsItemsID] ,[cdlKitsID],[ItemVerKit],[ItemOrKitID],[ItemCode]" +
						",[DescriptionCzKit],[DescriptionCzItemsOrKit],[ItemOrKitQuantityInt] ,[PackageType],ItemVerKitText" +
						  " FROM [dbo].[vwKitsIt] WHERE [IsActive] = {0} AND cdlKitsID = {1}", 1, grdData.SelectedValue);
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvKiItems.DataSource = myDataTable.DefaultView; this.gvKiItems.DataBind();
				this.gvKiItems.Visible = true;
				this.gvKiItems.SelectedIndex = -1;
			}
			catch
			{
			}
		}

		protected void gvKiItems_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";			
			this.tbxCpeQuantity.Text = "1";
			this.lblErrInfo.Text = "";
			base.ClearViewControls(vwEdit);
			BC.UnbindDataFromObject<GridView>(this.gvKiItems, this.grdData, this.gvItemsNew);

			this.mvwMain.ActiveViewIndex = 1;

			string proS = string.Empty;
			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
						" SELECT ID cValue,LEFT([DescriptionCz]+' ('+[GroupGoods]+'-'+[Code]+')',100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null AND ItemType='CPE') xx ORDER BY ctext";
			FillDdl(ref this.ddlCPE, proS);

			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
				   " SELECT ID cValue,LEFT(LTRIM([DescriptionCz])+' ('+[GroupGoods]+'-'+[Code]+')',100) ctext FROM [dbo].[cdlItems] WHERE [IsActive]=1 AND PC is not null AND ItemType<>'CPE' AND ItemType<>'NW') xx ORDER BY ctext";
			FillDdl(ref this.ddlNW, proS);

			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
		          " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlMeasures] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlMeasures, proS);

			BaseHelper.FillDdlQualities(ref this.ddlKitQualities);

			proS = "SELECT * FROM (SELECT '-1' cValue,' VYBERTE ' ctext UNION ALL " +
                  " SELECT ID cValue,[Code] ctext FROM [dbo].[cdlKitGroups] WHERE [IsActive]=1 ) xx ORDER BY ctext";
			FillDdl(ref this.ddlKitGroups, proS);
		}

		protected void btnPridatCpeDoSoupravy_Click(object sender, EventArgs e)
		{
			AddRec("CPE");
		}

		protected void btnPridatNwDoSoupravy_Click(object sender, EventArgs e)
		{
			AddRec("NW");
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0")) { 
			bool mOk = true;
			this.lblErrInfo.Text = "";
			string Err = string.Empty;
			if (this.ddlKitQualities.SelectedValue.ToString() == "-1" || this.ddlKitQualities == null) { Err += "Chybný výběr kvality Kitu <br />"; mOk = false; }
			if (this.ddlMeasures.SelectedValue.ToString() == "-1" || this.ddlMeasures == null) { Err += "Chybný výběr měrné jednotky Kitu <br />"; mOk = false; }
			if (string.IsNullOrWhiteSpace(this.tbxDescriptionCz.Text.Trim())) { Err += "Český popis Kitu je povinný údaj <br />"; mOk = false; }
			
			DataTable myT = new DataTable("myDt");
			ZalozTabulku(ref myT);
			
			if (this.gvItemsNew != null && gvItemsNew.Rows.Count > 0)
			{
				foreach (GridViewRow gvr in gvItemsNew.Rows)
				{
					try
					{
						DataRow Doly = myT.NewRow();
						CheckBox myChkb;
						myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
						Doly[0] = (myChkb.Checked) ? 1 : 0;	             // AnoNe
						Doly[1] = Convert.ToInt32(gvr.Cells[1].Text);    // ItemVerKit
						Doly[2] = Convert.ToInt32(gvr.Cells[2].Text);    // ItemOrKitID
						Doly[3] = HttpUtility.HtmlDecode(gvr.Cells[3].Text);    // ItemGroupGoods
						Doly[4] = gvr.Cells[4].Text;                            // ItemCode
						Doly[5] = HttpUtility.HtmlDecode(gvr.Cells[5].Text);    // DescriptionCzItemsOrKit
						Doly[6] = Convert.ToDecimal(gvr.Cells[6].Text);  // ItemOrKitQuantity

						myT.Rows.Add(Doly);

					}
					catch (Exception)
					{
						mOk = false;
					}
				}
			}
			else 
			{
				Err += "Obsahuje Kit nějaké položky? <br />"; mOk = false;
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
						sb.Append("<KittingCodeList>");
						sb.Append("<ItemVerKit>" + r[1].ToString() + "</ItemVerKit>");
						sb.Append("<ItemOrKitID>" + r[2].ToString() + "</ItemOrKitID>");
						sb.Append("<ItemGroupGoods>" + r[3].ToString() + "</ItemGroupGoods>");
						sb.Append("<ItemCode>" + r[4].ToString() + "</ItemCode>");
						sb.Append("<DescriptionCzItemsOrKit>" + r[5].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</DescriptionCzItemsOrKit>");
						sb.Append("<ItemOrKitQuantity>" + r[6].ToString() + "</ItemOrKitQuantity>");
						sb.Append("</KittingCodeList>");
					}
				}
				sb.Append("</NewDataSet>");
				
				string help = sb.ToString().Replace("{", "").Replace("}", "");
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(help);
				
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prKiCLins]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@Code", SqlDbType.NVarChar, 5).Value = this.tbxCode.Text.Trim();
				sqlComm.Parameters.Add("@DescriptionCz", SqlDbType.NVarChar, 50).Value = this.tbxDescriptionCz.Text.Trim();
				sqlComm.Parameters.Add("@DescriptionEng", SqlDbType.NVarChar, 50).Value = this.tbxDescriptionEng.Text.Trim();
				sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
				sqlComm.Parameters.Add("@MeasuresId", SqlDbType.Int).Value = WConvertStringToInt32(this.ddlMeasures.SelectedValue.ToString());
				sqlComm.Parameters.Add("@MeasuresCode", SqlDbType.VarChar, 50).Value = this.ddlMeasures.SelectedItem.Text;
				sqlComm.Parameters.Add("@KitQualitiesId", SqlDbType.Int).Value = WConvertStringToInt32(this.ddlKitQualities.SelectedValue.ToString());
				sqlComm.Parameters.Add("@KitQualitiesCode", SqlDbType.VarChar, 50).Value = this.ddlKitQualities.SelectedItem.Text;
				sqlComm.Parameters.Add("@KitPackaging", SqlDbType.Int).Value = WConvertStringToInt32(this.tbxPackaging.Text.Trim());
				sqlComm.Parameters.Add("@KitGroupsId", SqlDbType.Int).Value = WConvertStringToInt32(this.ddlKitGroups.SelectedValue.ToString());
				sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int);

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
				catch (Exception)
				{
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
			else {
				this.lblErrInfo.Text = Err;
			}
			if (mOk) { Session["IsRefresh"] = "1"; btnBack_Click(btnBack, EventArgs.Empty); }
		}
			else
			{
				if (Session["IsRefresh"].ToString() == "1")
				{
					this.fillPagerData(BC.PAGER_FIRST_PAGE);
					this.mvwMain.ActiveViewIndex = 0;
					this.gvItemsNew.DataSource = null; this.gvItemsNew.DataBind();
				}
			}

		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;
			this.ddlNW.Items.Clear();
			this.ddlCPE.Items.Clear();			
			this.pnlKiItems.Visible = false;
			this.grdData.SelectedIndex = -1;
			BC.UnbindDataFromObject<GridView>(this.gvKiItems);
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		private void ZalozTabulku(ref DataTable myDt)
		{
			DataColumn myDataColumn;

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Boolean");
			myDataColumn.ColumnName = "AnoNe";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			//myDataColumn = new DataColumn();
			//myDataColumn.DataType = System.Type.GetType("System.Int32");
			//myDataColumn.ColumnName = "Id";
			//// Add the Column to the DataColumnCollection.
			//myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ItemVerKit";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ItemOrKitId";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "ItemGroupGoods";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "ItemCode";
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
			myDataColumn.ColumnName = "PackageType";
			// Add the Column to the DataColumnCollection.
			myDt.Columns.Add(myDataColumn);

		} // ZalozTabulku

		private void AddRec(string par1)
		{
			DataTable myT = new DataTable("myDt");
			ZalozTabulku(ref myT);

			int mOK = 1;               // 2015-03-04
			this.lblErrInfo.Text = ""; // 2015-03-04

			if (this.gvItemsNew != null && gvItemsNew.Rows.Count > 0)
			{
				foreach (GridViewRow gvr in gvItemsNew.Rows)
				{
					// 2015-03-04  start
					if (par1 == "CPE")
					{
						if (this.ddlCPE.SelectedValue.ToString() == gvr.Cells[2].Text.Trim()) { mOK = 0; this.lblErrInfo.Text = "Položka <b>" + this.ddlCPE.SelectedItem.Text.ToString() + "</b> je ve výběru již obsažena<br/>"; }
					}
					else
					{
						if (this.ddlNW.SelectedValue.ToString() == gvr.Cells[2].Text.Trim()) { mOK = 0; this.lblErrInfo.Text = "Položka <b>" + this.ddlNW.SelectedItem.Text.ToString() + "</b> je ve výběru již obsažena<br/>"; }
					}
					// 2015-03-04  end

					DataRow Doly = myT.NewRow();
					CheckBox myChkb;
					myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					Doly[0] = (myChkb.Checked) ? 1 : 0;	             // AnoNe
					Doly[1] = Convert.ToInt32(gvr.Cells[1].Text);    // ItemVerKit
					Doly[2] = Convert.ToInt32(gvr.Cells[2].Text);    // ItemOrKitID
					Doly[3] = HttpUtility.HtmlDecode(gvr.Cells[3].Text);    // ItemGroupGoods
					Doly[4] = gvr.Cells[4].Text;                     // ItemCode
					Doly[5] = HttpUtility.HtmlDecode(gvr.Cells[5].Text);  // DescriptionCzItemsOrKit
					Doly[6] = Convert.ToDecimal(gvr.Cells[6].Text);  // ItemOrKitQuantity

					myT.Rows.Add(Doly);
				}
			}

			if (mOK == 1)// 2015-03-04
			{

				string proS = string.Empty;
				if (par1 == "CPE")
				{
					proS = "SELECT [GroupGoods] ,[Code] ,[DescriptionCz] FROM [dbo].[cdlItems] WHERE id=" + this.ddlCPE.SelectedValue.ToString();
				}
				else
				{
					proS = "SELECT [GroupGoods] ,[Code] ,[DescriptionCz] FROM [dbo].[cdlItems] WHERE id=" + this.ddlNW.SelectedValue.ToString();
				}
				DataTable myDataTable;
				myDataTable = BC.GetDataTable(proS);
				if (myDataTable != null && myDataTable.Rows.Count == 1)
				{
					DataRow Dolx = myT.NewRow();
					Dolx[0] = 1;	 // AnoNe
					Dolx[1] = 0;     // zařízení
					if (par1 == "CPE")
						Dolx[2] = Convert.ToInt32(this.ddlCPE.SelectedValue.ToString()); // ItemsId
					else
						Dolx[2] = Convert.ToInt32(this.ddlNW.SelectedValue.ToString()); // ItemsId
					Dolx[3] = myDataTable.Rows[0][0].ToString();                     // ItemGroupGoods
					Dolx[4] = myDataTable.Rows[0][1].ToString();
					Dolx[5] = myDataTable.Rows[0][2].ToString();
					if (par1 == "CPE")
						Dolx[6] = Convert.ToDecimal(this.tbxCpeQuantity.Text.Trim().Replace(".", ","));
					else
						Dolx[6] = Convert.ToDecimal(this.tbxNwQuantity.Text.Trim().Replace(".", ","));

					myT.Rows.Add(Dolx);
				}
				gvItemsNew.DataSource = myT.DefaultView;
				gvItemsNew.DataBind();
			}
			else
			{
				
			}

		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.pnlKiItems.Visible = false;
			this.grdData.SelectedIndex = -1;
			BC.UnbindDataFromObject<GridView>(this.gvKiItems);
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "ZmenaAktivity") {
				int ii = -1; Boolean mOK = true;
				try
				{
					ii = Convert.ToInt32(e.CommandArgument.ToString());
					string proS = string.Format("UPDATE [dbo].[cdlKits] SET [IsActive] =  case [IsActive] WHEN 0 THEN 1 WHEN 1 THEN 0 END , [ModifyUserId] = {0} WHERE ID = {1}", Session["Logistika_ZiCyZ"].ToString(), ii);

					try
					{
						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.Text;
						sqlComm.CommandText = proS;
						sqlComm.Connection = conn;
						conn.Open();
						sqlComm.ExecuteNonQuery();
					}
					catch (Exception)
					{
						mOK = false;
					}
				}
				catch (Exception)
				{
					mOK = false;
				}
				
				if (mOK) 
				{
					this.fillPagerData();
				}	
			}
		}
	}
}