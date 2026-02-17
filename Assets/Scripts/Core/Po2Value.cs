
public readonly struct Po2Value
{
    public int Value { get; }

    public Po2Value(int value)
    {
        Value = value < 2 ? 2 : value;
    }

    public bool CanMergeWith(in Po2Value other) => Value == other.Value;

    public Po2Value Next() => new Po2Value(Value * 2);
}