using System;
using System.Globalization;

public class GameStartedMessage : MessageContainer
{
    public Guid playerOneId, playerTwoId;
    public string playerOneName, playerTwoName;
    public Guid sessionId;

    public GameStartedMessage(Guid playerId, DateTime creationDate, Guid playerOneId, 
        Guid playerTwoId, string playerOneName, string playerTwoName, Guid sessionId) : base(playerId, MessageTypeEnum.GAME_STARTED, creationDate)
    {
        this.playerOneId = playerOneId;
        this.playerTwoId = playerTwoId;
        this.playerOneName = playerOneName;
        this.playerTwoName = playerTwoName;
        this.sessionId = sessionId;
    }

}
