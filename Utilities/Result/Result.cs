using Microsoft.AspNetCore.Components.Forms.Mapping;

namespace Tessa.Utilities.Result;
/// <summary>
/// Result wrapper for failable returns. To ensure the data integrity result can be set only once.
/// </summary>
/// <typeparam name="V">Enumeration type describing result</typeparam>
public class Result<V>
{
    private bool _setFired;
    private V? _value = default!;

    public Result(){}
    public Result(V? value)
    {
        SetResult(value);
    }
    
    /// <summary>
    /// Tries to retrieve result of an operation.
    /// </summary>
    /// <param name="value">Result data</param>
    /// <returns>Returns true if the data was set. If the action has not yet happened returns false with null data.</returns>
    public bool TryGetResult(out V? value)
    {
        if (_setFired == false)
        {
            value = default(V);
            return false;
        }

        value = _value;
        return true;
    }
    
    /// <summary>
    /// Sets the result for the operation.
    /// </summary>
    /// <param name="value">Result value</param>
    /// <exception cref="AlreadyFiredException">Result can be set only once</exception>
    public void SetResult(V? value)
    {
        if (_setFired == true) throw new AlreadyFiredException();
        _setFired = true;
        _value = value;
    }
    
}