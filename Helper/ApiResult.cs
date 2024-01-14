/*
* ==============================================================================
*
* FileName: ApiResult.cs
* Created: 2020/3/26 13:52:51
* Author: Meiam
* Description: 
*
* ==============================================================================
*/

namespace BJ.Quartz.Model
{
    /// <summary>
    /// 统一接口返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ApiResult
    {
        public ApiResult()
        {
            code = 0;
        }

        /// <summary>
        /// 请求状态
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回时间戳
        /// </summary>
        public string TimeStamp { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        /// <summary>
        /// 自定义属性
        /// </summary>
        public object attr { get; set; }


    }
    public class TextAttribute : Attribute
    {
        public TextAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    public enum StatusCodeType
    {
        /// <summary>
        /// 请求(或处理)成功  请用code=100
        /// </summary>
        [Text("请求(或处理)成功")]
        Success = 200,

        ///// <summary>
        ///// 内部请求出错
        ///// </summary>
        //[Text("内部请求出错")]
        //InnerError = 500,

        ///// <summary>
        ///// 访问请求未授权! 当前 SESSION 失效, 请重新登陆
        ///// </summary>
        //[Text("访问请求未授权! 当前 SESSION 失效, 请重新登陆")]
        //Unauthorized = 401,

        /// <summary>
        /// 请求参数不完整或不正确
        /// </summary>
        [Text("请求参数不完整或不正确")]
        ParameterError = 400,

        ///// <summary>
        ///// 您无权进行此操作，请求执行已拒绝
        ///// </summary>
        //[Text("您无权进行此操作，请求执行已拒绝")]
        //Forbidden = 403,

        ///// <summary>
        ///// 找不到与请求匹配的 HTTP 资源
        ///// </summary>
        //[Text("找不到与请求匹配的 HTTP 资源")]
        //NotFound = 404,

        ///// <summary>
        ///// HTTP请求类型不合法
        ///// </summary>
        //[Text("HTTP请求类型不合法")]
        //HttpMehtodError = 405,

        ///// <summary>
        ///// HTTP请求不合法,请求参数可能被篡改
        ///// </summary>
        //[Text("HTTP请求不合法,请求参数可能被篡改")]
        //HttpRequestError = 406,

        ///// <summary>
        ///// 该URL已经失效
        ///// </summary>
        //[Text("该URL已经失效")]
        //URLExpireError = 407,

        ///// <summary>
        ///// 已过期
        ///// </summary>
        //[Text("已过期")]
        //Expire = 408,

        /// <summary>
        /// 请求成功
        /// </summary>
        [Text("请求成功")]
        ApiSuccess = 100,

        /// <summary>
        /// 请求失败
        /// </summary>
        [Text("请求失败")]
        Fail = 101,

        /// <summary>
        /// 请求错误
        /// </summary>
        [Text("请求错误")]
        Error = -1,

        /// <summary>
        /// 未登录
        /// </summary>
        [Text("未登录")]
        NotLogin = 102,

        /// <summary>
        /// 时间戳已过期
        /// </summary>
        [Text("时间戳已过期")]
        TimeStampExipre = 103,

        /// <summary>
        /// 访问评率太高
        /// </summary>
        [Text("访问评率太高")]
        FrequencyHigh = 104,

        /// <summary>
        /// Token失效
        /// </summary>
        [Text("Token失效")]
        TokenLost = 105,

        /// <summary>
        /// 程序错误
        /// </summary>
        [Text("程序错误")]
        ProgramError = 106,

        /// <summary>
        /// 未授权
        /// </summary>
        [Text("未授权")]
        PowerLost = 107,
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        public ApiResult() : this("", StatusCodeType.Error, default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="content"></param>
        public ApiResult(string message, StatusCodeType code, T? data)
        {
            this.message = message;
            this.code = (int)code;
            attr = data;
        }

        /// <summary>
        /// ToSuccess
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult<T> ToSuccess(string message, T? data)
        {
            var response = new ApiResult<T>()
            {
                code = (int)StatusCodeType.ApiSuccess,
                message = message,
                attr = data,
            };
            return response;
        }

        /// <summary>
        /// ToFail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<T> ToFail(string message, T data)
        {
            var response = new ApiResult<T>()
            {
                code = (int)StatusCodeType.Fail,
                message = message,
                attr = data
            };
            return response;
        }

        /// <summary>
        /// ToProgramError
        /// </summary>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ApiResult<T> ToProgramError(string message, T data)
        {
            var response = new ApiResult<T>()
            {
                code = (int)StatusCodeType.ProgramError,
                message = message,
                attr = data
            };
            return response;
        }

        /// <summary>
        /// 接口返回值
        /// </summary>
        public T Data;

    }

    /// <summary>
    /// 返回带分页的Model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiPageResult<T> : ApiResult
    {
        /// <summary>
        /// 分页索引
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return Page > 0; }
        }
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get { return Page + 1 < TotalPages; }
        }

        public List<T> Items { get; set; }

        public object TotalField { get; set; }
    }
}
