﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneV.HttpSDK
{
    public interface IAopRequest<T> where T : AopResponse
    {

        /// <summary>
        /// 获取AOP的异步通知地址。
        /// </summary>
        /// <returns>异步通知地址</returns>
        string GetNotifyUrl();

        /// <summary>
        /// 设置AOP的异步通知地址。
        /// </summary>
        /// <returns>异步通知地址</returns>
        void SetNotifyUrl(string notifyUrl);

        /// <summary>
        /// 设置请求是否需要加密
        /// </summary>
        void SetNeedEncrypt(bool needEncrypt);

        /// <summary>
        /// 获取AOP请求是否需要加密
        /// </summary>
        /// <returns>结果是否加密</returns>
        bool GetNeedEncrypt();

        /// <summary>
        /// 获取AOP的API名称。
        /// </summary>
        /// <returns>API名称</returns>
        string GetApiName();


        /// <summary>
        /// 获取产品码。
        /// </summary>
        /// <returns>产品码</returns>
        string GetProdCode();

        /// <summary>
        /// 设置产品码。
        /// </summary>
        /// <returns>产品码</returns>
        void SetProdCode(string prodCode);

        /// <summary>
        /// 设置接口版本
        /// </summary>
        void SetApiVersion(string apiVersion);


        /// <summary>
        /// 返回接口版本
        /// </summary>
        /// <returns>接口版本</returns>
        string GetApiVersion();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 请求参数文本值
        /// </summary>
        /// <returns>文本请求参数字典</returns>
        IDictionary<string, string> GetParameters();
        /// <summary>
        /// 获取BizModel
        /// </summary>
        /// <returns></returns>
        AopObject GetModel();

    }
}
