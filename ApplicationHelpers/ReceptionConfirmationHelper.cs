using System;
using System.Data;
using System.Data.SqlClient;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro HomeReconciliation a ReReconciliation
	/// </summary>
	public class ReceptionConfirmationHelper : BaseHelper
	{
		/// <summary>
		/// Počet nezamítnutých R1 k R0
		/// </summary>
		/// <param name="findReceptionOrderConfirmationID"></param>
		/// <returns>počet nezamítnutých R1 k R0 (vždy alespoň = 1 .. vyhodnocovaná R1)</returns>
		public static int GetReceptionOrderConfirmationCount(string findReceptionOrderConfirmationID)
		{
			int pocet = 0;
			int defaultR1Count = 0;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prGetReceptionOrderConfirmationCount]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindR1ID", SqlDbType.Int).Value = int.Parse(findReceptionOrderConfirmationID);
				sqlCommand.Parameters.Add("@parCountR1", SqlDbType.Int);
				sqlCommand.Parameters["@parCountR1"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
				sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					pocet = ConvertExtensions.ToInt32(ExecuteNonQueryWithParam("@parCountR1", sqlConnection, sqlCommand), defaultR1Count);
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			}

			return pocet;
		}

		/// <summary>
		/// Kontrola R1 na R0
		/// (musí souhlasit počet itemů, musí souhlasit množství objednané se skutečně dodaným množstvím, musí souhlasit porovnání množství na jednotlivých itemech)
		/// </summary>
		/// <param name="findReceptionOrderConfirmationID">R1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckReceptionConfirmationBalance(string findReceptionOrderConfirmationID)
		{
			bool confirmation = true;
			int defaultCheckValue = 1;

			using (SqlConnection sqlConnection = new SqlConnection(BC.FENIXWrtConnectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = "[dbo].[prCheckReceptionOrderConfirmation]";
				sqlCommand.CommandType = CommandType.StoredProcedure;

				sqlCommand.Parameters.Add("@parFindR1ID", SqlDbType.Int).Value = int.Parse(findReceptionOrderConfirmationID);
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
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			}

			return confirmation;
		}

		/// <summary>
		/// Kontrola R1 - musí souhlasit počet požadovaných(CPE) sériových čísel a v db uložených sériových čísel
		/// </summary>
		/// <param name="receptionConfirmationID">R1 ID</param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckReceptionConfirmationSNsCounts(string receptionConfirmationID)
		{
			bool result = false;
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prReceptionConfirmationGetSN]";
				sqlComm.Connection = conn;
				sqlComm.Parameters.Add("@ReceptionConfirmationID", SqlDbType.Int).Value = receptionConfirmationID;								
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
						Exception ex = new Exception(string.Format("SP prReceptionConfirmationGetSN vrátila hodnotu {0}", sqlComm.Parameters["@ReturnValue"].Value.ToString()));
						BC.ProcessException(ex, ApplicationLog.GetMethodName());
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
	}
}