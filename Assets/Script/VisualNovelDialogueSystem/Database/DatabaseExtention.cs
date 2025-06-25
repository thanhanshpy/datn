using UnityEngine;

namespace Commands
{
    public abstract class DatabaseExtention
    {
        public static void Extend(CommandDatabase commandDatabase)
        {

        }

        public static CommandParameters ConvertDataToParameters(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
    }
}