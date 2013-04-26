using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints.Extensions;

namespace ServiceStackWinForm
{
    [TestFixture]
    public class Tester
    {
        [Test]
        public void testsomethign()
        {
            var mockedRequestContext = MockRepository.GenerateMock<IRequestContext>();
            var mockedHttpRequest = MockRepository.GenerateMock<IHttpRequest>();
            var mockedOriginalRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var mockedOriginalRequestContext = MockRepository.GenerateMock<RequestContext>();

            mockedOriginalRequest.Stub(x => x.RequestContext).Return(mockedOriginalRequestContext);
            mockedHttpRequest.Stub(x => x.OriginalRequest).Return(mockedOriginalRequest);

            mockedRequestContext.Stub(x => x.Get<IHttpRequest>()).Return(mockedHttpRequest);
            var service = new ServiceTests()
                {
                    RequestContext = mockedRequestContext
                };

            service.Delete(new DeleteRequest());
        }
    }

    public class DeleteRequest
    {
    }

    public class ServiceTests : Service
    {
        [RequireFormsAuthentication]
        public object Delete(DeleteRequest request)
        {
            var originalRequest = (HttpRequestBase) Request.OriginalRequest;
            var identity = originalRequest.RequestContext.HttpContext.User.Identity;
            return null;
            //return othercode(identity);
        }
    }

    public class RequireFormsAuthenticationAttribute : RequestFilterAttribute
    {
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            var originalRequest = (HttpRequest)req.OriginalRequest;
            var identity = originalRequest.RequestContext.HttpContext.User.Identity;
            if (!identity.IsAuthenticated)
            {
                res.StatusCode = (int)HttpStatusCode.Forbidden;
                res.EndServiceStackRequest(skipHeaders: true);
            }
        }
    }
}
