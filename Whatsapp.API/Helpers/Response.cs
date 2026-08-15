using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Whatsapp.API.Helpers;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public bool IsFail => !IsSuccess;
    public T Data { get; set; }
    public string? Meassage { get; set; }
    public string? Error { get; set; }

    public Response(bool isSuccess, T data, string? error = null, string? message = null)
    {
        this.IsSuccess = isSuccess;
        this.Data = data;
        this.Error = error;
        this.Meassage = message;
    }
    
    public static Response<T> Success(T data,string?message="")
        => new  Response<T>(true,data,null,message);
    
    public static Response<T> Fail(string? message)
    => new Response<T>(false,default(T),null,message);
}