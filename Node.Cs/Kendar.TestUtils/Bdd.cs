using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kendar.TestUtils
{
    public class BddException : Exception
    {
        public string FilePath { get; private set; }
        public string MemberName { get; private set; }
        public int LineNumber { get; private set; }
        public BddException(string message, Exception ex, string filePath, string memberName, int lineNumber)
            : base(message, ex)
        {
            FilePath = filePath;
            MemberName = memberName;
            LineNumber = lineNumber;
        }

        public override string StackTrace
        {
            get
            {
                var st = base.StackTrace + "\r\n" +
                    string.Format("   in {0} in {1}:line {2}", MemberName, FilePath, LineNumber);
                return st;
            }
        }

    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public class ThenAnd
    {
        public ThenAnd And(Expression<Action> then,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            try
            {

                then.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("Then '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return this;
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public class WhenAnd
    {
        public ThenAnd Then(Expression<Action> then,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            var thenAnd = new ThenAnd();
            try
            {

                then.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("Then '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return thenAnd;
        }
        public WhenAnd And(Expression<Action> when,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                when.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("When '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return this;
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public class GivenAnd
    {
        public WhenAnd When(Expression<Action> when,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            var whenAnd = new WhenAnd();
            try
            {
                when.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("When '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return whenAnd;
        }
        public GivenAnd And(Expression<Action> given,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                given.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("Given '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return this;
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public class BDD
    {
        private BDD()
        {

        }

        public static GivenAnd Given(Expression<Action> given,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            var givenAnd = new GivenAnd();
            try
            {
                given.Compile()();
            }
            catch (Exception ex)
            {
                throw new BddException(string.Format("Given '{0}:{1}'", memberName, lineNumber), ex, filePath, memberName, lineNumber);
            }
            return givenAnd;
        }
    }
}
