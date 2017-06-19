

using System.Security.Policy;

public sealed class Goals{
    public const string TAKE_A_REST_NAME = "rest_goal";
    public const string SURVIVE_NAME = "survive_goal";
    public const string FIND_ENEMY_NAME = "find_enemy_goal";
    public const string KILL_ENEMY_NAME = "kill_enemy_goal";

    public class RestGoal : Goal{
        public RestGoal(float importance) : base(TAKE_A_REST_NAME, importance){}
        public override float getDiscontentment(float newImportanceValue){
            return newImportanceValue * newImportanceValue;
        }
    }

    public class SurviveGoal : Goal
    {
        public SurviveGoal(float importance) : base(SURVIVE_NAME, importance){}
        public override float getDiscontentment(float newImportanceValue)
        {
            return newImportanceValue * newImportanceValue;
        }
    }

    public class FindEnemyGoal : Goal
    {
        public FindEnemyGoal(float importance) : base(FIND_ENEMY_NAME, importance) { }
        public override float getDiscontentment(float newImportanceValue)
        {
            return newImportanceValue * newImportanceValue;
        }
    }

    public class KillEnemyGoal : Goal
    {
        public KillEnemyGoal(float importance) : base(KILL_ENEMY_NAME, importance) { }
        public override float getDiscontentment(float newImportanceValue)
        {
            return newImportanceValue * newImportanceValue * newImportanceValue;
        }
    }
}

