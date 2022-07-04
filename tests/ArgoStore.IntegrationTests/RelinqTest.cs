

using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace ArgoStore.IntegrationTests
{
    public class RelinqTest : IntegrationTestsBase
    {
        public RelinqTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Test()
        {
            ExpressionTransformerRegistry transformerRegistry = ExpressionTransformerRegistry.CreateDefault();

            CompoundExpressionTreeProcessor processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);

            MethodInfoBasedNodeTypeRegistry nodeTypeRegistry = MethodInfoBasedNodeTypeRegistry.CreateFromRelinqAssembly();
            // registerNodeTypes?.Invoke(nodeTypeRegistry);

            ExpressionTreeParser expressionTreeParser = new ExpressionTreeParser(nodeTypeRegistry, processor);
            QueryParser parser = new QueryParser(expressionTreeParser);
            
        }
    }
}
