using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Utils
{
    /// <summary>
    /// Utilities to help with JSON operations.
    /// </summary>
    public abstract class JsonUtils
    {
        #region Fields
        /// <summary>
        /// Defines the settings for JSON merges.
        /// </summary>
        private static JsonMergeSettings _mergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union,
            MergeNullValueHandling = MergeNullValueHandling.Ignore,
        };
        #endregion

        #region Functions
        /// <summary>
        /// Merges given <see cref="JObject"/>-compatible objects together.
        /// </summary>
        /// <typeparam name="T">The type of the objects to merge.</typeparam>
        /// <param name="obj">The first object to be merged into.</param>
        /// <param name="obj2">The second object to merge into the first.</param>
        /// <returns>Returns the merged object.</returns>
        public static T? MergeObjects<T>(T obj, T obj2)
        {
            // If the second object is null, return the first simply.
            if (obj2 == null)
                return obj;

            // Otherwise merge.
            return MergeObjects(obj, JObject.FromObject(obj2));
        }

        /// <summary>
        /// Merges given <see cref="JObject"/>-compatible objects together.
        /// </summary>
        /// <typeparam name="T">The type of the objects to merge.</typeparam>
        /// <param name="obj">The first object to be merged into.</param>
        /// <param name="obj2">The second object to merge into the first.</param>
        /// <returns>Returns the merged object.</returns>
        public static T? MergeObjects<T>(T obj, JObject obj2)
        {
            // If the first object is null, return the second object.
            if (obj == null)
                return obj2.ToObject<T>();

            // Merge both objects and return the result.
            JObject mergedObj = JObject.FromObject(obj);
            mergedObj.Merge(obj2, _mergeSettings);
            return mergedObj.ToObject<T>();
        }
        #endregion

        #region Classes
        /// <summary>
        /// A <see cref="JsonConverter"/> used to serialize/deserialize <see cref="HashSet{T}"/> types.
        /// </summary>
        public class HashSetConverter<T> : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HashSet<T>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                // Deserialize an array and initialize a hashset with it.
                var arr = serializer.Deserialize<T[]>(reader) ?? Array.Empty<T>();
                return new HashSet<T>(arr);
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                // Verify the value is of the correct type.
                if (value?.GetType() != typeof(HashSet<T>))
                    return;

                // Serialize the value as an array.
                serializer.Serialize(writer, ((HashSet<string>)value).ToArray());
            }
        }
        #endregion

    }
}
