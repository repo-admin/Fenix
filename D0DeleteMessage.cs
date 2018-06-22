namespace Fenix
{
	public enum DeleteOrderMessageStatus
	{
		None = 1,
		WasDeleted = 2,
		CanBeDeleted = 3		
	}

	public class D0DeleteMessage
	{
		private int messageType;
		//private int id;

		private D0DeleteMessage()
		{ 
		}

		public D0DeleteMessage(int messageType)
		{
			this.messageType = messageType;
		}

		protected internal DeleteOrderMessageStatus GetStatusForID(int id)
		{
			return DeleteOrderMessageStatus.None;
		}
	}
}