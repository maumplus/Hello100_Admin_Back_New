namespace Hello100Admin.BuildingBlocks.Common.Errors;

public class ApiSuccessResponse<T>
{
    public bool Success { get; set; } = true;
    public T Data { get; set; }

    public ApiSuccessResponse(T data)
    {
        Success = true;
        Data = data;
    }
}
