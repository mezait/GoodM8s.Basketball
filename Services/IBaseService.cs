using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace GoodM8s.Basketball.Services {
    public interface IBaseService<out TPart, TRecord> : IDependency
        where TPart : ContentPart<TRecord>
        where TRecord : ContentPartRecord {
        int Count();
        IEnumerable<TPart> Get();
        TPart Get(int id);
    }
}