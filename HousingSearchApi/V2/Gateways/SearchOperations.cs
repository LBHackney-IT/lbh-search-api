using Nest;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HousingSearchApi.V2.Gateways;

static class SearchOperations
{
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
    NestedMultiMatch(string searchText, string path, Fields fields, TextQueryType matchType = TextQueryType.BestFields, Fuzziness fuzziness = null, int boost = 1)
    {
        return should => should
            .Nested(n => n
                .Path(path)
                .Query(qq => qq
                    .MultiMatch(mm =>
                        {
                            mm.Type(matchType)
                            .Query(searchText)
                            .Fields(fields)
                            .Boost(boost);
                            if (fuzziness != null)
                                mm.Fuzziness(fuzziness);
                            return mm;
                        }
                    )
                )
            );
    }

    public static Func<QueryContainerDescriptor<object>, QueryContainer>
    NestedMultiMatchPhrase(string searchText, string path, Fields fields, int boost = 1)
    {
        return should => should
            .Nested(n => n
                .Path(path)
                .Query(qq => qq
                    .MultiMatch(mm => mm
                        .Type(TextQueryType.Phrase)
                        .Query(searchText)
                        .Fields(fields)
                        .Boost(boost)
                    )
                )
            );
    }

    public static Func<QueryContainerDescriptor<object>, QueryContainer>
    SimpleQueryStringWords(string searchText, List<string> fields, int boost)
    {
        List<string> ProcessWildcards(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return new List<string>();
            return phrase.Split(' ').Select(word => $"*{word}*").ToList();
        }

        var listOfWildcardedWords = ProcessWildcards(searchText);
        var queryString = $"({string.Join(" AND ", listOfWildcardedWords)}) " + string.Join(" ", listOfWildcardedWords);

        return should => should
            .QueryString(qs => qs
                .Query(queryString)
                .Fields(fields.Select(f => (Field) f).ToArray())
                .DefaultOperator(Operator.And)
                .Boost(boost)
        );
    }

    // Score for matching a value which starts with the search text
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
        MatchPhrasePrefix(string searchText, string fieldName, int boost) =>
        should => should
            .MatchPhrasePrefix(mp => mp
                .Field(fieldName)
                .Query(searchText)
                .Boost(boost)
            );

    // Score for matching a single (best) field
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
        MultiMatchBestFields(string searchText, Fields fields = null, int boost = 1) =>
        should => should
            .MultiMatch(mm => mm
                .Fields(fields ?? new[] { "*" })
                .Query(searchText)
                .Type(TextQueryType.BestFields)
                .Operator(Operator.And)
                .Fuzziness(Fuzziness.Auto)
                .Boost(boost)
            );

    // Score for matching the combination of many fields
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
        MultiMatchCrossFields(string searchText, Fields fields = null, int boost = 1) =>
        should => should
            .MultiMatch(mm => mm
                .Fields(fields ?? new[] { "*" })
                .Query(searchText)
                .Type(TextQueryType.CrossFields)
                .Operator(Operator.Or)
                .Boost(boost)
            );

    // Score for matching a high number (quantity) of fields
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
        MultiMatchMostFields(string searchText, int boost, Fields fields = null) =>
        should => should
            .MultiMatch(mm => mm
                .Fields(fields ?? new[] { "*" })
                .Query(searchText)
                .Type(TextQueryType.MostFields)
                .Operator(Operator.Or)
                .Fuzziness(Fuzziness.Auto)
                .Boost(boost)
            );


    // Score for matching a value which contains the search text
    public static Func<QueryContainerDescriptor<object>, QueryContainer>
        WildcardMatch(string searchText, string fieldName, int boost)
    {
        List<string> ProcessWildcards(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return new List<string>();
            return phrase.Split(' ').Select(word => $"*{word}*").ToList();
        }

        var listOfWildcardedWords = ProcessWildcards(searchText);
        var wildcardQueries = listOfWildcardedWords.Select(term => new WildcardQuery
        {
            Field = fieldName,
            Value = $"*{term}*",
            Boost = boost
        }).ToList();

        return q => q.Bool(b => b
            .Should(wildcardQueries.Select(wq =>
                new QueryContainerDescriptor<object>()
                    .Wildcard(w => w
                        .Field(wq.Field)
                        .Value(wq.Value)
                        .Boost(wq.Boost)
                    )
                ).ToArray()
            )
        );
    }
}
