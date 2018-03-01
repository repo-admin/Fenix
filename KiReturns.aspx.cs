using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using FenixHelper;
using OfficeOpenXml;

namespace Fenix
{
	/// <summary>
	/// VR1 - Vratky CPE  (Returned Equipment)
	/// </summary>
	public partial class KiReturns : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Logistika_ZiCyZ"] == null)
			{
				base.CheckUserAcces("");
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
			this.grdPager.Command += this.pagerCommand;
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
			ViewState["Filter"] = "[IsActive] = 1";
			if (this.ddlDecisionFlt.SelectedValue != "-1") ViewState["Filter"] += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue;
			
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;						
			pagerData.TableName = "[dbo].[vwReturnsItems]";
			pagerData.OrderBy = "[MessageId] DESC";
			pagerData.ColumnList = " [MessageId], [MessageTypeId], [MessageDescription], [MessageDateOfReceipt], [Reconciliation], [ReconciliationAnoNe], [IsActive], [ModifyDate], [ModifyUserId]";
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
				BC.ProcessException(ex, AppLog.GetMethodName(), "ViewState[Filter] = " + ViewState["Filter"]);
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridViewRow selectedRow = this.grdData.SelectedRow;
			this.getMessageID(selectedRow);
			this.setBtnDecision(selectedRow);
			this.pnlDecision.Visible = true;
		}
		
		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "SerNumView") 
			{
				int messageID = WConvertStringToInt32(e.CommandArgument.ToString());
				btnSnExcel(messageID);
			}
		}
		
		private void btnSnExcel(int messageID)
		{			
			try
			{
				string proS = string.Format("SELECT RI.[ID], RI.[CMSOId], RI.[ItemOrKitQualityId], RI.[ItemOrKitQuality] " +
											",RI.[SN1], RI.[SN2], RI.[NDReceipt], RI.[ReturnedFrom], RI.[IsActive] " +
											",RI.[ModifyDate], RI.[ModifyUserId], R.MessageDateOfReceipt " +
											",R.Reconciliation, R.IsActive, RI.[ItemID], RI.[ItemDescription] " +
											"FROM [dbo].[CommunicationMessagesReturnItems]  RI " +
											"INNER JOIN [dbo].[CommunicationMessagesReturn]  R " +
													"ON RI.CMSOId = R.ID  " +
											"WHERE CMSOId IN (SELECT ID FROM [dbo].[CommunicationMessagesReturn] WHERE MessageId = {0}) " +
											"ORDER BY RI.[ID], RI.[CMSOId]", messageID);
								
				DataTable dt = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dt != null && dt.Rows.Count > 0) 
				{
					MemoryStream ms = new MemoryStream();
					using (ExcelPackage xls = new ExcelPackage(ms))
					{
						ExcelWorksheet worksheet = xls.Workbook.Worksheets.Add("Data");

						try
						{
							worksheet.Cells["A1"].Value = "ItemID";
							worksheet.Cells["A1"].Style.Font.Bold = true;
							worksheet.Cells["A1"].Style.Font.UnderLine = true;
							worksheet.Cells["B1"].Value = "ItemDescription";
							worksheet.Cells["B1"].Style.Font.Bold = true;
							worksheet.Cells["B1"].Style.Font.UnderLine = true;
							worksheet.Cells["C1"].Value = "SN1";
							worksheet.Cells["C1"].Style.Font.Bold = true;
							worksheet.Cells["C1"].Style.Font.UnderLine = true;
							worksheet.Cells["D1"].Value = "SN2";
							worksheet.Cells["D1"].Style.Font.Bold = true;
							worksheet.Cells["D1"].Style.Font.UnderLine = true;																
							worksheet.Cells["E1"].Value = "Od koho přišlo";
							worksheet.Cells["E1"].Style.Font.Bold = true;
							worksheet.Cells["E1"].Style.Font.UnderLine = true;
							worksheet.Cells["F1"].Value = "Datum MSG";
							worksheet.Cells["F1"].Style.Font.Bold = true;
							worksheet.Cells["F1"].Style.Font.UnderLine = true;
								
							int radek = 3;
							foreach (DataRow r in dt.Rows) 
							{
								worksheet.Cells[radek, 1].Value = r["ItemID"].ToString();
								worksheet.Cells[radek, 2].Value = r["ItemDescription"].ToString();
								worksheet.Cells[radek, 3].Value = r["SN1"].ToString();
								worksheet.Cells[radek, 4].Value = r["SN2"].ToString();									
								worksheet.Cells[radek, 5].Value = r["ReturnedFrom"].ToString();
								worksheet.Cells[radek, 6].Value = r["MessageDateOfReceipt"].ToString();
								radek +=1 ;
							}

							worksheet.Cells["A1:E10000"].Style.Numberformat.Format = @"@";
							worksheet.Column(1).AutoFit();
							worksheet.Column(2).AutoFit();
							worksheet.Column(3).AutoFit();
							worksheet.Column(4).AutoFit();
							worksheet.Column(5).AutoFit();
							worksheet.Column(6).AutoFit();
														
							xls.Workbook.Properties.Title = "Sériová čísla VR1 - Returned Equipment";
							xls.Workbook.Properties.Subject = "Sériová čísla";
							xls.Workbook.Properties.Keywords = "Office Open XML";
							xls.Workbook.Properties.Category = "Sériová čísla";
							xls.Workbook.Properties.Comments = "";
							xls.Workbook.Properties.Company = "UPC Česká republika, s.r.o.";
														
							xls.Save();
							ms.Flush();
							ms.Seek(0, SeekOrigin.Begin);

							Response.Clear();
							Response.Buffer = true;
							Response.AddHeader("content-disposition", "attachment;filename=VR1_Seriova_cisla_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
							Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
							Response.Charset = "";
							EnableViewState = false;

							Response.BinaryWrite(ms.ToArray());
							ms.Close();
							//HttpContext.Current.ApplicationInstance.CompleteRequest();
							Response.End();
						}
						catch (Exception)
						{
							// TODO
							//throw;
						}
					}					
				}
			}
			catch (Exception)
			{
				// TODO
				//throw;
			}
		}
		
		protected void btnDecision_Click(object sender, EventArgs e)
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
					sqlComm.CommandText = "[dbo].[prCMRCconsentVR1]";
					sqlComm.Connection = conn;
					sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
					sqlComm.Parameters.Add("@ID", SqlDbType.Int).Value = -1; //Id v tabulce [dbo].[CommunicationMessagesReturn] neodpovídá Id z přijatého XML (XML message se rozpadá dle kvality)
					sqlComm.Parameters.Add("@MessageID", SqlDbType.Int).Value = Convert.ToInt32(ViewState["MessageID"].ToString());  
					sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 9;										
					sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "ReturnedEquipment";	
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
						BC.ProcessException(ex, AppLog.GetMethodName(), "program prCMRCconsentVR1");
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
				btnBack_Click(btnDecision, EventArgs.Empty);
			}
		}
		
		protected void btnDecision_Click_OLD(object sender, EventArgs e)
		{
			bool mOK = true;
			
			string proU = String.Format("UPDATE [dbo].[CommunicationMessagesReturn] SET [Reconciliation] = {0}, [ModifyUserId] = {1} WHERE [MessageID] = {2}",
										 this.rdblDecision.SelectedValue, WConvertStringToInt32(Session["Logistika_ZiCyZ"].ToString()), ViewState["MessageID"]);

			SqlConnection sqlConnection = new SqlConnection { ConnectionString = BC.FENIXWrtConnectionString };
			SqlCommand sqlCommand = new SqlCommand
			{
				CommandText = proU,
				CommandType = CommandType.Text,
				Connection = sqlConnection
			};

			try
			{
				sqlConnection.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				mOK = false;
				BC.ProcessException(ex, AppLog.GetMethodName(), "<br />proU =  " + proU);				
			}
			finally
			{
				if (sqlConnection.State == ConnectionState.Open) sqlConnection.Close(); 
				sqlConnection.Dispose();
			    sqlCommand.Dispose();
			}

			if (mOK)
			{
				btnBack_Click(btnDecision, EventArgs.Empty);
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.pnlDecision.Visible = false;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void search_button_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		private void getMessageID(GridViewRow selectedGridViewRow)
		{
			const int COLUMN_MESSAGE_ID = 1;
			this.ViewState["MessageID"] = selectedGridViewRow.Cells[COLUMN_MESSAGE_ID].Text;
		}
		
		private void setBtnDecision(GridViewRow selectedGridViewRow)
		{
			const int COLUMN_RECONCILIATION_TEXT = 4;
			string decisionText = HttpUtility.HtmlDecode(selectedGridViewRow.Cells[COLUMN_RECONCILIATION_TEXT].Text).ToUpper();
			this.btnDecision.Enabled = (decisionText != BC.SCHVALENO && decisionText != BC.ZAMITNUTO && decisionText != BC.D0_ODESLANA);
		}
	}
}