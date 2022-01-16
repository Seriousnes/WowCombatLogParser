using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Utility
{
    public delegate T ObjectActivator<T>(params object[] args);

    public static class CombatLogActivator
    {
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = paramsInfo
                .ToList()
                .Select((p, i) =>
                {
                    Expression index = Expression.Constant(i);
                    Type paramType = p.ParameterType;
                    Expression paramAccessorExp = Expression.ArrayAccess(param, index);
                    Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                    return paramCastExp;
                })
                .ToArray();

            NewExpression newExp = Expression.New(ctor, argsExp);
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);
            return (ObjectActivator<T>)lambda.Compile();
        }
    }
}
