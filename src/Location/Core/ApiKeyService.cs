using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Patterns;

namespace Nwpie.Foundation.Location.Core
{
    public class ApiKeyService : SingleCObject<ApiKeyService>
    {
        protected override void InitialInConstructor() { }

        public IList<LocationInfo> GetApiKeyLocationInfo() =>
            GetApiKeyLocationInfo(new DateTime(2020, 1, 1));

        public IList<LocationInfo> GetApiKeyLocationInfo(DateTime lastUpdateTime) =>
            new List<LocationInfo>();

        public override void Dispose()
        {

        }

    }
}
