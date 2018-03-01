using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro KiShipment -> S0 - Objednávka expedice (Shipment Order)
	/// </summary>
	public class ShipmentHelper
	{
		/// <summary>
		/// Kontrola unikátnosti materiálu/kitu v nové S0
		/// </summary>
		/// <param name="gvKitsOrItemsNew"></param>
		/// <param name="checkBoxName"></param>
		/// <returns></returns>
		public static bool CheckAllRows(GridView gvKitsOrItemsNew, string checkBoxName)	
		{
			bool checkIsOK = true;

			List<string> idList = new List<string>();
			CheckBox myChkb;

			foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
			{
				myChkb = (CheckBox)gvr.FindControl(checkBoxName);
				if (myChkb.Checked)
				{
					idList.Add(gvr.Cells[1].Text);
				}
			}

			var grouped = idList
				.GroupBy(s => s)
				.Select(group => new { Word = group.Key, Count = group.Count() });

			foreach (var item in grouped)
			{
				if (item.Count > 1)
				{
					checkIsOK = false;
					break;
				}
			}
			
			return checkIsOK;
		}
		
		/// <summary>
		/// Kontrola unikátnosti(NW/KIT) přidávaného do nové S0
		/// (dané zboží se může vyskytovat pouze 1x)
		/// </summary>
		/// <param name="par1"></param>
		/// <returns></returns>
		public static bool GoodIsUnique(GridView gvKitsOrItemsNew, DropDownList ddlKits, DropDownList ddlNW, string checkBoxName, string par1)
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

					CheckBox myChkb;
					foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
					{
						myChkb = (CheckBox)gvr.FindControl(checkBoxName);
						int existingItemVerKit = ConvertExtensions.ToInt32(gvr.Cells[2].Text, -1);
						int existingItemOrKitID = ConvertExtensions.ToInt32(gvr.Cells[1].Text, -1);

						if (myChkb.Checked && (newItemVerKit == existingItemVerKit) && (newItemOrKitID == existingItemOrKitID))
						{
							isUnique = false;
							break;
						}
					}
				}
			}

			return isUnique;
		}

		/// <summary>
		/// Rozhodnutí, zda GridView má záznamy
		/// (řídící je objekt CheckBox - pokud je zatržený, s tímto záznamem chceme dál pracovat (např. uložit ho...)
		/// </summary>
		/// <param name="gvKitsOrItemsNew"></param>
		/// <param name="checkBoxName"></param>
		/// <returns></returns>
		public static bool GridViewHasRows(GridView gvKitsOrItemsNew, string checkBoxName)
		{
			bool hasRows = false;

			if (BaseHelper.GridViewHasRows(gvKitsOrItemsNew))
			{
				CheckBox myChkb;
				foreach (GridViewRow gvr in gvKitsOrItemsNew.Rows)
				{
					myChkb = (CheckBox)gvr.FindControl(checkBoxName);
					if (myChkb.Checked)
					{
						hasRows = true;
						break;
					}
				}
			}

			return hasRows;
		}

		/// <summary>
		/// Vrací poznámku pro zadané Shipment Order ID
		/// </summary>
		/// <param name="shipmentOrderID">ID, pro které se dohledá poznámka</param>
		/// <returns></returns>
		public static string GetRemarkForID(object shipmentOrderID)
		{
			string remark = String.Empty;

			string commText = String.Format("SELECT [Remark] FROM [dbo].[vwCMSOsent] WHERE [ID] = {0}", shipmentOrderID);
			using (SqlConnection conn = new SqlConnection(BC.FENIXRdrConnectionString))
			using (SqlCommand comm = new SqlCommand(commText, conn))
			{
				try
				{
					conn.Open();
					SqlDataReader rdr = comm.ExecuteReader();
					if (rdr.Read())
					{						
						if (rdr[0] != DBNull.Value)
						{							
							remark = (string)rdr[0];
						}						
					}
					rdr.Close();
					rdr.Dispose();
				}
				catch { }
			}

			return remark;
		}

		#region Filtering

		/// <summary>
		/// Naplní dropdown list seznamem názvů firem, které se vyskytují v objednávkách závozu
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCompanyName(ref DropDownList dropDownList)
		{
			string sql = "SELECT cValue, ctext FROM " +
							"(	SELECT '-1' cValue,' Vše' ctext " +
								"UNION ALL " +
								"SELECT cdlDestPlaces.[ID] cValue, cdlDestPlaces.[CompanyName] ctext " +
								"FROM [dbo].[cdlDestinationPlaces] cdlDestPlaces " +
								"WHERE cdlDestPlaces.[ID] in (select distinct(sos.[CustomerID]) FROM [dbo].[CommunicationMessagesShipmentOrdersSent] sos) " +
							") xx " +
							"ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem názvů statusů zpráv, které se vyskytují v objednávkách závozu
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlMessageStatus(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
							"( " +
								"SELECT '-1' cValue,' Vše' ctext " +
								"UNION ALL " +
								"SELECT cdlSt.ID cValue, [DescriptionCz]ctext FROM [dbo].[cdlStatuses] cdlSt " +
								"WHERE cdlSt.ID in (select distinct(MessageStatusId) FROM [dbo].[CommunicationMessagesShipmentOrdersSent]) " +
							") xx  " +
							"ORDER BY ctext ";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem jmen měst, která se vyskytují v objednávkách závozu
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCity(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
							"( " +
								"SELECT '-1' cValue,' Vše' ctext  " +
								"UNION ALL " +
								"SELECT ROW_NUMBER() OVER(ORDER BY tab.[CustomerCity]) as cValue, tab.[CustomerCity] cText  " +
								"FROM " +
								"( " +
									"SELECT DISTINCT(sos.[CustomerCity]) FROM [dbo].[CommunicationMessagesShipmentOrdersSent] sos " +
								") tab " +
							") xx  " +
							"ORDER BY ctext ";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		#endregion 
	}
}