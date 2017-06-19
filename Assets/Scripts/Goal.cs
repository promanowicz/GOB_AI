


public abstract class Goal{
    public string name;

    private float Importance;
    public float importance
    {
        get { return Importance; }
        set
        {
            this.Importance = value;
            if (this.Importance < 0) this.Importance = 0;
        }
    }

    //square of importance
    protected Goal(string name, float importance){
        this.name = name;
        this.importance = importance;
    }

    public abstract float getDiscontentment(float newImportanceValue);
}
