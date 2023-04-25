using System;

public class MessageContainer
{
	public Guid clientId;
	public MessageTypeEnum type;
	public DateTime creationDate;
	public string debugMessage;
	public MessageContainer(){}
	public MessageContainer(Guid playerId, MessageTypeEnum type, DateTime creationDate)
	{
		this.clientId = playerId;
		this.type = type;
		this.creationDate = creationDate;
		this.debugMessage = "";
	}
	public MessageContainer(Guid playerId, MessageTypeEnum type, DateTime creationDate, string debugMessage) : this(playerId, type, creationDate)
	{
		this.debugMessage = debugMessage;
	}
}
