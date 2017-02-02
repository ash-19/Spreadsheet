// Written and implemented by Snehashish Mishra for CS 3500, January 2016.
// uID: u0946268

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    /// <summary>
    /// Contains unit tests for DependencyGraph class.
    /// </summary>
    [TestClass]
    public class DGraphsBaseTests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        public void Construct01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);
        }

        /// <summary>
        /// Tests the constructor with null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Construct03()
        {
            DependencyGraph d1 = new DependencyGraph(null);
        }

        /// <summary>
        /// Tests all inclusive performance of the one argument 
        /// constructor.
        /// </summary>
        [TestMethod]
        public void Construct02()
        {
            DependencyGraph dg1 = new DependencyGraph();

            dg1.AddDependency("a", "b");
            dg1.AddDependency("b", "b");
            dg1.AddDependency("c", "a");

            DependencyGraph dg2 = new DependencyGraph(dg1);

            Assert.AreEqual(dg1.Size, dg2.Size);

            // Test getDependees
            HashSet<string> tSet1 = new HashSet<string>(dg1.GetDependees("a"));
            HashSet<string> tSet2 = new HashSet<string>(dg2.GetDependees("a"));

            Assert.IsFalse(tSet1.Contains("b") || tSet2.Contains("b"));
            Assert.IsTrue(tSet1.Contains("c") && tSet2.Contains("c"));

            // Test getDependents
            HashSet<string> sSet1 = new HashSet<string>(dg1.GetDependents("c"));
            HashSet<string> sSet2 = new HashSet<string>(dg2.GetDependents("c"));

            Assert.IsFalse(sSet1.Contains("b") || sSet2.Contains("b"));
            Assert.IsTrue(sSet1.Contains("a") && sSet2.Contains("a"));

            // Test independence after removing dependency in dg1
            dg1.RemoveDependency("c", "a");
            Assert.IsFalse(new HashSet<string>(dg1.GetDependents("c")).Contains("a"));
            Assert.IsTrue(new HashSet<string>(dg2.GetDependents("c")).Contains("a"));

            // Test independence after replacing dependents in dg1
            HashSet<string> newT = new HashSet<string>();
            newT.Add("c");
            dg1.ReplaceDependents("b", newT);

            Assert.IsFalse(new HashSet<string>(dg1.GetDependents("b")).Contains("b"));
            Assert.IsTrue(new HashSet<string>(dg1.GetDependents("b")).Contains("c"));
            Assert.IsTrue(new HashSet<string>(dg2.GetDependents("b")).Contains("b"));
            Assert.IsFalse(new HashSet<string>(dg2.GetDependents("b")).Contains("c"));

            // Test independence after replacing dependees in dg1
            HashSet<string> newS = new HashSet<string>();
            newS.Add("a");
            dg1.ReplaceDependees("a", newS);

            Assert.IsFalse(new HashSet<string>(dg1.GetDependees("a")).Contains("c"));
            Assert.IsTrue(new HashSet<string>(dg1.GetDependees("a")).Contains("a"));
            Assert.IsTrue(new HashSet<string>(dg2.GetDependees("a")).Contains("c"));
            Assert.IsFalse(new HashSet<string>(dg2.GetDependees("a")).Contains("a"));
        }

        /// <summary>
        /// Tests the size of DG with one (s,t).
        /// </summary>
        [TestMethod]
        public void Size01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with multiple (s,t).
        /// </summary>
        [TestMethod]
        public void Size02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            d1.AddDependency("Ash", "Squirtel");
            d1.AddDependency("Ash", "Charmander");
            d1.AddDependency("Ash", "Pigeot");
            Assert.AreEqual(4, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with duplicate (s,t).
        /// </summary>
        [TestMethod]
        public void Size03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with (s,t) and (r,t).
        /// </summary>
        [TestMethod]
        public void Size04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("Meowth", "Pikachu");
            Assert.AreEqual(2, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with (s,t) and (s,r).
        /// </summary>
        [TestMethod]
        public void Size05()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("Ash", "Squirtel");
            Assert.AreEqual(2, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with (s,s).
        /// </summary>
        [TestMethod]
        public void Size06()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Ash");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with two (s,s).
        /// </summary>
        [TestMethod]
        public void Size07()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Ash");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("Ash", "Ash");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG with 1000 (s,t).
        /// </summary>
        [TestMethod]
        public void Size08()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            for (int i = 0; i < 1000; i++)
            {
                d1.AddDependency("Ash", "Poke" + i);
                Assert.AreEqual(i+1, d1.Size);
            }

            Assert.AreEqual(1000, d1.Size);
        }

        /// <summary>
        /// Tests the size of DG after a removal.
        /// </summary>
        [TestMethod]
        public void Size09()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("Ash", "Squirtel");
            Assert.AreEqual(2, d1.Size);

            d1.RemoveDependency("Ash", "Squirtel");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests AddDependency(s, t) for first pair.
        /// </summary>
        [TestMethod]
        public void AddDependency01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests and and remove of the only single pair.
        /// </summary>
        [TestMethod]
        public void AddDependency02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
        }

        /// <summary>
        /// Tests and and remove of a single pair from size > 0 DG.
        /// </summary>
        [TestMethod]
        public void AddDependency03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            d1.AddDependency("b", "c");
            d1.AddDependency("a", "c");
            Assert.AreEqual(3, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(2, d1.Size);
        }

        /// <summary>
        /// Tests AddDependency(s, t) when s = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDependency04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency(null, "b");
        }

        /// <summary>
        /// Tests AddDependency(s, t) when t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDependency05()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", null);
        }

        /// <summary>
        /// Tests AddDependency(s, t) both s,t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDependency06()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency(null, null);
        }

        /// <summary>
        /// Tests AddDependency(s, t) for duplicate add.
        /// </summary>
        [TestMethod]
        public void AddDependency07()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);                // size should remain unchanged
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) for first pair.
        /// </summary>
        [TestMethod]
        public void RemoveDependency01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) from empty DG.
        /// </summary>
        [TestMethod]
        public void RemoveDependency02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) for mutiple removals.
        /// </summary>
        [TestMethod]
        public void RemoveDependency03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            d1.AddDependency("b", "c");
            d1.AddDependency("a", "c");
            d1.AddDependency("d", "a");
            d1.AddDependency("d", "c");
            d1.AddDependency("b", "d");
            d1.AddDependency("a", "c");     // Duplicate pair
            Assert.AreEqual(6, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(5, d1.Size);

            d1.RemoveDependency("b", "c");
            Assert.AreEqual(4, d1.Size);

            d1.RemoveDependency("b", "d");
            Assert.AreEqual(3, d1.Size);

            d1.RemoveDependency("d", "c");
            Assert.AreEqual(2, d1.Size);

            d1.RemoveDependency("d", "a");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) for cyclic pair.
        /// </summary>
        [TestMethod]
        public void RemoveDependency04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            d1.AddDependency("b", "c");
            d1.AddDependency("c", "a");
            Assert.AreEqual(3, d1.Size);

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(2, d1.Size);

            d1.RemoveDependency("c", "a");
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) when s = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDependency05()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.RemoveDependency(null, "b");
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) when t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDependency06()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.RemoveDependency("a", null);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) both s,t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDependency07()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.RemoveDependency(null, null);
        }

        /// <summary>
        /// Tests RemoveDependency(s, t) when s has no pairs.
        /// </summary>
        [TestMethod]
        public void RemoveDependency08()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);

            d1.RemoveDependency("a", "b");      // Remove (a,b)
            Assert.AreEqual(0, d1.Size);

            d1.RemoveDependency("a", "b");      // Try removing again
            Assert.AreEqual(0, d1.Size);        // Size shouldn't change
        }

        /// <summary>
        /// Tests HasDependents(s) for empty DG.
        /// </summary>
        [TestMethod]
        public void HasDependents01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            Assert.IsFalse(d1.HasDependents("Ash"));
        }

        /// <summary>
        /// Tests HasDependents(s) for (s,t).
        /// </summary>
        [TestMethod]
        public void HasDependents02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.IsTrue(d1.HasDependents("Ash"));
            Assert.AreEqual(1, d1.Size);

            Assert.IsFalse(d1.HasDependents("Pikachu"));
        }

        /// <summary>
        /// Tests when s = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependents03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.HasDependents(null);
        }

        /// <summary>
        /// Tests HasDependents(s, t) when s has no pairs.
        /// </summary>
        [TestMethod]
        public void HasDependents04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
            Assert.IsTrue(d1.HasDependents("a"));       // a has dependents

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
            Assert.IsFalse(d1.HasDependents("a"));      // a is in set but has no dependents

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
            Assert.IsFalse(d1.HasDependents("a"));      // a is removed and has no dependents
        }

        /// <summary>
        /// Tests HasDependees(t) for empty DG.
        /// </summary>
        [TestMethod]
        public void HasDependees01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            Assert.IsFalse(d1.HasDependees("Pikachu"));
        }

        /// <summary>
        /// Tests HasDependees(t) for (s,t).
        /// </summary>
        [TestMethod]
        public void HasDependees02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            Assert.IsTrue(d1.HasDependees("Pikachu"));
            Assert.AreEqual(1, d1.Size);
        }

        /// <summary>
        /// Tests when t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependees03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.HasDependees(null);
        }

        /// <summary>
        /// Tests HasDependees(s, t) when t has no pairs.
        /// </summary>
        [TestMethod]
        public void HasDependees04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
            Assert.IsTrue(d1.HasDependees("b"));       // b has dependees

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
            Assert.IsFalse(d1.HasDependees("b"));      // b is in set but has no dependees

            d1.RemoveDependency("a", "b");
            Assert.AreEqual(0, d1.Size);
            Assert.IsFalse(d1.HasDependents("b"));      // b is removed and has no dependees.
        }

        /// <summary>
        /// Tests GetDependents(s) for empty DG.
        /// </summary>
        [TestMethod]
        public void GetDependents01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            HashSet<string> tSet = new HashSet<string>(d1.GetDependents("Ash"));
            Assert.AreEqual(0, tSet.Count);
        }

        /// <summary>
        /// Tests GetDependents(s) for small size DG.
        /// </summary>
        [TestMethod]
        public void GetDependents02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            d1.AddDependency("Ash", "Squirtel");
            d1.AddDependency("Ash", "Charmander");
            d1.AddDependency("Ash", "Pigeot");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> tSet = new HashSet<string>(d1.GetDependents("Ash"));

            Assert.IsTrue(tSet.Contains("Pikachu"));
            Assert.IsTrue(tSet.Contains("Squirtel"));
            Assert.IsTrue(tSet.Contains("Charmander"));
            Assert.IsTrue(tSet.Contains("Pigeot"));

            // Check to see if s's are accidently added in dependents set
            Assert.IsFalse(tSet.Contains("Ash"));
        }

        /// <summary>
        /// Tests when s = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependents03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.GetDependents(null);
        }

        /// <summary>
        /// Tests GetDependees(t) for empty DG.
        /// </summary>
        [TestMethod]
        public void GetDependees01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            HashSet<string> tSet = new HashSet<string>(d1.GetDependees("Ash"));
            Assert.AreEqual(0, tSet.Count);
        }

        /// <summary>
        /// Tests GetDependees(s) for small size DG.
        /// </summary>
        [TestMethod]
        public void GetDependees02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("Ash", "Pikachu");
            d1.AddDependency("Ash", "Squirtel");
            d1.AddDependency("Ash", "Charmander");
            d1.AddDependency("Ash", "Pigeot");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> tSet = new HashSet<string>(d1.GetDependees("Pikachu"));
            Assert.IsTrue(tSet.Contains("Ash"));
            Assert.IsFalse(tSet.Contains("Pikachu"));

            tSet = new HashSet<string>(d1.GetDependees("Squirtel"));
            Assert.IsFalse(tSet.Contains("Squirtel"));
            Assert.IsTrue(tSet.Contains("Ash"));

            tSet = new HashSet<string>(d1.GetDependees("Charmander"));
            Assert.IsFalse(tSet.Contains("Charmander"));
            Assert.IsTrue(tSet.Contains("Ash"));

            tSet = new HashSet<string>(d1.GetDependees("Pigeot"));
            Assert.IsFalse(tSet.Contains("Pigeot"));
            Assert.IsTrue(tSet.Contains("Ash"));

            tSet = new HashSet<string>(d1.GetDependees("Meweoth"));
            Assert.IsFalse(tSet.Contains("Pigeot"));
            Assert.IsFalse(tSet.Contains("Ash"));
        }

        /// <summary>
        /// Tests when t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependees03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.GetDependees(null);
        }

        /// <summary>
        /// Tests ReplaceDependents(s, newT) for empty DG to 
        /// check if it creates new (s,t) pairs.
        /// </summary>
        [TestMethod]
        public void ReplaceDependents01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            HashSet<string> tSet = new HashSet<string>();
            tSet.Add("b");
            tSet.Add("c");
            tSet.Add("d");

            d1.ReplaceDependents("Ash", tSet);

            Assert.AreEqual(3, d1.Size);

            tSet = new HashSet<string>(d1.GetDependents("Ash"));
            Assert.IsFalse(tSet.Contains("e"));
            Assert.IsTrue(tSet.Contains("b"));
            Assert.IsTrue(tSet.Contains("c"));
            Assert.IsTrue(tSet.Contains("d"));
        }

        /// <summary>
        /// Tests ReplaceDependents(s, newT) for old and new pairs.
        /// </summary>
        [TestMethod]
        public void ReplaceDependents02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            d1.AddDependency("a", "c");
            d1.AddDependency("a", "d");
            d1.AddDependency("b", "a");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> tSet = new HashSet<string>();
            tSet.Add("x");
            tSet.Add("y");

            d1.ReplaceDependents("a", tSet);

            // 3 removed, 2 added. So size is now 1 less
            Assert.AreEqual(3, d1.Size);            

            // Check that old pairs dont exist
            tSet = new HashSet<string>(d1.GetDependents("a"));
            Assert.IsFalse(tSet.Contains("b"));
            Assert.IsFalse(tSet.Contains("c"));
            Assert.IsFalse(tSet.Contains("d"));

            // Check if new pairs are added
            Assert.IsTrue(tSet.Contains("x"));
            Assert.IsTrue(tSet.Contains("y"));
        }

        /// <summary>
        /// Tests ReplaceDependents(s, empty_set). Should only 
        /// remove (s,r) and not add anything.
        /// </summary>
        [TestMethod]
        public void ReplaceDependents03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("a", "b");
            d1.AddDependency("a", "c");
            d1.AddDependency("a", "d");
            d1.AddDependency("b", "a");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> tSet = new HashSet<string>();   // Empty newDependents
            d1.ReplaceDependents("a", tSet);

            // 3 removed, 0 added. So size is now 1
            Assert.AreEqual(1, d1.Size);                    

            // Check that old pairs dont exist
            tSet = new HashSet<string>(d1.GetDependents("a"));
            Assert.IsFalse(tSet.Contains("b"));
            Assert.IsFalse(tSet.Contains("c"));
            Assert.IsFalse(tSet.Contains("d"));
        }

        /// <summary>
        /// Tests when only s = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependents04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependents(null, new HashSet<string>());
        }

        /// <summary>
        /// Tests when only newT set = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependents05()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependents("s", null);
        }

        /// <summary>
        /// Tests when both s, newT set = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependents06()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependents(null, null);
        }

        /// <summary>
        /// Tests ReplaceDependees(t, newS) in empty DG.
        /// </summary>
        [TestMethod]
        public void ReplaceDependees01()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);
            
            HashSet<string> sSet = new HashSet<string>();   // Empty newDependents
            sSet.Add("x");
            sSet.Add("y");

            d1.ReplaceDependees("a", sSet);

            // 0 removed, 2 added. So size is now 2
            Assert.AreEqual(2, d1.Size);

            // Check that new pairs exist
            sSet = new HashSet<string>(d1.GetDependees("a"));
            Assert.IsTrue(sSet.Contains("x"));
            Assert.IsTrue(sSet.Contains("y"));
        }

        /// <summary>
        /// Tests ReplaceDependees(t, newS). Should  
        /// remove (r,t) and add (s,t) pairs.
        /// </summary>
        [TestMethod]
        public void ReplaceDependees02()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("b", "a");
            d1.AddDependency("c", "a");
            d1.AddDependency("d", "a");
            d1.AddDependency("a", "b");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> sSet = new HashSet<string>();   // Empty newDependents
            sSet.Add("x");
            sSet.Add("y");

            d1.ReplaceDependees("a", sSet);

            // 3 removed, 2 added. So size is now 1 less
            Assert.AreEqual(3, d1.Size);

            // Check that old pairs dont exist
            sSet = new HashSet<string>(d1.GetDependees("a"));
            Assert.IsFalse(sSet.Contains("b"));
            Assert.IsFalse(sSet.Contains("c"));
            Assert.IsFalse(sSet.Contains("d"));

            // Check that new pairs exist
            Assert.IsTrue(sSet.Contains("x"));
            Assert.IsTrue(sSet.Contains("y"));
        }

        /// <summary>
        /// Tests ReplaceDependees(t, empty_set). Should only 
        /// remove (r,t) and not add anything.
        /// </summary>
        [TestMethod]
        public void ReplaceDependees03()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.AddDependency("b", "a");
            d1.AddDependency("c", "a");
            d1.AddDependency("d", "a");
            d1.AddDependency("a", "b");
            Assert.AreEqual(4, d1.Size);

            HashSet<string> sSet = new HashSet<string>();   // Empty newDependents
            d1.ReplaceDependees("a", sSet);

            // 3 removed, 0 added. So size is now 1
            Assert.AreEqual(1, d1.Size);

            // Check that old pairs dont exist
            sSet = new HashSet<string>(d1.GetDependees("a"));
            Assert.IsFalse(sSet.Contains("b"));
            Assert.IsFalse(sSet.Contains("c"));
            Assert.IsFalse(sSet.Contains("d"));
        }

        /// <summary>
        /// Tests when only t = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependees04()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependees(null, new HashSet<string>());
        }

        /// <summary>
        /// Tests when only newS set = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependees05()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependees("s", null);
        }

        /// <summary>
        /// Tests when both t, newS set = null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependees06()
        {
            DependencyGraph d1 = new DependencyGraph();
            Assert.AreEqual(0, d1.Size);

            d1.ReplaceDependees(null, null);
        }
    }
}
