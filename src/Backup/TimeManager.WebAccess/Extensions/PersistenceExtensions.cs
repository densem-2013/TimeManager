namespace Infocom.TimeManager.WebAccess.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using NHibernate;

    public static class PersistenceExtensions
    {
        public static void DeleteIn<T>(this IEnumerable<T> target, ISession session, ICollection<T> collection)
        {
            for (int i = target.Count() - 1; i >= 0; i--)
            {
               session .Delete(target.ElementAt(i));
               collection.Remove(target.ElementAt(i));
            }
        }
    }
}