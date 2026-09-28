using System.Text.RegularExpressions;

namespace ApiTests.Helpers
{
    /// <summary>
    /// Masks credentials before request and response details are written to the Allure report.
    ///
    /// The reports are published (GitHub Pages, CI artifacts), and they log every request body - which
    /// is exactly how a real password once ended up committed to this repository inside an old
    /// allure-report folder. Anything that goes into an attachment passes through <see cref="Redact"/>.
    ///
    /// Covers the three shapes the clients log: "password: x" lines, "password=x" form or query
    /// parameters, and JSON such as "token": "x".
    /// </summary>
    public static class SensitiveData
    {
        private static readonly Regex Secret = new(
            @"(?i)(\b(?:password|newPassword|currentPassword|token|x-auth-token|authorization)\b""?\s*[:=]\s*""?)[^""&\r\n,}]+",
            RegexOptions.Compiled);

        public static string Redact(string? text) =>
            string.IsNullOrEmpty(text) ? string.Empty : Secret.Replace(text, "$1***");
    }
}
