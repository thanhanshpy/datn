using Commands;
using System;
using System.Collections;
using UnityEngine;

namespace Testing
{
    public class DatabaseExtendtionExample : DatabaseExtention
    {
        new public static void Extend(CommandDatabase database)
        {
            //add command with no parameters
            database.AddCommand("print", new Action(PrintDefaultMessage));
            database.AddCommand("print_1p", new Action<string>(PrintUserMessage));
            database.AddCommand("print_mp", new Action<string[]>(PrintLine));

            //add lambda with no parameters
            database.AddCommand("lambda", new Action(() => { Debug.Log(" printing default message from lambda"); }));
            database.AddCommand("lambda_1p", new Action<string>((arg) => { Debug.Log($"log user lambda message: '{arg}'"); }));
            database.AddCommand("lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

            //add coroutine with no parameters
            database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
            database.AddCommand("process_1p", new Func<string, IEnumerator>(LineProcess));
            database.AddCommand("process_mp", new Func<string[], IEnumerator>(MultiLineProcess));

            database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));
        }

        private static void PrintDefaultMessage()
        {
            Debug.Log(" printing default message");
        }

        private static void PrintUserMessage(string message)
        {
            Debug.Log($"User message: '{message}'");
        }
        private static void PrintLine(string[] lines)
        {
            int i = 1;
            foreach (string line in lines)
            {
                Debug.Log($"{i++}. '{line}'");
            }
        }

        private static IEnumerator SimpleProcess()
        {
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"process running ... [{i}]");
                yield return new WaitForSeconds(1);
            }
        }

        private static IEnumerator LineProcess(string data)
        {
            if (int.TryParse(data, out int num))
            {
                for (int i = 1; i <= num; i++)
                {
                    Debug.Log($"process running ... [{i}]");
                    yield return new WaitForSeconds(1);
                }
            }
        }

        private static IEnumerator MultiLineProcess(string[] data)
        {
            foreach (string line in data)
            {
                Debug.Log($"process running ... [{line}]");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator MoveCharacter(string direction)
        {
            bool left = direction.ToLower() == "left";

            // Get the variables I need. This would be defined somewhere else.
            Transform character = GameObject.Find("Image").transform;
            float moveSpeed = 15;

            // Calculate the target position for the image
            float targetX = left ? -7 : 7;

            // Calculate the current position of the image
            float currentX = character.position.x;

            // Move the image gradually towards the target position
            while (Mathf.Abs(targetX - currentX) > 0.1f)
            {
                currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
                character.position = new Vector3(currentX, character.position.y, character.position.z);
                yield return null;
            }
        }
    }
}