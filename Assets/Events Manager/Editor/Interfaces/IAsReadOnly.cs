

public interface IAsReadOnly<T>
{
    T AsReadOnly { get; }
    bool IsReadOnly { get; }
}