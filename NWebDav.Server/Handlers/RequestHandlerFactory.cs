﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using NWebDav.Server.Http;

namespace NWebDav.Server.Handlers
{
    public class RequestHandlerFactory : IRequestHandlerFactory
    {
        private static readonly IDictionary<string, Type> s_requestHandlers = new Dictionary<string, Type>();

        static RequestHandlerFactory()
        {
            var assemblyTypes = typeof(RequestHandlerFactory).GetTypeInfo().Assembly.DefinedTypes;
            foreach (var requestHandlerType in assemblyTypes.Where(t => typeof(IRequestHandler).GetTypeInfo().IsAssignableFrom(t)))
            {
                // Obtain the verbs of the request handler
                foreach (var verbAttribute in requestHandlerType.CustomAttributes.OfType<VerbAttribute>())
                    s_requestHandlers.Add(verbAttribute.Verb, requestHandlerType.AsType());
            }
        }

        public IRequestHandler GetRequestHandler(IHttpContext httpContext)
        {
            // Obtain the dispatcher
            Type requestHandlerType;
            if (!s_requestHandlers.TryGetValue(httpContext.Request.HttpMethod, out requestHandlerType))
                return null;

            // Create an instance of the request handler
            return (IRequestHandler)Activator.CreateInstance(requestHandlerType);
        }
    }
}
