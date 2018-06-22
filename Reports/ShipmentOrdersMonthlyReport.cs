using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UPC.Extensions.Convert;

namespace Fenix.Reports
{
	#region Skupiny zboží

	/*		  
		CPE		Zařízení
		CPV		CPV=CPE vrácené
		MKT		Marketing
		NW0		Materiál
		SPP		Náhradní díly
		SPR		Náhradní díly repasované
		SPV		SPV
		Kit     kity 
	*/

	#endregion
	
    /// <summary>
    /// 
    /// </summary>
	public class ShipmentOrdersMonthlyReport
	{
		#region Internal Classes

		internal class ExportData
		{
			internal DataTable DataTable;
			internal string ItemTypeCode;
			internal int RowsNum;
			internal long Sum;

			internal ExportData()
			{
				this.DataTable = null;
				this.ItemTypeCode = String.Empty;
				this.RowsNum = 0;
				this.Sum = 0L;
			}
		} 

		#endregion

		#region Properties

		private List<ExportData> dataForExport = new List<ExportData>();
		private string DateFrom;
		private string DateTo;

		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		private ShipmentOrdersMonthlyReport()
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="dateFrom"></param>
		/// <param name="dateTo"></param>
		public ShipmentOrdersMonthlyReport(string dateFrom, string dateTo)
		{			
			this.DateFrom = dateFrom;
			this.DateTo = dateTo;
		}

		/// <summary>
		/// Vytvoření reportu
		/// </summary>
		/// <returns></returns>
		public MemoryStream CreateReport()
		{
			this.prepare();
			this.readData();
			return this.createReport();			
		}
				
		/// <summary>
		/// Příprava dat - vytvoření seznamu typů zboží + Kity
		/// </summary>
		private void prepare()
		{
			string select = "SELECT [ID], [Code], [DescriptionCz] FROM [dbo].[cdlItemTypes] " +
							"WHERE [IsActive] = 1 AND [Code] <> '???' " +
							"ORDER BY [Code] ASC";			
			DataTable dataTable = BC.GetDataTable(select, BC.FENIXRdrConnectionString);

			foreach (DataRow dataRow in dataTable.Rows)
			{
				ExportData exportData = new ExportData();				
				exportData.ItemTypeCode = dataRow["Code"].ToString();
				this.dataForExport.Add(exportData);
			}
						
			ExportData kitExportData = new ExportData();			
			kitExportData.ItemTypeCode = "Kity";
			this.dataForExport.Add(kitExportData);
		}

		/// <summary>
		/// Načtení dat pro zjištěné typy zboží + Kity
		/// </summary>
		private void readData()
		{
			string select;

			for (int i = 0; i < this.dataForExport.Count - 1; i++)
			{
				select = createSql(this.dataForExport[i].ItemTypeCode);
				this.dataForExport[i].DataTable = BC.GetDataTable(select, BC.FENIXRdrConnectionString);
				this.dataForExport[i].RowsNum = this.dataForExport[i].DataTable.Rows.Count; 
			}
						
			select = createSqlForKit();
			this.dataForExport[this.dataForExport.Count - 1].DataTable = BC.GetDataTable(select, BC.FENIXRdrConnectionString);
			this.dataForExport[this.dataForExport.Count - 1].RowsNum = this.dataForExport[this.dataForExport.Count - 1].DataTable.Rows.Count;
		}
		
		/// <summary>
		/// Vytvoření selectu pro typ zboží, které není kit
		/// </summary>
		/// <param name="itemType"></param>
		/// <returns></returns>
		private string createSql(string itemType)
		{
			return String.Format("SELECT convert(varchar(2), day(i.[RealDateOfDelivery]))  " +
						"+ '.' + convert(varchar(2), month(i.[RealDateOfDelivery]))  " +
						"+ '.' + convert(varchar(4), year(i.[RealDateOfDelivery])) as N'Skutečné datum dodání' " +
				        ",c.[ShipmentOrderID] as N'Číslo objednávky' " +
				        ",i.[ItemOrKitID] as N'Item ID/Kit ID' " +
				        ",i.[ItemOrKitDescription] as N'Popis' " +
				        ",cast(i.[RealItemOrKitQuantity] as integer) as N'Skutečné dodané množství'  " +
				        ",items.ItemType " +
						",items.Packaging " +
			            "FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation] c " +
			            "INNER JOIN [dbo].[CommunicationMessagesShipmentOrdersConfirmationItems] i " +
				        "ON c.ID = i.CMSOId " +
			            "LEFT JOIN [dbo].[cdlItems] items " +
				        "ON i.[ItemOrKitID] = items.ID " +
			            "WHERE (i.[RealDateOfDelivery] >= '{1}' and i.[RealDateOfDelivery] <= '{2} 23:59:59.999')  " +
				        "and c.[IsActive] = 1 " +
				        "and c.Reconciliation = 1 " +
				        "and i.[ItemVerKit] = 0	" +
				        "and items.ItemType = '{0}' " +
			            "order by i.[RealDateOfDelivery] asc, c.[ShipmentOrderID] asc, i.[ItemOrKitID] asc ", 
						itemType, BC.DateDMYtoYMD(this.DateFrom), BC.DateDMYtoYMD(this.DateTo));
		}

		/// <summary>
		/// Vytvoření selectu pro kity
		/// </summary>
		/// <returns></returns>
		private string createSqlForKit()
		{
			return String.Format("SELECT convert(varchar(2), day(i.[RealDateOfDelivery]))  " +
								"+ '.' + convert(varchar(2), month(i.[RealDateOfDelivery]))  " +
								"+ '.' + convert(varchar(4), year(i.[RealDateOfDelivery])) as N'Skutečné datum dodání' " +
						",c.[ShipmentOrderID] as N'Číslo objednávky' " +
						",i.[ItemOrKitID] as N'Item ID/Kit ID' " +
						",i.[ItemOrKitDescription] as N'Popis' " +
						",cast(i.[RealItemOrKitQuantity] as integer) as N'Skutečné dodané množství'  " +
						",'Kit' as ItemType	 " +
						",kits.Packaging " +
						"FROM [dbo].[CommunicationMessagesShipmentOrdersConfirmation] c " +
						"INNER JOIN [dbo].[CommunicationMessagesShipmentOrdersConfirmationItems] i " +
						"ON c.ID = i.CMSOId " +
						"LEFT JOIN [dbo].[cdlKits] kits " +
						"ON i.[ItemOrKitID] = kits.ID " +
						"WHERE (i.[RealDateOfDelivery] >= '{0}' and i.[RealDateOfDelivery] <= '{1} 23:59:59.999')  " +
						"and c.[IsActive] = 1 " +
						"and c.Reconciliation = 1 " +
						"and i.[ItemVerKit] = 1			   " +
						"order by i.[RealDateOfDelivery] asc, c.[ShipmentOrderID] asc, i.[ItemOrKitID] asc ", 
						BC.DateDMYtoYMD(this.DateFrom), BC.DateDMYtoYMD(this.DateTo));
		}

		#region Export do Excelu
		
		/// <summary>
		/// Vlastní vytvoření reportu
		/// </summary>
		private MemoryStream createReport()
		{
			MemoryStream memStream = new MemoryStream();
			try
			{				
				using (ExcelPackage xls = new ExcelPackage(memStream))
				{
					this.createAndPrepareSummaryWorksheet(xls, "Sumář");

					for (int i = 0; i < this.dataForExport.Count; i++)
					{
						this.createAndFillWorksheet(xls, this.dataForExport[i]);
					}

					this.fillSummaryWorkSheet(xls);
										
					xls.Workbook.Properties.Title = "Report - objednávky závozu";
					xls.Workbook.Properties.Subject = "Report - objednávky závozu";
					xls.Workbook.Properties.Keywords = "Office Open XML";
					xls.Workbook.Properties.Category = "Report - objednávky závozu";
					xls.Workbook.Properties.Comments = "";
					xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";										
					xls.Save();

					memStream.Flush();
					memStream.Seek(0, SeekOrigin.Begin);
				}
			}
			catch (Exception)
			{
				memStream.Close();
				throw;
			}

			return memStream;
		}

		/// <summary>
		/// Vytvoří list Sumář a naformátuje ho
		/// </summary>
		/// <param name="package"></param>
		/// <param name="workSheetName"></param>
		private void createAndPrepareSummaryWorksheet(ExcelPackage package, string workSheetName)
		{
			ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(workSheetName);
			worksheet.Cells["A1:C1000"].Style.Numberformat.Format = @"@";      
		}

		/// <summary>
		/// Vyvtoří, vyplní list pro jednotlivé typy zboží + Kity a naformátuje ho
		/// </summary>
		/// <param name="package"></param>
		/// <param name="exportData"></param>
		private void createAndFillWorksheet(ExcelPackage package, ExportData exportData)
		{
			bool canCreateColPackage = this.canCreateColumnPackage(exportData);
			
			ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(exportData.ItemTypeCode);			
			if (exportData.RowsNum > 0)
			{
				int row = 2;
				this.createWorksheetHeader(canCreateColPackage, worksheet, row);
								
				foreach (DataRow dataRow in exportData.DataTable.Rows)
				{
					row++;
					this.createWorksheetRow(exportData, canCreateColPackage, worksheet, row, dataRow);					
				}

				row += 2;
				this.createWorksheetBottom(exportData, worksheet, row);
				
				worksheet.Cells["A1:AZ"].AutoFitColumns();
			}
			else
			{
				worksheet.Cells[2, 2].Value = String.Format("Žádný záznam");
				worksheet.Cells[2, 2].Style.Font.Bold = true;
			}
		}

		/// <summary>
		/// Vytvoří hlavičku sheetu, včetně jejího formátování
		/// </summary>
		/// <param name="canCreateColPackage"></param>
		/// <param name="worksheet"></param>
		/// <param name="radek"></param>
		private void createWorksheetHeader(bool canCreateColPackage, ExcelWorksheet worksheet, int radek)
		{
			worksheet.Cells[radek, 1].Value = String.Format("Skutečné datum dodání");
			worksheet.Cells[radek, 2].Value = String.Format("Číslo objednávky");
			worksheet.Cells[radek, 3].Value = String.Format("Item ID/Kit ID");
			worksheet.Cells[radek, 4].Value = String.Format("Popis");
			worksheet.Cells[radek, 5].Value = String.Format("Skutečné dodané množství");

			if (canCreateColPackage)
			{
				worksheet.Cells[radek, 6].Value = String.Format("Balení");
				worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Cells[radek, 7].Value = String.Format("ItemType");
			}
			else
			{
				worksheet.Cells[radek, 6].Value = String.Format("ItemType");
			}

			worksheet.Cells["A2:G2"].Style.Font.Bold = true;
		}

		/// <summary>
		/// Vytvoří řádek sheetu, včetně jeho formátování
		/// </summary>
		/// <param name="exportData"></param>
		/// <param name="canCreateColPackage"></param>
		/// <param name="worksheet"></param>
		/// <param name="row"></param>
		/// <param name="dataRow"></param>
		private void createWorksheetRow(ExportData exportData, bool canCreateColPackage, ExcelWorksheet worksheet, int row, DataRow dataRow)
		{
			worksheet.Cells[row, 1].Value = dataRow["Skutečné datum dodání"].ToString();
			worksheet.Cells[row, 1].Style.Numberformat.Format = "dd.mm.yyyy";
			worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

			worksheet.Cells[row, 2].Value = dataRow["Číslo objednávky"].ToString();
			worksheet.Cells[row, 2].Style.Numberformat.Format = "############";
			worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

			worksheet.Cells[row, 3].Value = dataRow["Item ID/Kit ID"].ToString();
			worksheet.Cells[row, 3].Style.Numberformat.Format = "############";
			worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

			worksheet.Cells[row, 4].Value = dataRow["Popis"].ToString();
			worksheet.Cells[row, 4].Style.Numberformat.Format = @"@";

			int skutDodaneMnozstvi = Convert.ToInt32(dataRow["Skutečné dodané množství"].ToString());
			worksheet.Cells[row, 5].Value = skutDodaneMnozstvi;
			worksheet.Cells[row, 5].Style.Numberformat.Format = "# ### ##0";
			worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

			if (canCreateColPackage)
			{
				int packageCount = this.calculatePackageCount(dataRow);
				if (packageCount >= 0)
				{
					worksheet.Cells[row, 6].Value = packageCount;
					worksheet.Cells[row, 6].Style.Numberformat.Format = "# ### ##0";
					worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
				worksheet.Cells[row, 7].Value = dataRow["ItemType"].ToString();
				worksheet.Cells[row, 7].Style.Numberformat.Format = @"@";
			}
			else
			{
				worksheet.Cells[row, 6].Value = dataRow["ItemType"].ToString();
				worksheet.Cells[row, 6].Style.Numberformat.Format = @"@";
			}

			exportData.Sum += skutDodaneMnozstvi;
		}

		/// <summary>
		/// Vytvoří patičku sheetu, včetně jejího formátování
		/// </summary>
		/// <param name="exportData"></param>
		/// <param name="worksheet"></param>
		/// <param name="row"></param>
		private void createWorksheetBottom(ExportData exportData, ExcelWorksheet worksheet, int row)
		{
			worksheet.Cells[row, 5].Value = exportData.Sum;
			worksheet.Cells[row, 5].Style.Font.Bold = true;
			worksheet.Cells[row, 5].Style.Numberformat.Format = "# ### ### ###";
			worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
		}

		/// <summary>
		/// Vyplní list Sumář a naformátuje ho
		/// </summary>
		/// <param name="package"></param>
		private void fillSummaryWorkSheet(ExcelPackage package)
		{
			long globalSum = 0L;	
			ExcelWorksheet worksheet = package.Workbook.Worksheets["Sumář"];

			int radek = 2;			
			worksheet.Cells[radek, 2].Value = String.Format("Období");
			worksheet.Cells[radek, 3].Value = this.DateFrom;
			worksheet.Cells[radek, 4].Value = this.DateTo;
			worksheet.Cells["C2:D2"].Style.Font.Bold = true;

			radek++;
			radek++;
			foreach (ExportData exportData in this.dataForExport)
			{
				worksheet.Cells[radek, 2].Value = exportData.ItemTypeCode;
				worksheet.Cells[radek, 4].Value = exportData.Sum;
				worksheet.Cells[radek, 4].Style.Numberformat.Format = "# ### ### ##0";
								
				globalSum += exportData.Sum;
				radek++;
			}

			radek++;
			worksheet.Cells[radek, 2].Value = "Celkem";
			worksheet.Cells[radek, 2].Style.Font.Bold = true;
			worksheet.Cells[radek, 4].Value = globalSum;
			worksheet.Cells[radek, 4].Style.Numberformat.Format = "# ### ### ##0";			
			worksheet.Cells[radek, 4].Style.Font.Bold = true;

			worksheet.Cells["B1:BZ"].AutoFitColumns();
		}

		/// <summary>
		/// Zjištění, zda je možné vytvořit sloupec Balení		
		/// </summary>
		/// <param name="exportData"></param>
		/// <returns>true .. pro skupiny zboží, které nejsou NW0   false .. pro NW0</returns>
		private bool canCreateColumnPackage(ExportData exportData)
		{
			return (exportData.ItemTypeCode.ToUpper() != "NW0");
		}

		/// <summary>
		/// Výpočet počtu balení
		/// </summary>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private int calculatePackageCount(DataRow dataRow)
		{
			int packCount = 0;
			int realSuppliedQuantity = Convert.ToInt32(dataRow["Skutečné dodané množství"].ToString());
			int packaging = ConvertExtensions.ToInt32(dataRow["Packaging"].ToString(), 0);
			if (packaging != 0)
			{
				if (realSuppliedQuantity >= packaging)
				{
					packCount = (int)(realSuppliedQuantity / packaging);
				}
				else
				{
					packCount = 0;
				}
			}
			else
			{
				packCount = int.MinValue;
			}
			
			return packCount;
		}

		#endregion	
	}
}