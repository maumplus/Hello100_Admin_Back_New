using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;

namespace Hello100Admin.Modules.Admin.Application.Common.Errors
{
    public static class AdminErrorDescProvider
    {
        private static readonly FrozenDictionary<AdminErrorCode, string> _codeDic = Build();

        private static FrozenDictionary<AdminErrorCode, string> Build()
        {
            var dict = new Dictionary<AdminErrorCode, string>();
            var t = typeof(AdminErrorCode);

            foreach (AdminErrorCode v in Enum.GetValues(t))
            {
                var name = Enum.GetName(t, v) ?? v.ToString();
                var field = t.GetField(name, BindingFlags.Public | BindingFlags.Static);

                var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
                dict[v] = desc ?? name;
            }

            return dict.ToFrozenDictionary();
        }

        public static string GetDescription(AdminErrorCode code)
            => _codeDic.TryGetValue(code, out var s) ? s : code.ToString();
    }
}
