using System;
using System.Linq;

namespace Fenix
{
	/// <summary>
	/// Interní doklad - evidence změn na skladové kartě
	/// </summary>
	public class InternalDocument
	{
		/// <summary>
		/// ZicyzId uživatele, který provedl změnu na skladové kartě
		/// </summary>
		private int zicyzId;

		/// <summary>
		/// Místo vzniku interního dokladu
		/// </summary>
		private InternalDocumentsSource source;

		/// <summary>
		/// Stav na skladové kartě před změnou/změnami
		/// </summary>
		public CardStockItems CardStockStatusBefore { get; set; }		
 
		/// <summary>
		/// Stav na skladové kartě po změně/změnách
		/// </summary>
		public CardStockItems CardStockStatusAfter { get; set; }

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="zicyzId"></param>
		/// <param name="source"></param>
		public InternalDocument(int zicyzId, InternalDocumentsSource source)
		{
			this.zicyzId = zicyzId;
			this.source = source;
		}

		/// <summary>
		/// Vrací skladovou kartu (dle ID)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CardStockItems GetCardStock(int id)
		{
			CardStockItems cardStockItem = new CardStockItems();

			using (var db = new FenixEntities())
			{
				try
				{
					db.Configuration.LazyLoadingEnabled = false;
					db.Configuration.ProxyCreationEnabled = false;

					cardStockItem = (from c in db.CardStockItems
									 where c.IsActive == true && c.ID == id
									 select c).FirstOrDefault();
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			}

			return cardStockItem;
		}

		/// <summary>
		/// Uložení interního dokladu do databáze
		/// </summary>
		public void InternalDocumentSave()
		{
			if ((this.CardStockStatusBefore == null) || (this.CardStockStatusAfter == null) || (this.isSame())) return;
			
			using (var db = new FenixEntities())
			{
				try
				{
					db.Configuration.LazyLoadingEnabled = false;
					db.Configuration.ProxyCreationEnabled = false;

					InternalDocuments internalDocument = new InternalDocuments();
					internalDocument.ItemVerKit = this.CardStockStatusBefore.ItemVerKit;
					internalDocument.ItemOrKitID = this.CardStockStatusBefore.ItemOrKitID;
					internalDocument.ItemOrKitUnitOfMeasureId = this.CardStockStatusBefore.ItemOrKitUnitOfMeasureId;
					internalDocument.ItemOrKitQualityId = this.CardStockStatusBefore.ItemOrKitQuality;
					//1
					internalDocument.ItemOrKitFreeBefore = this.CardStockStatusBefore.ItemOrKitFree;
					internalDocument.ItemOrKitFreeAfter = this.CardStockStatusAfter.ItemOrKitFree;
					//2
					internalDocument.ItemOrKitUnConsilliationBefore = this.CardStockStatusBefore.ItemOrKitUnConsilliation;
					internalDocument.ItemOrKitUnConsilliationAfter = this.CardStockStatusAfter.ItemOrKitUnConsilliation;
					//3
					internalDocument.ItemOrKitReservedBefore = this.CardStockStatusBefore.ItemOrKitReserved;
					internalDocument.ItemOrKitReservedAfter = this.CardStockStatusAfter.ItemOrKitReserved;
					//4
					internalDocument.ItemOrKitReleasedForExpeditionBefore = this.CardStockStatusBefore.ItemOrKitReleasedForExpedition;
					internalDocument.ItemOrKitReleasedForExpeditionAfter = this.CardStockStatusAfter.ItemOrKitReleasedForExpedition;
					//5
					internalDocument.ItemOrKitExpeditedBefore = this.CardStockStatusBefore.ItemOrKitExpedited;
					internalDocument.ItemOrKitExpeditedAfter = this.CardStockStatusAfter.ItemOrKitExpedited;

					internalDocument.StockId = 2;	//ND/XPO
					internalDocument.InternalDocumentsSourceId = (int)this.source;
					internalDocument.IsActive = true;
					internalDocument.ModifyDate = DateTime.Now;
					internalDocument.ModifyUserId = this.zicyzId;

					db.InternalDocuments.Add(internalDocument);
					db.SaveChanges();
				}
				catch (Exception ex)
				{
					BC.ProcessException(ex, ApplicationLog.GetMethodName());
				}
			} 
		} 

		/// <summary>
		/// Rozhodnutí, zda sledovaná množství na skladové kartě před změnou a po změně jsou stejná
		/// </summary>
		/// <returns></returns>
		private bool isSame()
		{
			return (
						(this.CardStockStatusBefore.ItemOrKitFree == this.CardStockStatusAfter.ItemOrKitFree) &&
						(this.CardStockStatusBefore.ItemOrKitUnConsilliation == this.CardStockStatusAfter.ItemOrKitUnConsilliation) &&
						(this.CardStockStatusBefore.ItemOrKitReserved == this.CardStockStatusAfter.ItemOrKitReserved) &&
						(this.CardStockStatusBefore.ItemOrKitReleasedForExpedition == this.CardStockStatusAfter.ItemOrKitReleasedForExpedition) &&
						(this.CardStockStatusBefore.ItemOrKitExpedited == this.CardStockStatusAfter.ItemOrKitExpedited)
					);

		}
	}
}