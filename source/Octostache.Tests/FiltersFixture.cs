﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Octostache.Tests
{
    [TestFixture]
    public class FiltersFixture : BaseFixture
    {

        [Test]
        public void UnmatchedSubstitutionsAreEchoedEvenWithFiltering()
        {
            var result = Evaluate("#{foo | ToUpper}", new Dictionary<string, string>());
            Assert.AreEqual("#{foo | ToUpper}", result);
        }

        [Test]
        public void UnknownFiltersAreEchoed()
        {
            var result = Evaluate("#{Foo | ToBazooka}", new Dictionary<string, string> { { "Foo", "Abc" } });
            Assert.AreEqual("#{Foo | ToBazooka}", result);
        }

        [Test]
        public void UnknownFiltersWithOptionsAreEchoed()
        {
            var result = Evaluate("#{Foo | ToBazooka 6}", new Dictionary<string, string> { { "Foo", "Abc" } });
            Assert.AreEqual("#{Foo | ToBazooka 6}", result);
        }

        [Test]
        public void FiltersAreApplied()
        {
            var result = Evaluate("#{Foo | ToUpper}", new Dictionary<string, string> { { "Foo", "Abc" } });
            Assert.AreEqual("ABC", result);
        }

        [Test]
        public void HtmlIsEscaped()
        {
            var result = Evaluate("#{Foo | HtmlEscape}", new Dictionary<string, string> { { "Foo", "A&'bc" } });
            Assert.AreEqual("A&amp;&apos;bc", result);
        }

        [Test]
        public void XmlIsEscaped()
        {
            var result = Evaluate("#{Foo | XmlEscape}", new Dictionary<string, string> { { "Foo", "A&'bc" } });
            Assert.AreEqual("A&amp;&apos;bc", result);
        }

        [Test]
        public void JsonIsEscaped()
        {
            var result = Evaluate("#{Foo | JsonEscape}", new Dictionary<string, string> { { "Foo", "A&\"bc" } });
            Assert.AreEqual("A&\\\"bc", result);
        }

        [Test]
        public void MarkdownIsProcessed()
        {
            var result = Evaluate("#{Foo | Markdown}", new Dictionary<string, string> { { "Foo", "_yeah!_" } });
            Assert.AreEqual("<p><em>yeah!</em></p>\n", result);
        }

        [Test]
        public void DateIsFormatted()
        {
            var dict = new Dictionary<string, string> { { "Foo", "22/05/2030 09:05:00" } };

            var result = Evaluate("#{Foo | Format Date \"HH dd-MMM-yyy\"}", dict);
            Assert.AreEqual("09 22-May-2030", result);
        }

        [Test]
        public void DateFormattingCanUseInnerVariable()
        {
            var dict = new Dictionary<string, string> { { "Foo", "22/05/2030 09:05:00" }, { "Format", "HH dd-MMM-yyyy" } };

            var result = Evaluate("#{Foo | Format Date #{Format}}", dict);
            Assert.AreEqual("09 22-May-2030", result);
        }

        [Test]
        public void GenericConverterAcceptsDouble()
        {
            var dict = new Dictionary<string, string> { { "Cash", "23.4" }};

            var result = Evaluate("#{Cash | Format Double C}", dict);
            Assert.AreEqual("$23.40", result);
        }
        
        [Test]
        public void GenericConverterAcceptsDate()
        {
            var dict = new Dictionary<string, string> { { "MyDate", "22/05/2030 09:05:00" }, { "Format", "HH dd-MMM-yyyy" } };
            var result = Evaluate("#{MyDate | Format DateTime \"HH dd-MMM-yyyy\" }", dict);
            Assert.AreEqual("09 22-May-2030", result);
        }

        [Test]
        public void FormatFunctionDefaultAssumeDecimal()
        {
            var dict = new Dictionary<string, string> { { "Cash", "23.4" } };

            var result = Evaluate("#{Cash | Format C}", dict);
            Assert.AreEqual("$23.40", result);
        }

        [Test]
        public void FormatFunctionWillTryDefaultDateTimeIfNotDecimal()
        {
            var dict = new Dictionary<string, string> { { "Date", "22/05/2030 09:05:00" } };
            var result = Evaluate("#{ Date | Format yyyy}", dict);
            Assert.AreEqual("2030", result);
        }

        [Test]
        public void FormatFunctionWillReturnUnreplacedIfNoDefault()
        {
            var dict = new Dictionary<string, string> { { "Invalid", "hello World" } };

            var result = Evaluate("#{Invalid | Format yyyy}", dict);
            Assert.AreEqual("#{Invalid | Format yyyy}", result);
        }


        [Test]
        public void NowDateReturnsNow()
        {
            var result = Evaluate("#{ | NowDate}", new Dictionary<string, string> ());
            Assert.That(DateTime.Parse(result), Is.EqualTo(DateTime.Now).Within(1).Minutes);
        }


        [Test]
        public void NowDateCanBeFormatted()
        {
            var result = Evaluate("#{ | NowDate yyyy}", new Dictionary<string, string>());
            Assert.AreEqual(DateTime.Now.Year.ToString(), result);
        }


        [Test]
        public void NowDateCanBeChained()
        {
            var result = Evaluate("#{ | NowDate | Format Date MMM}", new Dictionary<string, string>());
            Assert.AreEqual(DateTime.Now.ToString("MMM"), result);
        }

        [Test]
        public void NowDateReturnsNowInUtc()
        {
            var result = Evaluate("#{ | NowDateUtc}", new Dictionary<string, string>());
            Assert.That(DateTimeOffset.Parse(result), Is.EqualTo(DateTimeOffset.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void NowDateUtcCanBeChained()
        {
            var result = Evaluate("#{ | NowDateUtc | Format DateTimeOffset zz}", new Dictionary<string, string>());
            Assert.AreEqual("+00", result);

            var result1 = Evaluate("#{ | NowDate | Format DateTimeOffset zz}", new Dictionary<string, string>());
            Assert.AreEqual(DateTimeOffset.Now.ToString("zz"), result1);
        }

        [Test]
        public void FiltersAreAppliedInOrder()
        {
            var result = Evaluate("#{Foo|ToUpper|ToLower}", new Dictionary<string, string> { { "Foo", "Abc" } });
            Assert.AreEqual("abc", result);
        }
    }
}