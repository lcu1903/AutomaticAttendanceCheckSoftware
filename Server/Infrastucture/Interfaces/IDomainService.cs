namespace Core.Interfaces;

public interface IDomainService: IScopedService
{
    bool Commit();
}