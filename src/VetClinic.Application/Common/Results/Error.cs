using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Application.Common.Results;
public enum ErrorType
{
    Validation = 1,
    Conflict = 2,
    NotFound = 3,
}

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
}