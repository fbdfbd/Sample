using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SimpleContainer : IServiceCollection, IObjectResolver
{
    private readonly Dictionary<Type, object> _singletonInstances = new();
    private readonly Dictionary<Type, Type> _singletonImplementations = new();

    public void RegisterSingleton<TService>(TService instance)
    {
        _singletonInstances[typeof(TService)] = instance;
    }

    public void RegisterSingleton<TService, TImplementation>()
        where TImplementation : TService
    {
        _singletonImplementations[typeof(TService)] = typeof(TImplementation);
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    private object Resolve(Type serviceType)
    {
        if (_singletonInstances.TryGetValue(serviceType, out object instance))
        {
            return instance;
        }

        if (_singletonImplementations.TryGetValue(serviceType, out Type implementationType))
        {
            object createdInstance = CreateInstance(implementationType);
            _singletonInstances[serviceType] = createdInstance;
            return createdInstance;
        }

        if (serviceType.IsInterface || serviceType.IsAbstract)
        {
            throw new InvalidOperationException($"등록되지 않은 타입입니다: {serviceType.FullName}");
        }

        // 등록되지 않은 일반 클래스는 필요할 때마다 직접 생성합니다.
        return CreateInstance(serviceType);
    }

    private object CreateInstance(Type concreteType)
    {
        // 생성자 주입은 가장 파라미터가 많은 public 생성자를 기준으로 진행합니다.
        var constructor = concreteType
            .GetConstructors()
            .OrderByDescending(info => info.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"공용 생성자가 없습니다: {concreteType.FullName}");
        }

        var parameters = constructor.GetParameters();

        if (parameters.Length == 0)
        {
            return Activator.CreateInstance(concreteType);
        }

        object[] arguments = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            arguments[i] = Resolve(parameters[i].ParameterType);
        }

        return constructor.Invoke(arguments);
    }
}
