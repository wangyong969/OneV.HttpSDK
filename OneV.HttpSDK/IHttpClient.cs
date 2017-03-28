using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneV.HttpSDK
{
    interface IHttpClient
    {
        /// <summary>
        /// 执行AOP公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IAopRequest<T> request) where T : AopResponse;

    }
}
