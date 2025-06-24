using System;

namespace Project.Scripts.Util.DataPersistence;

public interface IPersistent {
    public abstract Guid Id { get; }
    
    public abstract Momento Save();
    
    public abstract void Load(Momento momento);
}
