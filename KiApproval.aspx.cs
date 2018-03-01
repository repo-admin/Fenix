using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	public partial class KiApproval : BasePage
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
				BaseHelper.FillDdlMessageStatuses(ref this.ddlMessageStatusFlt);
				this.fillData(BC.PAGER_FIRST_PAGE);
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
				this.fillData(currPageIndx);
			}
		}

		private void fillData()
		{
			this.fillData(this.grdPager.CurrentIndex);
		}

		private void fillData(int pageNo)
		{
			this.pnlEdit.Visible = false;
			this.grdPager.Visible = true;

			SqlConnection conn = new SqlConnection(BC.FENIXRdrConnectionString);
			DataSet ds = new DataSet();

			SqlCommand sqlComm = new SqlCommand("dbo.sp_SelectPage", conn);
			sqlComm.CommandType = CommandType.StoredProcedure;

			sqlComm.Parameters.Add("@PageNum", SqlDbType.Int).Value = pageNo;
			sqlComm.Parameters.Add("@PageSize", SqlDbType.Int).Value = this.grdPager.PageSize;
			sqlComm.Parameters.Add("@ItemCount", SqlDbType.BigInt).Direction = ParameterDirection.Output;
			sqlComm.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = "[dbo].[CommunicationMessagesKittingsApprovalSent]";
			sqlComm.Parameters.Add("@OrderBy", SqlDbType.VarChar).Value = "ID DESC";
			//sqlComm.Parameters.Add("@ColumnList", SqlDbType.VarChar).Value = "CASE ISNULL(CONVERT(CHAR(10),RequiredReleaseDate,104),'') WHEN '' THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END 'AnoNe',*";
			sqlComm.Parameters.Add("@ColumnList", SqlDbType.VarChar).Value = "CAST(1 AS BIT) 'AnoNe',*";
			sqlComm.Parameters.Add("@WhereClause", SqlDbType.VarChar).Value = "IsActive=1 AND (Released = 0 OR Released IS NULL)";
			sqlComm.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;

			try
			{
				SqlDataAdapter da = new SqlDataAdapter(sqlComm);
				da.Fill(ds);

				this.grdData.DataSource = ds.Tables[0].DefaultView; this.grdData.DataBind();

				this.grdPager.ItemCount = Convert.ToDouble(sqlComm.Parameters["@ItemCount"].Value);
				this.grdPager.CurrentIndex = pageNo;

				this.lblInfoRecordersCount.Text = "Počet záznamů: " + sqlComm.Parameters["@ItemCount"].Value.ToString();

				if (sqlComm.Parameters["@ItemCount"].Value.ToString() == "0") { this.grdPager.Visible = false; } else { this.grdPager.Visible = true; }

			}
			catch (Exception)
			{
				//PosliMail(ex.Message);
			}
			finally { conn.Close(); }

		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillData(BC.PAGER_FIRST_PAGE);
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlEdit.Visible = true;
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();

			string proS = "SELECT [ID]   ,[ApprovalID]   ,[KitID]   ,[KitDescription]   ,[KitQuantity]   ,[KitUnitOfMeasureID]   ,[KitUnitOfMeasure]   ,[KitQualityId]  ,[KitQuality]" +
				" FROM [dbo].[CommunicationMessagesKittingsApprovalKitsSent] WHERE [ApprovalID] = " + grdMainIdKey;
			DataTable myT = new DataTable("myDt");
			myT = BC.GetDataTable(proS);
			this.gvOrders.DataSource = myT.DefaultView; this.gvOrders.DataBind();

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
			myDataColumn.ColumnName = "ApprovalId";                         // ID tabulky   CommunicationMessagesKittingsApprovalSent
			myDt.Columns.Add(myDataColumn);

		}

		private void AddRec(string par1)
		{
			DataTable myT = new DataTable("myDt");
			ZalozTabulku(ref myT);

			if (this.grdData != null && grdData.Rows.Count > 0)
			{
				foreach (GridViewRow gvr in grdData.Rows)
				{
					DataRow Doly = myT.NewRow();
					CheckBox myChkb;
					myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					Doly[0] = (myChkb.Checked) ? 1 : 0;	                         // AnoNe
					Doly[1] = WConvertStringToInt32(gvr.Cells[1].Text);          // ApprovalId
					myT.Rows.Add(Doly);
				}
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			bool mOK = true;

			CultureInfo culture = new CultureInfo("cs-CZ");
			StringBuilder sb = new StringBuilder();
			if (this.grdData != null && grdData.Rows.Count > 0)
			{
				sb.Append("<NewDataSet>");
				foreach (GridViewRow gvr in grdData.Rows)
				{
					sb.Append("<Approval>");
					CheckBox myChkb;
					myChkb = (CheckBox)gvr.FindControl("CheckBoxR");
					if (myChkb.Checked) sb.Append("<ApprovalId>" + gvr.Cells[2].Text + "</ApprovalId>");
					sb.Append("</Approval>");
				}
				sb.Append("</NewDataSet>");
			}

			if (sb.ToString() != "")
			{
				string help = sb.ToString().Replace("{", "").Replace("}", "");
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(help);

				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prCMSAins]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;

				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 300);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					conn.Open();
					sqlComm.ExecuteNonQuery();
					if(sqlComm.Parameters["@ReturnValue"].Value.ToString()!="0"){
						mOK = false;}
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
			sb = null;
			if (mOK)
			{
				this.mvwMain.ActiveViewIndex = 0;
				this.fillData(BC.PAGER_FIRST_PAGE);
				Session["IsRefresh"] = "1";
			}
		}
	}
}