using System.Collections;
using Azure;
using Azure.Data.Tables;

namespace Carby.JobManager.Functions.JobModel;

internal sealed class JobContext : IJobContext, ITableEntity
{
    private readonly IDictionary<string, object> _dictionaryImplementation;

    public string JobId { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey
    {
        get => JobId;
        set => JobId = value;
    }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public ICollection<object> Values => _dictionaryImplementation.Values;
    public JobContext(string jobName, Dictionary<string, object>? dictObject)
    {
        _dictionaryImplementation = dictObject ?? new Dictionary<string, object>();
        JobId = Guid.NewGuid().ToString("D");
        PartitionKey = jobName;
    }
    
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _dictionaryImplementation.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionaryImplementation).GetEnumerator();
    }

    public void Add(KeyValuePair<string, object> item)
    {
        _dictionaryImplementation.Add(item);
    }

    public void Clear()
    {
        _dictionaryImplementation.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return _dictionaryImplementation.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        _dictionaryImplementation.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return _dictionaryImplementation.Remove(item);
    }

    public int Count => _dictionaryImplementation.Count;

    public bool IsReadOnly => _dictionaryImplementation.IsReadOnly;

    public void Add(string key, object value)
    {
        _dictionaryImplementation.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _dictionaryImplementation.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _dictionaryImplementation.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return _dictionaryImplementation.TryGetValue(key, out value!);
    }

    public object this[string key]
    {
        get => _dictionaryImplementation[key];
        set => _dictionaryImplementation[key] = value;
    }

    public ICollection<string> Keys => _dictionaryImplementation.Keys;

}