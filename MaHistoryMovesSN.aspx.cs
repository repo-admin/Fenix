using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;

namespace Fenix
{
	/// <summary>
	/// [Správa] Historie pohybů SN
	/// </summary>
	public partial class MaHistoryMovesSN : BasePage
	{
		#region Constants
		
		/// <summary>
		/// Minimální délka SN
		/// <value> = 9</value>
		/// </summary>
		private const int MIN_LENGTH_SN = 9;

		#endregion

		#region Properties

		//seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		private int[] hideGrdDataColumns = new int[] { 3 };

		#endregion		

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				BaseHelper.FillDdlDecision(ref this.ddlDecisionFlt, 0);
				this.hideMainData();
				this.hideAllDetails();
				this.debug();
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
				this.grdPager.CurrentIndex = currPageIndx;
				BC.UnbindDataFromObject<GridView>(this.grdData);
				this.fillPagerData(currPageIndx);
			}
		}

		/// <summary>
		/// Naplní hlavni grid daty s pohybem zadaného SN
		/// </summary>
		/// <param name="pageNo"></param>
		private void fillPagerData(int pageNo)
		{			
			this.hideAllDetails();
			BC.UnbindDataFromObject<GridView>(this.grdData);
			this.lblInfoRecordersCount.Visible = true;

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = String.Format("[dbo].[fnHistoryMovesSN] ('{0}')", txtSN.Text.Trim());
			pagerData.OrderBy = "[ReceiptDate] desc";
			pagerData.ColumnList = "*";
			
			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref this.grdData, hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
				this.grdData.Visible = (pagerData.ItemCount > 0 ? true : false);
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
				this.hideMainData();
			}
		}

		/// <summary>
		/// Dohledání pohybů zadaného SN
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.removeLabelsText();
			this.hideMainData();
			this.hideAllDetails();
			
			if (this.serialNumberIsValid())
			{
				this.fillPagerData(BC.PAGER_FIRST_PAGE);
			}
			else
			{
				this.lblErr.Text = String.Format("Nutno zadat SN o délce minimálně {0} znaků.", MIN_LENGTH_SN);
			}
		}

		/// <summary>
		/// Zobrazení dat a detailu dle typu zprávy		
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.hideAllDetails();
			
			string messageType =  this.grdData.SelectedRow.Cells[4].Text.ToUpper().Trim();
			switch (messageType)
			{
				case "K1":
					this.showKittingConfirmationData(this.grdData.SelectedRow);
					break;
				case "S1":
					this.showShipmentConfirmationData(this.grdData.SelectedRow);
					break;
				case "RF1":
					this.showRefurbishedConfirmationData(this.grdData.SelectedRow);
					break;
				case "VR1":									
				case "VR3":
					this.showReturnedData(messageType);					
					break;
				default:
					break;
			}
		}
			
		/// <summary>
		/// Zobrazení dat a detailu pro K1 - Kitting Confirmation
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void showKittingConfirmationData(GridViewRow gridViewRow)
		{
			this.removeLabelsText();
			this.pnlKittingConfirmation.Visible = true;
			this.fillKittingConfirmationGridData(gridViewRow);
			this.fillKittingConfirmationGridDetail(gridViewRow);
		}

		/// <summary>
		/// Zobrazení dat a detailu pro S1 - Shipment Confirmation
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void showShipmentConfirmationData(GridViewRow gridViewRow)
		{
			this.removeLabelsText();
			this.pnlShipmentConfirmation.Visible = true;
			this.fillShipmentConfirmationGridData(gridViewRow);
			this.fillShipmentConfirmationGridDetail(gridViewRow);
		}

		/// <summary>
		/// Zobrazení dat a detailu pro RF1 - Refurbished Confirmation
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void showRefurbishedConfirmationData(GridViewRow gridViewRow)
		{
			this.removeLabelsText();
			this.pnlRefurbishedConfirmation.Visible = true;
			this.fillRefurbishedConfirmationGridData(gridViewRow);
			this.fillRefurbishedConfirmationGridDetail(gridViewRow);
		}

		/// <summary>
		/// "Zpracování detailu" pro VR1 a VR3
		/// (nemají detail, místo něho se použije info text)
		/// </summary>
		/// <param name="messageType"></param>
		private void showReturnedData(string messageType)
		{
			this.removeLabelsText();
			this.lblInfo.Text = String.Format("Pro message typu {0} není definován detail.", messageType);
		}

		/// <summary>
		/// Naplní grid s údaji pro K1
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillKittingConfirmationGridData(GridViewRow gridViewRow)
		{
			BC.UnbindDataFromObject<GridView>(this.grdKittingConfirmationData);

			this.grdKittingConfirmationData.DataSource = HistoryMovesSN.GetKittingConfirmationData(gridViewRow).DefaultView;
			this.grdKittingConfirmationData.DataBind();
			this.grdKittingConfirmationData.SelectedIndex = -1;
		}

		/// <summary>
		/// Naplní grid s údaji pro K1 Detail
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillKittingConfirmationGridDetail(GridViewRow gridViewRow)
		{
			BC.UnbindDataFromObject<GridView>(this.grdKittingConfirmationDetail);

			this.grdKittingConfirmationDetail.DataSource = HistoryMovesSN.GetKittingConfirmationDetail(gridViewRow).DefaultView;
			this.grdKittingConfirmationDetail.DataBind();
			this.grdKittingConfirmationDetail.SelectedIndex = -1;
		}

		/// <summary>
		/// Naplní grid s údaji pro S1
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillShipmentConfirmationGridData(GridViewRow gridViewRow)
		{		
			BC.UnbindDataFromObject<GridView>(this.grdShipmentConfirmationData);

			this.grdShipmentConfirmationData.DataSource = HistoryMovesSN.GetShipmentConfirmationData(gridViewRow).DefaultView;
			this.grdShipmentConfirmationData.DataBind();
			this.grdShipmentConfirmationData.SelectedIndex = -1;
		}

		/// <summary>
		/// Naplní grid s údaji pro S1 Detail 
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillShipmentConfirmationGridDetail(GridViewRow gridViewRow)
		{
			//firma
			DataTable dataTable = HistoryMovesSN.GetShipmentConfirmationCompanyDetail(gridViewRow);
			this.lblScCustomerNameValue.Text = dataTable.Rows[0]["CustomerName"].ToString();
			this.lblScCustomerCityValue.Text = dataTable.Rows[0]["CustomerCity"].ToString();
			this.lblScCustomerZipCodeValue.Text = dataTable.Rows[0]["CustomerZipCode"].ToString();
			this.lblScRequiredDateOfShipmentValue.Text = wConvertStringToDatedd_mm_yyyy(dataTable.Rows[0]["RequiredDateOfShipment"].ToString());
			this.lblScCustomerAddress1Value.Text = dataTable.Rows[0]["CustomerAddress1"].ToString();
			this.lblScCustomerAddress2Value.Text = dataTable.Rows[0]["CustomerAddress2"].ToString();
			
			//detail
			BC.UnbindDataFromObject<GridView>(this.grdShipmentConfirmationDetail);
			this.grdShipmentConfirmationDetail.DataSource = HistoryMovesSN.GetShipmentConfirmationDetail(gridViewRow).DefaultView;
			this.grdShipmentConfirmationDetail.DataBind();
			this.grdShipmentConfirmationDetail.SelectedIndex = -1;			
		}

		/// <summary>
		/// Naplní grid s údaji pro RF1
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillRefurbishedConfirmationGridData(GridViewRow gridViewRow)
		{
			BC.UnbindDataFromObject<GridView>(this.grdRefurbishedConfirmationData);

			this.grdRefurbishedConfirmationData.DataSource = HistoryMovesSN.GetRefurbishedConfirmationData(gridViewRow).DefaultView;
			this.grdRefurbishedConfirmationData.DataBind();
			this.grdRefurbishedConfirmationData.SelectedIndex = -1;
		}

		/// <summary>
		/// Naplní grid s údaji pro RF1 Detail
		/// </summary>
		/// <param name="gridViewRow"></param>
		private void fillRefurbishedConfirmationGridDetail(GridViewRow gridViewRow)
		{
			//firma
			DataTable dataTable = HistoryMovesSN.GetRefurbishedConfirmationCompanyDetail(gridViewRow);
			this.lblRfCustomerNameValue.Text = dataTable.Rows[0]["CustomerName"].ToString();
			this.lblRfCustomerCityValue.Text = dataTable.Rows[0]["CustomerCity"].ToString();
			this.lblRfCustomerZipCodeValue.Text = dataTable.Rows[0]["CustomerZipCode"].ToString();
			this.lblRfRequiredDateOfShipmentValue.Text = wConvertStringToDatedd_mm_yyyy(dataTable.Rows[0]["RequiredDateOfShipment"].ToString());
			this.lblRfCustomerAddress1Value.Text = dataTable.Rows[0]["CustomerAddress1"].ToString();
			this.lblRfCustomerAddress2Value.Text = dataTable.Rows[0]["CustomerAddress2"].ToString();

			//detail
			BC.UnbindDataFromObject<GridView>(this.grdRefurbishedConfirmationDetail);
			this.grdRefurbishedConfirmationDetail.DataSource = HistoryMovesSN.GetRefurbishedConfirmationDetail(gridViewRow).DefaultView;
			this.grdRefurbishedConfirmationDetail.DataBind();
			this.grdRefurbishedConfirmationDetail.SelectedIndex = -1;
		}

		/// <summary>
		/// Odstraní text z err a info labelu
		/// </summary>		
		private void removeLabelsText()
		{
			this.lblErr.Text = String.Empty;
			this.lblInfo.Text = String.Empty;
		}

		/// <summary>
		/// Skryje všechny detaily
		/// </summary>
		private void hideAllDetails()
		{
			this.pnlKittingConfirmation.Visible = false;
			this.pnlShipmentConfirmation.Visible = false;
			this.pnlRefurbishedConfirmation.Visible = false;
		}

		/// <summary>
		/// Skryje grid s historií pohybu SN a objekty pageru
		/// </summary>
		private void hideMainData()
		{
			this.lblInfoRecordersCount.Visible = false;
			this.grdPager.Visible = false;
			this.grdData.Visible = false;
		}

		/// <summary>
		/// Test, zda je zadané sériové číslo validní
		/// (vyplněné a má minimální délku MIN_LENGTH_SN)
		/// </summary>
		/// <returns></returns>
		private bool serialNumberIsValid()
		{
			return !String.IsNullOrWhiteSpace(this.txtSN.Text.Trim()) && this.txtSN.Text.Trim().Length >= MIN_LENGTH_SN;
		}
		
		/// <summary>
		/// Vyplní SN   slouží pro zrychlení vývoje => metodu i její volání LZE po ukončení vývoje zrušit		
		/// </summary>
		private void debug()
		{
			/*  SN
				014161163333 {K1, S1, RF1}
				014162073333 {S1, 2x RF1}
				014166833773 {5x VR1, 1x VR3} 
				%9LCQE037172 {4x VR1} 
				014166304130 {2x VR3}
			 */

			if (BC.IsDebug)
			{
				this.txtSN.Text = "014161163333";				
			}
		}
	}
}