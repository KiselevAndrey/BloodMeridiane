namespace KAP.Extension
{
    public static class BoolExtencion
    {
        public static int ToInt(this bool value) => value == true ? 1 : 0;
    }
}