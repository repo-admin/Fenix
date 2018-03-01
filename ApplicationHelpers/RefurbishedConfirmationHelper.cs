using System;
using System.Data;
using System.Data.SqlClient;
using FenixHelper;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro VrRepaseRF1
	/// </summary>
	public class RefurbishedConfirmationHelper : BaseHelper
	{
		/// <summary>
		/// Počet nezamítnutých RF1 k RF0
		/// </summary>
		/// <param name="refurbishedOrderConfirmationID"></param>
		/// <returns>počet nezamítnutých RF1 k RF0 (vždy alespoň = 1 .. vyhodnocovaná RF1)</returns>
		public static int GetRefurbishedOrderConfirmationCount(string refurbishedOrderConfirmationID)
		{
			int pocet = 0;
			int defaultRF1Count = 0;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prGetRefurbishedOrderConfirmationCount]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindRF1ID", SqlDbType.Int).Value = int.Parse(refurbishedOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCountRF1", SqlDbType.Int);
				sqlCommand.Parameters["@parCountRF1"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					pocet = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCountRF1", sqlConnection, sqlCommand), defaultRF1Count);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, AppLog.GetMethodName());
				}
			}

			return pocet;
		}

		/// <summary>
		/// Kontrola RF1 na RF0
		/// (musí souhlasit počet itemů, musí souhlasit množství objednané se skutečně dodaným množstvím, musí souhlasit porovnání množství na jednotlivých itemech)
		/// </summary>
		/// <param name="refurbishedOrderConfirmationID">RF1 ID</param>
		/// <returns></returns>
		public static bool CheckRefurbishedOrderConfirmation(string refurbishedOrderConfirmationID)
		{
			bool confirmation = true;
			int defaultCheckValue = 1;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prCheckRefurbishedOrderConfirmation]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindRF1ID", SqlDbType.Int).Value = int.Parse(refurbishedOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCheckValue", SqlDbType.Int);
				sqlCommand.Parameters["@parCheckValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					int checkValue = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCheckValue", sqlConnection, sqlCommand), defaultCheckValue);
					confirmation = (checkValue == 1);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, AppLog.GetMethodName());
				}
			}

			return confirmation;
		}

		/// <summary>
		/// Kontrola RF1 - musí souhlasit počet požadovaných(CPE) sériových čísel a v db uložených sériových čísel
		/// </summary>
		/// <param name="refurbishedConfirmationID">RF1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckRefurbishedConfirmationSNsCounts(string refurbishedConfirmationID)
		{
			bool result = true;
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prRefurbishedConfirmationGetSN]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@RefurbishedConfirmationID", SqlDbType.Int).Value = refurbishedConfirmationID;
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
					BC.ProcessException(ex, AppLog.GetMethodName());
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
				BC.ProcessException(ex, AppLog.GetMethodName());
			}

			return result;
		}
	}
}