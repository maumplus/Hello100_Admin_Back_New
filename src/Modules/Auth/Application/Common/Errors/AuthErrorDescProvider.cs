using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;

namespace Hello100Admin.Modules.Auth.Application.Common.Errors
{
    public static class AuthErrorDescProvider
    {
        private static readonly FrozenDictionary<AuthErrorCode, string> _codeDic = Build();

        private static FrozenDictionary<AuthErrorCode, string> Build()
        {
            var dict = new Dictionary<AuthErrorCode, string>();
            var t = typeof(AuthErrorCode);

            foreach (AuthErrorCode v in Enum.GetValues(t))
            {
                var name = Enum.GetName(t, v) ?? v.ToString();
                var field = t.GetField(name, BindingFlags.Public | BindingFlags.Static);

                var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
                dict[v] = desc ?? name;
            }

            return dict.ToFrozenDictionary();
        }

        public static string GetDescription(AuthErrorCode code)
            => _codeDic.TryGetValue(code, out var s) ? s : code.ToString();
    }
}
