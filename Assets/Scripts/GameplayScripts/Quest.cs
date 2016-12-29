//Holds info for current quests for heroes
public class Quest{

    public int locationIndex, objectiveIndex, objectiveQuantity, goldReward;

    public int timesCompleted;

    public Quest(int _locationIndex, int _objectiveIndex, int _goldReward)
    {
        locationIndex = _locationIndex;
        objectiveIndex = _objectiveIndex;
        goldReward = _goldReward;
    }
}
