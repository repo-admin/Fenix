using System;
using System.Data;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	public class KartySkladuExcel : BasePage
	{
		public string ProSelect { get; set; }

		public void ExcelView()
		{
			DataTable d = new DataTable();
			d = BC.GetDataTable(ProSelect, BC.FENIXRdrConnectionString);
			MemoryStream ms = new MemoryStream();
			using (ExcelPackage xls = new ExcelPackage(ms))
			{
				ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
				worksheet.Cells["A1:P3"].Style.Numberformat.Format = @"@";      // ?
				try
				{
					int radek = 1;
					// nadpis
					worksheet.Row(1).Height = 24;
					worksheet.Cells[radek, 1, radek, 15].Merge = true;
					worksheet.Cells[radek, 1].Style.Font.Bold = true;
					worksheet.Cells[radek, 1].Style.Font.Size = 14;
					worksheet.Cells[radek, 1].Value = String.Format("Skladové karty");
					worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
					worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
					worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
					radek += 2;
					// hlavička
					worksheet.Cells[radek, 1].Value = String.Format("Id");
					worksheet.Cells[radek, 1].Style.Font.Bold = true;
					worksheet.Cells[radek, 2].Value = String.Format("Zboží");
					worksheet.Cells[radek, 2].Style.Font.Bold = true;
					worksheet.Cells[radek, 3].Value = String.Format("Kód");
					worksheet.Cells[radek, 3].Style.Font.Bold = true;
					worksheet.Cells[radek, 4].Value = String.Format("Popis");
					worksheet.Cells[radek, 4].Style.Font.Bold = true;
					worksheet.Cells[radek, 5].Value = String.Format("Kvalita");
					worksheet.Cells[radek, 5].Style.Font.Bold = true;
					worksheet.Cells[radek, 6].Value = String.Format("MJ");
					worksheet.Cells[radek, 6].Style.Font.Bold = true;
					worksheet.Cells[radek, 7].Value = String.Format("Mn.volné");
					worksheet.Cells[radek, 7].Style.Font.Bold = true;
					worksheet.Cells[radek, 8].Value = String.Format("Mn.ke schválení");
					worksheet.Cells[radek, 8].Style.Font.Bold = true;
					worksheet.Cells[radek, 9].Value = String.Format("Mn.rezervované");
					worksheet.Cells[radek, 9].Style.Font.Bold = true;
					worksheet.Cells[radek, 10].Value = String.Format("Mn.uvolněné");
					worksheet.Cells[radek, 10].Style.Font.Bold = true;
					worksheet.Cells[radek, 11].Value = String.Format("Mn.expedované");
					worksheet.Cells[radek, 11].Style.Font.Bold = true;
					worksheet.Cells[radek, 12].Value = String.Format("Typ");
					worksheet.Cells[radek, 12].Style.Font.Bold = true;
					worksheet.Cells[radek, 13].Value = String.Format("PC");
					worksheet.Cells[radek, 13].Style.Font.Bold = true;
					worksheet.Cells[radek, 14].Value = String.Format("Packaking");
					worksheet.Cells[radek, 14].Style.Font.Bold = true;
					worksheet.Cells[radek, 15].Value = String.Format("Aktivita");
					worksheet.Cells[radek, 15].Style.Font.Bold = true;
					//worksheet.Cells[radek, 16].Value = String.Format("");
					//worksheet.Cells[radek, 16].Style.Font.Bold = true;
					radek += 1;

					foreach (DataRow r in d.Rows)
					{
						worksheet.Cells[radek, 1].Value = r["Id"].ToString();
						//worksheet.Cells[radek, 1].Style.Font.Bold = true;
						worksheet.Cells[radek, 2].Value = r["ItemVerKitDescription"].ToString();
						//worksheet.Cells[radek, 2].Style.Font.Bold = true;
						worksheet.Cells[radek, 3].Value = r["ItemOrKitID"].ToString();
						//worksheet.Cells[radek, 3].Style.Font.Bold = true;
						worksheet.Cells[radek, 4].Value = r["DescriptionCz"].ToString();
						//worksheet.Cells[radek, 4].Style.Font.Bold = true;
						worksheet.Cells[radek, 5].Value = r["QualitiesCode"].ToString();
						//worksheet.Cells[radek, 5].Style.Font.Bold = true;
						worksheet.Cells[radek, 6].Value = r["MeasuresCode"].ToString();
						//worksheet.Cells[radek, 6].Style.Font.Bold = true;
						worksheet.Cells[radek, 7].Value = r["ItemOrKitFreeInteger"].ToString();
						worksheet.Cells[radek, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 8].Value = r["ItemOrKitUnConsilliationInteger"].ToString();
						worksheet.Cells[radek, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 9].Value = r["ItemOrKitReservedInteger"].ToString();
						worksheet.Cells[radek, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 10].Value = r["ItemOrKitReleasedForExpeditionInteger"].ToString();
						worksheet.Cells[radek, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 11].Value = r["ItemOrKitExpeditedInteger"].ToString();
						worksheet.Cells[radek, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 12].Value = r["ItemType"].ToString();
						//worksheet.Cells[radek, 12].Style.Font.Bold = true;
						worksheet.Cells[radek, 13].Value = r["PC"].ToString();
						//worksheet.Cells[radek, 13].Style.Font.Bold = true;
						worksheet.Cells[radek, 14].Value = r["Packaging"].ToString();
						worksheet.Cells[radek, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
						worksheet.Cells[radek, 15].Value = r["IsActive"].ToString();
						//worksheet.Cells[radek, 15].Style.Font.Bold = true;
						radek += 1;
					}

					worksheet.Column(1).AutoFit();
					worksheet.Column(2).AutoFit();
					worksheet.Column(3).AutoFit();
					worksheet.Column(4).AutoFit();
					worksheet.Column(5).AutoFit();
					worksheet.Column(6).AutoFit();
					worksheet.Column(7).AutoFit();
					worksheet.Column(8).AutoFit();
					worksheet.Column(9).AutoFit();
					worksheet.Column(10).AutoFit();
					worksheet.Column(11).AutoFit();
					worksheet.Column(12).AutoFit();
					worksheet.Column(13).AutoFit();
					worksheet.Column(14).AutoFit();
					worksheet.Column(15).AutoFit();

					// set some core property values
					xls.Workbook.Properties.Title = "Skladové karty";
					xls.Workbook.Properties.Subject = "Skladové karty";
					xls.Workbook.Properties.Keywords = "Office Open XML";
					xls.Workbook.Properties.Category = "Skladové karty";
					xls.Workbook.Properties.Comments = "";
					// set some extended property values
					xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";

					// save the new spreadsheet to the stream
					xls.Save();
					ms.Flush();
					ms.Seek(0, SeekOrigin.Begin);

					Response.Clear();
					Response.Buffer = true;
					Response.AddHeader("content-disposition", "attachment;filename=Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.Charset = "";
					EnableViewState = false;

					Response.BinaryWrite(ms.ToArray());
					ms.Close();
					Response.End();

				}
				catch (Exception x)
				{
					BC.ProcessException(x, ApplicationLog.GetMethodName());
				}
			}

			////

		}

	}
}