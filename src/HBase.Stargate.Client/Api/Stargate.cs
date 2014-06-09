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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using HBase.Stargate.Client.Models;
using HBase.Stargate.Client.TypeConversion;

using RestSharp;
using RestSharp.Injection;

namespace HBase.Stargate.Client.Api
{
  /// <summary>
  ///   Provides a default implementation <see cref="IStargate" />.
  /// </summary>
  public class Stargate : IStargate
  {
    /// <summary>
    ///   The default false row key
    /// </summary>
    public const string DefaultFalseRowKey = "row";

    /// <summary>
    ///   The default content type
    /// </summary>
    public const string DefaultContentType = HBaseMimeTypes.Xml;

    protected readonly IRestClient Client;
    protected readonly IMimeConverter Converter;
    protected readonly IErrorProvider ErrorProvider;
    protected readonly IResourceBuilder ResourceBuilder;
    protected readonly IRestSharpFactory RestSharp;
    protected readonly IScannerOptionsConverter ScannerConverter;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Stargate" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="resourceBuilderFactory">The resource builder factory.</param>
    /// <param name="restSharp">The RestSharp factory.</param>
    /// <param name="converterFactory">The converter factory.</param>
    /// <param name="errorProvider">The error provider.</param>
    /// <param name="scannerConverter">The scanner converter.</param>
    public Stargate(IStargateOptions options, Func<IStargateOptions, IResourceBuilder> resourceBuilderFactory, IRestSharpFactory restSharp,
      IMimeConverterFactory converterFactory, IErrorProvider errorProvider, IScannerOptionsConverter scannerConverter)
    {
      ResourceBuilder = resourceBuilderFactory(options);
      RestSharp = restSharp;
      ErrorProvider = errorProvider;
      ScannerConverter = scannerConverter;
      Client = RestSharp.CreateClient(options.ServerUrl);
      options.ContentType = string.IsNullOrEmpty(options.ContentType) ? DefaultContentType : options.ContentType;
      Converter = converterFactory.CreateConverter(options.ContentType);
      options.FalseRowKey = string.IsNullOrEmpty(options.FalseRowKey) ? DefaultFalseRowKey : options.FalseRowKey;
      Options = options;
    }

    /// <summary>
    ///   Gets the options.
    /// </summary>
    /// <value>
    ///   The options.
    /// </value>
    protected virtual IStargateOptions Options { get; private set; }

    /// <summary>
    ///   Writes the value to HBase using the identifier.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="value">The value.</param>
    [Obsolete("Use Task.Run(() => gate.WriteValue(identifier,value)) instead")]
    public virtual Task WriteValueAsync(Identifier identifier, string value)
    {
      return Task.Run(() => WriteValue(identifier, value));
    }

    /// <summary>
    ///   Writes the value to HBase using the identifier.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="value">The value.</param>
    public virtual void WriteValue(Identifier identifier, string value)
    {
      string contentType = Options.ContentType;
      string resource = ResourceBuilder.BuildSingleValueAccess(identifier);
      string content = Converter.ConvertCell(new Cell(identifier, value));
      IRestResponse response = SendRequest(Method.POST, resource, contentType, contentType, content);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Writes the cells to HBase.
    /// </summary>
    /// <param name="cells">The cells.</param>
    [Obsolete("Use Task.Run(() => gate.WriteCells(cells)) instead")]
    public virtual Task WriteCellsAsync(CellSet cells)
    {
      return Task.Run(() => WriteCells(cells));
    }

    /// <summary>
    ///   Writes the cells to HBase.
    /// </summary>
    /// <param name="cells">The cells.</param>
    public virtual void WriteCells(CellSet cells)
    {
      string contentType = Options.ContentType;
      var tableIdentifier = new Identifier {Table = cells.Table};
      string resource = ResourceBuilder.BuildBatchInsert(tableIdentifier);
      IRestResponse response = SendRequest(Method.POST, resource, contentType, contentType, Converter.ConvertCells(cells));
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Deletes the item with the matching identifier from HBase.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    [Obsolete("Use Task.Run(() => gate.DeleteItem(identifier)) instead")]
    public virtual Task DeleteItemAsync(Identifier identifier)
    {
      return Task.Run(() => DeleteItem(identifier));
    }

    /// <summary>
    ///   Deletes the item with the matching identifier from HBase.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    public virtual void DeleteItem(Identifier identifier)
    {
      string resource = ResourceBuilder.BuildDeleteItem(identifier);
      IRestResponse response = SendRequest(Method.DELETE, resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Reads the value with the matching identifier.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    [Obsolete("Use Task.Run(() => gate.ReadValue(identifier)) instead")]
    public virtual Task<string> ReadValueAsync(Identifier identifier)
    {
      return Task.Run(() => ReadValue(identifier));
    }

    /// <summary>
    ///   Reads the value with the matching identifier.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    public virtual string ReadValue(Identifier identifier)
    {
      string resource = identifier.Timestamp.HasValue
        ? ResourceBuilder.BuildCellOrRowQuery(identifier.ToQuery())
        : ResourceBuilder.BuildSingleValueAccess(identifier, true);

      IRestResponse response = SendRequest(Method.GET, resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK, HttpStatusCode.NotFound);

      return response.StatusCode == HttpStatusCode.OK
        ? Converter.ConvertCells(response.Content, identifier.Table).Select(cell => cell.Value).FirstOrDefault()
        : null;
    }

    /// <summary>
    ///   Finds the cells matching the query.
    /// </summary>
    /// <param name="query"></param>
    [Obsolete("Use Task.Run(() => gate.FindCells(query)) instead")]
    public virtual Task<CellSet> FindCellsAsync(CellQuery query)
    {
      return Task.Run(() => FindCells(query));
    }

    /// <summary>
    ///   Finds the cells matching the query.
    /// </summary>
    /// <param name="query"></param>
    public virtual CellSet FindCells(CellQuery query)
    {
      string resource = ResourceBuilder.BuildCellOrRowQuery(query);
      IRestResponse response = SendRequest(Method.GET, resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK, HttpStatusCode.NotFound);

      var set = new CellSet
      {
        Table = query.Table
      };

      if (response.StatusCode == HttpStatusCode.OK)
      {
        set.AddRange(Converter.ConvertCells(response.Content, query.Table));
      }

      return set;
    }

    /// <summary>
    ///   Creates the table.
    /// </summary>
    /// <param name="tableSchema">The table schema.</param>
    public virtual void CreateTable(TableSchema tableSchema)
    {
      string resource = ResourceBuilder.BuildTableSchemaAccess(tableSchema);
      ErrorProvider.ThrowIfSchemaInvalid(tableSchema);
      string data = Converter.ConvertSchema(tableSchema);
      IRestResponse response = SendRequest(Method.PUT, resource, Options.ContentType, Options.ContentType, data);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Creates the table.
    /// </summary>
    /// <param name="tableSchema">The table schema.</param>
    [Obsolete("Use Task.Run(() => gate.CreateTable(tableSchema)) instead")]
    public virtual Task CreateTableAsync(TableSchema tableSchema)
    {
      return Task.Run(() => CreateTable(tableSchema));
    }

    /// <summary>
    ///   Gets the table names.
    /// </summary>
    public virtual IEnumerable<string> GetTableNames()
    {
      IRestResponse response = SendRequest(Method.GET, string.Empty, HBaseMimeTypes.Text);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
      return ParseLines(response.Content);
    }

    /// <summary>
    ///   Gets the table names.
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use Task.Run(() => gate.GetTableNames()) instead")]
    public virtual Task<IEnumerable<string>> GetTableNamesAsync()
    {
      return Task.Run(() => GetTableNames());
    }

    /// <summary>
    ///   Deletes the table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    public virtual void DeleteTable(string tableName)
    {
      string resource = ResourceBuilder.BuildTableSchemaAccess(new TableSchema {Name = tableName});
      IRestResponse response = SendRequest(Method.DELETE, resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Deletes the table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    [Obsolete("Use Task.Run(() => gate.DeleteTable(tableName)) instead")]
    public virtual Task DeleteTableAsync(string tableName)
    {
      return Task.Run(() => DeleteTable(tableName));
    }

    /// <summary>
    ///   Gets the table schema async.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    [Obsolete("Use Task.Run(() => gate.GetTableSchema(tableName)) instead")]
    public virtual Task<TableSchema> GetTableSchemaAsync(string tableName)
    {
      return Task.Run(() => GetTableSchema(tableName));
    }

    /// <summary>
    ///   Gets the table schema.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    public virtual TableSchema GetTableSchema(string tableName)
    {
      string resource = ResourceBuilder.BuildTableSchemaAccess(new TableSchema {Name = tableName});
      IRestResponse response = SendRequest(Method.GET, resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
      return Converter.ConvertSchema(response.Content);
    }

    /// <summary>
    ///   Creates the scanner.
    /// </summary>
    /// <param name="options">The options.</param>
    [Obsolete("Use Task.Run(() => gate.CreateScanner(options)) instead")]
    public virtual Task<IScanner> CreateScannerAsync(ScannerOptions options)
    {
      return Task.Run(() => CreateScanner(options));
    }

    /// <summary>
    ///   Creates the scanner.
    /// </summary>
    /// <param name="options">The options.</param>
    public virtual IScanner CreateScanner(ScannerOptions options)
    {
      string resource = ResourceBuilder.BuildScannerCreate(options);
      IRestResponse response = SendRequest(Method.PUT, resource, HBaseMimeTypes.Xml, content: ScannerConverter.Convert(options));
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.Created);
      string scannerLocation =
        response.Headers.Where(header => header.Type == ParameterType.HttpHeader && header.Name == RestConstants.LocationHeader)
          .Select(header => header.Value.ToString())
          .FirstOrDefault();

      return string.IsNullOrEmpty(scannerLocation) ? null : new Scanner(options.TableName, new Uri(scannerLocation).PathAndQuery.Trim('/'), this);
    }

    /// <summary>
    ///   Deletes the scanner.
    /// </summary>
    /// <param name="scanner">The scanner.</param>
    public virtual void DeleteScanner(IScanner scanner)
    {
      IRestResponse response = SendRequest(Method.DELETE, scanner.Resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK);
    }

    /// <summary>
    ///   Deletes the scanner.
    /// </summary>
    /// <param name="scanner">The scanner.</param>
    [Obsolete("Use Task.Run(() => gate.DeleteScanner(scanner)) instead")]
    public virtual Task DeleteScannerAsync(IScanner scanner)
    {
      return Task.Run(() => DeleteScanner(scanner));
    }

    /// <summary>
    ///   Gets the scanner result.
    /// </summary>
    /// <param name="scanner">The scanner.</param>
    public virtual CellSet GetScannerResult(IScanner scanner)
    {
      IRestResponse response = SendRequest(Method.GET, scanner.Resource, Options.ContentType);
      ErrorProvider.ThrowIfStatusMismatch(response, HttpStatusCode.OK, HttpStatusCode.NoContent);
      return response.StatusCode == HttpStatusCode.NoContent ? null : new CellSet(Converter.ConvertCells(response.Content, scanner.Table));
    }

    /// <summary>
    ///   Gets the scanner result.
    /// </summary>
    /// <param name="scanner">The scanner.</param>
    [Obsolete("Use Task.Run(() => gate.GetScannerResult(scanner)) instead")]
    public virtual Task<CellSet> GetScannerResultAsync(IScanner scanner)
    {
      return Task.Run(() => GetScannerResult(scanner));
    }

    /// <summary>
    ///   Creates a new stargate with the specified options.
    /// </summary>
    /// <param name="serverUrl">The server URL.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="falseRowKey">The false row key.</param>
    public static IStargate Create(string serverUrl, string contentType = DefaultContentType, string falseRowKey = DefaultFalseRowKey)
    {
      return Create(new StargateOptions {ServerUrl = serverUrl, ContentType = contentType, FalseRowKey = falseRowKey});
    }

    /// <summary>
    ///   Creates a new stargate with the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    public static IStargate Create(IStargateOptions options)
    {
      Func<IStargateOptions, IResourceBuilder> resourceBuilderFactory = opt => new ResourceBuilder(opt);
      var restSharp = new RestSharpFactory(url => new RestClient(url), (resource, method) => new RestRequest(resource, method));
      var codec = new Base64Codec();
      var mimeConverters = new MimeConverterFactory(new[]
      {
        new XmlMimeConverter(new SimpleValueConverter(), codec)
      });
      var errors = new ErrorProvider();
      var scannerConverter = new ScannerOptionsConverter(codec);

      options.ContentType = string.IsNullOrEmpty(options.ContentType)
        ? DefaultContentType
        : options.ContentType;

      options.FalseRowKey = string.IsNullOrEmpty(options.FalseRowKey)
        ? DefaultFalseRowKey
        : options.FalseRowKey;

      return new Stargate(options, resourceBuilderFactory, restSharp, mimeConverters, errors, scannerConverter);
    }

    /// <summary>
    ///   Sends the request.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="acceptType">Type of the accept.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="content">The content.</param>
    [Obsolete("Use Task.Run(() => SendRequest(method,resource,acceptType,contentType,content)) instead")]
    protected virtual Task<IRestResponse> SendRequestAsync(Method method, string resource, string acceptType,
      string contentType = null, string content = null)
    {
      return Task.Run(() => SendRequest(method, resource, acceptType, contentType, content));
    }

    /// <summary>
    /// Gets the validated response.
    /// </summary>
    /// <param name="response">The response.</param>
    protected static IRestResponse GetValidatedResponse(IRestResponse response)
    {
      if (response.ResponseStatus == ResponseStatus.Error && response.ErrorException != null)
      {
        throw response.ErrorException;
      }

      return response;
    }

    /// <summary>
    ///   Sends the request.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="acceptType">Type of the accept.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="content">The content.</param>
    protected virtual IRestResponse SendRequest(Method method, string resource, string acceptType,
      string contentType = null, string content = null)
    {
      IRestRequest request = BuildRequest(method, resource, acceptType, contentType, content);

      IRestResponse response = Client.Execute(request);

      return GetValidatedResponse(response);
    }

    /// <summary>
    /// Builds the request.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="acceptType">Type of the accept.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="content">The content.</param>
    protected IRestRequest BuildRequest(Method method, string resource, string acceptType, string contentType, string content)
    {
      IRestRequest request = RestSharp.CreateRequest(resource, method)
        .AddHeader(HttpRequestHeader.Accept.ToString(), acceptType);

      if (!string.IsNullOrEmpty(content))
      {
        contentType = string.IsNullOrEmpty(contentType) ? acceptType : contentType;
        request.AddParameter(contentType, content, ParameterType.RequestBody);
      }
      return request;
    }

    //TODO: get rid of this (it's been written in 5 different places, so maybe it's time for code-patterns)
    private static IEnumerable<string> ParseLines(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        yield break;
      }

      using (var reader = new StringReader(text))
      {
        string line;
        while ((line = reader.ReadLine()) != null) yield return line;
      }
    }
  }
}