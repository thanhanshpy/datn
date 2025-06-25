using UnityEngine;

namespace Testing
{
    public class VariableStoresTesting : MonoBehaviour
    {
        public int var_int = 0;
        public float flt = 0;
        public bool var_bool = false;
        public string var_srt = "";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            VariableStore.CreateDatabase("DB1");
            VariableStore.CreateDatabase("DB544");
            VariableStore.CreateDatabase("DB723");

            VariableStore.CreateVariable("L_int", var_int, ()=>  var_int, value => var_int = value);

            VariableStore.CreateVariable("DB1.jdis", 1);
            VariableStore.CreateVariable("DB1.asdf", 2);
            VariableStore.CreateVariable("DB544.xcv", true);
            VariableStore.CreateVariable("DB544.werd", 7.5f);
            VariableStore.CreateVariable("DB723.jdis", "d23ds");
            VariableStore.CreateVariable("DB723.pkrdlei", "lkoim");

            VariableStore.PrintAllDatabases();

            VariableStore.PrintAllVariables();
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.A))
            {
                VariableStore.PrintAllVariables();
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                string variable = "DB1.jdis";
                VariableStore.TryGetValue(variable, out object v);
                VariableStore.TrySetValue(variable, (int)v + 5);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                VariableStore.TryGetValue("DB1.jdis", out object first);
                VariableStore.TryGetValue("DB1.asdf", out object second);

                Debug.Log($"first + second = {(int)first + (int)second}");
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                VariableStore.TryGetValue("L_int", out object linked_integer);
                VariableStore.TrySetValue("L_int", (int)linked_integer + 5);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                VariableStore.RemoveVariable("DB723.pkrdlei");
            }

            if (Input.GetKeyUp(KeyCode.G))
            {
                VariableStore.RemoveAllVariables();
            }
        }
    }
}