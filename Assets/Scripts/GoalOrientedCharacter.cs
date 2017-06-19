using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GoalOrientedCharacter : MonoBehaviour, ActionCallback{
    //MOVABLE CHARACTER

    //
    [SerializeField]
    private int hpPoints = 100;
    [SerializeField]
    private int stamina = 100;
    public string enemyTag;
    protected List<Goal> goals;
    protected List<Action> availableActions;
    protected Goal restGoal = new Goals.RestGoal(1);
    protected Goal surviveGoal = new Goals.SurviveGoal(0)  ;
    protected Goal  FindEnemyGoal =new Goals.FindEnemyGoal(2.5f);
    protected Goal killEnemyGoal = new Goals.KillEnemyGoal(3f);
    private RegenerateAction regenerateAction;
    public void Awake(){
        goals = new List<Goal>();
        //Initial values, reflecting state where stamina is full, hp is full, and no enemy is in range.
        goals.Add(surviveGoal);
        goals.Add(restGoal);
        goals.Add(FindEnemyGoal);
        goals.Add(killEnemyGoal);
        availableActions = new List<Action>();
        regenerateAction = new GoalOrientedCharacter.RegenerateAction(this, this);
        availableActions.Add(regenerateAction);

    }

    protected Action currentlyRunningAction = null;
    protected void updateAction(){
        if (currentlyRunningAction!=null||availableActions.Count == 0) return;

        Action bestAction = availableActions[0];
        float bestDiscontentment = calculateDiscontentment(availableActions[0], goals);
        StringBuilder builder = new StringBuilder();
        builder.Append(" restGoal: " + restGoal.importance);
        builder.Append(" surviveGoal: " + surviveGoal.importance);
        builder.Append(" FindEnemyGoal: " + FindEnemyGoal.importance);
        builder.Append("killEnemyGoal: " + killEnemyGoal.importance);

        Debug.Log(builder);
        Debug.Log("________________________________________");
        foreach (var VARIABLE in availableActions){
            float newDiscontentment = calculateDiscontentment(VARIABLE, goals);
            Debug.Log(VARIABLE.ToString()+ " discontentment: " + newDiscontentment);

            if (newDiscontentment <= bestDiscontentment){
                bestAction = VARIABLE;
                bestDiscontentment = newDiscontentment;
            }
        }
        Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

        currentlyRunningAction = bestAction;
        bestAction.performAction(this.gameObject);
    }

    private float calculateDiscontentment(Action a, List<Goal> goals){
        float discontentment = 0;
        foreach (var VARIABLE in goals){
            float valueAfterAction = VARIABLE.importance + a.getGoalChange(VARIABLE);
            discontentment += VARIABLE.getDiscontentment(valueAfterAction);
        }
        return discontentment;
    }

    public void Start(){
        updateAction();
    }


    public void Update(){
    
    }

    public void OnActionEnd(Action src){
        if (src == currentlyRunningAction){
            currentlyRunningAction = null;
        }
        updateAction();
    }

    protected void AddAction(Action newAct){
        if(!availableActions.Contains(newAct))
        availableActions.Add(newAct);
    }

    protected void RemoveAction(Action toBeRemoved){
        availableActions.Remove(toBeRemoved);
    }

    protected void decreaseStamina(int decreaseVal){
        stamina -= decreaseVal;
        restGoal.importance += 0.07f * decreaseVal;
    }

    protected void decreaseHP(int decreaseVal)
    {
        hpPoints -= decreaseVal;
        if (hpPoints < 50){
            surviveGoal.importance += 3;
        }
        else{
            surviveGoal.importance -= 3;
            if (surviveGoal.importance < 0) surviveGoal.importance = 0;
        }
    }

    public void OnCollisionEnter(Collision collision){
    }

    public void OnCollisionExit(Collision collision)
    {
    }

    public void OnTriggerEnter(Collider col){
        if (col.gameObject.tag.Equals(enemyTag)){
            RemoveAction(regenerateAction);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag.Equals(enemyTag))
        {
            AddAction(regenerateAction);
        }
    }

    private class RegenerateAction : Action{
        private GoalOrientedCharacter parent;

        public RegenerateAction(ActionCallback listener, GoalOrientedCharacter par) : base(listener){
            parent = par;
        }

        private IEnumerator coroutine;

        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.TAKE_A_REST_NAME:
                    return -0.16f;
                case Goals.SURVIVE_NAME:
                    return -0.08f;
                default:
                    return 0f;
            }
        }

        public override void performAction(GameObject go){
            Log("");
            if(parent.stamina<100)
            parent.decreaseStamina(-4);
            if (parent.hpPoints < 100)
            parent.decreaseHP(-2);

            parent.StartCoroutine(WaitSomeTime(1));
        }


        private IEnumerator WaitSomeTime(float waitTime){
            yield return new WaitForSeconds(waitTime);
            callback.OnActionEnd(this);
        }
    }

    public int hp(){
        return hpPoints;
    }
}
