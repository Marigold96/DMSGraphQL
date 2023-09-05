using GraphQL;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Client.Services.Providers;

public class GraphQLHttpClientProtected : GraphQLHttpClient
{

    public GraphQLHttpClientProtected() : base("http://localhost:5036/graphql/", new NewtonsoftJsonSerializer())
    {
    }

    public async Task<GraphQLResponse<TResponse>> SendQueryProtectedAsync<TResponse>(GraphQLRequest request, string? token = null, CancellationToken cancellationToken = default)
    {
        if(token != null)
        {
            this.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        return await base.SendQueryAsync<TResponse>(request, cancellationToken);
    }

    public async Task<GraphQLResponse<TResponse>> SendMutationProtectedAsync<TResponse>(GraphQLRequest request, string? token = null, CancellationToken cancellationToken = default)
    {
        if (token != null)
        {
            this.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        return await base.SendMutationAsync<TResponse>(request, cancellationToken);
    }

}
