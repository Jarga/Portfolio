using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System;

namespace PortfolioOne.Core.Utilities.Filters
{
    public class GoogleRecaptchaAttribute : ActionFilterAttribute
    {
        private const string RESPONSE_FIELD_KEY = "g-recaptcha-response";
        private GoogleRecaptchaResponse _googleRecaptchaResponse;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GoogleRecaptchaValidator googleRecaptchaValidator = new GoogleRecaptchaValidator
            {
                RemoteIp = GetIPAddress(filterContext.HttpContext.Request),
                Response = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY]
            };
            this._googleRecaptchaResponse = googleRecaptchaValidator.Validate();

            filterContext.ActionArguments["captchaValid"] = _googleRecaptchaResponse.IsValid;
            filterContext.ActionArguments["captchaErrorMessages"] = _googleRecaptchaResponse.ErrorMessages;
            base.OnActionExecuting(filterContext);
        }
        public string GetIPAddress(HttpRequest httpRequest)
        {
            string ip = httpRequest.Headers["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
                ip = httpRequest.Headers["REMOTE_ADDR"];
            else
                ip = ip.Split(',')[0];

            return ip;
        }
    }

    public class GoogleRecaptchaValidator
    {
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        public static string Secret = ConfigurationManager.AppSettings["RecaptchaSecret"];
        public static string SiteKey = ConfigurationManager.AppSettings["RecaptchaSiteKey"];
        public string Response { get; set; }

        private string _remoteIp;
        public string RemoteIp
        {
            get
            {
                return this._remoteIp;
            }
            set
            {
                IPAddress ipAddress = IPAddress.Parse(value);
                if (ipAddress == null || ipAddress.AddressFamily != AddressFamily.InterNetwork && ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
                    throw new ArgumentException("Expecting an IP address, got " + (object)ipAddress);
                this._remoteIp = ipAddress.ToString();
            }
        }

        public GoogleRecaptchaResponse Validate()
        {
            if (string.IsNullOrEmpty(Secret))
            {
                return new GoogleRecaptchaResponse() { ErrorMessages = new[] { GoogleRecaptchaResponse.MissingInputSecret } };
            }

            if (string.IsNullOrEmpty(this.Response))
            {
                return new GoogleRecaptchaResponse() { ErrorMessages = new[] { GoogleRecaptchaResponse.MissingInputResponse } };
            }

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(VerifyUrl);
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.Timeout = 30000;
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "reCAPTCHA/ASP.NET";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = Encoding.ASCII.GetBytes(string.Format("secret={0}{1}&response={2}", HttpUtility.UrlEncode(Secret), string.IsNullOrEmpty(RemoteIp) ? "" : "&remoteip=" + HttpUtility.UrlEncode(RemoteIp), HttpUtility.UrlEncode(Response)));

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                string responseJson;
                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    using (TextReader textReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseJson = textReader.ReadToEnd();
                    }
                }

                //TODO: If this ends up being too volatile we should just use a json deserializer
                const string successText = "\"success\":";
                const string errorCodesText = "\"error-codes\":";

                int successStart = responseJson.IndexOf(successText, StringComparison.Ordinal) + successText.Length;
                int errorCodesArrayStart = responseJson.IndexOf(errorCodesText, StringComparison.Ordinal);
                int errorCodesStart = errorCodesArrayStart > -1 ? responseJson.IndexOf("[", errorCodesArrayStart + errorCodesText.Length, StringComparison.Ordinal) + 1 : -1;

                string successResult = responseJson.Substring(successStart, responseJson.IndexOf(errorCodesArrayStart > -1 ? "," : "\n", successStart, StringComparison.Ordinal) - successStart).Trim();
                string[] errorCodes = errorCodesStart > -1 ? responseJson.Substring(errorCodesStart, responseJson.LastIndexOf("]", errorCodesStart, StringComparison.Ordinal) - errorCodesStart).Split(',') : new string[0];

                return GoogleRecaptchaResponse.Create(successResult.Equals("true"), errorCodes.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim()).ToArray());
            }
            catch (WebException)
            {
                return new GoogleRecaptchaResponse() { ErrorMessages = new[] { GoogleRecaptchaResponse.RecaptchaNotReachable } };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new GoogleRecaptchaResponse() { ErrorMessages = new[] { GoogleRecaptchaResponse.FailedToParseResponse } };
            }
        }
    }

    public class GoogleRecaptchaResponse
    {
        public static readonly string MissingInputSecret = "Invalid reCAPTCHA request. The secret parameter is missing.";
        public static readonly string InvalidInputSecret = "Invalid reCAPTCHA request. The secret parameter is invalid or malformed.";
        public static readonly string MissingInputResponse = "Invalid reCAPTCHA request. The response parameter is missing.";
        public static readonly string InvalidInputResponse = "Invalid reCAPTCHA request. The response parameter is invalid or malformed.";
        public static readonly string RecaptchaNotReachable = "The reCAPTCHA server is unavailable.";
        public static readonly string FailedToParseResponse = "The reCAPTCHA server returned an unreadable response.";

        public bool IsValid { get; set; }

        public string[] ErrorMessages { get; set; }

        public static GoogleRecaptchaResponse Create(bool isValid, string[] errorCodes)
        {
            if (isValid || errorCodes == null || errorCodes.Length < 1)
            {
                return new GoogleRecaptchaResponse() { IsValid = true };
            }
            List<string> results = new List<string>();
            foreach (string errorCode in errorCodes)
            {
                switch (errorCode)
                {
                    case "missing-input-secret":
                        results.Add(MissingInputSecret);
                        break;
                    case "invalid-input-secret":
                        results.Add(InvalidInputSecret);
                        break;
                    case "missing-input-response":
                        results.Add(MissingInputResponse);
                        break;
                    case "invalid-input-response":
                        results.Add(InvalidInputResponse);
                        break;
                    default:
                        results.Add(errorCode);
                        break;
                }
            }
            return new GoogleRecaptchaResponse() { ErrorMessages = results.ToArray() };
        }
    }
}