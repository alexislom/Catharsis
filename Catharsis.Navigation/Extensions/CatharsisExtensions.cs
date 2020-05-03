using System;
using Catharsis.Navigation.Mixins;

namespace Catharsis.Navigation.Extensions
{
    /// <summary>
    /// Extensions methods to setup the <see cref="Catharsis"/> instance.
    /// </summary>
    public static class CatharsisExtensions
    {
        /// <summary>
        /// Initializes the specified sextant.
        /// </summary>
        /// <param name="catharsis">The catharsis.</param>
        public static void Initialize(this Catharsis catharsis)
        {
            if (catharsis is null)
            {
                throw new ArgumentNullException(nameof(catharsis));
            }

            catharsis.MutableLocator.RegisterViewStackService();
        }
    }
}