using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

namespace EchoRelay.Core.Server.Storage.Resources
{
    [JsonConverter(typeof(SymbolCacheSerializer))]
    /// <summary>
    /// A two-way lookup between symbol identifiers amd symbol names.
    /// Symbols are used to reference various objects such as message identifiers, config resources, documents, etc.
    /// </summary>
    public class SymbolCache
    {
        #region Fields
        /// <summary>
        /// A lookup of symbol to names. This mirrors <see cref="_namesToSymbols"/>.
        /// </summary>
        private ConcurrentDictionary<long, string> _symbolsToName;
        /// <summary>
        /// A lookup of names to symbols. This mirrors <see cref="_symbolsToName"/>.
        /// </summary>
        private ConcurrentDictionary<string, long> _namesToSymbols;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new, empty <see cref="SymbolCache"/>.
        /// </summary>
        public SymbolCache() : this(new Dictionary<string, long>()) { }
        /// <summary>
        /// Initializes a new <see cref="SymbolCache"/> with the names and symbols provided by the keys
        /// and values of the provided dictionary.
        /// </summary>
        /// <param name="namesToSymbols">A lookup of symbol names to symbol identifiers to be added.</param>
        public SymbolCache(IDictionary<string, long> namesToSymbols)
        {
            // Set our names to symbols lookup to a copy of the provided.
            _namesToSymbols = new ConcurrentDictionary<string, long>(namesToSymbols);
            _symbolsToName = new ConcurrentDictionary<long, string>();

            // Create the reverse lookup.
            foreach (string name in _namesToSymbols.Keys)
                _symbolsToName[_namesToSymbols[name]] = name;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Adds a new symbol with the provided name.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <param name="symbol">The symbol identifier.</param>
        /// <param name="throwIfExists">Indicates whether an exception should be thrown if a different key-value pair already exists for either the name of symbol.</param>
        /// <exception cref="ArgumentException">An exception is thrown if a different key-value pair already exists than the provided one.</exception>
        public void Add(string name, long symbol, bool throwIfExists = false)
        {
            // Check if the symbol already exists in our lookup and throw an exception if we intend to.
            bool exists = _namesToSymbols.TryGetValue(name, out long existingSymbol);
            exists |= _symbolsToName.TryGetValue(symbol, out string? existingName);
            if (exists)
            {
                if (throwIfExists && (existingName != name || existingSymbol != symbol))
                {
                    throw new ArgumentException($"Failed to add symbol <name={name}, symbol={symbol}> to cache: conflicting symbol <name={existingName}, symbol={existingSymbol}> exists.");
                }

                // Perform a clean removal if it existed in any case, to avoid losing track of a previous name in the two-way lookup operations.
                Remove(name);
                Remove(symbol);
            }


            // Add it to our lookups.
            _namesToSymbols[name] = symbol;
            _symbolsToName[symbol] = name;
        }
        /// <summary>
        /// Adds all symbols from a provided <see cref="SymbolCache"/> to the current one.
        /// </summary>
        /// <param name="symbolCache">The <see cref="SymbolCache"/> to be absorbed into the current one.</param>
        /// <param name="throwIfExists">Indicates whether an exception should be thrown if a different key-value pair already exists for either the name of symbol.</param>
        public void Add(SymbolCache symbolCache, bool throwIfExists = false)
        {
            // Add our cache to this one.
            Add(symbolCache._namesToSymbols, throwIfExists);
        }
        /// <summary>
        /// Adds all symbols from a provided lookup of symbol names to symbols.
        /// </summary>
        /// <param name="nameToSymbols">A lookup of symbol names to symbols, to be added to the <see cref="SymbolCache"/>.</param>
        /// <param name="throwIfExists">An exception is thrown if a different key-value pair already exists than the provided one.</param>
        public void Add(IDictionary<string, long> nameToSymbols, bool throwIfExists = false)
        {
            // Loop for each symbol
            foreach (string name in _namesToSymbols.Keys)
            {
                Add(name, nameToSymbols[name], throwIfExists);
            }
        }
        /// <summary>
        /// Adds all symbols from .exe/.dll game files in the provided directory.
        /// </summary>
        /// <param name="directory">The game directory containing assemblies to extract symbols from.</param>
        /// <param name="searchOption">The depth to search for assemblies to extract symbols from.</param>
        /// <param name="throwIfExists">An exception is thrown if a different key-value pair already exists than the provided one.</param>
        /// <exception cref="DirectoryNotFoundException">An exception thrown if the provided directory does not exist.</exception>
        public void Add(string directory, SearchOption searchOption = SearchOption.AllDirectories, bool throwIfExists = false)
        {
            // Verify the path exists
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Cannot extract symbols from directory \"{directory}\" as the path could not be found.");

            // Obtain all assembly files.
            string[] targetFileExtensions = { "exe", "dll" };
            string[] assemblyFilePaths = Directory.GetFiles(directory, "*.*", searchOption)
                .Where(f => targetFileExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();

            // Define the symbol marker
            byte[] symbolMarker = Encoding.UTF8.GetBytes("OlPrEfIx");

            // Load each assembly
            foreach (string assemblyPath in assemblyFilePaths)
            {
                // Read the assembly data and begin looking for all symbols stored in it.
                byte[] data = File.ReadAllBytes(assemblyPath);
                Span<byte> dataSpan = data;

                // Loop throughout our entire buffer.
                for (int currentPosition = 8; currentPosition < dataSpan.Length; currentPosition++)
                {
                    // See if the symbol marker could be located at this position.
                    bool match = true;
                    for (int i = 0; i < symbolMarker.Length; i++)
                    {
                        if (symbolMarker[i] != dataSpan[currentPosition + i])
                        {
                            match = false;
                            break;
                        }
                    }

                    // If we couldn't match it at this position, advance.
                    if (!match)
                        continue;

                    // Obtain the 64-bit symbol prior to this index
                    long symbol = BitConverter.ToInt64(dataSpan.Slice(currentPosition - 8, 8));

                    // Determine the location of the name
                    int symbolNameStart = currentPosition + symbolMarker.Length;
                    int symbolNameEnd = Array.IndexOf(data, (byte)0x00, symbolNameStart);
                    if (symbolNameEnd < symbolNameStart)
                        symbolNameEnd = data.Length;
                    else
                        symbolNameEnd = Math.Max(symbolNameStart, symbolNameEnd);

                    // Obtain the null terminated symbol name.
                    string name = Encoding.UTF8.GetString(dataSpan.Slice(symbolNameStart, symbolNameEnd - symbolNameStart));

                    // Add the symbol
                    Add(name, symbol, throwIfExists);

                    // Advance our position to the location of the end of our added symbol's name.
                    currentPosition = symbolNameEnd;
                }
            }
        }
        /// <summary>
        /// Obtains a name associated with a symbol.
        /// </summary>
        /// <param name="symbol">The symbol to obtain a name for.</param>
        /// <returns>Returns the name associated with the symbol, or null if it could not be resolved.</returns>
        public string? GetName(long symbol)
        {
            // Try to get a name from this symbol.
            _symbolsToName.TryGetValue(symbol, out var name);
            return name;
        }
        /// <summary>
        /// Obtains a symbol for a given symbol name.
        /// </summary>
        /// <param name="name">The name to obtain a symbol for.</param>
        /// <returns>Returns the symbol for the provided name, or null if it could not be resolved.</returns>
        public long? GetSymbol(string name)
        {
            // Try to get a symbol from this name.
            _namesToSymbols.TryGetValue(name, out var symbol);
            return symbol;
        }
        /// <summary>
        /// Removes a symbol by its name, if it could be found.
        /// </summary>
        /// <param name="name">The name to remove a symbol for.</param>
        public void Remove(string name)
        {
            // Try to get the symbol for this name
            if (_namesToSymbols.TryGetValue(name, out long symbol))
            {
                _symbolsToName.Remove(symbol, out _);
                _namesToSymbols.Remove(name, out _);
            }
        }
        /// <summary>
        /// Removes a symbol by its identifier, if it could be found.
        /// </summary>
        /// <param name="symbol">The symbol identifier for the symbol to be removed.</param>
        public void Remove(long symbol)
        {
            // Try to get the name for this symbol
            if (_symbolsToName.TryGetValue(symbol, out string? name))
            {
                _namesToSymbols.Remove(name, out _);
                _symbolsToName.Remove(symbol, out _);
            }
        }
        /// <summary>
        /// Clears all symbols out of the <see cref="SymbolCache"/>.
        /// </summary>
        public void Clear()
        {
            _symbolsToName.Clear();
            _namesToSymbols.Clear();
        }
        /// <summary>
        /// Converts the <see cref="SymbolCache"/> into a dictionary lookup.
        /// </summary>
        /// <returns>Returns a symbol name to symbol identifier lookup.</returns>
        public Dictionary<string, long> ToDictionary()
        {
            return new Dictionary<string, long>(_namesToSymbols);
        }
        #endregion
    }


    /// <summary>
    /// A <see cref="JsonConverter"/> used to serialize/deserialize a <see cref="SymbolCache"/>.
    /// </summary>
    public class SymbolCacheSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SymbolCache);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // Deserialize a dictionary of names-to-symbol-ids, and initialize a symbol cache with it.
            var namesToSymbols = serializer.Deserialize<Dictionary<string, long>>(reader) ?? new Dictionary<string, long>();
            return new SymbolCache(namesToSymbols);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            // Verify the value is of the correct type.
            if (value?.GetType() != typeof(SymbolCache))
                return;

            // Serialize the names-to-symbol-ids lookup from the symbol cache.
            serializer.Serialize(writer, ((SymbolCache)value).ToDictionary());
        }
    }
}
