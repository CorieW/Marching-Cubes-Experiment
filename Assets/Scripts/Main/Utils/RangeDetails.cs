using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangeDetails
{
    [SerializeField] private RangeDetail[] _rangeDetails;

    public float GetDetailAtRange(float range)
    {
        foreach (RangeDetail rangeDetail in _rangeDetails)
        {
            if (range <= rangeDetail.range) return rangeDetail.detail;
        }

        // Not in range, therefore has no detail.
        return 0;
    }

    public RangeDetail[] GetRangeDetailsArray()
    {
        return _rangeDetails;
    }

    public int GetMaxRange()
    {
        return _rangeDetails[_rangeDetails.Length - 1].range;
    }
}
/// <summary>Represents at what range should a specific chunk detail show.</summary>
[System.Serializable]
public class RangeDetail
{
    /// <summary>
    /// Range is measured in chunks.
    /// <para>For example, a range of 8 means 8 chunks away.</para>
    /// </summary>
    public int range;
    [Range(0f, 1f)]
    public float detail;
}