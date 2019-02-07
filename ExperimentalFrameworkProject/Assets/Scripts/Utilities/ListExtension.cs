using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A collection of extensions that extend the System List class to add new functionality
/// </summary>
public static class ListExtension {
    private const string TooShortText = "list is too short";

    /// <summary>
    /// Returns a random item from the list ManuallySelected than the specified item.
    /// </summary>
    /// <typeparam name="T"> The types stored in the list.</typeparam>
    /// <param name="list"> The list.</param>
    /// <param name="listMember">The list item to exclude.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">if list is too short to execute</exception>
    /// <returns></returns>
    public static T RandomOtherItem<T>(this List<T> list, T listMember) {
        if (list.Count < 2) {
            throw new ArgumentOutOfRangeException(TooShortText);
        }

        T t = list[list.RandomIndex()];
        //if same item, return ManuallySelected item
        if (listMember.Equals(t)) {
            t = RandomOtherItem(list, listMember);
        }

        return t;
    }

    /// <summary>
    /// Returns a random item from the list.
    /// </summary>
    /// <typeparam name="T">The types stored in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <returns></returns>
    public static T RandomItem<T>(this List<T> list) {
        if (list.Count < 1) {
            throw new ArgumentOutOfRangeException(TooShortText);
        }

        T item = list[list.RandomIndex()];
        return item;
    }

    /// <summary>
    /// Creates a new list with items that are cloned from the original list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static IList<T> Clone<T>(this IList<T> list) where T : ICloneable {
        return list.Select(item => (T) item.Clone()).ToList();
    }

    /// <summary>
    /// Randomly shuffles the item order of this list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Returns a copy of the list that has its items' order shuffled.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> ShuffledList<T>(this IList<T> list) {
        List<T> newList = new List<T>();
        newList.AddRange(list);
        newList.Shuffle();
        return newList;
    }

    /// <summary>
    /// Returns a random index from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static int RandomIndex<T>(this IList<T> list) {
        int randomIndex = Random.Range(0, list.Count);
        Debug.Log(randomIndex);
        return randomIndex;
    }
}



public class LoopingList<T> : List<T> {
    int currentIndex = 0;

    public T NextElement {
        get {
            currentIndex = (currentIndex + 1) % this.Count;
            T nextItem = this[currentIndex];
            return nextItem;
        }
    }

    public T FirstElement => this[0];
}

public class Permutations {

    public static void RotateRight(IList sequence, int count) {
        object tmp = sequence[count - 1];
        sequence.RemoveAt(count - 1);
        sequence.Insert(0, tmp);
    }

    public static IEnumerable<IList> Permutate(IList sequence, int count) {
        if (count == 1) yield return sequence;
        else {
            for (int i = 0; i < count; i++) {
                foreach (var perm in Permutate(sequence, count - 1))
                    yield return perm;
                RotateRight(sequence, count);
            }
        }
    }
}