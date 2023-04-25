using System;

public class GameOperationMessage : MessageContainer
{
    public Operation operation;

    public GameOperationMessage(Guid playerId, DateTime creationDate, Operation operation) : base(playerId, MessageTypeEnum.GAME_OPERATION, creationDate)
    {
        this.operation = operation;
    }
}
