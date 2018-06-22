using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Fenix
{
	/// <summary>
	/// VR2 - Vratky příslušenství (Returned Item)
	/// </summary>
	public partial class VrRpVratkyVR2 : BasePage
	{
		#region Constants

		/// <summary>
		/// Bez rozhodnutí
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		#endregion

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
			ViewState["Filter"] = "IsActive = 1";
			if (this.ddlDecisionFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [Reconciliation] = " + this.ddlDecisionFlt.SelectedValue.ToString();

			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;			
			pagerData.TableName = "[dbo].[vwVR2Hd]";
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = " [ID],[MessageId]," +
								   "[MessageTypeId],[MessageDescription],[MessageDateOfReceipt],[Reconciliation]" +
								   ",ReconciliationAnoNe,[IsActive],[ModifyDate],[ModifyUserId],[DescriptionCz]";
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
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "<br />ViewState[Filter] = " + ViewState["Filter"].ToString());
			}
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblErrInfo.Text = "";
			GridViewRow drv = this.grdData.SelectedRow;
			Session["IsRefresh"] = "0";
			this.pnlItems.Visible = true;
			BC.UnbindDataFromObject<GridView>(this.gvConfirmationItems);

			string proS = string.Empty;
			try
			{
				proS = string.Format("SELECT [ID],[CMSOId],[ItemId],[ItemDescription],[ItemQuantity],ItemQuantityInt,[ItemOrKitQualityId],[ItemOrKitQuality] " +
									 ",[ItemUnitOfMeasureId],[ItemUnitOfMeasure],[SN],[NDReceipt],[ReturnedFrom],[IsActive],[ModifyDate]     " +
									 ",[ModifyUserId],[RIID],[RIMessageId],[RIMessageTypeId],[RIMessageDescription],[RIMessageDateOfReceipt] " +
									 ",[RIReconciliation],[RIIsActive],[RIModifyDate],[RIModifyUserId]                                       " +
									 "  FROM [dbo].[vwVR2It] WHERE [IsActive] = {0} AND RIIsActive = {1} AND CMSOId = {2}", 1, 1, grdData.SelectedValue);

				DataTable myDataTable = BC.GetDataTable(proS);
				this.gvConfirmationItems.DataSource = myDataTable.DefaultView; 
				this.gvConfirmationItems.DataBind();
				this.gvConfirmationItems.SelectedIndex = -1;

				DataRow dr = myDataTable.Rows[0];
				if (dr["RIReconciliation"].ToString() == WITHOUT_DECISION)
				{
					this.btnDecision.Enabled = true; 
					this.rdblDecision.Enabled = true;
				}
				else 
				{
					this.btnDecision.Enabled = false; 
					this.rdblDecision.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proS = " + proS);
			}						
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "SerNumView")
			{
				int id = WConvertStringToInt32(e.CommandArgument.ToString());
				btnSnExcelOut(id);
			}
		} 

		protected void search_button_Click(object sender, ImageClickEventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			//this.pnlItems.Visible = false;
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
					iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue.ToString());
					GridViewRow selectedRow = grdData.SelectedRow;					//2015-11-05
					if (grdData.SelectedValue != null && selectedRow != null)
					{
						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.StoredProcedure;
						sqlComm.CommandText = "[dbo].[prCMRCconsentVR2]";
						sqlComm.Connection = conn;
						sqlComm.Parameters.Add("@Decision", SqlDbType.Int).Value = iDecision;
						sqlComm.Parameters.Add("@Id", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());
						
						sqlComm.Parameters.Add("@DeleteMessageId", SqlDbType.Int).Value = Convert.ToInt32(selectedRow.Cells[2].Text);			//2015-11-05						
						sqlComm.Parameters.Add("@DeleteMessageTypeId", SqlDbType.Int).Value = 10;												//2015-11-05
						sqlComm.Parameters.Add("@DeleteMessageTypeDescription", SqlDbType.NVarChar, 200).Value = "ReturnedItem";				//2015-11-05
						
						sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());

						sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
						sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
						sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
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
							BC.ProcessException(ex, ApplicationLog.GetMethodName(), "<br />ViewState[Filter] = " + ViewState["Filter"].ToString());
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

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.pnlItems.Visible = false;			
			this.grdData.SelectedIndex = -1;			
			this.gvConfirmationItems.SelectedIndex = -1;			
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void btnSnExcelOut(int id) 
		{
			this.lblErrInfo.Text = "";

			try
			{				
				bool mOK = true;

				string proS = string.Format("SELECT [SN],[VR2ItItemId],[VR2ItItemDescription],[VR2ItItemQuantityInt],[VR2ItItemOrKitQuality],[VR2ItItemUnitOfMeasure]" +
							   ",[VR2ItReturnedFrom],[VR2ItRIMessageId],[VR2ItRIMessageDescription],[VR2ItRIReconciliation]  FROM [dbo].[vwVR2SN] " +
							   " WHERE VR2ItCMSOId = {0} ORDER BY 2", id);
				DataTable dt = new DataTable();
				dt = BC.GetDataTable(proS, BC.FENIXRdrConnectionString);
				if (dt == null || dt.Rows.Count < 1) 
				{	
					this.lblErrInfo.Text = "Žádné SN";
					mOK = false; 
				}

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
								worksheet.Cells[radek, 1, radek, 7].Merge = true;
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.Size = 14;
								worksheet.Cells[radek, 1].Value = String.Format("VR2 - VRATKA");
								worksheet.Cells[radek, 1].Style.Fill.PatternType = ExcelFillStyle.LightUp;
								worksheet.Cells[radek, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
								worksheet.Cells[radek, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
								radek = 2;
								worksheet.Cells[radek, 1].Value = "Sériová čísla";
								worksheet.Cells[radek, 1].Style.Font.Bold = true;
								worksheet.Cells[radek, 1].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 2].Value = "ReturnedFrom";
								worksheet.Cells[radek, 2].Style.Font.Bold = true;
								worksheet.Cells[radek, 2].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 3].Value = "ItemId";
								worksheet.Cells[radek, 3].Style.Font.Bold = true;
								worksheet.Cells[radek, 3].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 4].Value = "Popis";
								worksheet.Cells[radek, 4].Style.Font.Bold = true;
								worksheet.Cells[radek, 4].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 5].Value = "Kvalita";
								worksheet.Cells[radek, 5].Style.Font.Bold = true;
								worksheet.Cells[radek, 5].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 6].Value = "MeJe";
								worksheet.Cells[radek, 6].Style.Font.Bold = true;
								worksheet.Cells[radek, 6].Style.Font.UnderLine = true;
								worksheet.Cells[radek, 7].Value = "MessageID";
								worksheet.Cells[radek, 7].Style.Font.Bold = true;
								worksheet.Cells[radek, 7].Style.Font.UnderLine = true;

								radek = 3;
								foreach (DataRow dr in dt.Rows)
								{
									worksheet.Cells[radek, 1].Value = dr["SN"].ToString();
									worksheet.Cells[radek, 2].Value = dr["VR2ItReturnedFrom"].ToString();
									worksheet.Cells[radek, 3].Value = dr["VR2ItItemId"].ToString();
									worksheet.Cells[radek, 4].Value = dr["VR2ItItemDescription"].ToString();
									worksheet.Cells[radek, 5].Value = dr["VR2ItItemOrKitQuality"].ToString();
									worksheet.Cells[radek, 6].Value = dr["VR2ItItemUnitOfMeasure"].ToString();
									worksheet.Cells[radek, 7].Value = dr["VR2ItRIMessageId"].ToString();
									radek += 1;
								}  

								worksheet.Column(1).AutoFit();
								worksheet.Column(2).AutoFit();
								worksheet.Column(3).AutoFit();
								worksheet.Column(4).AutoFit();
								worksheet.Column(5).AutoFit();
								worksheet.Column(6).AutoFit();
								worksheet.Column(7).AutoFit();

								xls.Workbook.Properties.Title = "Sériová čísla VR2";
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
		
		protected void btnSnExcel_Click(object sender, EventArgs e)
		{
			int id = WConvertStringToInt32(grdData.SelectedValue.ToString());
			btnSnExcelOut(id);
		}

		protected void gvConfirmationItems_SelectedIndexChanged(object sender, EventArgs e)
		{
		}
	}
}