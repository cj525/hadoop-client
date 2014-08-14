#region FreeBSD

// Copyright (c) 2014, The Tribe
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using HBase.Stargate.Client.TypeConversion;

using Newtonsoft.Json.Linq;

namespace HBase.Stargate.Client.Api
{
  /// <summary>
  ///   This filter is used to filter cells based on value.
  /// </summary>
  public class SingleColumnValueFilter : ComparisonScannerFilterBase
  {
    private const string _familyPropertyName = "family";
    private const string _qualifierPropertyName = "qualifier";
    private const string _latestVersionPropertyName = "latestVersion";
    private const string _ifMissingPropertyName = "ifMissing";
    private readonly string _family;
    private readonly bool _latestVersion;
    private readonly bool _ifMissing;
    private readonly string _qualifier;

    /// <summary>
    ///   Initializes a new instance of the <see cref="SingleColumnValueFilter" /> class.
    /// </summary>
    /// <param name="family">The family.</param>
    /// <param name="qualifier">The qualifier.</param>
    /// <param name="value">The column.</param>
    /// <param name="comparison">The comparison.</param>
    /// <param name="latestVersion">
    ///   if set to <c>true</c>, only return the latest version.
    /// </param>
    /// <param name="ifMissing">
    ///   if set to <c>true</c>, filter entire row if column is not found.
    /// </param>
    public SingleColumnValueFilter(string family, string qualifier, string value, FilterComparisons comparison, bool latestVersion = true, bool ifMissing = true)
      : base(value, comparison)
    {
      _family = family;
      _qualifier = qualifier;
      _latestVersion = latestVersion;
      _ifMissing = ifMissing;
    }

    /// <summary>
    ///   Converts the filter to its JSON representation.
    /// </summary>
    /// <param name="codec">The codec to use for encoding values.</param>
    public override JObject ConvertToJson(ICodec codec)
    {
      // NOTE: SingleColumnValueFilter is peculiar about the order of its JSON
      //       representation.  The easiest fix is to *append* the base class
      //       JSON representation to our own, Stargate seems happy with that.
      var json = new JObject();

      json[_latestVersionPropertyName] = new JValue(_latestVersion);
      json[_ifMissingPropertyName] = new JValue(_ifMissing);
      json[_qualifierPropertyName] = new JValue(codec.Encode(_qualifier));
      json[_familyPropertyName] = new JValue(codec.Encode(_family));

      // TODO: replace with .Merge() once we upgrade to a version with it.
      json.Add(base.ConvertToJson(codec).Children());

      return json;
    }
  }
}