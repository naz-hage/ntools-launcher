using System.Collections.Generic;

namespace Ntools
{
    /// <summary>
    /// Represents a helper class for handling results
    /// </summary>
    public class ResultHelper
    {
        /// <summary>
        /// Predefined result codes
        /// </summary>
        public static readonly int SuccessCode = 0;
        public static readonly int InvalidParameter = -1;
        public static readonly int FileNotFound = -3;
        public static readonly int Exception = int.MaxValue;

        /// <summary>
        /// Predefined result messages
        /// </summary>
        protected const string SuccessMessage = "Success";
        private const string UndefinedMessage = "Undefined";
        private const string FailMessage = "Fail";

        /// <summary>
        /// The result code
        /// </summary>
        public int Code { get; set; } = Exception;

        /// <summary>
        /// The result output
        /// </summary>
        public List<string> Output { get; set; } = new List<string>();

        /// <summary>
        /// Checks if the result is a success
        /// </summary>
        /// <returns>True if the result is a success, otherwise false</returns>
        public bool IsSuccess()
        {
            return Code == SuccessCode;
        }

        /// <summary>
        /// Checks if the result is a failure
        /// </summary>
        /// <returns>True if the result is a failure, otherwise false</returns>
        public bool IsFail()
        {
            return Code != SuccessCode;
        }

        /// <summary>
        /// Gets the first output message or an undefined message if there are no output messages
        /// </summary>
        /// <returns>The first output message or an undefined message</returns>
        public string GetFirstOutput()
        {
            return Output.Count > 0 ? Output[0] : UndefinedMessage;
        }

        /// <summary>
        /// Creates a new success result with an optional message
        /// </summary>
        /// <param name="message">The optional message</param>
        /// <returns>A new success result</returns>
        public static ResultHelper Success(string message = SuccessMessage)
        {
            var result = new ResultHelper
            {
                Code = SuccessCode,
                Output = new List<string> { message }
            };
            return result;
        }

        /// <summary>
        /// Creates a new failure result with an optional code and message
        /// </summary>
        /// <param name="code">The optional code</param>
        /// <param name="message">The optional message</param>
        /// <returns>A new failure result</returns>
        public static ResultHelper Fail(int code = int.MinValue, string message = FailMessage)
        {
            var result = new ResultHelper
            {
                Code = code,
                Output = new List<string> { message }
            };
            return result;
        }

        /// <summary>
        /// Creates a new ResultHelper with default values
        /// </summary>
        /// <returns>A new ResultHelper with default values</returns>
        public static ResultHelper New()
        {
            return new ResultHelper
            {
                Code = Exception,
                Output = new List<string>()
            };
        }
    }
}
