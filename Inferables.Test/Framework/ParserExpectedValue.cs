#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Inferables.Test.Framework
{
    internal class ParserMatchExpectedValue
    {
        public string MatchPathNamespace { get; set; }
        public bool MatchPathHasNamespace { get; set; }
        public bool MatchPathIsWildCard { get; set; }
        public string BindingPathNamespace { get; set; }
        public bool BindingPathHasNamespace { get; set; }
        public bool BindingPathIsRelative { get; set; }
        public int BindingPathRootDepth { get; set; }
        public string ExpectedStringValue {get; set;}
    }

    internal class ParserMatchGroupExpectedValue
    {
        public string TestExpression {get; set;}
        public ParserMatchExpectedValue[] ExpectedValues { get; set; }
    }


    internal static class ParserTestTools
    {
        public static void TestVariationWithSameResults(this ParserMatchGroupExpectedValue value, params string [] testExpressions)
        {
            foreach(var expr in testExpressions)
            {
                value.TestExpression = expr;
                value.TestGroup();
            }
        }


        public static void TestGroup(this ParserMatchGroupExpectedValue value)
        {
            var results = BindingMapParser.GetMaps(value.TestExpression);

            Assert.AreEqual(value.ExpectedValues.Length, results.Count);
            for (int i = 0; i < value.ExpectedValues.Length; i++)
            {
                TestMatch(value.ExpectedValues[i], results[i]);
            }

        }

        private static void TestMatch(ParserMatchExpectedValue expectedValue, BindingMap bindingMap)
        {
            expectedValue.MatchPathNamespace =
                expectedValue.MatchPathHasNamespace ? expectedValue.MatchPathNamespace : String.Empty;

            expectedValue.BindingPathRootDepth =
                expectedValue.BindingPathIsRelative ? expectedValue.BindingPathRootDepth : 0;

            expectedValue.BindingPathNamespace =
                expectedValue.BindingPathHasNamespace ? expectedValue.BindingPathNamespace : String.Empty;

            Assert.AreEqual(expectedValue.MatchPathIsWildCard, bindingMap.MatchPath.IsWildcard);
            Assert.AreEqual(expectedValue.MatchPathHasNamespace, bindingMap.MatchPath.HasNamespace);
            Assert.AreEqual(expectedValue.MatchPathNamespace, bindingMap.MatchPath.Namespace);
            Assert.AreEqual(expectedValue.BindingPathIsRelative, bindingMap.BindingPath.IsRelative);
            Assert.AreEqual(expectedValue.BindingPathRootDepth, bindingMap.BindingPath.RootDepth);
            Assert.AreEqual(expectedValue.BindingPathHasNamespace, bindingMap.BindingPath.HasNamespace);
            Assert.AreEqual(expectedValue.BindingPathNamespace, bindingMap.BindingPath.Namespace);
            Assert.AreEqual(expectedValue.ExpectedStringValue, bindingMap.Value);
        }


    }

}
#endif
