using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.ExtensionMethods
{
    public static class ExpressionExtensions
    {

        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    if (unaryExpression.NodeType == ExpressionType.ArrayLength)
                        return "Length";
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                    {
                        var methodCallExpression = unaryExpression.Operand as MethodCallExpression;
                        if (methodCallExpression == null)
                            throw new NotImplementedException();

                        var arg = (ConstantExpression)methodCallExpression.Arguments[2];
                        return ((MethodInfo)(arg.Value)).Name;
                    }
                }
                else
                    throw new NotImplementedException();

            }

            var propertyName = memberExpression.Member.Name;
            return propertyName;

        }

        public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;

                if (unaryExpression != null)
                {
                    if (unaryExpression.NodeType == ExpressionType.ArrayLength)
                        return "Length";
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                    {
                        var methodCallExpression = unaryExpression.Operand as MethodCallExpression;
                        if (methodCallExpression == null)
                            throw new NotImplementedException();

                        var arg = (ConstantExpression)methodCallExpression.Arguments[2];
                        return ((MethodInfo)(arg.Value)).Name;
                    }
                }
                else
                    throw new NotImplementedException();
            }
            var propertyName = memberExpression.Member.Name;
            return propertyName;

        }

        public static String PropertyToString<R>(this Expression<Func<R>> action)
        {
            MemberExpression ex = (MemberExpression)action.Body;
            return ex.Member.Name;
        }

        public static void CheckIsNotNull<R>(this Expression<Func<R>> action)
        {
            CheckIsNotNull(action, null);
        }

        public static void CheckIsNotNull<R>(this Expression<Func<R>> action, string message)
        {
            MemberExpression ex = (MemberExpression)action.Body;
            string memberName = ex.Member.Name;
            if (action.Compile()() == null)
            {
                throw new ArgumentNullException(memberName, message);
            }
        }

    }
}
