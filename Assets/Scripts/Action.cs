

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface ActionCallback{
    void OnActionEnd();
}

public abstract class Action{
    protected readonly ActionCallback callback;
    public Action(ActionCallback listener){
        callback = listener;
    }

    public abstract float getGoalChange(Goal g);
    public abstract void performAction(GameObject go);
    public override string ToString(){
        return this.GetType().Name;
    }
}
