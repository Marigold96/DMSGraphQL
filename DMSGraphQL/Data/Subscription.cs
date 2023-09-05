using Common.Models;

namespace DMSGraphQL.Data;

public class Subscription
{
    [Subscribe]
    [Topic("DocumentShared")]
    public SharedDocumentModel DocumentShared([EventMessage] SharedDocumentModel sharedDocument)
       => sharedDocument;
}
