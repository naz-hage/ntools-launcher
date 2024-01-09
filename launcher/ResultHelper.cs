using System.Collections.Generic;

namespace Launcher
{
    public class ResultHelper
    {
        public static readonly int SuccessCode = 0;
        public static readonly int InvalidParameter = -1;
        public static readonly int FileNotFound = -3;
        public static readonly int Exception = int.MaxValue;

        private const string SuccessMessage = "Success";
        private const string UndefinedMessage = "Undefined";
        private const string FailMessage = "Fail";

        public int Code { get; set; } = Exception;
        public List<string> Output { get; set; } = new List<string>() { UndefinedMessage };


        public bool IsSuccess()
        {
            return Code == SuccessCode;
        }

        public bool IsFail()
        {
            return Code != SuccessCode;
        }
        public static ResultHelper Success(string message = SuccessMessage)
        {
            var result = new ResultHelper
            {
                Code = SuccessCode,
                Output =
                {
                    [0] = message
                }
            };
            return result;
        }

        public static ResultHelper Fail(int code = int.MinValue, string message = FailMessage)
        {
            var result = new ResultHelper
            {
                Code = code,
                Output =
                {
                    [0] = message
                }
            };
            return result;
        }

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
