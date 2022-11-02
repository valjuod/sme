namespace Sme.BankingApi.Commands.Common
{
    public class CommandResult
    {        
        public bool Success { get; }

        public List<ResultError> Errors { get; }

        protected CommandResult()
        {
            Errors = new List<ResultError>();
        }

        protected CommandResult(bool success) : this()
        {
            Success = success;
        }

        public void AddError(string code, string message)
        {
            Errors.Add(new ResultError(code, message));
        }

        public void AddError(string message)
        {
            Errors.Add(new ResultError(message));
        }

        public void AddErrors(List<ResultError> errors)
        {
            Errors.AddRange(errors);
        }

        public static CommandResult<TResult> Ok<TResult>(TResult data) => new CommandResult<TResult>(data);

        public static CommandResult<TResult> Failed<TResult>(string code, string message)
        {
            var result = new CommandResult<TResult>(false);
            result.AddError(code, message);
            return result;
        }

        public static CommandResult<TResult> Failed<TResult>(string message)
        {
            var result = new CommandResult<TResult>(false);
            result.AddError(message);
            return result;
        }

        public static CommandResult<TResult> Failed<TResult>(List<ResultError> errors)
        {
            var result = new CommandResult<TResult>(false);
            result.AddErrors(errors);
            return result;
        }
        public static CommandResult<TResult> Failed<TResult>(Exception e)
        {
            return Failed<TResult>(e.Message + " " + e.StackTrace);
        }
    }

    public class CommandResult<T> : CommandResult
    {
        public T Data { get; }

        public CommandResult(bool success) : base(success) { }

        /// <summary>
        /// Automatically set Success to TRUE
        /// </summary>
        /// <param name="data"></param>
        public CommandResult(T data) : base(true)
        {
            Data = data;
        }
    }

    public class ResultError
    {
        public string Code { get; }

        public string Message { get; }

        public ResultError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public ResultError(string message)
        {
            Message = message;
        }
    }
}
