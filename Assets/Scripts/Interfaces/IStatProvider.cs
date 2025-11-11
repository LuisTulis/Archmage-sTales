using System.Collections.Generic;

public interface IStatProvider {
    Dictionary<string, object> GetStats();
}