using System;

public class HelloReplyMessage : MessageContainer
{
    public Guid sessionId;
    public Scenario level;
    public Matchconfig settings;
    public CharacterInformation[] characterSettings;

    public HelloReplyMessage(Guid playerId, DateTime creationDate, Guid sessionId, 
        Scenario level, Matchconfig settings,CharacterInformation[] characterSettings) : base(playerId, MessageTypeEnum.HELLO_REPLY, creationDate)
    {
        this.sessionId = sessionId;
        this.level = level;
        this.settings = settings;
        this.characterSettings = characterSettings;
    }

}
