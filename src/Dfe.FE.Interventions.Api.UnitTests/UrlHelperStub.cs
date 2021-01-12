using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Dfe.FE.Interventions.Api.UnitTests
{
    public class UrlHelperStub : IUrlHelper
    {
        private readonly string _routeTemplate;
        private readonly string[] _queryParameterNames;

        public UrlHelperStub(MethodInfo actionMethod, string protocol = "https", string host = "localhost", int port = 1234, string path = null)
        {
            if (actionMethod == null)
            {
                throw new ArgumentNullException(nameof(actionMethod));
            }
            
            var routeAttribute = actionMethod.GetCustomAttribute<RouteAttribute>();
            _routeTemplate = routeAttribute?.Template;
            
            var parameters = actionMethod.GetParameters();
            _queryParameterNames = parameters
                .Where(p => p.GetCustomAttribute<FromQueryAttribute>() != null)
                .Select(p => p.Name)
                .ToArray();
            
            ActionContext = new ActionContext(
                new DefaultHttpContext
                {
                    Request =
                    {
                        Scheme = protocol,
                        Host = new HostString(host, port),
                    },
                },
                new RouteData(),
                new ActionDescriptor());

            BaseUrl = $"{protocol}://{host}:{port}{path}";
        }

        public string BaseUrl { get; }

        public string Action(UrlActionContext actionContext)
        {
            var query = new StringBuilder();
            var routePath = !string.IsNullOrEmpty(_routeTemplate) ? $"/{_routeTemplate}" : string.Empty;
            
            var properties = actionContext.Values != null ? actionContext.Values.GetType().GetProperties() : new PropertyInfo[0];
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var propertyValue = property.GetValue(actionContext.Values);

                routePath = routePath.Replace($"{{{property.Name}}}", propertyValue?.ToString(), StringComparison.InvariantCultureIgnoreCase);
                
                if (propertyValue == null || !_queryParameterNames.Any(x=>x.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                query.Append(query.Length > 0 ? "&" : "?");
                query.Append(property.Name);
                query.Append("=");
                query.Append(propertyValue);
            }

            return $"{BaseUrl}{routePath}{query}";
        }

        public string Content(string contentPath)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLocalUrl(string url)
        {
            throw new System.NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            throw new System.NotImplementedException();
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            throw new System.NotImplementedException();
        }

        public ActionContext ActionContext { get; }
    }
}