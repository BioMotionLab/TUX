using System.Collections.Generic;

namespace bmlTUX.Extensions {
    public class LoopingList<T> : List<T> {
        int currentIndex = 0;

        public T NextElement {
            get {
                currentIndex = (currentIndex + 1) % Count;
                T nextItem = this[currentIndex];
                return nextItem;
            }
        }

        public T FirstElement => this[0];
    }
}