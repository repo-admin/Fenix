using System;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	/// <summary>
	/// Uvolnění kitu
	/// </summary>
	public partial class KiRelease : BasePage
	{
		private const int HIDE_COLUMN = 7;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.ddlItemVerKitFlt.Enabled = false;
				this.mvwMain.ActiveViewIndex = 0;
				BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);
				BaseHelper.FillDdlGroupGoods(ref this.ddlGroupGoodsFlt);
				BaseHelper.FillDdlItemType(ref this.ddlItemTypeFlt22);
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
			if (this.ddlKitQualitiesFlt.SelectedValue != "-1") proW += " AND [ItemOrKitQuality]=" + this.ddlKitQualitiesFlt.SelectedValue;
			if (this.ddlItemVerKitFlt.SelectedValue != "-1") proW += " AND [ItemVerKit]= CAST(" + this.ddlItemVerKitFlt.SelectedValue + " AS BIT)";
			if (this.ddlGroupGoodsFlt.SelectedValue != "-1") proW += " AND [GroupGoods]= '" + this.ddlGroupGoodsFlt.SelectedValue + "'";
			if (this.ddlItemTypeFlt22.SelectedValue != "-1") proW += " AND [ItemType]= '" + this.ddlItemTypeFlt22.SelectedValue + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(ItemOrKitID AS VARCHAR(50))," + delka + ") = " + this.tbxMaterialCodeFlt.Text.Trim();
			}

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwCardStockItems]";
			pagerData.OrderBy = this.ddlTrideniFlt.SelectedValue;
			pagerData.ColumnList = "*";
			pagerData.WhereClause = proW;
			
			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, HIDE_COLUMN);
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

		protected void btnBack_Click(object sender, EventArgs e)
		{
			ClearViewControls(vwEdit);
			this.mvwMain.ActiveViewIndex = 0;
			this.grdData.SelectedIndex = -1;

			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (Session["IsRefresh"].ToString() == "0")
			{
				CardStockDetail1.Err = "";
				CardStockDetail1.Logistika_ZiCyZ = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString());
				CardStockDetail1.Source = InternalDocumentsSource.FenixReleaseKit;
				CardStockDetail1.SetCardStockRecord();
				if (CardStockDetail1.MOK) btnBack_Click(btnBack, EventArgs.Empty); else this.lblErr.Text = CardStockDetail1.Err;
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			this.lblErr.Text = "";
			if (e.CommandName == "btnEdit")
			{
				Session["IsRefresh"] = "0";
				int grDataId = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["ID"] = grDataId;
				//int grDataId = WConvertStringToInt32(e.CommandArgument.ToString());
				this.grdData.SelectedIndex = -1;
				this.mvwMain.ActiveViewIndex = 1;
				//********************************
				CardStockDetail1.Role = 2;
				CardStockDetail1.XID = WConvertStringToInt32(ViewState["ID"].ToString());
				CardStockDetail1.GetCardStockRecord();
			}
		}
	}
}