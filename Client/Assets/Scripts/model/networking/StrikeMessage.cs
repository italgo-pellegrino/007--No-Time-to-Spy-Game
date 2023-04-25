using System;

public class StrikeMessage : MessageContainer
{
    public int strikeNr;
    public int strikeMax;
    public string reason;

    public StrikeMessage(Guid playerId, DateTime creationDate, int strikeNr, int strikeMax, string reason) : base(playerId, MessageTypeEnum.STRIKE, creationDate)
    {
        this.strikeNr = strikeNr;
        this.strikeMax = strikeMax;
        this.reason = reason;
    }
}
