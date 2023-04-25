using System;

public class ReconnectMessage : MessageContainer 
{
	public Guid sessionId;
	public ReconnectMessage(Guid playerId, Guid sessionId, DateTime creationDate) : base(playerId, MessageTypeEnum.RECONNECT, creationDate)
	{
		this.sessionId = sessionId;
	}
}
