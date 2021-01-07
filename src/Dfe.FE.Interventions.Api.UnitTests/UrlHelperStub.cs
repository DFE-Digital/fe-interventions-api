using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Dfe.FE.Interventions.Api.UnitTests
{
    public class UrlHelperStub : IUrlHelper
    {
        public UrlHelperStub(string protocol = "https", string host = "localhost", int port = 1234, string path = null)
        {
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
            var url = new StringBuilder(BaseUrl);
            var properties = actionContext.Values != null ? actionContext.Values.GetType().GetProperties() : new PropertyInfo[0];

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var propertyValue = property.GetValue(actionContext.Values);
                if (propertyValue == null)
                {
                    continue;
                }

                url.Append(i == 0 ? "?" : "&");
                url.Append(property.Name);
                url.Append("=");
                url.Append(propertyValue);
            }

            return url.ToString();
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