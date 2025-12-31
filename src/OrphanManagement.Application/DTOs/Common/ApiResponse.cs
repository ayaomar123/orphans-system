namespace OrphanManagement.Application.DTOs.Common;

/// <summary>
/// Standard API response wrapper for consistent response structure.
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Response message (success or error message)
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// The actual data payload
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// List of validation errors (if any)
    /// </summary>
    public List<string>? Errors { get; set; }
    
    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
    
    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
