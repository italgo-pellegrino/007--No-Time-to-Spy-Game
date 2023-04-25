public class Scenario
{
    private FieldStateEnum[,] scenario;
    public Scenario(FieldStateEnum[,] scenario)
    {
        this.scenario = scenario;
    }
    public FieldStateEnum[,] getScenario()
    {
        return scenario;
    }
}