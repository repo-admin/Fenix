using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using FenixHelper;
using UPC.Extensions.Convert;

namespace Fenix.ApplicationHelpers.LotFile
{
	/// <summary>
	/// Vlastni kontrola LOT file
	/// sloupce odpovidajici 2 SN, 1 SN; pocet radku LOT file se musi rovnat zadanemu mnozstvi						
	/// </summary>
	public class LotFileChecking
	{
		public List<string> LotFileRows { get; set; }
		public string Error { get; set; }
		public DataTable ItemOrKit { get; set; }
		public string SerialNumbersCount { get; set; }
		public int ItemQuantity { get; set; }
		public int KitQuantity { get; set; }

		private int requiredColumnCount;

		public LotFileChecking(List<string> lotFileRows, DataTable itemOrKit, string serialNumbersCount)
		{
			this.LotFileRows = lotFileRows;
			this.ItemOrKit = itemOrKit;
			this.SerialNumbersCount = serialNumbersCount;
			this.requiredColumnCount = 19;
		}

		/// <summary>
		/// 2 SN, 1 SN
		/// pocet radku LOT file se musi rovnat zadanemu mnozstvi
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			if (this.checkFilling() == false)
			{
				return false;
			}

			if (this.checkRowCount() == false)
			{ 
				return false;
			}

			return true;
		}


		private bool checkFilling()
		{
			//LOT file s S1 (jedním SN)
			//1;;0001;;108400133301;;UAAP41922509;;;;;;;;;;;1500000094;RUW00391

			//LOT file s S2 (dvěma SN)
			//1;;1;;108400133301;;00651816655;;;;;;;;;;014165959181;1500000058;RKZ02122

			int[] checkColumns = null;
						
			if (this.SerialNumbersCount == "1SN")
				checkColumns = new int[] {1, 3, 5, 7, 18, 19};
			else if (this.SerialNumbersCount == "2SN")
				checkColumns = new int[] { 1, 3, 5, 7, 17, 18, 19 };
			else
			{
				this.Error = "Při kontrole LOT file je požadován neznámý počet SN.";
				return false;
			}

			foreach (string lotFileRow in this.LotFileRows)
			{
				char[] delims = { ',', ';' };
				string[] lotFileValues = lotFileRow.Split(delims);

				if (lotFileValues.Length == this.requiredColumnCount)
				{
					foreach (int checkColumn in checkColumns)
					{
						if (string.IsNullOrEmpty(lotFileValues[checkColumn - 1]))
						{
							this.Error = string.Format("LOT file nemá vyplněný sloupec číslo {1}.", checkColumn);
							return false;
						}
					}
				}
				else
				{
					this.Error = string.Format("LOT file neobsahuje požadovaný počet sloupců ({0}).", this.requiredColumnCount);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Pocet radku LOT file se musi rovnat zadanemu mnozstvi
		/// </summary>
		/// <returns></returns>
		private bool checkRowCount()
		{
			foreach (DataRow r in ItemOrKit.Rows)
			{
				if (r[0].ToString().ToUpper() == "TRUE")
				{
					int itemOrKitQuantity = ConvertExtensions.ToInt32(r[2].ToString(), 0) == 1 ? this.KitQuantity : this.ItemQuantity;
					if (itemOrKitQuantity != this.LotFileRows.Count)
					{
						this.Error = "Nesouhlasí požadované množství a počet řádků v LOT file.";
						return false;
					}
				}
				else
				{
					this.Error = "Nelze stanovit požadované množství.";
					return false;
				}
			}
			 
			return true;
		}
	}
}