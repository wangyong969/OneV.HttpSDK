/*
 使用支付宝的SDK，改成自己需要的HTTP请求
 
 */
using Jayrock.Json;
using Jayrock.Json.Conversion;
using OneV.HttpSDK.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneV.HttpSDK
{
    public class DefaultHttpClient : IHttpClient
    {
        public const string CONTENT_KEY = "content";
        public const string METHOD = "method";
        public const string FORMAT = "format";
        public const string TIMESTAMP = "timestamp";
        public const string VERSION = "version";
        public const string CHARSET = "charset";

        private string format;
        private string version;
        private string charset;
        private string serverUrl;
        public string Ver
        {
            get { return version != null ? version : "1.0"; }
            set { version = value; }
        }
        public string Format
        {
            get { return format ?? "json"; }
            set { format = value; }
        }

        private WebUtils webUtils;

        public DefaultHttpClient(string serverUrl)
        {
            this.serverUrl = serverUrl;
            this.webUtils = new WebUtils();
        }



        public T Execute<T>(IAopRequest<T> request, string accessToken) where T : AopResponse
        {
            if (string.IsNullOrEmpty(this.charset))
            {
                this.charset = "utf-8";
            }
            string apiVersion = null;

            if (!string.IsNullOrEmpty(request.GetApiVersion()))
            {
                apiVersion = request.GetApiVersion();
            }
            else
            {
                apiVersion = Ver;
            }
            // 添加协议级请求参数
            AopDictionary txtParams = new AopDictionary(request.GetParameters());

            txtParams.Add(METHOD, request.GetApiName());
            txtParams.Add(VERSION, apiVersion);
            //txtParams.Add(APP_ID, appId);
            txtParams.Add(FORMAT, format);
            txtParams.Add(TIMESTAMP, DateTime.Now);
            //txtParams.Add(ACCESS_TOKEN, accessToken);
            //txtParams.Add(SIGN_TYPE, signType);
            //txtParams.Add(TERMINAL_TYPE, request.GetTerminalType());
            //txtParams.Add(TERMINAL_INFO, request.GetTerminalInfo());
            //txtParams.Add(PROD_CODE, request.GetProdCode());
            txtParams.Add(CHARSET, charset);
            // 添加签名参数
            //txtParams.Add(SIGN, AopUtils.SignAopRequest(txtParams, privateKeyPem, charset, this.keyFromFile, signType));
            string body = webUtils.DoPost(this.serverUrl + "?" + CHARSET + "=" + this.charset, txtParams, this.charset);

            T rsp = null;
            IAopParser<T> parser = null;
            if ("xml".Equals(format))
            {
                parser = new AopXmlParser<T>();
                rsp = parser.Parse(body, charset);
            }
            else
            {
                parser = new AopJsonParser<T>();
                rsp = parser.Parse(body, charset);
            }
            return rsp;
        }



        public T Execute<T>(IAopRequest<T> request) where T : AopResponse
        {
            return Execute<T>(request, null);
        }

        public void SetTimeout(int timeOut)
        {
            webUtils.Timeout = timeOut;
        }



    }
}
