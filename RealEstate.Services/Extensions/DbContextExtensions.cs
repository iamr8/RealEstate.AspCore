using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Reflection;

namespace RealEstate.Services.Extensions
{
    public static class DbContextExtensions
    {
        public static IQueryable<object> AsQueryable(this DbContext context, string entityName)
        {
            var model = context.Model;

            var nameSpace = context.GetType().Namespace;
            var entityFullName = $"{nameSpace}.{nameof(RealEstate.Services.Database.Tables)}.{entityName}";
            var entityType = model.FindEntityType(entityFullName);
            var clrType = entityType.ClrType;
            var query = context.AsQueryable(clrType);
            return query;
        }

        public static IQueryable<object> AsQueryable(this DbContext context, Type entityType)
        {
            var contextType = typeof(DbContext);
            var setter = contextType.GetMethod(nameof(DbContext.Set));
            var iqueryable = setter.MakeGenericMethod(entityType).Invoke(context, null) as IQueryable<object>;
            return iqueryable;
        }

        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Microsoft.EntityFrameworkCore.Storage.Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }
    }
}