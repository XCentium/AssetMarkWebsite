using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Genworth.SitecoreExt.Services.Contracts.Data;

namespace Genworth.SitecoreExt.Services.Search
{
    [ServiceContract]
    [ServiceKnownType(typeof(Query))]
    [ServiceKnownType(typeof(BooleanQuery))]
    [ServiceKnownType(typeof(BooleanClause))]
    [ServiceKnownType(typeof(Occur))]
    [ServiceKnownType(typeof(WildcardQuery))]
    [ServiceKnownType(typeof(List<object>))]
    [ServiceKnownType(typeof(Document))]
    [ServiceKnownType(typeof(Field))]
    //[ServiceKnownType(typeof(Fieldable))]
    [ServiceKnownType(typeof(AbstractField))]
    [ServiceKnownType(typeof(TermQuery))]
    //[ServiceKnownType(typeof(Parameter))]
    [ServiceKnownType(typeof(PrefixQuery))]
    //[ServiceKnownType(typeof(ConstantScoreRangeQuery))]
    [ServiceKnownType(typeof(MatchAllDocsQuery))]
    public interface IGenSearchService
    {
        [OperationContract]
        List<Document> ExecuteQuery(Query query, string indexName);


        [OperationContract]
        List<Document> ExecuteBooleanQuery(BooleanQueryContract query, string indexName);

        [OperationContract]
        Dictionary<string, int> ExecuteGroupCountQuery(Query query, string indexName, string field);
    }
}
