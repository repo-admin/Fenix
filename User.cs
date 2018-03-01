using System;

namespace Fenix
{
	new public class User
	{
		public int ID_ZiCyz { get; set; }
		public string Login { get; set; }
		public string Lastname { get; set; }
		public string Firstname { get; set; }
		public Nullable<int> Region { get; set; }
		public bool LogistikNetwork { get; set; }
		public bool LogistikCPE { get; set; }
		public bool LogistikReport { get; set; }
		public bool LogistikNahled { get; set; }
		public bool R { get; set; }
		public bool W { get; set; }
		public bool Admin { get; set; }
		public bool IS_ACTIVE { get; set; }
		public System.DateTime EDIT_DATE { get; set; }
		public int EDIT_ID_USER { get; set; }
		public string COMMENT { get; set; }
	}
}