using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using FenixHelper;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída, která poskytuje data pro : Historie pohybů SN
	/// </summary>
	public class HistoryMovesSN
	{
		/// <summary>
		/// Pro zadané SN, vrací data pro historii pohybů SN
		/// </summary>
		/// <param name="snToFind"></param>
		/// <returns></returns>
		public static DataTable GetDataForSN(string snToFind)
		{			
			DataTable dataTable = new DataTable();

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{				
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prHistoryMovesSN]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@SNtoFind", SqlDbType.NVarChar, 50).Value = snToFind.Trim();
				sqlCommand.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;
				
				try
				{
					DataSet dataSet = new DataSet();
					sqlConnection.Open();
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
					sqlDataAdapter.Fill(dataSet);
					dataTable = dataSet.Tables[0];
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, AppLog.GetMethodName());					
				}				
			}

			return dataTable;
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro K1
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetKittingConfirmationData(GridViewRow gridViewRow)
		{
			string sqlSlect = string.Format(
							"SELECT   [ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [KitOrderID], [Reconciliation]  " +
							"        ,[MessageDateOfShipment], [KitDateOfDelivery], [HeliosObj], [ModelCPE], [OrderedKitQuantity], [DeliveredKitQuantity], [ReconciliationText] " +
							"FROM [dbo].[vwKitConfirmationHd] " +
							"WHERE ID = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro K1 Detail
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetKittingConfirmationDetail(GridViewRow gridViewRow)
		{
			string sqlSlect = string.Format(
							"SELECT	 [ID],[CMSOId],[HeliosOrderID] ,[HeliosOrderRecordId] ,[KitId] ,[KitDescription],[DescriptionCz],[KitQuantity],[CMRSIItemQuantity] " +
							"		,[KitQuantityInt],[CMRSIItemQuantityInt],[KitUnitOfMeasure], [KitQualityId] ,[IsActive] ,[ModifyDate],[ModifyUserId] " +
							"		,[CommunicationMessagesSentId],[Code] " +
							"FROM [dbo].[vwKitConfirmationIt] WHERE [IsActive] = 1 AND CMSOId = {0}",
							gridViewRow.Cells[3].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro S1
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetShipmentConfirmationData(GridViewRow gridViewRow)
		{
			string sqlSlect = string.Format(
							"SELECT [ID], [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [ShipmentOrderID], [Reconciliation], [ReconciliationYesNo] " +
							"      ,[MessageDateOfShipment], [RequiredDateOfShipment], [IsActive], [ModifyDate], [CompanyName], [CompanyID], [City] " +
							"      ,[OrderTypeID], [OrderTypeDescription], [ModifyUserId], [ModifyUserLastName], [ModifyUserFirstName] " +
							"FROM [dbo].[vwShipmentConfirmationHd] " +
							"WHERE ID = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro S1 Detail (údaje o firmě)
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetShipmentConfirmationCompanyDetail(GridViewRow gridViewRow)
		{
			string sqlSlect = string.Format(
							"SELECT CMSOS.[CustomerName], CMSOS.[CustomerAddress1], CMSOS.[CustomerAddress2], CMSOS.[CustomerAddress3], CMSOS.[CustomerCity], CMSOS.[CustomerZipCode] " +
							"      ,CMSOS.[RequiredDateOfShipment], CMSOS.[IsActive], vw.[ShipmentOrderID] " +
							"FROM [dbo].[vwShipmentConfirmationHd] vw " +
							"INNER JOIN [dbo].[CommunicationMessagesShipmentOrdersSent] CMSOS " +
							"  ON vw.ShipmentOrderID=CMSOS.ID WHERE vW.ID = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro S1 Detail
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetShipmentConfirmationDetail(GridViewRow gridViewRow)
		{			
			string sqlSlect = string.Format(
							"SELECT [ID] ,[CMSOId],[SingleOrMaster],[HeliosOrderRecordID],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[CMRSIItemQuantity],[ItemOrKitUnitOfMeasureId]" +
							"      ,[ItemOrKitUnitOfMeasure],[ItemOrKitQualityId],[ItemOrKitQualityCode],[IncotermsId],[IncotermDescription],[RealDateOfDelivery],[RealItemOrKitQuantity]" +
							"      ,[RealItemOrKitQualityID],[RealItemOrKitQuality],[Status],[KitSNs],[IsActive],[ModifyDate],[ModifyUserId],[Code],[CommunicationMessagesSentId]" +
							"      ,[ItemOrKitQuantityReal],[CardStockItemsId],[VydejkyId],[ShipmentOrderSource],RealItemOrKitQuantityInt,CMRSIItemQuantityInt" +
							"      ,ItemOrKitQuantityRealInt,CMSOSIItemOrKitQuantity " + 
							"FROM [dbo].[vwShipmentConfirmationIt] "+ 
							"WHERE ID = {0}", 
							gridViewRow.Cells[3].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}
				
		/// <summary>
		/// Z historie pohybů SN, vrací data pro RF1
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetRefurbishedConfirmationData(GridViewRow gridViewRow)
		{			
			string sqlSlect = string.Format(				
							"SELECT [ID], [MessageId], [MessageTypeId], [MessageDescription], [DateOfShipment], [RefurbishedOrderID], [CustomerID] " +
							"      ,[Reconciliation], [ReconciliationYesNo], [IsActive], [ModifyDate], [ModifyUserId], [MessageDateOfShipment] " +
							"      ,[DateOfDelivery], [CompanyName], [City] " +
							"FROM [dbo].[vwRefurbishedConfirmationHd] " +
							"WHERE ID = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro RF1 Detail (údaje o firmě)
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetRefurbishedConfirmationCompanyDetail(GridViewRow gridViewRow)
		{			
			string sqlSlect = string.Format(
							"SELECT	cDp.[CompanyName] [CustomerName], cDp.[StreetName] [CustomerAddress1], cDp.[StreetHouseNumber] [CustomerAddress2] " +
							"      ,cDp.[StreetOrientationNumber] [CustomerAddress3], cDp.[City] [CustomerCity], cDp.[ZipCode] [CustomerZipCode] " +
							"      ,vw.[DateOfDelivery] [RequiredDateOfShipment], cDp.[IsActive], vw.[RefurbishedOrderID] " +
							"FROM [dbo].[vwRefurbishedConfirmationHd] vw  " +
							"INNER JOIN [dbo].[cdlDestinationPlaces] cDp " +
							"  ON vw.[CustomerID]=cDp.ID WHERE vw.ID = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}

		/// <summary>
		/// Z historie pohybů SN, vrací data pro RF1 Detail
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		public static DataTable GetRefurbishedConfirmationDetail(GridViewRow gridViewRow)
		{			
			string sqlSlect = string.Format(
							"SELECT DISTINCT [ID],[CMSOId],[ItemVerKit],[ItemOrKitID],[ItemOrKitDescription],[ItemOrKitQuantity] " +
							"               ,[ItemOrKitQuantityInt],[ItemOrKitUnitOfMeasureId],[ItemOrKitUnitOfMeasure],ItID  " +
							"               ,[ItemOrKitQualityId],[ItemOrKitQualityCode],[IncotermsId],[IncotermDescription],[NDReceipt],[KitSNs],[IsActive],[ModifyDate],[ModifyUserId],[RefurbishedOrderID] " +
							"               ,[COIItemOrKitQuantityInt],[ItemOrKitQuantityDeliveredInt],ItemVerKitText   " +
							"FROM [dbo].[vwRefurbishedConfirmationIt]  " +
							"WHERE CMSOId = {0}",
							gridViewRow.Cells[1].Text.ToUpper().Trim()
							);

			return BC.GetDataTable(sqlSlect);
		}
	}
}