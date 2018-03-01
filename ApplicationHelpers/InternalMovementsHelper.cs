using System.Web.UI.WebControls;

namespace Fenix.ApplicationHelpers
{
	/// <summary>
	/// Pomocná třída pro MaInternalMovements -> Interní pohyby (Manka/Přebytky)
	/// </summary>
	public class InternalMovementsHelper
	{
		/// <summary>
		/// Naplní drop down list seznamem kitů (ze skladových karet)
		/// (1. položka je  VYBERTE)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlKits(ref DropDownList dropDownList)
		{
			string select = "SELECT *  " +
							"FROM " +
								"(  " +
									"SELECT '-1' cValue,' VYBERTE ' ctext  " +
									"UNION ALL  " +
									"SELECT vwc.ItemOrKitID cValue, LEFT([DescriptionCz]+' (' + QualitiesCode + ' ' + CAST(ItemOrKitReleasedForExpeditionInteger AS VARCHAR(50))+')',100) ctext  " +
									"FROM [dbo].[vwCardStockItems] vwc  " +
									"WHERE vwc.[IsActive] = 1 AND vwc.ItemVerKit = 1 AND vwc.cdlStocksID = 2  " +
								") tab  " +
							"ORDER BY ctext";
			
			BasePage.FillDdl(ref dropDownList, select);
		}

		/// <summary>
		/// Naplní drop down list seznamem itemů (ze skladových karet)
		/// (1. položka je  VYBERTE)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlItems(ref DropDownList dropDownList)
		{
			string select = "SELECT * " +
							"FROM " +
								"(  " +
									"SELECT '-1' cValue,' VYBERTE ' ctext   " +
									"UNION ALL    " +
									"SELECT ItemOrKitID cValue, LEFT([DescriptionCz]+' (' + QualitiesCode + ' ' + CAST([ItemOrKitFreeInteger] AS VARCHAR(50))+')',100) ctext   " +
									"FROM [dbo].[vwCardStockItems]   " +
									"WHERE [IsActive] = 1 AND ItemVerKit = 0 AND cdlStocksID = 2 AND (LTRIM(RTRIM(DescriptionCz)) <> '' AND DescriptionCz IS NOT NULL)  " +
								") TAB   " +
							"ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, select);
		}

		/// <summary>
		/// Naplní drop down list seznamem jakostí (kvalit)
		/// (zaostřená položka je  NEW - default kvalita)
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlQuality(ref DropDownList dropDownList)
		{
			BaseHelper.FillDdlQualities(ref dropDownList);

			//zaostří položku NEW (default kvalita)
			dropDownList.SelectedValue = "1";
		}

		/// <summary>
		/// Naplní drop down list seznamem 'od čeho se přičítá/odečítá' {Volné/uvolněné, Rezervované}
		/// (zaostřená položka je 'Volné/uvolněné')
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlAddSubBase(ref DropDownList dropDownList)
		{
			string select = "SELECT *    " +
							"FROM   " +
									"(    " +
										"SELECT '-1' cValue,' VYBERTE ' ctext    " +
										"UNION ALL    " +
										"SELECT IMB.[ID] cValue, IMB.[Description] ctext    " +
										"FROM [dbo].[InternalMovementsAddSubBase] IMB    " +
										"WHERE IMB.[IsActive] = 1  " +
									") tab    " +
							"ORDER BY cValue ";

			BasePage.FillDdl(ref dropDownList, select);

			//zaostří položku 'Volné/uvolněné' (default hodnota)
			dropDownList.SelectedValue = "1";
		}
	}
}