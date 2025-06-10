using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinqTest : MonoBehaviour
{
    private void Start()
    {
        List<Student> students = new List<Student>
       {
           new Student { StudentName = "ASFD", StudentAge = 0, Gender = "male" },
           new Student { StudentName = "QWER", StudentAge = 1, Gender = "female" },
           new Student { StudentName = "ZXC", StudentAge = 2, Gender = "male" },
           new Student { StudentName = "POIU", StudentAge = 3, Gender = "female" },
           new Student { StudentName = "LKJH", StudentAge = 4, Gender = "male" },
           new Student { StudentName = "MNBV", StudentAge = 5, Gender = "female" }
       };

        var all = from student in students select student;
        //foreach (var student in all)
        //{
        //    Debug.Log(student);
        //}

        // Fix: Use a boolean variable to store the result of the All() method  
        var areAllStudentsOlderThan20 = students.All(x => x.StudentAge > 20);

        // Fix: Use Where() to filter the list of students based on gender  
        var maleGroup = students.Where(x => x.Gender == "male").ToList();

        var test = from student in students
                   where student.Gender == "Female"
                   select student;
        foreach(var student in test)
        {
            Debug.Log(student);
        }

        var test2 = test.OrderByDescending(x => x.StudentAge).ToList();

        var maxtest = all.Max(x => x.StudentAge);
        var dis = students.Distinct().ToList();

        var distinctByAge = students.GroupBy(x => x.StudentAge).Select(g => g.First()).ToList();

        var maleCount = students.Count(x => x.Gender == "male");


    }
}
