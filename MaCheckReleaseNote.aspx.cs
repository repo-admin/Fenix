using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;
using FenixHelper;
using UPC.Extensions.Convert;

namespace Fenix
{
	/// <summary>
	/// [Expedice] Odsouhlasení výdejek
	/// </summary>
	public partial class MaCheckReleaseNote : BasePage
	{		
		/// <summary>
		/// Sloupec pro 'Done' (bude skrytý)
		/// </summary>
		private const int HIDE_COLUMN = 13;

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
			if (!string.IsNullOrWhiteSpace(this.tbxIdWfFlt.Text))
			{
				try
				{
					proW += " AND IdWf = " + Convert.ToInt32(this.tbxIdWfFlt.Text.Trim()).ToString();     // 2015-01-28
				}
				catch (Exception)
				{
					this.tbxIdWfFlt.Text = string.Empty;
				}
				//proW += " AND IdWf = " + this.tbxIdWfFlt.Text.Trim();    // 2015-01-28
			}
			if (!string.IsNullOrWhiteSpace(this.tbxCompanyNameFlt.Text))
			{
				proW += " AND CompanyName LIKE '" + this.tbxCompanyNameFlt.Text.Trim() + "%'";
			}
			if (!string.IsNullOrWhiteSpace(this.tbxCityFlt.Text))
			{
				proW += " AND City LIKE '" + this.tbxCityFlt.Text.Trim() + "%'";
			}
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(MaterialCode AS VARCHAR(50)),"+delka.ToString()+") = " + this.tbxMaterialCodeFlt.Text.Trim();
			}

			if (this.ddlDoneFlt.SelectedValue != "-1") proW += " AND DoneCase='" + this.ddlDoneFlt.SelectedValue.ToString() + "'";
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwVydejkySprWrhMaterials]";
			pagerData.OrderBy = "IdWf";
			pagerData.ColumnList = "*";
			pagerData.WhereClause = proW;
			
			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, HIDE_COLUMN);				
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
								
				this.allowModification();				
				ViewState["table"] = pagerData.DataSet.Tables[0];
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		/// <summary>
		/// Nastaví přístupnost možnosti upravit výdejku
		/// DĚS, BĚS - až bude čas, NUTNO předělat
		/// </summary>
		private void allowModification()
		{
            var test = Session["Logistika_ZiCyZ"].ToString();
			// Bárta, Bejšovec, Černý, Gubrická, Kurinec, Patáčik, Rezler, Stodolová, Tajbl, Weczerek
            if (Session["Logistika_ZiCyZ"].ToString() == "542" || 
				Session["Logistika_ZiCyZ"].ToString() == "14"  || 
				Session["Logistika_ZiCyZ"].ToString() == "20"  ||
                Session["Logistika_ZiCyZ"].ToString() == "809" ||
                Session["Logistika_ZiCyZ"].ToString() == "1084"||
                Session["Logistika_ZiCyZ"].ToString() == "9980" ||
                Session["Logistika_ZiCyZ"].ToString() == "10109" ||
                Session["Logistika_ZiCyZ"].ToString() == "1056" ||
				Session["Logistika_ZiCyZ"].ToString() == "10071" ||
				Session["Logistika_ZiCyZ"].ToString() == "780"
                )
			{	
				foreach (GridViewRow gr in this.grdData.Rows)
				{
					string b = gr.Cells[3].Text.Trim();
					//if (b == "A") gr.Cells[0].Enabled = false; else gr.Cells[0].Enabled = true;
					gr.Cells[0].Enabled = (b != "A" ? true : false);
				}
			}
			else
			{
				foreach (GridViewRow gr in this.grdData.Rows)
				{
					gr.Cells[0].Enabled = false;
				}
			}
		}
		
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.fillPagerData(BC.PAGER_FIRST_PAGE);			
		}

		protected void grdData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			this.grdData.EditIndex = -1;
			this.grdData.DataSource = ((DataTable)ViewState["table"]).DefaultView;
			this.grdData.DataBind();			
		}

		protected void grdData_RowEditing(object sender, GridViewEditEventArgs e)
		{
			GridViewRow row = this.grdData.Rows[e.NewEditIndex];
						
			this.lblErrorUpdate.Text = "";
			Session["IsRefresh"] = "0";

			this.grdData.EditIndex = e.NewEditIndex;
			this.grdData.DataSource = ((DataTable)ViewState["table"]).DefaultView;
			this.grdData.DataBind();

			if (this.rowIsNotInBalance(row))
			{
				this.grdData.EditRowStyle.BackColor = BC.RedColor;
			}
			else
			{
				this.grdData.EditRowStyle.BackColor = Color.Empty;
			}
		}

		protected void grdData_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			bool mOK = true; 
			string err = string.Empty; 
			int AnoNe=0; 
			bool xmOK = true;

			if (Session["IsRefresh"].ToString() == "0" || Session["IsRefresh"] == null)
			{
				//Update the values.
				this.lblErrorUpdate.Text = "";
				GridViewRow row = this.grdData.Rows[e.RowIndex];
				string xID = row.Cells[1].Text.Trim();                                 // ID
				string xDone = ((TextBox)(row.Cells[3].Controls[0])).Text.Trim();      // Done
				string xQuantity = ((TextBox)(row.Cells[6].Controls[0])).Text.Trim();  // SuppliedQuantities
				
				try
				{
					int ii = Convert.ToInt32(xID);
				}
				catch (Exception)
				{
					mOK = false; err += "ID není číslo<br />";
				}
				try
				{
					int ii = Convert.ToInt32(xQuantity);
				}
				catch (Exception)
				{
					mOK = false; err += "Množství není číslo<br />";
				}
				if (!(xDone == "A" || xDone == "N")) 
				{ mOK = false; err += "Souhlasíte? A/N<br />"; } 
				else{
				if (xDone == "A") AnoNe=1; else AnoNe=0;
				}
				if (mOK) { 
				

				} else {
					this.lblErrorUpdate.Text = err;
				}

				string proU = string.Format("UPDATE [dbo].[VydejkySprWrhMaterials] SET  [SuppliedQuantities] = {0},[Done] = {1} WHERE [Id]={2} ", xQuantity, AnoNe, xID);
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
					BC.ProcessException(ex, AppLog.GetMethodName(), "proU = " + proU);
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
					this.grdData.EditIndex = -1;
					Session["IsRefresh"] = 1;
					this.fillPagerData();
				}
			}
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		protected void grdData_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				if (this.rowIsNotInBalance(e.Row))
				{
					e.Row.BackColor = BC.RedColor;
				}
			}
		}
				
		// TODO" MR  popsat co metoda dela
		/// <summary>
		/// Test, zda množství TODO
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		private bool rowIsNotInBalance(GridViewRow gridViewRow)
		{
			if (gridViewRow.Cells[3].Text.ToUpper() == "N")
			{
				int requiredQuantities = ConvertExtensions.ToInt32(gridViewRow.Cells[5].Text, 0);
				int suppliedQuantities = ConvertExtensions.ToInt32(gridViewRow.Cells[6].Text, 0);
				if ((requiredQuantities != suppliedQuantities) && (suppliedQuantities != 0))
				{
					return true;
				}
			}

			return false;
		}
	}
}