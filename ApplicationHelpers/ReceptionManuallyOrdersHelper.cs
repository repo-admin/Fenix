using System.Web.UI.WebControls;

namespace Fenix.ApplicationHelpers
{
	public class ReceptionManuallyOrdersHelper
	{
		#region Filtering

		/// <summary>
		/// Naplní dropdown list seznamem názvů statusů zpráv, které se vyskytují v ... dle typu operace
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlMessageStatus(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM  " +
							"( " +
								"SELECT '-1' cValue,' Vše' ctext " +
								"UNION ALL " +
								"SELECT cdlSt.ID cValue, [DescriptionCz]ctext FROM [dbo].[cdlStatuses] cdlSt " +
								"WHERE cdlSt.ID in (select distinct(MessageStatusId) FROM [dbo].[vwCMRSent]) " +
							") xx  " +
							"ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		/// <summary>
		/// Naplní dropdown list seznamem dodavatelů (suppliers)
		/// 2015-05-18 zrusena podminka IsManually = 1 v klausuli WHERE
		/// </summary>
		/// <param name="dropDownList"></param>
		internal static void FillDdlCompanyName(ref DropDownList dropDownList)
		{
			string sql = "SELECT * FROM (SELECT '-1' cValue,' Vše' ctext UNION ALL " +
						 "SELECT DISTINCT [ItemSupplierId] cValue, [ItemSupplierDescription] ctext FROM [dbo].[vwCMRSent] WHERE IsActive=1) xx ORDER BY ctext";

			BasePage.FillDdl(ref dropDownList, sql);
		}

		#endregion
	}
}