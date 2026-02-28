namespace WinStudentGoalTracker.Models;

public class ResponseResult<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }


    public static ResponseResult<object> SuccessMessage(string message)
    {
        return new ResponseResult<object>
        {
            Success = true,
            Message = message,
            Data = null
        };
    }

    public static ResponseResult<object> FailureMessage(string message)
    {
        return new ResponseResult<object>
        {
            Success = false,
            Message = message,
            Data = null
        };
    }


}

public class EmptyResponse { }


public class ResponseResult
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public EmptyResponse? Data { get; set; } = new EmptyResponse();
    
    public static ResponseResult SuccessMessage(string message)
    {
        return new ResponseResult
        {
            Success = true,
            Message = message,
            Data = new EmptyResponse()
        };
    }

    public static ResponseResult FailureMessage(string message)
    {
        return new ResponseResult
        {
            Success = false,
            Message = message,
            Data = new EmptyResponse()
        };
    }
}


