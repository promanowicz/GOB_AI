using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GoalOrientedCharacter : MonoBehaviour, ActionCallback{
    //MOVABLE CHARACTER
    public bool isLoggable = false;
    public bool logDetails = false;
    //
    [SerializeField] private int hpPoints = 100;
    [SerializeField] private int stamina = 100;
    public string enemyTag;
    protected List<Goal> goals;
    protected List<Action> availableActions;
    protected Goal restGoal = new Goals.RestGoal(1);
    protected Goal surviveGoal = new Goals.SurviveGoal(0);
    protected Goal FindEnemyGoal = new Goals.FindEnemyGoal(2.5f);
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
        if (currentlyRunningAction != null || availableActions.Count == 0) return;

        Action bestAction = availableActions[0];
        float bestDiscontentment = calculateDiscontentment(availableActions[0], goals);
        if (isLoggable){
            StringBuilder builder = new StringBuilder();
            builder.Append(" restGoal: ".PadRight(16) + restGoal.importance);
            builder.Append(" surviveGoal: ".PadRight(16) + surviveGoal.importance);
            builder.Append(" FindEnemyGoal: ".PadRight(16) + FindEnemyGoal.importance);
            builder.Append(" killEnemyGoal: ".PadRight(16) + killEnemyGoal.importance);
            Debug.Log(builder);
            if (logDetails){
                Debug.Log("________________________________________");
            }
        }

        StringBuilder builder1 = new StringBuilder("Available actions: ");
        foreach (var VARIABLE in availableActions){
            builder1.Append(VARIABLE.GetType().Name + " ");
            float newDiscontentment = calculateDiscontentment(VARIABLE, goals);
            if (isLoggable && logDetails)
                Debug.Log(VARIABLE.ToString() + " discontentment: " + newDiscontentment);

            if (newDiscontentment <= bestDiscontentment){
                bestAction = VARIABLE;
                bestDiscontentment = newDiscontentment;
            }
        }
        if (isLoggable){
            Debug.Log(builder1);
            if (logDetails)
                Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        }

        currentlyRunningAction = bestAction;
        bestAction.performAction(this.gameObject, isLoggable);
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
        if (!availableActions.Contains(newAct))
            availableActions.Add(newAct);
    }

    protected void RemoveAction(Action toBeRemoved){
        availableActions.Remove(toBeRemoved);
    }

    protected void decreaseStamina(int decreaseVal){
        stamina -= decreaseVal;
        restGoal.importance += 0.07f * decreaseVal;
    }

    protected void decreaseHP(int decreaseVal){
        hpPoints -= decreaseVal;
        if (hpPoints < 50){
            surviveGoal.importance = 4;
        }
        else{
            surviveGoal.importance = 0.8f;
        }
    }

    public void OnCollisionEnter(Collision collision){
    }

    public void OnCollisionExit(Collision collision){
    }

    public void OnTriggerEnter(Collider col){
        if (col.gameObject.tag.Equals(enemyTag)){
            RemoveAction(regenerateAction);
            if (isLoggable) Debug.Log("Enemy entered");
        }
    }

    public void OnTriggerExit(Collider col){
        if (col.gameObject.tag.Equals(enemyTag)){
            AddAction(regenerateAction);
            if (isLoggable) Debug.Log("Enemy exited");
        }
    }

    public void OnTriggerStay(Collider col){
        if (col.gameObject.tag.Equals(enemyTag)){
            if (isLoggable) Debug.Log("Enemy stays");
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

        public override void performAction(GameObject go, bool isLoggable){
            if (isLoggable)
                Log("");
            if (parent.stamina < 100)
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
