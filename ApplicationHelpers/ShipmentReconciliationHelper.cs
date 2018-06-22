using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro KiShipmentReconciliation (S1 - Schválení expedice)
	/// </summary>
	public class ShipmentReconciliationHelper : BaseHelper
	{
		/// <summary>
		/// Počet nezamítnutých S1 k S0
		/// </summary>
		/// <param name="shipmentOrderConfirmationID">S1 ID</param>
		/// <returns>počet nezamítnutých S1 k S0 (vždy alespoň = 1 .. vyhodnocovaná S1)</returns>
		public static int GetShipmentOrderConfirmationCount(string shipmentOrderConfirmationID)
		{
			int pocet = 0;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prGetShipmentOrderConfirmationCount]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindS1ID", SqlDbType.Int).Value = int.Parse(shipmentOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCountS1", SqlDbType.Int);
				sqlCommand.Parameters["@parCountS1"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					pocet = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCountS1", sqlConnection, sqlCommand), 0);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			}

			return pocet;
		}

		/// <summary>
		/// Kontrola S1 na S0
		/// (musí souhlasit počet itemů, musí souhlasit množství objednané se skutečně dodaným množstvím, musí souhlasit porovnání množství na jednotlivých itemech)
		/// </summary>
		/// <param name="shipmentOrderConfirmationID">S1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckShipmentOrderConfirmation(string shipmentOrderConfirmationID)
		{
			bool confirmation = false;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prCheckShipmentOrderConfirmation]";
				sqlCommand.CommandType = CommandType.StoredProcedure;
				
				sqlCommand.Parameters.Add("@parFindS1ID", SqlDbType.Int).Value = int.Parse(shipmentOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCheckValue", SqlDbType.Int);
				sqlCommand.Parameters["@parCheckValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;
				
				try
				{
					int checkValue = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCheckValue", sqlConnection, sqlCommand), 0);
					confirmation = (checkValue == 1);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			}

			return confirmation;
		}

		/// <summary>
		/// Kontrola S1 - musí souhlasit počet požadovaných(CPE) sériových čísel a v db uložených sériových čísel
		/// </summary>
		/// <param name="shipmentConfirmationID">S1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckShipmentConfirmationSNsCounts(string shipmentConfirmationID)
		{
			bool result = true;
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prShipmentConfirmationGetSN]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@ShipmentConfirmationID", SqlDbType.Int).Value = shipmentConfirmationID;				
				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 2048);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;
				sqlComm.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;
				SqlDataReader reader;

				try
				{
					conn.Open();					
					reader = sqlComm.ExecuteReader();
					while (reader.Read())
					{
						if (CheckSNsCount(ConvertExtensions.ToDecimal(reader[0], 0M), ConvertExtensions.ToString(reader[1], string.Empty)) == false)
						{
							result = false;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
				finally
				{
					conn.Close();
					conn = null;
					sqlComm = null;
				}
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}

			return result;
		}
		
		#region Filtering

		/// <summary>
		/// Naplní dropdown list seznamem jmen měst, která se vyskytují v S1 .. shipment order confirmation  (potvrzení objednávky závozu)
		/// </summary>
		/// <param name="dropDownList"></param>
		/// <param name="operationType"></param>
		internal static void FillDdlCity(ref DropDownList dropDownList)
		{
			string sql = "SELECT cValue, ctext FROM " +
						 "(  " +
							"SELECT '-1' cValue,' Vše' ctext   " +
							"UNION ALL  " +
							"SELECT ROW_NUMBER() OVER(ORDER BY tab.[CustomerCity]) as cValue, tab.[CustomerCity] cText   " +
							"FROM  " +
							"(  " +
								"SELECT DISTINCT(cDP.City) as [CustomerCity] FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation] CMRC " +
								"INNER JOIN [dbo].[cdlDestinationPlaces] cDP " +
									"ON CMRC.CustomerId = cDP.ID " +
							") tab  " +
						 ") xx   " +
						 "ORDER BY ctext ";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem názvů firem, které se vyskytují v S1 .. shipment order confirmation  (potvrzení objednávky závozu)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCompanyName(ref DropDownList dropDownList)
		{
			string sql = "SELECT cValue, ctext FROM  " +
						 "( " +
							"SELECT '-1' cValue,' Vše' ctext  " +
							"UNION ALL  " +
							"SELECT cdlDestPlaces.[ID] cValue, cdlDestPlaces.[CompanyName] ctext  " +
							"FROM [dbo].[cdlDestinationPlaces] cdlDestPlaces  " +
							"WHERE cdlDestPlaces.[ID] in (select distinct(sos.[CustomerID]) FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation] sos)  " +
						 ") xx  " +
						 "ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem popisů itemů, které nejsou materiál (NW0)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlItemDescriptionWithoutMaterial(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
                         "( " +
                         	"SELECT '-1' cValue,' VYBERTE' ctext  " +
                         	"UNION ALL  " +
                         	"SELECT ID cValue, [DescriptionCz] ctext  " +
                         	"FROM [dbo].[cdlItems]  " +
                         	"WHERE [IsActive] = 1 AND [ItemType] <> 'NW0' " +
                         ") tab " +
                         "ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Kontrola unikátnosti(Material/KIT) přidávaného do filtrace dle popisu
		/// (dané zboží se může vyskytovat pouze 1x)
		/// </summary>
		/// <param name="par1"></param>
		/// <returns></returns>
		public static bool GoodIsUniqueInFltByDescr(GridView gvKitsOrItemsNew, DropDownList ddlKits, DropDownList ddlNW, string checkBoxName, string par1)
		{
			bool isUnique = true;

			if (ddlKits.SelectedValue != "-1" && par1 == "KIT" || ddlNW.SelectedValue != "-1" && par1 != "KIT")
			{
				if (BaseHelper.GridViewHasRows(gvKitsOrItemsNew))
				{
					int newItemVerKit = int.MinValue;
					int newItemOrKitID = int.MinValue;

					if (par1 == "KIT")
					{
						newItemVerKit = 1;
						newItemOrKitID = ConvertExtensions.ToInt32(ddlKits.SelectedValue, int.MinValue);
					}
					else
					{
						newItemVerKit = 0;
						newItemOrKitID = ConvertExtensions.ToInt32(ddlNW.SelectedValue, int.MinValue);
					}

					//CheckBox myChkb;
					foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
					{
						//myChkb = (CheckBox)gvr.FindControl(checkBoxName);
						int existingItemVerKit = ConvertExtensions.ToInt32(gvr.Cells[1].Text, -1);
						int existingItemOrKitID = ConvertExtensions.ToInt32(gvr.Cells[2].Text, -1);

						if ((newItemVerKit == existingItemVerKit) && (newItemOrKitID == existingItemOrKitID))
						{
							isUnique = false;
							break;
						}
					}
				}
			}

			return isUnique;
		}

		#endregion
	}
}