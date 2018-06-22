using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using UPC.Extensions.Enum;

namespace Fenix
{
	/// <summary>
	/// [Správa] Stav skladu
	/// </summary>
	public partial class MaCardStockItems : BasePage
	{
		private enum QuantityType
		{
			/// <summary>
			/// Množství volné 
			/// </summary>
			Free = 0,
						
			/// <summary>
			/// Množství ke schválení 
			/// </summary>
			UnConsiliation = 1,

			/// <summary>
			/// Množství rezervované
			/// </summary>
			Reserved = 2,

			/// <summary>
			/// Množství uvolněné k expedici
			/// </summary>
			ReleasedForExpedition = 3,

			/// <summary>    // 2015-02-27
			/// Množství záporné kdekoliv
			/// </summary>
			NegativNumberEnywhere = 4
		}

		private const int HIDE_COLUMN = 7;

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
				this.fillData(BC.PAGER_FIRST_PAGE);
				this.enableButtonSave();
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

		private void fillData(int pageNo)
		{
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwCardStockItems]";
			pagerData.OrderBy = this.ddlTrideniFlt.SelectedValue;  // 2015-03-05
			pagerData.ColumnList = "*";
			pagerData.WhereClause = this.createWhereClause();

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

		private string createWhereClause()
		{
			string proW = " IsActive = 1 ";

			if (this.ddlKitQualitiesFlt.SelectedValue != "-1") proW += " AND [ItemOrKitQuality]=" + this.ddlKitQualitiesFlt.SelectedValue;
			if (this.ddlItemVerKitFlt.SelectedValue != "-1") proW += " AND [ItemVerKit]= CAST(" + this.ddlItemVerKitFlt.SelectedValue + " AS BIT)";
			if (this.ddlGroupGoodsFlt.SelectedValue != "-1") proW += " AND [GroupGoods]= '" + this.ddlGroupGoodsFlt.SelectedValue + "'";
			if (this.ddlItemTypeFlt22.SelectedValue != "-1") proW += " AND [ItemType]= '" + this.ddlItemTypeFlt22.SelectedValue + "'";
			if (!string.IsNullOrWhiteSpace(this.tbxMaterialCodeFlt.Text))
			{
				int delka = this.tbxMaterialCodeFlt.Text.Trim().Length;
				proW += " AND LEFT(CAST(ItemOrKitID AS VARCHAR(50))," + delka + ") = " + this.tbxMaterialCodeFlt.Text.Trim();
			}

			// Množství 
			if (this.ddlQuantityTypeFlt.SelectedValue != "-1")
			{
				int selectedQuantityType = Convert.ToInt32(this.ddlQuantityTypeFlt.SelectedValue);
				
				if (selectedQuantityType == QuantityType.Free.ToInt())
				{
					proW += " AND [ItemOrKitFree] > 0";
				}

				if (selectedQuantityType == QuantityType.UnConsiliation.ToInt())
				{
					proW += " AND [ItemOrKitUnConsilliation] > 0";
				}

				if (selectedQuantityType == QuantityType.Reserved.ToInt())
				{
					proW += " AND [ItemOrKitReserved] > 0";
				}

				if (selectedQuantityType == QuantityType.ReleasedForExpedition.ToInt())
				{
					proW += " AND [ItemOrKitReleasedForExpedition] > 0";
				}

				if (selectedQuantityType == QuantityType.NegativNumberEnywhere.ToInt()) //2015-02-27
				{
					proW += " AND ( [ItemOrKitFree] < 0 OR [ItemOrKitUnConsilliation] < 0  OR [ItemOrKitReserved] < 0  OR [ItemOrKitReleasedForExpedition] < 0  )";
				}
			}

			return proW;
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			fillData(1);
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			ClearViewControls(vwEdit);
			this.mvwMain.ActiveViewIndex = 0;
			this.grdData.SelectedIndex = -1;

			fillData(1);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (Session["IsRefresh"].ToString() == "0")
				{
					CardStockDetail1.Err = "";
					CardStockDetail1.Logistika_ZiCyZ = WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString());
					CardStockDetail1.Source = InternalDocumentsSource.FenixCardStock;
					CardStockDetail1.SetCardStockRecord();
					if (CardStockDetail1.MOK) btnBack_Click(btnBack, EventArgs.Empty); else this.lblErr.Text = CardStockDetail1.Err;
				}

			}
			catch (Exception ex)
			{
				string err = "CardStockDetail1 y: " + WConvertStringToInt32(Session["Logistika_ZiCyZ"] + ", this.grdPager.PageSize: " + this.grdPager.PageSize);
				BC.SendMail("Fenix Stránka: " + Request.ServerVariables["SCRIPT_NAME"], "<br />; Uživatel: " +
					Request.ServerVariables["LOGON_USER"] + "<br />" + ex.Message + "<br />" + err +
					"<br />", true, "max.weczerek@upc.cz", "", "");
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), err);
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
				try
				{
					CardStockDetail1.Role = 1;
					CardStockDetail1.XID = WConvertStringToInt32(ViewState["ID"].ToString());
					CardStockDetail1.GetCardStockRecord();

				}
				catch (Exception ex)
				{
					string err = "CardStockDetail1 x: " + WConvertStringToInt32(ViewState["ID"].ToString()) + ", this.grdPager.PageSize: " + this.grdPager.PageSize;
					BC.SendMail("Fenix Stránka: " + Request.ServerVariables["SCRIPT_NAME"], "<br />; Uživatel: " +
						Request.ServerVariables["LOGON_USER"] + "<br />" + ex.Message + "<br />" + err +
						"<br />", true, "max.weczerek@upc.cz", "", "");
					BC.ProcessException(ex, ApplicationLog.GetMethodName(), err);
				}
			}
		}

		protected void btnExcel_Click(object sender, ImageClickEventArgs e)
		{
			string proW = this.createWhereClause();	//" 1=1 "  ???
			string proS = string.Format("SELECT [ID],[ItemVerKitDescription],[ItemOrKitID],[DescriptionCz]"+
						 ",[QualitiesCode],[MeasuresCode],[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger],[ItemOrKitReservedInteger]" +
						 ",[ItemOrKitReleasedForExpeditionInteger],[ItemOrKitExpeditedInteger]"+
						 ",[ItemType],[PC],[Packaging],[IsActive],[ItemVerKit]  FROM [dbo].[vwCardStockItems] WHERE {0}", proW);
			ExcelView(proS);
		}  
		
		/// <summary>Nastavení přístupnosti tlačítka save</summary>
		private void enableButtonSave()
		{
			if (Session["Logistika_ZiCyZ"].ToString() != BC.WECZEREK && 
				Session["Logistika_ZiCyZ"].ToString() != BC.MANAGER_LOGISTIKA &&
				Session["Logistika_ZiCyZ"].ToString() != BC.REZLER)
			{
				this.btnSave.Enabled = false;
			}
			else
			{
				this.btnSave.Enabled = true;
			}
		}
	}
}