using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fenix.ApplicationHelpers;
using Fenix.Extensions;
using UPC.Extensions.Convert;

namespace Fenix
{
	/// <summary>
	/// [Správa] Interní pohyby (manka/přebytky)
	/// </summary>
	public partial class MaInternalMovements : BasePage
	{
		#region Properties

		/// <summary>
		/// Bez rozhodnutí (z pohledu schvalovatele z logistiky)
		/// <value> = "0"</value>
		/// </summary>
		private const string WITHOUT_DECISION = "0";

		/// <summary>
		/// Manko
		/// <value> = "1"</value>
		/// </summary>
		private const string MANKO = "1";

		/// <summary>
		/// Přebytek
		/// <value> = "2"</value>
		/// </summary>
		private const string PREBYTEK = "2";
		
		/// <summary>
		/// Číslo sloupce pro ItemVerKit {0 .. item    1 .. kit}
		/// </summary>		
		private const int COLUMN_ITEM_VER_KIT = 14;

		/// <summary>
		/// Číslo sloupce pro QualityID {1 .. NEW   2 .. TRR   3 ..REF	4 .. Faulty  5 .. SCR	6.. CLA}
		/// </summary>
		private const int COLUMN_QUALITY_ID = 15;

		/// <summary>
		/// Číslo sloupce pro MovementsDecisionID {0	Bez rozhodnutí	1	Schváleno	2	Zamítnuto}
		/// (z pohledu schvalovatele z logistiky - nejde o vztah k ND .. volba D0 odeslána je nesmyslná)
		/// </summary>		
		private const int COLUMN_MOVEMENTS_DECISION_ID = 16;
		
		/// <summary>
		/// Číslo sloupce pro MovementsTypeID {1	Manko	2	Přebytek}
		/// </summary>
		private const int COLUMN_MOVEMENTS_TYPE_ID = 17;
				
		/// <summary>
		/// Číslo sloupce pro MovementsAddSubBaseID {1    Volné/uvolněné     2   Rezervované}
		/// </summary>
		private const int COLUMN_MOVEMENTS_ADD_SUB_BASE_ID = 18;

		/// <summary>
		/// seznam sloupců, se kterými chceme v objektu grdData pracovat, ale mají být neviditelné
		/// </summary>
		private int[] hideGrdDataColumns = new int[] {COLUMN_ITEM_VER_KIT, COLUMN_QUALITY_ID, COLUMN_MOVEMENTS_DECISION_ID, COLUMN_MOVEMENTS_TYPE_ID, COLUMN_MOVEMENTS_ADD_SUB_BASE_ID};

		/// <summary>
		/// ItemVerKit nového interního pohybu
		/// </summary>
		private int newItemVerKit = 0;
		
		/// <summary>
		/// ItemOrKitID nového interního pohybu
		/// </summary>
		private int newItemOrKitID = 0;

		/// <summary>
		/// IternalMovementQuantity nového interního pohybu
		/// </summary>
		private decimal iternalMovementQuantity = 0M;
		
		#endregion

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
				this.setButtonsForeColor();
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
			this.MessageText.Value = string.Empty;
			
			ViewState["Filter"] = " 1=1 ";						
			if (!string.IsNullOrWhiteSpace(this.tbxItemOrKitIDFlt.Text)) ViewState["Filter"] += " AND [ItemOrKitID] = " + this.tbxItemOrKitIDFlt.Text.Trim();
			if (this.ddlItemVerKitFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [ItemVerKit] = " + this.ddlItemVerKitFlt.SelectedValue.Trim();
			if (this.ddlKitQualitiesFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [QualityID] = " + this.ddlKitQualitiesFlt.SelectedValue.Trim();
			if (this.ddlDecisionFlt.SelectedValue.ToString() != "-1") ViewState["Filter"] += " AND [MovementsDecisionID] = " + this.ddlDecisionFlt.SelectedValue.Trim();
						
			PagerData pagerData = new PagerData();
			pagerData.PageNum = pageNo;
			pagerData.PageSize = this.grdPager.PageSize;
			pagerData.TableName = "[dbo].[vwInternalMovements]";	
			pagerData.OrderBy = "[ID] DESC";
			pagerData.ColumnList = "[ID], [ItemOrKitID], [ItemVerKit], [ItemVerKitDescription], [ItemOrKitUnitOfMeasureId], [MeasureCode], [IternalMovementQuantity], " +
								   "[IternalMovementQuantityInteger], [QualityID], [QualityCode], [Description], [MovementsTypeID], [MovementTypeDescription], " +
								   "[MovementsDecisionID], [MovementsDecisionDescription], [CreatedDate], [CreatedUserId], [IsActive], [ModifyDate], [ModifyUserId], " +
								   "[CreatedUserLastName], [Remark], [MovementsAddSubBaseID], [MovementsAddSubBaseAbbrev]";
			
			pagerData.WhereClause = ViewState["Filter"].ToString();

			try
			{
				pagerData.ReadData();
				pagerData.FillObject(ref grdData, hideGrdDataColumns);
				pagerData.SetPagerProperties(this.grdPager);
				pagerData.SetRecordCountInfoLabel(this.lblInfoRecordersCount);

				this.prepareMovementTypeAndDecision();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		protected void new_button_Click(object sender, ImageClickEventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 1;

			Session["IsRefresh"] = "0";
			ClearViewControls(vwEdit);
			this.pnlDecision.Visible = false;
			this.MessageText.Value = string.Empty;
			BC.UnbindDataFromObject<GridView>(this.grdData);

			InternalMovementsHelper.FillDdlKits(ref this.ddlKits);
			InternalMovementsHelper.FillDdlItems(ref this.ddlNW);
			InternalMovementsHelper.FillDdlQuality(ref this.ddlKitQualitiesNewIM);
			InternalMovementsHelper.FillDdlAddSubBase(ref this.ddlAddSubBase);
		}

		protected void grdData_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.MessageText.Value = string.Empty;

			if (rowHasNotDecision(this.grdData.SelectedRow) && this.userCanDoDecision())
			{
				this.pnlDecision.Visible = true;				
				this.rdblDecision.ClearSelection();
				Session["IsRefresh"] = "0";
			}
			else
			{
				this.pnlDecision.Visible = false;
			}
		}

		protected void grdData_RowCommand(object sender, GridViewCommandEventArgs e)
		{ 
		}

		/// <summary>
		/// Rozhodnutí schvaluji, resp. zamítám 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDecision_Click(object sender, EventArgs e)
		{
			if ((Session["IsRefresh"] == null || Session["IsRefresh"].ToString() == "0"))
			{
				bool xmOK = false;	
				Int16 iDecision;
				try
				{
					iDecision = Convert.ToInt16(this.rdblDecision.SelectedValue.ToString());
					if (grdData.SelectedValue != null)
					{
						SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
						SqlCommand sqlComm = new SqlCommand();
						sqlComm.CommandType = CommandType.StoredProcedure;
						sqlComm.CommandText = "[dbo].[prInternalMovementDecision]";
						sqlComm.Connection = conn;
						sqlComm.Parameters.Add("@InternalMovementID", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedValue.ToString());
						sqlComm.Parameters.Add("@DecisionID", SqlDbType.Int).Value = iDecision;
						sqlComm.Parameters.Add("@MovementAddSubBaseID", SqlDbType.Int).Value = Convert.ToInt32(grdData.SelectedRow.Cells[COLUMN_MOVEMENTS_ADD_SUB_BASE_ID].Text);						
						sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());
						sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
						sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
						sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 2048);
						sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

						try
						{
							conn.Open();
							sqlComm.ExecuteNonQuery();
							xmOK = (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0");
						}
						catch (Exception ex)
						{
							BC.ProcessException(ex, ApplicationLog.GetMethodName());
						}
						finally
						{
							conn.Close();
							conn = null;
							sqlComm = null;
						}
					}
					else
					{
						this.MessageText.Value = "Vybraný záznam - nelze ho určit. Operace nebude provedena";
					}
				}
				catch (Exception ex)
				{
					iDecision = -1;
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
				if (xmOK)
				{
					this.grdData.SelectedIndex = -1;
					this.pnlDecision.Visible = false;					
					Session["IsRefresh"] = "1";					
					this.fillPagerData(BC.PAGER_FIRST_PAGE);
				}
			}
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.mvwMain.ActiveViewIndex = 0;

			this.grdData.SelectedIndex = -1;
			this.fillPagerData(BC.PAGER_FIRST_PAGE);
		}

		/// <summary>
		/// Nový interní pohyb - MANKO
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDeficiency_Click(object sender, EventArgs e)
		{
			this.addNewInternalMovement(Convert.ToInt32(MANKO));
		}

		/// <summary>
		/// Nový interní pohyb - PŘEBYTEK
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSurplus_Click(object sender, EventArgs e)
		{
			this.addNewInternalMovement(Convert.ToInt32(PREBYTEK));
		}

		/// <summary>
		/// Kontrola zadání údajů pro nový interní pohyb 
		/// Zjištění údajů pro nový interní pohyb
		/// Uložení údajů pro nový interní pohyb 
		/// </summary>
		/// <param name="internalMovementTypeID">typ pohybu {manko, přebytek}</param>
		private void addNewInternalMovement(int internalMovementTypeID)
		{
			if (this.checkSelectingAddSubBase() == false)
			{
				this.MessageText.Value = "+/-  nutno vybrat k čemu se přičítá/od čeho se odečítá. Operaci nelze provést.";	
				return;
			}

			if (this.checkAndGetNewInternalMovementValues())
			{
				if (this.iternalMovementQuantity <= 0M)
				{
					this.MessageText.Value = "Požadované množství musí být větší než nula. Operaci nelze provést.";
					return;
				}

				if ((internalMovementTypeID == Convert.ToInt32(MANKO)) && (this.checkRequiredQuantity(internalMovementTypeID) == false))
				{
					this.MessageText.Value = "Požadované množství není k dispozici. Operaci nelze provést.";
					return;
				}

				if (this.saveNewInternalMovement(internalMovementTypeID))
				{
					this.btnBack_Click(null, EventArgs.Empty);
				}
			}
			else
			{
				this.MessageText.Value = "Nelze určit kombinaci materiálu/kitu a množství. Operaci nelze provést.";
			}
		}

		/// <summary>
		/// Test volby +/- (k čemu se přičítá/od čeho se odečítá)
		/// </summary>
		/// <returns></returns>
		private bool checkSelectingAddSubBase()
		{
			return (this.ddlAddSubBase.SelectedValue != "-1");			
		}

		/// <summary>
		/// Kontrola, zda pro manko platí, že množství volné/uvolněné, resp. rezervované >= požadované množství
		/// </summary>
		/// <returns></returns>
		private bool checkRequiredQuantity(int internalMovementTypeID)
		{
			bool checkQuantity = false;
			
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prInternalMovementStockCardQuantity]";
				sqlComm.Connection = conn;

				sqlComm.Parameters.Add("@ItemVerKit", SqlDbType.Int).Value = this.newItemVerKit;
				sqlComm.Parameters.Add("@ItemOrKitID", SqlDbType.Int).Value = this.newItemOrKitID;
				sqlComm.Parameters.Add("@ItemOrKitQuailityID", SqlDbType.Int).Value = Convert.ToInt32(this.ddlKitQualitiesNewIM.SelectedValue);
				sqlComm.Parameters.Add("@MovementAddSubBaseID", SqlDbType.Int).Value = Convert.ToInt32(this.ddlAddSubBase.SelectedValue);				
				sqlComm.Parameters.Add("@StockCardQuantity", SqlDbType.Decimal, 18);
				sqlComm.Parameters["@StockCardQuantity"].Direction = ParameterDirection.Output;				
				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 2048);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					conn.Open();
					sqlComm.ExecuteNonQuery();
					if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
					{
						checkQuantity = ((ConvertExtensions.ToDecimal(sqlComm.Parameters["@StockCardQuantity"].Value, 0M) >= this.iternalMovementQuantity));
					}
					else
					{
						this.MessageText.Value = sqlComm.Parameters["@ReturnMessage"].Value.ToString().Trim();
					}
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
				finally
				{
					conn.Close();
					conn = null;
					sqlComm = null;
				}
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}
			
			return checkQuantity;
		}

		/// <summary>
		/// Kontrola zadání údajů pro nový interní pohyb + zjištění údajů pro nový interní pohyb
		/// </summary>
		/// <returns></returns>
		private bool checkAndGetNewInternalMovementValues()
		{
			if (txtItemVerKitID.Text.Trim().IsNotNullOrEmpty())
			{
				if (txtItemVerKitQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					this.newItemVerKit = 0;
					this.newItemOrKitID = WConvertStringToInt32(txtItemVerKitID.Text.Trim());
					this.iternalMovementQuantity = Convert.ToDecimal(txtItemVerKitQuantity.Text.Trim());
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (txtItemVerKitQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					return false;
				}
			}

			if (ddlKits.SelectedValue != "-1")
			{
				if (txtKitsQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					this.newItemVerKit = 1;
					this.newItemOrKitID = WConvertStringToInt32(ddlKits.SelectedValue);
					this.iternalMovementQuantity = Convert.ToDecimal(txtKitsQuantity.Text.Trim());
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (txtKitsQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					return false;
				}
			}
			
			if (ddlNW.SelectedValue != "-1")
			{
				if (txtNwQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					this.newItemVerKit = 0;
					this.newItemOrKitID = WConvertStringToInt32(ddlNW.SelectedValue);
					this.iternalMovementQuantity = Convert.ToDecimal(txtNwQuantity.Text.Trim());
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (txtNwQuantity.Text.Trim().IsNotNullOrEmpty())
				{
					return false;
				}
			}
			
			return false;
		}

		/// <summary>
		/// Uložení nového interního pohybu
		/// </summary>
		/// <param name="internalMovementTypeID"></param>
		/// <returns></returns>
		private bool saveNewInternalMovement(int internalMovementTypeID)
		{			
			bool savedOK = false;
			try
			{
				SqlConnection conn = new SqlConnection(BC.FENIXWrtConnectionString);
				SqlCommand sqlComm = new SqlCommand();
				sqlComm.CommandType = CommandType.StoredProcedure;
				sqlComm.CommandText = "[dbo].[prInternalMovementSave]";
				sqlComm.Connection = conn;
					
				sqlComm.Parameters.Add("@ItemVerKit", SqlDbType.Int).Value = this.newItemVerKit;
				sqlComm.Parameters.Add("@ItemOrKitID", SqlDbType.Int).Value = this.newItemOrKitID;
				sqlComm.Parameters.Add("@ItemOrKitQuailityID", SqlDbType.Int).Value = Convert.ToInt32(this.ddlKitQualitiesNewIM.SelectedValue);
				sqlComm.Parameters.Add("@InternalMovementTypeID", SqlDbType.Int).Value = internalMovementTypeID;
				sqlComm.Parameters.Add("@IternalMovementQuantity", SqlDbType.Decimal, 18).Value = this.iternalMovementQuantity;
				sqlComm.Parameters.Add("@Remark", SqlDbType.NVarChar, 512).Value = this.txtRemark.Text != null ? this.txtRemark.Text.Trim() : String.Empty;
				sqlComm.Parameters.Add("@MovementAddSubBaseID", SqlDbType.Int).Value = Convert.ToInt32(this.ddlAddSubBase.SelectedValue);				
				sqlComm.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["Logistika_ZiCyZ"].ToString());
				sqlComm.Parameters.Add("@ReturnValue", SqlDbType.Int);
				sqlComm.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
				sqlComm.Parameters.Add("@ReturnMessage", SqlDbType.Char, 2048);
				sqlComm.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

				try
				{
					conn.Open();
					sqlComm.ExecuteNonQuery();
					if (sqlComm.Parameters["@ReturnValue"].Value.ToString() == "0")
					{
						savedOK = true;
					}
					else
					{
						this.MessageText.Value = sqlComm.Parameters["@ReturnMessage"].Value.ToString().Trim();
					}
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
				finally
				{							
					conn.Close();
					conn = null;
					sqlComm = null;
				}					
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());
			}

			return savedOK;
		}

		/// <summary>
		/// Nastavení barev textu tlačítek Manko a Přebytek
		/// </summary>
		private void setButtonsForeColor()
		{
			this.btnDeficiency.ForeColor = BC.RedColor;
			this.btnSurplus.ForeColor = BC.BlueColor;
		}

		private void prepareMovementTypeAndDecision()
		{
			foreach (GridViewRow gridViewRow in this.grdData.Rows)
			{
				//náhrada textu 'Bez rozhodnutí' znakem '?'
				if (this.rowHasNotDecision(gridViewRow))
				{
					gridViewRow.Cells[10].Text = "?";
				}

				if (gridViewRow.Cells[COLUMN_MOVEMENTS_TYPE_ID].Text == MANKO)
				{
					//manko má červený text
					gridViewRow.Cells[9].ForeColor = BC.RedColor;
				}
				else if (gridViewRow.Cells[COLUMN_MOVEMENTS_TYPE_ID].Text == PREBYTEK)
				{
					//přebytek má modrý text
					gridViewRow.Cells[9].ForeColor = BC.BlueColor;
				}
			}
		}

		/// <summary>
		/// Test, zda řádek je bez rozhodnutí
		/// </summary>
		/// <param name="gridViewRow"></param>
		/// <returns></returns>
		private bool rowHasNotDecision(GridViewRow gridViewRow)
		{
			return gridViewRow.Cells[COLUMN_MOVEMENTS_DECISION_ID].Text == WITHOUT_DECISION;
		}

		/// <summary>
		/// Test, zda uživatel může schvalovat/zamítat manka/přebytky
		/// </summary>
		/// <returns></returns>
		private bool userCanDoDecision()
		{
			bool canDecision = false;
			try
			{
				canDecision = (Session["Logistika_ZiCyZ"].ToString() == BC.MANAGER_LOGISTIKA || Session["Logistika_ZiCyZ"].ToString() == BC.REZLER ||
				               Session["Logistika_ZiCyZ"].ToString() == BC.WECZEREK);
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, ApplicationLog.GetMethodName());					
			}

			return canDecision;
		}
	}
}