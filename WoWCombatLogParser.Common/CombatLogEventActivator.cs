﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WoWCombatLogParser;

public delegate object ObjectActivator(params object[] args);

public static class CombatLogEventActivator
{
    public static ObjectActivator GetActivator<T>(ConstructorInfo ctor)
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
        LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);
        return (ObjectActivator)lambda.Compile();
    }
}
