namespace EchoRelay.Core.Utils
{
    /// <summary>
    /// A lock which can be used in asynchronous/awaitable contexts. It executes a given action with the lock.
    /// </summary>
    public class AsyncLock
    {
        /// <summary>
        /// The internal semaphore used for asynchronous locking.
        /// </summary>
        public SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Executes a given action in a locked context.
        /// </summary>
        /// <param name="action">The action to execute within a lock.</param>
        /// <returns></returns>
        public async Task ExecuteLocked(Func<Task> action)
        {
            try
            {
                // Define a locked state. We will acquire the lock, execute the action, and release the lock.
                var isLocked = false;

                // Keep trying to acquire the lock.
                do
                {
                    try
                    {
                        isLocked = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1));
                    }
                    catch
                    {
                    }
                }
                while (!isLocked);

                // Execute the action.
                await action();
            }
            finally
            {
                // If we obtained the lock, release it.
                _semaphore.Release();
            }
        }
    }
}
