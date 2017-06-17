using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GoalOrientedCharacter : MonoBehaviour, ActionCallback{
    public int hpPoints = 100;
    public int stamina = 100;
    protected List<Goal> goals;
    protected List<Action> availableActions;

    public readonly float hpRegenerateDelaySeconds = 5;
    private float timeFromLastHpUpdate = 0;
    public readonly float staminaRegenerateDelaySeconds = 1;
    private float timeFromLastStaminaUpdate = 0;

    public void Awake(){
        goals = new List<Goal>();
        //Initial values, reflecting state where stamina is full, hp is full, and no enemy is in range.
        goals.Add(new Goals.RestGoal(0));
        goals.Add(new Goals.SurviveGoal(0));
        goals.Add(new Goals.FindEnemyGoal(3));
        goals.Add(new Goals.KillEnemyGoal(0));
        availableActions = new List<Action>();
        availableActions.Add(new GoalOrientedCharacter.RegenerateAction(this,this));;
    }

    protected void updateAction(){
        if (availableActions.Count == 0) return;
        Action bestAction = availableActions[0];
        bestAction.performAction(this.gameObject);
        Debug.Log("Choosing new best action.");
        Debug.Log("New best action: " + bestAction.GetType().Name);
    }

    private float calculateDiscontentment(Action a, List<Goal> goals){
        return 0;
    }

    public void Start(){
        updateAction();
    }

    public void Update(){
        timeFromLastHpUpdate += Time.deltaTime;
        if (timeFromLastHpUpdate > hpRegenerateDelaySeconds){
            timeFromLastHpUpdate = 0;
            if (hpPoints < 100){
                hpPoints += 2;
            }
        }
        timeFromLastStaminaUpdate += Time.deltaTime;
        if (timeFromLastStaminaUpdate > staminaRegenerateDelaySeconds){
            timeFromLastStaminaUpdate = 0;
            if (stamina < 100){
                stamina += 1;
            }
        }
    }

    public void OnActionEnd(){
        updateAction();
    }

    protected void AddAction(Action newAct){
        availableActions.Add(newAct);
    }

    protected void RemoveAction(Action toBeRemoved){
        availableActions.Remove(toBeRemoved);
    }


    public void OnCollisionEnter(Collision collision)
    {
    }

    public void OnTriggerEnter(Collider col)
    {

    }

    private class RegenerateAction :Action{
        private GoalOrientedCharacter parent;
        public RegenerateAction(ActionCallback listener, GoalOrientedCharacter par) : base(listener){
            parent = par;
        }
        private IEnumerator coroutine;
        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.TAKE_A_REST_NAME:
                    return 0.5f;
                    break;
                case Goals.SURVIVE_NAME:
                    return 0.1f;
                    break;
                default:
                    return 0f;
                    break;
            }
        }
        public override void performAction(GameObject go){
           Debug.Log("Performing action: " + ToString());
            parent.StartCoroutine(WaitSomeTime(1));
        }

        private IEnumerator WaitSomeTime(float waitTime)
        {
                yield return new WaitForSeconds(waitTime);
                callback.OnActionEnd();
        }
    }
}
