using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace ArgoStore.Implementations;

internal class ArgoStoreQueryParser : IQueryParser
{
    private readonly QueryParser _parser;

    public ArgoStoreQueryParser()
    {
        CompoundExpressionTreeProcessor proc = ExpressionTreeParser.CreateDefaultProcessor(ExpressionTransformerRegistry.CreateDefault());
        MethodInfoBasedNodeTypeRegistry reg = MethodInfoBasedNodeTypeRegistry.CreateFromRelinqAssembly();

        ExpressionTreeParser expressionTreeParser = new ExpressionTreeParser(reg, proc);
        _parser = new QueryParser(expressionTreeParser);
    }

    public QueryModel GetParsedQuery(Expression expressionTreeRoot) => _parser.GetParsedQuery(expressionTreeRoot);
}