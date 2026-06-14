namespace BookGuess.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string message) => new() { Success = false, Message = message };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }

    public static ApiResponse Ok() => new() { Success = true };
    public static ApiResponse Fail(string message) => new() { Success = false, Message = message };
}
