using Commands;
using System.Collections;
using UnityEngine;

public class CommandTesting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Running());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CommandManager.instance.Execute("moveCharDemo", "left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CommandManager.instance.Execute("moveCharDemo", "right");
        }
    }
    IEnumerator Running()
    {
        yield return CommandManager.instance.Execute("print");
        yield return CommandManager.instance.Execute("print_1p", "1234kjlkd");
        yield return CommandManager.instance.Execute("print_mp", "asd", "asdf", "asdf");

        yield return CommandManager.instance.Execute("lambda");
        yield return CommandManager.instance.Execute("lambda_1p", "lambda");
        yield return CommandManager.instance.Execute("lambda_mp", "asd", "asdf", "asdf");

        yield return CommandManager.instance.Execute("process");
        yield return CommandManager.instance.Execute("process_1p", "3");
        yield return CommandManager.instance.Execute("process_mp", "asd", "asdf", "asdf");
        
    }
}
