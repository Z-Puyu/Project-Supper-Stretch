namespace SaveAndLoadSystem.Runtime {
    internal interface ISerialiser {
        internal string Serialise<T>(T obj);
        internal T Deserialise<T>(string data);
    }
}
