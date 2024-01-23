using System.Collections.Generic;

namespace Launcher
{
    // Represents a helper class for handling results
    public class ResultHelper
    {
        // Predefined result codes
        public static readonly int SuccessCode = 0;
        public static readonly int InvalidParameter = -1;
        public static readonly int FileNotFound = -3;
        public static readonly int Exception = int.MaxValue;

        // Predefined result messages
        private const string SuccessMessage = "Success";
        private const string UndefinedMessage = "Undefined";
        private const string FailMessage = "Fail";

        // The result code
        public int Code { get; set; } = Exception;

        // The result output
        public List<string> Output { get; set; } = new List<string>() { UndefinedMessage };

        // Checks if the result is a success
        public bool IsSuccess()
        {
            return Code == SuccessCode;
        }

        // Checks if the result is a failure
        public bool IsFail()
        {
            return Code != SuccessCode;
        }

        // Gets the first output message or an undefined message if there are no output messages
        public string GetFirstOutput()
        {
            return Output.Count > 0 ? Output[0] : UndefinedMessage;
        }

        // Creates a new success result with an optional message
        public static ResultHelper Success(string message = SuccessMessage)
        {
            var result = new ResultHelper
            {
                Code = SuccessCode,
                Output = new List<string> { message }
            };
            return result;
        }

        // Creates a new failure result with an optional code and message
        public static ResultHelper Fail(int code = int.MinValue, string message = FailMessage)
        {
            var result = new ResultHelper
            {
                Code = code,
                Output = new List<string> { message }
            };
            return result;
        }

        // Creates a new result with default values.  This is redundant with the default constructor.
        public static ResultHelper New()
        {
            return new ResultHelper
            {
                Code = Exception,
                Output = new List<string> ()
            };
        }
    }
}
