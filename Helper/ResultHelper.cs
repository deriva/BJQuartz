using BJ.Quartz.Model;

namespace BJ.Quartz.Helpers
{
    public class ResultHelper<T> where T : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="totalCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ApiPageResult<T> ToPageResponse(List<T> data, int totalCount, int page, int pageSize)
        {
            var response = new ApiPageResult<T>
            {
                code = totalCount == 0 ? (int)StatusCodeType.Fail : (int)StatusCodeType.ApiSuccess,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = data 
            };
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<List<T>> ToSuccess(List<T> t = null)
        {
            var response = new ApiResult<List<T>>
            {
                code = (int)StatusCodeType.ApiSuccess,
                message = "成功",
                Data = t
            };
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<List<T>> ToFails(string msg, List<T> t = null)
        {
            var response = new ApiResult<List<T>>
            {
                code = (int)StatusCodeType.Fail,
                message = msg,
                Data = t
            };
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<T> ToSuccess(T t = null)
        {
            var response = new ApiResult<T>
            {
                code = (int)StatusCodeType.ApiSuccess,
                message = "成功",
                Data = t
            };
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<T> ToFail(string msg, T t = null)
        {
            var response = new ApiResult<T>
            {
                code = (int)StatusCodeType.Fail,
                message = msg,
                Data = t
            };
            return response;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResultHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult ToResponse(int result, string message = "", object t = null)
        {
            var response = new ApiResult
            {
                code = (int)(result < 1 ? StatusCodeType.Fail : StatusCodeType.ApiSuccess),
                message = (result < 1 ? "失败:" : "成功:") + message,
                attr = t
            };
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult ToResult(int code, string message = "", object t = null)
        {
            var response = new ApiResult
            {
                code = code,
                message = message,
                attr = t
            };
            return response;
        }
        /// <summary>
        /// ToSuccess
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult ToSuccess(object t = null)
        {
            var response = new ApiResult
            {
                code = (int)StatusCodeType.ApiSuccess,
                message = "成功",
                attr = t
            };
            return response;
        }

        /// <summary>
        /// ToSuccess
        /// </summary>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult ToSuccess(string message = "", object t = null)
        {
            var response = new ApiResult
            {
                code = (int)StatusCodeType.ApiSuccess,
                message = message,
                attr = t
            };
            return response;
        }

        /// <summary>
        /// ToFail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult ToFail(string message = "", object t = null)
        {
            var response = new ApiResult
            {
                code = (int)StatusCodeType.Fail,
                message = $"失败:{message}",
                attr = t
            };
            return response;
        }
    }
}
