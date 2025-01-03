namespace XbrlInspector.ObjectIdMap
{
    /// <summary>
    /// A map of objects keyed by a GUID-based ID.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The app maintains pages for each class type to be supported in the app, and the navigation is
    /// set up to supply a page parameter to specify which object is to have its state displayed in the
    /// page.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">
    /// The type of object to be stored in the map.
    /// </typeparam>
    public class ObjectIdMapBase<T>
    {
        private Dictionary<Guid, T> map;

        internal ObjectIdMapBase()
        {
            map = new Dictionary<Guid, T>();
        }

        internal Guid Add(T obj)
        {
            var id = Guid.NewGuid();
            map.Add(id, obj);
            return id;
        }

        internal T? Get(Guid id)
        {
            map.TryGetValue(id, out T? obj);
            return obj;
        }
    }
}