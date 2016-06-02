# ReCaptchaForASP
Complete ReCaptcha wrapper for ASP.Net and above. Includes automatic reply and all optional features from Google (size, type, theme, language, callback, expired callback, etc.).

## Just get it done!

Usually in your Global.asax (Application_OnStart) or your _AppStart.cshtml, depending on the specific flavor of ASP.Net you are using, you should start by specifying your ReCaptcha's required settings:

- ReCaptcha.SecretKey
- ReCaptcha.DataSiteKey

You can add the script in the head of your html by using `@ReCaptcha.GenerateScript()` and you can render the widget anywhere you want in your body by using `@ReCaptcha.Widget`.

Afterwards, on server-side code, you can send create and verify a ReCaptcha request simply by calling `var captcha = ReCaptcha.CurrentReply`. Start your code with `if (captcha.Success)` and you're good to go!

## AppStart - for the tweakhead

You can also set a number of optional settings in AppStart, all of which are properly documented on [Google ReCaptcha's website](https://developers.google.com/recaptcha/intro) so I won't do it here.

Script features:
- ReCaptcha.OnLoad
- ReCaptcha.Render

Widget features:
- ReCaptcha.DataTheme
- ReCaptcha.DataType
- ReCaptcha.DataSize
- ReCaptcha.DataTabindex
- ReCaptcha.DataCallback
- ReCaptcha.DataExpiredCallback

Finally, and in the unlikely event that Google modifies its Api Urls etc., you can also change the following (but these are simple variables, not properties):
- ReCaptcha.VerifyApiUrl ("https://www.google.com/recaptcha/api/siteverify")
- ReCaptcha.ScriptApiUrl ("https://www.google.com/recaptcha/api.js")
- ReCaptcha.ResponseKey ("g-recaptcha-response")
- ReCaptcha.VerifyIp (true - automated)
- ReCaptcha.TimeoutAttempts (10)

After you have acquired `ReCaptcha.CurrentReply`, you can also access the error-codes as an array of string through `captcha.ErrorCodes`.

## Thanks
I had wondered for a while now why Google never released an official ASP.Net plug for their reCAPTCHA... If you would like to improve on this code, thanks to shoot me an [email](mailto:alexandrepigeot@gmail.com). I would appreciate some help on this.
