using System.Collections;
using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions;

internal sealed class NamedJobCollection : INamedJobCollection
{
    private readonly IDictionary<string, JobDescriptor> _namedJobCollectionImplementation = new Dictionary<string, JobDescriptor>();

    public NamedJobCollection(IEnumerable<IJobDescriptorBuilder> jobDescriptorBuilders)
    {
        foreach (var builder in jobDescriptorBuilders)
        {
            _namedJobCollectionImplementation[builder.Name] = builder.Build();
        }
    }
    
    public IEnumerator<KeyValuePair<string, JobDescriptor>> GetEnumerator()
    {
        return _namedJobCollectionImplementation.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_namedJobCollectionImplementation).GetEnumerator();
    }

    public void Add(KeyValuePair<string, JobDescriptor> item)
    {
        _namedJobCollectionImplementation.Add(item);
    }

    public void Clear()
    {
        _namedJobCollectionImplementation.Clear();
    }

    public bool Contains(KeyValuePair<string, JobDescriptor> item)
    {
        return _namedJobCollectionImplementation.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, JobDescriptor>[] array, int arrayIndex)
    {
        _namedJobCollectionImplementation.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, JobDescriptor> item)
    {
        return _namedJobCollectionImplementation.Remove(item);
    }

    public int Count => _namedJobCollectionImplementation.Count;

    public bool IsReadOnly => _namedJobCollectionImplementation.IsReadOnly;

    public void Add(string key, JobDescriptor value)
    {
        _namedJobCollectionImplementation.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _namedJobCollectionImplementation.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _namedJobCollectionImplementation.Remove(key);
    }

    public bool TryGetValue(string key, out JobDescriptor value)
    {
        return _namedJobCollectionImplementation.TryGetValue(key, out value!);
    }

    public JobDescriptor this[string key]
    {
        get => _namedJobCollectionImplementation[key];
        set => _namedJobCollectionImplementation[key] = value;
    }

    public ICollection<string> Keys => _namedJobCollectionImplementation.Keys;

    public ICollection<JobDescriptor> Values => _namedJobCollectionImplementation.Values;
}