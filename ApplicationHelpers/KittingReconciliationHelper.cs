using System;
using System.Data;
using System.Data.SqlClient;
using FenixHelper;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro KiReconciliation
	/// </summary>
	public class KittingReconciliationHelper : BaseHelper
	{
		/// <summary>
		/// Počet nezamítnutých K1 ke K0
		/// </summary>
		/// <param name="kittingOrderConfirmationID"></param>
		/// <returns>počet nezamítnutých K1 ke K0 (vždy alespoň = 1 .. vyhodnocovaná K1)</returns>
		public static int GetKittingOrderConfirmationCount(string kittingOrderConfirmationID)
		{
			int pocet = 0;
			int defaultK1Count = 0;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prGetKittingOrderConfirmationCount]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindK1ID", SqlDbType.Int).Value = int.Parse(kittingOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCountK1", SqlDbType.Int);
				sqlCommand.Parameters["@parCountK1"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					pocet = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCountK1", sqlConnection, sqlCommand), defaultK1Count);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, AppLog.GetMethodName());
				}
			}

			return pocet;
		}

		/// <summary>
		/// Kontrola K1 na K0
		/// (musí souhlasit počet itemů, musí souhlasit množství objednané se skutečně dodaným množstvím, musí souhlasit porovnání množství na jednotlivých itemech)
		/// </summary>
		/// <param name="kittingOrderConfirmationID">K1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckKittingOrderConfirmation(string kittingOrderConfirmationID)
		{
			bool confirmation = true;
			int defaultCheckValue = 1;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prCheckKittingOrderConfirmation]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindK1ID", SqlDbType.Int).Value = int.Parse(kittingOrderConfirmationID);
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
		/// Kontrola K1 - musí souhlasit počet požadovaných(CPE) sériových čísel a v db uložených sériových čísel
		/// </summary>
		/// <param name="kittingOrderConfirmationID">K1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckKittingConfirmationSNsCounts(string kittingOrderConfirmationID)
		{
			bool result = false;
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prKittingConfirmationGetSN]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@KittingConfirmationID", SqlDbType.Int).Value = kittingOrderConfirmationID;
				sqlComm.Parameters.Add("@CalculatedCountSN", SqlDbType.Decimal, 18);
				sqlComm.Parameters["@CalculatedCountSN"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@SavedSN", SqlDbType.VarChar, Int32.MaxValue);
				sqlComm.Parameters["@SavedSN"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 2048);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;
				sqlComm.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;

				try
				{
					conn.Open();
					sqlComm.ExecuteNonQuery();
					if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
					{
						result = CheckSNsCount(ConvertExtensions.ToDecimal(sqlComm.Parameters["@CalculatedCountSN"].Value, 0M), sqlComm.Parameters["@SavedSN"].Value.ToString());
					}
					else
					{
						Exception ex = new Exception(string.Format("SP prKittingConfirmationGetSN vrátila hodnotu {0}", sqlComm.Parameters["@ReturnValue"].Value.ToString()));
						BC.ProcessException(ex, AppLog.GetMethodName());
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