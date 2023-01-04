using System.Text;
using ArgoStore.Statements;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace ArgoStore;

internal class ArgoCommandBuilder
{
    private readonly Type _docType;
    private readonly Dictionary<string, object> _params = new();
    private readonly List<WhereStatement> _whereStatements = new();

    public string ItemName { get; set; }

    public ArgoCommandBuilder(QueryModel model)
        : this (model.MainFromClause.ItemType)
    {
    }

    public ArgoCommandBuilder(Type docType)
    {
        _docType = docType;
    }

    public ArgoCommand Build()
    {
        StringBuilder sb = StringBuilderBag.Default.Get();

        AddSelect(sb);
        AddWhere(sb);

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        return new ArgoCommand(sql, _params);
    }

    public void AddWhereClause(WhereClause whereClause, QueryModel queryModel)
    {
        _whereStatements.Add(new WhereStatement(whereClause, queryModel));
    }

    private void AddSelect(StringBuilder sb)
    {
        sb.Append("SELECT *");
    }

    private void AddFrom()
    {

    }

    private void AddWhere(StringBuilder sb)
    {

    }
}