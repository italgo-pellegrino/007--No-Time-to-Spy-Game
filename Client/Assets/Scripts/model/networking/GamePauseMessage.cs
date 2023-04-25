using System;

public class GamePauseMessage : MessageContainer
{
    public bool gamePaused;
    public bool serverEnforced;

    public GamePauseMessage(Guid playerId, DateTime creationDate, bool gamePaused, bool serverEnforced) : base(playerId, MessageTypeEnum.GAME_PAUSE, creationDate)
    {
        this.gamePaused = gamePaused;
        this.serverEnforced = serverEnforced;
    }
}
