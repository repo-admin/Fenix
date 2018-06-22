using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Fenix.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UPC.WebUtils.Users;

namespace Fenix
{
	internal enum OperationType
	{
		ReceptionOrder = 0,
		ReceptionOrderManually = 1,
		ReceptionConfirmation = 2,
		KittingOrder = 3,
		KittingConfirmation = 4,
		ShipmentOrder = 5,
		ShipmentConfirmation = 6
	}

	/// <summary>
	/// Místo(zdroj), kde vznikl interní doklad
	/// </summary>
	public enum InternalDocumentsSource
	{
		/// <summary>
		/// Fenix - skladová karta
		/// </summary>
		FenixCardStock = 1,

		/// <summary>
		/// Fenix - uvolnění kitů z TRR
		/// </summary>
		FenixReleaseKit = 2,

		/// <summary>
		/// FenixAutomat - D0 delete message email
		/// </summary>
		FenixAutomatDeleteMessageEmail = 3,

		/// <summary>
		/// Ručně (ad hoc)
		/// </summary>
		Manually = 4
	} 

	internal class BC
	{
		/// <summary>
		/// OK
		/// <value>0</value>
		/// </summary>
		internal const int OK = 0;

		/// <summary>
		/// Not OK
		/// <value>-1</value>
		/// </summary>
		internal const int NOT_OK = -1;
		
		/// <summary>
		/// Číslo první stránky pageru
		/// <value> = 1</value>
		/// </summary>
		internal const int PAGER_FIRST_PAGE = 1;

		/// <summary>
		/// SqlCommand command timeout [sec]
		/// <value> = 600</value>
		/// </summary>
		internal const int SQL_COMMAND_TIMEOUT = 600;
		
		/// <summary>
		/// Titulek aplikace (může být nahrazen údajem z web.config)
		/// </summary>
		internal const string DEFAULT_APP_TITLE = "FENIX";

		/// <summary>
		/// Formátovací řetězec pro datum bez času
		/// <value>: dd.MM.yyyy</value>
		/// </summary>
		internal const string DATE_TIME_FORMAT_DDMMYYY = "dd.MM.yyyy";

		/// <summary>
		/// Formátovací řetězec pro datum s časem
		/// <value>: dd.MM.yyyy HH:mm:ss</value>
		/// </summary>
		internal const string DATE_TIME_FORMAT_DDMMYYY_HHMMSS = "dd.MM.yyyy HH:mm:ss";
		
		/// <summary>
		/// Zicyz ID ([IDzc]) managera logistiky
		/// </summary>
		internal const string MANAGER_LOGISTIKA = "780";

		/// <summary>
		/// Zicyz ID ([IDzc]) REZLER 
		/// </summary>
		internal const string REZLER = "1084";

		/// <summary>
		/// Zicyz ID ([IDzc]) WECZEREK
		/// </summary>
		internal const string WECZEREK = "542";

		internal const string SCHVALENO = "SCHVÁLENO";
		
		internal const string ZAMITNUTO = "ZAMÍTNUTO";

		internal const string D0_ODESLANA = "D0 ODESLÁNA";
		
		internal static string FENIXRdrConnectionString
		{
			get { try { return ConfigurationManager.ConnectionStrings["FENIXR"].ConnectionString; } catch { return String.Empty; } }
		}

		internal static string FENIXWrtConnectionString
		{
			get { try { return ConfigurationManager.ConnectionStrings["FENIXW"].ConnectionString; } catch { return String.Empty; } }
		}
		
		internal static string ZczIntRdrConnectionString
		{
			get { try { return ConfigurationManager.ConnectionStrings["ZICYZ"].ConnectionString; } catch { return String.Empty; } }
		}

		internal static string UirAdrRdrConnectionStringReader
		{
			get { try { return ConfigurationManager.ConnectionStrings["UirAdr"].ConnectionString; } catch { return String.Empty; } }
		}

		internal static string MailErrorTo
		{
			get
			{
				try { return WebConfigurationManager.AppSettings["MailErrorTo"]; }
				catch { return String.Empty; }
			}
		}

		internal static string MailWarningTo
		{
			get
			{
				try { return WebConfigurationManager.AppSettings["MailWarningTo"]; }
				catch { return String.Empty; }
			}
		}

		internal static bool IsInternet
		{
			get { try { return WebConfigurationManager.AppSettings["IsInternet"] == "1"; } catch { return false; } }
		}

		internal static string SecretKey
		{
			get { try { return WebConfigurationManager.AppSettings["SecretKey"].Trim(); } catch { return String.Empty; } }
		}

		internal static double ValidityPeriod
		{
			get { try { return Double.Parse(WebConfigurationManager.AppSettings["ValidityPeriod"]); } catch { return 3600; } }
		}

		/// <summary>
		/// Titulek aplikace
		/// </summary>
		internal static string AppTitle
		{
			get { try { return WebConfigurationManager.AppSettings["AppTitle"].Trim(); } catch { return DEFAULT_APP_TITLE; } }
		}

		/// <summary>
		/// Režim DEBUG
		/// </summary>
		internal static bool IsDebug
		{
			get { try { return WebConfigurationManager.AppSettings["IsDebug"] == "1"; } catch { return false; } }
		}

		/// <summary>
		/// Červená barva
		/// </summary>
		internal static Color RedColor
		{
			get { return getColorFromWebConfig("Red", Color.Red); }
		}
		
		/// <summary>
		/// Modrá barva
		/// </summary>
		internal static Color BlueColor
		{
			get { return getColorFromWebConfig("Blue", Color.Blue); }
		}

		/// <summary>
		/// Barva pozadí lichého řádku
		/// </summary>
		internal static Color OddRowColor
		{
			get { return getColorFromWebConfig("OddRow", Color.White); }
		}

		/// <summary>
		/// Barva pozadí sudého řádku
		/// </summary>
		internal static Color EvenRowColor
		{
			get { return getColorFromWebConfig("EvenRow", Color.White); }
		}

		/// <summary>
		/// Barva pozadí vybraného řádku
		/// </summary>
		internal static Color SelectedRowColor
		{
			get { return getColorFromWebConfig("SelectedRow", Color.LightGray); }
		}

		/// <summary>
		/// Delete Message pomocí XML
		/// </summary>
		internal static bool DeleteMessageViaXML
		{			
			get { try { return WebConfigurationManager.AppSettings["DeleteMessageViaXML"] == "1"; } catch { return false; } }
		}

		private static Color getColorFromWebConfig(string colorName, Color defaultColor)
		{
			try
			{
				string colorsFromConfig = WebConfigurationManager.AppSettings["Colors"];
				string[] colorList = colorsFromConfig.Split(';');

				var color = from b in colorList
							where b.Contains(colorName)
							select b;

				string rColor = (color.ToList())[0].Split('=')[1].Trim();

				return ColorTranslator.FromHtml(rColor);
			}
			catch (Exception)
			{
				return defaultColor;
			}
		}
		
		/// <summary>
		/// Nastavení vlastností Labelu
		/// </summary>
		/// <param name="label"></param>
		/// <param name="text"></param>
		/// <param name="foreColor"></param>
		internal static void SetLabel(ref Label label, string text, Color foreColor)
		{
			if (label != null)
			{
				label.Text = text;
				label.ForeColor = foreColor;
			}
		}

		/// <summary>
		/// Zpracování vyjímky
		/// (odeslání chybového emailu, zápis chyby do tabulky logu aplikace)
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="methodName"></param>
		internal static void ProcessException(Exception exception, string methodName)
		{
			ProcessException(exception, methodName, String.Empty);
		}

		/// <summary>
		/// Zpracování vyjímky
		/// (odeslání chybového emailu, zápis chyby do tabulky logu aplikace)
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="methodName"></param>
		/// <param name="additionalInfo"></param>
		internal static void ProcessException(Exception exception, string methodName, string additionalInfo)
		{	
			SendErrEmail(exception, methodName, additionalInfo);
			AppLogWrite(ApplicationLog.LogCategoryError, exception.Message, MailErrorTo, String.Empty, User.LogonUserId, methodName);
		}
								
		/// <summary>
		/// Odešle chybový email
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="methodName"></param>		
		internal static void SendErrEmail(Exception exception, string methodName)
		{
			SendErrEmail(exception, methodName, String.Empty);
		}
	
		/// <summary>
		/// Odešle chybový email
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="methodName"></param>
		/// <param name="additionalInfo"></param>
		internal static void SendErrEmail(Exception exception, string methodName, string additionalInfo)
		{
			const bool IS_HTML = false;

			string emailSubject = String.Format("{0} ERROR", AppTitle);

			string emailBody = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].Substring(1) + "/" + methodName + Environment.NewLine;
			emailBody += "Uživatel: " + HttpContext.Current.Request.ServerVariables["LOGON_USER"] + Environment.NewLine;
			emailBody += exception.Message + (additionalInfo.IsNotNullOrEmpty() ? Environment.NewLine + additionalInfo : String.Empty);

			SendMail(emailSubject, emailBody, IS_HTML, MailErrorTo, "", "");
		}

		/// <summary>
		/// Zápis do tabulky logu aplikace
		/// </summary>
		/// <param name="type">typ logu</param>
		/// <param name="message">zpráva</param>
		/// <param name="xmlDeclaration">nevyužito</param>
		/// <param name="xmlMessage">nevyužito</param>
		/// <param name="userId">zicyz ID</param>
		/// <param name="source">jméno procedury</param>
		/// <returns></returns>
		internal static bool AppLogWrite(string type, string message, string xmlDeclaration, string xmlMessage, int userId, string source)
		{
			bool result = false;

			SqlConnection sqlConnection = new SqlConnection(FENIXWrtConnectionString);
			SqlCommand sqlCommand = new SqlCommand
			{
				CommandType = CommandType.StoredProcedure,
				CommandText = "[dbo].[prAppLogWriteNew]",
				Connection = sqlConnection
			};
			
			sqlCommand.Parameters.Add("@Type", SqlDbType.NVarChar, 20).Value = type;
			sqlCommand.Parameters.Add("@Message", SqlDbType.NVarChar, Int32.MaxValue).Value = message;
			sqlCommand.Parameters.Add("@XmlDeclaration", SqlDbType.NVarChar, 200).Value = xmlDeclaration;
			sqlCommand.Parameters.Add("@XmlMessage", SqlDbType.Xml).Value = xmlMessage;
			sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
			sqlCommand.Parameters.Add("@Source", SqlDbType.NVarChar, 200).Value = source;
			sqlCommand.Parameters.Add("@ReturnValue", SqlDbType.Int);
			sqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
			sqlCommand.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
			sqlCommand.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;
			sqlCommand.CommandTimeout = SQL_COMMAND_TIMEOUT;

			try
			{
				sqlConnection.Open();				
				sqlCommand.ExecuteNonQuery();
				if (sqlCommand.Parameters["@ReturnValue"].Value.ToString() == "0")
				{
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			finally
			{
				if (sqlConnection.State == ConnectionState.Open)
				{
					sqlConnection.Close();
				}
				sqlConnection = null;
				sqlCommand = null;
			}

			return result;
		}

		internal static DataTable GetDataTable(string proSelect, string connectionString)
		{
			DataTable myDataTable = new DataTable();

			SqlConnection sqlConnection = new SqlConnection {ConnectionString = connectionString};
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(proSelect, sqlConnection);

			try
			{
				sqlDataAdapter.Fill(myDataTable);
			}
			catch (Exception)
			{
				myDataTable = null;

			}
			finally
			{
				if (sqlConnection.State == ConnectionState.Open)
					sqlConnection.Close();
			}

			return myDataTable;
		}

		internal static DataTable GetDataTable(string proSelect)
		{
			DataTable myDataTable = new DataTable();

			SqlConnection sqlConnection = new SqlConnection {ConnectionString = FENIXRdrConnectionString};
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(proSelect, sqlConnection);

			try
			{
				sqlDataAdapter.Fill(myDataTable);
			}
			catch (Exception)
			{
				myDataTable = null;
			}
			finally
			{
				if (sqlConnection.State == ConnectionState.Open)
					sqlConnection.Close();
			}

			return myDataTable;
		}

		internal static DataTable CreateDataTable(string proSelect)
		{
			DataTable dataTable = new DataTable();
			SqlConnection conn = new SqlConnection(FENIXRdrConnectionString);
			SqlCommand sqlComm = new SqlCommand(proSelect, conn)
			{
				CommandType = CommandType.Text,
				CommandTimeout = SQL_COMMAND_TIMEOUT
			};

			try
			{
				conn.Open();
				SqlDataAdapter da = new SqlDataAdapter(sqlComm);
				da.Fill(dataTable);
			}
			catch (Exception ex)
			{
				dataTable = null;
				ProcessException(ex, ApplicationLog.GetMethodName());
			}			
			finally
			{
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
				}
				conn = null;
				sqlComm = null;
			}
			
			return dataTable;
		}

		/// <summary>
		/// Uvolní zdroj dat objektu
		/// </summary>
		/// <typeparam name="T">typ objektu, ze kterého se uvolňuje zdroj dat</typeparam>
		/// <param name="list">pole objektů, ze kterých se uvolňuje zdroj dat</param>
		internal static void UnbindDataFromObject<T>(params T[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				var obj = (object)list[i] as BaseDataBoundControl;
				if (obj != null)
				{
					obj.DataSource = null;
					obj.DataBind();
				}
			}
		}

		#region Date
		
		/// <summary>
		/// Test, zda zadané datum je validní na formát d.M.yyyy/dd.MM.yyyy
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		internal static bool DateIsValid(string date)
		{
			DateTime temp;
			return dateTimeTryParseExact(date, out temp);
		}

		/// <summary>
		/// Konverze datumoveho stringu z formatu ddMMyyyy na yyyyMMdd
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		internal static string DateDMYtoYMD(string date)
		{
			string ymdDate = String.Empty;

			DateTime temp;
			if (dateTimeTryParseExact(date, out temp))
			{
				ymdDate = temp.ToString("yyyyMMdd");
			}

			return ymdDate;
		}

		/// <summary>
		/// Konverze datumoveho stringu na DateTime
		/// </summary>
		/// <param name="date">datumovy string, ktery chceme konvertovat (očekává se formát d.M.yyy)</param>
		/// <param name="temp">konvertované datum</param>
		/// <returns>true .. konverze uspesna, false .. konverze neuspesna</returns>
		private static bool dateTimeTryParseExact(string date, out DateTime temp)
		{
			return DateTime.TryParseExact(date, "d.M.yyyy", new CultureInfo("cs-CZ"), DateTimeStyles.None, out temp);
		} 

		#endregion

		#region Excel

		/// <summary>
		/// Naplní buňku/y datumem[a časem] dle formátu
		/// (zarovnání vlevo, font - no bold)
		/// </summary>
		/// <param name="excelRange"></param>
		/// <param name="dateTimeValue"></param>
		/// <param name="format"></param>
		public static void ExcelFillCellWithDateTime(ExcelRange excelRange, object dateTimeValue, string format)
		{
			ExcelFillCellWithDateTime(excelRange, dateTimeValue, format, ExcelHorizontalAlignment.Left, false);
		}

		/// <summary>
		/// Naplní buňku/y datumem[a časem] dle formátu a zarovná je
		/// (font - no bold)
		/// </summary>
		/// <param name="excelRange"></param>
		/// <param name="dateTimeValue"></param>
		/// <param name="format"></param>
		/// <param name="horizontalAligment"></param>
		public static void ExcelFillCellWithDateTime(ExcelRange excelRange, object dateTimeValue, string format, ExcelHorizontalAlignment horizontalAligment)
		{
			ExcelFillCellWithDateTime(excelRange, dateTimeValue, format, horizontalAligment, false);
		}

		/// <summary>
		/// Naplní buňku/y datumem[a časem] dle formátu a nastaví 'tučnost' fontu
		/// (zarovnání vlevo)
		/// </summary>
		/// <param name="excelRange"></param>
		/// <param name="dateTimeValue"></param>
		/// <param name="format"></param>
		/// <param name="bold"></param>
		public static void ExcelFillCellWithDateTime(ExcelRange excelRange, object dateTimeValue, string format, bool bold)
		{
			ExcelFillCellWithDateTime(excelRange, dateTimeValue, format, ExcelHorizontalAlignment.Left, bold);
		}

		/// <summary>
		/// Naplní buňku/y datumem[a časem] dle formátu, zarovná je a nastaví 'tučnost' fontu
		/// </summary>
		/// <param name="excelRange"></param>
		/// <param name="dateTimeValue"></param>
		/// <param name="format"></param>
		/// <param name="horizontalAligment"></param>
		/// <param name="bold"></param>
		public static void ExcelFillCellWithDateTime(ExcelRange excelRange, object dateTimeValue, string format, ExcelHorizontalAlignment horizontalAligment, bool bold)
		{
			excelRange.Value = dateTimeValue;
			excelRange.Style.Numberformat.Format = format;
			excelRange.Style.HorizontalAlignment = horizontalAligment;
			excelRange.Style.Font.Bold = bold;
		}
		
		/// <summary>
		/// Vrací sériové číslo v exportech do Excelu
		/// </summary>
		/// <param name="serialNumber">sériové číslo</param>
		/// <returns>pokud je parametr vyplněn, vrací jeho obsah, jinak null</returns>
		internal static string ExcelPrepareSerialNumber(object serialNumber)
		{
			if (serialNumber == null) 
			{
				return null;
			}

			if (serialNumber.ToString().Trim() != String.Empty)
			{
				return serialNumber.ToString().Trim();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Podle hodnoty reconcilace {0,1,2,3} nastaví do buňky popis reconcilace a naformátuje ji
		/// </summary>
		/// <param name="excelRange"></param>
		/// <param name="reconciliation"></param>
		internal static void ExcelProcessReconciliation(ExcelRange excelRange, string reconciliation)
		{
			switch (reconciliation)
			{				
				case "3":
				case "2":
				case "0":
					excelRange.Style.Fill.PatternType = ExcelFillStyle.LightUp;
					excelRange.Style.Fill.BackgroundColor.SetColor(Color.Red);
					excelRange.Value = ExcelGetReconciliationDescription(reconciliation);
					break;
				case "1":
					// vyjímka - buňka je bez formátování
					excelRange.Value = ExcelGetReconciliationDescription(reconciliation);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Podle hodnoty reconcilace {0,1,2,3} vrací popis reconcilace
		/// </summary>
		/// <param name="reconciliation"></param>
		/// <returns></returns>
		internal static string ExcelGetReconciliationDescription(string reconciliation)
		{
			string description = string.Empty;

			switch (reconciliation)
			{
				case "3":
					description = "D0 odeslána";
					break;
				case "2":
					description = "Zamítnuto";
					break;
				case "1":
					description = "Schváleno";
					break;
				case "0":
					description = "Bez vyjádření";
					break;
				default:
					break;
			}

			return description;
		}


		#endregion

		/// <summary>
		/// Ze vstupního řetězce vytvoří hexa řetězec představující SHA256 hash
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		internal static string CreateSHA256Hash(string input)
		{
			byte[] data;

			using (SHA256Managed sha256Hash = new SHA256Managed())
			{
				data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			}

			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}

		#region GridView
		
		/// <summary>
		/// Nastavení barvy pozadí řádků
		/// </summary>
		/// <param name="gridView">GridView, kde nastavujeme barvy</param>
		public static void SetGridViewRowsBackcolor(GridView gridView)
		{
			int i = 1;
			foreach (GridViewRow gridViewRow in gridView.Rows)
			{
				gridViewRow.BackColor = (i % 2 == 0) ? gridViewRow.BackColor = EvenRowColor : gridViewRow.BackColor = OddRowColor;
				i++;
			}
		}

		/// <summary>
		/// Nastavení barvy pozadí vybraného řádku
		/// </summary>
		/// <param name="drv"></param>
		public static void SetSelectedRowBackColor(GridViewRow drv)
		{
			drv.BackColor = SelectedRowColor;
		}
		
		#endregion

		#region SendMail

		/// <summary>Odeslání mailu.</summary>
		/// <param name="mailSubject">Předmět mailu.</param>
		/// <param name="mailBody">Tělo mailu.</param>
		/// <param name="isBodyHtml">Příznak, jseslti je tělo formátu html.</param>
		/// <param name="mailTo">Adresy příjemců oddělené středníkem. Povinná je alespoň jeda adresa.</param>
		/// <param name="mailCC">Adresy příjemců kopie oddělené středníkem.</param>
		/// <param name="mailBcc">Adresy příjemců slepé kopie oddělené středníkem.</param>
		public static void SendMail(string mailSubject, string mailBody, bool isBodyHtml, string mailTo, string mailCC, string mailBcc)
		{
			try
			{
				char[] delims = { ';', ',' };

				// Adresy příjemců
				MailAddressCollection addrsTo = new MailAddressCollection();
				if (String.IsNullOrWhiteSpace(mailTo) == false)
				{
					string[] addrs = mailTo.Split(delims);
					for (int i = 0; i < addrs.Length; i++)
					{
						addrsTo.Add(new MailAddress(addrs[i]));
					}
				}

				// Adresy příjemců kopie
				MailAddressCollection addrsCC = new MailAddressCollection();
				if (String.IsNullOrWhiteSpace(mailCC) == false)
				{
					string[] addrs = mailCC.Split(delims);
					for (int i = 0; i < addrs.Length; i++)
					{
						addrsCC.Add(new MailAddress(addrs[i]));
					}
				}

				// Adresy příjemců slepé kopie
				MailAddressCollection addrsBcc = new MailAddressCollection();
				if (String.IsNullOrWhiteSpace(mailBcc) == false)
				{
					string[] addrs = mailBcc.Split(delims);
					for (int i = 0; i < addrs.Length; i++)
					{
						addrsBcc.Add(new MailAddress(addrs[i]));
					}
				}

				SendMail(mailSubject, mailBody, isBodyHtml, addrsTo, addrsCC, addrsBcc);
			}
			catch { }
		}

		/// <summary>Odeslání mailu.</summary>
		/// <param name="mailSubject">Předmět mailu.</param>
		/// <param name="mailBody">Tělo mailu.</param>
		/// <param name="isBodyHtml">Příznak, jseslti je tělo formátu html.</param>
		/// <param name="mailsTo">Adresy příjemců. Povinná je alespoň jeda adresa.</param>
		/// <param name="mailsCC">Adresy příjemců kopie.</param>
		/// <param name="mailsBcc">Adresy příjemců slepé kopie.</param>
		public static void SendMail(string mailSubject, string mailBody, bool isBodyHtml, MailAddressCollection mailsTo, MailAddressCollection mailsCC, MailAddressCollection mailsBcc)
		{
			if (mailsTo == null || mailsTo.Count <= 0) return;
			
			try
			{
				string mailServer = WebConfigurationManager.AppSettings["MailServer"];
				if (String.IsNullOrWhiteSpace(mailServer) == false)
				{
					using (MailMessage mailMsg = new MailMessage(new MailAddress(WebConfigurationManager.AppSettings["MailFrom"]), mailsTo[0]))
					{
						for (int i = 1; i < mailsTo.Count; i++)
						{
							mailMsg.To.Add(mailsTo[i]);
						}

						// Kopie mailu
						if (mailsCC != null)
						{
							foreach (MailAddress mailAddressCC in mailsCC)
							{
								if (mailMsg.CC.IndexOf(mailAddressCC) < 0)
									mailMsg.CC.Add(mailAddressCC);
							}
						}

						// Slepé kopie mailu
						if (mailsBcc != null)
						{
							foreach (MailAddress mailAddressBcc in mailsBcc)
							{
								if (mailMsg.Bcc.IndexOf(mailAddressBcc) < 0)
									mailMsg.Bcc.Add(mailAddressBcc);
							}
						}

						mailMsg.IsBodyHtml = isBodyHtml;
						mailMsg.Subject = mailSubject;
						mailMsg.Body = mailBody;	
						mailMsg.Priority = MailPriority.Normal;

						SmtpClient smtp = new SmtpClient(mailServer);
						smtp.Send(mailMsg);
					}
				}
			}
			catch { }
		}
 
		#endregion		
	}
}