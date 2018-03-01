using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using FenixHelper;

namespace Fenix
{
	public partial class MaInternalDocuments : BasePage
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
			if (!string.IsNullOrWhiteSpace(this.tbxIDFlt.Text)) ViewState["Filter"] += " AND [ID] = " + this.tbxIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxItemOrKitIDFlt.Text)) ViewState["Filter"] += " AND [ItemOrKitID] = " + this.tbxItemOrKitIDFlt.Text.Trim();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwInternalDocuments]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "[ID], [ItemVerKitText], [ItemOrKitID], [MeasureCode], [QualityCode], [ItemOrKitQuantityBefore], [ItemOrKitQuantityAfter] " +
								   ",[ItemOrKitFreeBefore], [ItemOrKitFreeAfter], [ItemOrKitUnConsilliationBefore], [ItemOrKitUnConsilliationAfter] " +
								   ",[ItemOrKitReservedBefore], [ItemOrKitReservedAfter], [ItemOrKitReleasedForExpeditionBefore], [ItemOrKitReleasedForExpeditionAfter] " +
								   ",[ItemOrKitExpeditedBefore], [ItemOrKitExpeditedAfter], [StockName] " +								   
								   ",[InternalDocumentsSourceId], [IsActive], [ModifyDate], [ModifyUserId] ";

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
				BC.ProcessException(ex, AppLog.GetMethodName());
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		//protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		//{
		//}

		//protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//}
	}
}