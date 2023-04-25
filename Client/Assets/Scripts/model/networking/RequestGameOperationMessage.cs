using System;
using UnityEngine;

public class RequestGameOperationMessage : MessageContainer
{
    public Guid characterId;

    public RequestGameOperationMessage(Guid playerId, DateTime creationDate,
        Guid characterId) : base(playerId, MessageTypeEnum.REQUEST_GAME_OPERATION, creationDate)
    {
        this.characterId = characterId;
    }

}
