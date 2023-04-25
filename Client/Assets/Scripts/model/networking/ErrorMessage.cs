using System;

public class ErrorMessage : MessageContainer
{
    public string reason;

    public ErrorMessage(Guid playerId,DateTime creationDate, string reason) : base(playerId, MessageTypeEnum.ERROR, creationDate)
    {
        this.reason = reason;
    }
}
