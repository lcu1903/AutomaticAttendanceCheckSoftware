namespace Core.Models;

public abstract class Entity
{
    public static bool operator ==(Entity a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity? b)
    {
        return !(a == b);
    }
}