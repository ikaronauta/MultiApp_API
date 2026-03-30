// Models/ApiResponse.cs

public class ApiResponse<T>
{
    public string Status { get; set; } = string.Empty;

    public T Data { get; set; } = default!;

    public string Message { get; set; } = string.Empty;

    public string? Error { get; set; }
}