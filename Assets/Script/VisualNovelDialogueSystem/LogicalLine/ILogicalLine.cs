using System.Collections;
using UnityEngine;

namespace Dialouge.LogicalLines
{
    public interface ILogicalLine
    {
        string keyWord {  get; }
        bool Matches(DialougeLine line);
        IEnumerator Execute(DialougeLine line);
    }
}