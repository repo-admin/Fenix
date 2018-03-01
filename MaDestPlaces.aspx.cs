using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using FenixHelper;

namespace Fenix
{
	/// <summary>
	/// [Správa] Cílové destinace
	/// </summary>
	public partial class MaDestPlaces : BasePage
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
			string proW = "IsActive=1";
			if (!string.IsNullOrWhiteSpace(this.tbxCompanyNameFlt.Text)) { proW += " AND CompanyName LIKE '" + this.tbxCompanyNameFlt.Text.Trim()+"%'"; }
			if (!string.IsNullOrWhiteSpace(this.tbxStreetFlt.Text)) { proW += " AND StreetName LIKE '" + this.tbxStreetFlt.Text.Trim() + "%'"; }

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[cdlDestinationPlaces]";
			pagerData.OrderBy = "CompanyName";
			pagerData.ColumnList = "*";
			pagerData.WhereClause = proW;

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);				
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		private void fillGvContacts(int grDataId)
		{
			string proS = string.Format("SELECT * FROM cdlDestinationPlacesContacts WHERE DestinationPlacesId = {0}", grDataId);
			DataTable dt = BC.GetDataTable(proS);
			this.gvContacts.DataSource = dt.DefaultView; 
			this.gvContacts.DataBind();
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			Session["IsRefresh"] = "0";
			this.grdData.SelectedIndex = -1;
			this.mvwMain.ActiveViewIndex = 1;
			ViewState["grDataId"] = null;
			ViewState["table"] = null;
			this.gvContactsUpdate.EditIndex = -1;
			this.gvContactsUpdate.DataSource = null; this.gvContactsUpdate.DataBind();

			this.tbxID.Text = string.Empty;
			this.tbxOrganisationNumber.Text = string.Empty;
			this.tbxCompanyName.Text = string.Empty;
			this.tbxCity.Text = string.Empty;
			this.tbxStreetName.Text = string.Empty;
			this.tbxStreetOrientationNumber.Text = string.Empty;
			this.tbxStreetHouseNumber.Text = string.Empty;
			this.tbxZipCode.Text = string.Empty;
			this.tbxIdCountry.Text = string.Empty;
			this.tbxICO.Text = string.Empty;
			this.tbxDIC.Text = string.Empty;
			this.tbxType.Text = string.Empty;
			this.tbxCountryISO.Text = string.Empty;
			this.chkbIsActive.Checked = true;

			this.tbxJmenoN.Enabled = false;
			this.tbxPrijmeniN.Enabled = false;
			this.tbxMestoN.Enabled = false;
			this.tbxUliceN.Enabled = false;
			this.tbxPSCN.Enabled = false;
			this.tbxCisPopis.Enabled = false;
			this.tbxCisOr.Enabled = false;
			this.tbxMailN.Enabled = false;
			this.tbxTelefonN.Enabled = false;
			this.tbxTypN.Enabled = false;

		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "RecordUpdate")
			{
				Session["IsRefresh"] = "0";
				int grDataId = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["grDataId"] = grDataId;				
				this.grdData.SelectedIndex = -1;
				this.mvwMain.ActiveViewIndex = 1;

				string proS = string.Format("SELECT [ID],[OrganisationNumber] ,[CompanyName],[City],[StreetName]" +
					",[StreetOrientationNumber],[StreetHouseNumber],[ZipCode],[IdCountry],[ICO],[DIC],[Type],[CountryISO], IsActive " +
					" FROM cdlDestinationPlaces WHERE Id = {0}", grDataId);
				DataTable dt = BC.GetDataTable(proS);
				this.tbxID.Text = dt.Rows[0][0].ToString();
				this.tbxOrganisationNumber.Text = dt.Rows[0][1].ToString();
				this.tbxCompanyName.Text = dt.Rows[0][2].ToString();
				this.tbxCity.Text = dt.Rows[0][3].ToString();
				this.tbxStreetName.Text = dt.Rows[0][4].ToString();
				this.tbxStreetOrientationNumber.Text = dt.Rows[0][5].ToString();
				this.tbxStreetHouseNumber.Text = dt.Rows[0][6].ToString();
				this.tbxZipCode.Text = dt.Rows[0][7].ToString();
				this.tbxIdCountry.Text = dt.Rows[0][8].ToString();
				this.tbxICO.Text = dt.Rows[0][9].ToString();
				this.tbxDIC.Text = dt.Rows[0][10].ToString();
				this.tbxType.Text = dt.Rows[0][11].ToString();
				this.tbxCountryISO.Text = dt.Rows[0][12].ToString();
				this.chkbIsActive.Checked = Convert.ToBoolean(dt.Rows[0][13].ToString());

				this.tbxJmenoN.Enabled = true;
				this.tbxPrijmeniN.Enabled = true;
				this.tbxMailN.Enabled = true;
				this.tbxTelefonN.Enabled = true;
				this.tbxTypN.Enabled = true;


				proS = string.Format("SELECT * FROM cdlDestinationPlacesContacts WHERE DestinationPlacesId = {0}", grDataId);
				dt = BC.GetDataTable(proS);
				ViewState["table"] = dt;
				this.gvContactsUpdate.DataSource = dt.DefaultView; this.gvContactsUpdate.DataBind();
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblErr.Text = string.Empty;
			Session["IsRefresh"] = 0;
			this.tbxJmenoN.Text = "";
			this.tbxPrijmeniN.Text = "";
			this.tbxMestoN.Text = "";
			this.tbxUliceN.Text = "";
			this.tbxPSCN.Text = "";
			this.tbxCisPopis.Text = "";
			this.tbxCisOr.Text = "";
			this.tbxMailN.Text = "";
			this.tbxTelefonN.Text = "";
			this.tbxTypN.Text = "";
			this.tbxJmenoN.Enabled = true;
			this.tbxPrijmeniN.Enabled = true;
			this.tbxMailN.Enabled = true;
			this.tbxTelefonN.Enabled = true;
			this.tbxTypN.Enabled = true;
			
			ViewState["grDataId"] = this.grdData.SelectedValue.ToString();
			fillGvContacts(WConvertStringToInt32(this.grdData.SelectedValue.ToString()));
		}

		protected void gvContacts_RowCommand(object sender, GridViewCommandEventArgs e)
		{

		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0")) { }
			this.lblErr.Text = "";
			bool mOK = true;
			StringBuilder sb = new StringBuilder();
			if (string.IsNullOrEmpty(this.tbxCompanyName.Text.Trim())) { sb.Append("Název je povinný údaj<br />"); mOK = false; }
			if (string.IsNullOrEmpty(this.tbxStreetName.Text.Trim())) { sb.Append("Ulice je povinný údaj<br />"); mOK = false; }
			if (string.IsNullOrEmpty(this.tbxStreetHouseNumber.Text.Trim())) { sb.Append("Č. popisné je povinný údaj<br />"); mOK = false; }
			if (string.IsNullOrEmpty(this.tbxZipCode.Text.Trim())) { sb.Append("PSČ je povinný údaj<br />"); mOK = false; }
			if (string.IsNullOrEmpty(this.tbxCountryISO.Text.Trim())) { sb.Append("ISO je povinný údaj<br />"); mOK = false; }
			this.lblErr.Text = sb.ToString();
			sb = null;
			if (mOK)
			{
				string ReturnValue = string.Empty;
				string ReturnMessage = string.Empty;

				SqlConnection conn = new SqlConnection();
				conn.ConnectionString = BC.FENIXWrtConnectionString;
				conn.Open();
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "prDestinationPlaces";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@ID", SqlDbType.Int).Value = WConvertStringToInt32(this.tbxID.Text);
				sqlComm.Parameters.Add("@OrganisationNumber", SqlDbType.Int).Value = WConvertStringToInt32(this.tbxOrganisationNumber.Text.Trim()); //
				sqlComm.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 100).Value = this.tbxCompanyName.Text.Trim();
				sqlComm.Parameters.Add("@City", SqlDbType.NVarChar, 150).Value = this.tbxCity.Text.Trim();
				sqlComm.Parameters.Add("@StreetName", SqlDbType.NVarChar, 100).Value = this.tbxStreetName.Text.Trim();
				sqlComm.Parameters.Add("@StreetOrientationNumber", SqlDbType.NVarChar, 15).Value = this.tbxStreetOrientationNumber.Text.Trim();
				sqlComm.Parameters.Add("@StreetHouseNumber", SqlDbType.NVarChar, 35).Value = this.tbxStreetHouseNumber.Text.Trim();
				sqlComm.Parameters.Add("@ZipCode", SqlDbType.NVarChar, 10).Value = this.tbxZipCode.Text.Trim();
				sqlComm.Parameters.Add("@IdCountry", SqlDbType.NVarChar, 3).Value = this.tbxIdCountry.Text.Trim();
				sqlComm.Parameters.Add("@ICO", SqlDbType.NVarChar, 20).Value = this.tbxICO.Text.Trim();
				sqlComm.Parameters.Add("@DIC", SqlDbType.NVarChar, 15).Value = this.tbxDIC.Text.Trim();
				sqlComm.Parameters.Add("@Type", SqlDbType.NVarChar, 10).Value = this.tbxType.Text.Trim();
				sqlComm.Parameters.Add("@CountryISO", SqlDbType.NVarChar, 3).Value = this.tbxCountryISO.Text.Trim();
				sqlComm.Parameters.Add("@chkbIsActive", SqlDbType.Bit).Value = Convert.ToBoolean(this.chkbIsActive.Checked);
				sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString());
				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					sqlComm.ExecuteNonQuery();

					ReturnValue = sqlComm.Parameters["@ReturnValue"].Value.ToString();
					ReturnMessage = sqlComm.Parameters["@ReturnMessage"].Value.ToString();
					if (ReturnValue != "0")
					{
						mOK = false;
						this.lblErr.Text = ReturnMessage;
					}

				}
				catch (Exception ex)
				{					
					BC.ProcessException(ex, AppLog.GetMethodName());
					mOK = false;
				}
				finally
				{
					if (conn.State == ConnectionState.Open) conn.Close();
					conn.Dispose();
					sqlComm.Dispose();
				}
				if (mOK) btnBack_Click(btnSave, EventArgs.Empty);
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.lblErrorUpdate.Text = "";
			this.gvContactsUpdate.EditIndex = -1;
			this.grdData.SelectedIndex = -1;
			this.mvwMain.ActiveViewIndex = 0;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void gvContactsUpdate_RowEditing(object sender, GridViewEditEventArgs e)
		{
			this.lblErrorUpdate.Text = "";
			Session["IsRefresh"] = 0;
			this.gvContactsUpdate.EditIndex = e.NewEditIndex;
			this.gvContactsUpdate.DataSource = ((DataTable)ViewState["table"]).DefaultView;
			this.gvContactsUpdate.DataBind();
		}

		protected void gvContactsUpdate_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			this.gvContactsUpdate.EditIndex = -1;
			this.gvContactsUpdate.DataSource = ((DataTable)ViewState["table"]).DefaultView;
			this.gvContactsUpdate.DataBind();
		}

		protected void gvContactsUpdate_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			if (Session["IsRefresh"].ToString() == "0" || Session["IsRefresh"] == null)
			{ 
			DataTable dt = ((DataTable)ViewState["table"]);
			//Update the values.
			GridViewRow row = gvContactsUpdate.Rows[e.RowIndex];

			string xID = row.Cells[1].Text.Trim();
			string xFirstName = ((TextBox)(row.Cells[2].Controls[0])).Text.Trim();
			string xLastName = ((TextBox)(row.Cells[3].Controls[0])).Text.Trim();
			string xPhoneNumber = ((TextBox)(row.Cells[4].Controls[0])).Text.Trim();
			string xContactEmail = ((TextBox)(row.Cells[5].Controls[0])).Text.Trim();
			string xType = ((TextBox)(row.Cells[6].Controls[0])).Text.Trim();
			string xIsActive = ((TextBox)(row.Cells[7].Controls[0])).Text.Trim();
			string xContactName = xFirstName + " " + xLastName;

			int xIsActiveIn = 1;
			if (xIsActive.ToUpper() == "FALSE") xIsActiveIn = 0;

			bool xmOK = true;
			if (string.IsNullOrWhiteSpace(xFirstName)
				|| string.IsNullOrWhiteSpace(xLastName)
				|| string.IsNullOrWhiteSpace(xPhoneNumber)
				|| string.IsNullOrWhiteSpace(xContactEmail)
				|| string.IsNullOrWhiteSpace(xType)
				|| string.IsNullOrWhiteSpace(xIsActive))
			{
				// něco schází
				this.lblErrorUpdate.Text = "Zkontrolujte správnost a úplnost údajů";
				xmOK = false;
			}
			else
			{
				// nic neschází

				string proU = string.Format("UPDATE [dbo].[cdlDestinationPlacesContacts] SET [PhoneNumber] = '{0}',[FirstName]='{1}',[LastName]='{2}', " +
									" [ContactName]='{3}',[ContactEmail] = '{4}',[Type] = '{5}', IsActive = {6} WHERE id = {7} ", xPhoneNumber, xFirstName, xLastName, xContactName, xContactEmail, xType, xIsActiveIn, xID);

				SqlConnection con = new SqlConnection();
				con.ConnectionString = BC.FENIXWrtConnectionString;
				SqlCommand com = new SqlCommand();
				com.Connection = con;
				com.CommandType = CommandType.Text;
				com.CommandText = proU;
				try
				{
					con.Open();
					com.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, AppLog.GetMethodName());
					xmOK = false;
				}
				finally
				{
					if (con.State == ConnectionState.Open) con.Close();
					con.Dispose();
					com.Dispose();
				}
			}

			if (xmOK)
			{
				this.gvContactsUpdate.EditIndex = -1;
				Session["IsRefresh"] = 1;
				string proS = string.Format("SELECT * FROM cdlDestinationPlacesContacts WHERE DestinationPlacesId = {0}", WConvertStringToInt32(ViewState["grDataId"].ToString()));
				dt = BC.GetDataTable(proS);
				ViewState["table"] = dt;
				this.gvContactsUpdate.DataSource = dt.DefaultView; this.gvContactsUpdate.DataBind();

			}
		}
		}

		protected void btnSaveContact_Click(object sender, EventArgs e)
		{
			if (Session["IsRefresh"].ToString() == "0" || Session["IsRefresh"] == null)
			{
				bool xmOK = true;
				if (
				string.IsNullOrWhiteSpace(this.tbxJmenoN.Text)
				|| string.IsNullOrWhiteSpace(this.tbxPrijmeniN.Text)
				|| string.IsNullOrWhiteSpace(this.tbxMailN.Text)
				|| string.IsNullOrWhiteSpace(this.tbxTelefonN.Text)
				|| string.IsNullOrWhiteSpace(this.tbxTypN.Text)
				//|| string.IsNullOrWhiteSpace(this.tbxMestoN.Text)
				//|| string.IsNullOrWhiteSpace(this.tbxUliceN.Text)
				//|| string.IsNullOrWhiteSpace(this.tbxPSCN.Text)
				//|| string.IsNullOrWhiteSpace(this.tbxCisPopis.Text)
				//|| string.IsNullOrWhiteSpace(this.tbxCisOr.Text)

				)
				{
					this.lblErrorUpdate.Text = "U nového kontaktu jsou všechny údaje povinné";
				}
				else
				{
					string ContactName = this.tbxJmenoN.Text.Trim() + " " + this.tbxPrijmeniN.Text.Trim();
					string proI = string.Format(" INSERT INTO [dbo].[cdlDestinationPlacesContacts] ([DestinationPlacesId] ,[PhoneNumber] ,[FirstName]" +
							   ",[LastName] ,[Title] ,[ContactName] ,[ContactEmail] ,[Type] ,[IsSent] ,[SentDate] ,[IsActive] ,[ModifyDate] ,[ModifyUserId])" +
							   "     VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11},{12})", ViewState["grDataId"].ToString(),
							   this.tbxTelefonN.Text.Trim(), this.tbxJmenoN.Text.Trim(), this.tbxPrijmeniN.Text.Trim(), this.ddlPohlaví.SelectedValue.ToString(), ContactName, this.tbxMailN.Text.Trim(),
							   this.tbxTypN.Text.Trim(), 0, null, 1, "GetDate()", Session["Logistika_ZiCyZ"].ToString());

					SqlConnection con = new SqlConnection();
					con.ConnectionString = BC.FENIXWrtConnectionString;
					SqlCommand com = new SqlCommand();
					com.Connection = con;
					com.CommandType = CommandType.Text;
					com.CommandText = proI;
					try
					{
						con.Open();
						com.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						BC.ProcessException(ex, AppLog.GetMethodName());
						xmOK = false;
					}
					finally
					{
						if (con.State == ConnectionState.Open) con.Close();
						con.Dispose();
						com.Dispose();
					}
					if (xmOK)
					{
						this.gvContactsUpdate.EditIndex = -1;
						Session["IsRefresh"] = 1;
						this.lblErr.Text = string.Empty;
						Session["IsRefresh"] = 0;
						this.tbxJmenoN.Text = "";
						this.tbxPrijmeniN.Text = "";
						this.tbxMestoN.Text = "";
						this.tbxUliceN.Text = "";
						this.tbxPSCN.Text = "";
						this.tbxCisPopis.Text = "";
						this.tbxCisOr.Text = "";
						this.tbxMailN.Text = "";
						this.tbxTelefonN.Text = "";
						this.tbxTypN.Text = "";

						DataTable dt = new DataTable();
						string proS = string.Format("SELECT * FROM cdlDestinationPlacesContacts WHERE DestinationPlacesId = {0}", WConvertStringToInt32(ViewState["grDataId"].ToString()));
						dt = BC.GetDataTable(proS);
						ViewState["table"] = dt;
						this.gvContactsUpdate.DataSource = dt.DefaultView; this.gvContactsUpdate.DataBind();

					}
				}
			}
		}
	}
}