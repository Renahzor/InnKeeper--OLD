public class StateTracker{

    public enum States { Idle, WantsFood, WantsDrink, WantsQuest, WantsSocial, WantsHealth }

    States state = States.Idle;

    public States GetCurrentState()
    {
        return state;
    }

    public void UpdateState(AdventurerNeeds needs)
    {
        state = States.Idle;

        float tmp = 85.0f;
        if (needs.socialNeed < tmp)
        {
            state = States.WantsSocial;
            tmp = needs.socialNeed;
        }

        if (needs.questNeed < tmp)
        {
            state = States.WantsQuest;
            tmp = needs.questNeed;
        }

        if (needs.drinkNeed < tmp)
        {
            state = States.WantsDrink;
            tmp = needs.drinkNeed;
        }

        if (needs.foodNeed <= tmp)
        {
            state = States.WantsFood;
            tmp = needs.foodNeed;
        }
    }

    public void ResetState()
    {
        state = States.Idle;
    }
}
