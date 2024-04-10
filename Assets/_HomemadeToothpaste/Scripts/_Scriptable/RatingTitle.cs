using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "rating_string", menuName = "Rating Title", order = 6)]
public class RatingTitle : ScriptableObject
{
    public string failedComment;
    public string badComment;
    public string neutralComment;
    public string goodComment;
}
