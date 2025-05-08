namespace MinhaCarteira.API.DTOs
{
  public class ApiResponse<T>
  {
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> CreateSuccess(T? data, string message = "")
    {
      return new ApiResponse<T>
      {
        Success = true,
        Message = message,
        Data = data,
        Errors = new List<string>()
      };
    }

    public static ApiResponse<T> CreateError(string message, List<string>? errors = null)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Message = message,
        Data = default,
        Errors = errors ?? new List<string>()
      };
    }
  }
}