using System;
using System.Linq.Expressions;

namespace RealEstate.Base
{
    public class HtmlTagViewModel<T>
    {
        public HtmlTagViewModel(string modelName, Expression<Func<T, object>> expression)
        {
            Name = $"{modelName}.{expression.GetProperty().Name}";
            Id = Name.Replace(".", "_");
        }

        public HtmlTagViewModel(Expression<Func<T, object>> expression)
        {
            Name = $"{expression.GetProperty().Name}";
            Id = Name.Replace(".", "_");
        }

        public string Name { get; set; }
        public string Id { get; set; }
    }
}