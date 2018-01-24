namespace Repelsteeltje
{
    public enum NameTypes
    {
        Both = 0,
        Boys = 1,
        Girls = 2,
    }
    public static class NameTypesExtensions
    {
        public static bool IsSingleGender(this NameTypes tp) => tp == NameTypes.Boys || tp == NameTypes.Girls;
    }
}
