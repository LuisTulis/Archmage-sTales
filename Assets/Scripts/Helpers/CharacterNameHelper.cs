using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class CharacterNameHelper
    {
        private static readonly string[] names = new string[]
        {
            "Alice", "Bob", "Charlie", "Diana", "Ethan", "Fiona", "George", "Hannah",
            "Ian", "Julia", "Kevin", "Laura", "Michael", "Nina", "Oscar", "Paula",
            "Quinn", "Rachel", "Sam", "Tina", "Umar", "Vera", "Will", "Xena",
            "Yara", "Zane"
        };

        private static readonly string[] clayNames = new string[]
        {
            "Arcillita", "Gredín", "Barrita", "Terrico", "Modelcito"
        };

        private static readonly string[] skeletonNames = new string[]
        {
            "Huesin", "Calacito", "Oseito", "Tibin", "Ribiton"
        };

        private static readonly string[] surnames = new string[]
        {
            "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson",
            "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
            "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall"
        };

        private static readonly string[] claySurnames = new string[]
        {
            "Suavero", "Amasito", "Del Barrito", "Modelares", "Tiernoble"
        };

        private static readonly string[] skeletonSurnames = new string[]
        {
            "Del Huesero", "Marcalito", "Costarelli", "Oseñito", "Calaverin"
        };

        private static readonly string[] profes = new string[]
        {
            "Alejandro Elisei", "Lorenzo Caballero", "El Dogthor 😎"
        };

        public static string GetRandomProfeName()
        {
            int indexName = Random.Range(0, profes.Length);
            return profes[indexName];
        }

        public static string GetRandomName()
        {
            int indexName = Random.Range(0, names.Length);
            int indexSurname = Random.Range(0, surnames.Length);
            var workersName = names[indexName] + " " + surnames[indexSurname];

            try
            {
                foreach (var worker in GlobalCharactersManager.Instance.Workers)
                {

                    if (worker.GetComponent<CharacterModel>().CharacterName.Equals(workersName))
                    {
                        Debug.LogWarning("The name already exists. Generating a new one");
                        workersName = GetRandomName();
                        break;
                    }
                }
            }
            catch { }

            return workersName;
        }

        public static string GetRandomClayName()
        {
            int indexName = Random.Range(0, clayNames.Length);
            int indexSurname = Random.Range(0, claySurnames.Length);
            var workersName = clayNames[indexName] + " " + claySurnames[indexSurname];

            try
            {
                foreach (var worker in GlobalCharactersManager.Instance.Workers)
                {

                    if (worker.GetComponent<CharacterModel>().CharacterName.Equals(workersName))
                    {
                        Debug.LogWarning("The name already exists. Generating a new one");
                        workersName = GetRandomClayName();
                        break;
                    }
                }
            }
            catch { }

            return workersName;
        }

        public static string GetRandomSkeletonName()
        {
            int indexName = Random.Range(0, skeletonNames.Length);
            int indexSurname = Random.Range(0, skeletonSurnames.Length);
            var workersName = skeletonNames[indexName] + " " + skeletonSurnames[indexSurname];

            try
            {
                foreach (var worker in GlobalCharactersManager.Instance.Workers)
                {

                    if (worker.GetComponent<CharacterModel>().CharacterName.Equals(workersName))
                    {
                        Debug.LogWarning("The name already exists. Generating a new one");
                        workersName = GetRandomSkeletonName();
                        break;
                    }
                }
            }
            catch { }

            return workersName;
        }
    }
}