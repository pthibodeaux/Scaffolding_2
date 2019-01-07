using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffolding.Core.Tests.DI
{
    public class DummyServiceCollection : IServiceCollection
    {
		private readonly List<ServiceDescriptor> _inner = new List<ServiceDescriptor>();

	    public IEnumerator<ServiceDescriptor> GetEnumerator()
	    {
		    return _inner.GetEnumerator();

	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    return GetEnumerator();
	    }

	    public void Add(ServiceDescriptor item)
	    {
		    _inner.Add(item);
	    }

	    public void Clear()
	    {
		    _inner.Clear();
	    }

	    public bool Contains(ServiceDescriptor item)
	    {
		    return _inner.Contains(item);
	    }

	    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
	    {
		   _inner.CopyTo(array, arrayIndex);
	    }

	    public bool Remove(ServiceDescriptor item)
	    {
		    return _inner.Remove(item);
	    }

	    public int Count => _inner.Count;
	    public bool IsReadOnly { get; }
	    public int IndexOf(ServiceDescriptor item)
	    {
		    return _inner.IndexOf(item);
	    }

	    public void Insert(int index, ServiceDescriptor item)
	    {
		    _inner.Insert(index, item);
	    }

	    public void RemoveAt(int index)
	    {
		    _inner.RemoveAt(index);
	    }

	    public ServiceDescriptor this[int index]
	    {
		    get => _inner[index];
		    set => _inner[index] = value;
	    }
    }
}
