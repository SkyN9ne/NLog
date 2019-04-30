// 
// Copyright (c) 2004-2019 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace NLog.LayoutRenderers
{
    using System;
    using System.Text;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// Render a Global Diagnostics Context item. See <see cref="GlobalDiagnosticsContext"/>
    /// </summary>
    [LayoutRenderer("gdc")]
    [ThreadAgnostic]
    [ThreadSafe]
    public class GdcLayoutRenderer : LayoutRenderer, IRawValue, IStringValueRenderer
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        /// <docgen category='Rendering Options' order='50' />
        public string Format { get; set; }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            object value = GetValue();
            var formatProvider = GetFormatProvider(logEvent, null);
            builder.AppendFormattedValue(value, Format, formatProvider);
        }

        /// <inheritdoc/>
        bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
        {
            value = GetValue();
            return true;
        }

        /// <inheritdoc/>
        string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent) => GetStringValue(logEvent);

        private string GetStringValue(LogEventInfo logEvent)
        {
            if (Format != MessageTemplates.ValueFormatter.FormatAsJson)
            {
                object value = GetValue();
                string stringValue = FormatHelper.TryFormatToString(value, Format, GetFormatProvider(logEvent, null));
                return stringValue;
            }
            return null;
        }

        private object GetValue()
        {
            //don't use GlobalDiagnosticsContext.Get to ensure we are not locking the Factory (indirect by LogManager.Configuration).
            return GlobalDiagnosticsContext.GetObject(Item);
        }
    }
}
