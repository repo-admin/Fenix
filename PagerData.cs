using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using UPC.Extensions.Convert;
using UPC.WebControls;

namespace Fenix
{
	/// <summary>
	/// Data pro Pager
	/// </summary>
	public class PagerData
	{	
		protected SqlConnection SqlConnection { get; set; }
		protected SqlCommand SqlCommand { get; set; }		
		protected SqlDataAdapter SqlDataAdapter { get; set; }
		public string ConnectionString { get; set; }
		public DataSet DataSet { get; private set; }		
		public int PageNum { get; set; }
		public int PageSize { get; set; }
		public int ItemCount { get; private set; }
		public string TableName { get; set; }
		public string OrderBy { get; set; }
		public string ColumnList { get; set; }
		public string WhereClause { get; set; }

		/// <summary>
		/// ctor
		/// </summary>
		public PagerData()
			:this(BC.FENIXRdrConnectionString)
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="connectionString"></param>
		public PagerData(string connectionString)
		{
			this.ConnectionString = connectionString;
			this.DataSet = new DataSet();
			this.SqlConnection = new SqlConnection(connectionString);			
		}

		/// <summary>
		/// Načte data
		/// </summary>
		/// <returns></returns>
		public bool ReadData()
		{
			bool result = false;

			this.SqlCommand = new SqlCommand("dbo.sp_SelectPage", SqlConnection);
			this.SqlCommand.CommandType = CommandType.StoredProcedure;
			this.SqlCommand.Parameters.Add("@PageNum", SqlDbType.Int).Value = this.PageNum;
			this.SqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = this.PageSize;
			this.SqlCommand.Parameters.Add("@ItemCount", SqlDbType.BigInt).Direction = ParameterDirection.Output;
			this.SqlCommand.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = this.TableName;
			this.SqlCommand.Parameters.Add("@OrderBy", SqlDbType.VarChar).Value = this.OrderBy;			
			this.SqlCommand.Parameters.Add("@ColumnList", SqlDbType.VarChar).Value = this.ColumnList;
			this.SqlCommand.Parameters.Add("@WhereClause", SqlDbType.VarChar).Value = this.WhereClause;
			this.SqlCommand.CommandTimeout = BC.SQL_COMMAND_TIMEOUT;

			try
			{
				SqlDataAdapter = new SqlDataAdapter(this.SqlCommand);
				SqlDataAdapter.Fill(this.DataSet);
				this.ItemCount = ConvertExtensions.ToInt32(this.SqlCommand.Parameters["@ItemCount"].Value, 0);
				result = true;
			}
			catch
			{
				throw;
			}
			finally 
			{
				if (SqlConnection.State == ConnectionState.Open)
				{
					SqlConnection.Close();
				}
			}
			
			return result;
		}

		/// <summary>
		/// Naplní GridView daty a skryje požadovaný sloupec/požadované sloupce
		/// </summary>
		/// <param name="grdData">GridView, které plníme daty</param>
		/// <param name="hideColumn">pole sloupců, se kterými chceme pracovat, ale mají být neviditelné</param>
		public void FillObject(ref GridView grdData, int[] hideColumns)
		{
			foreach (var hideColumn in hideColumns)
			{
				grdData.Columns[hideColumn].Visible = true;	
			}
						
			this.FillObject(ref grdData);

			foreach (var hideColumn in hideColumns)
			{
				grdData.Columns[hideColumn].Visible = false;
			}			
		}

		/// <summary>
		/// Naplní objekt daty a skryje požadovaný sloupec
		/// </summary>
		/// <param name="grdData"></param>
		/// <param name="hideColumn"></param>
		public void FillObject(ref GridView grdData, int hideColumn)
		{
			grdData.Columns[hideColumn].Visible = true;
			this.FillObject(ref grdData);
			grdData.Columns[hideColumn].Visible = false;
		}
		
		/// <summary>
		/// Naplní objekt daty
		/// </summary>
		/// <param name="grdData"></param>
		public void FillObject(ref GridView grdData)
		{
			grdData.DataSource = this.DataSet.Tables[0].DefaultView;
			grdData.DataBind();			
		}

		/// <summary>
		/// Nastaví vlastnosti Pageru
		/// </summary>
		/// <param name="pager"></param>
		public void SetPagerProperties(Pager pager)
		{
			pager.ItemCount = Convert.ToDouble(this.ItemCount);
			pager.CurrentIndex = this.PageNum;
			pager.Visible = (this.ItemCount > 0 ? true : false);			
		}

		/// <summary>
		/// Naplní label textem a počtem záznamů
		/// </summary>
		/// <param name="label"></param>
		internal void SetRecordCountInfoLabel(Label label)
		{
			label.Text = "Počet záznamů: " + this.ItemCount.ToString();
		}
	}
}