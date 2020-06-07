using System;
using Catharsis.Navigation.Abstractions;

namespace Catharsis.Navigation.Extensions
{
    /// <summary>
    /// Class of extension method for object life cycle in Catharsis.
    /// </summary>
    public static class ViewModelActionExtensions
    {
        /// <summary>
        /// This is a thing I lifted from Prism.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="action">An action.</param>
        /// <typeparam name="T">A type.</typeparam>
        /// <returns>The object.</returns>
        public static object InvokeViewModelAction<T>(this object viewModel, Action<T> action) where T : class
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (viewModel is IViewModel element)
            {
                if (element is T viewModelAsT)
                {
                    action(viewModelAsT);
                }
            }

            return viewModel;
        }
    }
}