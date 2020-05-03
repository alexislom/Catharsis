using System;
using Catharsis.Navigation.Mixins;
namespace Catharsis.Navigation.Forms.Mixins
{
    /// <summary>
    /// Extension methods interact with <see cref="Catharsis"/>.
    /// </summary>
    public static class CatharsisExtensions
    {
        /// <summary>
        /// Initializes the Catharsis.
        /// </summary>
        /// <param name="sextant">The catharsis.</param>
        public static void InitializeForms(this Catharsis catharsis)
        {
            if (catharsis is null)
            {
                throw new ArgumentNullException(nameof(catharsis));
            }

            catharsis
                .MutableLocator
                .RegisterNavigationView()
                .RegisterViewStackService();
            //.RegisterParameterViewStackService()
            //.RegisterViewModelFactory(() => new DefaultViewModelFactory());
        }
    }
}