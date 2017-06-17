


public abstract class Goal{
    public string name;
    public float importance;
    //square of importance
    protected Goal(string name, float importance){
        this.name = name;
        this.importance = importance;
    }

    public abstract float getDiscontentment(float newImportanceValue);
}
