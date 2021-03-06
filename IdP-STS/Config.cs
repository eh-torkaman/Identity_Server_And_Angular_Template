// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace STS
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources =>
             new ApiResource[]
            {
                new ApiResource(){Name="IdPApi",Scopes={ "IdPApi"} },//,ApiSecrets= { new Secret("IdPApi_secret") }
                 new ApiResource(){Name="WebApi1",Scopes={ "WebApi1"},ApiSecrets= { new Secret("secret123") } }
            };
    public static IEnumerable<IdentityResource> IdentityResources =>      
            new IdentityResource[]  {   
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
             new ApiScope[]
            {
                new ApiScope("IdPApi"),
                new ApiScope("WebApi1")
            };

    public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("not yet defined client".Sha256()) },
                   // AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("not yet defined client".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile"}
                },
                  new Client
                {
                    ClientId = "spa-client",
                    ClientName = "Projects SPA",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,


                    RedirectUris =           { "http://localhost:4200/signin-callback" , "http://localhost:4200/assets/silent-callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:4200/signout-callback" },
                    AllowedCorsOrigins =     { "http://localhost:4200" },
                 //   FrontChannelLogoutUri=  "http://localhost:4200/signout-callback" ,
                
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "IdPApi","WebApi1"
                    },
                    AccessTokenLifetime = 600
                },

            };
    }
}
