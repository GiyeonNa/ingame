using UnityEngine;

public class Student : MonoBehaviour
{

    public string StudentName;
    public int StudentAge;
    public string Gender;

    public override string ToString()
    {
        return $"{StudentName} ({StudentAge} {Gender})";
    }
}
