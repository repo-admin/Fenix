using System;
using System.Data;
using System.Web.UI.WebControls;
using UPC.Extensions.Convert;

namespace Fenix
{
	/// <summary>
	/// D0 - zrušené objednávky
	/// objednávka je zrušena když IsActive = false a existuje o ní záznam v tabulce DeleteMessageSent
	/// [rušení probíhá pomocí emailů (z UPC do XPO a nazpět)] 
	/// </summary>
	public partial class MaDeletedOrders : BasePage
	{
		#region Properties

		private enum Show
		{
			None = 0,
			Reception = 1,
			Kitting = 3,
			Shipment = 6,
			Refurbished = 12
		}

		/// <summary>
		/// Číslo sloupce pro DeleteMessageTypeId
		/// </summary>
		private const int COLUMN_DELETE_MESSAGE_TYPE_ID = 9;

		/// <summary>
		/// seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		/// </summary>
		private int[] hideGrdDataColumns = new int[] { COLUMN_DELETE_MESSAGE_TYPE_ID };

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
			ViewState["Filter"] = " 1=1 ";
			if (!string.IsNullOrWhiteSpace(this.tbxIDFlt.Text)) ViewState["Filter"] += " AND [ID] = " + this.tbxIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxMessageIDFlt.Text)) ViewState["Filter"] += " AND [MessageID] = " + this.tbxMessageIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderIDFlt.Text)) ViewState["Filter"] += " AND [DeleteId] = " + this.tbxDeletedOrderIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderMessageIDFlt.Text)) ViewState["Filter"] += " AND [DeleteMessageId] = " + this.tbxDeletedOrderMessageIDFlt.Text.Trim();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwDeletedOrders]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "[ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageStatusId], [DeleteId], [DeleteMessageId], " +
								   "[DeleteMessageTypeId], [DeleteMessageDescription], [Notice], [SentDate], [SentUserId], [ReceivedDate], " +
								   "[ReceivedUserId], [IsActive], [ModifyDate], [ModifyUserId], [DeletedUserLastName], [DeletedUserFirstName]";

			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, hideGrdDataColumns);
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
			GridViewRow selectedRow = grdData.SelectedRow;
			int show = ConvertExtensions.ToInt32(selectedRow.Cells[9].Text.Trim(), -1);

			if (show == (int)Show.Reception)
			{				
				this.showPanel((Show)show);
				this.showReception(selectedRow);
			}

			if (show == (int)Show.Kitting)
			{
				this.showPanel((Show)show);
				this.showKitting(selectedRow);
			}

			if (show == (int)Show.Shipment)
			{
				this.showPanel((Show)show);
				this.showShipment(selectedRow);
			}

			if (show == (int)Show.Refurbished)
			{
				this.showPanel((Show)show);
				this.showRefurbished(selectedRow);
			}
		}

		private void showPanel(Show showPanel)
		{
			switch (showPanel)
			{
				case Show.None:
					this.pnlReception.Visible = false;
					this.pnlKitting.Visible = false;
					this.pnlShipment.Visible = false;
					this.pnlRefurbished.Visible = false;
					break;
				case Show.Reception:
					this.showPanel(Show.None);
					this.pnlReception.Visible = true;					
					break;
				case Show.Kitting:
					this.showPanel(Show.None);
					this.pnlKitting.Visible = true;					
					break;
				case Show.Shipment:
					this.showPanel(Show.None);
					this.pnlShipment.Visible = true;					
					break;
				case Show.Refurbished:
					this.showPanel(Show.None);
					this.pnlRefurbished.Visible = true;					
					break;
				default:
					break;
			}
		}
		
		private void showReception(GridViewRow selectedRow)
		{			
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID],[MessageId],[MessageDateOfShipment],[ItemSupplierDescription] " +
						  " FROM [dbo].[CommunicationMessagesReceptionSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReceptionHeader.DataSource = myDataTable.DefaultView; this.gvReceptionHeader.DataBind();
				this.gvReceptionHeader.SelectedIndex = -1;

				proS = string.Format("SELECT C.*,S.SourceCode  FROM [dbo].[vwCMRSentIt] C " + 
					                 "LEFT OUTER JOIN cdlSources S ON C.SourceId = S.Id " + 
									 "WHERE C.CMSOId={0}", selectedRow.Cells[5].Text.Trim());
				
				myDataTable = BC.GetDataTable(proS);
				this.gvReceptionItems.DataSource = myDataTable.DefaultView; this.gvReceptionItems.DataBind();
				this.gvReceptionItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS =  " + proS);
			}
		}

		private void showKitting(GridViewRow selectedRow)
		{
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CompanyName] " +
						  " FROM [dbo].[vwCMKSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvKittingHeader.DataSource = myDataTable.DefaultView; 
				this.gvKittingHeader.DataBind();
				this.gvKittingHeader.SelectedIndex = -1;

				proS = String.Format("SELECT [ID], [KitId], [KitDescription], [KitQuantity], [KitQuantityDelivered], " +
											 "[MeasuresID], [KitUnitOfMeasure], [KitQualityId], [KitQualityCode], [HeliosOrderID], " +
											 "[CardStockItemsId], KitQuantityInt, KitQuantityDeliveredInt " + 
											 "FROM [dbo].[vwCMKSentItems] " +
											 "WHERE CMSOId = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvKittingItems.DataSource = myDataTable.DefaultView;
				this.gvKittingItems.DataBind();
				this.gvKittingItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS =  " + proS);
			}
		}

		private void showShipment(GridViewRow selectedRow)
		{			
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CustomerName], [CustomerCity] " +
						  " FROM [dbo].[CommunicationMessagesShipmentOrdersSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvShipmentHeader.DataSource = myDataTable.DefaultView;
				this.gvShipmentHeader.DataBind();
				this.gvShipmentHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID], SingleOrMaster, " +
										"ItemVerKit, [ItemOrKitID], [ItemOrKitDescription], [ItemOrKitUnitOfMeasure], " +
										"[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt " +
										"FROM [dbo].[vwShipmentOrderIt] WHERE [CMSOId] = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvShipmentItems.DataSource = myDataTable.DefaultView;
				this.gvShipmentItems.DataBind();
				this.gvShipmentItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS =  " + proS);
			}
		}

		private void showRefurbished(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CompanyName], [CustomerCity] " +
						  " FROM [dbo].[vwCMRF0Sent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedHeader.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedHeader.DataBind();
				this.gvRefurbishedHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered]" +
									 ",[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId]" +
									 ",[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  FROM [dbo].[vwCMRF0SentIt] WHERE [CMSOId] = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedItems.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedItems.DataBind();
				this.gvRefurbishedItems.SelectedIndex = -1;				
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS =  " + proS);
			}
		}	
	}
}