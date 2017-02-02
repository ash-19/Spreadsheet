// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Joe Zachary, January 29, 2016
// 
// Skeleton implemented by Snehashish Mishra for CS 3500, January 2016.
// uID: u0946268

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(t) are used in the specification of the methods of 
    /// this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  If a null parameter is 
    /// passed, an ArgumentNullException is thrown with a descriptive message.
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// A dictionary providing contant time access to all the dependees (s) mapped to the 
        /// "Key" dependents (t). As such, a dependent may have zero or more dependees contained 
        /// within a HashSet of strings. The time taken to iterate through all the dependees 
        /// mapped to a dependent is proportional to the number of dependees contained in the HashSet.
        /// </summary>
        
        private Dictionary<string, HashSet<string>> dependentsOfS;

        /// <summary>
        /// A dictionary providing contant time access to all the dependents (t) mapped to the 
        /// "Key" dependeees (s). As such, a dependee may have zero or more dependents contained 
        /// within a HashSet of String.The time taken to iterate through all the dependents mapped 
        /// to a dependee is proportional to the number of dependents contained in the HashSet.
        /// </summary>
        private Dictionary<string, HashSet<string>> dependeesOfT;

        /// <summary>
        /// Contains the total number of dependencies (s,t) contained in this DependencyGraph.
        /// </summary>
        private int size;


        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            dependeesOfT = new Dictionary<string, HashSet<string>>();
            dependentsOfS = new Dictionary<string, HashSet<string>>();
            size = 0;
        }

        /// <summary>
        /// Creates a new DependencyGraph object from the passed DependencyGraph. 
        /// This object is a copy of the passed DependencyGraph and its data is 
        /// independent of passed DependencyGraph. That is, if the passed object 
        /// was modified, the contents of this object remain unchanged. As this, 
        /// this constructor can be used to create a copy of the passed 
        /// DependencyGraph. If the passed DependencyGraph references null, throws 
        /// an ArgumentNullException. 
        /// </summary>
        public DependencyGraph(DependencyGraph dg1)
        {
            if( dg1 == null )
            {
                throw new ArgumentNullException("Passed DependencyGraph references "
                	        + "to null!");
            }
            
            this.dependeesOfT = new Dictionary<string, HashSet<string>>();
            this.dependentsOfS = new Dictionary<string, HashSet<string>>();

            // Copy the values of dependeesOfT from the passed dictionary to 
            // this dictionary
            foreach(KeyValuePair<string, HashSet<string>> kvp in dg1.dependeesOfT)
            {
                HashSet<string> t = new HashSet<string>(kvp.Value);
                this.dependeesOfT.Add(kvp.Key, t);
            }

            // Copy the values of dependentsOfS from the passed Dictionary to 
            // this Dictionary
            foreach (KeyValuePair<string, HashSet<string>> kvp in dg1.dependentsOfS)
            {
                HashSet<string> t = new HashSet<string>(kvp.Value);
                this.dependentsOfS.Add(kvp.Key, t);
            }

            this.size = dg1.size;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// <para>Reports whether dependents(s) is non-empty.</para>
        /// <para>If passed s is null, throws ArgumentNullExpception.
        /// </para></summary>
        /// 
        /// <param name="s">
        /// The dependee s whose dependents are to be checked.</param>
        /// 
        /// <returns>
        /// true, if s has dependents. False otherwise.
        /// </returns>
        public bool HasDependents(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Passed string references to null!");
            }

            // temp variable to get size of the dependents HashSet
            HashSet<string> t;                              
            
            // If a dependee s exists, see if it has any dependents t?
            // If so, return whether s has dependents or not. Else, 
            // there is no dependee s itself in the DG
            if (dependentsOfS.TryGetValue(s, out t))        
            {
                return t.Count == 0 ? false : true;
            }
            else  
            {
                return false;
            }
        }

        /// <summary>
        /// <para>Reports whether dependees(t) is non-empty.</para>
        /// <para>If passed t is null, throws ArgumentNullExpception.
        /// </para></summary>
        /// 
        /// <param name="t">
        /// The dependent t whose dependees are to be checked.</param>
        /// 
        /// <returns>
        /// true, if t has dependees. False otherwise.
        /// </returns>
        public bool HasDependees(string t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("Passed string references to null!");
            }

            // temp variable to get size of the dependees HashSet
            HashSet<string> s;                              

            // If a dependent t exists, see if it has any dependees s?
            // If so, return whether t has dependees or not. Else, there 
            // is no dependent t itself in the DG
            if (dependeesOfT.TryGetValue(t, out s)) 
            {
                return s.Count == 0 ? false : true; 
            }
            else                                            
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates dependents of s.  If s is null, throws an 
        /// ArgumentNullException.</summary>
        /// 
        /// <param name="s">
        /// The dependee whose dependents needs to be obtained.</param>
        /// 
        /// <returns>
        /// An IEnumerable collection of strings containing all the 
        /// dependents of s.
        /// </returns>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Passed string references to null!");
            }

            // If there is a dependee s in this DG, return a copy of dependents(s) as 
            // an IEnumerable collection. Else return an empty IEnumerable collection.
            return dependentsOfS.ContainsKey(s) ? 
                        new HashSet<String>(dependentsOfS[s]) : 
                            new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependewe of t.  If s is null, throws an 
        /// ArgumentNullException.</summary>
        /// 
        /// <param name="t">
        /// The dependent whose dependees needs to be obtained.</param>
        /// 
        /// <returns>
        /// An IEnumerable collection of strings containing all the 
        /// dependees of t.
        /// </returns>
        public IEnumerable<string> GetDependees(string t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("Passed string references to null!");
            }

            // If there is a dependent t in this DG, return a copy of dependees(t) as 
            // an IEnumerable collection. Else return an empty IEnumerable collection.
            return dependeesOfT.ContainsKey(t) ? 
                        new HashSet<String>(dependeesOfT[t]) : 
                            new HashSet<string>();
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this 
        /// DependencyGraph. If either s or t is null, throws 
        /// an ArgumentNullException.</summary>
        /// 
        /// <param name="s">The dependee s.</param>
        /// <param name="t">The dependent t.</param>
        /// 
        public void AddDependency(string s, string t)
        {
            if( s == null || t == null)
            {
                throw new ArgumentNullException("One of the passed string "
                                + "references to null!");
            }
            
            // Update dependentsOfS Dictionary
            if (dependentsOfS.ContainsKey(s))                        // If the dependentsOfS dictionary already has an s key,
            {
                if (dependentsOfS[s].Add(t))                         // Add the dependent t to it's value HashSet (if not already present) 
                {
                    size++;                                          // to make (s,t) and increment size.
                }
            }
            else                                                     // Else the dependentsOfS dictionary does not contains an s key,
            {
                HashSet<string> dependents = new HashSet<string>();  // Create an empty dependents HashSet for its value field,
                dependents.Add(t);                                   // Add the dependent t to this HashSet,
                dependentsOfS.Add(s, dependents);                    // Create a new dictionary entry (s,t)
                size++;                                              // Increment the size
            }

            // Update dependeesOfT Dictionary
            if (dependeesOfT.ContainsKey(t))                         // If the dependeesOfT dictionary already has a t key,
            {
                dependeesOfT[t].Add(s);                              // Add the dependee s to it's value HashSet (if not already present) 
            }                                                        // to make (t, s).
            else                                                     // Else the dependentsOfS dictionary does not contains an s key,
            {
                HashSet<string> dependees = new HashSet<string>();   // Create an empty dependees HashSet for its value field,
                dependees.Add(s);                                    // Add the dependee s to this HashSet,
                dependeesOfT.Add(t, dependees);                      // Create a new dictionary entry (t,s)
            }
        }

        ///<summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// This has no effect if (s,t) does not belong to this DependencyGraph.
        /// If either s or t is null,  throws an ArgumentNullException.
        /// </summary>
        /// 
        /// <param name="s">The dependee s.</param>
        /// <param name="t">The dependent t.</param>
        /// 
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("One of the passed string references to null!");
            }
            
            // Update the dependentsOfS Dictionary
            if( dependentsOfS.ContainsKey(s) )
            {
                if ( dependentsOfS[s].Count == 0 )      // If s has all its dependents removed, 
                {
                    dependentsOfS.Remove(s);            // remove its entry as well.
                }
                else                                    // Else s has atleast one dependent paired to it,
                {
                    dependentsOfS[s].Remove(t);         // Remove it and,
                    size--;                             // Decrement the size of this DependencyGraph
                }
            }

            // Update the dependeesOfT Dictionary
            if (dependeesOfT.ContainsKey(t))
            {
                if (dependeesOfT[t].Count == 0)         // If t has all its dependee's removed, 
                {
                    dependeesOfT.Remove(t);             // remove its entry as well.
                }
                else                                    // Else t has atleast one dependee paired to it,
                {
                    dependeesOfT[t].Remove(s);          // Remove it.
                }
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        /// 
        /// <param name="s">The dependees whose dependents needs to be replaced.</param>
        /// 
        /// <param name="newDependents">A collection of new dependent to be paired with 
        /// the passed dependee</param>
        /// 
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null || newDependents == null)
            {
                throw new ArgumentNullException("One of the passed string references to null!");
            }

            IEnumerable<string> oldDependents = this.GetDependents(s);   // Get the old dependents of s

            foreach(string r in oldDependents)                           // Remove all the old (s, r) dependencies
            {
                this.RemoveDependency(s, r);
            }

            foreach(string t in newDependents)                           // Add all the new (s, t) dependencies
            {
                this.AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        /// 
        /// <param name="t">The dependent whose dependees needs to be replaced.</param>
        /// 
        /// <param name="newDependees">A collection of new dependees to be paired with 
        /// the passed dependent.</param>
        /// 
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null || newDependees == null)
            {
                throw new ArgumentNullException("One of the passed string "
                                    + "references to null!");
            }

            IEnumerable<string> oldDependees = this.GetDependees(t);  // Get the old dependees of t

            foreach (string r in oldDependees)                        // Remove all the old (r, t) dependencies
            {
                this.RemoveDependency(r, t);
            }

            foreach (string s in newDependees)                        // Add all the new (s, t) dependencies
            {
                this.AddDependency(s, t);
            }
        }
    }
}
