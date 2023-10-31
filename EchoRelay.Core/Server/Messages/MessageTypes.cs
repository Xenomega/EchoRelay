using System.Collections.Concurrent;
using System.Reflection;

namespace EchoRelay.Core.Server.Messages
{
    /// <summary>
    /// A lookup of <see cref="Message.MessageTypeSymbol"/> to <see cref="Message"/>s. This is used by a <see cref="Packet"/> to determine the types of underlying messages to decode.
    /// </summary>
    public abstract class MessageTypes
    {
        #region Fields
        /// <summary>
        /// A lookup of <see cref="Message"/> types to <see cref="Message.MessageTypeSymbol"/>s.
        /// </summary>
        private static ConcurrentDictionary<Type, long> _typesToSymbols = new ConcurrentDictionary<Type, long>();

        /// <summary>
        /// A lookup of <see cref="Message.MessageTypeSymbol"/> identifiers to <see cref="Message"/> type.
        /// </summary>
        private static ConcurrentDictionary<long, Type> _symbolsToTypes = new ConcurrentDictionary<long, Type>();
        #endregion

        #region Constructor
        /// <summary>
        /// Loads all classes which inherit from <see cref="Message"/>.
        /// </summary>
        static MessageTypes()
        {
            // Obtain this assembly.
            Assembly? currentAssembly = Assembly.GetAssembly(typeof(Message));

            // If our current assembly is not null, obtain our types for it.
            if (currentAssembly != null)
            {
                var assemblyMessageTypes = currentAssembly.GetTypes().Where(t => typeof(Message).IsAssignableFrom(t));

                // Register each type in our assembly.
                foreach (var type in assemblyMessageTypes)
                {
                    // Skip the unimplemented message type
                    if (type == typeof(UnimplementedMessage) || type == typeof(Message))
                        continue;

                    // Register the message type.
                    Register(type);
                }
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Indicates whether the provided type is that of a <see cref="Message"/>.
        /// </summary>
        /// <param name="type">The type to check is a <see cref="Message"/>.</param>
        /// <returns>Returns a boolean indicating whether the provided type is a <see cref="Message"/> type.</returns>
        public static bool IsMessageType(Type type)
        {
            // Check if the type is assignable.
            return typeof(Message).IsAssignableFrom(type);
        }


        /// <summary>
        /// Creates a <see cref="Message"/> of the type which corresponds to the given identifier.
        /// </summary>
        /// <param name="typeSymbol">A symbol indicating the type of <see cref="Message"/> to create.</param>
        /// <param name="errorIfUnimplemented">If the message identifier provided is unknown, throw an exception rather than returning an <see cref="UnimplementedMessage"/> type.</param>
        /// <returns>Returns an instance of a <see cref="Message"/> of the type associated with the provided message identifier.</returns>
        /// <exception cref="ArgumentException">An exception is thrown if the message identifier is unknown or the message could not be instantiated.</exception>
        public static Message CreateMessage(long typeSymbol, bool errorIfUnimplemented = false)
        {
            // Obtain the type for this identifier.
            Message? message = null;
            bool typeRegistered = _symbolsToTypes.TryGetValue(typeSymbol, out var type);
            if (!typeRegistered || type == null)
            {
                if (errorIfUnimplemented)
                {
                    throw new ArgumentException($"Failed to create message from type symbol. No message type was registered for symbol {typeSymbol}");
                }
                return new UnimplementedMessage(typeSymbol);
            }

            // Create an instance of the message.
            message = (Message?)Activator.CreateInstance(type);
            if (message == null)
            {
                throw new ArgumentException($"Failed to create message from type symbol. {type.Name} could not be instantiated as a {nameof(Message)} type.");
            }
            return message;
        }

        /// <summary>
        /// Registers a given type for <see cref="Packet"/> deserialization operations.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        public static void Register<T>() where T : Message
        {
            Register(typeof(T));
        }
        /// <summary>
        /// Registers a given type for <see cref="Packet"/> deserialization operations.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the type provided is not a <see cref="Message"/> type or it could not be instantiated with its non-paramaterized constructor.</exception>
        public static void Register(Type type)
        {
            // If the type is not deriving from our message type, throw an exception.
            if (!IsMessageType(type) || type == typeof(UnimplementedMessage) || type == typeof(Message))
            {
                throw new ArgumentException($"Failed to register message type. {type.Name} is not a valid {nameof(Message)} type to register.");
            }


            // Create an instance of the message.
            Message? defaultMessage = (Message?)Activator.CreateInstance(type);
            if (defaultMessage == null)
            {
                throw new ArgumentException($"Failed to register message type. {type.Name} could not be instantiated as a {nameof(Message)} type.");
            }

            // Set it in our lookups.
            _typesToSymbols[type] = defaultMessage.MessageTypeSymbol;
            _symbolsToTypes[defaultMessage.MessageTypeSymbol] = type;
        }

        /// <summary>
        /// Unregisters a given type from <see cref="Packet"/> deserialization operations.
        /// </summary>
        /// <typeparam name="T">The type to unregister.</typeparam>
        public static void Unregister<T>() where T: Message
        {
            Unregister(typeof(T));
        }
        /// <summary>
        /// Unregisters a given type from <see cref="Packet"/> deserialization operations.
        /// </summary>
        /// <param name="type">The type to unregister.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the type provided is not a <see cref="Message"/> type or it could not be instantiated with its non-paramaterized constructor.</exception>
        public static void Unregister(Type type)
        {
            // If the type is not deriving from our message type, throw an exception.
            if (!IsMessageType(type))
            {
                throw new ArgumentException($"Failed to unregister message type. {type.Name} is not a valid {nameof(Message)} type.");
            }

            // Create an instance of the message.
            Message? defaultMessage = (Message?)Activator.CreateInstance(type);
            if (defaultMessage == null)
            {
                throw new ArgumentException($"Failed to unregister message type. {type.Name} could not be instantiated as a {nameof(Message)} type.");
            }

            // Remove the type from the lookups.
            _typesToSymbols.Remove(type, out _);
            _symbolsToTypes.Remove(defaultMessage.MessageTypeSymbol, out _);
        }
        #endregion
    }
}
