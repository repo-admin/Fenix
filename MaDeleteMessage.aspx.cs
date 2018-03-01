using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using FenixHelper;
using UPC.Extensions.Convert;

namespace Fenix
{
	/// <summary>
	/// D0 - zrušené objednávky
	/// [rušení probíhá pomocí XML zpráv] 
	/// </summary>
	public partial class MaDeleteMessage : BasePage
	{
		#region Properties

		private enum ShowPanel
		{
			None = 0,
			ReceptionOrder = 1,
			ReceptionConfirmation = 2,
			KittingOrder = 3,
			KittingConfirmation = 4,
			ShipmentOrder = 6,
			ShipmentConfirmation = 7,
			ReturnedEquipment = 9,
			ReturnedItem = 10,
			ReturnedShipment = 11,
			RefurbishedOrder = 12,
			RefurbishedConfirmation = 13
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
			ViewState["Filter"] = " IsActive = 1 ";
			//if (!string.IsNullOrWhiteSpace(this.tbxIDFlt.Text)) ViewState["Filter"] += " AND [ID] = " + this.tbxIDFlt.Text.Trim();								//30.11.2015
			if (!string.IsNullOrWhiteSpace(this.tbxMessageIDFlt.Text)) ViewState["Filter"] += " AND [MessageID] = " + this.tbxMessageIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderIDFlt.Text)) ViewState["Filter"] += " AND [DeleteId] = " + this.tbxDeletedOrderIDFlt.Text.Trim();
			if (!string.IsNullOrWhiteSpace(this.tbxDeletedOrderMessageIDFlt.Text)) ViewState["Filter"] += " AND [DeleteMessageId] = " + this.tbxDeletedOrderMessageIDFlt.Text.Trim();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwDeleteMessage]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "[ID], [MessageId], [DeleteId], [DeleteMessageId], [DeleteMessageTypeId], [DeleteMessageTypeDescription], " +
								   "[SentDate], [DeleteMessageDate], [SentUserId], [IsActive], [ModifyDate], [ModifyUserId], [Source], " +
								   "[Notice], [DeletedByUserLastName], [DeletedByUserFirstName]";
			
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
				BC.ProcessException(ex, AppLog.GetMethodName());
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

			this.showPanel(ShowPanel.None);
			return;

			//GridViewRow selectedRow = grdData.SelectedRow;
			//int show = ConvertExtensions.ToInt32(selectedRow.Cells[9].Text.Trim(), -1);

			//if (show == (int)ShowPanel.ReceptionOrder)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showReceptionOrder(selectedRow);
			//}

			//if (show == (int)ShowPanel.ReceptionConfirmation)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showReceptionConfirmation(selectedRow);
			//}

			//if (show == (int)ShowPanel.KittingOrder)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showKittingOrder(selectedRow);
			//}

			//if (show == (int)ShowPanel.KittingConfirmation)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showKittingConfirmation(selectedRow);
			//}

			//if (show == (int)ShowPanel.ShipmentOrder)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showShipmentOrder(selectedRow);
			//}

			//if (show == (int)ShowPanel.ShipmentConfirmation)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showShipmentConfirmation(selectedRow);
			//}

			//if (show == (int)ShowPanel.RefurbishedOrder)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showRefurbishedOrder(selectedRow);
			//}

			//if (show == (int)ShowPanel.RefurbishedConfirmation)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showRefurbishedConfirmation(selectedRow);
			//}

			//if (show == (int)ShowPanel.ReturnedEquipment)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showReturnedEquipment(selectedRow);
			//}

			//if (show == (int)ShowPanel.ReturnedItem)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showReturnedItem(selectedRow);
			//}

			//if (show == (int)ShowPanel.ReturnedShipment)
			//{
			//	this.showPanel((ShowPanel)show);
			//	this.showReturnedShipment(selectedRow);
			//}

		}
				
		private void showPanel(ShowPanel showPanel)
		{
			switch (showPanel)
			{
				case ShowPanel.None:
					this.pnlReceptionOrder.Visible = false;
					this.pnlReceptionConfirmation.Visible = false;
					this.pnlKittingOrder.Visible = false;
					this.pnlKittingConfirmation.Visible = false;
					this.pnlShipmentOrder.Visible = false;
					this.pnlShipmentConfirmation.Visible = false;
					this.pnlRefurbishedOrder.Visible = false;
					this.pnlRefurbishedConfirmation.Visible = false;
					this.pnlReturnedEquipment.Visible = false;
					this.pnlReturnedItem.Visible = false;
					this.pnlReturnedShipment.Visible = false;
					break;

				case ShowPanel.ReceptionOrder:
					this.showPanel(ShowPanel.None);
					this.pnlReceptionOrder.Visible = true;
					break;

				case ShowPanel.ReceptionConfirmation:
					this.showPanel(ShowPanel.None);
					this.pnlReceptionConfirmation.Visible = true;
					break;

				case ShowPanel.KittingOrder:
					this.showPanel(ShowPanel.None);
					this.pnlKittingOrder.Visible = true;
					break;

				case ShowPanel.KittingConfirmation:
					this.showPanel(ShowPanel.None);
					this.pnlKittingConfirmation.Visible = true;
					break;

				case ShowPanel.ShipmentOrder:
					this.showPanel(ShowPanel.None);
					this.pnlShipmentOrder.Visible = true;
					break;

				case ShowPanel.ShipmentConfirmation:
					this.showPanel(ShowPanel.None);
					this.pnlShipmentConfirmation.Visible = true;
					break;

				case ShowPanel.RefurbishedOrder:
					this.showPanel(ShowPanel.None);
					this.pnlRefurbishedOrder.Visible = true;
					break;

				case ShowPanel.RefurbishedConfirmation:
					this.showPanel(ShowPanel.None);
					this.pnlRefurbishedConfirmation.Visible = true;
					break;

				case ShowPanel.ReturnedEquipment:
					this.showPanel(ShowPanel.None);
					this.pnlReturnedEquipment.Visible = true;
					break;

				case ShowPanel.ReturnedItem:
					this.showPanel(ShowPanel.None);
					this.pnlReturnedItem.Visible = true;
					break;
									
				case ShowPanel.ReturnedShipment:
					this.showPanel(ShowPanel.None);
					this.pnlReturnedShipment.Visible = true;
					break;
					
				default:
					break;
			}
		}

		private void showReceptionOrder(GridViewRow selectedRow)
		{
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [ItemSupplierDescription] " +
						  " FROM [dbo].[CommunicationMessagesReceptionSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReceptionOrderHeader.DataSource = myDataTable.DefaultView; 
				this.gvReceptionOrderHeader.DataBind();
				this.gvReceptionOrderHeader.SelectedIndex = -1;

				proS = string.Format("SELECT C.*,S.SourceCode  FROM [dbo].[vwCMRSentIt] C " +
									 "LEFT OUTER JOIN cdlSources S ON C.SourceId = S.Id " +
									 "WHERE C.CMSOId={0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvReceptionOrderItems.DataSource = myDataTable.DefaultView;
				this.gvReceptionOrderItems.DataBind();
				this.gvReceptionOrderItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for warning removal reasons
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
			}
		}

		private void showReceptionConfirmation(GridViewRow selectedRow)
		{			
			string proS = string.Empty;

			try
			{
				proS =  string.Format("SELECT [ID],[MessageId], [MessageDateOfReceipt], [CommunicationMessagesSentId], [ItemSupplierId], [ItemSupplierDescription], " +
						              "       [Reconciliation], [MessageDateOfShipment], [ItemDateOfDelivery] " +
						              "FROM   [dbo].[vwReceptionConfirmationHd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());
				
				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReceptionConfirmationHeader.DataSource = myDataTable.DefaultView;
				this.gvReceptionConfirmationHeader.DataBind();
				this.gvReceptionConfirmationHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID], [CMSOId], [ItemID], [ItemDescription], [ItemQuantityInt], [ItemUnitOfMeasure], " +
					                 "       [ItemQualityId], [IsActive], [ModifyDate], [ModifyUserId], [GroupGoods], [Code], [DescriptionCz], CMRSIItemQuantity " +
						             "FROM [dbo].[vwReceptionConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvReceptionConfirmationItems.DataSource = myDataTable.DefaultView;
				this.gvReceptionConfirmationItems.DataBind();
				this.gvReceptionConfirmationItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
			}
		}

		private void showKittingOrder(GridViewRow selectedRow)
		{
			string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CompanyName] " +
						  " FROM [dbo].[vwCMKSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvKittingOrderHeader.DataSource = myDataTable.DefaultView;
				this.gvKittingOrderHeader.DataBind();
				this.gvKittingOrderHeader.SelectedIndex = -1;

				proS = String.Format("SELECT [ID], [KitId], [KitDescription], [KitQuantity], [KitQuantityDelivered], " +
											 "[MeasuresID], [KitUnitOfMeasure], [KitQualityId], [KitQualityCode], [HeliosOrderID], " +
											 "[CardStockItemsId], KitQuantityInt, KitQuantityDeliveredInt " +
											 "FROM [dbo].[vwCMKSentItems] " +
											 "WHERE CMSOId = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvKittingOrderItems.DataSource = myDataTable.DefaultView;
				this.gvKittingOrderItems.DataBind();
				this.gvKittingOrderItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showKittingConfirmation(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [KitOrderID], [Reconciliation], " +
									  "       [MessageDateOfShipment], [KitDateOfDelivery], [HeliosObj], [ModelCPE], [OrderedKitQuantity], [DeliveredKitQuantity] " +
									  "FROM   [dbo].[vwKitConfirmationHd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvKittingConfirmationHeader.DataSource = myDataTable.DefaultView;
				this.gvKittingConfirmationHeader.DataBind();
				this.gvKittingConfirmationHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID],[CMSOId],[HeliosOrderID] ,[HeliosOrderRecordId] ,[KitId] ,[KitDescription],[DescriptionCz],[KitQuantity],[CMRSIItemQuantity]" +
									 ",[KitQuantityInt],[CMRSIItemQuantityInt],[KitUnitOfMeasure]  ,[KitQualityId] ,[IsActive] ,[ModifyDate],[ModifyUserId],[CommunicationMessagesSentId],[Code]" +
									 " FROM [dbo].[vwKitConfirmationIt] WHERE [IsActive] = {0} AND CMSOId = {1}", 1, selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvKittingConfirmationItems.DataSource = myDataTable.DefaultView;
				this.gvKittingConfirmationItems.DataBind();
				this.gvKittingConfirmationItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showShipmentOrder(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CustomerName], [CustomerCity] " +
						  " FROM [dbo].[CommunicationMessagesShipmentOrdersSent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvShipmentOrderHeader.DataSource = myDataTable.DefaultView;
				this.gvShipmentOrderHeader.DataBind();
				this.gvShipmentOrderHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID], SingleOrMaster, " +
										"ItemVerKit, [ItemOrKitID], [ItemOrKitDescription], [ItemOrKitUnitOfMeasure], " +
										"[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt " +
										"FROM [dbo].[vwShipmentOrderIt] WHERE [CMSOId] = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvShipmentOrderItems.DataSource = myDataTable.DefaultView;
				this.gvShipmentOrderItems.DataBind();
				this.gvShipmentOrderItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showShipmentConfirmation(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [ShipmentOrderID] " +
									  "     ,[Reconciliation], [ReconciliationYesNo], [MessageDateOfShipment], [RequiredDateOfShipment] " +
									  "     ,[IsActive], [ModifyDate], [CompanyName], [CompanyID], [City], [OrderTypeID], [OrderTypeDescription] " +
									  "     ,[ModifyUserId], [ModifyUserLastName], [ModifyUserFirstName]  " +
									  "FROM  [dbo].[vwShipmentConfirmationHd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvShipmentConfirmationHeader.DataSource = myDataTable.DefaultView;
				this.gvShipmentConfirmationHeader.DataBind();
				this.gvShipmentConfirmationHeader.SelectedIndex = -1;
				int shipmentOrderId = 0;
				if (myDataTable.DefaultView.Count > 0)
				{
					shipmentOrderId = ConvertExtensions.ToInt32(myDataTable.Rows[0]["ShipmentOrderID"], 0);
				}

				proS = string.Format("SELECT [ID] ,SingleOrMaster " +
									 "       ,ItemVerKit,[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitUnitOfMeasure] ,[ItemOrKitQualityCode], [ItemOrKitQuantityInt], ItemOrKitQuantityRealInt " +
									 "FROM [dbo].[vwShipmentOrderIt] WHERE [IsActive] = {0} AND [CMSOId] = {1}", 1, shipmentOrderId);

				myDataTable = BC.GetDataTable(proS);
				this.gvShipmentConfirmationItems.DataSource = myDataTable.DefaultView;
				this.gvShipmentConfirmationItems.DataBind();
				if (myDataTable.DefaultView.Count > 0)
				{
					this.gvShipmentConfirmationItems.SelectedIndex = -1;
				}
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showRefurbishedOrder(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageDateOfShipment], [CompanyName], [CustomerCity] " +
						  " FROM [dbo].[vwCMRF0Sent] " +
						  " WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedOrderHeader.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedOrderHeader.DataBind();
				this.gvRefurbishedOrderHeader.SelectedIndex = -1;

				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered]" +
									 ",[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId]" +
									 ",[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  FROM [dbo].[vwCMRF0SentIt] WHERE [CMSOId] = {0}", selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedOrderItems.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedOrderItems.DataBind();
				this.gvRefurbishedOrderItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showRefurbishedConfirmation(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[DateOfShipment],[RefurbishedOrderID],[ReconciliationYesNo], " +
									  "     [MessageDateOfShipment], [DateOfDelivery], [CompanyName], [City], [Reconciliation] " +
									  "FROM  [dbo].[vwRefurbishedConfirmationHd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedConfirmationHeader.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedConfirmationHeader.DataBind();
				this.gvRefurbishedConfirmationHeader.SelectedIndex = -1;
				int refurbishedOrderId = ConvertExtensions.ToInt32(myDataTable.Rows[0]["RefurbishedOrderID"], 0);
				 
				proS = string.Format("SELECT [ID],[CMSOId],[ItemVerKit],[ItemVerKitText],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity],[ItemOrKitQuantityDelivered] " +
						             "      ,[ItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId] " +
						             "      ,[ItemOrKitQualityCode],[IsActive],[ModifyDate],[ModifyUserId]  " + 
									 "FROM [dbo].[vwCMRF0SentIt] " +
									 "WHERE [IsActive] = {0} AND [CMSOId] = {1} ORDER BY ItemVerKit,ItemOrKitID", 1, refurbishedOrderId);

				myDataTable = BC.GetDataTable(proS);
				this.gvRefurbishedConfirmationItems.DataSource = myDataTable.DefaultView;
				this.gvRefurbishedConfirmationItems.DataBind();
				this.gvRefurbishedConfirmationItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showReturnedEquipment(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [Reconciliation], [IsActive], [ModifyDate], [ModifyUserId] " +									  
									  "FROM  [dbo].[vwReturnsItems] " +
									  "WHERE MessageID = {0}", selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReturnedEquipmentHeader.DataSource = myDataTable.DefaultView;
				this.gvReturnedEquipmentHeader.DataBind();
				this.gvReturnedEquipmentHeader.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }
		
		private void showReturnedItem(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID],[MessageId], [MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[Reconciliation] " +
									  "     ,[IsActive], [ModifyDate], [ModifyUserId], [DescriptionCz] " +
									  "FROM  [dbo].[vwVR2Hd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReturnedItemHeader.DataSource = myDataTable.DefaultView;
				this.gvReturnedItemHeader.DataBind();
				this.gvReturnedItemHeader.SelectedIndex = -1;
				
				proS = string.Format("SELECT [ID],[CMSOId],[ItemId],[ItemDescription],[ItemQuantity],ItemQuantityInt,[ItemOrKitQualityId],[ItemOrKitQuality] " +
									 "      ,[ItemUnitOfMeasureId],[ItemUnitOfMeasure],[SN],[NDReceipt],[ReturnedFrom],[IsActive],[ModifyDate]     " +
									 "      ,[ModifyUserId],[RIID],[RIMessageId],[RIMessageTypeId],[RIMessageDescription],[RIMessageDateOfReceipt] " +
									 "      ,[RIReconciliation],[RIIsActive],[RIModifyDate],[RIModifyUserId]                                       " +
									 "FROM [dbo].[vwVR2It] "+
									 "WHERE [IsActive] = {0} AND RIIsActive = {1} AND CMSOId = {2}", 1, 1, selectedRow.Cells[5].Text.Trim());

				myDataTable = BC.GetDataTable(proS);
				this.gvReturnedItemItems.DataSource = myDataTable.DefaultView;
				this.gvReturnedItemItems.DataBind();
				this.gvReturnedItemItems.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
            }
        }

		private void showReturnedShipment(GridViewRow selectedRow)
		{
			string proS = string.Empty;

			try
			{
				proS = string.Format("SELECT [ID], [MessageId], [MessageTypeId], [MessageDescription], [Reconciliation] " +
					                  "     ,[IsActive], [ModifyDate], [ModifyUserId], [DescriptionCz], CompanyName, ContactName " +
									  "FROM  [dbo].[vwVR3Hd] " +
									  "WHERE ID = {0} AND MessageID = {1}", selectedRow.Cells[5].Text.Trim(), selectedRow.Cells[6].Text.Trim());

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvReturnedShipmentHeader.DataSource = myDataTable.DefaultView;
				this.gvReturnedShipmentHeader.DataBind();
				this.gvReturnedShipmentHeader.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
                // 22.06.2016 (DJ) -> uncommented for project warning removal reasons
                BC.ProcessException(ex, AppLog.GetMethodName(), "proS =  " + proS);
			}
		}
	}
}