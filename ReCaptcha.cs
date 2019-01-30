using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace System.Web
{
	public static class ReCaptcha
    {
		/// <summary>
		/// Your secret key. Stays on the server only!
		/// </summary>
		public static string SecretKey { private get; set; }

		/// <summary>
		/// Googlesez: Optional. The name of your callback function to be executed once all the dependencies have loaded.
		/// </summary>
		public static string OnLoad { get; set; }

		/// <summary>
		/// Googlesez: Optional. Whether to render the widget explicitly. Defaults to onload, which will render the widget in the first g-recaptcha tag it finds. NOTE: "explicit" is a C# keyword.
		/// </summary>
		public static RenderOptions Render { get; set; } = RenderOptions.renderOnload;

		/// <summary>
		/// Googlesez: Your sitekey.
		/// </summary>
		public static string DataSiteKey { get; set; }

		/// <summary>
		/// Googlesez: Optional. The color theme of the widget.
		/// </summary>
		public static ThemeOptions DataTheme { get; set; } = ThemeOptions.light;

		/// <summary>
		/// Googlesez: Optional. The type of CAPTCHA to serve.
		/// </summary>
		public static TypeOptions DataType { get; set; } = TypeOptions.image;

		/// <summary>
		/// Googlesez: Optional. The size of the widget.
		/// </summary>
		public static SizeOptions DataSize { get; set; } = SizeOptions.normal;

		/// <summary>
		/// Googlesez: Optional. The tabindex of the widget and challenge. If other elements in your page use tabindex, it should be set to make user navigation easier.
		/// </summary>
		public static int DataTabindex { get; set; } = 0;

		/// <summary>
		/// Googlesez: Optional. The name of your callback function to be executed when the user submits a successful CAPTCHA response. The user's response, g-recaptcha-response, will be the input for your callback function.
		/// </summary>
		public static string DataCallback { get; set; }

		/// <summary>
		/// Googlesez: Optional. The name of your callback function to be executed when the recaptcha response expires and the user needs to solve a new CAPTCHA.
		/// </summary>
		public static string DataExpiredCallback { get; set; }

		/// <summary>
		/// Generate the script tag to be inserted in the document's head.
		/// </summary>
		/// <param name="LanguageCode">The language of your CAPTCHA. https://developers.google.com/recaptcha/docs/language </param>
		/// <returns>The script tag.</returns>
		public static HtmlString GenerateScript(string LanguageCode = "")
		{
			bool containsParams = false;
			string result = "<script src='" + ScriptApiUrl;

			if (!string.IsNullOrEmpty(OnLoad))
			{
				result += "?onload=" + OnLoad;
				containsParams = true;
			}				

			if (Render == RenderOptions.renderExplicit)
			{
				if (!containsParams)
					result += "?";
				else result += "&";
				result += "render=explicit";
			}

			if (!string.IsNullOrEmpty(LanguageCode))
			{
				if (!containsParams)
					result += "?";
				else result += "&";
				result += "hl=" + LanguageCode;
			}

			result += "'></script>";

			return new HtmlString(result);
		}

		/// <summary>
		/// Generate the Widget's code.
		/// </summary>
		public static HtmlString Widget
		{
			get
			{
				string result = "<div class='g-recaptcha' data-sitekey='" + DataSiteKey + "'";

				if (DataTheme == ThemeOptions.dark)
					result += " data-theme='dark'";

				if (DataType == TypeOptions.audio)
					result += " data-type='audio'";

				if (DataSize == SizeOptions.compact)
					result += " data-size='compact'";

				if (DataTabindex != 0)
					result += " data-tabindex='" + DataTabindex.ToString() + "'";

				if (!string.IsNullOrEmpty(DataCallback))
					result += " data-callback='" + DataCallback + "'";

				if (!string.IsNullOrEmpty(DataExpiredCallback))
					result += " data-expired-callback='" + DataExpiredCallback + "'";

				result += "></div>";

				return new HtmlString(result);
			}
		}

		/// <summary>
		/// Verify the CATPCHA with Google.
		/// </summary>
		public static Reply CurrentReply
		{
			get
			{
				int attempt = 0;
				try
				{
					string result;
					using (var client = new WebClient())
					{
						client.Encoding = Encoding.UTF8;
						client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						result = client.UploadString(VerifyApiUrl, parameters);
					}

					Reply reply;

					using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
					{
						var ser = new DataContractJsonSerializer(typeof(Reply));
						reply = ser.ReadObject(stream) as Reply;
					}

					return reply;
				}

				catch (Exception ex)
				{
					if (attempt < TimeoutAttempts)
					{
						attempt++;
						return CurrentReply;
					}

					else
						return new Reply() { Success = false, ErrorCodes = new string[] { "Request timed out on server. " + ex.Message } };
				}
			}
		}

		// UNDOCUMENTED TWEAKING

		public static string VerifyApiUrl = "https://www.google.com/recaptcha/api/siteverify";
		public static string ScriptApiUrl = "https://www.google.com/recaptcha/api.js";
		public static string ResponseKey = "g-recaptcha-response";
		public static bool VerifyIp = true;
		public static int TimeoutAttempts { private get; set; } = 10;

		// PRIVATE STUFF

		private static HttpRequest request
		{
			get { return HttpContext.Current.Request; }
		}
		private static string userIp
		{
			get
			{
				if (!string.IsNullOrEmpty(request.UserHostAddress))
					return request.UserHostAddress;
				else return null;
			}
		}
		private static string response
		{
			get
			{
				if (!string.IsNullOrEmpty(request[ResponseKey]))
					return request[ResponseKey];
				else return null;
			}
		}
		private static string parameters
		{
			get
			{
				string result;
				result = "secret=" + SecretKey;
				result += "&response=" + response;

				if (VerifyIp && !string.IsNullOrEmpty(userIp))
					result += "&remoteip=" + userIp;

				return result;
			}
		}

		[DataContract]
		public class Reply
		{
			[DataMember(Name = "success")]
			public bool Success;
			[DataMember(Name = "error-codes")]
			public string[] ErrorCodes;
		}
		public enum RenderOptions { renderExplicit, renderOnload }
		public enum ThemeOptions { dark, light }
		public enum TypeOptions { audio, image }
		public enum SizeOptions { compact, normal }
	}	
}
