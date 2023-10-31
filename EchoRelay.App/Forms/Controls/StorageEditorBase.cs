using EchoRelay.App.Utils;
using EchoRelay.Core.Server.Storage;
using System.ComponentModel;

namespace EchoRelay.App.Forms.Controls
{
    public class StorageEditorBase : UserControl
    {
        private ServerStorage? _storage;
        public ServerStorage? Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                // Unsubscribe from previous storage events.
                if (_storage != null)
                {
                    _storage.OnStorageOpened -= OnStorageLoaded;
                    _storage.OnStorageClosed -= OnStorageUnloaded;

                    // Signal storage unloaded
                    this.InvokeUIThread(() =>
                    {
                        OnStorageUnloaded(_storage);
                    });
                }

                // Set the new storage
                _storage = value;

                // Subscribe to new storage events
                if (_storage != null)
                {
                    _storage.OnStorageOpened += OnStorageLoaded;
                    _storage.OnStorageClosed += OnStorageUnloaded;

                    // Signal storage loaded
                    this.InvokeUIThread(() =>
                    {
                        OnStorageLoaded(_storage);
                    });
                }
            }
        }

        private bool _changed;
        public bool Changed
        {
            get
            {
                return _changed;
            }
            set
            {
                // Record whether unsaved changes state changed
                bool unsavedChangesStateChanged = _changed != value;

                // Update the change state.
                _changed = value;

                // If our unsaved changes state changed, fire an event.
                if (unsavedChangesStateChanged)
                    OnUnsavedChangesStateChange?.Invoke(this, _changed);
            }
        }

        /// <summary>
        /// Event for a storage editor now having unsaved changes or no longer having them.
        /// </summary>
        /// <param name="storageEditor">The <see cref="StorageEditorBase"/> which changed its state.</param>
        /// <param name="hasUnsavedChanges">Indicates whether the editor now has unsaved changes or not.</param>
        public delegate void UnsavedChangesStateChange(StorageEditorBase storageEditor, bool hasUnsavedChanges);
        /// <summary>
        /// Event for a storage editor now having unsaved changes or no longer having them.
        /// </summary>
        [Browsable(true)]
        [Category("Changes")]
        [Description("Invoked when unsaved changes are made or cleared.")]
        public event UnsavedChangesStateChange? OnUnsavedChangesStateChange;

        protected virtual void OnStorageLoaded(ServerStorage storage) { }
        protected virtual void OnStorageUnloaded(ServerStorage storage) { }
        public virtual void SaveChanges() { }
        public virtual void RevertChanges() { }
    }
}
