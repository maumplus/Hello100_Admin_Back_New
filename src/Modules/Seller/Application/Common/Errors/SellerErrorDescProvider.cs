using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;

namespace Hello100Admin.Modules.Seller.Application.Common.Errors
{
    public static class SellerErrorDescProvider
    {
        private static readonly FrozenDictionary<SellerErrorCode, string> _codeDic = Build();

        private static FrozenDictionary<SellerErrorCode, string> Build()
        {
            var dict = new Dictionary<SellerErrorCode, string>();
            var t = typeof(SellerErrorCode);

            foreach (SellerErrorCode v in Enum.GetValues(t))
            {
                var name = Enum.GetName(t, v) ?? v.ToString();
                var field = t.GetField(name, BindingFlags.Public | BindingFlags.Static);

                var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
                dict[v] = desc ?? name;
            }

            return dict.ToFrozenDictionary();
        }

        public static string GetDescription(SellerErrorCode code)
            => _codeDic.TryGetValue(code, out var s) ? s : code.ToString();
    }
}
