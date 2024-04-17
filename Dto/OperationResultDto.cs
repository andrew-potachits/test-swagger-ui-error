namespace testSwaggerUIError.Dto;

public class OperationResultDto 
{
    public bool Success { get; set; }
    public virtual object Result { get; set; }
    public ResponseExtras Extras { get; set; }

    public static OperationResultDto Succeeded = new() { Success = true };
    public static OperationResultDto Failed = new() { Success = false };
}

public class OperationResultDto<T> 
{
    public bool Success { get; set; }
    public T Result { get; set; }
    public ResponseExtras Extras { get; set; }

    public static OperationResultDto<T> Failed(T res = default) => new() { Success = false, Result = res };
    public static OperationResultDto<T> Succeeded(T res) => new() { Success = true, Result = res };
}


public class ResponseExtras
{
    public uint? UndoCount { get; set; }
    public uint? RedoCount { get; set; }
    public bool? HasMessages { get; set; }
    public Kind? Kind { get; set; }
}