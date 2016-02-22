namespace Infocom.TimeManager.Core.Extensions
{
    using System;

    public static class GenericExtensions
    {
        #region Public Methods

        public static void Using<TContext>(this TContext target, Action<TContext> action) where TContext : IDisposable
        {
            using (target)
            {
                action(target);
            }
        }

        public static TResult Using<TContext, TResult>(this TContext target, Func<TContext, TResult> action)
            where TContext : IDisposable
        {
            using (target)
            {
                return action(target);
            }
        }

        #endregion
    }
}