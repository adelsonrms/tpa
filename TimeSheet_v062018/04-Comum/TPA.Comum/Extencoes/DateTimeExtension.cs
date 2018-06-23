namespace TPA.Comum.Extensoes
{
    public static class DateTimeExtension
    {
        public static string ToDate(this System.DateTime data)
        {
            return data.Date.ToString("dd/MM/yyyy");
        }
    }
}
