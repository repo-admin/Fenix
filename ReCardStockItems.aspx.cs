using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	/// <summary>
	/// Zásobování - Karty materiálů a zařízení
	/// </summary>
	public partial class ReCardStockItems : BasePage
	{
		private const int HIDE_COLUMN = 6;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);
				BaseHelper.FillDdlGroupGoods(ref this.ddlGroupGoodsFlt);
				BaseHelper.FillDdlItemType(ref this.ddlItemTypeFlt22);
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
			this.grdPager.Visible = true;

			string proW = "IsActive=1";
			if (this.ddlCardQuantityFlt.SelectedValue.ToString() != "-1"){
				if(this.ddlCardQuantityFlt.SelectedValue.ToString() == "0")proW += " AND [ItemOrKitFree]<>0"; else proW += " AND [ItemOrKitFree]=0";
			}
			if (this.ddlKitQualitiesFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemOrKitQuality]=" + this.ddlKitQualitiesFlt.SelectedValue.ToString();
			if (this.ddlItemVerKitFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemVerKit]= CAST(" + this.ddlItemVerKitFlt.SelectedValue.ToString() + " AS BIT)";
			if (this.ddlGroupGoodsFlt.SelectedValue.ToString() != "-1") proW += " AND [GroupGoods]= '" + this.ddlGroupGoodsFlt.SelectedValue.ToString() + "'";
			if (this.ddlItemTypeFlt22.SelectedValue.ToString() != "-1") proW += " AND [ItemType]= '" + this.ddlItemTypeFlt22.SelectedValue.ToString() + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(ItemOrKitID AS VARCHAR(50))," + delka.ToString() + ") = " + this.tbxMaterialCodeFlt.Text.Trim();
			}
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwCardStockItems]";
			pagerData.OrderBy = "ItemVerKit";
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
			fillPagerData(1);
		}

		protected void btnExcel_Click(object sender, ImageClickEventArgs e)
		{

			string proW = " 1=1";
			if (this.ddlCardQuantityFlt.SelectedValue.ToString() != "-1")
			{
				if (this.ddlCardQuantityFlt.SelectedValue.ToString() == "0") proW += " AND [ItemOrKitFree]<>0"; else proW += " AND [ItemOrKitFree]=0";
			}
			if (this.ddlKitQualitiesFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemOrKitQuality]=" + this.ddlKitQualitiesFlt.SelectedValue.ToString();
			if (this.ddlItemVerKitFlt.SelectedValue.ToString() != "-1") proW += " AND [ItemVerKit]= CAST(" + this.ddlItemVerKitFlt.SelectedValue.ToString() + " AS BIT)";
			if (this.ddlGroupGoodsFlt.SelectedValue.ToString() != "-1") proW += " AND [GroupGoods]= '" + this.ddlGroupGoodsFlt.SelectedValue.ToString() + "'";
			if (this.ddlItemTypeFlt22.SelectedValue.ToString() != "-1") proW += " AND [ItemType]= '" + this.ddlItemTypeFlt22.SelectedValue.ToString() + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(ItemOrKitID AS VARCHAR(50))," + delka.ToString() + ") = " + this.tbxMaterialCodeFlt.Text.Trim();

			}
			string proS = string.Format("SELECT [ID],[ItemVerKitDescription],[ItemOrKitID],[DescriptionCz]" +
						 ",[QualitiesCode],[MeasuresCode],[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger],[ItemOrKitReservedInteger]" +
						 ",[ItemOrKitReleasedForExpeditionInteger],[ItemOrKitExpeditedInteger]" +
						 ",[ItemType],[PC],[Packaging],[IsActive],[ItemVerKit]  FROM [dbo].[vwCardStockItems] WHERE {0}", proW);

			ExcelView(proS);
		}
	}
}