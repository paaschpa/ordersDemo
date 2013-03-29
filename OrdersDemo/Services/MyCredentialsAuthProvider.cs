using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints.Extensions;

namespace OrdersDemo.Services
{
    public class MyCredentialsAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(ServiceStack.ServiceInterface.IServiceBase authService, string userName, string password)
        {
            var session = authService.GetSession(false);
            if (session.Id == null)
            {
                var req = authService.RequestContext.Get<IHttpRequest>();
                var sessId = HttpContext.Current.Response.ToResponse().CreateSessionIds(req);
                session.Id = sessId;
                req.SetItem(SessionFeature.SessionId, sessId);
            }

            var authRepo = authService.TryResolve<IUserAuthRepository>();
            UserAuth userAuth = null;
            if (authRepo.TryAuthenticate(userName, password, out userAuth))
            {
                session.PopulateWith(userAuth);
                session.IsAuthenticated = true;
                session.UserAuthId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
                session.ProviderOAuthAccess = authRepo.GetUserOAuthProviders(session.UserAuthId)
                    .ConvertAll(x => (IOAuthTokens)x);

                authService.SaveSession(session, SessionExpiry);
                return true;
            }

            return false;
        }

    }
}