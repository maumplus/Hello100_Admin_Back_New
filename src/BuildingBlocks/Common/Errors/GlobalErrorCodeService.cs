using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;

namespace Hello100Admin.BuildingBlocks.Common.Errors
{
    public static class GlobalErrorCodeService
    {
        private static readonly FrozenDictionary<GlobalErrorCode, string> _codeDic = Build();

        private static FrozenDictionary<GlobalErrorCode, string> Build()
        {
            var dict = new Dictionary<GlobalErrorCode, string>();
            var t = typeof(GlobalErrorCode);

            foreach (GlobalErrorCode v in Enum.GetValues(t))
            {
                var name = Enum.GetName(t, v) ?? v.ToString();
                var field = t.GetField(name, BindingFlags.Public | BindingFlags.Static);

                var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
                dict[v] = desc ?? name;
            }

            return dict.ToFrozenDictionary();
        }

        public static string GetDescription(GlobalErrorCode code)
            => _codeDic.TryGetValue(code, out var s) ? s : code.ToString();
    }
}
