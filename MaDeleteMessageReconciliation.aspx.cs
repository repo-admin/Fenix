using System;
using System.Web.UI.WebControls;

namespace Fenix
{
	/// <summary>
	/// D1 - zrušené objednávky
	/// [rušení probíhá pomocí XML zpráv] 
	/// </summary>
	public partial class MaDeleteMessageReconciliation : BasePage
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

		private void fillPagerData(int pageNo)
		{
			ViewState["Filter"] = " IsActive = 1 ";
			//if (!string.IsNullOrWhiteSpace(this.tbxIDFlt.Text)) ViewState["Filter"] += " AND [ID] = " + this.tbxIDFlt.Text.Trim();						//30.12.2015
			if (!string.IsNullOrWhiteSpace(this.tbxMessageIDFlt.Text)) ViewState["Filter"] += " AND [MessageID] = " + this.tbxMessageIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderIDFlt.Text)) ViewState["Filter"] += " AND [DeleteId] = " + this.tbxDeletedOrderIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderMessageIDFlt.Text)) ViewState["Filter"] += " AND [DeleteMessageId] = " + this.tbxDeletedOrderMessageIDFlt.Text.Trim();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwDeleteMessageConfirmation]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "[ID], [MessageId], [DeleteMessageDate], [DeleteId], [DeleteMessageId] " +								   
								   ",[DeleteMessageTypeDescription], [Source]";

			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
		}
	}
}