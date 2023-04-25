using System;
using System.Collections.Generic;

public class GameStatusMessage : MessageContainer
{
    public Guid? activeCharacterId;
    public List<BaseOperation> operations;
    public State state;
    public bool isGameOver;

    public GameStatusMessage(Guid playerId, DateTime creationDate, Guid? activeCharacterId,
        List<BaseOperation> operations, State state, bool isGameOver) : base(playerId, MessageTypeEnum.GAME_STATUS, creationDate)
    {
        this.activeCharacterId = activeCharacterId;
        this.operations = operations;
        this.state = state;
        this.isGameOver = isGameOver;
    }
}
