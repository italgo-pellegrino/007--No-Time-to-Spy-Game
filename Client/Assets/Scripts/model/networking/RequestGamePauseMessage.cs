using System;

public class RequestGamePauseMessage : MessageContainer
{
    public bool gamePause;
    public RequestGamePauseMessage(Guid playerId, bool gamePause) : base(playerId, MessageTypeEnum.REQUEST_GAME_PAUSE, DateTime.Now)
    {
        this.gamePause = gamePause;
    }
}
