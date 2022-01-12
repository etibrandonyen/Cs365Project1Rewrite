using System;

namespace Cs365Project1Rewrite.Code.Util
{
    public static class NullableExtensions
    {
#nullable enable
        public static U? ToNullable<T, U>(this T t, Func<T, U?> creation)
            where T : class
            where U : class => creation(t);

        public static U? BindReferenceToValue<T, U>(this T? nullable, Func<T, U?> bind)
            where T : class
            where U : struct => nullable == null ? null : bind(nullable);

#nullable disable

        public static U? Bind<T, U>(this T? nullable, Func<T, U?> bind) 
            where T : struct 
            where U : struct => nullable.HasValue ? bind(nullable.Value) : null;

        public static U? Transform<T, U>(this T? nullable, Func<T, U> map)
            where T : struct
            where U : struct => nullable.Bind(nVal => (U?)map(nVal));

        public static void Finally<T>(this T? t, Action<T> action) where T : struct
        {
            if (t.HasValue) { action(t.Value); }
        }
    }
}
