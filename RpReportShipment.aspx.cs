using System;
using System.IO;
using System.Threading;
using Fenix.Reports;
using FenixHelper;

namespace Fenix
{
	/// <summary>
	/// [Expedice] Report expedice
	/// </summary>
	public partial class RpReportShipment : BasePage
	{
		#region Properties

		private string dateFrom;
		private string dateTo;

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
				this.debug();
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (this.DesignMode) return;			
		}

		protected void btnExport_Click(object sender, EventArgs e)
		{
			string errMessage = String.Empty;

			if (validation(ref errMessage))
			{
				if (this.doExport(ref errMessage) == false)
				{
					this.lblErrInfo.Text = errMessage; 
				}
			}
			else
			{
				this.lblErrInfo.Text = errMessage; 
			}
		}

		#region Private Methods

		/// <summary>
		/// Pro zrychlení vývoje => metodu i její volání LZE po ukončení vývoje zrušit
		/// </summary>
		private void debug()
		{
			if (Session["Logistika_ZiCyZ"].ToString() == "1084" /*M. Rezler*/)
			{
				this.txbDateFrom.Text = "1.12.2014";
				this.txbDateTo.Text = "31.12.2014";
			}
		}

		/// <summary>
		/// Kontrola zadání datumu od a datumu do
		/// - datumy musí být vyplněné a musí být zadány správně na tvar d.m.yyyy
		/// - nesmyslné zadání např. 1.1.1769 není chyba = nic se nevyexportuje
		/// </summary>
		/// <param name="errMessage"></param>
		/// <returns></returns>
		private bool validation(ref string errMessage)
		{			
			this.removeSpacesFromInputDates();

			this.dateFrom = this.txbDateFrom.Text.Trim();
			this.dateTo = this.txbDateTo.Text.Trim();

			if (this.dateFrom.IsNullOrEmpty())
			{
				errMessage += string.Format("Nutno zadat Datum Od:{0}", "<br />");
			}
			else
			{
				if (BC.DateIsValid(this.dateFrom) == false)
				{
					errMessage += string.Format("Datum Od:{0} zadáno chybně {1}", this.dateFrom, "<br />");
				}
			}

			if (this.dateTo.IsNullOrEmpty())
			{
				errMessage += string.Format("Nutno zadat Datum Do:{0}", "<br />");
			}
			else
			{
				if (BC.DateIsValid(this.dateTo) == false)
				{
					errMessage += string.Format("Datum Do:{0} zadáno chybně {1}", this.dateTo, "<br />");
				}
			}
			
			return (errMessage == String.Empty);
		}

		private bool doExport(ref string errMessage)
		{
			errMessage = String.Empty;

			using (MemoryStream memStream = new MemoryStream())
			{
				try
				{
					ShipmentOrdersMonthlyReport shipmentOrdersMonthlyReport = new ShipmentOrdersMonthlyReport(this.txbDateFrom.Text, this.txbDateTo.Text);
					MemoryStream memoryStream = shipmentOrdersMonthlyReport.CreateReport();
					Response.Clear();
					Response.Buffer = true;
					Response.AddHeader("content-disposition", "attachment;filename=Report_shipment_monthly_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".xlsx");
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.Charset = "";
					EnableViewState = false;
					Response.BinaryWrite(memoryStream.ToArray());				
					Response.End();
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception ex)
				{					
					errMessage = ex.Message;
				}
			}

			return (errMessage == String.Empty);
		}

		/// <summary>Zrušení mezer v zadání datumů</summary>
		private void removeSpacesFromInputDates()
		{
			this.txbDateFrom.Text = this.txbDateFrom.Text.Trim().Replace(" ", "");
			this.txbDateTo.Text = this.txbDateTo.Text.Trim().Replace(" ", "");
		}

		#endregion
	}
}