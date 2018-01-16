﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XForm.IO.StreamProvider
{
    public class StreamProviderCache : IStreamProvider
    {
        private IStreamProvider _inner;
        private Cache<StreamAttributes> _attributesCache;
        private Cache<IEnumerable<StreamAttributes>> _enumerateCache;
        private Cache<byte[]> _smallFileCache;
        private Cache<ItemVersions> _versionCache;

        public StreamProviderCache(IStreamProvider inner)
        {
            _inner = inner;
            _attributesCache = new Cache<StreamAttributes>();
            _enumerateCache = new Cache<IEnumerable<StreamAttributes>>();
            _smallFileCache = new Cache<byte[]>();
            _versionCache = new Cache<ItemVersions>();
        }

        public string Description => _inner.Description;

        public StreamAttributes Attributes(string logicalPath)
        {
            return _attributesCache.GetOrBuild(logicalPath, null, () => _inner.Attributes(logicalPath));
        }

        public void Delete(string logicalPath)
        {
            _attributesCache.Remove(logicalPath);
            _inner.Delete(logicalPath);
        }

        public IEnumerable<StreamAttributes> Enumerate(string underLogicalPath, EnumerateTypes types, bool recursive)
        {
            return _enumerateCache.GetOrBuild($"{recursive}|{types}|{underLogicalPath}", null, () =>
            {
                return _inner.Enumerate(underLogicalPath, types, recursive).ToList();
            });
        }

        public ItemVersions ItemVersions(LocationType location, string itemName)
        {
            return _versionCache.GetOrBuild($"{location}|{itemName}", null, () => _inner.ItemVersions(location, itemName));
        }

        public Stream OpenAppend(string logicalPath)
        {
            _attributesCache.Remove(logicalPath);
            _smallFileCache.Remove(logicalPath);
            return _inner.OpenAppend(logicalPath);
        }

        public Stream OpenRead(string logicalPath)
        {
            if (logicalPath.EndsWith(".xql"))
            {
                return new MemoryStream(_smallFileCache.GetOrBuild(logicalPath, null, () =>
                {
                    using (Stream stream = _inner.OpenRead(logicalPath))
                    {
                        byte[] contents = new byte[stream.Length];
                        stream.Read(contents, 0, contents.Length);
                        return contents;
                    }
                }), false);
            }
            else
            {
                return _inner.OpenRead(logicalPath);
            }
        }

        public Stream OpenWrite(string logicalPath)
        {
            _attributesCache.Remove(logicalPath);
            _smallFileCache.Remove(logicalPath);
            return _inner.OpenWrite(logicalPath);
        }

        public void Publish(string logicalTablePath)
        {
            string[] parts = logicalTablePath.Split('\\');
            if(parts.Length > 1) _versionCache.Remove($"{parts[0]}|{parts[1]}");
            _inner.Publish(logicalTablePath);
        }
    }
}
