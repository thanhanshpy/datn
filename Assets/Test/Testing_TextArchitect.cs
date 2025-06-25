using UnityEngine;
using TMPro;
using Dialouge;
namespace Testing
{
    public class Testing_TextArchitect : MonoBehaviour
    {
        DialougeSystem ds;
        TextArchitect architect;
        

        string[] line = new string[5]
        {
            "as;djfhaihkasj aksdj kjsfhaskjdf",
            "aksdfhn23784saadfh q82yriuhjf uygquhj",
            "ajkdhkahjshdlkahwejkajk ajbfjahwbejhb ",
            "aksldfhanafuifhkawh awhej98rifafw fiuhawkjfhaiwhfaw",
            "awoeiu,mxnc,sownh ,asdnkaehn KHKJLHKHIHKJHKJK"
        };
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //ds = DialougeSystem.instance;
            //architect = new TextArchitect(ds.dialougeContainer.text);
            //architect.buildMenthod = TextArchitect.BuildMenthod.typewriter;
            //architect.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            string longline = "ksldhf kkasjdhfoawe kashdfeh akhefkj h ajfkaewua  kafhiae kasjfhweu hksadfh iwh akdhfiuaweh kasjfh iawue ksldhf kkasjdhfoawe kashdfeh akhefkj h ajfkaewua  kafhiae kasjfhweu hksadfh iwh akdhfiuaweh kasjfh iawu ";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                }
                else
                    architect.Build(longline);
                    //architect.Build(line[Random.Range(0, line.Length)]);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                architect.Append(longline);
                //architect.Append(line[Random.Range(0, line.Length)]);
            }
        }
    }
}

