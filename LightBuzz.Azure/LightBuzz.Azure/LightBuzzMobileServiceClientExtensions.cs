using Mono.Data.Sqlite;

namespace LightBuzz.Azure
{
    /// <summary>
    /// <see cref="Mono.Data.Sqlite"/> extension methods.
    /// </summary>
    public static class LightBuzzMobileServiceClientExtensions
    {
        /// <summary>
        /// Closes the current connection.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        public static void Close(this SqliteConnection connection)
        {
            connection.Close();
        }
    }
}
