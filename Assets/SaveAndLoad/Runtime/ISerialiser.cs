namespace SaveAndLoad {
    internal interface ISerialiser {
        internal string Serialise<T>(T obj);
        internal T Deserialise<T>(string data);
    }
}
