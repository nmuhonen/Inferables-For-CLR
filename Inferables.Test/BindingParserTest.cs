#if DEBUG
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Inferables.Internal;
using Inferables.Test.Framework;

namespace Inferables.Test
{
    [TestClass]
    public class BindingParserTest
    {
        [TestMethod, 
        ExpectedException(typeof(ArgumentException))]
        public void ShouldFailOnEmptyString()
        {
            var results = BindingMapParser.GetMaps("");
        }

        [TestMethod]
        public void ShouldParseRelativeExpression()
        {

            var testConfig = new ParserMatchGroupExpectedValue
                {
                    ExpectedValues = new[]
                    {
                        new ParserMatchExpectedValue
                        {
                            MatchPathIsWildCard = true,
                            MatchPathHasNamespace = false,
                            BindingPathIsRelative = true,
                            BindingPathRootDepth = 0,
                            BindingPathHasNamespace = false,
                            ExpectedStringValue = "* => ~"
                        }
                    }
                };

            testConfig.TestVariationWithSameResults("~", "* => ~", "   ~ ", "*=>~", " *=> ~"," * =>~");

        }

        [TestMethod]
        public void ShouldParseNamespaceExpression()
        {
            
           var testConfig = new ParserMatchGroupExpectedValue
                {
                    ExpectedValues = new[]
                    {
                        new ParserMatchExpectedValue
                        {
                            MatchPathIsWildCard = true,
                            MatchPathHasNamespace = false,
                            BindingPathIsRelative = false,
                            BindingPathRootDepth = 0,
                            BindingPathHasNamespace = true,
                            BindingPathNamespace = "Foo.Bar",
                            ExpectedStringValue = "* => Foo.Bar"
                        }
                    }
                };

           testConfig.TestVariationWithSameResults("Foo.Bar", "* => Foo.Bar", "  *=>Foo.Bar");
        }
    }
}
#endif