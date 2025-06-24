namespace Project.Scripts.Util.DataPersistence;

public abstract class Momento {
    public string Id { get; private init; }
    
    protected Momento(string id) {
        this.Id = id;
    }
}
