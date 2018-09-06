using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Lotus.Core
{
    [Serializable]
    public class XResult<T>
    {
        private static readonly XResult<T> DefaultValue = new XResult<T>(default(T));

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public Object State { get; set; }

        /// <summary>
        /// Represents the operation is success, and the value of property "Value" will return the default(T).
        /// </summary>
        public XResult(T value)
        {
            this.Value = value;
            this.Exceptions = new Collection<Exception>();
        }

        /// <summary>
        /// Initialize the instance of Result with value and errors.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exceptions"></param>
        public XResult(T value, params Exception[] exceptions)
        {
            this.Value = (value != null ? value : default(T));
            this.Exceptions = new Collection<Exception>();

            if (exceptions != null && exceptions.Length > 0)
            {
                foreach (var ex in exceptions)
                {
                    this.Exceptions.Add(ex);
                }
            }
        }

        /// <summary>
        /// Gets the instance of Result&lt;T&gt; with default constructor。
        /// </summary>
        public static XResult<T> Default
        {
            get
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// Gets executions.
        /// </summary>
        public Collection<Exception> Exceptions { get; private set; }

        /// <summary>
        /// Indicates whether the execution result is successful or failed. True is successful, False is failed.
        /// </summary>
        public Boolean Success
        {
            get
            {
                return this.Exceptions.Count == 0;
            }
        }

        /// <summary>
        /// Gets the return value object.
        /// </summary>
        public T Value { get; private set; }
    }
}
