using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers {
    public static class CharacterNameHelper {
        private static readonly string[] names = new string[]
        {
            "Alice", "Bob", "Charlie", "Diana", "Ethan", "Fiona", "George", "Hannah",
            "Ian", "Julia", "Kevin", "Laura", "Michael", "Nina", "Oscar", "Paula",
            "Quinn", "Rachel", "Sam", "Tina", "Umar", "Vera", "Will", "Xena",
            "Yara", "Zane"
        };

        private static readonly string[] surnames = new string[]
        {
            "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson",
            "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
            "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall"
        };

        public static string GetRandomName() {
            int indexName = Random.Range(0, names.Length);
            int indexSurname = Random.Range(0, surnames.Length);
            return names[indexName] + " " + surnames[indexSurname];
        }
    }
}