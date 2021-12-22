public class MainType
{
    public float[] Timers { get; } = new float[4]; // defaultEndless, defualtTimer, changesE1, changesE2
    protected float[] _minMax = new float[2]; // minimumPercent, maximumPercent

    public MainType(float[] timers, float[] minMax)
    {
        Timers = timers;
        _minMax = minMax;
    }

    public virtual void NextLevel()
    {
    }

    public virtual void RepeatLevel()
    {
    }
}
