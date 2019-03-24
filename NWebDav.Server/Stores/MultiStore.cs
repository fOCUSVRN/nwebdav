using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NWebDav.Server.Stores
{
    public class MultiStore : IStore
    {
        private readonly IDictionary<string, IStore> _storeResolvers = new Dictionary<string, IStore>();

        public void AddStore(string prefix, IStore store)
        {
            // Convert the prefix to lower-case
            prefix = prefix.ToLowerInvariant();

            // Add the prefix to the store
            _storeResolvers.Add(prefix, store);
        }

        public void RemoveStore(string prefix)
        {
            // Convert the prefix to lower-case
            prefix = prefix.ToLowerInvariant();

            // Add the prefix to the store
            _storeResolvers.Remove(prefix);
        }

        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext)
        {
            return Resolve(uri, (storeResolver, subUri) => storeResolver.GetItemAsync(subUri, httpContext));
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext)
        {
            return Resolve(uri, (storeResolver, subUri) => storeResolver.GetCollectionAsync(subUri, httpContext));
        }

        private T Resolve<T>(Uri uri, Func<IStore, Uri, T> action)
        {
            // Determine the path
            var requestedPath = uri.LocalPath;
            var endOfPrefix = requestedPath.IndexOf('/');
            var prefix = (endOfPrefix >= 0 ? requestedPath.Substring(0, endOfPrefix) : requestedPath).ToLowerInvariant();
            var subUri = UriHelper.Combine(uri, endOfPrefix >= 0 ? requestedPath.Substring(endOfPrefix + 1) : string.Empty);

            // Try to find the store
            return !_storeResolvers.TryGetValue(prefix, out var store)
                ? default
                : action(store, subUri); // Resolve via the action
        }
    }
}
