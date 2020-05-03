namespace Catharsis.Navigation.Abstractions
{
    /// <summary>
    /// Interface that defines a view model for a page for the navigation or modal stack.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Gets the ID of the page.
        /// </summary>
        string Id { get; }
    }
}