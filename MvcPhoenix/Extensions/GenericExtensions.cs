using System.Runtime.Serialization;

namespace MvcPhoenix.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// This extension clones any data object and returns the new cloned object.
        /// I haven't tested its perfromance against other approaches.
        /// Always test data object when using this generic object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">The original object.</param>
        /// <returns>The clone of the object.</returns>
        public static T Clone<T>(this T source)
        {
            var dcs = new DataContractSerializer(typeof(T));
            using (var ms = new System.IO.MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
    }
}