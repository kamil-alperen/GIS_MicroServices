using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS.Common
{
    public enum ErrorType
    {
        Failure,
        NotFound,
        BadRequest,
        UnAuthorized
    }

    public class Error
    {
        public string Code { get; private set; }

        public string Desription { get; private set; }

        public ErrorType ErrorType { get; private set; }

        private Error(string code, string desription, ErrorType errorType)
        {
            Code = code;
            Desription = desription;
            ErrorType = errorType;
        }

        public static Error FailureError(string code, string description) => new Error(code, description, ErrorType.Failure);

        public static Error NotFoundError(string code, string description) => new Error(code, description, ErrorType.NotFound);

        public static Error BadRequestError(string code, string description) => new Error(code, description, ErrorType.BadRequest);

        public static Error UnAuthorizedError(string code, string description) => new Error(code, description, ErrorType.UnAuthorized);
    }

    public class Result<T>
    {
        public T? Value { get; private set; }

        public Error? Error { get; private set; }

        private Result(T? value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(Error? error)
        {
            IsSuccess = false;
            Error = error;
        }

        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;

        public static implicit operator Result<T>(T? value) => new Result<T>(value);

        public static implicit operator Result<T>(Error? error) => new Result<T>(error);

        public static Result<T> Success(T? value) => new Result<T>(value);
        public static Result<T> Failure(Error? error) => new Result<T>(error);
    }
}
