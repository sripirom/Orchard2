﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Layers.Indexes;
using Orchard.Layers.Models;
using YesSql.Core.Services;

namespace Orchard.Layers.Services
{
	public class LayerService : ILayerService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ISession _session;

		private const string LayersCacheKey = "LayersDocument";

		public LayerService(
			ISession session,
			IMemoryCache memoryCache)
		{
			_session = session;
			_memoryCache = memoryCache;
		}

		public async Task<LayersDocument> GetLayersAsync()
		{
			LayersDocument layers;

			if (!_memoryCache.TryGetValue(LayersCacheKey, out layers))
			{
				layers = await _session.QueryAsync<LayersDocument>().FirstOrDefault();

				if (layers == null)
				{
					lock (_memoryCache)
					{
						if (!_memoryCache.TryGetValue(LayersCacheKey, out layers))
						{
							layers = new LayersDocument();

							_session.Save(layers);
							_memoryCache.Set(LayersCacheKey, layers);
						}
					}
				}
				else
				{
					_memoryCache.Set(LayersCacheKey, layers);
				}
			}

			return layers;
		}

		public async Task<IEnumerable<LayerMetadata>> GetLayerWidgetsAsync(Expression<Func<ContentItemIndex, bool>> predicate)
		{
			var allWidgets = await _session
				.QueryAsync<ContentItem, LayerMetadataIndex>()
				.With(predicate)
				.List();

			return allWidgets.Select(x => x.As<LayerMetadata>()).Where(x => x != null).ToList();
		}

		public async Task UpdateAsync(LayersDocument layers)
		{
			var existing = await _session.QueryAsync<LayersDocument>().FirstOrDefault();

			existing.Layers = layers.Layers;
			_session.Save(existing);
			_memoryCache.Set(LayersCacheKey, existing);
		}
	}
}
