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
        public const string APP_ID = "appid";
        public const string CONTENT_KEY = "content";
        public const string METHOD = "method";
        public const string FORMAT = "format";
        public const string TIMESTAMP = "timestamp";
        public const string VERSION = "version";
        public const string CHARSET = "charset";
        public const string PROD_CODE = "prover";
        public const string ACCESS_TOKEN = "token";
        public const string SIGN_TYPE = "sign_type";
        public const string SIGN = "sign";
        public const string ENCRYPT_TYPE = "encrypt_type";

        private string format;
        private string version;
        private string charset;
        private string serverUrl;
        private string signType = "RSA";
        private string privateKey;
        private string graspPublicKey;

        private string encyptKey;
        private string encyptType = "AES";

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

        public DefaultHttpClient(string serverUrl) : this(serverUrl, string.Empty)
        {

        }
        public DefaultHttpClient(string serverUrl, string appid)
        {
            this.serverUrl = serverUrl;
            this.appId = appid;
        }

        public DefaultHttpClient(string serverUrl, string appid, string myPrivateKey, string graspPulicKey, string encyptKey)
        {
            this.serverUrl = serverUrl;
            this.appId = appid;
            this.privateKey = myPrivateKey;
            this.graspPublicKey = graspPulicKey;

            this.encyptKey = encyptKey;
            this.encyptType = "AES";
            this.webUtils = new WebUtils();
        }






        private string appId;

        /// <summary>
        /// 代理商APPID
        /// </summary>
        public string AppID
        {
            get { return appId; }
            set { appId = value; }
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

            // 序列化BizModel
            txtParams = SerializeModel(txtParams, request);
            txtParams.Add(METHOD, request.GetApiName());
            txtParams.Add(VERSION, apiVersion);
            txtParams.Add(APP_ID, appId);
            txtParams.Add(FORMAT, Format);
            txtParams.Add(TIMESTAMP, DateTime.Now);
            txtParams.Add(ACCESS_TOKEN, accessToken);
            txtParams.Add(SIGN_TYPE, signType);
            txtParams.Add(PROD_CODE, request.GetProdCode());
            txtParams.Add(CHARSET, charset);

            if (request.GetNeedEncrypt())
            {
                if (string.IsNullOrEmpty(txtParams[CONTENT_KEY]))
                {
                    throw new AopException("API 请求异常! 来源: 加密内容为空!");
                }

                if (string.IsNullOrEmpty(this.encyptKey) || string.IsNullOrEmpty(this.encyptType))
                {
                    throw new AopException("API 请求异常! 来源: 加密类型或加密Key不能为空");
                }

                if (!"AES".Equals(this.encyptType))
                {
                    throw new AopException("API暂时只支持AES加密");
                }
                string encryptContent = txtParams[CONTENT_KEY];// AopUtils.AesEncrypt(this.encyptKey, txtParams[BIZ_CONTENT], this.charset);
                txtParams.Remove(CONTENT_KEY);
                txtParams.Add(CONTENT_KEY, encryptContent);
                txtParams.Add(ENCRYPT_TYPE, this.encyptType);
            }

            // 添加签名参数AopUtils.SignAopRequest(txtParams, privateKeyPem, charset, this.keyFromFile, signType)
            txtParams.Add(SIGN, string.Empty);
            string body = webUtils.DoPost(this.serverUrl + "?" + CHARSET + "=" + this.charset, txtParams, this.charset);

            T rsp = null;
            IAopParser<T> parser = null;
            if ("xml".Equals(format))
            {
                throw new NotImplementedException("暂时不支持xml进行序列号!");
                //parser = new AopXmlParser<T>();
                //rsp = parser.Parse(body, charset);
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

        #region Model Serialize

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestParams"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private AopDictionary SerializeModel<T>(AopDictionary requestParams, IAopRequest<T> request) where T : AopResponse
        {
            AopDictionary result = requestParams;
            Boolean isBizContentEmpty = !requestParams.ContainsKey(CONTENT_KEY) || String.IsNullOrEmpty(requestParams[CONTENT_KEY]);
            if (isBizContentEmpty && request.GetModel() != null)
            {
                AopObject bizModel = request.GetModel();
                String content = Serialize(bizModel);
                result.Add(CONTENT_KEY, content);
            }
            return result;
        }

        /// <summary>
        /// AopObject序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private String Serialize(AopObject obj)
        {
            // 使用AopModelParser序列化对象
            AopModelParser parser = new AopModelParser();
            JsonObject jo = parser.serializeAopObject(obj);

            // 根据JsonObject导出String格式的Json
            String result = JsonConvert.ExportToString(jo);

            return result;
        }

        #endregion

    }
}
