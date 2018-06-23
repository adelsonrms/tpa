using System;
namespace TPA.Comum.Extensoes
{
    public static class EnumExtension
    {
        public static int Value(this Enum Item) { return Convert.ToInt32(Item); }
    }
}
