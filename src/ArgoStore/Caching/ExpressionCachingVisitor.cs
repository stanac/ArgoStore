using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Command;
using ArgoStore.Helpers;

namespace ArgoStore.Caching;

/// <summary>
/// Transforms expression, replaces constants and members with parameters
/// and create caching key
/// </summary>
internal class ExpressionCachingVisitor : ExpressionVisitor
{
    public ArgoCommandParameterCollection Params { get; private set; } = new();
    public int ExpressionHash => _hash.ToHashCode();
    public bool CanBeCached { get; private set; } = true;

    private HashCode _hash = new ();

    public override Expression? Visit(Expression? node)
    {
        return base.Visit(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Type.IsSupportedPrimitiveType())
        {
            string paramName = Params.AddNewParameter(node.Value!, ArgoCommandParameter.TransformPrefix);

            ParameterExpression p = Expression.Parameter(node.Type, paramName);
            Trace.WriteLine($"Adding parameter: `{paramName}`");

            AddHash(node.Type);
            AddHash(paramName);
            return p;
        }

        CanBeCached = false;
        return base.VisitConstant(node);
    }
    
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member is PropertyInfo pi)
        {
            if (pi.DeclaringType == typeof(DateTime))
            {
                Func<object>? valueFactory = null;

                if (pi.Name == "Now")
                {
                    valueFactory = () => DateTime.Now;
                }
                else if (pi.Name == "UtcNow")
                {
                    valueFactory = () => DateTime.UtcNow;
                }

                if (valueFactory != null)
                {
                    string paramName = Params.AddNewParameter(valueFactory, ArgoCommandParameter.TransformPrefix);
                    return Expression.Parameter(node.Type, paramName);
                }
            }
        }
        else if (node.Member is FieldInfo fi)
        {

        }
        
        return base.VisitMember(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        AddHash(node.NodeType);
        return base.VisitBinary(node);
    }
    
    //protected override Expression VisitBlock(BlockExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitBlock(node);
    //}

    //protected override Expression VisitConditional(ConditionalExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitConditional(node);
    //}

    //protected override Expression VisitDebugInfo(DebugInfoExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitDebugInfo(node);
    //}

    //protected override Expression VisitDefault(DefaultExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitDefault(node);
    //}

    //protected override Expression VisitDynamic(DynamicExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitDynamic(node);
    //}

    //protected override Expression VisitExtension(Expression node)
    //{
    //    AddHash(node);
    //    return base.VisitExtension(node);
    //}

    //protected override Expression VisitGoto(GotoExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitGoto(node);
    //}

    //protected override Expression VisitIndex(IndexExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitIndex(node);
    //}

    //protected override Expression VisitInvocation(InvocationExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitInvocation(node);
    //}

    //protected override Expression VisitLabel(LabelExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitLabel(node);
    //}

    //protected override Expression VisitListInit(ListInitExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitListInit(node);
    //}

    //protected override Expression VisitLoop(LoopExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitLoop(node);
    //}

    //protected override Expression VisitMemberInit(MemberInitExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitMemberInit(node);
    //}

    //protected override Expression VisitMethodCall(MethodCallExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitMethodCall(node);
    //}

    //protected override Expression VisitNew(NewExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitNew(node);
    //}

    //protected override Expression VisitNewArray(NewArrayExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitNewArray(node);
    //}

    //protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitRuntimeVariables(node);
    //}

    //protected override Expression VisitSwitch(SwitchExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitSwitch(node);
    //}

    //protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitTypeBinary(node);
    //}

    //protected override Expression VisitUnary(UnaryExpression node)
    //{
    //    AddHash(node);
    //    return base.VisitUnary(node);
    //}
    
    private void AddHash(object o)
    {
        if (CanBeCached)
        {
            _hash.Add(o);
        }
    }
}