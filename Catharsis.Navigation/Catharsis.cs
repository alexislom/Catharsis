using System;
using Catharsis.Navigation.Extensions;
using Splat;

namespace Catharsis.Navigation
{
    /// <summary>
    /// The main registration point for Catharsis.
    /// </summary>
    public class Catharsis
    {
        private static readonly Lazy<Catharsis> _catharsis = new Lazy<Catharsis>();

        static Catharsis()
        {
            Locator.RegisterResolverCallbackChanged(() =>
            {
                if (Locator.CurrentMutable == null)
                {
                    return;
                }

                Instance.Initialize();
            });
        }

        /// <summary>
        /// Gets the instance of <see cref="Catharsis"/>.
        /// </summary>
        public static Catharsis Instance => _catharsis.Value;

        /// <summary>
        /// Gets the mutable dependency resolver.
        /// </summary>
        public IMutableDependencyResolver MutableLocator => Locator.CurrentMutable;
    }
}