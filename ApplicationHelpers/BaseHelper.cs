using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.Extensions;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Bázová třída pro helpery
	/// </summary>
	public class BaseHelper
	{
		protected static object ExecuteNonQueryWithParam(string parameterName, SqlConnection sqlConnection, SqlCommand sqlCommand)
		{
			sqlCommand.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;

			sqlConnection.Open();
			sqlCommand.ExecuteNonQuery();

			if (sqlCommand.Parameters["@ReturnValue"].Value.ToString() == "0")
			{
				return sqlCommand.Parameters[parameterName].Value;
			}
			else
			{
				throw new Exception(sqlCommand.Parameters["@ReturnMessage"].Value.ToString());
			}
		}

		/// <summary>
		/// Rozhodnutí, zda GridView má záznamy
		/// </summary>
		/// <param name="gridView">testované GridView</param>
		/// <returns></returns>
		public static bool GridViewHasRows(GridView gridView)
		{
			return (gridView != null && gridView.Rows.Count > 0);
		}

		/// <summary>
		/// Vyhodnocení počtu seriových čísel (počet dle CPE x uložený počet v db)
		/// </summary>
		/// <param name="calculatedSN"></param>
		/// <param name="savedSN"></param>
		/// <returns>true .. kontrola je OK   false .. kontrola není OK</returns>
		public static bool CheckSNsCount(decimal calculatedSN, string savedSN)
		{
			if (calculatedSN == 0M && savedSN.IsNullOrEmpty())
				return true;
			else if (calculatedSN == 0M && savedSN.IsNotNullOrEmpty())
				return false;
			else if (calculatedSN > 0M && savedSN.IsNullOrEmpty())
				return false;
			else if (calculatedSN > 0M && savedSN.IsNotNullOrEmpty())
			{
				string sns = savedSN.Replace(";", ",").Replace(" ", "");
				string[] savedSNs = sns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				return (calculatedSN == savedSNs.GetLength(0));
			}

			return true;
		}

		#region Filtering 
		
		/// <summary>
		/// Naplní dropdown list seznamem zadavatelů (uživatelů)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlUserModify(ref DropDownList dropDownList)
		{
			string fileUsers = HttpContext.Current.Server.MapPath("App_Data") + @"\Users.xml";

			DataTable dt = new DataTable();

			DataColumn myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int32");
			myDataColumn.ColumnName = "ID";
			dt.Columns.Add(myDataColumn);

			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "Jmeno";
			dt.Columns.Add(myDataColumn);

			DataRow r = dt.NewRow();
			r[0] = -1; r[1] = " Vše ";
			dt.Rows.Add(r);

			string proSx = string.Empty; string s = string.Empty;
			using (XmlReader reader = XmlReader.Create(fileUsers))
			{
				reader.ReadStartElement("USERS");

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "Nazev")
						{
							s = reader.ReadString().ToUpper();
							proSx = string.Format("SELECT ZC_ID, Last_Name+' '+First_Name AS Jmeno FROM [dbo].[VW_EMPLOYEES] WHERE LOGIN_NAME='{0}' ORDER BY Last_Name,First_Name ", s);
							DataTable t = BC.GetDataTable(proSx, BC.ZczIntRdrConnectionString);
							DataRow rx = dt.NewRow();
							rx[0] = t.Rows[0][0].ToString();
							rx[1] = t.Rows[0][1].ToString();
							dt.Rows.Add(rx);
						}
					}
					reader.MoveToElement();
				}
				DataView dv = dt.DefaultView;
				dv.Sort = "Jmeno";
				dropDownList.DataSource = dv;
				dropDownList.DataValueField = "ID";
				dropDownList.DataTextField = "Jmeno";
				dropDownList.DataBind();
				dropDownList.SelectedValue = "-1";
			}
		}

		/// <summary>
		/// 'Manuálně' naplní dropdown list seznamem typů objednávek
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlOrderType(ref DropDownList dropDownList)
		{
			dropDownList.Items.Insert(0, new ListItem("Vše", "-1"));
			dropDownList.Items.Insert(1, new ListItem("Objednávka CPE", "1"));
			dropDownList.Items.Insert(2, new ListItem("Objednávka MAT", "2"));
		}

		/// <summary>
		/// 'Manuálně' naplní dropdown list seznamem rozhodnutí a zaostří volbu 'Bez vyjádření'
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlDecision(ref DropDownList dropDownList)
		{
			FillDdlDecision(ref dropDownList, 1);
		}

		/// <summary>
		/// 'Manuálně' naplní dropdown list seznamem rozhodnutí a zaostří volbu dle parametru selectedIndex
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlDecision(ref DropDownList dropDownList, int selectedIndex)
		{
			dropDownList.Items.Insert(0, new ListItem("Vše", "-1"));
			dropDownList.Items.Insert(1, new ListItem("Bez vyjádření", "0"));
			dropDownList.Items.Insert(2, new ListItem("Schváleno", "1"));
			dropDownList.Items.Insert(3, new ListItem("Zamítnuto", "2"));
			dropDownList.Items.Insert(4, new ListItem("D0 odeslána", "3"));
			//dropDownList.Items.Insert(5, new ListItem("Zrušeno D1", "4"));

			if (selectedIndex >= 0 && selectedIndex <= dropDownList.Items.Count)
			{
				dropDownList.SelectedIndex = selectedIndex;
			}
		}


		/// <summary>
		/// Naplní dropdown list seznamem jakostí (qualities)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlQualities(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
						 "(SELECT '-1' cValue,' VŠE ' ctext  " +
						  "UNION ALL " +
						  "SELECT ID [cValue], [Code] ctext  " +
						  "FROM [dbo].[cdlQualities] WHERE [IsActive] = 1 " +
						 ") xx  " +
						 "ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem statusů zpráv (statuses)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlMessageStatuses(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
						 "( " +
							 "SELECT '-1' cValue,' Vše' ctext  " +
							 "UNION ALL  " +
							 "SELECT ID [cValue], [DescriptionCz] ctext FROM [dbo].[cdlStatuses] " +
						 ") xx  " +
						 "ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem skupin zboží (GroupGoods)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlGroupGoods(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
						 "( " +
							"SELECT '-1' cValue,' VŠE ' ctext  " +
							"UNION ALL  " +
							"SELECT DISTINCT [GroupGoods] cValue, [GroupGoods] ctext FROM [dbo].[cdlItems] WHERE [IsActive] = 1 AND PC is not null " +
						 ") xx  " +
						 "ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem názvů firem/zákazníků (cdlDestinationPlaces)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCustomerName(ref DropDownList dropDownList)
		{
			string sql = "SELECT '-1' cValue,' Vše' ctext   " +
						 "UNION ALL   " +
						 "SELECT cdlDestPlaces.[ID] cValue, cdlDestPlaces.[CompanyName] ctext   " +
						 "FROM [dbo].[cdlDestinationPlaces] cdlDestPlaces   " +
						 "WHERE cdlDestPlaces.IsActive = 1 " +
						 "ORDER BY ctext ASC";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem měst firem/zákazníků (cdlDestinationPlaces)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCustomerCity(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM " +
						 "( " +
							"SELECT '-1' cValue,' Vše' ctext " +
							"UNION ALL   " +
							"SELECT ROW_NUMBER() OVER(ORDER BY tab.[CustomerCity]) as cValue, tab.[CustomerCity] cText " +
							"FROM   " +
							"(   " +
								"SELECT DISTINCT([City]) as [CustomerCity] FROM [dbo].[cdlDestinationPlaces]   " +
							") tab " +
						 ") xx " +
						 "ORDER BY ctext ASC";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem kódů kitů (cdlKits)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlKitCode(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM " +
						 "( " +
							"SELECT '-1' cValue,' Vše' cText " +
							"UNION ALL   " +
							"SELECT ROW_NUMBER() OVER(ORDER BY tab.[cText]) as cValue, tab.cText " +
							"FROM   " +
							"(   " +
								"SELECT DISTINCT([Code]) cText FROM [dbo].[cdlKits] WHERE IsActive = 1 " +
							") tab " +
						 ") xx " +
						 "ORDER BY ctext ASC";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem popisů kitů (cdlKits)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlKitDescription(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
	                     "( " +
	                     	"SELECT '-1' cValue,' VYBERTE' ctext  " +
	                     	"UNION ALL  " +
	                     	"SELECT ID cValue, [DescriptionCz] ctext  " +
	                     	"FROM [dbo].[cdlKits] "+
							"WHERE [IsActive] = 1 " +
	                     ") tab " +
	                     "ORDER BY ctext";
			
			BasePage.FillDdl(ref dropDownList, sql);
		}
		
		/// <summary>
		/// Naplní dropdown list seznamem typů itemů (cdlItemTypes)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlItemType(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
                		 "( " +
                			"SELECT '-1' cValue,' VŠE ' ctext  " +
                			"UNION ALL  " +
                			"SELECT [Code] cValue, [Code] ctext  " +
                			"FROM [dbo].[cdlItemTypes] WHERE [IsActive] = 1 " +
                		 ") xx  " +
                		 "ORDER BY ctext ";

			BasePage.FillDdl(ref dropDownList, sql);
		}
		
		#endregion

		/// <summary>
		/// Pro D0 (DeleteMessageSent a DeleteMessage) vrací MessageStatusID
		/// </summary>
		/// <param name="deleteId"></param>
		/// <param name="deleteMessageId"></param>
		/// <param name="deleteMessageTypeId"></param>
		/// <returns></returns>
		internal static int D0GetLastMessageStatus(string deleteId, string deleteMessageId, string deleteMessageTypeId)
		{
			int result = -1;
			string tableName = BC.DeleteMessageViaXML == false ? "DeleteMessageSent" : "CommunicationMessagesDeleteMessage";

			string commText = String.Format(
								"SELECT TOP 1 ISNULL([MessageStatusId], 0) " +
								"FROM [dbo].[{0}] " +
								"WHERE [DeleteId] = {1} AND [DeleteMessageId] = {2} AND [DeleteMessageTypeId] = {3} AND IsActive = 1 " +
								"ORDER BY [ModifyDate] DESC", tableName, deleteId, deleteMessageId, deleteMessageTypeId);

			using (SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString))
			using (SqlCommand comm = new SqlCommand(commText, conn))
			{
				try
				{
					conn.Open();
					SqlDataReader rdr = comm.ExecuteReader();
					if (rdr.Read())
					{
						result = rdr.GetInt32(0);
					}
					else
					{
						result = 0;
					}
					rdr.Close();
					rdr.Dispose();
				}
				catch { }
			}

			return result;
		}

		/// <summary>
		/// Zrušení objednávky I. etapa - vložení záznamu do tabulky DeleteMessageSent (pro zrušení message emailem)
		/// </summary>
		/// <param name="deleteId"></param>
		/// <param name="deleteMessageId"></param>
		/// <param name="deleteMessageTypeId"></param>
		/// <param name="deleteMessageDescription"></param>
		/// <returns></returns>
		internal static bool D0SentIns(string deleteId, string deleteMessageId, string deleteMessageTypeId, string deleteMessageDescription, int deleteUserID)
		{
			bool result = false;

			SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
			SqlCommand sqlComm = new SqlCommand();
			sqlComm.CommandType = CommandType.StoredProcedure;
			sqlComm.CommandText = "[dbo].[prDMD0SentIns]";
			sqlComm.Connection = conn;

			sqlComm.Parameters.Add("@DeleteId", SqlDbType.Int).Value = ConvertExtensions.ToInt32(deleteId, 0);
			sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = ConvertExtensions.ToInt32(deleteMessageId, 0);
			sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = ConvertExtensions.ToInt32(deleteMessageTypeId, 0);
			sqlComm.Parameters.Add("@DeleteMessageDescription", SqlDbType.NVarChar, 200).Value = deleteMessageDescription;
			sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = deleteUserID;
			
			sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
			sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
			sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
			sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

			try
			{
				conn.Open();
				sqlComm.ExecuteNonQuery();
				result = (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0");
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

			return result;
		}

		/// <summary>
		/// Nastavení obrázku "Zrušit objednávku" (pro R0, K0, S0, RF0)
		/// </summary>
		internal static void SetPictureDeleteOrder(ref GridView grdData, int columnMessageSatusId, string messageTypeId)
		{
			ImageButton img = new ImageButton();
			foreach (GridViewRow gvr in grdData.Rows)
			{
				img = (ImageButton)gvr.FindControl("btnDeleteOrder");				
				if (gvr.Cells[columnMessageSatusId].Text == "3")
				{
					int result = BaseHelper.D0GetLastMessageStatus(gvr.Cells[2].Text, gvr.Cells[3].Text, messageTypeId);
					if (result >= 0)
					{
						if (result == 0)
						{
							img.ImageUrl = "img/delete_dustbin.png";
							img.ToolTip = "Zrušit objednávku";
							img.Enabled = true;
						}
						else
						{
							img.ImageUrl = "img/delete_dustbin_red2.png";
							img.ToolTip = string.Empty;
							img.Enabled = false;
						}
						img.Visible = true;
					}
				}
				else
				{
					img.Enabled = false;
					img.Visible = false;
				}
			}
		}

		/// <summary>
		/// Zrušení objednávky I. etapa 
		/// - vložení záznamu do tabulky DeleteMessageSent  (pro zrušení message emailem)
		/// - vložení záznamu do tabulky CommunicationMessagesDeleteMessage (pro zrušení message pomocí XML - R0, K0, S0, RF0)
		/// - barva obrázku 'popelnice' změněna na červenou
		/// </summary>
		/// <param name="e"></param>
		internal static void ProcessDeleteOrder(GridViewCommandEventArgs e, ref GridView grdData, string messageTypeId, string messageTypeDescription, int userID)
		{
			string id = e.CommandArgument.ToString();
			foreach (GridViewRow gridViewRow in grdData.Rows)
			{
				if (gridViewRow.Cells[2].Text.Trim() == id)
				{
					try
					{
						if (BC.DeleteMessageViaXML == false)
						{
							BaseHelper.D0SentIns(id, gridViewRow.Cells[3].Text, messageTypeId, messageTypeDescription, userID);
						}
						else
						{
							BaseHelper.DeleteMessageIns(id, gridViewRow.Cells[3].Text, messageTypeId, messageTypeDescription, userID);
						}
						ImageButton ib = (ImageButton)gridViewRow.FindControl("btnDeleteOrder");
						ib.ImageUrl = "img/delete_dustbin_red2.png";
						ib.ToolTip = string.Empty;
						ib.Enabled = false;
					}
					catch (Exception ex)
					{
						BC.ProcessException(ex, ApplicationLog.GetMethodName());
					}
					break;
				}
			}
		}

		/// <summary>
		/// Zrušení objednávky I. etapa - vložení záznamu do tabulky CommunicationMessagesDeleteMessage (pro zrušení message pomocí XML)
		/// </summary>
		/// <param name="deleteId"></param>
		/// <param name="deleteMessageId"></param>
		/// <param name="deleteMessageTypeId"></param>
		/// <param name="deleteMessageDescription"></param>
		/// <param name="deleteUserID"></param>
		private static void DeleteMessageIns(string deleteId, string deleteMessageId, string deleteMessageTypeId, string deleteMessageDescription, int deleteUserID)
		{			
			using (var db = new FenixEntities())
			{
				using (var tr = db.Database.BeginTransaction())
				{
					try
					{
						db.Configuration.LazyLoadingEnabled = false;
						db.Configuration.ProxyCreationEnabled = false;

						CommunicationMessagesDeleteMessage communicationMessagesDeleteMessage = new CommunicationMessagesDeleteMessage();
						communicationMessagesDeleteMessage.MessageId = getLastFreeNumber(db);
						communicationMessagesDeleteMessage.MessageTypeId = 15;
						communicationMessagesDeleteMessage.MessageTypeDescription = "DeleteMessage";
						communicationMessagesDeleteMessage.MessageStatusId = 1;

						communicationMessagesDeleteMessage.DeleteId = ConvertExtensions.ToInt32(deleteId, 0);
						communicationMessagesDeleteMessage.DeleteMessageId = ConvertExtensions.ToInt32(deleteMessageId, 0);
						communicationMessagesDeleteMessage.DeleteMessageTypeId = ConvertExtensions.ToInt32(deleteMessageTypeId, 0);
						communicationMessagesDeleteMessage.DeleteMessageTypeDescription = deleteMessageDescription;

						communicationMessagesDeleteMessage.Notice = null;
						communicationMessagesDeleteMessage.SentDate = null;
						communicationMessagesDeleteMessage.SentUserId = null;

						communicationMessagesDeleteMessage.IsActive = true;
						communicationMessagesDeleteMessage.ModifyDate = DateTime.Now;
						communicationMessagesDeleteMessage.ModifyUserId = deleteUserID;

						db.CommunicationMessagesDeleteMessage.Add(communicationMessagesDeleteMessage);
						db.SaveChanges();

						tr.Commit();						
					}
					catch (Exception ex)
					{						
						BC.ProcessException(ex, ApplicationLog.GetMethodName());
						tr.Rollback();
					}
				}
			}

		}
		
		private static int getLastFreeNumber(FenixEntities db)
		{
			int lastFreeNumber = 0;

			var messageNumber = (from n in db.cdlMessageNumber
							     where n.Code == "1"
							     select n).FirstOrDefault();
			if (messageNumber != null)
			{
				lastFreeNumber = messageNumber.LastFreeNumber;
			}
			else
			{
				lastFreeNumber = 1;
			}

			messageNumber.LastFreeNumber = lastFreeNumber + 1;
			db.SaveChanges();

			return lastFreeNumber;
		}
	}
}