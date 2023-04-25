using System;

public class HelloMessage : MessageContainer
{
	public string name;
	public RoleEnum role;
	public HelloMessage(Guid playerId, DateTime creationDate, string name, RoleEnum role) : base(playerId, MessageTypeEnum.HELLO, creationDate)
	{
		this.name = name;
		this.role = role;
	}
}
