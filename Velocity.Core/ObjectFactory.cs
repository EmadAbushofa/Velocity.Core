using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Velocity.Core.Extensions;

namespace Velocity.Core
{
    public static class ObjectFactory
    {
        private delegate TObject ObjectActivator<out TObject>(params object[] args);

        private static ObjectActivator<TObject> GetActivator<TObject>
            (ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param =
                Expression.Parameter(typeof(object[]), "args");

            var argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                var paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                var paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda =
                Expression.Lambda(typeof(ObjectActivator<TObject>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<TObject>)lambda.Compile();
            return compiled;
        }

        public static TObject Create<TObject>(Type type, params object[] args)
        {
            var ctor = args.Length == 0
                ? type.GetTypeInfo().DeclaredConstructors.First(c => c.GetParameters().Length == 0)
                : type.GetTypeInfo().DeclaredConstructors.First();
            var createdActivator = GetActivator<TObject>(ctor);
            var instance = createdActivator(args);

            if (instance == null)
                throw new NullReferenceException("failed to create object of type : " + type);

            return instance;
        }

        public static async Task<TObject> CreateAsync<TObject>(Type type, params object[] args)
        {
            return await Task.Run(() => Create<TObject>(type, args));
        }

        public static TObject Create<TObject>()
        {
            return Create<TObject>(typeof(TObject));
        }

        public static void SetValue(this object obj, string prpertyName, object newValue)
        {
            obj.GetType().GetRuntimeProperty(prpertyName)?.SetValue(obj, newValue);
        }

        public static void SetValue<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> expression, TProperty newValue)
        {
            obj.GetType().GetRuntimeProperty(expression.ToExpressionTarget())?.SetValue(obj, newValue);
        }

        public static Task<bool> AreEqualToAsync<TObject>(this TObject obj1, TObject obj2)
        {
            return Task.Run(() => AreEqualTo(obj1, obj2));
        }

        public static bool AreEqualTo<TObject>(this TObject obj1, TObject obj2)
        {
            return obj1.GetType().GetRuntimeProperties().All(property => property.GetValue(obj1) == property.GetValue(obj2));
        }
    }
}