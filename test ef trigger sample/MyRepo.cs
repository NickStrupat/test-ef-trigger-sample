using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_ef_trigger_sample
{
    public class MyRepo
    {
        public interface IRepositoryBase<T> : IDisposable where T : class
        {
            int Update(T t);
        }

        public class RepositoryBase<TObject> : IRepositoryBase<TObject> where TObject : class
        {
            protected DbContext Context = null;
            private bool ShareContext = false;

            public void Dispose()
            {
                if (ShareContext && (Context != null))
                    Context.Dispose();
            }

            public RepositoryBase(DbContext _Context)
            {
                Context = _Context;
                Context.Configuration.ProxyCreationEnabled = false;
                Context.Configuration.LazyLoadingEnabled = false;
            }

            private IEnumerable<string> GetPrimaryKeys<T>() where T : class
            {
                ObjectContext objectthis = ((IObjectContextAdapter)Context).ObjectContext;

                ObjectSet<T> set = objectthis.CreateObjectSet<T>();

                IEnumerable<string> keyNames = set.EntitySet.ElementType.KeyMembers.Select(k => k.Name);

                return keyNames;
            }

            protected DbSet<TObject> DbSet
            {
                get
                {
                    return Context.Set<TObject>();
                }
            }

            public virtual TObject Create(TObject TObject)
            {
                try
                {
                    var newEntry = DbSet.Add(TObject);

                    if (!ShareContext)
                        Context.SaveChanges();

                    return newEntry;
                }
                catch
                {
                    throw;
                }
            }

            public virtual int Update(TObject newEnt)
            {
             
                    var objContext = ((IObjectContextAdapter)Context).ObjectContext;
                    var objSet = objContext.CreateObjectSet<TObject>();
                    var ListKey = GetPrimaryKeys<TObject>().ToArray();

                    var entry = Context.Entry(newEnt);
                    List<Object> objs = new List<object>();
                    foreach (var p in ListKey)
                    {
                        var obj = entry.Property(p).CurrentValue;
                        objs.Add(obj);
                    }

                    var entity = Context.Set<TObject>().Find(objs.ToArray());
                    Context.Entry<TObject>(entity).CurrentValues.SetValues(newEnt);
                    if (!ShareContext)
                        return Context.SaveChanges();
                    return 0;
              
            }

        }
    }
}
