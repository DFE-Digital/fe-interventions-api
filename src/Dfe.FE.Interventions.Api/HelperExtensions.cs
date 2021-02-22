namespace Dfe.FE.Interventions.Api
{
    public static class HelperExtensions
    {
        public static bool TryParseAsNullableInt(this string value, out int? parsed)
        {
            if (!int.TryParse(value, out var temp))
            {
                parsed = null;
                return false;
            }

            parsed = temp;
            return true;
        }
        public static bool TryParseAsInt(this string value, out int parsed)
        {
            if (!int.TryParse(value, out var temp))
            {
                parsed = 0;
                return false;
            }

            parsed = temp;
            return true;
        }
    }
}