using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// VR3 - Expedice do TRR (Returned Shipment)
	/// </summary>
	public partial class VrRpVratkyVR3 : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				CheckUserAcces("");
			}

			if (!Page.IsPostBack)
			{
				this.mvwMain.ActiveViewIndex = 0;
				BaseHelper.FillDdlDecision(ref this.ddlDecisionFlt, 0);
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

		private void fillPagerData(int pageNo)
		{
			this.pnlItems.Visible = false;  // 2015-02-26

			ViewState["Filter"] = "IsActive = 1";
			if (this.ddlDecisionFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue.ToString();
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwVR3Hd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = " [ID],[MessageId]," +
								   "[MessageTypeId],[MessageDescription],[Reconciliation]" +
								   ",ReconciliationAnoNe,[IsActive],[ModifyDate],[ModifyUserId],[DescriptionCz],CompanyName,ContactName";
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "ViewState[Filter] = " + ViewState["Filter"].ToString());
			}
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "SerNumView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				this.btnSnExcelOut(id);
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblErrInfo.Text = "";
			GridViewRow drv = this.grdData.SelectedRow;
			Session["IsRefresh"] = "0";
			this.pnlItems.Visible = true;
						
			string proS = "SELECT [ID],[ItemOrKitQualityCode],[IncotermDescription]  FROM [dbo].[CommunicationMessagesReturnedShipmentItems] WHERE ISACTIVE = 1 AND [CMSOId] = " + drv.Cells[1].Text;

			DataTable myDT = BC.GetDataTable(proS);
			this.gvConfirmationItems.DataSource = myDT.DefaultView;
			this.gvConfirmationItems.DataBind();

			if (HttpUtility.HtmlDecode(drv.Cells[5].Text).Trim() == "?")
			{
				this.rdblDecision.Enabled = true;
				this.btnDecision.Enabled = true;
			}
			else
			{
				this.rdblDecision.Enabled = false;
				this.btnDecision.Enabled = false;
			}
		}

		protected void search_button_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSnExcelOut(int id)
		{
			try
			{
				this.lblErrInfo.Text = "";
				bool mOK = true;
				string proS = string.Format(" SELECT H.[ID],H.[MessageId],H.[MessageTypeId],H.[MessageDescription],H.[Reconciliation],H.[ReconciliationAnoNe],H.[IsActive]" +
						 " ,H.[ModifyDate],H.[ModifyUserId],H.[DescriptionCz] " +
						 " ,RSI.[ItemOrKitQualityCode], RSI.[IncotermDescription], SN.SN1,SN.SN2 " +
						 " FROM [dbo].[vwVR3Hd] H " +
						 " INNER JOIN [dbo].[CommunicationMessagesReturnedShipmentItems] RSI " +
						 "	ON H.id = RSI.[CMSOId] " +
						 " INNER JOIN [dbo].[CommunicationMessagesReturnedShipmentItemsSerNum] SN " +
						 "	ON RSI.ID = SN.[ReturnedShipmentItemsID] " +
						 " WHERE H.ID = {0} ORDER BY 2", id);
				DataTable dt = new DataTable();
				dt = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dt == null || dt.Rows.Count < 1) { mOK = false; this.lblErrInfo.Text = "Žádné SN"; }
				if (mOK)
				{
					if (true)
					{
						MemoryStream ms = new MemoryStream();
						using (ExcelPackage xls = new ExcelPackage(ms))
						{
							ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");

							try
							{
								worksheet.Cells["A1:B20000"].Style.Numberformat.Format = @"@";
								int radek = 1;
								// nadpis
								worksheet.Row(1).Height = 24;
								worksheet.Cells[radek, 1, radek, 8].Merge = true;
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.Size = 14;
								worksheet.Cells[radek, 1].Value = String.Format("VR3 - VRATKA");
								worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
								worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
								worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
								radek = 2;
								worksheet.Cells[radek, 1].Value = "MessageId";
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 2].Value = "MessageDescription";
								worksheet.Cells[radek, 2].Style.Font.Bold = true;
								worksheet.Cells[radek, 2].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 3].Value = "Vyjádření";
								worksheet.Cells[radek, 3].Style.Font.Bold = true;
								worksheet.Cells[radek, 3].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 4].Value = "DescriptionCz";
								worksheet.Cells[radek, 4].Style.Font.Bold = true;
								worksheet.Cells[radek, 4].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 5].Value = "Kvalita";
								worksheet.Cells[radek, 5].Style.Font.Bold = true;
								worksheet.Cells[radek, 5].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 6].Value = "Incoterm";
								worksheet.Cells[radek, 6].Style.Font.Bold = true;
								worksheet.Cells[radek, 6].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 7].Value = "SN1";
								worksheet.Cells[radek, 7].Style.Font.Bold = true;
								worksheet.Cells[radek, 7].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 8].Value = "SN2";
								worksheet.Cells[radek, 8].Style.Font.Bold = true;
								worksheet.Cells[radek, 8].Style.Font.UnderLine = true;

								radek = 3;
								foreach (DataRow dr in dt.Rows)
								{
									worksheet.Cells[radek, 1].Value = dr["MessageId"].ToString();
									worksheet.Cells[radek, 2].Value = dr["MessageDescription"].ToString();
									worksheet.Cells[radek, 3].Value = dr["ReconciliationAnoNe"].ToString();
									worksheet.Cells[radek, 4].Value = dr["DescriptionCz"].ToString();
									worksheet.Cells[radek, 5].Value = dr["ItemOrKitQualityCode"].ToString();
									worksheet.Cells[radek, 6].Value = dr["IncotermDescription"].ToString();
									worksheet.Cells[radek, 7].Value = dr["SN1"].ToString();
									worksheet.Cells[radek, 8].Value = dr["SN2"].ToString();
									radek += 1;
								}  // foreach

								worksheet.Column(1).AutoFit();
								worksheet.Column(2).AutoFit();
								worksheet.Column(3).AutoFit();
								worksheet.Column(4).AutoFit();
								worksheet.Column(5).AutoFit();
								worksheet.Column(6).AutoFit();
								worksheet.Column(7).AutoFit();
								worksheet.Column(8).AutoFit();

								xls.Workbook.Properties.Title = "Sériová čísla VR3 Returned Shipment";
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
			}
			catch (Exception)
			{
				// TODO
				throw;
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.pnlItems.Visible = false;
			this.grdData.SelectedIndex = -1;			
			this.gvConfirmationItems.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnDecision_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				bool xmOK = false;
				Int16 iDecision;
				try
				{
					iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue);
					GridViewRow selectedRow = grdData.SelectedRow;					
					if (grdData.SelectedValue != null && selectedRow != null)
					{

						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.StoredProcedure;
						sqlComm.CommandText = "[dbo].[prCMRCconsentVR3]";
						sqlComm.Connection = conn;
						sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
						sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());												
						sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);		
						sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 11;											
						sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "ReturnedShipment";
						sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());
						sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
						sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
						sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 300);
						sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

						try
						{
							conn.Open();
							sqlComm.ExecuteNonQuery();
							if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
							{
								xmOK = true;
							}
						}
						catch (Exception ex)
						{
							BC.ProcessException(ex, ApplicationLog.GetMethodName(), "program prCMRCconsentVR3");
						}
						finally
						{							
							conn.Close();
							conn = null;
							sqlComm = null;
						}
					}
					else
					{  // upozornit
					}
				}
				catch (Exception)
				{
					iDecision = -1;
					// musíte vybrat
				}
				if (xmOK)
				{
					this.pnlItems.Visible = false;
					this.grdData.SelectedIndex = -1;
					this.gvConfirmationItems.SelectedIndex = -1;
					Session["IsRefresh"] = "1";
					this.fillPagerData(BC.PAGER_FIRST_PAGE);
				}
			}
		}
		
		protected void btnDecision_Click_OLD(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{				
				bool xmOK = true;

				SqlConnection con = new SqlConnection();
				con.ConnectionString = BC.FENIXWrtConnectionString;
				string proU = string.Empty;

				SqlCommand com = new SqlCommand();
				com.Connection = con;
				com.CommandType = CommandType.Text;

				try
				{
					Int16 iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue.ToString());
					proU = "UPDATE [dbo].[CommunicationMessagesReturnedShipment] SET [Reconciliation] = " + iDecision.ToString() + " WHERE ID = " + grdData.SelectedValue.ToString();
					com.CommandText = proU;

					con.Open();
					com.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
					xmOK = false;
				}
				finally
				{
					if (con.State == ConnectionState.Open)
					{ 
						con.Close(); 
					}
					con.Dispose();
					com.Dispose();
				}

				if (xmOK)
				{
					this.pnlItems.Visible = false;
					this.grdData.SelectedIndex = -1;					
					this.gvConfirmationItems.SelectedIndex = -1;
					Session["IsRefresh"] = "1";
					this.fillPagerData(BC.PAGER_FIRST_PAGE);
				}
			}
		}

		protected void btnSnExcel_Click(object sender, EventArgs e)
		{
			int id = WConvertStringToInt32(grdData.SelectedValue.ToString());
			btnSnExcelOut(id);
		}
	}
}