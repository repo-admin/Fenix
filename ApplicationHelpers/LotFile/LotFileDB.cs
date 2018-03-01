using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using FenixHelper;

namespace Fenix.ApplicationHelpers.LotFile
{
	/// <summary>
	/// Třída pro manipulaci s LOT file
	/// (předpokládá se, že LOT file je sekvenční ASCII soubor)
	/// </summary>
	public class LotFileDB
	{
		// Každý GZIP stream začíná následujícími třemi bajty
		private static readonly byte[] signGZip = { 0x1f, 0x8b, 0x08 };

		public string LotFileID { get; set; }

		public string Error { get; set; }

		public LotFileDB(string lotFileID)
		{
			this.LotFileID = lotFileID;
		}

		public bool ReadLotFileFromDB(ref List<string> lotFileRows)
		{
			bool result = false;

			SqlConnection conn = null;
			SqlCommand comm = null;
			
			try
			{
				conn = new SqlConnection(BC.FENIXWrtConnectionString);
				comm = new SqlCommand("SELECT FileName, FileData FROM dbo.LotFiles WHERE ID = @ID", conn);
				comm.Parameters.AddWithValue("@ID", this.LotFileID);

				conn.Open();
				SqlDataReader rdr = comm.ExecuteReader();

				if (rdr.Read())
				{
					string fileName = rdr.GetString(0);
					System.Data.SqlTypes.SqlBytes bytes = rdr.GetSqlBytes(1);
					if (bytes != null && bytes.Buffer != null)
					{
						// Načíst první tři bajty ze streamu
						byte[] sign = new byte[3];
						bytes.Stream.Read(sign, 0, 3);
						bytes.Stream.Seek(0, SeekOrigin.Begin);

						if (sign[0] == signGZip[0] && sign[1] == signGZip[1] && sign[2] == signGZip[2])
						{
							MemoryStream ms = new MemoryStream(bytes.Buffer);
							GZipStream gz = new GZipStream(ms, CompressionMode.Decompress);

							// Stream pro dekomprimaci
							MemoryStream msDecomp = new MemoryStream();

							// Čtení komprimovaného streamu a zápis do dekomprimovaného streamu
							int bytesRead = 0;
							int bytesDecomp = 1000;
							byte[] buffStore = new byte[bytesDecomp];
							while ((bytesRead = gz.Read(buffStore, 0, buffStore.Length)) > 0)
								msDecomp.Write(buffStore, 0, bytesRead);

							msDecomp.Flush();
							byte[] buffDecomp = msDecomp.ToArray();

							gz.Close();
							ms.Close();
							msDecomp.Close();

							//context.Response.BinaryWrite(buffDecomp);
							lotFileRows = this.byteBufferToList(buffDecomp);
						}
						else
							//context.Response.BinaryWrite(bytes.Buffer);
							lotFileRows = this.byteBufferToList(bytes.Buffer);

						result = true;
					}
					else
						this.Error = "Data nenalezena";						
				}
				rdr.Close();
			}
			catch (Exception ex)
			{
				BC.ProcessException(ex, AppLog.GetMethodName());
				this.Error = ex.Message;
			}
			finally
			{
				conn.Close();
				comm.Dispose();
			}

			return result;
		}

		private List<string> byteBufferToList(byte[] buffer)
		{
			List<string> list = new List<string>();

			using (MemoryStream m = new MemoryStream(buffer))
			using (StreamReader sr = new StreamReader(m))
			{
				while (!sr.EndOfStream)
				{
					string s = sr.ReadLine();
					list.Add(s);
				}
			}

			return list;
		}

	}
}