using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Fenix
{
	/// <summary>
	/// User control pro skladové karty
	/// (automaticky vytváří interní doklad)
	/// </summary>
	public partial class CardStockDetail : UserControl
	{
		#region Properties
		public Int32 XID { get; set; }
		public string ItemVerKit { get; set; }
		public Int32 ItemOrKitID { get; set; }
		public Int32 ItemOrKitFree { get; set; }
		public Int32 ItemOrKitUnConsilliation { get; set; }
		public Int32 ItemOrKitReserved { get; set; }
		public Int32 ItemOrKitReleasedForExpedition { get; set; }
		public Int32 ItemOrKitExpedited { get; set; }
		public Int32 Logistika_ZiCyZ { get; set; }
		public Boolean IsActive { get; set; }
		public string DescriptionCz { get; set; }
		public string ItemOrKitQuality { get; set; }
		public string GroopsAndCode { get; set; }
		public string ModifyDate { get; set; }
		public string ModifyUserId { get; set; }
		public string StockName { get; set; }
		public string Err { get; set; }
		public Boolean MOK { get; set; }
		public Int16 Role { get; set; }									// 1... přístupno vše, 2...jen pro převod Kitů
		public InternalDocumentsSource Source { get; set; }				// místo vzniku interního dokladu
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		public void GetCardStockRecord()
		{

			string proS = string.Format("SELECT [ID],[ItemVerKitDescription],[ItemOrKitID],[QualitiesCode]+'/'+[MeasuresCode] AS QM" +
				  ",[DescriptionCz],ISNULL(CAST([GroupGoods] AS VARCHAR(50))+'/','')+[Code] GC,[ItemOrKitFreeInteger],[ItemOrKitUnConsilliationInteger] " +
				  ",[ItemOrKitReservedInteger],[ItemOrKitReleasedForExpeditionInteger] " +
				  ",[ItemOrKitExpeditedInteger],[ModifyDate],[ModifyUserId],[cdlStocksName],[IsActive],[cdlKitGroupsCode],FULL_NAME " +
				  " FROM [dbo].[vwCardStockItems] WHERE Id = {0}", XID.ToString());
			DataTable dt = BC.GetDataTable(proS);

			ItemVerKit = dt.Rows[0][1].ToString();
			ItemOrKitID = Convert.ToInt32(dt.Rows[0][2].ToString());
			ItemOrKitQuality = dt.Rows[0][3].ToString();
			DescriptionCz = dt.Rows[0][4].ToString();
			GroopsAndCode = dt.Rows[0][5].ToString();
			ItemOrKitFree = Convert.ToInt32(dt.Rows[0][6].ToString());
			ItemOrKitUnConsilliation = Convert.ToInt32(dt.Rows[0][7].ToString());
			ItemOrKitReserved = Convert.ToInt32(dt.Rows[0][8].ToString());
			ItemOrKitReleasedForExpedition = Convert.ToInt32(dt.Rows[0][9].ToString());
			ItemOrKitExpedited = Convert.ToInt32(dt.Rows[0][10].ToString());
			ModifyDate = dt.Rows[0][11].ToString();
			ModifyUserId = dt.Rows[0][12].ToString() + " " + dt.Rows[0][16].ToString();
			StockName = dt.Rows[0][13].ToString();
			IsActive = Convert.ToBoolean(dt.Rows[0][14].ToString());

			this.tbxID.Text = XID.ToString();
			this.tbxItemVerKit.Text = ItemVerKit.ToString();
			this.tbxItemOrKitID.Text = ItemOrKitID.ToString();
			this.tbxItemOrKitFree.Text = ItemOrKitFree.ToString();   //
			this.tbxItemOrKitUnConsilliation.Text = ItemOrKitUnConsilliation.ToString();
			this.tbxItemOrKitReserved.Text = ItemOrKitReserved.ToString();
			this.tbxItemOrKitReleasedForExpedition.Text = ItemOrKitReleasedForExpedition.ToString();
			this.tbxItemOrKitExpedited.Text = ItemOrKitExpedited.ToString();
			this.chkbIsActive.Checked = IsActive;
			this.tbxDescriptionCz.Text = DescriptionCz;
			this.tbxItemOrKitQuality.Text = ItemOrKitQuality;
			this.tbxGroopsAndCode.Text = GroopsAndCode;
			this.tbxModifyDate.Text = ModifyDate;
			this.tbxModifyUserId.Text = ModifyUserId;
			this.lblStockName.Text = StockName;

			if (Role == 2)
			{
				this.tbxItemOrKitFree.Enabled = false;
				this.tbxItemOrKitReleasedForExpedition.Enabled = false;
				this.tbxItemOrKitExpedited.Enabled = false;
				this.tbxItemOrKitUnConsilliation.Enabled = false;
				this.tbxItemOrKitReserved.Enabled = false;
				this.lblRelease.Visible = true;
				this.tbxRelease.Visible = true; this.tbxRelease.Text = "";
			}
		}

		public void SetCardStockRecord() 
		{
			if (Session["IsRefresh"].ToString() == "0")
			{
				this.Err = "";
				// ***** Kontroly *****
				#region kontroly
				bool mOK = true;
				int iiItemOrKitFree = 0; int iiItemOrKitReleasedForExpedition = 0; int iiItemOrKitExpedited = 0;
				int iiItemOrKitUnConsilliation = 0; int iiItemOrKitReserved = 0; int IsActive = 0; int iiItemOrKitRelease = 0;

				if (this.tbxRelease.Visible == true)
				{
					try
					{
						iiItemOrKitFree = Convert.ToInt32(this.tbxItemOrKitFree.Text.Trim());
						iiItemOrKitRelease = Convert.ToInt32(this.tbxRelease.Text.Trim());
						iiItemOrKitReleasedForExpedition = Convert.ToInt32(this.tbxItemOrKitReleasedForExpedition.Text.Trim());
						iiItemOrKitFree = iiItemOrKitFree - iiItemOrKitRelease;
						iiItemOrKitReleasedForExpedition = iiItemOrKitReleasedForExpedition + iiItemOrKitRelease;
						this.tbxItemOrKitFree.Text = iiItemOrKitFree.ToString();  // 24-02-24
						this.tbxItemOrKitReleasedForExpedition.Text = iiItemOrKitReleasedForExpedition.ToString(); //2015-03-02

					}
					catch (Exception)
					{
						mOK = false;
						this.Err += "Zkontrolujte hodnotu množství k uvolnění<br />";
					}
				}

				try
				{
					iiItemOrKitFree = Convert.ToInt32(this.tbxItemOrKitFree.Text.Trim());
					// 2015-01-19  == 0, 24-02-24   < 1
					if (iiItemOrKitFree < 0 && Session["Logistika_ZiCyZ"].ToString() != "542" && Session["Logistika_ZiCyZ"].ToString() != "780")
					{
						mOK = false;
						this.Err += "Volné množství nesmí být nula (0) nebo menší než nula <br />";   // 24-02-24  byla kontrola na nulu,
					}

				}
				catch (Exception)
				{
					mOK = false;
					this.Err += "Zkontrolujte hodnotu volného množství<br />";
				}
				try
				{
					iiItemOrKitReleasedForExpedition = Convert.ToInt32(this.tbxItemOrKitReleasedForExpedition.Text.Trim());
				}
				catch (Exception)
				{
					mOK = false;
					this.Err += "Zkontrolujte hodnotu uvolněného množství pro Expedici<br />";
				}
				try
				{
					iiItemOrKitExpedited = Convert.ToInt32(this.tbxItemOrKitExpedited.Text.Trim());
				}
				catch (Exception)
				{
					mOK = false;
					this.Err += "Zkontrolujte hodnotu expedovaného množství<br />";
				}

				try
				{
					iiItemOrKitUnConsilliation = Convert.ToInt32(this.tbxItemOrKitUnConsilliation.Text.Trim());
				}
				catch (Exception)
				{
					mOK = false;
					this.Err += "Zkontrolujte hodnotu množství ke schválení<br />";
				}
				try
				{
					iiItemOrKitReserved = Convert.ToInt32(this.tbxItemOrKitReserved.Text.Trim());
				}
				catch (Exception)
				{
					mOK = false;
					this.Err += "Zkontrolujte hodnotu rezervovaného množství<br />";
				}

				IsActive = Convert.ToInt32(this.chkbIsActive.Checked);

				
				#endregion

				if (mOK)
				{
					InternalDocument internalDocument = new InternalDocument(this.Logistika_ZiCyZ, this.Source);
					internalDocument.CardStockStatusBefore = internalDocument.GetCardStock(Convert.ToInt32(this.tbxID.Text.Trim()));

					SqlConnection con = new SqlConnection();
					con.ConnectionString = BC.FENIXWrtConnectionString;
					string proU = string.Empty;

					SqlCommand com = new SqlCommand();
					com.Connection = con;

					com.CommandType = CommandType.StoredProcedure;
					com.CommandText = "[dbo].[prCardStockItemsManuallyUpd]";   //

					com.Parameters.Add("@ItemID", SqlDbType.Int).Value = Convert.ToInt32(this.tbxID.Text.Trim());
					com.Parameters.Add("@ItemOrKitFree", SqlDbType.Int).Value = iiItemOrKitFree;
					com.Parameters.Add("@ItemOrKitUnConsilliation", SqlDbType.Int).Value = iiItemOrKitUnConsilliation;
					com.Parameters.Add("@ItemOrKitReserved", SqlDbType.Int).Value = iiItemOrKitReserved;
					com.Parameters.Add("@ItemOrKitReleasedForExpedition", SqlDbType.Int).Value = iiItemOrKitReleasedForExpedition;
					com.Parameters.Add("@ItemOrKitExpedited", SqlDbType.Int).Value = iiItemOrKitExpedited;
					com.Parameters.Add("@ModifyUserId", SqlDbType.Int).Value = Logistika_ZiCyZ.ToString();  // ID ZiCyz

					com.Parameters.Add("@ReturnValue", SqlDbType.Int);
					com.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
					com.Parameters.Add("@ReturnMessage", SqlDbType.NVarChar, 2048);
					com.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;

					try
					{
						con.Open();
						com.ExecuteNonQuery();
						if (com.Parameters["@ReturnValue"].Value.ToString() == "0")
						{
							mOK = true;
							internalDocument.CardStockStatusAfter = internalDocument.GetCardStock(Convert.ToInt32(this.tbxID.Text.Trim()));
						}
					}
					catch (Exception ex)
					{
						BC.ProcessException(ex, ApplicationLog.GetMethodName(), "proU =  " + proU);
						mOK = false;
					}
					finally
					{
						if (con.State == ConnectionState.Open) con.Close();
						con.Dispose();
						com.Dispose();
						MOK = mOK;
					}
					if (mOK)
					{
						Session["IsRefresh"] = "1";
						internalDocument.InternalDocumentSave();
					}
				}
			}
		}
	}
}