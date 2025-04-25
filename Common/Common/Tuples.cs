using System;
using System.Collections.Generic;

namespace Common
{
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2> tupleList) => tupleList.ForEach(Add);
        public void Add(T1 item1, T2 item2) => Add(item1, item2);
    }

    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3> tupleList) => tupleList.ForEach(Add);
        public void Add(T1 item1, T2 item2, T3 item3) => Add(item1, item2, item3);
    }

    public class TupleList<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3, T4> tupleList) => tupleList.ForEach(Add);
        public void Add(T1 item1, T2 item2, T3 item3, T4 item4) => Add(item1, item2, item3, item4);
    }
}