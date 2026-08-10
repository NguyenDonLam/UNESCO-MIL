using System;
using System.IO;
using Bedrot.Narrative.Application;
using UnityEngine;

namespace Bedrot.Save
{
    public sealed class JsonSaveGameRepository : ISaveGameRepository
    {
        private readonly string _path;
        public JsonSaveGameRepository(string path = null) =>
            _path = path ?? Path.Combine(Application.persistentDataPath, "bedrot-save.json");
        public bool HasSave => File.Exists(_path);
        public void Save(GameSessionMemento memento)
        {
            string directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            string temporaryPath = _path + ".tmp";
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(memento, true));
            if (File.Exists(_path)) File.Delete(_path);
            File.Move(temporaryPath, _path);
        }
        public GameSessionMemento Load()
        {
            if (!HasSave) throw new InvalidOperationException("No saved game exists.");
            return JsonUtility.FromJson<GameSessionMemento>(File.ReadAllText(_path));
        }
        public void Delete() { if (HasSave) File.Delete(_path); }
    }
}
