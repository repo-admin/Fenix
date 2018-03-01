using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Fenix.ApplicationHelpers;
using FenixHelper;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// Objednávky kittingu
	/// </summary>
    public partial class KiKittingBrowse : BasePage
    {
		private const int HIDE_COLUMN = 12;
		
        protected void Page_Load(object sender, EventArgs e)
        {
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
			}

            if (!Page.IsPostBack)
            {
                this.mvwMain.ActiveViewIndex = 0;
				BaseHelper.FillDdlQualities(ref this.ddlKitQualitiesFlt);
				BaseHelper.FillDdlMessageStatuses(ref this.ddlMessageStatusFlt);
				this.fillPagerData(BC.PAGER_FIRST_PAGE);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.DesignMode) return;

            base.OnInit(e);
            this.grdPager.ShowNewItem = false;
            this.grdPager.PageSize = 10;
            this.grdPager.Command += new CommandEventHandler(this.pagerCommand);
        }

        private void pagerCommand(object sender, CommandEventArgs e)
        {
            int currPageIndx = Convert.ToInt32(e.CommandArgument);
            if (currPageIndx == -1)
            {
                // Novy zaznam
                //this.OnNewRecord(sender, e);
            }
            else
            {
                this.grdPager.CurrentIndex = currPageIndx;
                this.fillPagerData(currPageIndx);
            }
        }

        private void fillPagerData()
        {
            this.fillPagerData(this.grdPager.CurrentIndex);
        }

        private void fillPagerData(int pageNo)
        {            
            this.pnlItems.Visible = false;
			this.pnlCardStockItems.Visible = false;			
			BC.UnbindDataFromObject<GridView>(this.grdData, this.gvCardStockItems); 
			
			PagerData pagerData = new PagerData();			
			pagerData.PageNum = pageNo;
            pagerData.PageSize = this.grdPager.PageSize;            
            pagerData.TableName = "[dbo].[vwCMKSent]";
            pagerData.OrderBy = "ID DESC";
            pagerData.ColumnList = "[ID] ,[MessageId] ,[MessageTypeID]  ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId]  ,[KitDateOfDelivery] ,[CompanyName] ,[HeliosOrderID], StockId, Code, DescriptionCz,ModifyDate, Reconciliation";
            pagerData.WhereClause = "IsActive=1";
            
            try
            {	
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, HIDE_COLUMN);				
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

                ImageButton img = new ImageButton();
                foreach (GridViewRow gvr in this.grdData.Rows)
                {
                    if (gvr.Cells[HIDE_COLUMN].Text == "2")
                    {
                        img = (ImageButton)gvr.FindControl("btnK1new");
                        img.Enabled = true;
                        img.Visible = true;
                    }
                    else
                    {
                        img = (ImageButton)gvr.FindControl("btnK1new");
                        img.Enabled = false;
                        img.Visible = false;
                    }
                }
            }
            catch(Exception ex)
            {
				BC.ProcessException(ex, AppLog.GetMethodName());
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
        }

        protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pnlK1.Visible = false;
            this.pnlItems.Visible = true;
            this.pnlCardStockItems.Visible = false;
            string grdMainIdKey = grdData.SelectedDataKey.Value.ToString();
            this.gvItems.SelectedIndex = -1;
			BC.UnbindDataFromObject<GridView>(this.gvK1, this.gvCardStockItems); 

            string proSelect = String.Format("SELECT [ID] ,[KitId],[KitDescription],[KitQuantityInt],[KitQuantityDeliveredInt] ,[MeasuresID]  ,[KitUnitOfMeasure] ,[KitQualityId] ,[KitQualityCode]  ,[HeliosOrderID] ,[CardStockItemsId] FROM [dbo].[vwCMKSentItems] WHERE [IsActive] = 1 AND CMSOId={0}", grdMainIdKey);
            try
            {                
				DataTable myDataTable = BC.GetDataTable(proSelect, BC.FENIXRdrConnectionString);
                this.gvItems.DataSource = myDataTable.DefaultView;
                this.gvItems.DataBind();
            }
            catch
            {
            }
        }

        protected void gvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pnlCardStockItems.Visible = true;
            string grdMainIdKey = gvItems.SelectedDataKey.Value.ToString();
            string sl = gvItems.SelectedRow.Cells[2].Text;

            string proS = string.Format("SELECT ID,[cdlStocksName],[ItemVerKitDescription],[Code] ,[DescriptionCz],[ItemOrKitQuantityInt],[ItemOrKitFreeInt],[ItemOrKitUnConsilliationInt] ,[ItemOrKitReservedInt] ,[ItemOrKitReleasedForExpeditionInt] ,MeasuresCode,KitQualitiesCode" +
              " FROM [dbo].[vwCardStockItemsK] WHERE [IsActive] = {0} AND ItemOrKitId = {1}", 1, sl);
            
			DataTable myDataTable = BC.GetDataTable(proS);
            this.gvCardStockItems.DataSource = myDataTable.DefaultView; 
			this.gvCardStockItems.DataBind();
        }

        protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
			if (e.CommandName == "OrderView") 
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMKSentID"] = id.ToString();
				this.OrderView(id);
			}

			if (e.CommandName == "K1New")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				ViewState["vwCMKSentID"] = id.ToString();
				this.K1New(id);
			}
        }

        protected void OrderView(int id)
        {
            try
            {
                bool mOK = true;
                bool mOKR = true;

                string proS = string.Format("SELECT  [ID] ,[MessageId] ,[MessageTypeID] ,[MessageDescription] ,[MessageDateOfShipment] ,[MessageStatusId] ,[KitDateOfDelivery] ,[StockId] ,[IsActive]" +
                                            " ,[ModifyDate] ,[ModifyUserId] ,[CompanyName] ,[DescriptionCz] ,[Code] ,[HeliosOrderID]" +
                                            " FROM [dbo].[vwCMKSent] WHERE Id = {0} ORDER BY 1,2", id);
                DataTable dtObjHl = new DataTable();
				dtObjHl = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
                if (dtObjHl == null || dtObjHl.Rows.Count < 1) { mOK = false; }

                proS = string.Format("SELECT CI.[ID] ,CI.[CMSOId] ,CI.[HeliosOrderID] ,CI.[HeliosOrderRecordId] ,CI.[KitId] ,CI.[KitDescription] ,CI.[KitQuantityInt] ,CI.[KitQuantityDeliveredInt] "+
                                     ",CI.[MeasuresID] ,CI.[KitUnitOfMeasure]  ,CI.[KitQualityId] ,CI.[KitQualityCode] ,CI.[CardStockItemsId] ,CI.[IsActive] ,CI.[ModifyDate] ,CI.[ModifyUserId], K.[Code]"+
                                     " FROM [dbo].[vwCMKSentItems]  CI INNER JOIN cdlKits  K ON CI.[KitId]=K.Id WHERE CMSOId={0}", id);
                DataTable dtObjR = new DataTable();
				dtObjR = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
                if (dtObjR == null || dtObjR.Rows.Count < 1) { mOK = false; }

                proS = string.Format("SELECT [ID],[MessageId],[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[KitOrderID],[Reconciliation],[IsActive],[ModifyDate],[ModifyUserId]" +
                                     " FROM [dbo].[CommunicationMessagesKittingsConfirmation] where [KitOrderID]={0} AND Reconciliation<>0", id);
                DataTable dtObjHlCon = new DataTable();
				dtObjHlCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
                if (dtObjHlCon == null || dtObjHlCon.Rows.Count < 1) { mOKR = false; }

                DataTable dtObjHlRCon = new DataTable();
                if (mOKR)
                {
                    string ids = "-99";
                    foreach (DataRow dr in dtObjHlCon.Rows)
                    {
                        ids += "," + dr[0].ToString();
                    }

                    proS = string.Format("SELECT KI.[ID],KI.[CMSOId],KI.[KitID],KI.[KitDescription],KI.[KitQuantityInt],KI.[KitUnitOfMeasure],KI.[KitQualityId],KI.[KitSNs],KI.[IsActive]"+
                                         ",KI.[ModifyDate],KI.[ModifyUserId],K.Code" +
                                         " FROM [dbo].[vwKitConfirmationIt]  KI INNER JOIN cdlKits  K ON KI.[KitId]=K.Id where [CMSOId] in ({0})", ids);
					dtObjHlRCon = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
                    if (dtObjHlRCon == null || dtObjHlRCon.Rows.Count < 1) { mOKR = false; }
                }
                if (mOK)
                {

                    MemoryStream ms = new MemoryStream();
                    using (ExcelPackage xls = new ExcelPackage(ms))
                    {
                        ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");
                        worksheet.Cells["A1:H3"].Style.Numberformat.Format = @"@";
                        worksheet.Cells["A1:B20000"].Style.Numberformat.Format = @"@";

                        try
                        {
                            int radek = 1;
                            // nadpis
                            worksheet.Row(1).Height = 24;
                            worksheet.Cells[radek, 1, radek, 8].Merge = true;
                            worksheet.Cells[radek, 1].Style.Font.Bold = true;
                            worksheet.Cells[radek, 1].Style.Font.Size = 14;
                            worksheet.Cells[radek, 1].Value = String.Format("K0 - Objednávka");
                            worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
                            worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                            worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            radek += 2;
                            // hlavicka objednavky
                            worksheet.Cells[radek, 1].Value = String.Format("Message ID");
                            worksheet.Cells[radek, 2].Value = dtObjHl.Rows[0][1].ToString();  // MessageID
                            worksheet.Cells[radek, 2].Style.Font.Bold = true;
                            worksheet.Cells[radek, 3].Value = String.Format("Message Popis");
                            worksheet.Cells[radek, 4].Value = dtObjHl.Rows[0][4].ToString();  // MessageDescription
                            worksheet.Cells[radek, 4].Style.Font.Bold = true;
                            worksheet.Cells[radek, 5].Value = String.Format("Kompletační místo");
                            worksheet.Cells[radek, 6].Value = dtObjHl.Rows[0]["CompanyName"].ToString();  // ItemSupplierDescription
                            worksheet.Cells[radek, 6].Style.Font.Bold = true;
                            worksheet.Cells[radek, 7].Value = String.Format("Datum dodání");
                            worksheet.Cells[radek, 8].Value = dtObjHl.Rows[0]["KitDateOfDelivery"].ToString();    // MessageDateOfShipment	
                            worksheet.Cells[radek, 8].Style.Font.Bold = true;
                            worksheet.Cells[radek, 9].Value = String.Format("");
                            worksheet.Cells[radek, 10].Value = String.Format(""); 
                            worksheet.Cells[radek, 10].Style.Font.Bold = true;
                            radek += 1;

                            // detaily objednávky
                            worksheet.Cells[radek, 1].Value = String.Format("HeliosOrderRecordId");
                            worksheet.Cells[radek, 1].Style.Font.Bold = true;
                            worksheet.Cells[radek, 2].Value = String.Format("Id Kitu");
                            worksheet.Cells[radek, 2].Style.Font.Bold = true;
                            worksheet.Cells[radek, 3].Value = String.Format("Kód");
                            worksheet.Cells[radek, 3].Style.Font.Bold = true;
                            worksheet.Cells[radek, 4].Value = String.Format("Popis");
                            worksheet.Cells[radek, 4].Style.Font.Bold = true;
                            worksheet.Cells[radek, 5].Value = String.Format("Objed. množství");
                            worksheet.Cells[radek, 5].Style.Font.Bold = true;
                            worksheet.Cells[radek, 6].Value = String.Format("Dodané množství");
                            worksheet.Cells[radek, 6].Style.Font.Bold = true;
                            worksheet.Cells[radek, 7].Value = String.Format("MJ");
                            worksheet.Cells[radek, 7].Style.Font.Bold = true;
                            worksheet.Cells[radek, 8].Value = String.Format("Kvalita");
                            worksheet.Cells[radek, 8].Style.Font.Bold = true;
                            worksheet.Cells[radek, 9].Value = String.Format("");
                            worksheet.Cells[radek, 9].Style.Font.Bold = true;
                            worksheet.Cells[radek, 10].Value = String.Format("");
                            worksheet.Cells[radek, 10].Style.Font.Bold = true;
                            radek += 1;
                            foreach (DataRow dr in dtObjR.Rows)
                            {
                                worksheet.Cells[radek, 1].Value = dr["HeliosOrderRecordId"].ToString();
                                worksheet.Cells[radek, 2].Value = dr["KitId"].ToString();
                                worksheet.Cells[radek, 3].Value = dr["Code"].ToString();
                                worksheet.Cells[radek, 4].Value = dr["KitDescription"].ToString();
                                worksheet.Cells[radek, 5].Value = dr["KitQuantityInt"].ToString();
                                worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[radek, 6].Value = dr["KitQuantityDeliveredInt"].ToString();
                                worksheet.Cells[radek, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[radek, 7].Value = dr["KitUnitOfMeasure"].ToString();
                                worksheet.Cells[radek, 8].Value = dr["KitQualityCode"].ToString();
                                worksheet.Cells[radek, 9].Value = String.Format("");
                                worksheet.Cells[radek, 10].Value = String.Format("");
                                radek += 1;
                            }
                            if (mOKR)
                            {
                                radek += 2;
                                // hlavička konfirmace
                                worksheet.Cells[radek, 1, radek, 8].Merge = true;
                                worksheet.Cells[radek, 1].Style.Font.Bold = true;
                                worksheet.Cells[radek, 1].Style.Font.Size = 14;
                                worksheet.Cells[radek, 1].Value = String.Format("K1 - Confirmace objednávky");
                                worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
                                worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                                foreach (DataRow dr in dtObjHlCon.Rows)
                                {
                                    radek += 1;
                                    worksheet.Cells[radek, 1].Value = String.Format("Message Id");
                                    worksheet.Cells[radek, 1].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 2].Value = String.Format("Message popis");
                                    worksheet.Cells[radek, 2].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 3].Value = String.Format("");
                                    worksheet.Cells[radek, 3].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 4].Value = String.Format("Odsouhlasení");
                                    worksheet.Cells[radek, 4].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 5].Value = String.Format("Datum zapsání do Fenixu");
                                    worksheet.Cells[radek, 5].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 6].Value = String.Format("Aktivita");
                                    worksheet.Cells[radek, 6].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 7].Value = String.Format("");
                                    worksheet.Cells[radek, 7].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 8].Value = String.Format("");
                                    worksheet.Cells[radek, 8].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 9].Value = String.Format("");
                                    worksheet.Cells[radek, 9].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 10].Value = String.Format("");
                                    worksheet.Cells[radek, 10].Style.Font.Bold = true;
                                    radek += 1;
                                    worksheet.Cells[radek, 1].Value = dr["MessageId"].ToString();
                                    worksheet.Cells[radek, 2].Value = dr["MessageDescription"].ToString();
                                    worksheet.Cells[radek, 3].Value = String.Format("");

                                    if (dr["Reconciliation"].ToString() == "2")
                                    {
                                        worksheet.Cells[radek, 4].Style.Fill.PatternType = ExcelFillStyle.LightUp;
                                        worksheet.Cells[radek, 4].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                        worksheet.Cells[radek, 4].Value = String.Format("Zamítnuto");
                                    }
                                    else
                                        worksheet.Cells[radek, 4].Value = String.Format("Schváleno");
                                    worksheet.Cells[radek, 5].Value = dr["ModifyDate"].ToString();
                                    worksheet.Cells[radek, 6].Value = dr["IsActive"].ToString();
                                    worksheet.Cells[radek, 7].Value = String.Format("");
                                    worksheet.Cells[radek, 8].Value = String.Format("");
                                    worksheet.Cells[radek, 9].Value = String.Format("");
                                    worksheet.Cells[radek, 10].Value = String.Format("");
                                    radek += 2;

                                    worksheet.Cells[radek, 1].Value = String.Format("Kit Id");
                                    worksheet.Cells[radek, 1].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 2].Value = String.Format("Kód");
                                    worksheet.Cells[radek, 2].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 3].Value = String.Format("Popis");
                                    worksheet.Cells[radek, 3].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 4].Value = String.Format("Množství");
                                    worksheet.Cells[radek, 4].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 5].Value = String.Format("MJ");
                                    worksheet.Cells[radek, 5].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 6].Value = String.Format("");
                                    worksheet.Cells[radek, 6].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 7].Value = String.Format("");
                                    worksheet.Cells[radek, 7].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 8].Value = String.Format("");
                                    worksheet.Cells[radek, 8].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 9].Value = String.Format("");
                                    worksheet.Cells[radek, 9].Style.Font.Bold = true;
                                    worksheet.Cells[radek, 10].Value = String.Format("");
                                    worksheet.Cells[radek, 10].Style.Font.Bold = true;
                                    radek += 1;

                                    for (int ii = 0; ii <= dtObjHlRCon.Rows.Count - 1; ii++)
                                    {
                                        DataRow drc = dtObjHlRCon.Rows[ii];

                                        if (dr["ID"].ToString() == drc["CMSOId"].ToString())
                                        {
                                            worksheet.Cells[radek, 1].Value = drc["KitID"].ToString();
                                            worksheet.Cells[radek, 2].Value = drc["Code"].ToString();
                                            worksheet.Cells[radek, 3].Value = drc["KitDescription"].ToString();
                                            worksheet.Cells[radek, 4].Value = drc["KitQuantityInt"].ToString();
                                            worksheet.Cells[radek, 5].Value = drc["KitUnitOfMeasure"].ToString();
                                            worksheet.Cells[radek, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells[radek, 6].Value = String.Format("");
                                            worksheet.Cells[radek, 7].Value = String.Format("");
                                            worksheet.Cells[radek, 8].Value = String.Format("");
                                            worksheet.Cells[radek, 9].Value = String.Format("");
                                            worksheet.Cells[radek, 10].Value = String.Format("");
                                            radek += 1;
                                            if (!string.IsNullOrWhiteSpace(drc["KitSNs"].ToString()))
                                            {
                                                worksheet.Cells[radek, 1].Value = String.Format("Sériová čísla");
                                                worksheet.Cells[radek, 1].Style.Font.Bold = true;
                                                radek += 1;
                                                string[] sn = drc["KitSNs"].ToString().Split(';');
                                                string[] dvojice;
                                                foreach (var e in sn)
                                                {
                                                    dvojice = e.ToString().Split(',');
                                                    worksheet.Cells[radek, 1].Value = dvojice[0].IsNotNullOrEmpty() ? dvojice[0].Trim() : String.Empty;
                                                    worksheet.Cells[radek, 2].Value = dvojice[1].IsNotNullOrEmpty() ? dvojice[1].Trim() : String.Empty;
                                                    radek += 1;
                                                }
                                            }
                                        }
                                    }

                                }

                            }

                            worksheet.Column(1).AutoFit();
                            worksheet.Column(2).AutoFit();
                            worksheet.Column(3).AutoFit();
                            worksheet.Column(4).AutoFit();
                            worksheet.Column(5).AutoFit();
                            worksheet.Column(6).AutoFit();
                            worksheet.Column(7).AutoFit();
                            worksheet.Column(9).AutoFit();
                            worksheet.Column(10).AutoFit();

                            // lets set the header text 
                            worksheet.HeaderFooter.OddHeader.CenteredText = "Tinned Goods Sales";
                            // add the page number to the footer plus the total number of pages
                            worksheet.HeaderFooter.OddFooter.RightAlignedText =
                                string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                            // add the sheet name to the footer
                            worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                            // add the file path to the footer
                            worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                            //// change the sheet view to show it in page layout mode
                            //worksheet.View.PageLayoutView = true;

                            // set some core property values
                            xls.Workbook.Properties.Title = "KO objednávka";
                            xls.Workbook.Properties.Subject = "Sériová čísla";
                            xls.Workbook.Properties.Keywords = "Office Open XML";
                            xls.Workbook.Properties.Category = "Sériová čísla";
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

                        catch (Exception)
                        {
                            // TODO
                            throw;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        protected void K1New(int id)
        {
            this.pnlK1.Visible = true;
            string proS = String.Format("SELECT [ID],[CMSOId],[KitId],[KitDescription],[KitQuantity],[KitQuantityDelivered],[KitQuantityInt]  " +
                          " ,[KitQuantityDeliveredInt],[MeasuresID],[KitUnitOfMeasure],[KitQualityId],[KitQualityCode]           " +
                          " ,[HeliosOrderID],[HeliosOrderRecordId],[CardStockItemsId],[IsActive],[ModifyDate],[ModifyUserId]     " +
                          " FROM [dbo].[vwCMKSentItems] C WHERE C.[IsActive] = 1 AND C.CMSOId={0}", id);
            try
            {
                this.pnlItems.Visible = false; 
				BC.UnbindDataFromObject<GridView>(this.gvItems);
                
				DataTable myDataTable = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				this.gvK1.Columns[9].Visible=true;this.gvK1.Columns[10].Visible=true;
                this.gvK1.DataSource = myDataTable.DefaultView;
                this.gvK1.DataBind();
				this.gvK1.Columns[9].Visible=false;this.gvK1.Columns[10].Visible=false;
            }
            catch(Exception ex)
            {
				BC.ProcessException(ex, AppLog.GetMethodName(), "proS = " + proS);
            }
        }

        protected void btnK1Back_Click(object sender, EventArgs e)
        {
            this.pnlK1.Visible = false; 
			BC.UnbindDataFromObject<GridView>(this.gvK1);
        }

        protected void btnK1Save_Click(object sender, EventArgs e)
        {
            // kontrola hodnot
            bool mOK = true; string Err = string.Empty; int ii = 0; int iMaSmysl = 0;
            TextBox tbx = new TextBox();
            foreach (GridViewRow gvr in this.gvK1.Rows)
            {
                try
                {
                    tbx = (TextBox)gvr.FindControl("tbxQuantity");
                    if (!string.IsNullOrEmpty(tbx.Text.Trim()))
                    {
                        ii = Convert.ToInt32(tbx.Text.Trim());
                        iMaSmysl += ii;
                    }
                }
                catch (Exception)
                {
                    mOK = false;
                }
            }  // foreach (GridViewRow gvr in this.gvK1.Rows)

                        // zpracovani
			if (mOK && iMaSmysl > 0)
			{
				//mOK = false;
				DataTable dt = new DataTable();
				dt = BC.GetDataTable("SELECT min([MessageId]) as minMessageId FROM [dbo].[CommunicationMessagesKittingsConfirmation]", BC.FENIXRdrConnectionString);
				int iMessageID = WConvertStringToInt32(dt.Rows[0][0].ToString()) - 1;
				CultureInfo culture = new CultureInfo("cs-CZ");
				StringBuilder sb = new StringBuilder();

				dt = BC.GetDataTable(" SELECT [ID],[MessageId],[MessageTypeID],[MessageDescription],[MessageDateOfShipment],[MessageStatusId]" +
						 " ,[KitDateOfDelivery],[StockId],[IsActive],[ModifyDate],[ModifyUserId],[CompanyName],[DescriptionCz]   " +
						 " ,[Code],[HeliosOrderID],[Reconciliation]                                                              " +
						 "  FROM [dbo].[vwCMKSent] WHERE id=" + ViewState["vwCMKSentID"].ToString(), BC.FENIXRdrConnectionString);
				
				sb.Append("<NewDataSet>");
				sb.Append("<CommunicationMessagesKittingConfirmation>");
				sb.Append("<ID>-1</ID>");
				sb.Append("<MessageID>" + iMessageID.ToString() + "</MessageID>");
				sb.Append("<MessageTypeID>4</MessageTypeID>");
				sb.Append("<MessageTypeDescription>KittingConfirmation</MessageTypeDescription>");
				sb.Append("<MessageDateOfReceipt>" + DateTime.Today.ToString("yyyy-MM-dd") + "</MessageDateOfReceipt>");
				sb.Append("<KitOrderID>" + dt.Rows[0][0].ToString() + "</KitOrderID>");
				sb.Append("<HeliosOrderID>" + dt.Rows[0][14].ToString() + "</HeliosOrderID>");   //

				foreach (GridViewRow gvr in this.gvK1.Rows)
				{
					try
					{
						tbx = (TextBox)gvr.FindControl("tbxQuantity");
						if (!string.IsNullOrEmpty(tbx.Text.Trim()))
						{
							ii = Convert.ToInt32(tbx.Text.Trim());
							sb.Append("<KitID>" + gvr.Cells[1].Text + "</KitID>");
							sb.Append("<KitDescription>" + HttpUtility.HtmlDecode(gvr.Cells[2].Text) + "</KitDescription>");
							sb.Append("<KitQuantity>" + ii.ToString() + "</KitQuantity>");
							sb.Append("<KitUnitOfMeasureID>" + gvr.Cells[9].Text + "</KitUnitOfMeasureID>");
							sb.Append("<KitUnitOfMeasure>" + HttpUtility.HtmlDecode(gvr.Cells[8].Text) + "</KitUnitOfMeasure>");
							sb.Append("<KitQualityID>" + gvr.Cells[10].Text + "</KitQualityID>");
							sb.Append("<KitQuality>" + HttpUtility.HtmlDecode(gvr.Cells[6].Text) + "</KitQuality>");
							sb.Append("<NDReceipt></NDReceipt>");
						}
					}
					catch (Exception)
					{
						mOK = false;
					}
				}
				sb.Append("</CommunicationMessagesKittingConfirmation>");
				sb.Append("</NewDataSet>");

				string help = sb.ToString().Replace("{", "").Replace("}", "");
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(help);


				SqlConnection con = new SqlConnection();
				con.ConnectionString = BC.FENIXWrtConnectionString;
				SqlCommand com = new SqlCommand();
				com.CommandText = "prCMRCKins";
				com.CommandType = CommandType.StoredProcedure;
				com.Connection = con;
				com.Parameters.Add("@par1", SqlDbType.Xml).Value = doc.OuterXml;
				com.Parameters.Add("@ReturnValue", SqlDbType.Int);
				com.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				com.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2000);
				com.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					con.Open();
					com.ExecuteNonQuery();
					if (com.Parameters["@ReturnValue"].Value.ToString() != "0")
					{
						mOK = false; 
						Err = com.Parameters["@ReturnMessage"].Value.ToString();
					}


				}
				catch (Exception)
				{
					mOK = false;
				}
				finally {
					com = null;
				
				}
			}
			if (mOK) { 
			btnK1Back_Click(btnK1Back, EventArgs.Empty);
			}


				//////

        }
    }
}