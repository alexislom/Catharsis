namespace Catharsis.Navigation.Abstractions
{
    /// <summary>
    /// Interface representing an object capable of being destroyed.
    /// </summary>
    public interface IDestructible
    {
        /// <summary>
        /// Destroy the destructible object.
        /// </summary>
        public void Destroy();
    }
}