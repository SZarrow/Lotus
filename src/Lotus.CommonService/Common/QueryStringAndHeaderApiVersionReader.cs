using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Primitives;

namespace Lotus.CommonService.Common
{
    public class QueryStringAndHeaderApiVersionReader : IApiVersionReader
    {
        private String _parameterName;

        public QueryStringAndHeaderApiVersionReader(String parameterName)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            _parameterName = parameterName;
        }

        public void AddParameters(IApiVersionParameterDescriptionContext context)
        {
            context.AddParameter(_parameterName, ApiVersionParameterLocation.Query);
            context.AddParameter(_parameterName, ApiVersionParameterLocation.Header);
        }

        public String Read(HttpRequest request)
        {
            String version = String.Empty;

            StringValues values;
            if (request.Query.TryGetValue(_parameterName, out values))
            {
                version = values.ToString();
            }

            if (request.Headers.TryGetValue(_parameterName, out values))
            {
                version = values.ToString();
            }

            return version;
        }
    }
}
