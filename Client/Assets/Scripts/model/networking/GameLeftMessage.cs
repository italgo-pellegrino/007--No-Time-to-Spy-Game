using System;

public class GameLeftMessage : MessageContainer
{
    public Guid leftUserId;
    public GameLeftMessage(Guid playerId, DateTime creationDate ,Guid leftUserId) : base(playerId, MessageTypeEnum.GAME_LEFT, creationDate)
    {
        this.leftUserId = leftUserId;
    }
}
