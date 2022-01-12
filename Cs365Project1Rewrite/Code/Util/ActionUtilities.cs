using System;

namespace Cs365Project1Rewrite.Code.Util
{
    public struct Unit 
    {
        public static Unit Default => new();
    }

    public static class ActionUtilities
    {
        public static Func<Unit> ToUnitFunc(this Action action)
            => () =>
            {
                action();
                return Unit.Default;
            };
    }
}
