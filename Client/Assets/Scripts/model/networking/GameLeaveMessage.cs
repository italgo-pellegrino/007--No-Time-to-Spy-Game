using System;

public class GameLeaveMessage : MessageContainer
{
    public GameLeaveMessage(Guid playerId) : base(playerId, MessageTypeEnum.GAME_LEAVE, DateTime.Now)
    {
    }
}
