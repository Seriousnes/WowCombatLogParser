using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WoWCombatLogParser.Parser;

internal delegate object CombatLogEventConstructor(params object[] args);

internal static class CombatLogEventActivator
{
    public static CombatLogEventConstructor GetCombatLogEventConstructor(Type type)
    {
        return GetCombatLogEventConstructor(type.GetConstructor([])!);
    }

    public static CombatLogEventConstructor GetCombatLogEventConstructor(ConstructorInfo ctor)
    {
        Type type = ctor.DeclaringType!;
        ParameterInfo[] paramsInfo = ctor.GetParameters();

        ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
        Expression[] argsExp = [.. paramsInfo
            .ToList()
            .Select((p, i) =>
            {
                Expression index = Expression.Constant(i);
                Type paramType = p.ParameterType;
                Expression paramAccessorExp = Expression.ArrayAccess(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                return paramCastExp;
            })];

        NewExpression newExp = Expression.New(ctor, argsExp);
        LambdaExpression lambda = Expression.Lambda(typeof(CombatLogEventConstructor), newExp, param);
        return (CombatLogEventConstructor)lambda.Compile();
    }
}
