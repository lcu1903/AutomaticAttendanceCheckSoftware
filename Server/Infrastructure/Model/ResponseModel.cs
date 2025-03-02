namespace Infrastructure.Models;
public class ResponseModel<T> 
{
    public bool Success {get; set;}
    public IEnumerable<ErrorMessage>? Errors {get; set;}
    public T? Data {get; set;}
}

public class ErrorMessage
{
    public string Key { get; set; } = null!;
    public string? Description { get; set; }
}