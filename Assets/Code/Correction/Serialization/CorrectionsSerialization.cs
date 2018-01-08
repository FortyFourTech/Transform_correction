using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Correction {
    public class CorrectionsSerialization {
        public static string k_saveFileName = "ObjectsCorrection.xml";
        public static string k_serializePath = Path.Combine(Application.persistentDataPath, k_saveFileName);

        public static void serialize(GameObject[] objects) {
            //var objFullName = getFullName(obj);
            var changesList = new List<GameObjectCorrection>();

            foreach (var obj in objects) {
                var objToSerialize = new GameObjectCorrection(obj);
                changesList.Add(objToSerialize);
            }

            var changes = new CorrectionsList { ChangedObjects = changesList.ToArray() };

            var serializer = new XmlSerializer(typeof(CorrectionsList));

            var filePath = k_serializePath;
            var fileStream = new FileStream(filePath, FileMode.Create);

            serializer.Serialize(fileStream, changes);

            fileStream.Close();
        }

        public static void deserialize(string filePath = null) {
            if (filePath == null)
                filePath = k_serializePath;

            if (!File.Exists(filePath)) {
                return;
            } else {
                var serializer = new XmlSerializer(typeof(CorrectionsList));
                var fileStream = new FileStream(filePath, FileMode.Open);

                var changes = (CorrectionsList)serializer.Deserialize(fileStream);
                foreach (var readedObj in changes.ChangedObjects) {
                    readedObj.Apply();
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Import correction")]
        public static void Import() {
            var filePath = UnityEditor.EditorUtility.OpenFilePanel("choose mesh file", "", "");
            CorrectionsSerialization.deserialize(filePath);
        }
#endif
    }
}
