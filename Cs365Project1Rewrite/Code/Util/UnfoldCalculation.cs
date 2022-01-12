using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs365Project1Rewrite.Code.Util
{
    public static class UnfoldCalculation
    {
        public static IEnumerable<IEnumerable<T>> SelectWithPrevious<T>(this IEnumerable<T> ts) where T : class
        {
            foreach (var t in ts)
            {
                yield return ts.TakeWhile(tInner => !tInner.Equals(t)).Append(t);
            }
        }
        
        public static IEnumerable<TResult> UnfoldWhile<TResult, TStep, TState>(
            TState state,
            Func<TState, TStep> stepper,
            Func<TState, bool> predicate,
            Func<TStep, TState> stateTransform,
            Func<TState, TResult> resultTransform)
        {
            while (predicate(state))
            {
                yield return resultTransform(state);

                state = stateTransform(stepper(state));
            }
        }
    }
}
