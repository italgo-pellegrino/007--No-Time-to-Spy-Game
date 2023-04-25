using System;

public class StatisticsMessage : MessageContainer
{
    public Statistics statistics;
    public Guid winner;
    public VictoryEnum reason;
    public bool hasReplay;
    public StatisticsMessage(Guid playerId, DateTime creationDate, Statistics statistics,
        Guid winner, VictoryEnum reason, bool hasReplay) : base(playerId, MessageTypeEnum.STATISTICS, creationDate)
    {
        this.statistics = statistics;
        this.winner = winner;
        this.reason = reason;
        this.hasReplay = hasReplay;
    }
}
