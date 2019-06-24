using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Util.Dto.Configuration;

namespace webapp.Util.Helpers
{
    public class IdentityConfigHelpers
    {
        // http://docs.identityserver.io/en/latest/topics/resources.html
        // define the resources to protect (identity resource has unique name and you can assign arbitrary claim types to it)
        // will be included in the identity toekn for the user (client will use the scope parameter to request access to identity resource)
        // OpenID Connect spec - minimum requirement is you provide support for emitting a unique ID for your users (the subject id)
        // this is done by exposing the standard identity resource (openid)
        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>()
        {
            new IdentityResources.OpenId() // TO-DO move to separate config file
        };

        // http://docs.identityserver.io/en/latest/topics/resources.html#defining-api-resources
        // to allow clients to request access tokens for your APIs you need to define API resources
        // to get access tokesn for APIs you need to register them as a scope
        public static IEnumerable<ApiResource> Apis = new List<ApiResource>()
        {
            // this assigns scope for using the AddLocalApiAuthentication helper
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<Client> GetClients(AppConfig config)
        {
            return new List<Client>()
                {
                    // backend client
                    new Client
                    {
                        ClientId = "client",
                        ClientName = "Backend Client",

                        // no interactive user, use the clientid/secret for authentication
                        // (server-to-server communication - tokens always requested on behalf of client, not user)
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret(config.Authorization.Secret.Sha256())
                        },

                        // scopes the client has access to
                        AllowedScopes = { IdentityServerConstants.LocalApi.ScopeName }
                    },

                    // JavaScript client
                    new Client
                    {
                        ClientId = "spa",
                        ClientName = "JavaScript Client",
                        // for Javascript-based applications use Authorization Code with PKCE instead of implicit
                        // Code (Authorization Code) = provides a way to retrieve tokens on a back-channel as opposed to browser front-channel
                        AllowedGrantTypes = GrantTypes.Code,
                        // "Proof Key for Code Exchange" - way to make OAuth 2.0 and OpenID Connect operations using an authorization code more secure
                        // applies to authorization/token requests whenever code grant type is involved
                        RequirePkce = true,
                        RequireClientSecret = false,

                        RedirectUris = {$"{config.Authorization.ClientHostUrl}/callback.html"},
                        PostLogoutRedirectUris = {$"{config.Authorization.ClientHostUrl}/index.html"},
                        // TODO - always use current address (add additional for local dev CORS)
                        AllowedCorsOrigins = {$"{config.Authorization.ClientHostUrl}" },

                        // scopes the client has access to
                        AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.LocalApi.ScopeName }
                    }
                };
        }

    }
}
